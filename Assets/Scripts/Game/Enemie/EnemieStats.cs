using UnityEngine;

[System.Serializable]
public struct Loot
{
    [Tooltip("Сам предмет выпадаемого лута")]
    public MyInventoryItem lootObj;
    [Tooltip("Шанс выпадения именного этого предмета")]
    [Range(0,100)]
    public int lootChance;
}

[CreateAssetMenu(fileName = "EnemieStats", menuName = "ScriptableObjects/Enemie")]
public class EnemieStats : ScriptableObject
{
    [Header("Main Stats")]
    [Space(10)]
    [Header("Health And Armor")]
    [Tooltip("Основное - начальное здоровье врага")]
    public int baseHealth = 5;
    [Tooltip("Основная - начальная броня врага, снижает урон в процентах")]
    public int baseArmor = 0;

    [Space(10)]
    [Header("Attack Stats")]
    [Tooltip("Основной - начальный урон врага")]
    public int baseAttackDamage = 1;
    [Tooltip("Длительность самого удара в секундах")]
    public float attackDurability = 2f;
    [Tooltip("Длительность подготовки перед ударом в секундах")]
    public float readyAtckCoolDown = 3f;
    [Tooltip("Дальний тип атаки или ближний (False - ближний)")]
    public bool rangeAttack = false;

    [Space(10)]
    [Header("Exp Settings")]
    [Tooltip("Количество выпадаемого опыта с врага")]
    [Range(1,99)]
    public int experienceAmount;

    [Space(10)]
    [Header("Spawn Chance")]
    [Tooltip("Шанс спавна данного врага в процентах")]
    [Range(0,100)]
    public int spawnChance = 50;

    [Space(10)]
    [Header("Loot Settings")]
    [Tooltip("Шанс выпадения любого лута из врага в процентах")]
    [Range(0, 100)]
    public int anyLootCahance = 50;
    [Tooltip("Весь возможный лут, выпадаемый с данного врага")]
    public Loot[] allLoots;

    [Space(10)]
    [Header("Sounds Settings")]
    [Tooltip("Звук удара текущего врага")]
    public AudioClip enemieHitSound;

}
