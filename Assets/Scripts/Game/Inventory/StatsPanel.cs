using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//все характеристики персонажа
public enum AllPlayerStatsType
{
    SwordDamage,
    BowDamage,
    Health,
    Armor,
    Luck,
    ChargeTime,
    ReadyTime,
    SwordAtckTime,
    BowAtckTime
}

//структура для текста у характеристик
public struct StatText
{
    public string StatTitle;
    public string StatDescription;
    public float StatMeleeDamage;
    public float StatRangeDamage;
    public float StatCritDamage;
}

public class StatsPanel : MonoBehaviour
{

    [Header("Player Stats Btns")]
    [Tooltip("Все кнопки характеристик персонажа")]
    public StatBtn[] statBtns;

    [Header("Item Stats Btn")]
    [Tooltip("Все кнопки характеристик предмета")]
    public StatBtn[] statItemsBtns;
    public GameObject itemStatsBtnsPanel;
    public GameObject itemStatsView;
    public TMPro.TMP_Text itemName;
    public GameObject titlePanel;
    public GameObject itemNamePanel;
    public TMPro.TMP_Text itemNamePanelTxt;

    [Header("Current Stats")]
    public PlayerCurrentStats currentStats;

    public static StatsPanel instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    //обновляем отображаемые характеристики
    public void UpdateStatsBtns()
    {
        //проходимся по всем кнопкам и записываем харакетристики в зависимости от типа
        foreach (StatBtn statBtn in statBtns)
        {
            switch (statBtn.GetCurrStatType())
            {
                case AllPlayerStatsType.SwordDamage:
                    statBtn.SetStat(currentStats.currSwordAttackDamage);
                    break;
                case AllPlayerStatsType.BowDamage:
                    statBtn.SetStat(currentStats.currBowAttackDamage);
                    break;
                case AllPlayerStatsType.Health:
                    statBtn.SetStat(currentStats.currHealth);
                    break;
                case AllPlayerStatsType.Armor:
                    statBtn.SetStat(currentStats.currArmor, false, true);
                    break;
                case AllPlayerStatsType.Luck:
                    statBtn.SetStat(currentStats.currLuck, false, true);
                    break;
                case AllPlayerStatsType.ChargeTime:
                    statBtn.SetStat(currentStats.currChangeDurability, true);
                    break;
                case AllPlayerStatsType.ReadyTime:
                    statBtn.SetStat(currentStats.currReadyAtckCoolDown, true);
                    break;
                case AllPlayerStatsType.SwordAtckTime:
                    statBtn.SetStat(currentStats.currSwordAttackDurability, true);
                    break;
                case AllPlayerStatsType.BowAtckTime:
                    statBtn.SetStat(currentStats.currBowAttackDurability, true);
                    break;
            }
        }
    }

    //устанавливаем текста для всех айтемов
    public StatText GetStatsTexts(AllPlayerStatsType statType)
    {
        StatText statText = new StatText();

        //записываем нужный текст в зависимости от типа
        switch (statType)
        {
            case AllPlayerStatsType.SwordDamage:
                statText.StatTitle = "Урон Мечом";
                statText.StatDescription = "Урон наносимый мечом в зависимости от расстояния";
                statText.StatMeleeDamage = currentStats.currSwordAttackDamage;
                statText.StatRangeDamage = currentStats.currSwordAttackDamage * currentStats.currSwordDamageToRangeKoef;
                statText.StatCritDamage = currentStats.currSwordAttackDamage + currentStats.GetCurrSwordAddCriteDmg;
                break;
            case AllPlayerStatsType.BowDamage:
                statText.StatTitle = "Урон Луком";
                statText.StatDescription = "Урон наносимый луком в зависимости от расстояния";
                statText.StatMeleeDamage = currentStats.currBowAttackDamage * currentStats.currBowDamageToMeleeKoef;
                statText.StatRangeDamage = currentStats.currBowAttackDamage;
                statText.StatCritDamage = currentStats.currBowAttackDamage + currentStats.GetCurrBowAddCriteDmg;
                break;
            case AllPlayerStatsType.Health:
                statText.StatTitle = "Количество здоровья персонажа";
                statText.StatDescription = "Максимальное количество здоровья персонажа";
                break;
            case AllPlayerStatsType.Armor:
                statText.StatTitle = "Защита персонажа в процентах";
                statText.StatDescription = "Уменьшает полученный урон в процентах";
                break;
            case AllPlayerStatsType.Luck:
                statText.StatTitle = "Удача персонажа в процентах";
                statText.StatDescription = "Увеличивает шанс критического удара в процентах";
                break;
            case AllPlayerStatsType.ChargeTime:
                statText.StatTitle = "Длительность замены оружия";
                statText.StatDescription = "Время необходимое на замену оружия в секнудах";
                break;
            case AllPlayerStatsType.ReadyTime:
                statText.StatTitle = "Длительность подготовки к удару";
                statText.StatDescription = "Время необходимое для подготовки к удару в секнудах";
                break;
            case AllPlayerStatsType.SwordAtckTime:
                statText.StatTitle = "Длительность удара Мечом";
                statText.StatDescription = "Время необходимое для совершения удара Мечом в секнудах";
                break;
            case AllPlayerStatsType.BowAtckTime:
                statText.StatTitle = "Длительность удара Луком";
                statText.StatDescription = "Время необходимое для совершения удара Луком в секнудах";
                break;
        }

        return statText;
    }

    //взяли айтем и смотрим открываем ли статы
    public void TakeItem(MyInventoryItem takedItem)
    {
        //устанавливаем имя айтема
        SetItemName(takedItem.itemName);

        //если нужный тип, то открываем меню статов предмета
        if (IsWeapon(takedItem) || IsArmor(takedItem))
        {
            DefineOpenItemStats(takedItem);
            IsEnableItemStats(true);
        }
        //если зелье то включаем только количество даваемого хп
        else if (takedItem.itemType == InventoryItemType.HealthPotion)
        {
            DefineHealthPoitionStats(takedItem);
            IsEnableItemStats(true);
        }
        else
        {
            //если не броня и не оружие то включаем только имя
            IsEnableItemName(true);
        }
    }

    //поставили предмет - выключаем статы предмета если включены
    public void DropItem()
    {
        //если включены статы предмета - то выключаем статы айтема и включаем персонада
        if (itemStatsBtnsPanel.activeSelf || itemNamePanel.activeSelf)
        {
            IsEnableTitlePanel(true);
        }
    }

    //включаем только нужные слоты предметов, в зависимости от предмета
    private void DefineOpenItemStats(MyInventoryItem takedItem)
    {
        //для начала выключаем все статы
        foreach(StatBtn itemStats in statItemsBtns)
        {
            itemStats.gameObject.SetActive(false);
        }

        //включаем необходимые характеристик в зависимости от типа предмета
        //и записываем в них нужные статы
        switch (takedItem.itemType)
        {
            case InventoryItemType.SwordWeapon:
                {
                    EnableSelectedStat(AllPlayerStatsType.SwordDamage, takedItem.weaponDamage);
                    EnableSelectedStat(AllPlayerStatsType.Luck, takedItem.weaponLuck, false, true);
                    EnableSelectedStat(AllPlayerStatsType.SwordAtckTime, takedItem.swordAttackDurability, true);
                    break;
                }
            case InventoryItemType.BowWeapon:
                {
                    EnableSelectedStat(AllPlayerStatsType.BowDamage, takedItem.weaponDamage);
                    EnableSelectedStat(AllPlayerStatsType.Luck, takedItem.weaponLuck, false, true);
                    EnableSelectedStat(AllPlayerStatsType.BowAtckTime, takedItem.bowAttackDurability, true);
                    break;
                }
            case InventoryItemType.Helmet:
            case InventoryItemType.Gloves:
            case InventoryItemType.Torso:
            case InventoryItemType.Boots:
                {
                    EnableSelectedStat(AllPlayerStatsType.Armor, takedItem.armorArmor, false, true);
                    EnableSelectedStat(AllPlayerStatsType.Health, takedItem.armorHealth);
                    EnableSelectedStat(AllPlayerStatsType.Luck, takedItem.armorLuck, false, true);
                    EnableSelectedStat(AllPlayerStatsType.ReadyTime, takedItem.armorAddReadyToAtck, true);
                    break;
                }
        }
    }

    //включаем отображение статов для зелья
    private void DefineHealthPoitionStats(MyInventoryItem takedItem)
    {
        //для начала выключаем все статы
        foreach (StatBtn itemStats in statItemsBtns)
        {
            itemStats.gameObject.SetActive(false);
        }

        //включаем необходимые характеристик в зависимости от типа предмета
        //и записываем в них нужные статы
        EnableSelectedStat(AllPlayerStatsType.Health, takedItem.poitionHealthAmount);
    }

    //включаем нужную харакетирстику из статов предмтов
    private void EnableSelectedStat(AllPlayerStatsType selectedStatType, 
        float currStatValue, bool isSec = false, bool isProcent = false, bool isAnim = false)
    {
        //пробегаемся по всем статм и включаем нужную
        foreach (StatBtn itemStats in statItemsBtns)
        {
            if (itemStats.currentStatType == selectedStatType)
            {
                //записываем в него нужную характеристику
                itemStats.SetStat(currStatValue, isSec, isProcent, isAnim);
                itemStats.gameObject.SetActive(true);
                return;
            }
        }
    }

    //включаем заголовок
    private void IsEnableTitlePanel(bool value)
    {
        titlePanel.SetActive(value);
        if (value) 
        { 
            IsEnableItemStats(false); 
            IsEnableItemName(false);
        }
    }

    //включаем только имя предмета
    private void IsEnableItemName(bool value)
    {
        itemNamePanel.SetActive(value);
        if (value)
        {
            IsEnableItemStats(false); 
            IsEnableTitlePanel(false);
        }
    }

    //включаем описание айтема
    private void IsEnableItemStats(bool value)
    {
        if (value) 
        { 
            IsEnableTitlePanel(false);
            IsEnableItemName(false);

            //включаем характеристики
            itemStatsView.SetActive(true);
        }
        itemStatsBtnsPanel.SetActive(value);
    }

    //отображаем имя айтема
    private void SetItemName(string currName)
    {
        itemName.text = currName;
        itemNamePanelTxt.text = currName;
    }

    //открыли инвентарь - закрываем все панели на кнопках
    public void OpenInventory()
    {
        //включаем статы персонажа
        IsEnableTitlePanel(true);
        //выключаем все панели подсказок
        foreach (StatBtn statBtn in statBtns)
        {
            statBtn.DisableDescribePanel();
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
}
