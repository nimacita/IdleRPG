using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    [Header("Level Panel")]
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private Animation levelPanelAnim;
    [SerializeField] private Image levlFill;
    [SerializeField] private TMPro.TMP_Text currLvlTxt;
    [SerializeField] private TMPro.TMP_Text currLvlExpTxt;

    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private AnimationClip statsOn;
    [SerializeField] private AnimationClip statsOff;

    [Header("Level Pop Up Anims")]
    [SerializeField] private Animation levelPopUp;
    [SerializeField] private AnimationClip addExpAnim;
    [SerializeField] private AnimationClip newLevelAnim;
    [SerializeField] private TMPro.TMP_Text addExpTxt;

    [Header("Level Add Stats")]
    [SerializeField] private TMPro.TMP_Text addSwordDmg;
    [SerializeField] private TMPro.TMP_Text addBowDmg;
    [SerializeField] private TMPro.TMP_Text addHealth;
    [SerializeField] private TMPro.TMP_Text addArmor;
    [SerializeField] private TMPro.TMP_Text addLuck;

    [Header("Stats")]
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private PlayerCurrentStats currentStats;

    //Stats
    private int swordDamage;
    private int bowDamage;
    private int health;
    private int armor;
    private int luck;

    public static LevelController instance;

    private void OnEnable()
    {
        EnemieController.onEnemieAddExp += AddExp;
    }

    private void OnDisable()
    {
        EnemieController.onEnemieAddExp -= AddExp;
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        statsPanel.SetActive(false);

        AddStatsNull();
        UpdateLevelPanel();
        SetLevelsStats();
    }

    //обнуляем бонусы с уровней на старте
    private void AddStatsNull()
    {
        currentStats.addLevelSwordDamage = 0;
        currentStats.addLevelBowDamage = 0;
        currentStats.addLevelHealth = 0;
        currentStats.addLevelArmor = 0;
        currentStats.addLevelLuck = 0;
    }

    //обновляем панель уровня
    private void UpdateLevelPanel()
    {
        //текущий уровень
        currLvlTxt.text = $"{currentStats.Level}";

        //нужный для уровня опыт
        int neededExp = levelSettings.Levels[currentStats.Level - 1].neededExp;

        //текущей опыт
        currLvlExpTxt.text = $"{currentStats.Exp}/{neededExp}";

        //заполняем 
        float fill = ((float)currentStats.Exp / neededExp);
        if(fill > 1f) fill = 1f;
        levlFill.fillAmount = fill;

    }

    //устанавливаем прибалвяемые характеристики от уровней
    private void SetLevelsStats()
    {
        //обнуляем текущие прибавки от уровней
        PlayerController.instance.AddLevelsStats(-1);

        //обнуляем
        swordDamage = 0;
        bowDamage = 0;
        health = 0;
        armor = 0;
        luck = 0;

        //проходимся по всем открытм уровням начиная сначала
        for (int i = 1; i <= currentStats.Level; i++) 
        {
            //прибавляем характерситики с этого уровня
            swordDamage += levelSettings.Levels[i - 1].levelStats.addSwordDamage;
            bowDamage += levelSettings.Levels[i - 1].levelStats.addBowDamage;
            health += levelSettings.Levels[i - 1].levelStats.addHealth;
            armor += levelSettings.Levels[i - 1].levelStats.addArmor;
            luck += levelSettings.Levels[i - 1].levelStats.addLuck;
        }

        //записываем
        addSwordDmg.text = $"+{swordDamage}";
        addBowDmg.text = $"+{bowDamage}";
        addHealth.text = $"+{health}";
        addArmor.text = $"+{armor}";
        addLuck.text = $"+{luck}";

        //обновляем характерстики
        currentStats.addLevelSwordDamage = swordDamage;
        currentStats.addLevelBowDamage = bowDamage;
        currentStats.addLevelHealth = health;
        currentStats.addLevelArmor = armor;
        currentStats.addLevelLuck = luck;

        //обновляем харакетристи у игрока
        PlayerController.instance.AddLevelsStats();

        //обновляем отображение в инвенторе
        StatsPanel.instance.UpdateStatsBtns();
    }

    //прибавляем опыт
    private void AddExp(int expCount)
    {
        if (expCount > 0) 
        {
            currentStats.Exp += expCount;
            int currExp = currentStats.Exp;
            int currNeededExp = levelSettings.Levels[currentStats.Level - 1].neededExp;
            //играем анимацию
            AddExpAnim(expCount);

            //если опыта больше или равно для перехода, и не достигнут максимальный уровень
            if (currExp >= currNeededExp && currentStats.Level < levelSettings.Levels.Length)
            {
                //переходим на след уровень
                currentStats.Level++;

                //прибавляем очки за уровень
                int currPoints = levelSettings.Levels[currentStats.Level - 1].pointsCount;
                currentStats.Points += currPoints;
                //обновляем кнопку дерева умений
                GameController.instance.UpdateSkillTreeBtn();

                //играем анимацию
                NewLevelAnim();

                //играем звук
                AudioController.instance.PlayLevelUpSound();

                //обновляем опыт уровня
                currentStats.Exp = currExp - currNeededExp;
            }

            //обновляем панель
            UpdateLevelPanel();

            //обновляем статы
            SetLevelsStats();
        }
    }

    //анимация опыта
    private void AddExpAnim(int expCount)
    {
        addExpTxt.text = $"+{expCount} Exp";
        levelPopUp.Play(addExpAnim.name);
    }

    //анимация уровня
    private void NewLevelAnim()
    {
        levelPopUp.Play(newLevelAnim.name);
    }

    //выключаем панель стат
    public void StatsPanelOff()
    {
        levelPanelAnim.Play(statsOff.name);
    }

    //включаем панель стат
    public void StatsPanelOn()
    {
        levelPanelAnim.Play(statsOn.name);
        statsPanel.SetActive(true);
    }

    //повышаем текущий уровень
    public void SetNewLevel()
    {
        //если не последний уровень
        if (currentStats.Level < levelSettings.Levels.Length)
        {
            int needeExp = levelSettings.Levels[currentStats.Level - 1].neededExp - currentStats.Exp;
            AddExp(needeExp);
        }
        else
        {
            Debug.Log("Достигнут максимальный уровень");
        }
    }
}
