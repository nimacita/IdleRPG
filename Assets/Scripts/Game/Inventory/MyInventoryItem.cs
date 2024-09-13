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
    [Tooltip("Название предмета, по которому он будет сохарняться в инвенторе")]
    public string itemName;
    [Tooltip("Иконка предмета, отображаемая в инвенторе")]
    public Sprite itemIcon;

    [Header("Item Type")]
    [Tooltip("Тип предмета в инвенторе, зелье оружие, элемент брони, или обычный предмет")]
    public InventoryItemType itemType;

    [Header("ItemSettings")]
    [Tooltip("Цена предмета для продажи")]
    [Range(0,9999)]
    public int itemPrice = 0;

    //для зелья
    [Header("Health Poition Settings")]
    [Tooltip("Количество здоровья исцеляемое зельем")]
    [Range(1,999)]
    public int poitionHealthAmount = 1;

    //для брони
    [Header("Armor Settings")]
    [Tooltip("Количество прибавляемой брони")]
    [Range(0, 100)]
    public int armorArmor;
    [Tooltip("Количество приблвяемого здоровья")]
    [Range(0, 999)]
    public int armorHealth;
    [Tooltip("Количество прибалвяемой удачи")]
    [Range(0, 100)]
    public int armorLuck;
    [Tooltip("Количество прибавляемых, или отнимаемых, к скорости подготовки атаки, секунд")]
    [Range(-5f, 100f)]
    public float armorAddReadyToAtck;

    //для шлема
    [Header("Helmet Settings")]
    [Tooltip("Спрайт шлема на персонаже")]
    public Sprite helmetOnCharacterSprite;

    //для перчаток
    [Header("Gloves Settings")]
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesFinger;
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesForearmR;
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesForearmL;
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesHandL;
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesHandR;
    [Tooltip("Составной спрайт Перчаток на руках персонажа")]
    public Sprite glovesSleever;

    //для нагрудника
    [Header("Torso Settings")]
    [Tooltip("Составной спрайт Нагрудника на персонажа")]
    public Sprite torsoArmL;
    [Tooltip("Составной спрайт Нагрудника на персонажа")]
    public Sprite torsoArmR;
    [Tooltip("Составной спрайт Нагрудника на персонажа")]
    public Sprite torsoPelvis;
    [Tooltip("Составной спрайт Нагрудника на персонажа")]
    public Sprite torsoTorso;
    [Header("Blow to Armor Sound")]
    [Tooltip("Звук удара по текущей броне")]
    public AudioClip armorBlowSound;

    //для ботинок
    [Header("Boots Settings")]
    [Tooltip("Составной спрайт Ботинок на персонажа")]
    public Sprite bootsLeg;
    [Tooltip("Составной спрайт Ботинок на персонажа")]
    public Sprite bootsShin;

    //для оружия
    [Header("Weapon Settings")]
    [Tooltip("Прибалвляемый урон от оружия")]
    [Range(0, 999)]
    public int weaponDamage;
    [Tooltip("Количество прибалвяемой удачи")]
    [Range(0, 100)]
    public int weaponLuck;

    //для меча
    [Tooltip("Длительность атаки мечом, без учета бонусов")]
    [Range(0.1f, 50f)]
    public float swordAttackDurability = 0.1f;
    [Tooltip("Спрайт меча в руке у персонажа")]
    public Sprite swordInHandSprite;
    [Tooltip("Звук взмаха данным мечом")]
    public AudioClip swordSwingSound;
    [Tooltip("Звук удара данным мечом")]
    public AudioClip swordHitSound;

    //для лука
    [Tooltip("Длительность атаки луком, без учета бонусов")]
    [Range(0.1f, 50f)]
    public float bowAttackDurability = 0.1f;
    [Tooltip("Составной спрайт лука в руке у персонажа")]
    public Sprite bowArrowInHandSprite;
    [Tooltip("Составной спрайт лука в руке у персонажа")]
    public Sprite bowLimbInHandSprite;
    [Tooltip("Составной спрайт лука в руке у персонажа")]
    public Sprite bowRiserInHandSprite;
    [Tooltip("Звуквыстрела из лука")]
    public AudioClip bowShotSound;
    [Tooltip("Звук натягивания титевы")]
    public AudioClip bowDrawSound;

}
