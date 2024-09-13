using Assets.HeroEditor.Common.Scripts.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [Header("Game View Settings")]
    [SerializeField] private Button healthBtn;
    [SerializeField] private GameObject enemieFind;
    [SerializeField] private Button critBtn;
    [SerializeField] private Button mapBtn;
    [SerializeField] private Button settBtn;

    [Header("Inventory Btn")]
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private GameObject fullInventoryTxt;
    [SerializeField] private Animation inventoryAnim;
    [SerializeField] private AnimationClip fullInventory;

    [Header("Fight Btn")]
    [SerializeField] private Button fightBtn;
    [SerializeField] private GameObject fightBtnView;
    [SerializeField] private GameObject runBtnView;

    [Header("Change Btn")]
    [SerializeField] private Button changeBtn;
    [SerializeField] private GameObject swordChangeView;
    [SerializeField] private GameObject bowChangeView;

    [Header("Skill Tree Btn")]
    [SerializeField] private Button skillTreeBtn;
    [SerializeField] private TMPro.TMP_Text pointsCount;
    [SerializeField] private Animation skillTreeAnim;
    [SerializeField] private AnimationClip skillTreeNotif;
    [SerializeField] private AnimationClip skillTreeStay;

    [Header("Current Player Stats")]
    [SerializeField] private PlayerCurrentStats playerCurrentStats;

    //bools
    private bool isFight = false;
    private bool isFind = false;
    private bool isCritClicked = false;

    public static Action onFightStarted;
    public static Action onFightEnded;
    public static Action onChangeClicked;
    public static Action onHealthBtnClicked;
    public static Action<bool> onPaused;
    public static Action onCritClicked;

    private PlayerController playerController;
    private InventoryController inventoryController;
    private LocationController locationController;
    private SkillTreeController skillTreeController;
    private SettingsController settingsController;
    public static GameController instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        PlayerController.onPlayerDead += PlayerDead;
        PlayerController.onPlayerChanged += UpdateChangeBtn;
        EnemieSpawner.onStopFinded += StopFind;
        EnemieSpawner.onStartFinded += StartFind;
        PlayerController.onCritEnded += IsCritEnded;
        PlayerController.onCritReady += UpdateCritBtn;
    }

    private void OnDisable()
    {
        PlayerController.onPlayerDead -= PlayerDead;
        PlayerController.onPlayerChanged -= UpdateChangeBtn;
        EnemieSpawner.onStopFinded -= StopFind;
        EnemieSpawner.onStartFinded -= StartFind;
        PlayerController.onCritEnded -= IsCritEnded;
        PlayerController.onCritReady -= UpdateCritBtn;
    }

    void Start()
    {
        playerController = PlayerController.instance;
        inventoryController = InventoryController.instance;
        locationController = LocationController.instance;
        skillTreeController = SkillTreeController.instance;
        settingsController = SettingsController.instance;

        ButtonSettings();
        StartSettings();
        UpdateSkillTreeBtn();
        UpdateUI();
    }

    //начальные настройки
    private void StartSettings()
    {
        fullInventoryTxt.SetActive(false);
    }

    //начальные настройки кнопок
    private void ButtonSettings()
    {
        fightBtn.onClick.AddListener(FightBtnClick);
        changeBtn.onClick.AddListener(ChangeBtnClick);
        healthBtn.onClick.AddListener(HealthBtnClick);
        inventoryBtn.onClick.AddListener(InventoryBtnClick);
        critBtn.onClick.AddListener(CritBtnClicked);
        mapBtn.onClick.AddListener(MapBtnClick);
        skillTreeBtn.onClick.AddListener(SkillTreeClick);
        settBtn.onClick.AddListener(SettingaBtnClick);
    }

    //обновляем UI компоненты
    private void UpdateUI()
    {
        UpdateFightBtn();
        HealthBtnUpdate();
        UpdateChangeBtn();
        UpdateEnemieFind();
        UpdateCritBtn();
        UpdateMapBtn();
    }

    //обновляем отображение кнопки восстановления здоровья
    private void HealthBtnUpdate()
    {
        if (!isFight)
        {
            //если не в бою
            if (playerController.CurrentHealth < playerController.Health)
            {
                //если хп игрока меньше максимального
                healthBtn.gameObject.SetActive(true);
            }
            else
            {
                healthBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            //если в бою
            healthBtn.gameObject.SetActive(false);
        }
    }

    //обновляем отображение кнопки боя
    private void UpdateFightBtn()
    {
        if (isFight)
        {
            //если бой
            runBtnView.SetActive(true);
            fightBtnView.SetActive(false);
        }
        else
        {
            //если не бой
            fightBtnView.SetActive(true);
            runBtnView.SetActive(false);
        }
    }

    //обновляем отображения кнопки крита
    private void UpdateCritBtn()
    {
        //если в бою, выбрана нужная настройка, уже не нажали и крит готов
        if (isFight && !playerCurrentStats.isCritAutomaticly && 
            !isCritClicked && playerController.IsCritReady)
        {
            critBtn.gameObject.SetActive(true);
        }
        else
        {
            critBtn.gameObject.SetActive(false);
        }
    }

    //Совершили крит удар
    private void IsCritEnded()
    {
        isCritClicked = false;
        UpdateCritBtn();
    }

    //обновляем отображение кнопки смены
    private void UpdateChangeBtn()
    {
        if (!isFight)
        {
            //если не в бою
            changeBtn.gameObject.SetActive(false);
        }
        else
        {
            //если в бою
            changeBtn.gameObject.SetActive(true);
        }
        if (playerController.IsSword)
        {
            //если в руках меч
            bowChangeView.SetActive(true);
            swordChangeView.SetActive(false);
        }
        else
        {
            //если в руках лук
            swordChangeView.SetActive(true);
            bowChangeView.SetActive(false);
        }
    }

    //обновляем отображение поиска врагов
    private void UpdateEnemieFind()
    {
        if (!isFight)
        {
            //если не в бою
            enemieFind.SetActive(false);
        }
        else
        {
            //если в бою
            enemieFind.SetActive(isFind);
        }
    }

    //обновляем кнопку карты
    private void UpdateMapBtn()
    {
        if (!isFight)
        {
            //если не в бою
            mapBtn.gameObject.SetActive(true);
        }
        else
        {
            mapBtn.gameObject.SetActive(false);
        }
    }

    //обновляем кнопку дерева умений
    public void UpdateSkillTreeBtn()
    {
        int points = playerCurrentStats.Points;
        pointsCount.text = $"{points}";

        if (points > 0)
        {
            if(!skillTreeAnim.isPlaying) skillTreeAnim.Play(skillTreeNotif.name);
        }
        else
        {
            skillTreeAnim.Play(skillTreeStay.name);
        }
    }

    //нажали на кнопку смены оружия
    private void ChangeBtnClick()
    {
        onChangeClicked?.Invoke();
    }

    //нажали на кнопку настроек
    private void SettingaBtnClick()
    {
        SetPause(true);
        settingsController.OpenSettingsView();
    }

    //начинаем бой
    private void StartFight()
    {
        isFight = true;
        onFightStarted?.Invoke();
        StartFind();
        UpdateUI();

        //играем музыку
        AudioController.instance.PlaySelectedMusic(isFight);
    }

    //останавливаем бой
    private void StopFight()
    {
        isFight = false;
        onFightEnded?.Invoke();
        UpdateUI();
        StopFind();

        //играем музыку
        AudioController.instance.PlaySelectedMusic(isFight);
    }

    //игрок умер
    private void PlayerDead()
    {
        StopFight();
    }

    //нажали на кнопку боя
    private void FightBtnClick()
    {
        if (isFight)
        {
            StopFight();
        }
        else
        {
            StartFight();
        }
    }

    //нажали на кнопку хила
    private void HealthBtnClick()
    {
        onHealthBtnClicked?.Invoke();
        HealthBtnUpdate();

        //играем звук
        AudioController.instance.PlayHealthBtnSound();
    }

    //нажали на кнопку крита
    private void CritBtnClicked()
    {
        if (!isCritClicked) 
        {
            //вызываем событие
            onCritClicked?.Invoke();
            isCritClicked = true;
            UpdateCritBtn();
        }
    }

    //нажали на кнопку дерева умений
    private void SkillTreeClick()
    {
        SetPause(true);
        skillTreeController.OpenSkillTree();

        //играем звук
        AudioController.instance.PlaySkillTreeSound();
    }

    //начали поиск
    private void StartFind()
    {
        isFind = true;
        UpdateEnemieFind();
    }

    //закончили поиск
    private void StopFind()
    {
        isFind = false;
        UpdateEnemieFind();
    }

    //нажали на кнопку инвенторя
    private void InventoryBtnClick()
    {
        inventoryController.InventoryOpen();
        SetPause(true);

        //играем звук
        AudioController.instance.PlayInventorySound();
    }

    //нажали на кнопку карты
    private void MapBtnClick()
    {
        locationController.MapOpen();

        //играем звук
        AudioController.instance.PlayMapBtnSound();
    }

    //включаем анимацию полного инвентаря
    public void PlayFullInvenoty()
    {
        StartCoroutine("FullInventoryAnim");
    }

    //анимация инвенторя
    private IEnumerator FullInventoryAnim()
    {
        //включаем текст
        fullInventoryTxt.SetActive(true);
        //играем анимацию
        inventoryAnim.Play(fullInventory.name);
        //ждем конца анимации
        yield return new WaitForSeconds(fullInventory.length);
        //выключаем текст
        fullInventoryTxt.SetActive(false);
    }

    //пауза вкл-выкл
    public void SetPause(bool state)
    {
        onPaused?.Invoke(state);
    }

    public bool IsFight
    {
        get { return isFight; }
    }
}
