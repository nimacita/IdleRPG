using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{

    [Header("Branch Settings")]
    [Tooltip("������ ����� � ������� ��������� ��������")]
    [Range(0, 2)]
    [SerializeField] private int branchIndex;
    [Tooltip("����� ������ �������� � ������ �����")]
    [Range(0,5)]
    [SerializeField] private int skillIndex;
    [Tooltip("������� ��� ������")]
    [SerializeField] private AllPlayerStatsType currentSkill;
    [Tooltip("������� ����� ������")]
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
    [Tooltip("������ ������, �� ��������, �� ������� ���� ����, ���� ����, ������ " +
        "����� �������� ������ ���� ��� ��������� ������ ������������")]
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

    //����������� �������� ������� �� ������� ��������, �� ������� ����� � ������ ��������
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

    //��������� ���������� �����������
    private void StartUpdateBtnView()
    {
        //���������� ������� ��������
        currentSkill = skillTreeSettings.skillTreeBranches[branchIndex].branchSkills[skillIndex].skillType;
        skillBonus = skillTreeSettings.skillTreeBranches[branchIndex].branchSkills[skillIndex].skillBonus;

        //��������� �����
        bonusTxt.text = GetBonusTxt(currentSkill, skillBonus);

        //��������� ����� ��������� �����
        if (skillIndex >= skillTreeSettings.skillTreeBranches[branchIndex].branchSkills.Length - 1)
        {
            //���� ��� ��������� �����
            branchNameTxt.text = skillTreeSettings.skillTreeBranches[branchIndex].branchName;
            branchNameTxt.gameObject.SetActive(true);
        }
        else
        {
            branchNameTxt.gameObject.SetActive(false);
        }

        //��������� ������
        (Sprite currIconSprite, bool isTimeIconActive) = GetBonusIcon(currentSkill);
        itemImg.sprite = currIconSprite;
        itemTimeImg.SetActive(isTimeIconActive);

        //��������� ������ ��� ������ � ���� ��������
        (Sprite currBtnSprite, Color currGlowColor) = GetSkillColor(currentSkill);
        bg.sprite = currBtnSprite;
        glow.color = currGlowColor;
        borders.color = currGlowColor;
    }

    //��������� ����������� ��������
    public void UpdateSkillBtnView()
    {
        //���� ������������
        if (IsActive)
        {
            //��������� ������ ������
            currSkillBtn.interactable = false;

            //�������� �������� � �������
            glow.gameObject.SetActive(true);
            borders.gameObject.SetActive(true);

            //������ ���� ������ �� ������
            foreach (Image way in backWays)
            {
                way.color = openWayColor;
            }
        }
        else
        {
            //�������� ������ ������
            currSkillBtn.interactable = true;

            //��������� �������� � �������
            glow.gameObject.SetActive(false);
            borders.gameObject.SetActive(false);

            //���� �������
            if (IsOpen())
            {
                //������ ���� ������ �� ������
                foreach (Image way in backWays)
                {
                    way.color = openWayColor;
                }

                //��������� ������ �������� ������
                closeBg.SetActive(false);

            }
            //���� �������
            else
            {
                //������ ���� ������ �� ������
                foreach (Image way in backWays)
                {
                    way.color = closeWayColor;
                }

                //�������� ������ �������� ������
                closeBg.SetActive(true);

                //��������� ������
                currSkillBtn.interactable = false;
            }
        }
       
    }

    //������� �� ������
    private bool IsOpen()
    {
        //���� ��� ����� �� ���������
        if (playerCurrentStats.Points <1)
        {
            return false;
        }
        if (skillIndex == 0)
        {
            //���� ��� ������ ������, �� �������
            return true;
        }
        else
        {
            //���������� �� ���� ������� � ���� ���� ���� �� �������, �� false
            foreach (SkillBtn sb in lastSkillBtns)
            {
                if (!sb.IsActive)
                {
                    return false;
                }
            }
            //���� �� �������� ��� - �� true
            return true;
        }

    }

    //������� �� ������ ������
    private void SkillBtnClick()
    {
        //���� �������� � �� �������
        if (IsOpen() && !IsActive)
        {
            //���� ���������� �����
            if (playerCurrentStats.Points > 0)
            {
                //���������� ��������� �����
                IsActive = true;
                //�������� ����
                playerCurrentStats.Points--;
                //��������� ��� ���� ������ �������
                skillTreeController.UpdateAllSkillsView();
                //��������� ��� ����� �� �������
                skillTreeController.SetSkillsTreeStats();

                //������ ����
                AudioController.instance.PlaySkillTreeBtnSound();
            }
        }
    }

    //�������� ������� ����� �������� � ����������� �� ����
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
                string st = currentBonus.ToString("0.##") + " ���";
                return st;
            default:
                return "";
        }
    }

    //�������� ������� ������ �������� � ����������� �� ����
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

    //�������� ������� ������ ������ � ���� �������� � ����������� �� ����
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

    //���������� ������ �����
    public int GetBranchInd()
    {
        return branchIndex;
    }

    //���������� ������ ������ � �����
    public int GetSkillInd()
    {
        return skillIndex;
    }
}
