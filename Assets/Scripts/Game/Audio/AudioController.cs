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

        //определяем текущие фоновые звуки
        DefineAmbient(false);

        //запускаем музыку
        PlaySelectedMusic(false, false);
    }

    //начальные параметры громкости
    private void StartVolume()
    {
        //получаем начальную громкость всех фоновых звуков
        audioMixer.GetFloat(forestVolumeParamenter, out startForestAmbientVolume);
        audioMixer.GetFloat(swampVolumeParamenter, out startSwampAmbientVolume);
        audioMixer.GetFloat(darkForestVolumeParamenter, out startDarkForestAmbientVolume);
        audioMixer.GetFloat(pirateShipVolumeParamenter, out startPirateShipAmbientVolume);
        audioMixer.GetFloat(musictVolumeParamenter, out startMusicVolume);
        audioMixer.GetFloat(fightMusictVolumeParamenter, out startFightMusicVolume);
    }

    //определяем какие фоновые звуки должны играть на старте
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

    //проигрываем выбранную амбиентную музыку
    public void PlayAmbientSelectedSounds(AmbientTheme currTheme, bool isStopLast = true)
    {
        AudioSource currSource = null;
        string currParameter = "";
        float targetVolume = muteVolume;

        //присавиваем текущие значения темы
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

        //останалвиваем предедущюу если нужно
        if (isStopLast)
        {
            if (currPlayingTheme != currTheme) 
            {
                StopLastAmbient(currPlayingTheme);
            }
        }

        //устанавливаем громкость из настроек
        currSource.volume = ((float)currentStats.SoundVolume / 10f);

        if (currPlayingTheme != currTheme || !isStopLast) 
        {
            //StopCoroutine(StartAmbientSound(currSource, currParameter, targetVolume));
            //запускаем карутину
            StartCoroutine(StartAmbientSound(currSource, currParameter, targetVolume));
        }

        //меняем значение текущей
        currPlayingTheme = currTheme;
    }

    //проигрываем выбранную амбиентную музыку
    public void PlaySelectedMusic(bool isFight,bool isStopLast = true)
    {
        AudioSource currSource, stopSource = null;
        string currParameter, stopParameter = "";
        float targetVolume, stopVolume = muteVolume;

        //присавиваем текущие музыки в зависимости от ситуации
        if (isFight)
        {
            //если в бою
            currSource = fightMusic;
            currParameter = fightMusictVolumeParamenter;
            targetVolume = startFightMusicVolume;

            stopSource = ambientMusic;
            stopParameter = musictVolumeParamenter;
            stopVolume = startMusicVolume;
        }
        else
        {
            //если не в бою
            currSource = ambientMusic;
            currParameter = musictVolumeParamenter;
            targetVolume = startMusicVolume;

            stopSource = fightMusic;
            stopParameter = fightMusictVolumeParamenter;
            stopVolume = startFightMusicVolume;
        }

        currPlayingMusic = currSource;

        //останалвиваем предедущюу если нужно
        if (isStopLast)
        {
            /*if (isFight != currIsFight) 
            {
                StartCoroutine(StopMusic(stopSource, stopParameter, stopVolume));
            }*/
            StartCoroutine(StopMusic(stopSource, stopParameter, stopVolume));
        }

        //устанавливаем громкость из настроек
        currSource.volume = ((float)currentStats.MusicVolume / 10f);

        /*if (isFight != currIsFight || !isStopLast)
        {
            StartCoroutine(StartMusic(currSource, currParameter, targetVolume));
        }*/
        //StopCoroutine(StopMusic(currSource, currParameter, targetVolume));
        StartCoroutine(StartMusic(currSource, currParameter, targetVolume));

        //устанавливаем значение
        currIsFight = isFight;
    }

    //останавливаем проигрывание текущих фоновых звуков
    private void StopLastAmbient(AmbientTheme currPlayingTheme)
    {
        AudioSource currSource = null;
        string currParameter = "";
        float startVolume = muteVolume;

        //присавиваем текущие значения темы
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

        //запускаем карутину остановки текущей темы
        StartCoroutine(StopAmbientSound(currSource, currParameter, startVolume));
    }

    //карутина запуска фоновых звуков
    private IEnumerator StartAmbientSound(AudioSource ambientSource, string parametr, float targetVolume)
    {
        //если ресурс не пустой
        if (ambientSource != null) 
        {
            //включаем проигрывание
            ambientSource.Play();
            float currentTime = 0f;

            // Установить громкость на минимальный уровень
            audioMixer.SetFloat(parametr, muteVolume);

            // Постепенно увеличиваем громкость до targetVolume
            while (currentTime < ambientFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(muteVolume, targetVolume, currentTime / ambientFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // Убедимся, что громкость точно равна targetVolume
            audioMixer.SetFloat(parametr, targetVolume);
        }
    }

    //карутина остановки фоновых звуков
    private IEnumerator StopAmbientSound(AudioSource ambientSource, string parametr, float startVolume)
    {
        if (ambientSource != null) 
        {
            float currentTime = 0f;

            float targetVolume = muteVolume;

            // Постепенно увеличиваем громкость до targetVolume
            while (currentTime < ambientFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / ambientFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // Убедимся, что громкость точно равна targetVolume
            audioMixer.SetFloat(parametr, targetVolume);

            //останавливаем
            ambientSource.Stop();
        }
    }

    //карутина запуска музыки
    private IEnumerator StartMusic(AudioSource musicSource, string parametr, float targetVolume)
    {
        //если ресурс не пустой
        if (musicSource != null)
        {
            //включаем проигрываниеесли не играет
            if(!musicSource.isPlaying) musicSource.Play();

            float currentTime = 0f;

            // Установить громкость на минимальный уровень
            audioMixer.SetFloat(parametr, muteVolume);

            // Постепенно увеличиваем громкость до targetVolume
            while (currentTime < musicStartFadetime)
            {
                currentTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(muteVolume, targetVolume, currentTime / musicStartFadetime);
                audioMixer.SetFloat(parametr, newVolume);
                yield return null;
            }

            // Убедимся, что громкость точно равна targetVolume
            audioMixer.SetFloat(parametr, targetVolume);
        }
    }

    //карутина остановки музыки
    private IEnumerator StopMusic(AudioSource musicSource, string parametr, float startVolume)
    {
        if (musicSource != null)
        {
            float currentTime = 0f;

            float targetVolume = muteVolume;

            // Постепенно увеличиваем громкость до targetVolume
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

            // Убедимся, что громкость точно равна targetVolume
            audioMixer.SetFloat(parametr, targetVolume);

            //останавливаем
            //musicSource.Stop();
        }
    }

    //обновляем звук фоновых звуков
    public void UpdateAmbientVolume(float value)
    {
        forestAmbient.volume = ((float)value / 10f);
        swampAmbient.volume = ((float)value / 10f);
        darkForestAmbient.volume = ((float)value / 10f);
        pirateShipAmbient.volume = ((float)value / 10f);
    }

    //обновляем звук фоновой музыки
    public void UpdateMusicVolume(float value)
    {
        ambientMusic.volume = ((float)value / 10f);
        fightMusic.volume = ((float)value / 10f);
    }

    //играем выбранный звук взмаха меча
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

    //играем выбранный звук удара меча
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

    //играем выбранный звук натягивания лука
    public void PlayBowDrawSound(AudioClip drawSound)
    {
        if (drawSound == null) return;
        bowDraw.clip = drawSound;
        PlaySound(bowDraw);
    }

    //играем выбранный звук стрельбы из лука лука
    public void PlayBowShotSound(AudioClip shotSound)
    {
        if (shotSound == null) return;
        bowShot.clip = shotSound;
        PlaySound(bowShot);
    }

    //играем выбранный звук удара врага
    public void PlayEnemieHitSound(AudioClip hitSound)
    {
        if (hitSound == null) return;
        enemieHit.clip = hitSound;
        PlaySound(enemieHit);
    }

    //играем выбранный звук удара по броне
    public void PlayBlowToArmortSound(AudioClip blowToArmorSound)
    {
        if (blowToArmorSound == null) return;
        blowToArmor.clip = blowToArmorSound;
        PlaySound(blowToArmor);
    }

    //играем выбранный звук удара по игроку
    public void PlayPlayerTakeHitSound(AudioClip takeHit)
    {
        playerTakeHitSound.clip = takeHit;
        PlaySound(playerTakeHitSound);
    }

    //играем выбранный звук удара по игроку
    public void PlayChangeWeaponSound(AudioClip change)
    {
        if (!changeWeaponSound.isPlaying) {
            changeWeaponSound.clip = change;
            PlaySound(changeWeaponSound);
        }
    }

    //играем звук монет
    public void PlayCoinsSound()
    {
        PlaySound(coinsSound);
    }

    //играем звук дерева скиллов
    public void PlaySkillTreeSound()
    {
        PlaySound(skillTreeSound);
    }

    //играем звук кнопки лечения
    public void PlayHealthBtnSound()
    {
        PlaySound(healthBtnSound);
    }

    //играем звук Зелья исцеления
    public void PlayHealthPotionSound()
    {
        PlaySound(healthPotionSound);
    }

    //играем звук кнопки инвентаря
    public void PlayInventorySound()
    {
        PlaySound(inventoryBtnSound);
    }

    //играем звук поднятия уровня
    public void PlayLevelUpSound()
    {
        PlaySound(levelUpSound);
    }

    //играем звук дерева скиллов
    public void PlayMapBtnSound()
    {
        PlaySound(mapBtnSound);
    }

    //играем звук кнопки умения
    public void PlaySkillTreeBtnSound()
    {
        PlaySound(skillTreeBtnSound);
    }

    //играем звук экипировки
    public void PlayEquipPutSound()
    {
        PlaySound(equipPutSound);
    }

    //играем сурс и меняем громкость на текущую
    private void PlaySound(AudioSource source)
    {
        source.volume = ((float)currentStats.SoundVolume / 10f);
        source.Play();
    }
}
