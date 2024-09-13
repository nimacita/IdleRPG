using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{

    [Header("View Settings")]
    [SerializeField] private GameObject inventoryView;
    [SerializeField] private Button closeBtn;

    [Header("Slots View")]
    [SerializeField] private GameObject slotsPanel;
    [SerializeField] private List<InventorySlot> slotsList;

    [Header("Qeuiped View")]
    [SerializeField] private GameObject equipedPanel;
    [SerializeField] private List<EquipedItem> equipedList;

    [Header("Moved In Hand Item")]
    [SerializeField] private GameObject moveItem;
    [SerializeField] private GameObject movedItemBg;
    [SerializeField] private Image movedItemSprite;
    [SerializeField] private TMPro.TMP_Text movedItemCount;

    [Header("Coins")]
    [SerializeField] private TMPro.TMP_Text coinsTxt;
    [SerializeField] private CoinPanel coinPanel;

    [Header("Health Panel")]
    [SerializeField] private GameObject healthPanel;
    [SerializeField] private HealthPanelController healthController;

    [Header("Sell Panel")]
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private SellPanelController sellPanelController;

    [Header("All Slots")]
    [SerializeField] AllInventoryItems allInventoryItems;

    [Header("Player Current Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;

    //bools
    private bool isInventory = false;
    private bool isTakeItem = false;
    private bool canTake = true;

    //selectedInd
    private int currSelectedInd = -1;
    private int currEquipedSelectedInd = -1;
    private int itemSelectedToSell = -1;
    private int equipSelectedToSell = -1;
    private int itemSelectedToHealth = -1;

    //stats
    private int swordDamage;
    private int bowDamage;
    private int health;
    private int armor;
    private int luck;
    private float readyTime;
    private float swordAttackTime;
    private float bowAttackTime;

    public static InventoryController instance;

    private void Awake()
    {
        instance = this;
        SetAllItemList();
    }

    void Start()
    {
        inventoryView.SetActive(false);
        sellPanel.SetActive(false);
        healthPanel.SetActive(false);

        //������������ ��� ��������
        InitializedSlotsItems();
        InitializedEquipedItems();

        //��������� �����
        AddStatsNull();
        SetAllEquipedStats();

        //��������� ������
        ButtonSettings();

        //������������� �� ��������� ��������
        SetDefault();
    }

    //��������� ������
    private void ButtonSettings()
    {
        closeBtn.onClick.AddListener(InventoryClose);
    }

    //�������� ������ � ������� �� ������
    private void AddStatsNull()
    {
        currentStats.addEquipSwordDamage = 0;
        currentStats.addEquipBowDamage = 0;
        currentStats.addEquipHealth = 0;
        currentStats.addEquipArmor = 0;
        currentStats.addEquipLuck = 0;
        currentStats.addEquipReadyTime = 0;
        currentStats.addEquipSwordAttackTime = 0;
        currentStats.addEquipBowAttackTime = 0;
    }

    //������������� ������������ �������������� �� ������������� ������
    public void SetAllEquipedStats()
    {
        //�������� ������� �������� �� ����������
        PlayerController.instance.AddEquipeStats(-1);

        //��������
        swordDamage = 0;
        bowDamage = 0;
        health = 0;
        armor = 0;
        luck = 0;
        readyTime = 0f;
        swordAttackTime = 0f;
        bowAttackTime = 0f;

        //���������� �� ���� ���������� � ���������� ��� �����
        foreach (EquipedItem eq in equipedList)
        {
            swordDamage += eq.SwordDamage;
            bowDamage += eq.BowDamage;
            health += eq.Health;
            armor += eq.Armor;
            luck += eq.Luck;
            readyTime += eq.ReadyAttack;
            swordAttackTime += eq.SwordAttackDurability;
            bowAttackTime += eq.BowAttackDurability;
        }

        //��������� �������������
        currentStats.addEquipSwordDamage = swordDamage;
        currentStats.addEquipBowDamage = bowDamage;
        currentStats.addEquipHealth = health;
        currentStats.addEquipArmor = armor;
        currentStats.addEquipLuck = luck;
        currentStats.addEquipReadyTime = readyTime;
        currentStats.addEquipSwordAttackTime = swordAttackTime;
        currentStats.addEquipBowAttackTime = bowAttackTime;

        //��������� ������������ � ������
        PlayerController.instance.AddEquipeStats();

        //��������� ����������� � ���������
        StatsPanel.instance.UpdateStatsBtns();
    }

    private void Update()
    {
        //������� ��������� �������
        MoveTakedItem();
        UpdateClickUp();
    }

    //�������� ��������� ������� � �����
    public void SetToSlots(MyInventoryItem setItem, int itemCounts = 1)
    {
        //���� ��������� ������� �� ����� null
        if (setItem != null)
        {
            //���� �� ����� � �� ������
            if (!IsArmor(setItem) && !IsWeapon(setItem))
            {
                //�������, ���� �� ��� ����� ������� � ���������, ���� ����, �� ���������
                foreach (InventorySlot slot in slotsList)
                {
                    if (setItem == slot.GetCurrItem())
                    {
                        //���������
                        slot.AddItems(itemCounts);
                        return;
                    }
                }
            }
            //���� ������ �������� ���, ��� ����� ��� ������, �� ��������� � ������ ���������
            if (IsFreeSlots())
            {
                //���� ���� ������
                foreach (InventorySlot slot in slotsList)
                {
                    //���������� �� ���� � ��������� � ������ ���������
                    if (slot.IsFree)
                    {
                        slot.SetItem(setItem, itemCounts);
                        return;
                    }
                }
            }
            else
            {
                //��� ������ ������ - �� ������, ������ �������� ������� ���������
                GameController.instance.PlayFullInvenoty();
                //Debug.LogError("��� ������ ������");
            }
        }
    }

    //������� ��� ��������
    private void RemoveAllItems()
    {
        foreach (InventorySlot slot in slotsList)
        {
            RemoveCurrentItem(slot);
        }
        foreach (EquipedItem equip in equipedList)
        {
            RemoveCurrentEquiped(equip);
        }
    }

    //������� ��������� �������
    public void RemoveCurrentItem(InventorySlot slot)
    {
        slot.SetItem(null);
    }

    //������� ��������� ����������
    public void RemoveCurrentEquiped(EquipedItem equip)
    {
        equip.SetItem(null);
    }

    //��������� ������ �� ������ ����
    private void UpdateClickUp()
    {
        if (Input.GetMouseButtonUp(0) && isInventory)
        {
            //��������� ������� ���� � ����
            if (isTakeItem)
            {
                //��������� ������� ��������
                UpdateMoveItem(false);
                //��������� �� �������
                IsPutItem();

                isTakeItem = false;
            }
        }
    }

    //����������� ������ �������
    private void MoveTakedItem()
    {
        if (isTakeItem && isInventory)
        {
            //Vector3 moveVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 moveVector = Input.mousePosition;
            moveVector.z = 0;
            moveItem.transform.position = moveVector;
        }
    }

    //��������� ����������� ����������� ��������
    private void UpdateMoveItem(bool value)
    {
        moveItem.SetActive(value);
    }

    //��������� �� �������
    private void IsPutItem()
    {
        //������� ������ �� ������� (���� �� ���������� ����)
        int selectedInd = IsSelectedSlot();

        //���� �� ���������� ���� ����� ����������
        int equipSelectedInd = IsSelectedEquiped();

        //���� �� ���������� ������ �������
        bool cellSelected = IsSelectedSellBtn();
        
        //������� ���������� ��� ������ � �������� ���������
        MyInventoryItem currentSelectedItem = null;
        int currentCount = 0;
        InventorySlot currentSlot = null;
        EquipedItem currentEquiped = null;

        //�������� ������� ��������� �������, ���� �� ����������, ���� �� ������
        if (currSelectedInd != -1)
        {
            //���� �� ������
            currentSlot = slotsList[currSelectedInd];
            currentSelectedItem = currentSlot.GetCurrItem();
            currentCount = currentSlot.GetCurrCount();
        }
        else if(currEquipedSelectedInd != -1)
        {
            //���� �� ����������
            currentEquiped = equipedList[currEquipedSelectedInd];
            currentSelectedItem = currentEquiped.GetCurrItem();
            currentCount = 1;
        }
        else
        {
            //���� ��������� ���, �� �������
            return;
        }

        bool isPut;
        //���� ���������� ���� � �� �� ����� ���� �� �������� �����
        if (selectedInd != -1 && selectedInd != currSelectedInd) isPut = true;
        else isPut = false;

        //���� ���� ���������� ����
        if (isPut)
        {
            //���� ���������� ���������
            if (slotsList[selectedInd].IsFree)
            {
                //������ �� ������ ������ �������
                slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                //�������� ������ �������
                if (currentSlot != null) currentSlot.SetItem();
                else if (currentEquiped != null) currentEquiped.SetItem();
            }
            else
            {
                //���� ���������� �����, �� ������ �������
                MyInventoryItem tempItem = slotsList[selectedInd].GetCurrItem();
                int tempCount = slotsList[selectedInd].GetCurrCount();

                //��������
                //if (currentSlot != null) 
                //������ �� ������
                if (currentSlot != null) 
                {
                    currentSlot.SetItem();
                    currentSlot.SetItem(tempItem, tempCount);

                    //������ �� ���������
                    slotsList[selectedInd].SetItem();
                    slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                }
                else if (currentEquiped != null)
                {
                    //���� ����� ��������� �� ����������
                    if (currentEquiped.IsCorrectItem(tempItem))
                    {
                        currentEquiped.SetItem();
                        currentEquiped.SetItem(tempItem);

                        //������ �� ���������
                        slotsList[selectedInd].SetItem();
                        slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                    }
                    else
                    {
                        //���� �� ����� ��������, ��� ��� ������ �� �� ������ �������, �� �������� �����
                        EnItemPut();
                        return;
                    }
                }

            }
        }
        //���� ���� ���������� ����������
        else if (equipSelectedInd != -1 && equipSelectedInd != currEquipedSelectedInd)
        {
            //��������� ����� �� ��������� ������� ���� �� ���������� ����������
            if (equipedList[equipSelectedInd].IsCorrectItem(currentSelectedItem))
            {
                if (equipedList[equipSelectedInd].IsFree)
                {
                    //���� ������, �� ������ �� ����
                    equipedList[equipSelectedInd].SetItem(currentSelectedItem);
                    //�������� ������ �������
                    if (currentSlot != null) currentSlot.SetItem();
                    else if (currentEquiped != null) currentEquiped.SetItem();

                    //������ ����
                    AudioController.instance.PlayEquipPutSound();
                }
                else
                {
                    //���� ���������� ����� �� ������ �������
                    MyInventoryItem tempItem = equipedList[equipSelectedInd].GetCurrItem();

                    //������ �� ���������
                    equipedList[equipSelectedInd].SetItem();
                    equipedList[equipSelectedInd].SetItem(slotsList[currSelectedInd].GetCurrItem());

                    //�������� ������ �������
                    if (currentSlot != null) currentSlot.SetItem();
                    else if (currentEquiped != null) currentEquiped.SetItem();
                    //������ �� ������
                    if (currentSlot != null) currentSlot.SetItem(tempItem, 1);
                    else if (currentEquiped != null) currentEquiped.SetItem(tempItem);

                    //������ ����
                    AudioController.instance.PlayEquipPutSound();
                }
            }
            else
            {
                //���� ����� ���� ��������� ������, �� ���������
                EnItemPut();
                return;
            }
        }
        //���� ��������� ������ �������
        else if (cellSelected)
        {
            //��������� ���� ������
            OpenSellPanel(currentSelectedItem, currentCount);
            //���������� ������� ���������� � ������
            itemSelectedToSell = currSelectedInd;
            equipSelectedToSell = currEquipedSelectedInd;
        }
        //���� ������ ��������� � ������ �� �����
        else
        {
            EnItemPut();
        }

        EnItemPut();

        //currSelectedInd = -1;
        //currEquipedSelectedInd = -1;
        ////�������� � coin panel, ��� �����-���� ������� ���������
        //coinPanel.ItemDrop();
        ////��������� ����� ��������
        //StatsPanel.instance.DropItem();
    }

    //��������� ������� �������
    private void EnItemPut()
    {
        DeselectAllItems();

        if (currSelectedInd != -1)
        {
            //�������� ������ �������, ���� ����
            slotsList[currSelectedInd].EnableItem();
        }
        if (currEquipedSelectedInd != -1)
        {
            //�������� ������ �������, ���� ����
            equipedList[currEquipedSelectedInd].EnableItem();
        }
        currSelectedInd = -1;
        currEquipedSelectedInd = -1;

        //�������� � coin panel, ��� �����-���� ������� ���������
        coinPanel.ItemDrop();
        //��������� ����� ��������
        StatsPanel.instance.DropItem();
    }

    //������� ������� �� ���������
    public void SelectItem(int ind)
    {
        //���� ����� �����
        if (canTake)
        {
            //������������� ��������� �������
            MyInventoryItem item = slotsList[ind].GetCurrItem();
            //���������� ���������� ��������
            int count = slotsList[ind].GetCurrCount();

            //��������� ������ �������
            currSelectedInd = ind;

            //������������� ��� ��� ������������ ��������
            if (!IsArmor(item) && !IsWeapon(item))
            {
                //������ � ��������� ������ � �������� ����� ����������
                movedItemSprite.sprite = item.itemIcon;
                movedItemCount.text = $"{count}";
                movedItemCount.gameObject.SetActive(true);

                movedItemBg.SetActive(true);
            }
            else
            {
                //���� ����� ��� ������ - ��������� ����� ����������
                movedItemCount.gameObject.SetActive(false);

                //������ � ��������� ������
                movedItemSprite.sprite = item.itemIcon;
                movedItemBg.SetActive(true);
            }

            //�����
            isTakeItem = true;
            //��������� � �������
            MoveTakedItem();
            //��������
            UpdateMoveItem(true);
            //�������� � coin panel, ��� �����-���� ������� ����
            coinPanel.ItemTake();

            //��������� ������������ ��� �������� ���� �����
            StatsPanel.instance.TakeItem(item);
        }
    }

    //������� ������� �� ����������
    public void SelectEquiped(int ind)
    {
        //���� ����� �����
        if (canTake)
        {
            //������������� ��������� �������
            MyInventoryItem item = equipedList[ind].GetCurrItem();

            //��������� ������ �������
            currEquipedSelectedInd = ind;

            //������ � ��������� ������
            movedItemSprite.sprite = item.itemIcon;
            movedItemBg.SetActive(true);
            movedItemCount.gameObject.SetActive(false);

            //�����
            isTakeItem = true;
            //��������� � �������
            MoveTakedItem();
            //��������
            UpdateMoveItem(true);
            //�������� � coin panel, ��� �����-���� ������� ����
            coinPanel.ItemTake();

            //��������� ������������ ��� �������� ���� �����
            StatsPanel.instance.TakeItem(item);
        }
    }

    //������� ��������� �������� ���� ����
    public void SellSelectedItems(MyInventoryItem itm, int cellCount)
    {
        //���� �� ������
        if (itemSelectedToSell != -1)
        {
            //��������� ��� �� ��� �������
            if (slotsList[itemSelectedToSell].GetCurrItem() == itm)
            {
                //���� ���, �� ������� ����������� ����������
                if (cellCount >= slotsList[itemSelectedToSell].GetCurrCount())
                {
                    //���� ����, �� ��������
                    RemoveCurrentItem(slotsList[itemSelectedToSell]);
                }
                else
                {
                    //���� �� ����, �� �������� ������ ����������
                    int currCount = slotsList[itemSelectedToSell].GetCurrCount() - cellCount;
                    //������ ����� ����������
                    slotsList[itemSelectedToSell].SetCurrCount(currCount);
                }
            }
            else
            {
                Debug.LogError("��������� ��� ������� ������� �� ������������� �������� � �����");
            }
        }
        //���� �� �����
        else if (equipSelectedToSell != -1)
        {
            //��������� ��� �� ��� �������
            if (equipedList[equipSelectedToSell].GetCurrItem() == itm)
            {
                //���� ���, �� �������
                if (cellCount == 1)
                {
                    //���� ����, �� ��������
                    RemoveCurrentEquiped(equipedList[equipSelectedToSell]);
                }
                else
                {
                    Debug.LogError("������������ ���������� ��� �������");
                }
            }
            else
            {
                Debug.LogError("��������� ��� ������� ������� �� ������������� �������� � �����");
            }
        }
        //�������� �������
        itemSelectedToSell = -1;
        equipSelectedToSell = -1;
    }

    //����������� �������� ������� ������
    private bool IsFirst
    {
        get
        {
            if (!PlayerPrefs.HasKey("IsFirst"))
            {
                PlayerPrefs.SetInt("IsFirst", 1);
            }
            if (PlayerPrefs.GetInt("IsFirst") == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("IsFirst", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsFirst", 0);
            }
        }
    }

    //������������� ��������� ������, ���� ������ ���
    private void SetDefault()
    {
        if (IsFirst)
        {
            //������� ��� �����
            RemoveAllItems();
            //�������� ��������� ��� � ��� �� �����
            MyInventoryItem defaultSword = null;
            MyInventoryItem defaultBow = null;
            foreach (MyInventoryItem item in allInventoryItems.AllInvenoryItems)
            {
                if(item.itemName == "��������� ���")
                {
                    defaultSword = item;
                }
                if (item.itemName == "��������� ���")
                {
                    defaultBow = item;
                }
            }
            if (defaultSword == null)
            {
                Debug.LogError("�� ����� ��������� ���");
                return;
            }
            if (defaultBow == null)
            {
                Debug.LogError("�� ����� ��������� ���");
                return;
            }

            //������������� ��������� ��� � ���
            foreach (EquipedItem equipedItem in equipedList)
            {
                if (equipedItem.GetEquipType() == EquipedTypes.SwordWeapon)
                {
                    //������������� ��������� ���
                    equipedItem.SetItem(defaultSword);
                }
                if (equipedItem.GetEquipType() == EquipedTypes.BowWeapon)
                {
                    //������������� ��������� ���
                    equipedItem.SetItem(defaultBow);
                }
            }
            IsFirst = false;
        }
    }

    //������ ��� �� �����
    public void GiveFullSet(string setName)
    {
        int setInd = -1;
        for (int i = 0; i < allInventoryItems.AllArmorSets.Count; i++) 
        {
            if (allInventoryItems.AllArmorSets[i].SetName == setName)
            {
                setInd = i;
                break;
            }
        }
        if (setInd != -1)
        {
            //���� ����� �� ������ ��� ��������
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Sword);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Bow);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Helmet);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Torso);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Gloves);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Boots);
        }
        else
        {
            //���� �� �����, �� ������� ������
            Debug.Log($"��� {setName} �� ���������� � ������ ����� �����");
        }
    }

    //��������� ���� ������ ��������� ��������� 
    private void SetAllItemList()
    {
        //��������� ��������� ������
        AddToSetList(allInventoryItems.defaultSword);
        AddToSetList(allInventoryItems.defaultBow);

        //���������� �� ���� ����� � ��������� �� ������� � ����� ������
        foreach (ArmorSet armorSet in allInventoryItems.AllArmorSets)
        {
            AddToSetList(armorSet.Sword);
            AddToSetList(armorSet.Bow);
            AddToSetList(armorSet.Helmet);
            AddToSetList(armorSet.Gloves);
            AddToSetList(armorSet.Torso);
            AddToSetList(armorSet.Boots);
        }

        //���������� �� ���� ������ � ��������� ��
        foreach (MyInventoryItem itm in allInventoryItems.allPoition)
        {
            AddToSetList(itm);
        }

        //���������� �� ���� ������� ��������� � ��������� ��
        foreach (MyInventoryItem itm in allInventoryItems.allItems)
        {
            AddToSetList(itm);
        }
    }

    //��������� � ���� ����� ���� ������ �������� ���
    private void AddToSetList(MyInventoryItem currItem)
    {
        //���������� �� ����� ���� � ������� ���� �� ����� �� �������
        foreach (MyInventoryItem item in allInventoryItems.AllInvenoryItems)
        {
            if (item == currItem)
            {
                return;
            }
        }
        //���� ����� ������� �� ����� �� ���������
        allInventoryItems.AllInvenoryItems.Add(currItem);
    }

    //�������������� ��� ��������
    private void InitializedSlotsItems()
    {
        slotsList = new List<InventorySlot>();

        for (int i = 0; i < slotsPanel.transform.childCount; i++)
        {
            slotsList.Add(slotsPanel.transform.GetChild(i).gameObject.GetComponent<InventorySlot>());
            //������������� ������
            slotsList[i].SetIndex(i);
            //������������ ��� ��������
            slotsList[i].SetAllItems(allInventoryItems);
            //������������
            slotsList[i].UpdateItemInSlot();
        }
    }

    //�������������� ��� �������� ����������
    private void InitializedEquipedItems()
    {
        equipedList = new List<EquipedItem>();

        for (int i = 0; i < equipedPanel.transform.childCount; i++)
        {
            equipedList.Add(equipedPanel.transform.GetChild(i).gameObject.GetComponent<EquipedItem>());
            //������������ ��� ��������
            equipedList[i].SetAllItems(allInventoryItems);
            //������ ������
            equipedList[i].SetInd(i);
            //������������
            equipedList[i].UpdateItemInEquip();
        }

        //��������� ����������� ���������
        PlayerController.instance.UpdatePlayerView();
        //��������� �������������� ���������
        //PlayerController.instance.UpdateCurrentStats();
    }

    //���� �� ��������� �����
    public bool IsFreeSlots()
    {
        foreach (InventorySlot slot in slotsList)
        {
            if (slot.IsFree)
            {
                return true;
            }
        }
        return false;
    }

    //���� �� �������� ����
    private int IsSelectedSlot()
    {
        for (int i = 0; i < slotsList.Count; i++) 
        {
            if (slotsList[i].IsSelected)
            {
                return i;
            }
        }
        return -1;
    }

    //���� �� ��������� ���� � ����������
    private int IsSelectedEquiped()
    {
        for (int i = 0; i < equipedList.Count; i++)
        {
            if (equipedList[i].IsSelected)
            {
                return i;
            }
        }
        return -1;
    }

    //������� �� ������ �������
    private bool IsSelectedSellBtn()
    {
       return coinPanel.IsSelected;
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

    //��������� ���������
    public void InventoryOpen()
    {
        UpdateCurrency();
        inventoryView.SetActive(true);
        StatsPanel.instance.OpenInventory();
        isInventory = true;
    }

    //��������� �������� � ���� ������
    private void DeselectAllItems()
    {
        foreach (InventorySlot slot in slotsList)
        {
            slot.DeSelectItem();
        }
        foreach (EquipedItem equip in equipedList)
        {
            equip.DeSelectItem();
        }
        coinPanel.DeSelectItem();
    }

    //��������� ���������� �����
    public void UpdateCurrency()
    {
        coinsTxt.text = $"{currentStats.Coins}";
    }

    //��������� ������ ��� �������
    private void OpenSellPanel(MyInventoryItem itm, int itmCount)
    {
        sellPanelController.OpenSellPanel(itm, itmCount);
        sellPanel.SetActive(true);
    }

    //����� �� ������ �������
    public void CloseSellPanel()
    {
        //�������� �������� ���� ��������
        if (itemSelectedToSell != -1)
        {
            //�������� ������ �������, ���� ����
            slotsList[itemSelectedToSell].EnableItem();
        }
        if (equipSelectedToSell != -1)
        {
            //�������� ������ �������, ���� ����
            equipedList[equipSelectedToSell].EnableItem();
        }
        itemSelectedToSell = -1;
        equipSelectedToSell = -1;
    }

    //��������� ������ ��
    public void OpenHealthPanel(MyInventoryItem itm, int itmCount, int itemInd)
    {
        if (itmCount > 0 && itemInd != -1) 
        {
            itemSelectedToHealth = itemInd;
            healthController.OpenHealthPanel(itm, itmCount);
            healthPanel.SetActive(true);
        }
    }

    //��������� ������ ��
    public void CloseHealthPanel()
    {
        if (itemSelectedToHealth != -1)
        {
            //�������� ���� ����
            slotsList[itemSelectedToHealth].EnableItem();
        }
        itemSelectedToHealth = -1;
    }

    //�������� ������ ����� ���� ����
    public void RemoveSelectedHealth(MyInventoryItem itm, int cellCount)
    {
        //�� ������
        if (itemSelectedToHealth != -1)
        {
            //��������� ��� �� ��� �������
            if (slotsList[itemSelectedToHealth].GetCurrItem() == itm)
            {
                //���� ���, �� ������� ����������� ����������
                if (cellCount >= slotsList[itemSelectedToHealth].GetCurrCount())
                {
                    //���� ����, �� ��������
                    RemoveCurrentItem(slotsList[itemSelectedToHealth]);
                }
                else
                {
                    //���� �� ����, �� �������� ������ ����������
                    int currCount = slotsList[itemSelectedToHealth].GetCurrCount() - cellCount;
                    //������ ����� ����������
                    slotsList[itemSelectedToHealth].SetCurrCount(currCount);
                }
            }
            else
            {
                Debug.LogError("��������� ��� �������� ����� �� ������������� �������� � �����");
            }
        }
        //�������� �������
        itemSelectedToHealth = -1;
    }

    //��������� ���������
    private void InventoryClose()
    {
        inventoryView.SetActive(false);
        isInventory = false;
        GameController.instance.SetPause(false);
    }
}
