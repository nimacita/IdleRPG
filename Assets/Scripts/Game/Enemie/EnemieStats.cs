using UnityEngine;

[System.Serializable]
public struct Loot
{
    [Tooltip("��� ������� ����������� ����")]
    public MyInventoryItem lootObj;
    [Tooltip("���� ��������� �������� ����� ��������")]
    [Range(0,100)]
    public int lootChance;
}

[CreateAssetMenu(fileName = "EnemieStats", menuName = "ScriptableObjects/Enemie")]
public class EnemieStats : ScriptableObject
{
    [Header("Main Stats")]
    [Space(10)]
    [Header("Health And Armor")]
    [Tooltip("�������� - ��������� �������� �����")]
    public int baseHealth = 5;
    [Tooltip("�������� - ��������� ����� �����, ������� ���� � ���������")]
    public int baseArmor = 0;

    [Space(10)]
    [Header("Attack Stats")]
    [Tooltip("�������� - ��������� ���� �����")]
    public int baseAttackDamage = 1;
    [Tooltip("������������ ������ ����� � ��������")]
    public float attackDurability = 2f;
    [Tooltip("������������ ���������� ����� ������ � ��������")]
    public float readyAtckCoolDown = 3f;
    [Tooltip("������� ��� ����� ��� ������� (False - �������)")]
    public bool rangeAttack = false;

    [Space(10)]
    [Header("Exp Settings")]
    [Tooltip("���������� ����������� ����� � �����")]
    [Range(1,99)]
    public int experienceAmount;

    [Space(10)]
    [Header("Spawn Chance")]
    [Tooltip("���� ������ ������� ����� � ���������")]
    [Range(0,100)]
    public int spawnChance = 50;

    [Space(10)]
    [Header("Loot Settings")]
    [Tooltip("���� ��������� ������ ���� �� ����� � ���������")]
    [Range(0, 100)]
    public int anyLootCahance = 50;
    [Tooltip("���� ��������� ���, ���������� � ������� �����")]
    public Loot[] allLoots;

    [Space(10)]
    [Header("Sounds Settings")]
    [Tooltip("���� ����� �������� �����")]
    public AudioClip enemieHitSound;

}
