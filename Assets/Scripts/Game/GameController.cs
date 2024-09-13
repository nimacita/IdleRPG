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

    //��������� ���������
    private void StartSettings()
    {
        fullInventoryTxt.SetActive(false);
    }

    //��������� ��������� ������
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

    //��������� UI ����������
    private void UpdateUI()
    {
        UpdateFightBtn();
        HealthBtnUpdate();
        UpdateChangeBtn();
        UpdateEnemieFind();
        UpdateCritBtn();
        UpdateMapBtn();
    }

    //��������� ����������� ������ �������������� ��������
    private void HealthBtnUpdate()
    {
        if (!isFight)
        {
            //���� �� � ���
            if (playerController.CurrentHealth < playerController.Health)
            {
                //���� �� ������ ������ �������������
                healthBtn.gameObject.SetActive(true);
            }
            else
            {
                healthBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            //���� � ���
            healthBtn.gameObject.SetActive(false);
        }
    }

    //��������� ����������� ������ ���
    private void UpdateFightBtn()
    {
        if (isFight)
        {
            //���� ���
            runBtnView.SetActive(true);
            fightBtnView.SetActive(false);
        }
        else
        {
            //���� �� ���
            fightBtnView.SetActive(true);
            runBtnView.SetActive(false);
        }
    }

    //��������� ����������� ������ �����
    private void UpdateCritBtn()
    {
        //���� � ���, ������� ������ ���������, ��� �� ������ � ���� �����
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

    //��������� ���� ����
    private void IsCritEnded()
    {
        isCritClicked = false;
        UpdateCritBtn();
    }

    //��������� ����������� ������ �����
    private void UpdateChangeBtn()
    {
        if (!isFight)
        {
            //���� �� � ���
            changeBtn.gameObject.SetActive(false);
        }
        else
        {
            //���� � ���
            changeBtn.gameObject.SetActive(true);
        }
        if (playerController.IsSword)
        {
            //���� � ����� ���
            bowChangeView.SetActive(true);
            swordChangeView.SetActive(false);
        }
        else
        {
            //���� � ����� ���
            swordChangeView.SetActive(true);
            bowChangeView.SetActive(false);
        }
    }

    //��������� ����������� ������ ������
    private void UpdateEnemieFind()
    {
        if (!isFight)
        {
            //���� �� � ���
            enemieFind.SetActive(false);
        }
        else
        {
            //���� � ���
            enemieFind.SetActive(isFind);
        }
    }

    //��������� ������ �����
    private void UpdateMapBtn()
    {
        if (!isFight)
        {
            //���� �� � ���
            mapBtn.gameObject.SetActive(true);
        }
        else
        {
            mapBtn.gameObject.SetActive(false);
        }
    }

    //��������� ������ ������ ������
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

    //������ �� ������ ����� ������
    private void ChangeBtnClick()
    {
        onChangeClicked?.Invoke();
    }

    //������ �� ������ ��������
    private void SettingaBtnClick()
    {
        SetPause(true);
        settingsController.OpenSettingsView();
    }

    //�������� ���
    private void StartFight()
    {
        isFight = true;
        onFightStarted?.Invoke();
        StartFind();
        UpdateUI();

        //������ ������
        AudioController.instance.PlaySelectedMusic(isFight);
    }

    //������������� ���
    private void StopFight()
    {
        isFight = false;
        onFightEnded?.Invoke();
        UpdateUI();
        StopFind();

        //������ ������
        AudioController.instance.PlaySelectedMusic(isFight);
    }

    //����� ����
    private void PlayerDead()
    {
        StopFight();
    }

    //������ �� ������ ���
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

    //������ �� ������ ����
    private void HealthBtnClick()
    {
        onHealthBtnClicked?.Invoke();
        HealthBtnUpdate();

        //������ ����
        AudioController.instance.PlayHealthBtnSound();
    }

    //������ �� ������ �����
    private void CritBtnClicked()
    {
        if (!isCritClicked) 
        {
            //�������� �������
            onCritClicked?.Invoke();
            isCritClicked = true;
            UpdateCritBtn();
        }
    }

    //������ �� ������ ������ ������
    private void SkillTreeClick()
    {
        SetPause(true);
        skillTreeController.OpenSkillTree();

        //������ ����
        AudioController.instance.PlaySkillTreeSound();
    }

    //������ �����
    private void StartFind()
    {
        isFind = true;
        UpdateEnemieFind();
    }

    //��������� �����
    private void StopFind()
    {
        isFind = false;
        UpdateEnemieFind();
    }

    //������ �� ������ ���������
    private void InventoryBtnClick()
    {
        inventoryController.InventoryOpen();
        SetPause(true);

        //������ ����
        AudioController.instance.PlayInventorySound();
    }

    //������ �� ������ �����
    private void MapBtnClick()
    {
        locationController.MapOpen();

        //������ ����
        AudioController.instance.PlayMapBtnSound();
    }

    //�������� �������� ������� ���������
    public void PlayFullInvenoty()
    {
        StartCoroutine("FullInventoryAnim");
    }

    //�������� ���������
    private IEnumerator FullInventoryAnim()
    {
        //�������� �����
        fullInventoryTxt.SetActive(true);
        //������ ��������
        inventoryAnim.Play(fullInventory.name);
        //���� ����� ��������
        yield return new WaitForSeconds(fullInventory.length);
        //��������� �����
        fullInventoryTxt.SetActive(false);
    }

    //����� ���-����
    public void SetPause(bool state)
    {
        onPaused?.Invoke(state);
    }

    public bool IsFight
    {
        get { return isFight; }
    }
}
