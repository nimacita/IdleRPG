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

    //текущий предмет
    MyInventoryItem currentItem;
    //текущая стоимость предмета
    private int currItemPrice = 0;
    private int currSumm = 0;
    //текущее количество предметов для продажи
    private int currSellCount = 0;

    void Start()
    {
        ButtonSettings();
    }

    //настройка кнопок
    private void ButtonSettings()
    {
        selectCountSlider.onValueChanged.AddListener(UpdateSellPanel);
        sellBtn.onClick.AddListener(SellBtnClick);
        backBtn.onClick.AddListener(CloseSellPanel);
    }

    //обновляем вид панели когда открыли
    public void OpenSellPanel(MyInventoryItem itm, int itmCount)
    {
        currentItem = itm;

        //начальное значение на слайдере при открыти
        int startValue = 1;

        //слайдер настройки
        selectCountSlider.maxValue = itmCount;
        selectCountSlider.value = startValue;

        //иконка
        itemIcon.sprite = currentItem.itemIcon;

        //цена
        currItemPrice = currentItem.itemPrice;
        itemPriceTxt.text = $"{currItemPrice}";

        //общая сумма
        summCoinTxt.text = $"{currSumm}";

        //количество у слайдера
        maxCountTxt.text = $"{itmCount}";
        currentCountTxt.text = $"{startValue}";

        UpdateSellPanel(startValue);
    }

    //обновляем когда двигаем слайдер
    private void UpdateSellPanel(float value)
    {
        currSellCount = (int)value;

        //количество у слайдера
        currentCountTxt.text = $"{currSellCount}";

        //считаем итоговую сумму
        currSumm = currSellCount * currItemPrice;

        //общая сумма
        summCoinTxt.text = $"{currSumm}";

        //включение кнопки
        if (currSellCount > 0)
        {
            sellBtn.interactable = true;
        }
        else
        {
            sellBtn.interactable = false;
        }
    }

    //продаем
    private void SellBtnClick()
    {
        if (currSellCount > 0)
        {
            //если предметов достаточно то продаем
            inventoryController.SellSelectedItems(currentItem, currSellCount);

            //добавляем необходимую сумму
            playerCurrentStats.Coins += currSumm;

            //играем звук
            AudioController.instance.PlayCoinsSound();

            //обновляем количество денег
            inventoryController.UpdateCurrency();

            CloseSellPanel();
        }
    }

    //закрываем панель
    private void CloseSellPanel()
    {
        //обновляем количество денег
        inventoryController.UpdateCurrency();
        inventoryController.CloseSellPanel();

        sellView.SetActive(false);
    }
}
