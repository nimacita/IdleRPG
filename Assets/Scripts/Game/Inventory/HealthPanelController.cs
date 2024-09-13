using Assets.HeroEditor.Common.Scripts.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject healthView;
    [SerializeField] private Button backBtn;
    [SerializeField] private Slider selectCountSlider;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TMP_Text currentCountTxt;
    [SerializeField] private TMPro.TMP_Text maxCountTxt;
    [SerializeField] private TMPro.TMP_Text summHealthTxt;
    [SerializeField] private TMPro.TMP_Text itemHealthAmountTxt;
    [SerializeField] private Button healthBtn;
    [SerializeField] private TMPro.TMP_Text warningTxt;
    [Tooltip("�������������� ���� �������� ������")]
    [SerializeField] private string maxHealthWarning = "�������� ������";
    [Tooltip("�������������� ���� ������ �����")]
    [SerializeField] private string deadWarning = "����� �����";

    [Header("Inventory")]
    [SerializeField] InventoryController inventoryController;

    [Header("Current Stats")]
    [SerializeField] private PlayerController playerController;

    //������� �������
    MyInventoryItem currentItem;
    //������� ��������� ��������
    private int currItemHealthAmount = 0;
    private int currHealthSumm = 0;
    //������� ���������� ��������� ��� �������
    private int currHealthCount = 0;

    void Start()
    {
        ButtonSettings();
    }

    //��������� ������
    private void ButtonSettings()
    {
        selectCountSlider.onValueChanged.AddListener(UpdateSellPanel);
        healthBtn.onClick.AddListener(HealthBtnClick);
        backBtn.onClick.AddListener(CloseHealthPanel);
    }

    //��������� ��� ������ ����� �������
    public void OpenHealthPanel(MyInventoryItem itm, int itmCount)
    {
        currentItem = itm;

        //������
        itemIcon.sprite = currentItem.itemIcon;

        //��
        currItemHealthAmount = currentItem.poitionHealthAmount;
        itemHealthAmountTxt.text = $"{currItemHealthAmount}";

        //������� ����������� ��������� ������� �� ������������ �������� ������ � �� ���� � ���������� �����
        int missingPlayerHealth = playerController.Health - playerController.CurrentHealth;
        int maxHealth = Mathf.CeilToInt((float)missingPlayerHealth / currItemHealthAmount);
        if (maxHealth > itmCount)
        {
            maxHealth = itmCount;
        }
        if (maxHealth <= 0) 
        {
            maxHealth = 1;
        }

        //��������� �������� �� �������� ��� �������
        int startValue = 1;

        //������� ���������
        selectCountSlider.maxValue = maxHealth;
        selectCountSlider.value = startValue;

        //����� �����
        summHealthTxt.text = $"{currHealthSumm}";

        //���������� � ��������
        maxCountTxt.text = $"{maxHealth}";
        currentCountTxt.text = $"{startValue}";

        //������ ��� ������� ���� ��
        if (playerController.CurrentHealth < playerController.Health && !playerController.IsDead)
        {
            //�������� ������
            healthBtn.SetActive(true);
            warningTxt.SetActive(false);
        }
        else
        {
            //�������� �������
            healthBtn.SetActive(false);
            if (playerController.IsDead)
            {
                //���� ������
                warningTxt.text = deadWarning;
            }
            if(playerController.CurrentHealth >= playerController.Health)
            {
                //���� ���� ��
                warningTxt.text = maxHealthWarning;
            }
            warningTxt.SetActive(true);
        }

        UpdateSellPanel(startValue);
    }

    //��������� ����� ������� �������
    private void UpdateSellPanel(float value)
    {
        currHealthCount = (int)value;

        //���������� � ��������
        currentCountTxt.text = $"{currHealthCount}";

        //������� �������� �����
        currHealthSumm = currHealthCount * currItemHealthAmount;

        //����� �����
        summHealthTxt.text = $"{currHealthSumm}";

        //��������� ������
        if (currHealthCount > 0)
        {
            healthBtn.interactable = true;
        }
        else
        {
            healthBtn.interactable = false;
        }
    }

    //�������
    private void HealthBtnClick()
    {
        if (currHealthCount > 0)
        {
            //���� ��������� ���������� �� �����
            inventoryController.RemoveSelectedHealth(currentItem, currHealthCount);

            //��������� ����������� ���������� ��
            playerController.PlayerHealth(currHealthSumm);

            //������ ����
            AudioController.instance.PlayHealthPotionSound();

            CloseHealthPanel();
        }
    }

    //��������� ������
    private void CloseHealthPanel()
    {
        //���������� ������
        inventoryController.CloseHealthPanel();

        healthView.SetActive(false);
    }

}
