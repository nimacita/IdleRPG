using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{

    [Header("Branch Settings")]
    [Tooltip("Индекс ветки в которой находится усиление")]
    [Range(0, 2)]
    [SerializeField] private int branchIndex;
    [Tooltip("Инекс самого усиления в данной ветке")]
    [Range(0,5)]
    [SerializeField] private int skillIndex;
    [Tooltip("Текущий тип скилла")]
    [SerializeField] private AllPlayerStatsType currentSkill;
    [Tooltip("Текущий бонус скилла")]
    [SerializeField] private float skillBonus;

    [Header("Components")]
    [SerializeField] private Button currSkillBtn;
    [SerializeField] private Image bg;
    [SerializeField] private Image itemImg;
    [SerializeField] private GameObject itemTimeImg;
    [SerializeField] private Image glow;
    [SerializeField] private Image borders;
    [SerializeField] private TMPro.TMP_Text bonusTxt;
    [SerializeField] private GameObject closeBg;
    [SerializeField] private TMPro.TMP_Text branchNameTxt;
    [SerializeField] private Image[] backWays;

    [Header("Way Settings")]
    [SerializeField] private Color closeWayColor;
    [SerializeField] private Color openWayColor;

    [Header("Btn Bg Settings")]
    [SerializeField] private Sprite redBtnSprite;
    [SerializeField] private Color redGlowColor;
    [SerializeField] private Sprite purpleBtnSprite;
    [SerializeField] private Color purpleGlowColor;
    [SerializeField] private Sprite greenBtnSprite;
    [SerializeField] private Color greenGlowColor;
    [SerializeField] private Sprite yellowBtnSprite;
    [SerializeField] private Color yellowGlowColor;
    [SerializeField] private Sprite blueBtnSprite;
    [SerializeField] private Color blueGlowColor;

    [Header("Icon Sprites")]
    [SerializeField] private Sprite swordAttackSprite;
    [SerializeField] private Sprite bowAttackSprite;
    [SerializeField] private Sprite healthSprite;
    [SerializeField] private Sprite armorSprite;
    [SerializeField] private Sprite luckSprite;
    [SerializeField] private Sprite changeTimeSprite;
    [SerializeField] private Sprite readyTimeSprite;

    [Header("Last Skill")]
    [Tooltip("Кнопки скилла, до нынешней, от которой идет путь, если есть, кнопка " +
        "будет доступна только если все указанные кнопки активированы")]
    [SerializeField] private SkillBtn[] lastSkillBtns;

    [Header("Stats")]
    [SerializeField] private SkillTreeSettings skillTreeSettings;
    [SerializeField] private PlayerCurrentStats playerCurrentStats;

    private SkillTreeController skillTreeController;


    void Start()
    {
        skillTreeController = SkillTreeController.instance;

        currSkillBtn.onClick.AddListener(SkillBtnClick);

        StartUpdateBtnView();
        UpdateSkillBtnView();
    }

    //сохранненое значение активна ли текущее усиление, по пндексу ветки и самого усиления
    public bool IsActive
    {
        get
        {
            if (!PlayerPrefs.HasKey($"IsActiveSkill{skillIndex}In{branchIndex}"))
            {
                PlayerPrefs.SetInt($"IsActiveSkill{skillIndex}In{branchIndex}", 0);
            }
            if (PlayerPrefs.GetInt($"IsActiveSkill{skillIndex}In{branchIndex}") == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private set
        {
            if (value)
            {
                PlayerPrefs.SetInt($"IsActiveSkill{skillIndex}In{branchIndex}", 1);
            }
            else
            {
                PlayerPrefs.SetInt($"IsActiveSkill{skillIndex}In{branchIndex}", 0);
            }
        }
    }

    //начальное обновление компонентов
    private void StartUpdateBtnView()
    {
        //записываем текущее усиление
        currentSkill = skillTreeSettings.skillTreeBranches[branchIndex].branchSkills[skillIndex].skillType;
        skillBonus = skillTreeSettings.skillTreeBranches[branchIndex].branchSkills[skillIndex].skillBonus;

        //обновляем текст
        bonusTxt.text = GetBonusTxt(currentSkill, skillBonus);

        //обновляем текст навзвания ветки
        if (skillIndex >= skillTreeSettings.skillTreeBranches[branchIndex].branchSkills.Length - 1)
        {
            //если это последний айтем
            branchNameTxt.text = skillTreeSettings.skillTreeBranches[branchIndex].branchName;
            branchNameTxt.gameObject.SetActive(true);
        }
        else
        {
            branchNameTxt.gameObject.SetActive(false);
        }

        //обновляем иконку
        (Sprite currIconSprite, bool isTimeIconActive) = GetBonusIcon(currentSkill);
        itemImg.sprite = currIconSprite;
        itemTimeImg.SetActive(isTimeIconActive);

        //обновляем задний фон кнопки и цвет свечения
        (Sprite currBtnSprite, Color currGlowColor) = GetSkillColor(currentSkill);
        bg.sprite = currBtnSprite;
        glow.color = currGlowColor;
        borders.color = currGlowColor;
    }

    //обновляем отображение усиления
    public void UpdateSkillBtnView()
    {
        //если активировано
        if (IsActive)
        {
            //выключаем кнопку скилла
            currSkillBtn.interactable = false;

            //включаем свечение и границы
            glow.gameObject.SetActive(true);
            borders.gameObject.SetActive(true);

            //меняем цвет дороги до умения
            foreach (Image way in backWays)
            {
                way.color = openWayColor;
            }
        }
        else
        {
            //включаем кнопку скилла
            currSkillBtn.interactable = true;

            //выключаем свечение и границы
            glow.gameObject.SetActive(false);
            borders.gameObject.SetActive(false);

            //если открыто
            if (IsOpen())
            {
                //меняем цвет дороги до умения
                foreach (Image way in backWays)
                {
                    way.color = openWayColor;
                }

                //выключаем панель закрытой кнопки
                closeBg.SetActive(false);

            }
            //если закрыто
            else
            {
                //меняем цвет дороги до умения
                foreach (Image way in backWays)
                {
                    way.color = closeWayColor;
                }

                //включаем панель закрытой кнопки
                closeBg.SetActive(true);

                //выключаем кнопку
                currSkillBtn.interactable = false;
            }
        }
       
    }

    //открыта ли кнопка
    private bool IsOpen()
    {
        //если нет очков то выключена
        if (playerCurrentStats.Points <1)
        {
            return false;
        }
        if (skillIndex == 0)
        {
            //если это первое умение, то открыто
            return true;
        }
        else
        {
            //проходимся по всем прошлым и если хоть одно не активно, то false
            foreach (SkillBtn sb in lastSkillBtns)
            {
                if (!sb.IsActive)
                {
                    return false;
                }
            }
            //если не активынх нет - то true
            return true;
        }

    }

    //нажатие на кнопку скилла
    private void SkillBtnClick()
    {
        //если отркрыто и не активно
        if (IsOpen() && !IsActive)
        {
            //если достаточно очков
            if (playerCurrentStats.Points > 0)
            {
                //активируем выбранный скилл
                IsActive = true;
                //отнимаем очко
                playerCurrentStats.Points--;
                //обновляем вид всех кнопок скиллов
                skillTreeController.UpdateAllSkillsView();
                //обновляем все статы от скиллов
                skillTreeController.SetSkillsTreeStats();

                //играем звук
                AudioController.instance.PlaySkillTreeBtnSound();
            }
        }
    }

    //получаем текущий текст усиления в зависимости от типа
    private string GetBonusTxt(AllPlayerStatsType currentSkil, float currentBonus)
    {
        switch (currentSkil)
        {
            case AllPlayerStatsType.SwordDamage:
            case AllPlayerStatsType.BowDamage:
            case AllPlayerStatsType.Health:
            case AllPlayerStatsType.Armor:
            case AllPlayerStatsType.Luck:
                return $"+{(int)currentBonus}";
            case AllPlayerStatsType.ChargeTime:
            case AllPlayerStatsType.ReadyTime:
            case AllPlayerStatsType.SwordAtckTime:
            case AllPlayerStatsType.BowAtckTime:
                string st = currentBonus.ToString("0.##") + " сек";
                return st;
            default:
                return "";
        }
    }

    //получаем текущую иконку усиления в зависимости от типа
    private (Sprite,bool) GetBonusIcon(AllPlayerStatsType currentSkil)
    {
        switch (currentSkil)
        {
            case AllPlayerStatsType.SwordDamage:
                return (swordAttackSprite,false);
            case AllPlayerStatsType.BowDamage:
                return (bowAttackSprite, false);
            case AllPlayerStatsType.Health:
                return (healthSprite, false);
            case AllPlayerStatsType.Armor:
                return (armorSprite, false);
            case AllPlayerStatsType.Luck:
                return (luckSprite, false);
            case AllPlayerStatsType.ChargeTime:
                return (changeTimeSprite, false);
            case AllPlayerStatsType.ReadyTime:
                return (readyTimeSprite, false);
            case AllPlayerStatsType.SwordAtckTime:
                return (swordAttackSprite, true);
            case AllPlayerStatsType.BowAtckTime:
                return (bowAttackSprite, true);
            default:
                return (null, false);
        }
    }

    //получаем текущий спрайт кнопки и цвет свечения в зависимости от типа
    private (Sprite, Color) GetSkillColor(AllPlayerStatsType currentSkil)
    {
        switch (currentSkil)
        {
            case AllPlayerStatsType.SwordDamage:
            case AllPlayerStatsType.BowDamage:
            case AllPlayerStatsType.SwordAtckTime:
            case AllPlayerStatsType.BowAtckTime:
                return (purpleBtnSprite, purpleGlowColor);
            case AllPlayerStatsType.Health:
                return (redBtnSprite, redGlowColor);
            case AllPlayerStatsType.Armor:
                return (yellowBtnSprite, yellowGlowColor);
            case AllPlayerStatsType.Luck:
                return (greenBtnSprite, greenGlowColor);
            case AllPlayerStatsType.ChargeTime:
            case AllPlayerStatsType.ReadyTime:
                return (blueBtnSprite, blueGlowColor);
            default:
                return (null, redGlowColor);
        }
    }

    //возвращаем Индекс ветки
    public int GetBranchInd()
    {
        return branchIndex;
    }

    //возвращаем индекс кнопки в ветке
    public int GetSkillInd()
    {
        return skillIndex;
    }
}
