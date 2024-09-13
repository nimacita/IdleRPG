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

    //�������� ������ � ������� �� ������
    private void AddStatsNull()
    {
        currentStats.addLevelSwordDamage = 0;
        currentStats.addLevelBowDamage = 0;
        currentStats.addLevelHealth = 0;
        currentStats.addLevelArmor = 0;
        currentStats.addLevelLuck = 0;
    }

    //��������� ������ ������
    private void UpdateLevelPanel()
    {
        //������� �������
        currLvlTxt.text = $"{currentStats.Level}";

        //������ ��� ������ ����
        int neededExp = levelSettings.Levels[currentStats.Level - 1].neededExp;

        //������� ����
        currLvlExpTxt.text = $"{currentStats.Exp}/{neededExp}";

        //��������� 
        float fill = ((float)currentStats.Exp / neededExp);
        if(fill > 1f) fill = 1f;
        levlFill.fillAmount = fill;

    }

    //������������� ������������ �������������� �� �������
    private void SetLevelsStats()
    {
        //�������� ������� �������� �� �������
        PlayerController.instance.AddLevelsStats(-1);

        //��������
        swordDamage = 0;
        bowDamage = 0;
        health = 0;
        armor = 0;
        luck = 0;

        //���������� �� ���� ������� ������� ������� �������
        for (int i = 1; i <= currentStats.Level; i++) 
        {
            //���������� �������������� � ����� ������
            swordDamage += levelSettings.Levels[i - 1].levelStats.addSwordDamage;
            bowDamage += levelSettings.Levels[i - 1].levelStats.addBowDamage;
            health += levelSettings.Levels[i - 1].levelStats.addHealth;
            armor += levelSettings.Levels[i - 1].levelStats.addArmor;
            luck += levelSettings.Levels[i - 1].levelStats.addLuck;
        }

        //����������
        addSwordDmg.text = $"+{swordDamage}";
        addBowDmg.text = $"+{bowDamage}";
        addHealth.text = $"+{health}";
        addArmor.text = $"+{armor}";
        addLuck.text = $"+{luck}";

        //��������� �������������
        currentStats.addLevelSwordDamage = swordDamage;
        currentStats.addLevelBowDamage = bowDamage;
        currentStats.addLevelHealth = health;
        currentStats.addLevelArmor = armor;
        currentStats.addLevelLuck = luck;

        //��������� ������������ � ������
        PlayerController.instance.AddLevelsStats();

        //��������� ����������� � ���������
        StatsPanel.instance.UpdateStatsBtns();
    }

    //���������� ����
    private void AddExp(int expCount)
    {
        if (expCount > 0) 
        {
            currentStats.Exp += expCount;
            int currExp = currentStats.Exp;
            int currNeededExp = levelSettings.Levels[currentStats.Level - 1].neededExp;
            //������ ��������
            AddExpAnim(expCount);

            //���� ����� ������ ��� ����� ��� ��������, � �� ��������� ������������ �������
            if (currExp >= currNeededExp && currentStats.Level < levelSettings.Levels.Length)
            {
                //��������� �� ���� �������
                currentStats.Level++;

                //���������� ���� �� �������
                int currPoints = levelSettings.Levels[currentStats.Level - 1].pointsCount;
                currentStats.Points += currPoints;
                //��������� ������ ������ ������
                GameController.instance.UpdateSkillTreeBtn();

                //������ ��������
                NewLevelAnim();

                //������ ����
                AudioController.instance.PlayLevelUpSound();

                //��������� ���� ������
                currentStats.Exp = currExp - currNeededExp;
            }

            //��������� ������
            UpdateLevelPanel();

            //��������� �����
            SetLevelsStats();
        }
    }

    //�������� �����
    private void AddExpAnim(int expCount)
    {
        addExpTxt.text = $"+{expCount} Exp";
        levelPopUp.Play(addExpAnim.name);
    }

    //�������� ������
    private void NewLevelAnim()
    {
        levelPopUp.Play(newLevelAnim.name);
    }

    //��������� ������ ����
    public void StatsPanelOff()
    {
        levelPanelAnim.Play(statsOff.name);
    }

    //�������� ������ ����
    public void StatsPanelOn()
    {
        levelPanelAnim.Play(statsOn.name);
        statsPanel.SetActive(true);
    }

    //�������� ������� �������
    public void SetNewLevel()
    {
        //���� �� ��������� �������
        if (currentStats.Level < levelSettings.Levels.Length)
        {
            int needeExp = levelSettings.Levels[currentStats.Level - 1].neededExp - currentStats.Exp;
            AddExp(needeExp);
        }
        else
        {
            Debug.Log("��������� ������������ �������");
        }
    }
}
