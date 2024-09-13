using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Utils;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{

    [Header("Stats")]
    public PlayerStats MainPlayerStats;
    public PlayerCurrentStats CurrentStats;

    [Header("Components")]
    [SerializeField] private Character characterController;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AnimationClip swordAttack;
    [SerializeField] private AnimationClip bowAttack;
    [SerializeField] private AnimationClip changeAnim;
    [SerializeField] private AnimationClip deathAnim;

    [Header("UI Components")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMPro.TMP_Text healthBarText;
    [SerializeField] private GameObject readyToAtckBg;
    [SerializeField] private Image readyToAtckFill;
    [SerializeField] private GameObject atckBg;
    [SerializeField] private Image atckIcon;
    [SerializeField] private Image atckFill;
    [SerializeField] private Image atckBorder;
    [SerializeField] private GameObject swordAtckImg;
    [SerializeField] private GameObject bowAtckImg;
    [SerializeField] private GameObject critAtckImg;
    [SerializeField] private Color baseAttckColor;
    [SerializeField] private Color critAttckColor;
    [SerializeField] private GameObject changeBg;
    [SerializeField] private Image changeFill;

    [Header("Damage PopUp")]
    [SerializeField] private GameObject movedDmgText;
    [SerializeField] TMPro.TMP_Text damagePopTxt;
    [SerializeField] TMPro.TMP_Text critDamagePopTxt;
    [SerializeField] private Animation damagePopUpAnim;
    [SerializeField] private AnimationClip[] damagePopUps;

    //All Sprites
    private List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    private List<Color> allColors = new List<Color>();

    //defaultSprites
    private Sprite defaultHair;
    private Sprite defaultFinger;
    private Sprite defaultForearmL;
    private Sprite defaultForearmR;
    private Sprite defaultHandL;
    private Sprite defaultHandR;
    private Sprite defaultSleever;
    private Sprite defaultArmL;
    private Sprite defaultArmR;
    private Sprite defaultPelvis;
    private Sprite defaultTorso;
    private Sprite defaultLeg;
    private Sprite defaultShin;

    //Main Stats
    private int health;
    private int armor;
    private int swordAttackDamage;
    private int bowAttackDamage;
    private float readyAtckCoolDown;
    private float minReadyAtckCoolDown;
    private float swordAttackDurability;
    private float minSwordAttackDurability;
    private float minBowAttackDurability;
    private float bowAttackDurability;
    private float changeDurability;
    private float swordDamageToRangeKoef;
    private float bowDamageToMeleeKoef;
    private float luck;

    //Changed Stats
    private int currHealth;
    private float currReadyAtckCd;
    private float attackLength;
    private float currAtckCd;
    private float currChangeDurability;
    private float currDeathCd;
    private float currSwordCritKoef;
    private float currBowCritKoef;

    //bools
    private bool isFight = false;
    private bool isEnemy = false;
    private bool isRangeEnemie = false;
    private bool isSword = true;
    private bool isBow = false;
    private bool isAttack = false;
    private bool isReadyToAttack = false;
    private bool isPause = false;
    private bool isDead = false;
    private bool isChange = false;
    [SerializeField]
    private bool isCritReady = false;
    [SerializeField]
    private bool isCritBtnClick = false;

    public static Action<int, bool> onDamageHit;
    public static Action onPlayerDead;
    public static Action onPlayerChanged;
    public static Action onCritEnded;
    public static Action onCritReady;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;
        TakeDefaultSprites();
        SetBaseStats();
    }

    private void OnEnable()
    {
        GameController.onFightStarted += FightClick;
        GameController.onFightEnded += FightEndClick;
        GameController.onChangeClicked += ChangeClicked;
        EnemieController.onEnemieDamageHit += TakeDamage;
        EnemieController.onEnemieDiedToPlayer += EnemieIsDead;
        EnemieSpawner.onEnemieSpawned += EnemieIsSpawn;
        GameController.onHealthBtnClicked += HealthClick;
        GameController.onPaused += SetPause;
        GameController.onCritClicked += CritClicked;
    }

    private void OnDisable()
    {
        GameController.onFightStarted -= FightClick;
        GameController.onFightEnded -= FightEndClick;
        GameController.onChangeClicked -= ChangeClicked;
        EnemieController.onEnemieDamageHit -= TakeDamage;
        EnemieController.onEnemieDiedToPlayer -= EnemieIsDead;
        EnemieSpawner.onEnemieSpawned -= EnemieIsSpawn;
        GameController.onHealthBtnClicked -= HealthClick;
        GameController.onPaused -= SetPause;
        GameController.onCritClicked -= CritClicked;
    }

    void Start()
    {
        TakeAllSprites(transform);
        UpdateHealthBar();
    }

    //устанавливаем начальные характеристики
    private void SetBaseStats()
    {
        //присваиваем начальные статы
        health = MainPlayerStats.baseHealth;
        armor = MainPlayerStats.baseArmor;
        swordAttackDamage = MainPlayerStats.baseSwordAttackDamage;
        bowAttackDamage = MainPlayerStats.baseBowAttackDamage;
        readyAtckCoolDown = MainPlayerStats.readyAtckCoolDown;
        minReadyAtckCoolDown = MainPlayerStats.minReadyToAtckCoolDown;
        changeDurability = MainPlayerStats.changeDurability;
        swordDamageToRangeKoef = MainPlayerStats.swordDamageToRangeKoef;
        bowDamageToMeleeKoef = MainPlayerStats.bowDamageToMeleeKoef;
        luck = MainPlayerStats.baseLuck;

        //криты
        currSwordCritKoef = MainPlayerStats.baseSwordCritKoef;
        currBowCritKoef = MainPlayerStats.baseBowCritKoef;

        //устанавливаем из текущих
        swordAttackDurability = CurrentStats.startSwordAttackDurability;
        minSwordAttackDurability = MainPlayerStats.minSwordAttackDurability;

        bowAttackDurability = CurrentStats.startBowAttackDurability;
        minBowAttackDurability = MainPlayerStats.minBowAttackDurability;

        CurrentStats.currSwordAttackDurability = swordAttackDurability;
        CurrentStats.currBowAttackDurability = bowAttackDurability;

        //обновляем отображение денег
        CurrentStats.currentCoins = CurrentStats.Coins;

        currHealth = health;

        //выключаем нажатие на крит
        isCritBtnClick = false;

        SetCurrStats();
    }

    //устанавливаем в текущии, начальные характеристики
    private void SetCurrStats()
    {
        //устанавливаем в текщии
        CurrentStats.currHealth = health;
        CurrentStats.currArmor = armor;
        CurrentStats.currSwordAttackDamage = swordAttackDamage;
        CurrentStats.currBowAttackDamage = bowAttackDamage;
        CurrentStats.currReadyAtckCoolDown = readyAtckCoolDown;
        CurrentStats.currChangeDurability = changeDurability;
        CurrentStats.currSwordDamageToRangeKoef = swordDamageToRangeKoef;
        CurrentStats.currBowDamageToMeleeKoef = bowDamageToMeleeKoef;
        CurrentStats.currLuck = luck;
        CurrentStats.currSwordCritKoef = currSwordCritKoef;
        CurrentStats.currBowCritKoef = currBowCritKoef;

        //ставим лук в руку или меч
        bool isBow = CurrentStats.IsBow;
        if (isBow) 
        {
            SetBow(true);
        }
        else
        {
            SetSword(true);
        }
    }

    //обновляем из текущих характеристик
    public void UpdateCurrentStats()
    {
        //если не мертв
        if (!isDead)
        {
            health = CurrentStats.currHealth;
            armor = CurrentStats.currArmor;
            swordAttackDamage = CurrentStats.currSwordAttackDamage;
            bowAttackDamage = CurrentStats.currBowAttackDamage;
            changeDurability = CurrentStats.currChangeDurability;
            swordDamageToRangeKoef = CurrentStats.currSwordDamageToRangeKoef;
            bowDamageToMeleeKoef = CurrentStats.currBowDamageToMeleeKoef;
            luck = CurrentStats.currLuck;

            //криты
            currSwordCritKoef = CurrentStats.currSwordCritKoef;
            currBowCritKoef = CurrentStats.currBowCritKoef;

            //ставим длительность подготовки к атаке
            readyAtckCoolDown = CurrentStats.currReadyAtckCoolDown;
            //если она меньше минимальной
            if(readyAtckCoolDown < minReadyAtckCoolDown)
            {
                readyAtckCoolDown = minReadyAtckCoolDown;
                CurrentStats.currReadyAtckCoolDown = minReadyAtckCoolDown;
            }

            //ставим скорость атаки мечом
            swordAttackDurability = CurrentStats.currSwordAttackDurability;
            //если она меьше минимальной
            if (swordAttackDurability < minSwordAttackDurability)
            {
                swordAttackDurability = minSwordAttackDurability;
                CurrentStats.currSwordAttackDurability = minSwordAttackDurability;
            }

            //ставим скорости атаки луком
            bowAttackDurability = CurrentStats.currBowAttackDurability;
            //если она меьше минимальной
            if (bowAttackDurability < minBowAttackDurability)
            {
                bowAttackDurability = minBowAttackDurability;
                CurrentStats.currBowAttackDurability = minBowAttackDurability;
            }

            //если удачи или брони больше 100
            if (luck > 100)
            {
                luck = 100;
                CurrentStats.currLuck = luck;
            }
            if (armor > 100)
            {
                armor = 100;
                CurrentStats.currArmor = armor;
            }

            //ставим лук в руку или меч
            bool isBow = CurrentStats.IsBow;
            if (isBow)
            {
                SetBow(true);
            }
            else
            {
                SetSword(true);
            }

            //если сейчас происходит атака, то начинаем подготовку к ней заново
            if (isAttack)
            {
                //выключаем анимацию
                playerAnimator.SetBool("Attack", false);
                isAttack = false;
                isReadyToAttack = false;
            }

            //если не мервы - обновляем здровье
            if (!isDead)
            {
                if (isFight)
                {
                    //если в бою, то можем только понизить текущее хп
                    if (CurrentStats.currHealth < currHealth)
                    {
                        currHealth = CurrentStats.currHealth;
                    }
                }
                else
                {
                    //если не вбою
                    currHealth = CurrentStats.currHealth;
                }
            }
            //обновляем хп бар
            UpdateHealthBar();
        }
    }

    //прибавляем характеристики от уровней, factor = -1 значит вычитаем характеристики
    public void AddLevelsStats(int factor = 1)
    {
        CurrentStats.currSwordAttackDamage += CurrentStats.addLevelSwordDamage * factor;
        CurrentStats.currBowAttackDamage += CurrentStats.addLevelBowDamage * factor;
        CurrentStats.currHealth += CurrentStats.addLevelHealth * factor;
        CurrentStats.currArmor += CurrentStats.addLevelArmor * factor;
        CurrentStats.currLuck += CurrentStats.addLevelLuck * factor;

        if(factor > 0)UpdateCurrentStats();
    }

    //прибавляем характеристики от дерева скиллов, factor = -1 значит вычитаем характеристики
    public void AddSkillTreeStats(int factor = 1)
    {
        CurrentStats.currSwordAttackDamage += CurrentStats.addSkillTreeSwordDamage * factor;
        CurrentStats.currBowAttackDamage += CurrentStats.addSkillTreeBowDamage * factor;
        CurrentStats.currHealth += CurrentStats.addSkillTreeHealth * factor;
        CurrentStats.currArmor += CurrentStats.addSkillTreeArmor * factor;
        CurrentStats.currLuck += CurrentStats.addSkillTreeLuck * factor;

        CurrentStats.currChangeDurability += CurrentStats.addSkillTreeChangeTime * factor;
        CurrentStats.currReadyAtckCoolDown += CurrentStats.addSkillTreeReadyTime * factor;
        CurrentStats.currSwordAttackDurability += CurrentStats.addSkillTreeSwordAttackTime * factor;
        CurrentStats.currBowAttackDurability += CurrentStats.addSkillTreeBowAttackTime * factor;

        if (factor > 0) UpdateCurrentStats();
    }

    //прибавляем характеристики от экипировки, factor = -1 значит вычитаем характеристики
    public void AddEquipeStats(int factor = 1)
    {
        CurrentStats.currSwordAttackDamage += CurrentStats.addEquipSwordDamage * factor;
        CurrentStats.currBowAttackDamage += CurrentStats.addEquipBowDamage * factor;
        CurrentStats.currHealth += CurrentStats.addEquipHealth * factor;
        CurrentStats.currArmor += CurrentStats.addEquipArmor * factor;
        CurrentStats.currLuck += CurrentStats.addEquipLuck * factor;

        CurrentStats.currReadyAtckCoolDown += CurrentStats.addEquipReadyTime * factor;
        CurrentStats.currSwordAttackDurability += CurrentStats.addEquipSwordAttackTime * factor;
        CurrentStats.currBowAttackDurability += CurrentStats.addEquipBowAttackTime * factor;

        if (factor > 0) UpdateCurrentStats();
    }

    void FixedUpdate()
    {
        ReadyToAttackCD();
        AttackCD();
        ChangeCD();
        DeathCD();

        UpdateUI();
    }

    //начинаем ли атаку
    private void IsStartAttack()
    {
        if (isFight && isEnemy && !isPause && !isAttack && !isReadyToAttack && !isDead && !isChange)
        {
            //начинаем подготовку к атаке
            isReadyToAttack = true;
            currReadyAtckCd = readyAtckCoolDown;
        }
    }

    //в бою ли еще
    private void IsFight()
    {
        //вышли из боя
        if (!isFight)
        {
            isReadyToAttack = false;
            isAttack = false;
            isChange = false;
            playerAnimator.SetBool("Attack", false);
            playerAnimator.SetBool("Change", false);

            currReadyAtckCd = 0f;
            currAtckCd = 0f;
            currChangeDurability = 0f;
        }
        else
        {
            IsStartAttack();
        }
    }

    //считаем кул даун для подготовки к атаке
    private void ReadyToAttackCD()
    {
        //если выбран лук, но лука в руках нет
        if (CurrentStats.IsBow && CurrentStats.currentBowArrowSprite == null)
        {
            isReadyToAttack = false;
        }
        if (isReadyToAttack && !isAttack && isFight && isEnemy && !isDead && !isChange && !isPause)
        {
            if (currReadyAtckCd > 0f)
            {
                currReadyAtckCd -= Time.fixedDeltaTime;
            }
            else
            {
                currReadyAtckCd = 0f;
                isReadyToAttack = false;

                //закончили подготовку, начинаем атаку
                if (isSword) SetSwordAttack();
                else if (isBow) SetBowAttack();
            }
        }
    }

    //устанавливем значения для атаки мечом
    private void SetSwordAttack()
    {
        //устанавливаем скорость атаки
        playerAnimator.SetFloat("AttackSpeed", swordAttack.length / swordAttackDurability);
        attackLength = swordAttackDurability;
        currAtckCd = attackLength;

        //играем звук 
        AudioController.instance.PlaySwordSwingSound(CurrentStats.currSwordSwingSound);

        //начинаем атаку
        StartAttack();
    }

    //устанавливем значения для атаки луком
    private void SetBowAttack()
    {
        //устанавливаем скорость атаки
        playerAnimator.SetFloat("BowAttackSpeed", bowAttack.length / bowAttackDurability);
        attackLength = bowAttackDurability;
        currAtckCd = attackLength;

        //играем звук 
        AudioController.instance.PlayBowDrawSound(CurrentStats.currBowDrawSound);

        //начинаем атаку
        StartAttack();
    }


    //начинаем атаку
    private void StartAttack()
    {
        //запускаем анимацию
        playerAnimator.SetBool("Attack", true);

        //считаем будет ли крит на атаке
        if (!isCritReady) 
        {
            isCritReady = IsCrit();
        }

        //вызываем событие
        onCritReady?.Invoke();

        UpdateAttackIcon();

        isAttack = true;
    }

    //считаем кул даун для атаки
    private void AttackCD()
    {
        if (isAttack && !isReadyToAttack && isFight && isEnemy && !isDead && !isPause)
        {
            if (currAtckCd > 0f) 
            {
                currAtckCd -= Time.fixedDeltaTime;
            }
            else
            {
                currAtckCd = 0f;

                //выключаем анимацию
                playerAnimator.SetBool("Attack", false);
                isAttack = false;
                //бьем
                HitEnemie();
            }
        }
    }

    //устанавливаем значения для замены оружия
    private void SetChangeStart()
    {
        if (isFight && !isChange && !isPause && !isDead) 
        {
            //устанавливаем скорость смены
            playerAnimator.SetFloat("ChangeSpeed", changeAnim.length / changeDurability);
            //запускаем анимацию
            playerAnimator.SetBool("Change", true);

            currChangeDurability = changeDurability;
            isChange = true;

        }
    }

    //считаем кул даун для смены
    private void ChangeCD()
    {
        if (isChange && !isAttack && isFight && !isDead && !isPause)
        {
            if (isChange && !isAttack && isFight && !isDead && !isPause)
            {
                //играем звук смены оружия
                AudioController.instance.PlayChangeWeaponSound(MainPlayerStats.changeWeaponSound);
            }
            if (currChangeDurability > 0f)
            {
                currChangeDurability -= Time.fixedDeltaTime;
            }
            else
            {
                currChangeDurability = 0f;

                //выключаем анимацию
                playerAnimator.SetBool("Change", false);
                //меняем оружие
                ChangeWeapon();
            }
        }
    }

    //меняем оружие
    private void ChangeWeapon()
    {
        if (isSword)
        {
            SetBow(true);
        }
        else
        {
            SetSword(true);
        }
        isChange = false;
        onPlayerChanged?.Invoke();
        IsStartAttack();
    }

    //бьем врага
    public void HitEnemie()
    {
        //считаем урон
        int damage = 0;

        //будет ли крит
        bool isCrit = false;

        //если крит автоматически
        if (CurrentStats.isCritAutomaticly)
        {
            //присваваем полученое значение готовности крита
            isCrit = isCritReady;
        }
        else
        {
            //если не автоматически
            if (isCritReady && isCritBtnClick)
            {
                //если крит есть, и нажата кнопка
                isCrit = true;
            }
            else
            {
                isCrit = false;
            }
        }

        //получаем посчитаный урон и был ли крит
        damage = CurrentDamage(isCrit);

        if (isSword)
        {
            //проигрываем звук
            AudioController.instance.PlaySwordHitSound(CurrentStats.currSwordHitSound);
        }
        else if (isBow)
        {
            //играем звук 
            AudioController.instance.PlayBowShotSound(CurrentStats.currBowShotSound);
        }

        //наносим урон
        onDamageHit?.Invoke(damage, isCrit);

        IsStartAttack();
    }

    //считаем урон наносимый врагу
    private int CurrentDamage(bool isCrit)
    {
        float damage = 0;

        //в руках меч
        if (isSword)
        {
            damage = swordAttackDamage;
            if (isRangeEnemie)
            {
                //если персонаж далеко
                damage *= swordDamageToRangeKoef;
            }

            //прибавляем крит урон, если есть крит
            if (isCrit)
            {
                if(damage != 0) damage += CurrentStats.GetCurrSwordAddCriteDmg;

                isCritReady = false;
                isCritBtnClick = false;

                //вызываем событие совершения крита
                onCritEnded?.Invoke();
            }
        }
        //в руках лук
        else if (isBow)
        {
            damage = bowAttackDamage;
            if (!isRangeEnemie)
            {
                //если персонаж близко
                damage *= bowDamageToMeleeKoef;
            }

            //прибавляем крит урон, если есть крит
            if (isCrit)
            {
                if (damage != 0) damage += CurrentStats.GetCurrBowAddCriteDmg;

                isCritReady = false;
                isCritBtnClick = false;

                //вызываем событие совершения крита
                onCritEnded?.Invoke();
            }
        }

        //возвращаем урон и был ли крит
        return Mathf.RoundToInt(damage);
    }

    //будет ли крит
    private bool IsCrit()
    {
        //генерируем случайную вероятность от 1 до 100
        int randCrit = Random.Range(1, 101);

        //если в диапазоне, то крит
        if (randCrit <= luck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //получаем урон
    private void TakeDamage(int damage)
    {
        int currentDamage = damage;

        //если есть защита то скоращеаем полученный урон в процентах
        if (armor > 0)
        {
            float armorDamage = (float)currentDamage - (float)currentDamage * (armor / 100f);
            currentDamage = Mathf.RoundToInt(armorDamage);
        }

        //отнимаем полученный урон
        currHealth -= currentDamage;

        //анимация урона
        SetDamagePopUp(currentDamage);

        //окрашываем персонажа в красный на время если урон больше 0
        if (currentDamage > 0) StartCoroutine(Utils.ChangeColorCoroutine(allSprites, allColors));

        //играем звук удара по броне
        AudioController.instance.PlayBlowToArmortSound(CurrentStats.currArmorBlowSound);

        if (currHealth <= 0)
        {
            currHealth = 0;
            SetDead();
            //играем звук смерти
            AudioController.instance.PlayPlayerTakeHitSound(MainPlayerStats.deathSound);
        }
        else
        {
            //играем случайный звук получения урона
            TakeDamageSound();
        }

        UpdateHealthBar();
    }

    //играем случайный звук получения урона
    private void TakeDamageSound()
    {
        int randSound = Random.Range(0, MainPlayerStats.takeDamageSounds.Length);
        AudioController.instance.PlayPlayerTakeHitSound(MainPlayerStats.takeDamageSounds[randSound]);
    }

    //анимация цифры получения урона
    private void SetDamagePopUp(int damage, bool isCrit = false)
    {
        movedDmgText.SetActive(false);
        //если не крит
        if (!isCrit)
        {
            //записываем и включаем нужный текст
            damagePopTxt.text = $"-{damage}";
            damagePopTxt.gameObject.SetActive(true);
            critDamagePopTxt.gameObject.SetActive(false);
        }
        //если крит
        else
        {
            //записываем и включаем нужный текст
            critDamagePopTxt.text = $"-{damage}";
            critDamagePopTxt.gameObject.SetActive(true);
            damagePopTxt.gameObject.SetActive(false);
        }
        //генерируем случайный индекс для анимации
        int randAnim = UnityEngine.Random.Range(0, damagePopUps.Length);
        //играем случанйю анимацию для урона
        damagePopUpAnim.Play(damagePopUps[randAnim].name);
    }

    //умераем
    private void SetDead()
    {
        isDead = true;
        currDeathCd = deathAnim.length;
        playerAnimator.SetBool("Death", true);
    }

    //считаем кул даун для смерти
    private void DeathCD()
    {
        if (isDead && isFight && !isPause)
        {
            if (currDeathCd > 0f)
            {
                currDeathCd -= Time.fixedDeltaTime;
            }
            else
            {
                currDeathCd = 0f;

                Dead();
            }
        }
    }

    //умерли
    private void Dead()
    {
        //gameObject.SetActive(false);
        onPlayerDead?.Invoke();

        currAtckCd = 0f;
        currChangeDurability = 0f;
        currReadyAtckCd = 0f;
        //выключаем анимацию смерти
        //playerAnimator.SetBool("Death", false);
    }

    //обновляем отображение UI элементов
    private void UpdateUI()
    {
        if (isAttack && !isDead)
        {
            atckBg.SetActive(true);
            atckFill.fillAmount = (float)(attackLength - currAtckCd) /attackLength;
        }
        else
        {
            atckBg.SetActive(false);
        }

        if (isReadyToAttack && !isChange && !isDead)
        {
            readyToAtckBg.SetActive(true);
            readyToAtckFill.fillAmount = (float)(readyAtckCoolDown - currReadyAtckCd) / readyAtckCoolDown;
        }
        else
        {
            readyToAtckBg.SetActive(false);
        }

        if (isChange && !isAttack && !isDead)
        {
            changeBg.SetActive(true);
            changeFill.fillAmount = (float)(changeDurability - currChangeDurability) / changeDurability;
        }
        else
        {
            changeBg.SetActive(false);
        }
    }

    //обновляем иконку атаки
    private void UpdateAttackIcon()
    {
        //определяем картинку
        if (IsSword) swordAtckImg.SetActive(true);
        else swordAtckImg.SetActive(false);

        if (isBow) bowAtckImg.SetActive(true);
        else bowAtckImg.SetActive(false);

        //будет ли крит
        bool isCrit = false;
        //если крит автоматически
        if (CurrentStats.isCritAutomaticly)
        {
            //присваваем полученое значение готовности крита
            isCrit = isCritReady;
        }
        else
        {
            //если не автоматически
            if (isCritReady && isCritBtnClick)
            {
                //если крит есть, и нажата кнопка
                isCrit = true;
            }
            else
            {
                isCrit = false;
            }
        }

        //если крит
        if (isCrit)
        {
            critAtckImg.SetActive(true);
            atckFill.color = critAttckColor;
            atckIcon.color = critAttckColor;
            atckBorder.color = critAttckColor;
        }
        //если не крит
        else
        {
            critAtckImg.SetActive(false);
            atckFill.color = baseAttckColor;
            atckIcon.color = baseAttckColor;
            atckBorder.color = baseAttckColor;
        }
    }

    //обновляем хп бар игрока
    private void UpdateHealthBar()
    {
        healthBar.value = (float)currHealth / health;
        healthBarText.text = $"{currHealth}/{health}";
    }

    //устанавлвиаем тип атаки - меч
    private void SetSword(bool value)
    {
        isSword = value;
        playerAnimator.SetBool("Sword", value);

        if (value)
        {
            //выключаем лук
            SetBow(false);

            characterController.WeaponType = HeroEditor.Common.Enums.WeaponType.Melee1H;
            characterController.Initialize();
        }
    }

    //устаналваием тип атаки - лук
    private void SetBow(bool value)
    {
        isBow = value;
        CurrentStats.IsBow = value;
        playerAnimator.SetBool("Bow", value);

        if (value)
        {
            //выключаем меч
            SetSword(false);

            characterController.WeaponType = HeroEditor.Common.Enums.WeaponType.Bow;
            characterController.Initialize();
        }
    }

    //хилим игрока на определенное количество хп
    public void PlayerHealth(int hp, bool isAlive = false)
    {
        if (isDead)
        {
            //если мертвы воскрешаем
            if (isAlive)
            {
                playerAnimator.SetBool("Death", false);
                isDead = false;
                UpdatePlayerView();
                UpdateCurrentStats();

                currHealth += hp;
                if (currHealth > health) currHealth = health;
            }
        }
        else
        {
            //если не мертвы просто хилим
            currHealth += hp;
            if (currHealth > health) currHealth = health;
        }

        UpdateHealthBar();
    }

    //обновляем вид игрока
    public void UpdatePlayerView()
    {
        //если не мертв
        if (!isDead) 
        {
            UpdatePlayerSword();
            UpdatePlayerBow();
            UpdatePlayerHelmet();
            UpdatePlayerGloves();
            UpdatePlayerTorso();
            UpdatePlayerBoots();

            TakeAllSprites(transform);
            characterController.Initialize();
        }
    }

    //обновляем вид меча игрока
    private void UpdatePlayerSword()
    {
        characterController.PrimaryMeleeWeapon = CurrentStats.currentSwordSprite;
    }

    //обновляем вид лука игрока
    private void UpdatePlayerBow()
    {
        characterController.Bow[0] = CurrentStats.currentBowArrowSprite;
        characterController.Bow[1] = CurrentStats.currentBowLimbSprite;
        characterController.Bow[2] = CurrentStats.currentBowRiserSprite;
    }

    //обновляем шлем игрока
    private void UpdatePlayerHelmet()
    {
        if (CurrentStats.currentHelmetSprite == null)
        {
            //если шлема нет то включаем волосы
            characterController.Hair = defaultHair;
            characterController.Helmet = null;
        }
        else
        {
            //если есть шлем, то ставим и выключаем волосы
            characterController.Hair = null;
            characterController.Helmet = CurrentStats.currentHelmetSprite;
        }
    }

    //обновляем перчатки игрока
    private void UpdatePlayerGloves()
    {
        if (CurrentStats.currentFingerSprite == null)
        {
            //если перчаток нет то включаем дефолтные
            characterController.Armor[2] = defaultFinger;
            characterController.Armor[3] = defaultForearmL;
            characterController.Armor[4] = defaultForearmR;
            characterController.Armor[5] = defaultHandL;
            characterController.Armor[6] = defaultHandR;
            characterController.Armor[10] = defaultSleever;
        }
        else
        {
            //если перчатки есть, то ставим
            characterController.Armor[2] = CurrentStats.currentFingerSprite;
            characterController.Armor[3] = CurrentStats.currentForearmLSprite;
            characterController.Armor[4] = CurrentStats.currentForearmRSprite;
            characterController.Armor[5] = CurrentStats.currentHandLSprite;
            characterController.Armor[6] = CurrentStats.currentHandRSprite;
            characterController.Armor[10] = CurrentStats.currentSleeverSprite;
        }
    }

    //обновляем перчатки игрока
    private void UpdatePlayerTorso()
    {
        if (CurrentStats.currentArmLSprite == null)
        {
            //если нагрудника нет то включаем дефолтные
            characterController.Armor[0] = defaultArmL;
            characterController.Armor[1] = defaultArmR;
            characterController.Armor[8] = defaultPelvis;
            characterController.Armor[11] = defaultTorso;
        }
        else
        {
            //если нагрудник есть, то ставим
            characterController.Armor[0] = CurrentStats.currentArmLSprite;
            characterController.Armor[1] = CurrentStats.currentArmRSprite;
            characterController.Armor[8] = CurrentStats.currentPelvisSprite;
            characterController.Armor[11] = CurrentStats.currentTorsoSprite;
        }
    }

    //обновляем ботинки игрока
    private void UpdatePlayerBoots()
    {
        if (CurrentStats.curretnLegSprite == null)
        {
            //если ботинок нет, то выключаем дефолтные
            characterController.Armor[7] = defaultLeg;
            characterController.Armor[9] = defaultShin;
        }
        else
        {
            //если нагрдуник есть, то ставим
            characterController.Armor[7] = CurrentStats.curretnLegSprite;
            characterController.Armor[9] = CurrentStats.curretnShinSprite;
        }
    }

    //проходимся по всем спрайтам
    private void TakeAllSprites(Transform parent)
    {
        // Проверяем каждого ребёнка в родительском объекте
        foreach (Transform child in parent)
        {
            // Если у объекта есть компонент SpriteRenderer, добавляем его в список
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                allSprites.Add(spriteRenderer);
                allColors.Add(spriteRenderer.color);
            }

            // Рекурсивно вызываем метод для всех дочерних объектов
            TakeAllSprites(child);
        }
    }

    //берем дефолтные спрайты
    private void TakeDefaultSprites()
    {
        //Hair
        defaultHair = characterController.Hair;

        //Gloves
        defaultFinger = characterController.Armor[2];
        defaultForearmL = characterController.Armor[3];
        defaultForearmR = characterController.Armor[4];
        defaultHandL = characterController.Armor[5];
        defaultHandR = characterController.Armor[6];
        defaultSleever = characterController.Armor[10];

        //Torso
        defaultArmL = characterController.Armor[0];
        defaultArmR = characterController.Armor[1];
        defaultPelvis = characterController.Armor[8];
        defaultTorso = characterController.Armor[11];

        //Boots
        defaultLeg = characterController.Armor[7];
        defaultShin = characterController.Armor[9];
    }

    //нажали на смену оружия
    private void ChangeClicked()
    {
        SetChangeStart();
    }

    //нажали на бой
    private void FightClick()
    {
        isFight = true;
        isCritBtnClick = false;
        IsFight();
        UpdateAttackIcon();
    }

    //нажали на выход из боя
    private void FightEndClick()
    {
        isEnemy = false;
        isFight = false;
        isCritReady = false;
        isCritBtnClick = false;
        IsFight();
        UpdateAttackIcon();
    }

    //нажали на кнопку хила
    private void HealthClick()
    {
        //лечим на недостоющее здоровье
        int currHP = health - currHealth;
        PlayerHealth(currHP, true);
    }

    //появился враг
    private void EnemieIsSpawn(bool isRange = false)
    {
        isRangeEnemie = isRange;
        isEnemy = true;
        IsFight();
    }

    //враг умер
    private void EnemieIsDead()
    {
        isEnemy = false;
        IsFight();
    }

    //пауза
    private void SetPause(bool state)
    {
        isPause = state;

        //если вышли с паузы
        if (!isPause)
        {
            //проверка атакуем ли
            IsStartAttack();
            //продолжаем анимации
            ResumeAnimation(playerAnimator);
        }
        //если зашли в паузу
        else
        {
            //приостанавливаем анимации
            PauseAnimation(playerAnimator);
        }
    }

    //приостанавливаем анимации
    private void PauseAnimation(Animator animator)
    {
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    //продолжаем анимации
    private void ResumeAnimation(Animator animator)
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
    }

    //нажали на крит
    private void CritClicked()
    {
        isCritBtnClick = true;
        UpdateAttackIcon();
    }

    //текущее здоровье
    public int CurrentHealth
    {
        get { return currHealth; }
    }

    //всего здоровья
    public int Health
    {
        get { return health; }
    }

    //если меч
    public bool IsSword
    {
        get { return isSword; }
    }

    //если мертвы
    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    //крит готов
    public bool IsCritReady
    {
        get { return isCritReady; }
    }

}
