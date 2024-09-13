using UnityEngine;


public enum InventoryItemType
{
    Item,
    HealthPotion,
    Helmet,
    Torso,
    Gloves,
    Boots,
    SwordWeapon,
    BowWeapon
}

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/InventoryItem")]
public class MyInventoryItem : ScriptableObject
{
    [Header("Visual")]
    [Tooltip("�������� ��������, �� �������� �� ����� ����������� � ���������")]
    public string itemName;
    [Tooltip("������ ��������, ������������ � ���������")]
    public Sprite itemIcon;

    [Header("Item Type")]
    [Tooltip("��� �������� � ���������, ����� ������, ������� �����, ��� ������� �������")]
    public InventoryItemType itemType;

    [Header("ItemSettings")]
    [Tooltip("���� �������� ��� �������")]
    [Range(0,9999)]
    public int itemPrice = 0;

    //��� �����
    [Header("Health Poition Settings")]
    [Tooltip("���������� �������� ���������� ������")]
    [Range(1,999)]
    public int poitionHealthAmount = 1;

    //��� �����
    [Header("Armor Settings")]
    [Tooltip("���������� ������������ �����")]
    [Range(0, 100)]
    public int armorArmor;
    [Tooltip("���������� ������������ ��������")]
    [Range(0, 999)]
    public int armorHealth;
    [Tooltip("���������� ������������ �����")]
    [Range(0, 100)]
    public int armorLuck;
    [Tooltip("���������� ������������, ��� ����������, � �������� ���������� �����, ������")]
    [Range(-5f, 100f)]
    public float armorAddReadyToAtck;

    //��� �����
    [Header("Helmet Settings")]
    [Tooltip("������ ����� �� ���������")]
    public Sprite helmetOnCharacterSprite;

    //��� ��������
    [Header("Gloves Settings")]
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesFinger;
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesForearmR;
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesForearmL;
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesHandL;
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesHandR;
    [Tooltip("��������� ������ �������� �� ����� ���������")]
    public Sprite glovesSleever;

    //��� ����������
    [Header("Torso Settings")]
    [Tooltip("��������� ������ ���������� �� ���������")]
    public Sprite torsoArmL;
    [Tooltip("��������� ������ ���������� �� ���������")]
    public Sprite torsoArmR;
    [Tooltip("��������� ������ ���������� �� ���������")]
    public Sprite torsoPelvis;
    [Tooltip("��������� ������ ���������� �� ���������")]
    public Sprite torsoTorso;
    [Header("Blow to Armor Sound")]
    [Tooltip("���� ����� �� ������� �����")]
    public AudioClip armorBlowSound;

    //��� �������
    [Header("Boots Settings")]
    [Tooltip("��������� ������ ������� �� ���������")]
    public Sprite bootsLeg;
    [Tooltip("��������� ������ ������� �� ���������")]
    public Sprite bootsShin;

    //��� ������
    [Header("Weapon Settings")]
    [Tooltip("������������� ���� �� ������")]
    [Range(0, 999)]
    public int weaponDamage;
    [Tooltip("���������� ������������ �����")]
    [Range(0, 100)]
    public int weaponLuck;

    //��� ����
    [Tooltip("������������ ����� �����, ��� ����� �������")]
    [Range(0.1f, 50f)]
    public float swordAttackDurability = 0.1f;
    [Tooltip("������ ���� � ���� � ���������")]
    public Sprite swordInHandSprite;
    [Tooltip("���� ������ ������ �����")]
    public AudioClip swordSwingSound;
    [Tooltip("���� ����� ������ �����")]
    public AudioClip swordHitSound;

    //��� ����
    [Tooltip("������������ ����� �����, ��� ����� �������")]
    [Range(0.1f, 50f)]
    public float bowAttackDurability = 0.1f;
    [Tooltip("��������� ������ ���� � ���� � ���������")]
    public Sprite bowArrowInHandSprite;
    [Tooltip("��������� ������ ���� � ���� � ���������")]
    public Sprite bowLimbInHandSprite;
    [Tooltip("��������� ������ ���� � ���� � ���������")]
    public Sprite bowRiserInHandSprite;
    [Tooltip("������������ �� ����")]
    public AudioClip bowShotSound;
    [Tooltip("���� ����������� ������")]
    public AudioClip bowDrawSound;

}
