using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCurrentStats", menuName = "ScriptableObjects/Player/PlayerCurrentStats")]
public class PlayerCurrentStats : ScriptableObject
{

    [Header("Текущие Значения Персонажа")]
    [Header("READ ONLY")]

    [Space(5)]
    [Header("Характеристики")]
    [Space(2)]
    [Header("Health And Armor")]
    public int currHealth;
    public int currArmor;

    [Space(2)]
    [Header("Attack Stats")]
    public int currSwordAttackDamage;
    public int currBowAttackDamage;
    public float currReadyAtckCoolDown;
    public float currSwordDamageToRangeKoef;
    public float currBowDamageToMeleeKoef;
    public float currSwordCritKoef;
    public float currBowCritKoef;
    public bool isBow = false;
    public bool IsBow
    {
        get
        {
            if (!PlayerPrefs.HasKey("IsBow"))
            {
                PlayerPrefs.SetInt("IsBow", 0);
                isBow = false;
            }
            if (PlayerPrefs.GetInt("IsBow") == 0)
            {
                isBow = false;
                return false;
            }
            else
            {
                isBow = true;
                return true;
            }
        }
        set
        {
            if (value)
            {
                PlayerPrefs.SetInt("IsBow", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsBow", 0);
            }
            isBow = true;
        }
    }

    //получаем текущую прибавку от крита
    public float GetCurrSwordAddCriteDmg
    {
        get
        {
            float currAddCrit = Mathf.RoundToInt(currSwordAttackDamage * currSwordCritKoef);
            return currAddCrit;
        }
    }
    public float GetCurrBowAddCriteDmg
    {
        get
        {
            float currAddCrit = Mathf.RoundToInt(currBowAttackDamage * currBowCritKoef);
            return currAddCrit;
        }
    }

    [Space(2)]
    [Header("Weapon Stats")]
    public float currChangeDurability;
    //attackDurability
    public float currSwordAttackDurability;
    public float currBowAttackDurability;

    //длительность атаки вблизи без меча
    public float startSwordAttackDurability = 2.5f;
    //длительность атаки луком без лука (0 - атака не идет)
    public float startBowAttackDurability = 0;

    [Space(2)]
    [Header("Luck Stat")]
    public float currLuck;

    [Space(2)]
    [Header("Current Coins")]
    public int currentCoins;
    //сохраненное значение текущих монет
    public int Coins 
    { 
        get 
        {
            if (!PlayerPrefs.HasKey("PlayerCoins"))
            {
                PlayerPrefs.SetInt("PlayerCoins", 0);
            }
            currentCoins = PlayerPrefs.GetInt("PlayerCoins");
            return PlayerPrefs.GetInt("PlayerCoins");
        }
        set
        {
            PlayerPrefs.SetInt("PlayerCoins", value);
            currentCoins = PlayerPrefs.GetInt("PlayerCoins");
        }
    }

    [Space(2)]
    [Header("Настройка вида нанесения крит урона")]
    [Tooltip("Значение True - автоматически, False - по нажатию кнопки")]
    public bool isCritAutomaticly;
    public bool IsCritAutomaticly
    {
        get
        {
            if (!PlayerPrefs.HasKey("IsCritAutomaticly"))
            {
                //по дефолту выключены
                PlayerPrefs.SetInt("IsCritAutomaticly", 0);
            }
            if (PlayerPrefs.GetInt("IsCritAutomaticly") == 0)
            {
                isCritAutomaticly = false;
                return false;
            }
            else
            {
                isCritAutomaticly = true;
                return true;
            }
        }
        set
        {
            isCritAutomaticly = value;
            if (value)
            {
                PlayerPrefs.SetInt("IsCritAutomaticly", 1);
            }
            else
            {
                PlayerPrefs.SetInt("IsCritAutomaticly", 0);
            }
        }
    }

    [Space(2)]
    [Header("Текущая открытая локация")]
    [Tooltip("Индекс текущей открытой локации")]
    [Range(0,3)]
    public int currLocationIndex;
    //сохраненное значение открытой локакции
    public int CurrentLocationIndex
    {
        get
        {
            if (!PlayerPrefs.HasKey("CurrentLocationIndex"))
            {
                PlayerPrefs.SetInt("CurrentLocationIndex", 0);
            }
            currLocationIndex = PlayerPrefs.GetInt("CurrentLocationIndexl");
            return PlayerPrefs.GetInt("CurrentLocationIndex");
        }
        set
        {
            PlayerPrefs.SetInt("CurrentLocationIndex", value);
            currLocationIndex = PlayerPrefs.GetInt("CurrentLocationIndex");
        }
    }

    [Space(2)]
    [Header("Текущей уровень")]
    [Tooltip("Текущей уровень персонажа")]
    public int currentLevel;
    //сохраненное значение уровня
    public int Level
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerLevel"))
            {
                //по дефолту - 1
                PlayerPrefs.SetInt("PlayerLevel", 1);
            }
            currentLevel = PlayerPrefs.GetInt("PlayerLevel");
            return PlayerPrefs.GetInt("PlayerLevel");
        }
        set
        {
            PlayerPrefs.SetInt("PlayerLevel", value);
            currentLevel = PlayerPrefs.GetInt("PlayerLevel");
        }
    }

    [Space(2)]
    [Header("Текущий опыт персонажа")]
    [Tooltip("Текущей накопленный опыт на данном уровне")]
    public int currentExp;
    //сохраненное значение опыта
    public int Exp
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerExp"))
            {
                PlayerPrefs.SetInt("PlayerExp", 0);
            }
            currentExp = PlayerPrefs.GetInt("PlayerExp");
            return PlayerPrefs.GetInt("PlayerExp");
        }
        set
        {
            PlayerPrefs.SetInt("PlayerExp", value);
            currentExp = PlayerPrefs.GetInt("PlayerExp");
        }
    }

    [Space(2)]
    [Header("Текущие Прибавки от Экипировки")]
    [Tooltip("Прибавки полученные от надейтой экипировки")]
    public int addEquipSwordDamage;
    public int addEquipBowDamage;
    public int addEquipHealth;
    public int addEquipArmor;
    public int addEquipLuck;
    public float addEquipReadyTime;
    public float addEquipSwordAttackTime;
    public float addEquipBowAttackTime;

    [Space(2)]
    [Header("Текущие Прибавки от Уровней")]
    [Tooltip("Прибавки полученные от достигнутых уровней")]
    public int addLevelSwordDamage;
    public int addLevelBowDamage;
    public int addLevelHealth;
    public int addLevelArmor;
    public int addLevelLuck;

    [Space(2)]
    [Header("Текущие Прибавки от Дерева умений")]
    [Tooltip("Прибавки полученные от открытых скиллов в дереве умений")]
    public int addSkillTreeSwordDamage;
    public int addSkillTreeBowDamage;
    public int addSkillTreeHealth;
    public int addSkillTreeArmor;
    public int addSkillTreeLuck;
    public float addSkillTreeChangeTime;
    public float addSkillTreeReadyTime;
    public float addSkillTreeSwordAttackTime;
    public float addSkillTreeBowAttackTime;

    [Space(2)]
    [Header("Текущее Количество поинтов")]
    [Tooltip("Текущее Количество Поинтов для прокачки скиллов")]
    public int currentSkillPoints;
    //сохраненное значение очков
    public int Points
    {
        get
        {
            if (!PlayerPrefs.HasKey("PlayerPoints"))
            {
                PlayerPrefs.SetInt("PlayerPoints", 0);
            }
            currentSkillPoints = PlayerPrefs.GetInt("PlayerPoints");
            return PlayerPrefs.GetInt("PlayerPoints");
        }
        set
        {
            PlayerPrefs.SetInt("PlayerPoints", value);
            currentSkillPoints = PlayerPrefs.GetInt("PlayerPoints");
        }
    }

    [Space(5)]
    [Header("Спрайты")]
    [Space(2)]
    [Header("Текущий спрайт Меча персонажа")]
    public Sprite currentSwordSprite;

    [Space(2)]
    [Header("Текущие спрайты лука персонажа")]
    public Sprite currentBowArrowSprite;
    public Sprite currentBowLimbSprite;
    public Sprite currentBowRiserSprite;

    [Space(2)]
    [Header("Текущий спрайт Шлема персонажа")]
    public Sprite currentHelmetSprite = null;

    [Space(2)]
    [Header("Текущие экипированные спрайты перчаток персонажа")]
    public Sprite currentFingerSprite;
    public Sprite currentForearmLSprite;
    public Sprite currentForearmRSprite;
    public Sprite currentHandLSprite;
    public Sprite currentHandRSprite;
    public Sprite currentSleeverSprite;

    [Space(2)]
    [Header("Текущие экипированные спрайты нагрудника персонажа")]
    public Sprite currentArmLSprite;
    public Sprite currentArmRSprite;
    public Sprite currentPelvisSprite;
    public Sprite currentTorsoSprite;

    [Space(2)]
    [Header("Текущие экипированные спрайты ботинок персонажа")]
    public Sprite curretnLegSprite;
    public Sprite curretnShinSprite;

    [Space(5)]
    [Header("Звуки")]
    [Space(2)]
    [Header("Текущие звуки меча")]
    public AudioClip currSwordSwingSound;
    public AudioClip currSwordHitSound;

    [Space(2)]
    [Header("Текущие звуки лука")]
    public AudioClip currBowShotSound;
    public AudioClip currBowDrawSound;

    [Space(2)]
    [Header("Текущие звуки брони")]
    public AudioClip currArmorBlowSound;


    //сохраненное значение громкости музыки
    public float MusicVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("MusicVolume"))
            {
                PlayerPrefs.SetFloat("MusicVolume", 10f);
            }
            return PlayerPrefs.GetFloat("MusicVolume");
        }

        set
        {
            if (value > 10f) value = 10f;
            if (value < 0f) value = 0f;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    //сохраненное значение громкости звуков
    public float SoundVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("SoundVolume"))
            {
                PlayerPrefs.SetFloat("SoundVolume", 10f);
            }
            return PlayerPrefs.GetFloat("SoundVolume");
        }

        set
        {
            if (value > 10f) value = 10f;
            if (value < 0f) value = 0f;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }
}
