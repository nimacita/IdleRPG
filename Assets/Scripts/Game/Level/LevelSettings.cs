using UnityEngine;

[System.Serializable]
public struct LevelAddStats
{
    [Tooltip("�������������� ���� � ���� ��� ���������� ������� ������")]
    public int addSwordDamage;
    [Tooltip("�������������� ���� � ���� ��� ���������� ������� ������")]
    public int addBowDamage;
    [Tooltip("�������������� �������� ��� ���������� ������� ������")]
    public int addHealth;
    [Tooltip("�������������� ����� ��� ���������� ������� ������")]
    public int addArmor;
    [Tooltip("�������������� ����� ��� ���������� ������� ������")]
    public int addLuck;
}

[System.Serializable]
public struct Level
{
    public string levelName;
    [Tooltip("���� ����������� ��� �������� �� ��������� �������")]
    public int neededExp;
    [Tooltip("������������ �������������� �� ������ ������")]
    public LevelAddStats levelStats;
    [Header("Skill Points")]
    [Tooltip("���������� ����� �������� ������ ���������� �� ������ ������")]
    public int pointsCount;
}

[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/Levels")]
public class LevelSettings : ScriptableObject
{

    [Header("Levels")]
    [Tooltip("��� ��������� ������")]
    public Level[] Levels;

}
