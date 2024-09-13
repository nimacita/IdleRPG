using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellPanelController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameObject sellView;
    [SerializeField] private Button backBtn;
    [SerializeField] private Slider selectCountSlider;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TMP_Text currentCountTxt;
    [SerializeField] private TMPro.TMP_Text maxCountTxt;
    [SerializeField] private TMPro.TMP_Text summCoinTxt;
    [SerializeField] private TMPro.TMP_Text itemPriceTxt;
    [SerializeField] private Button sellBtn;

    [Header("Inventory")]
    [SerializeField] InventoryController inventoryController;

    [Header("Current Stats")]
    [SerializeField] private PlayerCurrentStats playerCurrentStats; 

    //������� �������
    MyInventoryItem currentItem;
    //������� ��������� ��������
    private int currItemPrice = 0;
    private int currSumm = 0;
    //������� ���������� ��������� ��� �������
    private int currSellCount = 0;

    void Start()
    {
        ButtonSettings();
    }

    //��������� ������
    private void ButtonSettings()
    {
        selectCountSlider.onValueChanged.AddListener(UpdateSellPanel);
        sellBtn.onClick.AddListener(SellBtnClick);
        backBtn.onClick.AddListener(CloseSellPanel);
    }

    //��������� ��� ������ ����� �������
    public void OpenSellPanel(MyInventoryItem itm, int itmCount)
    {
        currentItem = itm;

        //��������� �������� �� �������� ��� �������
        int startValue = 1;

        //������� ���������
        selectCountSlider.maxValue = itmCount;
        selectCountSlider.value = startValue;

        //������
        itemIcon.sprite = currentItem.itemIcon;

        //����
        currItemPrice = currentItem.itemPrice;
        itemPriceTxt.text = $"{currItemPrice}";

        //����� �����
        summCoinTxt.text = $"{currSumm}";

        //���������� � ��������
        maxCountTxt.text = $"{itmCount}";
        currentCountTxt.text = $"{startValue}";

        UpdateSellPanel(startValue);
    }

    //��������� ����� ������� �������
    private void UpdateSellPanel(float value)
    {
        currSellCount = (int)value;

        //���������� � ��������
        currentCountTxt.text = $"{currSellCount}";

        //������� �������� �����
        currSumm = currSellCount * currItemPrice;

        //����� �����
        summCoinTxt.text = $"{currSumm}";

        //��������� ������
        if (currSellCount > 0)
        {
            sellBtn.interactable = true;
        }
        else
        {
            sellBtn.interactable = false;
        }
    }

    //�������
    private void SellBtnClick()
    {
        if (currSellCount > 0)
        {
            //���� ��������� ���������� �� �������
            inventoryController.SellSelectedItems(currentItem, currSellCount);

            //��������� ����������� �����
            playerCurrentStats.Coins += currSumm;

            //������ ����
            AudioController.instance.PlayCoinsSound();

            //��������� ���������� �����
            inventoryController.UpdateCurrency();

            CloseSellPanel();
        }
    }

    //��������� ������
    private void CloseSellPanel()
    {
        //��������� ���������� �����
        inventoryController.UpdateCurrency();
        inventoryController.CloseSellPanel();

        sellView.SetActive(false);
    }
}
