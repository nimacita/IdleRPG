using Assets.HeroEditor.Common.Scripts.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("Index")]
    [SerializeField] private int slotIndex = -1;

    [Header("AllItems")]
    private AllInventoryItems allItems;

    [Header("Components")]
    [SerializeField] private Image armorItem;
    [SerializeField] private Image item;
    [SerializeField] private TMPro.TMP_Text itemCount;

    [Header("Debug")]
    //Current Item
    [SerializeField] private MyInventoryItem currentItem = null;

    //bools
    private bool isFree = false;
    [SerializeField]
    private bool isSelected = false;

    void Start()
    {

    }

    //����������� �������� �������� � ����� �� ��� ������� � �������� ��������
    private string InventorySlotItem
    {
        get
        {
            if (!PlayerPrefs.HasKey($"InventorySlotItem{slotIndex}"))
            {
                PlayerPrefs.SetString($"InventorySlotItem{slotIndex}", "");
            }
            return PlayerPrefs.GetString($"InventorySlotItem{slotIndex}");
        }
        set
        {
            PlayerPrefs.SetString($"InventorySlotItem{slotIndex}", value);
        }
    }

    //����������� �������� ���������� �������� �������� � ���������
    private int InventorySlotItemCount
    {
        get
        {
            if (!PlayerPrefs.HasKey($"InventorySlotItemCount{slotIndex}"))
            {
                PlayerPrefs.SetInt($"InventorySlotItemCount{slotIndex}", 0);
            }
            return PlayerPrefs.GetInt($"InventorySlotItemCount{slotIndex}");
        }
        set
        {
            PlayerPrefs.SetInt($"InventorySlotItemCount{slotIndex}", value);
        }
    }

    //��������� ����� � �����, ���� ����
    public void UpdateItemInSlot()
    {
        if (slotIndex != -1) 
        {
            if (InventorySlotItem != "")
            {
                //����� ������� �� ������������ �����
                currentItem = TakeItem(InventorySlotItem);
                if(currentItem == null)
                {
                    InventorySlotItem = "";
                    InventorySlotItemCount = 0;
                }
            }
            else
            {
                currentItem = null;
            }
            //��������� �����������
            UpdateSlotView();
        }
        else
        {
            Debug.LogError("�� ��������� ������");
        }
    }

    //��������� ����������� �����, � ����������� �� ��������
    private void UpdateSlotView()
    {
        if (currentItem != null)
        {
            //���� �����
            isFree = false;
            item.gameObject.SetActive(false);
            armorItem.gameObject.SetActive(false);

            //���������� ����
            if (!IsWeapon(currentItem) && !IsArmor(currentItem))
            {
                //���� �� ������ � �� �����
                //���������� ������
                item.sprite = currentItem.itemIcon;
                item.gameObject.SetActive(true);

                //��������� ����� ����������
                itemCount.gameObject.SetActive(true);
                itemCount.text = $"{InventorySlotItemCount}";
            }
            else
            {
                //���� ������ ��� �����
                item.sprite = currentItem.itemIcon;
                item.gameObject.SetActive(true);

                //��������� �����
                itemCount.gameObject.SetActive(false);
            }
        }
        else
        {
            //���� �� �����
            isFree = true;
            itemCount.gameObject.SetActive(false);
            item.gameObject.SetActive(false);
            armorItem.gameObject.SetActive(false);
        }
    }

    //������ �������, ���� null - �� ������� �������
    public void SetItem(MyInventoryItem itm = null, int itemCount = 1)
    {
        if (itm != null)
        {
            //������ ������� ���� ��������
            if (isFree && currentItem == null)
            {
                currentItem = itm;
                InventorySlotItem = itm.itemName;
                if (!IsWeapon(itm) && !IsArmor(itm))
                {
                    //���� �� ������ � �� ����� �� ������ ��������� ����������
                    InventorySlotItemCount = itemCount;
                }
                else
                {
                    //���� ���� ������ ���� ����� �� ������ �������
                    InventorySlotItemCount = 1;
                }
            }
            else
            {
                Debug.LogError($"���� {slotIndex} �����, �������� ������� ����������");
            }
        }
        else
        {
            //������� �������
            InventorySlotItem = "";
            InventorySlotItemCount = 0;
            currentItem = null;
        }
        //��������� �����������
        UpdateItemInSlot();
    }

    //��������� ��������� ����� �� ����
    public void AddItems(int counts)
    {
        if (currentItem != null && !isFree)
        {
            InventorySlotItemCount += counts;
            //��������� �����������
            UpdateItemInSlot();
        }
        else
        {
            Debug.LogError("���������� ��������, ��� ��� ������ ���");
        }
    }

    //����� ����� �� ������ ���� �� �����
    private MyInventoryItem TakeItem(string name)
    {
        foreach (MyInventoryItem itm in allItems.AllInvenoryItems)
        {
            if (itm.itemName == name)
            {
                return itm;
            }
        }
        Debug.LogError($"�������� {name} �� ������� � ������ ���������");
        return null;
    }

    //���� �������� �������, �� ��������
    public void IsItemDrag()
    {
        //���� ���� ��� �����
        if (!isFree && currentItem != null)
        {
            InventoryController.instance.SelectItem(slotIndex);
            DisableItem();
        }
    }

    //��������� ������� ���������
    public void DisableItem()
    {
        itemCount.gameObject.SetActive(false);
        item.gameObject.SetActive(false);
        armorItem.gameObject.SetActive(false);
    }

    //�������� �������
    public void EnableItem()
    {
        UpdateSlotView();
    }

    //������ �� ����
    public void ClickOnSlot()
    {
        if (currentItem != null)
        {
            //���� ���� ���������� ������, �� �������� ������ ����
            if (currentItem.itemType == InventoryItemType.HealthPotion)
            {
                InventoryController.instance.OpenHealthPanel(currentItem, InventorySlotItemCount, slotIndex);
            }
        }
    }

    //�������� �� ������ ������� �������
    private bool IsArmor(MyInventoryItem itm)
    {
        switch (itm.itemType)
        {
            case InventoryItemType.Helmet:
            case InventoryItemType.Torso:
            case InventoryItemType.Gloves:
            case InventoryItemType.Boots:
                return true;
            default: 
                return false;
        }
    }

    //�������� �� ������� ������� ����
    private bool IsWeapon(MyInventoryItem itm)
    {
        switch (itm.itemType)
        {
            case InventoryItemType.SwordWeapon:
            case InventoryItemType.BowWeapon:
                return true;
            default:
                return false;
        }
    }

    public void SetIndex(int ind)
    {
        slotIndex = ind;
    }

    public void SetAllItems(AllInventoryItems allInventoryItems)
    {
        allItems = allInventoryItems;
    }

    public void SelectItem()
    {
        isSelected = true;
    }

    public void DeSelectItem()
    {
        isSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Input.touchSupported)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                SelectItem();
            }
            else
            {
                DeSelectItem();
            }
        }
        else
        {
            DeSelectItem();
        }
    }

    public bool IsFree
    {
        get { return isFree; }
    }

    public bool IsSelected
    {
        get { return isSelected; }
    }

    public MyInventoryItem GetCurrItem()
    {
        return currentItem;
    }

    public int GetCurrCount()
    {
        return InventorySlotItemCount;
    }

    public void SetCurrCount(int count)
    {
        InventorySlotItemCount = count;
        UpdateSlotView();
    }

}
