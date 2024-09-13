using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/Player/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Main Base Stats")]
    [Space(10)]
    [Header("Health And Armor")]
    [Tooltip("�������� - ��������� �������� ���������")]
    public int baseHealth = 10;
    [Tooltip("�������� - ��������� ����� ���������, ������� ���� � ���������")]
    public int baseArmor = 0;

    [Space(10)]
    [Header("Attack Stats")]
    [Tooltip("�������� - ��������� ���� ��������� � ����")]
    public int baseSwordAttackDamage = 1;
    [Tooltip("�������� - ��������� ���� ��������� � ����")]
    public int baseBowAttackDamage = 1;
    [Tooltip("������� ������������ ���������� ����� ������ � ��������")]
    public float readyAtckCoolDown = 2.5f;
    [Tooltip("����������� ������������ ���������� ����� ������ � ��������")]
    public float minReadyToAtckCoolDown = 0.5f;

    [Space(2)]
    [Header("Swords Settings")]
    [Tooltip("���������� ����� ���� �� ������� ������")]
    public float swordDamageToRangeKoef = 0f;
    [Tooltip("����������� ������������ ����� �����")]
    public float minSwordAttackDurability = 0.25f;
    [Tooltip("������� ��������� �����, ������������ � �������� ����� � ������ ���� ����� �����")]
    public float baseSwordCritKoef = 0.5f;

    [Space(2)]
    [Header("Bow Settings")]
    [Tooltip("���������� ����� ���� �� ������� ������")]
    public float bowDamageToMeleeKoef = 0.25f;
    [Tooltip("����������� ������������ ����� �����")]
    public float minBowAttackDurability = 0.25f;
    [Tooltip("������� ��������� �����, ������������ � �������� ����� � ������ ���� ����� �����")]
    public float baseBowCritKoef = 0.5f;

    [Space(10)]
    [Header("Luck Stat")]
    [Tooltip("������� ����� - ���� ���� ���������")]
    public int baseLuck;

    [Space(10)]
    [Header("Weapon Stats")]
    [Tooltip("������������ ����� ������ � ��������")]
    public float changeDurability = 2f;

    [Space(10)]
    [Header("Sounds Stats")]
    [Tooltip("������ ������ ��������� �����")]
    public AudioClip[] takeDamageSounds;
    [Header("Death Sound")]
    [Tooltip("���� ������ ������")]
    public AudioClip deathSound;
    [Header("Change Weapon Sound")]
    [Tooltip("���� ����� ������")]
    public AudioClip changeWeaponSound;

}
