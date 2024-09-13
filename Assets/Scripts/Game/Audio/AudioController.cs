using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public enum AmbientTheme
{
    forest,
    swamp,
    darkForest,
    pirateShip
}

public class AudioController : MonoBehaviour
{

    [Header("Musics")]
    [SerializeField] private AudioSource ambientMusic;
    [SerializeField] private AudioSource fightMusic;

    [Header("Sounds")]
    [Space(2)]
    [Header("Swords Sounds")]
    [SerializeField] private AudioSource swordSwing;
    [SerializeField] private AudioClip defaultSwingSound;
    [SerializeField] private AudioSource swordHit;
    [SerializeField] private AudioClip defaultSwordHitSound;

    [Header("Bows Sounds")]
    [SerializeField] private AudioSource bowDraw;
    [SerializeField] private AudioSource bowShot;

    [Header("Enemie Sounds")]
    [SerializeField] private AudioSource enemieHit;

    [Header("Player Sounds")]
    [SerializeField] private AudioSource playerTakeHitSound;
    [SerializeField] private AudioSource changeWeaponSound;

    [Header("Game Sounds")]
    [SerializeField] private AudioSource healthBtnSound;
    [SerializeField] private AudioSource inventoryBtnSound;
    [SerializeField] private AudioSource mapBtnSound;
    [SerializeField] private AudioSource skillTreeBtnSound;
    [SerializeField] private AudioSource skillTreeSound;
    [SerializeField] private AudioSource levelUpSound;
    [SerializeField] private AudioSource healthPotionSound;
    [SerializeField] private AudioSource coinsSound;
    [SerializeField] private AudioSource equipPutSound;

    [Header("Armor Sounds")]
    [SerializeField] private AudioSource blowToArmor;

    [Header("Ambietns Sounds")]
    [SerializeField] private AudioSource forestAmbient;
    [SerializeField] private AudioSource swampAmbient;
    [SerializeField] private AudioSource darkForestAmbient;
    [SerializeField] private AudioSource pirateShipAmbient;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;


    //Mixer Parameteres
    private string musictVolumeParamenter = "MusicVolume";
    private string fightMusictVolumeParamenter = "FightMusicVolume";
    private string forestVolumeParamenter = "ForestAmbientVolume";
    private string swampVolumeParamenter = "SwampAmbientVolume";
    private string darkForestVolumeParamenter = "DarkForestAmbientVolume";
    private string pirateShipVolumeParamenter = "PirateShipAmbientVolume";

    //Current Theme
    [SerializeField]
    private AmbientTheme currPlayingTheme;
    private bool currIsFight = false;
    private AudioSource currPlayingMusic;

    //stats
    private float startMusicVolume;
    private float startFightMusicVolume;
    private float startForestAmbientVolume;
    private float startSwampAmbientVolume;
    private float startDarkForestAmbientVolume;
    private float startPirateShipAmbientVolume;
    private float muteVolume = -80f;
    private float ambientFadetime = 1f;
    private float musicStartFadetime = 1f;
    private float musicStopFadetime = 3f;


    public static AudioController instance;

    void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartVolume();

        //���������� ������� ������� �����
        DefineAmbient(false);

        //��������� ������
        PlaySelectedMusic(false, false);
    }

    //��������� ��������� ���������
    private void StartVolume()
    {
        //�������� ��������� ��������� ���� ������� ������
        audioMixer.GetFloat(forestVolumeParamenter, out startForestAmbientVolume);
        audioMixer.GetFloat(swampVolumeParamenter, out startSwampAmbientVolume);
        audioMixer.GetFloat(darkForestVolumeParamenter, out startDarkForestAmbientVolume);
        audioMixer.GetFloat(pirateShipVolumeParamenter, out startPirateShipAmbientVolume);
        audioMixer.GetFloat(musictVolumeParamenter, out startMusicVolume);
        audioMixer.GetFloat(fightMusictVolumeParamenter, out startFightMusicVolume);
    }

    //���������� ����� ������� ����� ������ ������ �� ������
    public void DefineAmbient(bool isStopLast = true)
    {
        int locationIndex = currentStats.CurrentLocationIndex;

        switch (locationIndex)
        {
            case 0:
                PlayAmbientSelectedSounds(AmbientTheme.forest, isStopLast);
                break;
            case 1:
                PlayAmbientSelectedSounds(AmbientTheme.swamp, isStopLast);
                break;
            case 2:
                PlayAmbientSelectedSounds(AmbientTheme.darkForest, isStopLast);
                break;
            case 3:
                PlayAmbientSelectedSounds(AmbientTheme.pirateShip, isStopLast);
                break;
        }
    }

    //����������� ��������� ���������� ������
    public void PlayAmbientSelectedSounds(AmbientTheme currTheme, bool isStopLast = true)
    {
        AudioSource currSource = null;
        string currParameter = "";
        float targetVolume = muteVolume;

        //����������� ������� �������� ����
        switch (currTheme)
        {
            case AmbientTheme.forest:
                {
                    currSource = forestAmbient;
                    currParameter = forestVolumeParamenter;
                    targetVolume = startForestAmbientVolume;
                    break;
                }
            case AmbientTheme.swamp:
                {
                    currSource = swampAmbient;
                    currParameter = swampVolumeParamenter;
                    targetVolume = startSwampAmbientVolume;
                    break;
                }
            case AmbientTheme.darkForest:
                {
                    currSource = darkForestAmbient;
                    currParameter = darkForestVolumeParamenter;
                    targetVolume = startDarkForestAmbientVolume;
                    break;
                }
            case AmbientTheme.pirateShip:
                {
                    currSource = pirateShipAmbient;
                    currParameter = pirateShipVolumeParamenter;
                    targetVolume = startPirateShipAmbientVolume;
                    break;
                }
        }

        //������������� ���������� ���� �����
        if (isStopLast)
        {
            if (currPlayingTheme != currTheme) 
            {
                StopLastAmbient(currPlayingTheme);
            }
        }

        //������������� ��������� �� ��������
        currSource.volume = ((float)currentStats.SoundVolume / 10f);

        if (currPlayingTheme != currTheme || !isStopLast) 
        {
            //StopCoroutine(StartAmbientSound(currSource, currParameter, targetVolume));
            //��������� ��������
            StartCoroutine(StartAmbientSound(currSource, currParameter, targetVolume));
        }

        //������ �������� �������
        currPlayingTheme = currTheme;
    }

    //����������� ��������� ���������� ������
    public void PlaySelectedMusic(bool isFight,bool isStopLast = true)
    {
        AudioSource currSource, stopSource = null;
        string currParameter, stopParameter = "";
        float targetVolume, stopVolume = muteVolume;

        //����������� ������� ������ � ����������� �� ��������
        if (isFight)
        {
            //���� � ���
            currSource = fightMusic;
            currParameter = fightMusictVolumeParamenter;
            targetVolume = startFightMusicVolume;

            stopSource = ambientMusic;
            stopParameter = musictVolumeParamenter;
            stopVolume = startMusicVolume;
        }
        else
        {
            //���� �� � ���
            currSource = ambientMusic;
            currParameter = musictVolumeParamenter;
            targetVolume = startMusicVolume;

            stopSource = fightMusic;
            stopParameter = fightMusictVolumeParamenter;
            stopVolume = startFightMusicVolume;
        }

        currPlayingMusic = currSource;

        //������������� ���������� ���� �����
        if (isStopLast)
        {
            /*if (isFight != currIsFight) 
            {
                StartCoroutine(StopMusic(stopSource, stopParameter, stopVolume));
            }*/
            StartCoroutine(StopMusic(stopSource, stopParameter, stopVolume));
        }

        //������������� ��������� �� ��������
        currSource.volume = ((float)currentStats.MusicVolume / 10f);

        /*if (isFight != currIsFight || !isStopLast)
        {
            StartCoroutine(StartMusic(currSource, currParameter, targetVolume));
        }*/
        //StopCoroutine(StopMusic(currSource, currParameter, targetVolume));
        StartCoroutine(StartMusic(currSource, currParameter, targetVolume));

        //������������� ��������
        currIsFight = isFight;
    }

    //������������� ������������ ������� ������� ������
    private void StopLastAmbient(AmbientTheme currPlayingTheme)
    {
        AudioSource currSource = null;
        string currParameter = "";
        float startVolume = muteVolume;

        //����������� ������� �������� ����
        switch (currPlayingTheme)
        {
            case AmbientTheme.forest:
                {
                    currSource = forestAmbient;
                    currParameter = forestVolumeParamenter;
                    startVolume = startForestAmbientVolume;
                    break;
                }
            case AmbientTheme.swamp:
                {
                    currSource = swampAmbient;
                    currParameter = swampVolumeParamenter;
                    startVolume = startSwampAmbientVolume;
                    break;
                }
            case AmbientTheme.darkForest:
                {
                    currSource = darkForestAmbient;
                    currParameter = darkForestVolumeParamenter;
                    startVolume = startDarkForestAmbientVolume;
                    break;
                }
            case AmbientTheme.pirateShip:
                {
                    currSource = pirateShipAmbient;
                    currParameter = pirateShipVolumeParamenter;
                    startVolume = startPirateShipAmbientVolume;
                    break;
                }
        }

        //��������� �������� ��������� ������� ����
        StartCoroutine(StopAmbientSound(currSource, currParameter, startVolume));
    }

    //�������� ������� ������� ������
    private IEnumerator StartAmbientSound(AudioSource ambientSource, string parametr, float targetVolume)
    {
        //���� ������ �� ������
        if (ambientSource != null) 
        {
            //�������� ������������
            ambientSource.Play();
            float currentTime = 0f;

            // ���������� ��������� �� ����������� �������
            audioMixer.SetFloat(parametr, muteVolume);

            // ���������� ����������� ��������� �� targetVolume
            while (currentTime < ambientFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(muteVolume, targetVolume, currentTime / ambientFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // ��������, ��� ��������� ����� ����� targetVolume
            audioMixer.SetFloat(parametr, targetVolume);
        }
    }

    //�������� ��������� ������� ������
    private IEnumerator StopAmbientSound(AudioSource ambientSource, string parametr, float startVolume)
    {
        if (ambientSource != null) 
        {
            float currentTime = 0f;

            float targetVolume = muteVolume;

            // ���������� ����������� ��������� �� targetVolume
            while (currentTime < ambientFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / ambientFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // ��������, ��� ��������� ����� ����� targetVolume
            audioMixer.SetFloat(parametr, targetVolume);

            //�������������
            ambientSource.Stop();
        }
    }

    //�������� ������� ������
    private IEnumerator StartMusic(AudioSource musicSource, string parametr, float targetVolume)
    {
        //���� ������ �� ������
        if (musicSource != null)
        {
            //�������� ���������������� �� ������
            if(!musicSource.isPlaying) musicSource.Play();

            float currentTime = 0f;

            // ���������� ��������� �� ����������� �������
            audioMixer.SetFloat(parametr, muteVolume);

            // ���������� ����������� ��������� �� targetVolume
            while (currentTime < musicStartFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(muteVolume, targetVolume, currentTime / musicStartFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // ��������, ��� ��������� ����� ����� targetVolume
            audioMixer.SetFloat(parametr, targetVolume);
        }
    }

    //�������� ��������� ������
    private IEnumerator StopMusic(AudioSource musicSource, string parametr, float startVolume)
    {
        if (musicSource != null)
        {
            float currentTime = 0f;

            float targetVolume = muteVolume;

            // ���������� ����������� ��������� �� targetVolume
            while (currentTime < musicStopFadetime)
            {
                if (musicSource != currPlayingMusic) 
                {
                    currentTime += Time.deltaTime;
                    float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / musicStopFadetime);
                    audioMixer.SetFloat(parametr, newVolume);
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            // ��������, ��� ��������� ����� ����� targetVolume
            audioMixer.SetFloat(parametr, targetVolume);

            //�������������
            //musicSource.Stop();
        }
    }

    //��������� ���� ������� ������
    public void UpdateAmbientVolume(float value)
    {
        forestAmbient.volume = ((float)value / 10f);
        swampAmbient.volume = ((float)value / 10f);
        darkForestAmbient.volume = ((float)value / 10f);
        pirateShipAmbient.volume = ((float)value / 10f);
    }

    //��������� ���� ������� ������
    public void UpdateMusicVolume(float value)
    {
        ambientMusic.volume = ((float)value / 10f);
        fightMusic.volume = ((float)value / 10f);
    }

    //������ ��������� ���� ������ ����
    public void PlaySwordSwingSound(AudioClip swingSound)
    {
        if(swingSound == null)
        {
            swordSwing.clip = defaultSwingSound;
        }
        else
        {
            swordSwing.clip = swingSound;
        }
        PlaySound(swordSwing);
    }

    //������ ��������� ���� ����� ����
    public void PlaySwordHitSound(AudioClip strikeSound)
    {
        if (strikeSound == null)
        {
            swordHit.clip = defaultSwordHitSound;
        }
        else
        {
            swordHit.clip = strikeSound;
        }
        PlaySound(swordHit);
    }

    //������ ��������� ���� ����������� ����
    public void PlayBowDrawSound(AudioClip drawSound)
    {
        if (drawSound == null) return;
        bowDraw.clip = drawSound;
        PlaySound(bowDraw);
    }

    //������ ��������� ���� �������� �� ���� ����
    public void PlayBowShotSound(AudioClip shotSound)
    {
        if (shotSound == null) return;
        bowShot.clip = shotSound;
        PlaySound(bowShot);
    }

    //������ ��������� ���� ����� �����
    public void PlayEnemieHitSound(AudioClip hitSound)
    {
        if (hitSound == null) return;
        enemieHit.clip = hitSound;
        PlaySound(enemieHit);
    }

    //������ ��������� ���� ����� �� �����
    public void PlayBlowToArmortSound(AudioClip blowToArmorSound)
    {
        if (blowToArmorSound == null) return;
        blowToArmor.clip = blowToArmorSound;
        PlaySound(blowToArmor);
    }

    //������ ��������� ���� ����� �� ������
    public void PlayPlayerTakeHitSound(AudioClip takeHit)
    {
        playerTakeHitSound.clip = takeHit;
        PlaySound(playerTakeHitSound);
    }

    //������ ��������� ���� ����� �� ������
    public void PlayChangeWeaponSound(AudioClip change)
    {
        if (!changeWeaponSound.isPlaying) {
            changeWeaponSound.clip = change;
            PlaySound(changeWeaponSound);
        }
    }

    //������ ���� �����
    public void PlayCoinsSound()
    {
        PlaySound(coinsSound);
    }

    //������ ���� ������ �������
    public void PlaySkillTreeSound()
    {
        PlaySound(skillTreeSound);
    }

    //������ ���� ������ �������
    public void PlayHealthBtnSound()
    {
        PlaySound(healthBtnSound);
    }

    //������ ���� ����� ���������
    public void PlayHealthPotionSound()
    {
        PlaySound(healthPotionSound);
    }

    //������ ���� ������ ���������
    public void PlayInventorySound()
    {
        PlaySound(inventoryBtnSound);
    }

    //������ ���� �������� ������
    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound);
    }

    //������ ���� ������ �������
    public void PlayMapBtnSound()
    {
        PlaySound(mapBtnSound);
    }

    //������ ���� ������ ������
    public void PlaySkillTreeBtnSound()
    {
        PlaySound(skillTreeBtnSound);
    }

    //������ ���� ����������
    public void PlayEquipPutSound()
    {
        PlaySound(equipPutSound);
    }

    //������ ���� � ������ ��������� �� �������
    private void PlaySound(AudioSource source)
    {
        source.volume = ((float)currentStats.SoundVolume / 10f);
        source.Play();
    }
}
