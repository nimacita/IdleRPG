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

    //��������� ������
    private void ButtonSettings()
    {
        autoCritBtn.GetComponent<Button>().onClick.AddListener(AutoCritBtnClick);
        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        soundSlider.onValueChanged.AddListener(UpdateSoundVolume);
        closeBtn.onClick.AddListener(CloseSettingsView);
    }

    //��������� ����������� ��� ���������
    public void UpdateSettingsView()
    {
        //��������� ��������
        musicSlider.value = currentStats.MusicVolume;
        currMusicValue.text = $"{Mathf.CeilToInt(currentStats.MusicVolume)}";

        soundSlider.value = currentStats.SoundVolume;
        currSoundValue.text = $"{Mathf.CeilToInt(currentStats.SoundVolume)}";

        //��������� ������
        if (currentStats.IsCritAutomaticly)
        {
            //���� ����� ������������� �� ������ �������� ���
            autoCritBtn.GetComponent<Image>().sprite = btnOn;
        }
        else
        {
            //���� ����� �� ������������� �� ������ �������� ����
            autoCritBtn.GetComponent<Image>().sprite = btnOff;
        }
    }

    //��������� �������� ������
    private void UpdateMusicVolume(float value)
    {
        currentStats.MusicVolume = value;
        AudioController.instance.UpdateMusicVolume(value);
        UpdateSettingsView();
    }

    //��������� �������� ������
    private void UpdateSoundVolume(float value)
    {
        currentStats.SoundVolume = value;
        AudioController.instance.UpdateAmbientVolume(value);
        UpdateSettingsView();
    }

    //������ �� ������ ������
    private void AutoCritBtnClick()
    {
        //����������� ��������
        currentStats.IsCritAutomaticly = !currentStats.IsCritAutomaticly;
        UpdateSettingsView();
    }

    //��������� ����� ��������
    private void CloseSettingsView()
    {
        StartCoroutine(CloseAnim());
    }

    //�������� ��������
    private IEnumerator CloseAnim()
    {
        settingsAnim.Play(settingsOff.name);
        yield return new WaitForSeconds(settingsOff.length);
        GameController.instance.SetPause(false);
        settingsView.SetActive(false);
    }

    //��������� ����� ��������
    public void OpenSettingsView()
    {
        settingsView.SetActive(true);
        settingsAnim.Play(settingsOn.name);
    }
}
