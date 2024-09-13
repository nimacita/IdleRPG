using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBtn : MonoBehaviour
{

    [Header("Main Settings")]
    [Tooltip("������� �������������� ������������ ������ �������")]
    public AllPlayerStatsType currentStatType;

    [Header("Components")]
    public TMPro.TMP_Text statTxt;

    [Header("Animations")]
    public Animation statBtnAnim;
    public AnimationClip plusStatAnim;
    public AnimationClip minusStatAnim;

    [Header("Describe Panel")]
    public GameObject describePanel;
    public TMPro.TMP_Text titleTxt;
    public TMPro.TMP_Text describeTxt;
    public GameObject rangeIcons;
    public TMPro.TMP_Text meleeDmgTxt;
    public TMPro.TMP_Text rangeDmgTxt;
    public TMPro.TMP_Text critDmgTxt;

    [Header("Stats Panel")]
    public StatsPanel statsPanel;

    //stat
    private float currentStatCount;


    void Start()
    {
        DisableDescribePanel();
    }

    //������������� ��������
    public void SetStat(float stat, bool isSec = false, bool isProcent = false, bool isAnim = true)
    {
        //������������� ������
        string st = stat.ToString("0.##");

        //���� ����� - ��������� ������� �������
        if (isSec) st += " ���";
        //���� ��������, �� ���������
        else if (isProcent) st += "%";

        //���������� � ��������� ����
        statTxt.text = st;

        //������ ��������
        if (isAnim)
        {
            PlayStatAnim(stat);
        }

        //��������� ��������
        currentStatCount = stat;
    }

    //����� ������ ��������
    private void PlayStatAnim(float stat)
    {
        switch (currentStatType)
        {
            //���� �� ��������� ��������������
            case AllPlayerStatsType.Armor:
            case AllPlayerStatsType.Health:
            case AllPlayerStatsType.Luck:
            case AllPlayerStatsType.SwordDamage:
            case AllPlayerStatsType.BowDamage:
                {
                    if (stat > currentStatCount)
                    {
                        //������ ������� ��������
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat < currentStatCount)
                    {
                        //������ ������� ��������
                        PlayAnim(minusStatAnim);
                    }
                    break;
                }
            case AllPlayerStatsType.ReadyTime:
            case AllPlayerStatsType.ChargeTime:
            case AllPlayerStatsType.SwordAtckTime:
            case AllPlayerStatsType.BowAtckTime:
                {
                    if(stat == 0f)
                    {
                        //������ ������� ��������
                        PlayAnim(minusStatAnim);
                    }
                    else if (currentStatCount == 0)
                    {
                        //������ ������� ��������
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat < currentStatCount)
                    {
                        //������ ������� ��������
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat > currentStatCount)
                    {
                        //������ ������� ��������
                        PlayAnim(minusStatAnim);
                    }
                    break;
                }
        }
    }

    //��������� ��� ������
    private void UpdateDescribeViewPanel()
    {
        StatText statText = statsPanel.GetStatsTexts(currentStatType);

        //���������
        titleTxt.text = statText.StatTitle;

        //��������
        describeTxt.text = statText.StatDescription;

        //����� ����������� ����� �� ���������� ���� ������
        if (currentStatType == AllPlayerStatsType.BowDamage ||
            currentStatType == AllPlayerStatsType.SwordDamage)
        {
            //����� ������ �����
            meleeDmgTxt.text = $"{Mathf.RoundToInt(statText.StatMeleeDamage)}";

            //����� ����� �� ����������
            rangeDmgTxt.text = $"{Mathf.RoundToInt(statText.StatRangeDamage)}";

            //����� ���� �����
            critDmgTxt.text = $"{Mathf.RoundToInt(statText.StatCritDamage)}";

            rangeIcons.SetActive(true);
        }
        else
        {
            rangeIcons.SetActive(false);
        }
    }

    //����� ��������
    private void PlayAnim(AnimationClip anim)
    {
        statBtnAnim.Play(anim.name);
    }

    //�������� ������
    public void EnableDescribePanel()
    {
        if (!describePanel.activeSelf) 
        {
            UpdateDescribeViewPanel();
            describePanel.SetActive(true);
        }
    }

    //��������� ������
    public void DisableDescribePanel()
    {
        describePanel.SetActive(false);
    }

    //���������� ��� ������
    public AllPlayerStatsType GetCurrStatType()
    {
        return currentStatType;
    }
}
