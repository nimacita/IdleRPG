using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBtn : MonoBehaviour
{

    [Header("Main Settings")]
    [Tooltip("Текущая характеристика отображаемая данной кнопкой")]
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

    //устанавливаем значение
    public void SetStat(float stat, bool isSec = false, bool isProcent = false, bool isAnim = true)
    {
        //устанавливаем строку
        string st = stat.ToString("0.##");

        //если нужно - добавляем подпись секунды
        if (isSec) st += " сек";
        //если проценты, то добавляем
        else if (isProcent) st += "%";

        //записываем в текстовое поле
        statTxt.text = st;

        //играем анимацию
        if (isAnim)
        {
            PlayStatAnim(stat);
        }

        //сохраняем значение
        currentStatCount = stat;
    }

    //какую играем анимацию
    private void PlayStatAnim(float stat)
    {
        switch (currentStatType)
        {
            //если не временные характеристики
            case AllPlayerStatsType.Armor:
            case AllPlayerStatsType.Health:
            case AllPlayerStatsType.Luck:
            case AllPlayerStatsType.SwordDamage:
            case AllPlayerStatsType.BowDamage:
                {
                    if (stat > currentStatCount)
                    {
                        //играем зеленую анимацию
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat < currentStatCount)
                    {
                        //играем красную анимацию
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
                        //играем красную анимацию
                        PlayAnim(minusStatAnim);
                    }
                    else if (currentStatCount == 0)
                    {
                        //играем зеленую анимацию
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat < currentStatCount)
                    {
                        //играем зеленую анимацию
                        PlayAnim(plusStatAnim);
                    }
                    else if (stat > currentStatCount)
                    {
                        //играем красную анимацию
                        PlayAnim(minusStatAnim);
                    }
                    break;
                }
        }
    }

    //обновляем вид панели
    private void UpdateDescribeViewPanel()
    {
        StatText statText = statsPanel.GetStatsTexts(currentStatType);

        //заголовок
        titleTxt.text = statText.StatTitle;

        //описание
        describeTxt.text = statText.StatDescription;

        //текст отображения урона на расстоянии если оружие
        if (currentStatType == AllPlayerStatsType.BowDamage ||
            currentStatType == AllPlayerStatsType.SwordDamage)
        {
            //текст вблизи урона
            meleeDmgTxt.text = $"{Mathf.RoundToInt(statText.StatMeleeDamage)}";

            //текст урона на расстоянии
            rangeDmgTxt.text = $"{Mathf.RoundToInt(statText.StatRangeDamage)}";

            //текст крит урона
            critDmgTxt.text = $"{Mathf.RoundToInt(statText.StatCritDamage)}";

            rangeIcons.SetActive(true);
        }
        else
        {
            rangeIcons.SetActive(false);
        }
    }

    //ираем анимацию
    private void PlayAnim(AnimationClip anim)
    {
        statBtnAnim.Play(anim.name);
    }

    //включаем панель
    public void EnableDescribePanel()
    {
        if (!describePanel.activeSelf) 
        {
            UpdateDescribeViewPanel();
            describePanel.SetActive(true);
        }
    }

    //выключаем панель
    public void DisableDescribePanel()
    {
        describePanel.SetActive(false);
    }

    //возвращаем тип кнопки
    public AllPlayerStatsType GetCurrStatType()
    {
        return currentStatType;
    }
}
