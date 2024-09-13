using UnityEngine;

[System.Serializable]
public struct LevelAddStats
{
    [Tooltip("Дополнительный урон с меча при достижении данного уровня")]
    public int addSwordDamage;
    [Tooltip("Дополнительный урон с лука при достижении данного уровня")]
    public int addBowDamage;
    [Tooltip("Дополнительное здоровье при достижении данного уровня")]
    public int addHealth;
    [Tooltip("Дополнительная броня при достижении данного уровня")]
    public int addArmor;
    [Tooltip("Дополнительная удача при достижении данного уровня")]
    public int addLuck;
}

[System.Serializable]
public struct Level
{
    public string levelName;
    [Tooltip("Опыт необходимый для перехода на следующий уровень")]
    public int neededExp;
    [Tooltip("Прибавляемые характеристики на данном уровне")]
    public LevelAddStats levelStats;
    [Header("Skill Points")]
    [Tooltip("Количество Очков прочкачи умений выдаваемое на данном уровне")]
    public int pointsCount;
}

[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/Levels")]
public class LevelSettings : ScriptableObject
{

    [Header("Levels")]
    [Tooltip("Все достпуные уровни")]
    public Level[] Levels;

}
