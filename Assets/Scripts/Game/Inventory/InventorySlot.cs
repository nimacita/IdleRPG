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

    //сохраненное значения предмета в слоте по его индексу и названию предмета
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

    //сохраненное значение количество текущего предмета в инвенторе
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

    //обновляем айтем в слоте, если есть
    public void UpdateItemInSlot()
    {
        if (slotIndex != -1) 
        {
            if (InventorySlotItem != "")
            {
                //берем предмет по сохраненному имени
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
            //обновляем отображение
            UpdateSlotView();
        }
        else
        {
            Debug.LogError("Не выставлен индекс");
        }
    }

    //обновляем отображение слота, в зависимости от предмета
    private void UpdateSlotView()
    {
        if (currentItem != null)
        {
            //слот занят
            isFree = false;
            item.gameObject.SetActive(false);
            armorItem.gameObject.SetActive(false);

            //показываем слот
            if (!IsWeapon(currentItem) && !IsArmor(currentItem))
            {
                //если не оружие и не броня
                //показываем спрайт
                item.sprite = currentItem.itemIcon;
                item.gameObject.SetActive(true);

                //обновляем текст количества
                itemCount.gameObject.SetActive(true);
                itemCount.text = $"{InventorySlotItemCount}";
            }
            else
            {
                //если оружие или броня
                item.sprite = currentItem.itemIcon;
                item.gameObject.SetActive(true);

                //выключаем текст
                itemCount.gameObject.SetActive(false);
            }
        }
        else
        {
            //слот не занят
            isFree = true;
            itemCount.gameObject.SetActive(false);
            item.gameObject.SetActive(false);
            armorItem.gameObject.SetActive(false);
        }
    }

    //ставим предмет, если null - То снимаем предмет
    public void SetItem(MyInventoryItem itm = null, int itemCount = 1)
    {
        if (itm != null)
        {
            //ставим предмет если возможно
            if (isFree && currentItem == null)
            {
                currentItem = itm;
                InventorySlotItem = itm.itemName;
                if (!IsWeapon(itm) && !IsArmor(itm))
                {
                    //если не оружия и не броня то ставим выбранное количество
                    InventorySlotItemCount = itemCount;
                }
                else
                {
                    //если либо оружие либо броня то ставим еденицу
                    InventorySlotItemCount = 1;
                }
            }
            else
            {
                Debug.LogError($"Слот {slotIndex} занят, поставиь предмет невозможно");
            }
        }
        else
        {
            //снимаем предмет
            InventorySlotItem = "";
            InventorySlotItemCount = 0;
            currentItem = null;
        }
        //обновляем отображение
        UpdateItemInSlot();
    }

    //добавляем предметов этого же типа
    public void AddItems(int counts)
    {
        if (currentItem != null && !isFree)
        {
            InventorySlotItemCount += counts;
            //обновляем отображение
            UpdateItemInSlot();
        }
        else
        {
            Debug.LogError("Невозможно добавить, так как ничего нет");
        }
    }

    //берем айтем из списка всех по имени
    private MyInventoryItem TakeItem(string name)
    {
        foreach (MyInventoryItem itm in allItems.AllInvenoryItems)
        {
            if (itm.itemName == name)
            {
                return itm;
            }
        }
        Debug.LogError($"Предмета {name} не найдено в списке предметов");
        return null;
    }

    //если схватили предмет, то передаем
    public void IsItemDrag()
    {
        //если есть что брать
        if (!isFree && currentItem != null)
        {
            InventoryController.instance.SelectItem(slotIndex);
            DisableItem();
        }
    }

    //выключаем предмет визуально
    public void DisableItem()
    {
        itemCount.gameObject.SetActive(false);
        item.gameObject.SetActive(false);
        armorItem.gameObject.SetActive(false);
    }

    //включаем предмет
    public void EnableItem()
    {
        UpdateSlotView();
    }

    //нажали на слот
    public void ClickOnSlot()
    {
        if (currentItem != null)
        {
            //если слот экипирован зельем, то включаем панель хила
            if (currentItem.itemType == InventoryItemType.HealthPotion)
            {
                InventoryController.instance.OpenHealthPanel(currentItem, InventorySlotItemCount, slotIndex);
            }
        }
    }

    //является ли броней текущий предмет
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

    //является ли оружием текущий слот
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
