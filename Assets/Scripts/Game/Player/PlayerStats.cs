using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Main Base Stats")]
    [Space(10)]
    [Header("Health And Armor")]
    [Tooltip("Основное - начальное здоровье персонажа")]
    public int baseHealth = 10;
    [Tooltip("Основная - начальная броня персонажа, снижает урон в процентах")]
    public int baseArmor = 0;

    [Space(10)]
    [Header("Attack Stats")]
    [Tooltip("Основной - начальный урон персонажа с меча")]
    public int baseSwordAttackDamage = 1;
    [Tooltip("Основной - начальный урон персонажа с Лука")]
    public int baseBowAttackDamage = 1;
    [Tooltip("Базовая Длительность подготовки перед ударом в секундах")]
    public float readyAtckCoolDown = 2.5f;
    [Tooltip("Минимальная Длительность подготовки перед ударом в секундах")]
    public float minReadyToAtckCoolDown = 0.5f;

    [Space(2)]
    [Header("Swords Settings")]
    [Tooltip("Коэфициент урона меча по дальним врагам")]
    public float swordDamageToRangeKoef = 0f;
    [Tooltip("Минимальная длительность удара мечом")]
    public float minSwordAttackDurability = 0.25f;
    [Tooltip("базовый Множитель урона, прибавляемый к осовному урону в случае крит удара мечом")]
    public float baseSwordCritKoef = 0.5f;

    [Space(2)]
    [Header("Bow Settings")]
    [Tooltip("Коэфициент урона лука по ближним врагам")]
    public float bowDamageToMeleeKoef = 0.25f;
    [Tooltip("Минимальная длительность удара луком")]
    public float minBowAttackDurability = 0.25f;
    [Tooltip("базовый Множитель урона, прибавляемый к осовному урону в случае крит удара луком")]
    public float baseBowCritKoef = 0.5f;

    [Space(10)]
    [Header("Luck Stat")]
    [Tooltip("Базовая Удача - Крит Шанс персонажа")]
    public int baseLuck;

    [Space(10)]
    [Header("Weapon Stats")]
    [Tooltip("Длительность смены оружия в секундах")]
    public float changeDurability = 2f;

    [Space(10)]
    [Header("Sounds Stats")]
    [Tooltip("Массив звуков получения урона")]
    public AudioClip[] takeDamageSounds;
    [Header("Death Sound")]
    [Tooltip("Звук Смерти игрока")]
    public AudioClip deathSound;
    [Header("Change Weapon Sound")]
    [Tooltip("Звук смены оружия")]
    public AudioClip changeWeaponSound;

}
