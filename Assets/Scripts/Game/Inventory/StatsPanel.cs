using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��� �������������� ���������
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

//��������� ��� ������ � �������������
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
    [Tooltip("��� ������ ������������� ���������")]
    public StatBtn[] statBtns;

    [Header("Item Stats Btn")]
    [Tooltip("��� ������ ������������� ��������")]
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

    //��������� ������������ ��������������
    public void UpdateStatsBtns()
    {
        //���������� �� ���� ������� � ���������� �������������� � ����������� �� ����
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

    //������������� ������ ��� ���� �������
    public StatText GetStatsTexts(AllPlayerStatsType statType)
    {
        StatText statText = new StatText();

        //���������� ������ ����� � ����������� �� ����
        switch (statType)
        {
            case AllPlayerStatsType.SwordDamage:
                statText.StatTitle = "���� �����";
                statText.StatDescription = "���� ��������� ����� � ����������� �� ����������";
                statText.StatMeleeDamage = currentStats.currSwordAttackDamage;
                statText.StatRangeDamage = currentStats.currSwordAttackDamage * currentStats.currSwordDamageToRangeKoef;
                statText.StatCritDamage = currentStats.currSwordAttackDamage + currentStats.GetCurrSwordAddCriteDmg;
                break;
            case AllPlayerStatsType.BowDamage:
                statText.StatTitle = "���� �����";
                statText.StatDescription = "���� ��������� ����� � ����������� �� ����������";
                statText.StatMeleeDamage = currentStats.currBowAttackDamage * currentStats.currBowDamageToMeleeKoef;
                statText.StatRangeDamage = currentStats.currBowAttackDamage;
                statText.StatCritDamage = currentStats.currBowAttackDamage + currentStats.GetCurrBowAddCriteDmg;
                break;
            case AllPlayerStatsType.Health:
                statText.StatTitle = "���������� �������� ���������";
                statText.StatDescription = "������������ ���������� �������� ���������";
                break;
            case AllPlayerStatsType.Armor:
                statText.StatTitle = "������ ��������� � ���������";
                statText.StatDescription = "��������� ���������� ���� � ���������";
                break;
            case AllPlayerStatsType.Luck:
                statText.StatTitle = "����� ��������� � ���������";
                statText.StatDescription = "����������� ���� ������������ ����� � ���������";
                break;
            case AllPlayerStatsType.ChargeTime:
                statText.StatTitle = "������������ ������ ������";
                statText.StatDescription = "����� ����������� �� ������ ������ � ��������";
                break;
            case AllPlayerStatsType.ReadyTime:
                statText.StatTitle = "������������ ���������� � �����";
                statText.StatDescription = "����� ����������� ��� ���������� � ����� � ��������";
                break;
            case AllPlayerStatsType.SwordAtckTime:
                statText.StatTitle = "������������ ����� �����";
                statText.StatDescription = "����� ����������� ��� ���������� ����� ����� � ��������";
                break;
            case AllPlayerStatsType.BowAtckTime:
                statText.StatTitle = "������������ ����� �����";
                statText.StatDescription = "����� ����������� ��� ���������� ����� ����� � ��������";
                break;
        }

        return statText;
    }

    //����� ����� � ������� ��������� �� �����
    public void TakeItem(MyInventoryItem takedItem)
    {
        //������������� ��� ������
        SetItemName(takedItem.itemName);

        //���� ������ ���, �� ��������� ���� ������ ��������
        if (IsWeapon(takedItem) || IsArmor(takedItem))
        {
            DefineOpenItemStats(takedItem);
            IsEnableItemStats(true);
        }
        //���� ����� �� �������� ������ ���������� ��������� ��
        else if (takedItem.itemType == InventoryItemType.HealthPotion)
        {
            DefineHealthPoitionStats(takedItem);
            IsEnableItemStats(true);
        }
        else
        {
            //���� �� ����� � �� ������ �� �������� ������ ���
            IsEnableItemName(true);
        }
    }

    //��������� ������� - ��������� ����� �������� ���� ��������
    public void DropItem()
    {
        //���� �������� ����� �������� - �� ��������� ����� ������ � �������� ���������
        if (itemStatsBtnsPanel.activeSelf || itemNamePanel.activeSelf)
        {
            IsEnableTitlePanel(true);
        }
    }

    //�������� ������ ������ ����� ���������, � ����������� �� ��������
    private void DefineOpenItemStats(MyInventoryItem takedItem)
    {
        //��� ������ ��������� ��� �����
        foreach(StatBtn itemStats in statItemsBtns)
        {
            itemStats.gameObject.SetActive(false);
        }

        //�������� ����������� ������������� � ����������� �� ���� ��������
        //� ���������� � ��� ������ �����
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

    //�������� ����������� ������ ��� �����
    private void DefineHealthPoitionStats(MyInventoryItem takedItem)
    {
        //��� ������ ��������� ��� �����
        foreach (StatBtn itemStats in statItemsBtns)
        {
            itemStats.gameObject.SetActive(false);
        }

        //�������� ����������� ������������� � ����������� �� ���� ��������
        //� ���������� � ��� ������ �����
        EnableSelectedStat(AllPlayerStatsType.Health, takedItem.poitionHealthAmount);
    }

    //�������� ������ �������������� �� ������ ��������
    private void EnableSelectedStat(AllPlayerStatsType selectedStatType, 
        float currStatValue, bool isSec = false, bool isProcent = false, bool isAnim = false)
    {
        //����������� �� ���� ����� � �������� ������
        foreach (StatBtn itemStats in statItemsBtns)
        {
            if (itemStats.currentStatType == selectedStatType)
            {
                //���������� � ���� ������ ��������������
                itemStats.SetStat(currStatValue, isSec, isProcent, isAnim);
                itemStats.gameObject.SetActive(true);
                return;
            }
        }
    }

    //�������� ���������
    private void IsEnableTitlePanel(bool value)
    {
        titlePanel.SetActive(value);
        if (value) 
        { 
            IsEnableItemStats(false); 
            IsEnableItemName(false);
        }
    }

    //�������� ������ ��� ��������
    private void IsEnableItemName(bool value)
    {
        itemNamePanel.SetActive(value);
        if (value)
        {
            IsEnableItemStats(false); 
            IsEnableTitlePanel(false);
        }
    }

    //�������� �������� ������
    private void IsEnableItemStats(bool value)
    {
        if (value) 
        { 
            IsEnableTitlePanel(false);
            IsEnableItemName(false);

            //�������� ��������������
            itemStatsView.SetActive(true);
        }
        itemStatsBtnsPanel.SetActive(value);
    }

    //���������� ��� ������
    private void SetItemName(string currName)
    {
        itemName.text = currName;
        itemNamePanelTxt.text = currName;
    }

    //������� ��������� - ��������� ��� ������ �� �������
    public void OpenInventory()
    {
        //�������� ����� ���������
        IsEnableTitlePanel(true);
        //��������� ��� ������ ���������
        foreach (StatBtn statBtn in statBtns)
        {
            statBtn.DisableDescribePanel();
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
}
