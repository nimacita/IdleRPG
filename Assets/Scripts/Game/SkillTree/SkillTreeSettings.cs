using UnityEngine;


[System.Serializable]
public struct SkillOnTree
{
    [Tooltip("’арактеристика дл€ улучшени€")]
    public AllPlayerStatsType skillType;
    [Tooltip("ѕрибавл€емый бонус выбранной характеристики")]
    public float skillBonus;
}

[System.Serializable]
public struct SkillBranch
{
    public string branchName;
    [Tooltip("¬се умени€ на данной ветке, прокачка каждого следущего умени€, " +
        "возможна при активации предедущего")]
    public SkillOnTree[] branchSkills;
}


[CreateAssetMenu(fileName = "SkillTreeSettings", menuName = "ScriptableObjects/SkillTreeSettings")]
public class SkillTreeSettings : ScriptableObject
{
    [Header("Main Skill Tree Settings")]
    [Tooltip("¬се доступные ветки с умени€ми на дереве скиллов")]
    public SkillBranch[] skillTreeBranches;
}
