using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameObject settingsView;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMPro.TMP_Text currMusicValue;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private TMPro.TMP_Text currSoundValue;
    [SerializeField] private GameObject autoCritBtn;
    [SerializeField] private Sprite btnOn;
    [SerializeField] private Sprite btnOff;

    [Header("Animation Settings")]
    [SerializeField] private Animation settingsAnim;
    [SerializeField] private AnimationClip settingsOn;
    [SerializeField] private AnimationClip settingsOff;

    [Header("Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;

    public static SettingsController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        settingsView.SetActive(false);

        UpdateSettingsView();
        ButtonSettings();
    }

    //настройка кнопок
    private void ButtonSettings()
    {
        autoCritBtn.GetComponent<Button>().onClick.AddListener(AutoCritBtnClick);
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        soundSlider.onValueChanged.AddListener(UpdateSoundVolume);
        closeBtn.onClick.AddListener(CloseSettingsView);
    }

    //обновляем отображение при включении
    public void UpdateSettingsView()
    {
        //обновляем слайдеры
        musicSlider.value = currentStats.MusicVolume;
        currMusicValue.text = $"{Mathf.CeilToInt(currentStats.MusicVolume)}";

        soundSlider.value = currentStats.SoundVolume;
        currSoundValue.text = $"{Mathf.CeilToInt(currentStats.SoundVolume)}";

        //обновляем кнопку
        if (currentStats.IsCritAutomaticly)
        {
            //если криты автоматически то ставим значение вкл
            autoCritBtn.GetComponent<Image>().sprite = btnOn;
        }
        else
        {
            //если криты не автоматически то ставим значение выкл
            autoCritBtn.GetComponent<Image>().sprite = btnOff;
        }
    }

    //обновляем значение музыки
    private void UpdateMusicVolume(float value)
    {
        currentStats.MusicVolume = value;
        AudioController.instance.UpdateMusicVolume(value);
        UpdateSettingsView();
    }

    //обновляем значение звуков
    private void UpdateSoundVolume(float value)
    {
        currentStats.SoundVolume = value;
        AudioController.instance.UpdateAmbientVolume(value);
        UpdateSettingsView();
    }

    //нажали на кнопку критов
    private void AutoCritBtnClick()
    {
        //переключаем значение
        currentStats.IsCritAutomaticly = !currentStats.IsCritAutomaticly;
        UpdateSettingsView();
    }

    //закрываем экран настроек
    private void CloseSettingsView()
    {
        StartCoroutine(CloseAnim());
    }

    //анимация закрытия
    private IEnumerator CloseAnim()
    {
        settingsAnim.Play(settingsOff.name);
        yield return new WaitForSeconds(settingsOff.length);
        GameController.instance.SetPause(false);
        settingsView.SetActive(false);
    }

    //открываем экран настроек
    public void OpenSettingsView()
    {
        settingsView.SetActive(true);
        settingsAnim.Play(settingsOn.name);
    }
}
