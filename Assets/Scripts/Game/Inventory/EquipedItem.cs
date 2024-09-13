using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EquipedTypes
{
    Default,
    Helmet,
    Torso,
    Gloves,
    Boots,
    SwordWeapon,
    BowWeapon
}

public class EquipedItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("EquipedType")]
    [SerializeField] private EquipedTypes equipedType;
    [SerializeField] private Sprite currMiniIcon;
    [SerializeField] private int equipedInd = -1;

    [Header("AllItems")]
    private AllInventoryItems allItems;

    [Header("PlayerCurrentStats")]
    [SerializeField] private PlayerCurrentStats playerCurrentStats;

    [Header("Components")]
    [SerializeField] private Image item;
    [SerializeField] private Image miniIconImg;
    [SerializeField] private GameObject itemLightning;

    [Header("Debug")]
    //Current Item
    [SerializeField] private MyInventoryItem currentEquipItem = null;

    //bools
    private bool isFree = false;
    [SerializeField]
    private bool isSelected = false;

    //Stats
    private int armor = 0; 
    private int health = 0; 
    private int luck = 0; 
    private int swordDamage = 0; 
    private int bowDamage = 0;
    private float addToReadyAtck = 0f;
    private float swordAttackDurabilty = 0f;
    private float bowAttackDurabilty = 0f;


    void Start()
    {

    }

    //����������� �������� �������� � ����� �� ��� ������� � �������� ��������
    private string EquipedSlotItem
    {
        get
        {
            if (!PlayerPrefs.HasKey($"EquipedSlotItem{equipedType}"))
            {
                PlayerPrefs.SetString($"EquipedSlotItem{equipedType}", "");
            }
            return PlayerPrefs.GetString($"EquipedSlotItem{equipedType}");
        }
        set
        {
            PlayerPrefs.SetString($"EquipedSlotItem{equipedType}", value);
        }
    }

    //��������� ����� � �����, ���� ����
    public void UpdateItemInEquip()
    {
        if (equipedType != EquipedTypes.Default)
        {
            if (EquipedSlotItem != "")
            {
                //����� ������� �� ������������ �����
                currentEquipItem = TakeItem(EquipedSlotItem);
            }
            else
            {
                currentEquipItem = null;
            }
            //��������� �����������
            UpdateEquipView();
        }
        else
        {
            Debug.LogError("�� ��������� ���");
        }
        SetPlayerItemsSprites();
        SetPlayerItemStats();
    }

    //��������� ����������� �����, � ����������� �� ��������
    private void UpdateEquipView()
    {
        miniIconImg.sprite = currMiniIcon;
        if (currentEquipItem != null)
        {
            //���� �����
            isFree = false;
            miniIconImg.gameObject.SetActive(false);

            //���������� ������
            item.sprite = currentEquipItem.itemIcon;
            itemLightning.SetActive(true);
            item.gameObject.SetActive(true);
        }
        else
        {
            //���� �� �����
            isFree = true;
            itemLightning.SetActive(false);
            miniIconImg.gameObject.SetActive(true);
            item.gameObject.SetActive(false);
        }
    }

    //������ �������, ���� null - �� ������� �������
    public void SetItem(MyInventoryItem itm = null)
    {
        if (itm != null)
        {
            if (IsCorrectItem(itm)) 
            {
                //������ ������� ���� ��������
                if (isFree && currentEquipItem == null)
                {
                    currentEquipItem = itm;
                    EquipedSlotItem = itm.itemName;
                }
                else
                {
                    Debug.LogError($"���� {equipedType} �����, �������� ������� ����������");
                }
            }
            else
            {
                Debug.LogError($"� ���� {equipedType} ���������� ��������� {itm.itemType} ��� ��������");
            }
        }
        else
        {
            //������� �������
            EquipedSlotItem = "";
            currentEquipItem = null;
        }
        //��������� �����������
        UpdateItemInEquip();
        //��������� ��������������
        InventoryController.instance.SetAllEquipedStats();
    }

    //��������� ������ �������� � ����������� �� �������������
    private void SetPlayerItemsSprites()
    {
        //��������� �������� � ����������� �� �������������
        switch (equipedType)
        {
            case EquipedTypes.SwordWeapon:
                {
                    if (currentEquipItem != null)
                    {
                        playerCurrentStats.currentSwordSprite = currentEquipItem.swordInHandSprite;
                    }
                    else
                    {
                        playerCurrentStats.currentSwordSprite = null;
                    }
                    break;
                }
            case EquipedTypes.BowWeapon:
                {
                    if (currentEquipItem != null)
                    {
                        playerCurrentStats.currentBowArrowSprite = currentEquipItem.bowArrowInHandSprite;
                        playerCurrentStats.currentBowLimbSprite = currentEquipItem.bowLimbInHandSprite;
                        playerCurrentStats.currentBowRiserSprite = currentEquipItem.bowRiserInHandSprite;
                    }
                    else
                    {
                        playerCurrentStats.currentBowArrowSprite = null;
                        playerCurrentStats.currentBowLimbSprite = null;
                        playerCurrentStats.currentBowRiserSprite = null;
                    }
                    break;
                }
            case EquipedTypes.Helmet:
                {
                    if(currentEquipItem != null)
                    {
                        playerCurrentStats.currentHelmetSprite = currentEquipItem.helmetOnCharacterSprite;
                    }
                    else
                    {
                        playerCurrentStats.currentHelmetSprite = null;
                    }
                    break;
                }
            case EquipedTypes.Gloves:
                {
                    if (currentEquipItem != null)
                    {
                        playerCurrentStats.currentFingerSprite = currentEquipItem.glovesFinger;
                        playerCurrentStats.currentForearmLSprite = currentEquipItem.glovesForearmL;
                        playerCurrentStats.currentForearmRSprite = currentEquipItem.glovesForearmR;
                        playerCurrentStats.currentHandLSprite = currentEquipItem.glovesHandL;
                        playerCurrentStats.currentHandRSprite = currentEquipItem.glovesHandR;
                        playerCurrentStats.currentSleeverSprite = currentEquipItem.glovesSleever;
                    }
                    else
                    {
                        playerCurrentStats.currentFingerSprite = null;
                        playerCurrentStats.currentForearmLSprite = null;
                        playerCurrentStats.currentForearmRSprite = null;
                        playerCurrentStats.currentHandLSprite = null;
                        playerCurrentStats.currentHandRSprite = null;
                        playerCurrentStats.currentSleeverSprite = null;
                    }
                    break;
                }
            case EquipedTypes.Torso:
                {
                    if (currentEquipItem != null)
                    {
                        playerCurrentStats.currentArmLSprite = currentEquipItem.torsoArmL;
                        playerCurrentStats.currentArmRSprite = currentEquipItem.torsoArmR;
                        playerCurrentStats.currentPelvisSprite = currentEquipItem.torsoPelvis;
                        playerCurrentStats.currentTorsoSprite = currentEquipItem.torsoTorso;
                    }
                    else
                    {
                        playerCurrentStats.currentArmLSprite = null;
                        playerCurrentStats.currentArmRSprite = null;
                        playerCurrentStats.currentPelvisSprite = null;
                        playerCurrentStats.currentTorsoSprite = null;
                    }
                    break;
                }
            case EquipedTypes.Boots:
                {
                    if(currentEquipItem != null)
                    {
                        playerCurrentStats.curretnLegSprite = currentEquipItem.bootsLeg;
                        playerCurrentStats.curretnShinSprite = currentEquipItem.bootsShin;
                    }
                    else
                    {
                        playerCurrentStats.curretnLegSprite = null;
                        playerCurrentStats.curretnShinSprite = null;
                    }
                    break;
                }
        }
        //��������� ����������� ���������
        PlayerController.instance.UpdatePlayerView();
    }

    //��������� �������������� ��������� � ����������� �� �������������
    private void SetPlayerItemStats()
    {
        //��������� �������� � ����������� �� �������������
        switch (equipedType)
        {
            case EquipedTypes.SwordWeapon:
                {
                    if (currentEquipItem != null)
                    {
                        //���������� � �����
                        swordDamage = currentEquipItem.weaponDamage;
                        //������ ������������ �����
                        swordAttackDurabilty = currentEquipItem.swordAttackDurability - playerCurrentStats.startSwordAttackDurability;
                        //���������� � ����� (���� ����)
                        luck = currentEquipItem.weaponLuck;

                        //������ �����
                        playerCurrentStats.currSwordSwingSound = currentEquipItem.swordSwingSound;
                        playerCurrentStats.currSwordHitSound = currentEquipItem.swordHitSound;
                    }
                    else
                    {
                        //���� ����� - �������� ������������ �������
                        swordDamage = 0;
                        swordAttackDurabilty = 0;
                        luck = 0;

                        //������� �����
                        playerCurrentStats.currSwordSwingSound = null;
                        playerCurrentStats.currSwordHitSound = null;
                    }
                    break;
                }
            case EquipedTypes.BowWeapon:
                {
                    if(currentEquipItem != null)
                    {
                        //���������� � �����
                        bowDamage = currentEquipItem.weaponDamage;
                        //������ ������������ �����
                        bowAttackDurabilty = currentEquipItem.bowAttackDurability - playerCurrentStats.startBowAttackDurability;
                        //���������� � ����� (���� ����)
                        luck = currentEquipItem.weaponLuck;

                        //������ �����
                        playerCurrentStats.currBowShotSound = currentEquipItem.bowShotSound;
                        playerCurrentStats.currBowDrawSound = currentEquipItem.bowDrawSound;
                    }
                    else
                    {
                        //���� ����� - �������� ������������ ������
                        bowDamage = 0;
                        bowAttackDurabilty = 0;
                        luck = 0;

                        //������� �����
                        playerCurrentStats.currBowShotSound = null;
                        playerCurrentStats.currBowDrawSound = null;
                    }
                    break;
                }
            case EquipedTypes.Helmet:
            case EquipedTypes.Gloves:
            case EquipedTypes.Torso:
            case EquipedTypes.Boots:
                {
                    if (currentEquipItem != null)
                    {
                        //���������� � �����
                        armor = currentEquipItem.armorArmor;
                        //���������� � ��
                        health = currentEquipItem.armorHealth;
                        //���������� � ����� (���� ����)
                        luck = currentEquipItem.armorLuck;
                        //���������� (��� ��������) � �������� ���������� �����
                        addToReadyAtck = currentEquipItem.armorAddReadyToAtck;

                        //���� ��� ����, �� ������ ���� �����
                        if (equipedType == EquipedTypes.Torso)
                        {
                            playerCurrentStats.currArmorBlowSound = currentEquipItem.armorBlowSound;
                        }
                    }
                    else
                    {
                        //���� ����� - �������� �������
                        armor = 0;
                        health = 0;
                        luck = 0;
                        addToReadyAtck = 0f;

                        //���� ��� ����, �� ������� ���� �����
                        if (equipedType == EquipedTypes.Torso)
                        {
                            playerCurrentStats.currArmorBlowSound = null;
                        }
                    }
                    break;
                }
        }
        //��������� �������������� ���������
        //PlayerController.instance.UpdateCurrentStats();
        //StatsPanel.instance.UpdateStatsBtns();
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
    public void IsEquipDrag()
    {
        //���� ���� ��� �����
        if (!isFree && currentEquipItem != null)
        {
            InventoryController.instance.SelectEquiped(equipedInd);
            DisableEquipItem();
        }
    }

    //��� �� ��� ��� ��������, ������� ������
    public bool IsCorrectItem(MyInventoryItem itm)
    {
        switch (itm.itemType)
        {
            case InventoryItemType.Helmet:
                if (equipedType == EquipedTypes.Helmet) return true;
                else return false;
            case InventoryItemType.Torso:
                if (equipedType == EquipedTypes.Torso) return true;
                else return false;
            case InventoryItemType.Gloves:
                if (equipedType == EquipedTypes.Gloves) return true;
                else return false;
            case InventoryItemType.Boots:
                if (equipedType == EquipedTypes.Boots) return true;
                else return false;
            case InventoryItemType.SwordWeapon:
                if (equipedType == EquipedTypes.SwordWeapon) return true;
                else return false;
            case InventoryItemType.BowWeapon:
                if (equipedType == EquipedTypes.BowWeapon) return true;
                else return false;
            default:
                return false;
        }
    }

    //��������� ������� ���������
    public void DisableEquipItem()
    {
        miniIconImg.gameObject.SetActive(true);
        item.gameObject.SetActive(false);
        itemLightning.SetActive(false);
    }

    //�������� �������
    public void EnableItem()
    {
        UpdateEquipView();
    }

    public void SetAllItems(AllInventoryItems allInventoryItems)
    {
        allItems = allInventoryItems;
    }

    public void SetInd(int index)
    {
        equipedInd = index;
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

    public EquipedTypes GetEquipType()
    {
        return equipedType;
    }

    public MyInventoryItem GetCurrItem()
    {
        return currentEquipItem;
    }

    public int Armor { get { return armor; } }
    public int Health { get { return health; } }
    public int Luck { get { return luck; } }
    public int SwordDamage { get { return swordDamage; } }
    public int BowDamage { get { return bowDamage; } }
    public float ReadyAttack { get { return addToReadyAtck; } }

    public float SwordAttackDurability { get { return swordAttackDurabilty; } }
    public float BowAttackDurability { get { return bowAttackDurabilty; } }
}
