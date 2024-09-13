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

        //инициализуем все элементы
        InitializedSlotsItems();
        InitializedEquipedItems();

        //обновляем статы
        AddStatsNull();
        SetAllEquipedStats();

        //настройка кнопок
        ButtonSettings();

        //устанавливаем ли дефолтные предметы
        SetDefault();
    }

    //настройка кнопок
    private void ButtonSettings()
    {
        closeBtn.onClick.AddListener(InventoryClose);
    }

    //обнуляем бонусы с уровней на старте
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

    //устанавливаем прибалвяемые характеристики от экипированных слотов
    public void SetAllEquipedStats()
    {
        //обнуляем текущие прибавки от экипировки
        PlayerController.instance.AddEquipeStats(-1);

        //обнуляем
        swordDamage = 0;
        bowDamage = 0;
        health = 0;
        armor = 0;
        luck = 0;
        readyTime = 0f;
        swordAttackTime = 0f;
        bowAttackTime = 0f;

        //проходимся по всей экипировке и прибавляем все статы
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

        //обновляем характерстики
        currentStats.addEquipSwordDamage = swordDamage;
        currentStats.addEquipBowDamage = bowDamage;
        currentStats.addEquipHealth = health;
        currentStats.addEquipArmor = armor;
        currentStats.addEquipLuck = luck;
        currentStats.addEquipReadyTime = readyTime;
        currentStats.addEquipSwordAttackTime = swordAttackTime;
        currentStats.addEquipBowAttackTime = bowAttackTime;

        //обновляем харакетристи у игрока
        PlayerController.instance.AddEquipeStats();

        //обновляем отображение в инвентаре
        StatsPanel.instance.UpdateStatsBtns();
    }

    private void Update()
    {
        //двигаем выбранный предмет
        MoveTakedItem();
        UpdateClickUp();
    }

    //помещаем выбранный предмет в слоты
    public void SetToSlots(MyInventoryItem setItem, int itemCounts = 1)
    {
        //если выюранный предмет не равен null
        if (setItem != null)
        {
            //если не броня и не оружие
            if (!IsArmor(setItem) && !IsWeapon(setItem))
            {
                //смотрим, есть ли уже такой предмет в инвентаре, если есть, то добавляем
                foreach (InventorySlot slot in slotsList)
                {
                    if (setItem == slot.GetCurrItem())
                    {
                        //добавляем
                        slot.AddItems(itemCounts);
                        return;
                    }
                }
            }
            //если такого предмета нет, или броня или оружие, то добавляем к пустым предметам
            if (IsFreeSlots())
            {
                //если есть пустые
                foreach (InventorySlot slot in slotsList)
                {
                    //проходимся по всем и добавляем в первый свободный
                    if (slot.IsFree)
                    {
                        slot.SetItem(setItem, itemCounts);
                        return;
                    }
                }
            }
            else
            {
                //нет пустых слотов - не ставим, играем анимацию полного инвентаря
                GameController.instance.PlayFullInvenoty();
                //Debug.LogError("Нет пустых слотов");
            }
        }
    }

    //снимаем все предметы
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

    //снимаем выбранный предмет
    public void RemoveCurrentItem(InventorySlot slot)
    {
        slot.SetItem(null);
    }

    //снимаем выбранную экипировку
    public void RemoveCurrentEquiped(EquipedItem equip)
    {
        equip.SetItem(null);
    }

    //обновляем зажата ли кнопка мыши
    private void UpdateClickUp()
    {
        if (Input.GetMouseButtonUp(0) && isInventory)
        {
            //отпускаем предмет если в руке
            if (isTakeItem)
            {
                //обновляем взятого предмета
                UpdateMoveItem(false);
                //поставили ли предмет
                IsPutItem();

                isTakeItem = false;
            }
        }
    }

    //Передвигаем взятый предмет
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

    //обновляем отображение двигающейся картинки
    private void UpdateMoveItem(bool value)
    {
        moveItem.SetActive(value);
    }

    //поставили ли предмет
    private void IsPutItem()
    {
        //смотрим ставим ли предмет (есть ли выделенный слот)
        int selectedInd = IsSelectedSlot();

        //есть ли выделенный слот среди экипировки
        int equipSelectedInd = IsSelectedEquiped();

        //есть ли выделенная кнопка продажи
        bool cellSelected = IsSelectedSellBtn();
        
        //создаем переменные для работы с выбраным предметом
        MyInventoryItem currentSelectedItem = null;
        int currentCount = 0;
        InventorySlot currentSlot = null;
        EquipedItem currentEquiped = null;

        //выбираем текущий выбранный предмет, либо из экипировки, либо из слотов
        if (currSelectedInd != -1)
        {
            //если из слотов
            currentSlot = slotsList[currSelectedInd];
            currentSelectedItem = currentSlot.GetCurrItem();
            currentCount = currentSlot.GetCurrCount();
        }
        else if(currEquipedSelectedInd != -1)
        {
            //если из экипировки
            currentEquiped = equipedList[currEquipedSelectedInd];
            currentSelectedItem = currentEquiped.GetCurrItem();
            currentCount = 1;
        }
        else
        {
            //если выбранных нет, то выходим
            return;
        }

        bool isPut;
        //если выделенный есть и он не равен тому из которого взяли
        if (selectedInd != -1 && selectedInd != currSelectedInd) isPut = true;
        else isPut = false;

        //если есть выделенный слот
        if (isPut)
        {
            //если выделенный свободный
            if (slotsList[selectedInd].IsFree)
            {
                //ставим на пустой взятый предмет
                slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                //обнуялем взятый предмет
                if (currentSlot != null) currentSlot.SetItem();
                else if (currentEquiped != null) currentEquiped.SetItem();
            }
            else
            {
                //если выделенный занят, то меняем местами
                MyInventoryItem tempItem = slotsList[selectedInd].GetCurrItem();
                int tempCount = slotsList[selectedInd].GetCurrCount();

                //обнуляем
                //if (currentSlot != null) 
                //ставим на взятый
                if (currentSlot != null) 
                {
                    currentSlot.SetItem();
                    currentSlot.SetItem(tempItem, tempCount);

                    //ставим на выбранный
                    slotsList[selectedInd].SetItem();
                    slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                }
                else if (currentEquiped != null)
                {
                    //если можем поставить на экипировку
                    if (currentEquiped.IsCorrectItem(tempItem))
                    {
                        currentEquiped.SetItem();
                        currentEquiped.SetItem(tempItem);

                        //ставим на выбранный
                        slotsList[selectedInd].SetItem();
                        slotsList[selectedInd].SetItem(currentSelectedItem, currentCount);
                    }
                    else
                    {
                        //если не можем заменить, так как меняем не на нужный предмет, то отменяем выбор
                        EnItemPut();
                        return;
                    }
                }

            }
        }
        //если есть выделенная экипировка
        else if (equipSelectedInd != -1 && equipSelectedInd != currEquipedSelectedInd)
        {
            //проверяем можем ли поставить текущий слот на выделенную экипировку
            if (equipedList[equipSelectedInd].IsCorrectItem(currentSelectedItem))
            {
                if (equipedList[equipSelectedInd].IsFree)
                {
                    //если пустой, то ставим на него
                    equipedList[equipSelectedInd].SetItem(currentSelectedItem);
                    //обнуялем взятый предмет
                    if (currentSlot != null) currentSlot.SetItem();
                    else if (currentEquiped != null) currentEquiped.SetItem();

                    //играем звук
                    AudioController.instance.PlayEquipPutSound();
                }
                else
                {
                    //если выделенный занят то меняем местами
                    MyInventoryItem tempItem = equipedList[equipSelectedInd].GetCurrItem();

                    //ставим на выбранный
                    equipedList[equipSelectedInd].SetItem();
                    equipedList[equipSelectedInd].SetItem(slotsList[currSelectedInd].GetCurrItem());

                    //обнуялем взятый предмет
                    if (currentSlot != null) currentSlot.SetItem();
                    else if (currentEquiped != null) currentEquiped.SetItem();
                    //ставим на взятый
                    if (currentSlot != null) currentSlot.SetItem(tempItem, 1);
                    else if (currentEquiped != null) currentEquiped.SetItem(tempItem);

                    //играем звук
                    AudioController.instance.PlayEquipPutSound();
                }
            }
            else
            {
                //если такой слот поставить нельзя, то выключаем
                EnItemPut();
                return;
            }
        }
        //если выделеная кнопка продажи
        else if (cellSelected)
        {
            //открываем меню продаж
            OpenSellPanel(currentSelectedItem, currentCount);
            //запоминаем индексы экипировки и айтема
            itemSelectedToSell = currSelectedInd;
            equipSelectedToSell = currEquipedSelectedInd;
        }
        //если просто отпустили и ничего не нашли
        else
        {
            EnItemPut();
        }

        EnItemPut();

        //currSelectedInd = -1;
        //currEquipedSelectedInd = -1;
        ////передаем в coin panel, что какой-либо предмет поставили
        //coinPanel.ItemDrop();
        ////выключаем статы предмета
        //StatsPanel.instance.DropItem();
    }

    //закончили ставить предмет
    private void EnItemPut()
    {
        DeselectAllItems();

        if (currSelectedInd != -1)
        {
            //включаем взятый обратно, если есть
            slotsList[currSelectedInd].EnableItem();
        }
        if (currEquipedSelectedInd != -1)
        {
            //включаем взятый обратно, если есть
            equipedList[currEquipedSelectedInd].EnableItem();
        }
        currSelectedInd = -1;
        currEquipedSelectedInd = -1;

        //передаем в coin panel, что какой-либо предмет поставили
        coinPanel.ItemDrop();
        //выключаем статы предмета
        StatsPanel.instance.DropItem();
    }

    //выбрали предмет из инвенторя
    public void SelectItem(int ind)
    {
        //если можем брать
        if (canTake)
        {
            //устанавливаем выбранный предмет
            MyInventoryItem item = slotsList[ind].GetCurrItem();
            //количество выбранного предмета
            int count = slotsList[ind].GetCurrCount();

            //запомнили взятый предмет
            currSelectedInd = ind;

            //устанавливаем вид для двигающегося предмета
            if (!IsArmor(item) && !IsWeapon(item))
            {
                //ставим в малеьнкую иконку и включаем текст количества
                movedItemSprite.sprite = item.itemIcon;
                movedItemCount.text = $"{count}";
                movedItemCount.gameObject.SetActive(true);

                movedItemBg.SetActive(true);
            }
            else
            {
                //если броня или оружие - выключаем текст количества
                movedItemCount.gameObject.SetActive(false);

                //ставим в малеьнкую иконку
                movedItemSprite.sprite = item.itemIcon;
                movedItemBg.SetActive(true);
            }

            //взяли
            isTakeItem = true;
            //подвинули к курсору
            MoveTakedItem();
            //включили
            UpdateMoveItem(true);
            //передаем в coin panel, что какой-либо предмет взят
            coinPanel.ItemTake();

            //обновляем характеристи для предмета если нужно
            StatsPanel.instance.TakeItem(item);
        }
    }

    //выбрали предмет из экипировки
    public void SelectEquiped(int ind)
    {
        //если можем брать
        if (canTake)
        {
            //устанавливаем выбранный предмет
            MyInventoryItem item = equipedList[ind].GetCurrItem();

            //запомнили взятый предмет
            currEquipedSelectedInd = ind;

            //ставим в малеьнкую иконку
            movedItemSprite.sprite = item.itemIcon;
            movedItemBg.SetActive(true);
            movedItemCount.gameObject.SetActive(false);

            //взяли
            isTakeItem = true;
            //подвинули к курсору
            MoveTakedItem();
            //включили
            UpdateMoveItem(true);
            //передаем в coin panel, что какой-либо предмет взят
            coinPanel.ItemTake();

            //обновляем характеристи для предмета если нужно
            StatsPanel.instance.TakeItem(item);
        }
    }

    //продаем выбранные предметы если есть
    public void SellSelectedItems(MyInventoryItem itm, int cellCount)
    {
        //если из слотов
        if (itemSelectedToSell != -1)
        {
            //проверяем тот ли это предмет
            if (slotsList[itemSelectedToSell].GetCurrItem() == itm)
            {
                //если тот, то продаем необходимое количество
                if (cellCount >= slotsList[itemSelectedToSell].GetCurrCount())
                {
                    //если весь, то обнуляем
                    RemoveCurrentItem(slotsList[itemSelectedToSell]);
                }
                else
                {
                    //если не весь, то отнимаем нужное количество
                    int currCount = slotsList[itemSelectedToSell].GetCurrCount() - cellCount;
                    //ставим новое количество
                    slotsList[itemSelectedToSell].SetCurrCount(currCount);
                }
            }
            else
            {
                Debug.LogError("Выбранные для продажи предмет не соотсветсвует предмету в слоте");
            }
        }
        //если из брони
        else if (equipSelectedToSell != -1)
        {
            //проверяем тот ли это предмет
            if (equipedList[equipSelectedToSell].GetCurrItem() == itm)
            {
                //если тот, то продаем
                if (cellCount == 1)
                {
                    //если весь, то обнуляем
                    RemoveCurrentEquiped(equipedList[equipSelectedToSell]);
                }
                else
                {
                    Debug.LogError("Некорректное количество для продажи");
                }
            }
            else
            {
                Debug.LogError("Выбранные для продажи предмет не соотсветсвует предмету в слоте");
            }
        }
        //обнуляем индексы
        itemSelectedToSell = -1;
        equipSelectedToSell = -1;
    }

    //сохраненное значение первого захода
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

    //устанавливаем дефолтные оружия, если первый раз
    private void SetDefault()
    {
        if (IsFirst)
        {
            //очищаем все слоты
            RemoveAllItems();
            //Выбираем стартовый меч и лук по имени
            MyInventoryItem defaultSword = null;
            MyInventoryItem defaultBow = null;
            foreach (MyInventoryItem item in allInventoryItems.AllInvenoryItems)
            {
                if(item.itemName == "Стартовый меч")
                {
                    defaultSword = item;
                }
                if (item.itemName == "Стартовый лук")
                {
                    defaultBow = item;
                }
            }
            if (defaultSword == null)
            {
                Debug.LogError("Не нашли стартовый меч");
                return;
            }
            if (defaultBow == null)
            {
                Debug.LogError("Не нашли стартовый лук");
                return;
            }

            //устанавливаем дефолтный меч и лук
            foreach (EquipedItem equipedItem in equipedList)
            {
                if (equipedItem.GetEquipType() == EquipedTypes.SwordWeapon)
                {
                    //устанавливаем дефолтный меч
                    equipedItem.SetItem(defaultSword);
                }
                if (equipedItem.GetEquipType() == EquipedTypes.BowWeapon)
                {
                    //устанавливаем дефолтный меч
                    equipedItem.SetItem(defaultBow);
                }
            }
            IsFirst = false;
        }
    }

    //выдаем сет по имени
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
            //если нашли то выдаем все предметы
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Sword);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Bow);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Helmet);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Torso);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Gloves);
            SetToSlots(allInventoryItems.AllArmorSets[setInd].Boots);
        }
        else
        {
            //если не нашли, то выводим ошибку
            Debug.Log($"Имя {setName} не существует в списке сетов брони");
        }
    }

    //заполняем весь список доступных предметов 
    private void SetAllItemList()
    {
        //добавляем дефолтные айтемы
        AddToSetList(allInventoryItems.defaultSword);
        AddToSetList(allInventoryItems.defaultBow);

        //проходимся по всем сетам и добавляем их объекты в общий список
        foreach (ArmorSet armorSet in allInventoryItems.AllArmorSets)
        {
            AddToSetList(armorSet.Sword);
            AddToSetList(armorSet.Bow);
            AddToSetList(armorSet.Helmet);
            AddToSetList(armorSet.Gloves);
            AddToSetList(armorSet.Torso);
            AddToSetList(armorSet.Boots);
        }

        //проходимся по всем зельям и добавляем их
        foreach (MyInventoryItem itm in allInventoryItems.allPoition)
        {
            AddToSetList(itm);
        }

        //проходимся по всем обычным предметам и добавляем их
        foreach (MyInventoryItem itm in allInventoryItems.allItems)
        {
            AddToSetList(itm);
        }
    }

    //добавляем в лист брони если такого предмета нет
    private void AddToSetList(MyInventoryItem currItem)
    {
        //проходимся по всему сету и смотрим есть ли такой же предмет
        foreach (MyInventoryItem item in allInventoryItems.AllInvenoryItems)
        {
            if (item == currItem)
            {
                return;
            }
        }
        //если такой предмет не нашли то добавляем
        allInventoryItems.AllInvenoryItems.Add(currItem);
    }

    //инициализируем все элементы
    private void InitializedSlotsItems()
    {
        slotsList = new List<InventorySlot>();

        for (int i = 0; i < slotsPanel.transform.childCount; i++)
        {
            slotsList.Add(slotsPanel.transform.GetChild(i).gameObject.GetComponent<InventorySlot>());
            //устанавлвиаем индекс
            slotsList[i].SetIndex(i);
            //устанвливаем все элементы
            slotsList[i].SetAllItems(allInventoryItems);
            //инициализуем
            slotsList[i].UpdateItemInSlot();
        }
    }

    //инициализируем все элементы экипировки
    private void InitializedEquipedItems()
    {
        equipedList = new List<EquipedItem>();

        for (int i = 0; i < equipedPanel.transform.childCount; i++)
        {
            equipedList.Add(equipedPanel.transform.GetChild(i).gameObject.GetComponent<EquipedItem>());
            //устанвливаем все элементы
            equipedList[i].SetAllItems(allInventoryItems);
            //ставим индекс
            equipedList[i].SetInd(i);
            //инициализуем
            equipedList[i].UpdateItemInEquip();
        }

        //обновляем отображение персонажа
        PlayerController.instance.UpdatePlayerView();
        //обновляем характеристики персонажа
        //PlayerController.instance.UpdateCurrentStats();
    }

    //есть ли свободные слоты
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

    //есть ли выбраный слот
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

    //есть ли выбранный слот в экипировке
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

    //выбрана ли кнопка продажи
    private bool IsSelectedSellBtn()
    {
       return coinPanel.IsSelected;
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

    //открываем инвентарь
    public void InventoryOpen()
    {
        UpdateCurrency();
        inventoryView.SetActive(true);
        StatsPanel.instance.OpenInventory();
        isInventory = true;
    }

    //выключаем выделени у всех слотов
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

    //обновляем отбражение денег
    public void UpdateCurrency()
    {
        coinsTxt.text = $"{currentStats.Coins}";
    }

    //открываем панель для продажи
    private void OpenSellPanel(MyInventoryItem itm, int itmCount)
    {
        sellPanelController.OpenSellPanel(itm, itmCount);
        sellPanel.SetActive(true);
    }

    //вышли из панели продажи
    public void CloseSellPanel()
    {
        //включаем предметы если остались
        if (itemSelectedToSell != -1)
        {
            //включаем взятый обратно, если есть
            slotsList[itemSelectedToSell].EnableItem();
        }
        if (equipSelectedToSell != -1)
        {
            //включаем взятый обратно, если есть
            equipedList[equipSelectedToSell].EnableItem();
        }
        itemSelectedToSell = -1;
        equipSelectedToSell = -1;
    }

    //открываем панель хп
    public void OpenHealthPanel(MyInventoryItem itm, int itmCount, int itemInd)
    {
        if (itmCount > 0 && itemInd != -1) 
        {
            itemSelectedToHealth = itemInd;
            healthController.OpenHealthPanel(itm, itmCount);
            healthPanel.SetActive(true);
        }
    }

    //закрываем панель хп
    public void CloseHealthPanel()
    {
        if (itemSelectedToHealth != -1)
        {
            //включаем если есть
            slotsList[itemSelectedToHealth].EnableItem();
        }
        itemSelectedToHealth = -1;
    }

    //вычитаем взятые хилки если есть
    public void RemoveSelectedHealth(MyInventoryItem itm, int cellCount)
    {
        //из слотов
        if (itemSelectedToHealth != -1)
        {
            //проверяем тот ли это предмет
            if (slotsList[itemSelectedToHealth].GetCurrItem() == itm)
            {
                //если тот, то продаем необходимое количество
                if (cellCount >= slotsList[itemSelectedToHealth].GetCurrCount())
                {
                    //если весь, то обнуляем
                    RemoveCurrentItem(slotsList[itemSelectedToHealth]);
                }
                else
                {
                    //если не весь, то отнимаем нужное количество
                    int currCount = slotsList[itemSelectedToHealth].GetCurrCount() - cellCount;
                    //ставим новое количество
                    slotsList[itemSelectedToHealth].SetCurrCount(currCount);
                }
            }
            else
            {
                Debug.LogError("Выбранные для удаления зелья не соотсветсвует предмету в слоте");
            }
        }
        //обнуляем индексы
        itemSelectedToHealth = -1;
    }

    //закрываем инвентарь
    private void InventoryClose()
    {
        inventoryView.SetActive(false);
        isInventory = false;
        GameController.instance.SetPause(false);
    }
}
