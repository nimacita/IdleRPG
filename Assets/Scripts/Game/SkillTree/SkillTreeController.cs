using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeController : MonoBehaviour
{

    [Header("Skill View")]
    [SerializeField] private GameObject skillTreeView;
    [SerializeField] private Button backBtn;

    [Header("Current Points")]
    [SerializeField] private TMPro.TMP_Text pointsTxt;
    [SerializeField] private TMPro.TMP_Text pointsNameTxt;
    [SerializeField] private GameObject pointsGlow;
    [SerializeField] private GameObject pointsBorders;

    [Header("Branches obj")]
    [Tooltip("Все родитеьские объекты веток, содержащие в себе кнопки скиллов")]
    [SerializeField] private GameObject[] allObjBranches;
    //все кнопки скиллов
    private List<SkillBtn> allSkills;

    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private Animation statsAnim;
    [SerializeField] private AnimationClip statsOn;
    [SerializeField] private AnimationClip statsOff;
    [SerializeField] private TMPro.TMP_Text statsSwordDamage;
    [SerializeField] private TMPro.TMP_Text statsBowDamage;
    [SerializeField] private TMPro.TMP_Text statsHealth;
    [SerializeField] private TMPro.TMP_Text statsLuck;
    [SerializeField] private TMPro.TMP_Text statsArmor;
    [SerializeField] private TMPro.TMP_Text statsChangeTime;
    [SerializeField] private TMPro.TMP_Text statsReadyTime;
    [SerializeField] private TMPro.TMP_Text statsSwordAtckTime;
    [SerializeField] private TMPro.TMP_Text statsBowAtckTime;
    [SerializeField] private Color defaultTxtColor;
    [SerializeField] private Color addTxtColor;

    [Header("Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;
    [SerializeField] private SkillTreeSettings skillTreeSettings;

    //stats
    private int swordDamage;
    private int bowDamage;
    private int health;
    private int armor;
    private int luck;
    private float changeTime;
    private float readyTime;
    private float swordAttackTime;
    private float bowAttackTime;

    public static SkillTreeController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        skillTreeView.SetActive(false);
        statsPanel.SetActive(false);

        InitializeAllSkills();
        ButtonSettings();

        AddStatsNull();
        SetSkillsTreeStats();
        PointsUpdate();
    }

    //настройка кнопок
    private void ButtonSettings()
    {
        backBtn.onClick.AddListener(CloseSkillTree);
    }

    //обнуляем бонусы с уровней на старте
    private void AddStatsNull()
    {
        currentStats.addSkillTreeSwordDamage = 0;
        currentStats.addSkillTreeBowDamage = 0;
        currentStats.addSkillTreeHealth = 0;
        currentStats.addSkillTreeArmor = 0;
        currentStats.addSkillTreeLuck = 0;
        currentStats.addSkillTreeChangeTime = 0;
        currentStats.addSkillTreeReadyTime = 0;
        currentStats.addSkillTreeSwordAttackTime = 0;
        currentStats.addSkillTreeBowAttackTime = 0;
    }

    //устанавливаем прибалвяемые характеристики от всех скиллов в дереве
    public void SetSkillsTreeStats()
    {
        //обнуляем текущие прибавки от дерева скиллов
        PlayerController.instance.AddSkillTreeStats(-1);

        //обнуляем
        swordDamage = 0;
        bowDamage = 0;
        health = 0;
        armor = 0;
        luck = 0;
        changeTime = 0f;
        readyTime = 0f;
        swordAttackTime = 0f;
        bowAttackTime = 0f;

        //проходимся по всем скиллам и записываем в статы, если активные
        foreach (SkillBtn sb in allSkills)
        {
            //если активный
            if (sb.IsActive)
            {
                //записываем тип скилла данного умения
                AllPlayerStatsType currType = skillTreeSettings.skillTreeBranches[sb.GetBranchInd()].
                    branchSkills[sb.GetSkillInd()].skillType;
                //записываем прибавку от данного умения
                float currBonus = skillTreeSettings.skillTreeBranches[sb.GetBranchInd()].
                    branchSkills[sb.GetSkillInd()].skillBonus;
                //добавляем к статам
                AddToStats(currType, currBonus);
            }
        }

        //обновляем панель статов
        UpdateStatsPanel();

        //обновляем характерстики
        currentStats.addSkillTreeSwordDamage = swordDamage;
        currentStats.addSkillTreeBowDamage = bowDamage;
        currentStats.addSkillTreeHealth = health;
        currentStats.addSkillTreeArmor = armor;
        currentStats.addSkillTreeLuck = luck;
        currentStats.addSkillTreeReadyTime = readyTime;
        currentStats.addSkillTreeChangeTime = changeTime;
        currentStats.addSkillTreeSwordAttackTime = swordAttackTime;
        currentStats.addSkillTreeBowAttackTime = bowAttackTime;

        //обновляем харакетристи у игрока
        PlayerController.instance.AddSkillTreeStats();

        //обновляем отображение в инвенторе
        StatsPanel.instance.UpdateStatsBtns();

        //обновляем отображение поинтов
        PointsUpdate();
    }

    //добавляем в выбранную стату в зависимости от типа
    private void AddToStats(AllPlayerStatsType currType, float currBonus)
    {
        switch (currType)
        {
            case AllPlayerStatsType.SwordDamage:
                swordDamage += (int)currBonus;
                break;
            case AllPlayerStatsType.BowDamage:
                bowDamage += (int)currBonus;
                break;
            case AllPlayerStatsType.Health:
                health += (int)currBonus;
                break;
            case AllPlayerStatsType.Armor:
                armor += (int)currBonus;
                break;
            case AllPlayerStatsType.Luck:
                luck += (int)currBonus;
                break;
            case AllPlayerStatsType.ReadyTime:
                readyTime += currBonus;
                break;
            case AllPlayerStatsType.ChargeTime:
                changeTime += currBonus;
                break;
            case AllPlayerStatsType.SwordAtckTime:
                swordAttackTime += currBonus;
                break;
            case AllPlayerStatsType.BowAtckTime:
                bowAttackTime += currBonus;
                break;
        }
    }

    //обновляем значение поинтов
    public void PointsUpdate()
    {
        int currPoints = currentStats.Points;

        //обновляем текст 
        pointsTxt.text = $"{currPoints}";
        pointsNameTxt.text = UpdateNameTxt(currPoints);

        //обновляем свечение и границы
        if (currPoints > 0)
        {
            pointsGlow.SetActive(true);
            pointsBorders.SetActive(true);
        }
        else
        {
            pointsGlow.SetActive(false);
            pointsBorders.SetActive(false);
        }
    }

    //обновляем текст имени "очков" в зависимости от количества
    private string UpdateNameTxt(int currPoints)
    {
        int lastDigit = currPoints % 10;
        int lastTwoDigits = currPoints % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
        {
            return $"очков";
        }

        switch (lastDigit)
        {
            case 1:
                return $"очко";
            case 2:
            case 3:
            case 4:
                return $"очка";
            default:
                return $"очков";
        }
    }

    //обновляем панель статов, и красим если есть добавление
    private void UpdateStatsPanel()
    {
        statsSwordDamage.text = $"+{swordDamage}";
        if (swordDamage != 0) statsSwordDamage.color = addTxtColor;
        else statsSwordDamage.color = defaultTxtColor;

        statsBowDamage.text = $"+{bowDamage}";
        if (bowDamage != 0) statsBowDamage.color = addTxtColor;
        else statsBowDamage.color = defaultTxtColor;

        statsHealth.text = $"+{health}";
        if (health != 0) statsHealth.color = addTxtColor;
        else statsHealth.color = defaultTxtColor;

        statsArmor.text = $"+{armor}";
        if (armor != 0) statsArmor.color = addTxtColor;
        else statsArmor.color = defaultTxtColor;

        statsLuck.text = $"+{luck}";
        if (luck != 0) statsLuck.color = addTxtColor;
        else statsLuck.color = defaultTxtColor;

        statsChangeTime.text = $"{changeTime} сек";
        if (changeTime != 0) statsChangeTime.color = addTxtColor;
        else statsChangeTime.color = defaultTxtColor;

        statsReadyTime.text = $"{readyTime} сек";
        if (readyTime != 0) statsReadyTime.color = addTxtColor;
        else statsReadyTime.color = defaultTxtColor;

        statsSwordAtckTime.text = $"{swordAttackTime} сек";
        if (swordAttackTime != 0) statsSwordAtckTime.color = addTxtColor;
        else statsSwordAtckTime.color = defaultTxtColor;

        statsBowAtckTime.text = $"{bowAttackTime} сек";
        if (bowAttackTime != 0) statsBowAtckTime.color = addTxtColor;
        else statsBowAtckTime.color = defaultTxtColor;
    }

    //обновляем вид всех кнопок
    public void UpdateAllSkillsView()
    {
        foreach (SkillBtn sb in allSkills)
        {
            sb.UpdateSkillBtnView();
        }
    }

    //инициализируем все скиллы
    private void InitializeAllSkills()
    {
        allSkills = new List<SkillBtn>();

        //проходимся по свем веткам
        foreach (var branch in allObjBranches)
        {
            //проходимся по всем кнопкам скилла в этой ветке
            for (int i = 0; i < branch.transform.childCount; i++) 
            {
                allSkills.Add(branch.transform.GetChild(i).gameObject.GetComponent<SkillBtn>());
            }
        }
    }

    //открываем дерево скиллов
    public void OpenSkillTree()
    {
        UpdateAllSkillsView();
        PointsUpdate();
        statsPanel.SetActive(false);
        skillTreeView.SetActive(true);
    }

    //закрываем дерево скиллов
    public void CloseSkillTree()
    {
        //обновляем кнопку дерева умений
        GameController.instance.UpdateSkillTreeBtn();
        skillTreeView.SetActive(false);
        GameController.instance.SetPause(false);
    }

    //открываем панель статов
    public void StatsPanelOn()
    {
        statsAnim.Play(statsOn.name);
    }

    //закрываем панель статов
    public void StatsPanelOff()
    {
        statsAnim.Play(statsOff.name);
    }
}
