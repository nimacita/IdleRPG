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
    [Tooltip("Предупреждение если здоровье полное")]
    [SerializeField] private string maxHealthWarning = "Здоровье Полное";
    [Tooltip("Предупреждение если игррок мертв")]
    [SerializeField] private string deadWarning = "Игрок Мертв";

    [Header("Inventory")]
    [SerializeField] InventoryController inventoryController;

    [Header("Current Stats")]
    [SerializeField] private PlayerController playerController;

    //текущий предмет
    MyInventoryItem currentItem;
    //текущая стоимость предмета
    private int currItemHealthAmount = 0;
    private int currHealthSumm = 0;
    //текущее количество предметов для продажи
    private int currHealthCount = 0;

    void Start()
    {
        ButtonSettings();
    }

    //настройка кнопок
    private void ButtonSettings()
    {
        selectCountSlider.onValueChanged.AddListener(UpdateSellPanel);
        healthBtn.onClick.AddListener(HealthBtnClick);
        backBtn.onClick.AddListener(CloseHealthPanel);
    }

    //обновляем вид панели когда открыли
    public void OpenHealthPanel(MyInventoryItem itm, int itmCount)
    {
        currentItem = itm;

        //иконка
        itemIcon.sprite = currentItem.itemIcon;

        //хп
        currItemHealthAmount = currentItem.poitionHealthAmount;
        itemHealthAmountTxt.text = $"{currItemHealthAmount}";

        //считаем максимально возможное лечение от недостающего здоровья игрока и от силы и количества зелья
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

        //начальное значение на слайдере при открыти
        int startValue = 1;

        //слайдер настройки
        selectCountSlider.maxValue = maxHealth;
        selectCountSlider.value = startValue;

        //общая сумма
        summHealthTxt.text = $"{currHealthSumm}";

        //количество у слайдера
        maxCountTxt.text = $"{maxHealth}";
        currentCountTxt.text = $"{startValue}";

        //кнопка или надпись макс хп
        if (playerController.CurrentHealth < playerController.Health && !playerController.IsDead)
        {
            //включаем кнопку
            healthBtn.SetActive(true);
            warningTxt.SetActive(false);
        }
        else
        {
            //включаем надпись
            healthBtn.SetActive(false);
            if (playerController.IsDead)
            {
                //если мертвы
                warningTxt.text = deadWarning;
            }
            if(playerController.CurrentHealth >= playerController.Health)
            {
                //если макс хп
                warningTxt.text = maxHealthWarning;
            }
            warningTxt.SetActive(true);
        }

        UpdateSellPanel(startValue);
    }

    //обновляем когда двигаем слайдер
    private void UpdateSellPanel(float value)
    {
        currHealthCount = (int)value;

        //количество у слайдера
        currentCountTxt.text = $"{currHealthCount}";

        //считаем итоговую сумму
        currHealthSumm = currHealthCount * currItemHealthAmount;

        //общая сумма
        summHealthTxt.text = $"{currHealthSumm}";

        //включение кнопки
        if (currHealthCount > 0)
        {
            healthBtn.interactable = true;
        }
        else
        {
            healthBtn.interactable = false;
        }
    }

    //продаем
    private void HealthBtnClick()
    {
        if (currHealthCount > 0)
        {
            //если предметов достаточно то хилим
            inventoryController.RemoveSelectedHealth(currentItem, currHealthCount);

            //добавляем необходимое количество хп
            playerController.PlayerHealth(currHealthSumm);

            //играем звук
            AudioController.instance.PlayHealthPotionSound();

            CloseHealthPanel();
        }
    }

    //закрываем панель
    private void CloseHealthPanel()
    {
        //заакрываем панель
        inventoryController.CloseHealthPanel();

        healthView.SetActive(false);
    }

}
