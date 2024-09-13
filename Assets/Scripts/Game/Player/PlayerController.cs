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

    //������������� ��������� ��������������
    private void SetBaseStats()
    {
        //����������� ��������� �����
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

        //�����
        currSwordCritKoef = MainPlayerStats.baseSwordCritKoef;
        currBowCritKoef = MainPlayerStats.baseBowCritKoef;

        //������������� �� �������
        swordAttackDurability = CurrentStats.startSwordAttackDurability;
        minSwordAttackDurability = MainPlayerStats.minSwordAttackDurability;

        bowAttackDurability = CurrentStats.startBowAttackDurability;
        minBowAttackDurability = MainPlayerStats.minBowAttackDurability;

        CurrentStats.currSwordAttackDurability = swordAttackDurability;
        CurrentStats.currBowAttackDurability = bowAttackDurability;

        //��������� ����������� �����
        CurrentStats.currentCoins = CurrentStats.Coins;

        currHealth = health;

        //��������� ������� �� ����
        isCritBtnClick = false;

        SetCurrStats();
    }

    //������������� � �������, ��������� ��������������
    private void SetCurrStats()
    {
        //������������� � ������
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

        //������ ��� � ���� ��� ���
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

    //��������� �� ������� �������������
    public void UpdateCurrentStats()
    {
        //���� �� �����
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

            //�����
            currSwordCritKoef = CurrentStats.currSwordCritKoef;
            currBowCritKoef = CurrentStats.currBowCritKoef;

            //������ ������������ ���������� � �����
            readyAtckCoolDown = CurrentStats.currReadyAtckCoolDown;
            //���� ��� ������ �����������
            if(readyAtckCoolDown < minReadyAtckCoolDown)
            {
                readyAtckCoolDown = minReadyAtckCoolDown;
                CurrentStats.currReadyAtckCoolDown = minReadyAtckCoolDown;
            }

            //������ �������� ����� �����
            swordAttackDurability = CurrentStats.currSwordAttackDurability;
            //���� ��� ����� �����������
            if (swordAttackDurability < minSwordAttackDurability)
            {
                swordAttackDurability = minSwordAttackDurability;
                CurrentStats.currSwordAttackDurability = minSwordAttackDurability;
            }

            //������ �������� ����� �����
            bowAttackDurability = CurrentStats.currBowAttackDurability;
            //���� ��� ����� �����������
            if (bowAttackDurability < minBowAttackDurability)
            {
                bowAttackDurability = minBowAttackDurability;
                CurrentStats.currBowAttackDurability = minBowAttackDurability;
            }

            //���� ����� ��� ����� ������ 100
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

            //������ ��� � ���� ��� ���
            bool isBow = CurrentStats.IsBow;
            if (isBow)
            {
                SetBow(true);
            }
            else
            {
                SetSword(true);
            }

            //���� ������ ���������� �����, �� �������� ���������� � ��� ������
            if (isAttack)
            {
                //��������� ��������
                playerAnimator.SetBool("Attack", false);
                isAttack = false;
                isReadyToAttack = false;
            }

            //���� �� ����� - ��������� �������
            if (!isDead)
            {
                if (isFight)
                {
                    //���� � ���, �� ����� ������ �������� ������� ��
                    if (CurrentStats.currHealth < currHealth)
                    {
                        currHealth = CurrentStats.currHealth;
                    }
                }
                else
                {
                    //���� �� ����
                    currHealth = CurrentStats.currHealth;
                }
            }
            //��������� �� ���
            UpdateHealthBar();
        }
    }

    //���������� �������������� �� �������, factor = -1 ������ �������� ��������������
    public void AddLevelsStats(int factor = 1)
    {
        CurrentStats.currSwordAttackDamage += CurrentStats.addLevelSwordDamage * factor;
        CurrentStats.currBowAttackDamage += CurrentStats.addLevelBowDamage * factor;
        CurrentStats.currHealth += CurrentStats.addLevelHealth * factor;
        CurrentStats.currArmor += CurrentStats.addLevelArmor * factor;
        CurrentStats.currLuck += CurrentStats.addLevelLuck * factor;

        if(factor > 0)UpdateCurrentStats();
    }

    //���������� �������������� �� ������ �������, factor = -1 ������ �������� ��������������
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

    //���������� �������������� �� ����������, factor = -1 ������ �������� ��������������
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

    //�������� �� �����
    private void IsStartAttack()
    {
        if (isFight && isEnemy && !isPause && !isAttack && !isReadyToAttack && !isDead && !isChange)
        {
            //�������� ���������� � �����
            isReadyToAttack = true;
            currReadyAtckCd = readyAtckCoolDown;
        }
    }

    //� ��� �� ���
    private void IsFight()
    {
        //����� �� ���
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

    //������� ��� ���� ��� ���������� � �����
    private void ReadyToAttackCD()
    {
        //���� ������ ���, �� ���� � ����� ���
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

                //��������� ����������, �������� �����
                if (isSword) SetSwordAttack();
                else if (isBow) SetBowAttack();
            }
        }
    }

    //������������ �������� ��� ����� �����
    private void SetSwordAttack()
    {
        //������������� �������� �����
        playerAnimator.SetFloat("AttackSpeed", swordAttack.length / swordAttackDurability);
        attackLength = swordAttackDurability;
        currAtckCd = attackLength;

        //������ ���� 
        AudioController.instance.PlaySwordSwingSound(CurrentStats.currSwordSwingSound);

        //�������� �����
        StartAttack();
    }

    //������������ �������� ��� ����� �����
    private void SetBowAttack()
    {
        //������������� �������� �����
        playerAnimator.SetFloat("BowAttackSpeed", bowAttack.length / bowAttackDurability);
        attackLength = bowAttackDurability;
        currAtckCd = attackLength;

        //������ ���� 
        AudioController.instance.PlayBowDrawSound(CurrentStats.currBowDrawSound);

        //�������� �����
        StartAttack();
    }


    //�������� �����
    private void StartAttack()
    {
        //��������� ��������
        playerAnimator.SetBool("Attack", true);

        //������� ����� �� ���� �� �����
        if (!isCritReady) 
        {
            isCritReady = IsCrit();
        }

        //�������� �������
        onCritReady?.Invoke();

        UpdateAttackIcon();

        isAttack = true;
    }

    //������� ��� ���� ��� �����
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

                //��������� ��������
                playerAnimator.SetBool("Attack", false);
                isAttack = false;
                //����
                HitEnemie();
            }
        }
    }

    //������������� �������� ��� ������ ������
    private void SetChangeStart()
    {
        if (isFight && !isChange && !isPause && !isDead) 
        {
            //������������� �������� �����
            playerAnimator.SetFloat("ChangeSpeed", changeAnim.length / changeDurability);
            //��������� ��������
            playerAnimator.SetBool("Change", true);

            currChangeDurability = changeDurability;
            isChange = true;

        }
    }

    //������� ��� ���� ��� �����
    private void ChangeCD()
    {
        if (isChange && !isAttack && isFight && !isDead && !isPause)
        {
            if (isChange && !isAttack && isFight && !isDead && !isPause)
            {
                //������ ���� ����� ������
                AudioController.instance.PlayChangeWeaponSound(MainPlayerStats.changeWeaponSound);
            }
            if (currChangeDurability > 0f)
            {
                currChangeDurability -= Time.fixedDeltaTime;
            }
            else
            {
                currChangeDurability = 0f;

                //��������� ��������
                playerAnimator.SetBool("Change", false);
                //������ ������
                ChangeWeapon();
            }
        }
    }

    //������ ������
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

    //���� �����
    public void HitEnemie()
    {
        //������� ����
        int damage = 0;

        //����� �� ����
        bool isCrit = false;

        //���� ���� �������������
        if (CurrentStats.isCritAutomaticly)
        {
            //���������� ��������� �������� ���������� �����
            isCrit = isCritReady;
        }
        else
        {
            //���� �� �������������
            if (isCritReady && isCritBtnClick)
            {
                //���� ���� ����, � ������ ������
                isCrit = true;
            }
            else
            {
                isCrit = false;
            }
        }

        //�������� ���������� ���� � ��� �� ����
        damage = CurrentDamage(isCrit);

        if (isSword)
        {
            //����������� ����
            AudioController.instance.PlaySwordHitSound(CurrentStats.currSwordHitSound);
        }
        else if (isBow)
        {
            //������ ���� 
            AudioController.instance.PlayBowShotSound(CurrentStats.currBowShotSound);
        }

        //������� ����
        onDamageHit?.Invoke(damage, isCrit);

        IsStartAttack();
    }

    //������� ���� ��������� �����
    private int CurrentDamage(bool isCrit)
    {
        float damage = 0;

        //� ����� ���
        if (isSword)
        {
            damage = swordAttackDamage;
            if (isRangeEnemie)
            {
                //���� �������� ������
                damage *= swordDamageToRangeKoef;
            }

            //���������� ���� ����, ���� ���� ����
            if (isCrit)
            {
                if(damage != 0) damage += CurrentStats.GetCurrSwordAddCriteDmg;

                isCritReady = false;
                isCritBtnClick = false;

                //�������� ������� ���������� �����
                onCritEnded?.Invoke();
            }
        }
        //� ����� ���
        else if (isBow)
        {
            damage = bowAttackDamage;
            if (!isRangeEnemie)
            {
                //���� �������� ������
                damage *= bowDamageToMeleeKoef;
            }

            //���������� ���� ����, ���� ���� ����
            if (isCrit)
            {
                if (damage != 0) damage += CurrentStats.GetCurrBowAddCriteDmg;

                isCritReady = false;
                isCritBtnClick = false;

                //�������� ������� ���������� �����
                onCritEnded?.Invoke();
            }
        }

        //���������� ���� � ��� �� ����
        return Mathf.RoundToInt(damage);
    }

    //����� �� ����
    private bool IsCrit()
    {
        //���������� ��������� ����������� �� 1 �� 100
        int randCrit = Random.Range(1, 101);

        //���� � ���������, �� ����
        if (randCrit <= luck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //�������� ����
    private void TakeDamage(int damage)
    {
        int currentDamage = damage;

        //���� ���� ������ �� ���������� ���������� ���� � ���������
        if (armor > 0)
        {
            float armorDamage = (float)currentDamage - (float)currentDamage * (armor / 100f);
            currentDamage = Mathf.RoundToInt(armorDamage);
        }

        //�������� ���������� ����
        currHealth -= currentDamage;

        //�������� �����
        SetDamagePopUp(currentDamage);

        //���������� ��������� � ������� �� ����� ���� ���� ������ 0
        if (currentDamage > 0) StartCoroutine(Utils.ChangeColorCoroutine(allSprites, allColors));

        //������ ���� ����� �� �����
        AudioController.instance.PlayBlowToArmortSound(CurrentStats.currArmorBlowSound);

        if (currHealth <= 0)
        {
            currHealth = 0;
            SetDead();
            //������ ���� ������
            AudioController.instance.PlayPlayerTakeHitSound(MainPlayerStats.deathSound);
        }
        else
        {
            //������ ��������� ���� ��������� �����
            TakeDamageSound();
        }

        UpdateHealthBar();
    }

    //������ ��������� ���� ��������� �����
    private void TakeDamageSound()
    {
        int randSound = Random.Range(0, MainPlayerStats.takeDamageSounds.Length);
        AudioController.instance.PlayPlayerTakeHitSound(MainPlayerStats.takeDamageSounds[randSound]);
    }

    //�������� ����� ��������� �����
    private void SetDamagePopUp(int damage, bool isCrit = false)
    {
        movedDmgText.SetActive(false);
        //���� �� ����
        if (!isCrit)
        {
            //���������� � �������� ������ �����
            damagePopTxt.text = $"-{damage}";
            damagePopTxt.gameObject.SetActive(true);
            critDamagePopTxt.gameObject.SetActive(false);
        }
        //���� ����
        else
        {
            //���������� � �������� ������ �����
            critDamagePopTxt.text = $"-{damage}";
            critDamagePopTxt.gameObject.SetActive(true);
            damagePopTxt.gameObject.SetActive(false);
        }
        //���������� ��������� ������ ��� ��������
        int randAnim = UnityEngine.Random.Range(0, damagePopUps.Length);
        //������ �������� �������� ��� �����
        damagePopUpAnim.Play(damagePopUps[randAnim].name);
    }

    //�������
    private void SetDead()
    {
        isDead = true;
        currDeathCd = deathAnim.length;
        playerAnimator.SetBool("Death", true);
    }

    //������� ��� ���� ��� ������
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

    //������
    private void Dead()
    {
        //gameObject.SetActive(false);
        onPlayerDead?.Invoke();

        currAtckCd = 0f;
        currChangeDurability = 0f;
        currReadyAtckCd = 0f;
        //��������� �������� ������
        //playerAnimator.SetBool("Death", false);
    }

    //��������� ����������� UI ���������
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

    //��������� ������ �����
    private void UpdateAttackIcon()
    {
        //���������� ��������
        if (IsSword) swordAtckImg.SetActive(true);
        else swordAtckImg.SetActive(false);

        if (isBow) bowAtckImg.SetActive(true);
        else bowAtckImg.SetActive(false);

        //����� �� ����
        bool isCrit = false;
        //���� ���� �������������
        if (CurrentStats.isCritAutomaticly)
        {
            //���������� ��������� �������� ���������� �����
            isCrit = isCritReady;
        }
        else
        {
            //���� �� �������������
            if (isCritReady && isCritBtnClick)
            {
                //���� ���� ����, � ������ ������
                isCrit = true;
            }
            else
            {
                isCrit = false;
            }
        }

        //���� ����
        if (isCrit)
        {
            critAtckImg.SetActive(true);
            atckFill.color = critAttckColor;
            atckIcon.color = critAttckColor;
            atckBorder.color = critAttckColor;
        }
        //���� �� ����
        else
        {
            critAtckImg.SetActive(false);
            atckFill.color = baseAttckColor;
            atckIcon.color = baseAttckColor;
            atckBorder.color = baseAttckColor;
        }
    }

    //��������� �� ��� ������
    private void UpdateHealthBar()
    {
        healthBar.value = (float)currHealth / health;
        healthBarText.text = $"{currHealth}/{health}";
    }

    //������������� ��� ����� - ���
    private void SetSword(bool value)
    {
        isSword = value;
        playerAnimator.SetBool("Sword", value);

        if (value)
        {
            //��������� ���
            SetBow(false);

            characterController.WeaponType = HeroEditor.Common.Enums.WeaponType.Melee1H;
            characterController.Initialize();
        }
    }

    //������������ ��� ����� - ���
    private void SetBow(bool value)
    {
        isBow = value;
        CurrentStats.IsBow = value;
        playerAnimator.SetBool("Bow", value);

        if (value)
        {
            //��������� ���
            SetSword(false);

            characterController.WeaponType = HeroEditor.Common.Enums.WeaponType.Bow;
            characterController.Initialize();
        }
    }

    //����� ������ �� ������������ ���������� ��
    public void PlayerHealth(int hp, bool isAlive = false)
    {
        if (isDead)
        {
            //���� ������ ����������
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
            //���� �� ������ ������ �����
            currHealth += hp;
            if (currHealth > health) currHealth = health;
        }

        UpdateHealthBar();
    }

    //��������� ��� ������
    public void UpdatePlayerView()
    {
        //���� �� �����
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

    //��������� ��� ���� ������
    private void UpdatePlayerSword()
    {
        characterController.PrimaryMeleeWeapon = CurrentStats.currentSwordSprite;
    }

    //��������� ��� ���� ������
    private void UpdatePlayerBow()
    {
        characterController.Bow[0] = CurrentStats.currentBowArrowSprite;
        characterController.Bow[1] = CurrentStats.currentBowLimbSprite;
        characterController.Bow[2] = CurrentStats.currentBowRiserSprite;
    }

    //��������� ���� ������
    private void UpdatePlayerHelmet()
    {
        if (CurrentStats.currentHelmetSprite == null)
        {
            //���� ����� ��� �� �������� ������
            characterController.Hair = defaultHair;
            characterController.Helmet = null;
        }
        else
        {
            //���� ���� ����, �� ������ � ��������� ������
            characterController.Hair = null;
            characterController.Helmet = CurrentStats.currentHelmetSprite;
        }
    }

    //��������� �������� ������
    private void UpdatePlayerGloves()
    {
        if (CurrentStats.currentFingerSprite == null)
        {
            //���� �������� ��� �� �������� ���������
            characterController.Armor[2] = defaultFinger;
            characterController.Armor[3] = defaultForearmL;
            characterController.Armor[4] = defaultForearmR;
            characterController.Armor[5] = defaultHandL;
            characterController.Armor[6] = defaultHandR;
            characterController.Armor[10] = defaultSleever;
        }
        else
        {
            //���� �������� ����, �� ������
            characterController.Armor[2] = CurrentStats.currentFingerSprite;
            characterController.Armor[3] = CurrentStats.currentForearmLSprite;
            characterController.Armor[4] = CurrentStats.currentForearmRSprite;
            characterController.Armor[5] = CurrentStats.currentHandLSprite;
            characterController.Armor[6] = CurrentStats.currentHandRSprite;
            characterController.Armor[10] = CurrentStats.currentSleeverSprite;
        }
    }

    //��������� �������� ������
    private void UpdatePlayerTorso()
    {
        if (CurrentStats.currentArmLSprite == null)
        {
            //���� ���������� ��� �� �������� ���������
            characterController.Armor[0] = defaultArmL;
            characterController.Armor[1] = defaultArmR;
            characterController.Armor[8] = defaultPelvis;
            characterController.Armor[11] = defaultTorso;
        }
        else
        {
            //���� ��������� ����, �� ������
            characterController.Armor[0] = CurrentStats.currentArmLSprite;
            characterController.Armor[1] = CurrentStats.currentArmRSprite;
            characterController.Armor[8] = CurrentStats.currentPelvisSprite;
            characterController.Armor[11] = CurrentStats.currentTorsoSprite;
        }
    }

    //��������� ������� ������
    private void UpdatePlayerBoots()
    {
        if (CurrentStats.curretnLegSprite == null)
        {
            //���� ������� ���, �� ��������� ���������
            characterController.Armor[7] = defaultLeg;
            characterController.Armor[9] = defaultShin;
        }
        else
        {
            //���� ��������� ����, �� ������
            characterController.Armor[7] = CurrentStats.curretnLegSprite;
            characterController.Armor[9] = CurrentStats.curretnShinSprite;
        }
    }

    //���������� �� ���� ��������
    private void TakeAllSprites(Transform parent)
    {
        // ��������� ������� ������ � ������������ �������
        foreach (Transform child in parent)
        {
            // ���� � ������� ���� ��������� SpriteRenderer, ��������� ��� � ������
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                allSprites.Add(spriteRenderer);
                allColors.Add(spriteRenderer.color);
            }

            // ���������� �������� ����� ��� ���� �������� ��������
            TakeAllSprites(child);
        }
    }

    //����� ��������� �������
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

    //������ �� ����� ������
    private void ChangeClicked()
    {
        SetChangeStart();
    }

    //������ �� ���
    private void FightClick()
    {
        isFight = true;
        isCritBtnClick = false;
        IsFight();
        UpdateAttackIcon();
    }

    //������ �� ����� �� ���
    private void FightEndClick()
    {
        isEnemy = false;
        isFight = false;
        isCritReady = false;
        isCritBtnClick = false;
        IsFight();
        UpdateAttackIcon();
    }

    //������ �� ������ ����
    private void HealthClick()
    {
        //����� �� ����������� ��������
        int currHP = health - currHealth;
        PlayerHealth(currHP, true);
    }

    //�������� ����
    private void EnemieIsSpawn(bool isRange = false)
    {
        isRangeEnemie = isRange;
        isEnemy = true;
        IsFight();
    }

    //���� ����
    private void EnemieIsDead()
    {
        isEnemy = false;
        IsFight();
    }

    //�����
    private void SetPause(bool state)
    {
        isPause = state;

        //���� ����� � �����
        if (!isPause)
        {
            //�������� ������� ��
            IsStartAttack();
            //���������� ��������
            ResumeAnimation(playerAnimator);
        }
        //���� ����� � �����
        else
        {
            //���������������� ��������
            PauseAnimation(playerAnimator);
        }
    }

    //���������������� ��������
    private void PauseAnimation(Animator animator)
    {
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    //���������� ��������
    private void ResumeAnimation(Animator animator)
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
    }

    //������ �� ����
    private void CritClicked()
    {
        isCritBtnClick = true;
        UpdateAttackIcon();
    }

    //������� ��������
    public int CurrentHealth
    {
        get { return currHealth; }
    }

    //����� ��������
    public int Health
    {
        get { return health; }
    }

    //���� ���
    public bool IsSword
    {
        get { return isSword; }
    }

    //���� ������
    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    //���� �����
    public bool IsCritReady
    {
        get { return isCritReady; }
    }

}
