using UnityEngine;


[System.Serializable]
public struct SkillOnTree
{
    [Tooltip("�������������� ��� ���������")]
    public AllPlayerStatsType skillType;
    [Tooltip("������������ ����� ��������� ��������������")]
    public float skillBonus;
}

[System.Serializable]
public struct SkillBranch
{
    public string branchName;
    [Tooltip("��� ������ �� ������ �����, �������� ������� ��������� ������, " +
        "�������� ��� ��������� �����������")]
    public SkillOnTree[] branchSkills;
}


[CreateAssetMenu(fileName = "SkillTreeSettings", menuName = "ScriptableObjects/SkillTreeSettings")]
public class SkillTreeSettings : ScriptableObject
{
    [Header("Main Skill Tree Settings")]
    [Tooltip("��� ��������� ����� � �������� �� ������ �������")]
    public SkillBranch[] skillTreeBranches;
}
