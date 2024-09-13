using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdleRPG.Utils;

public class EnemieController : MonoBehaviour
{

    [Header("Stats")]
    public EnemieStats MainEnemieStats;

    [Header("Loot Components")]
    [SerializeField] private EnemieLoot loot;

    [Header("Components")]
    [SerializeField] private Animator enemieAnimator;
    [SerializeField] private AnimationClip attackAnim;
    [SerializeField] private AnimationClip deathAnim;
    [SerializeField] private AnimationClip takeDamageAnim;
    [SerializeField] private GameObject enemieVisual;

    [Header("UI Components")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMPro.TMP_Text healthBarText;
    [SerializeField] private GameObject readyToAtckBg;
    [SerializeField] private Image readyToAtckFill;
    [SerializeField] private GameObject atckBg;
    [SerializeField] private Image atckFill;
    [SerializeField] private GameObject rangeAttackIcon;

    [Header("Damage PopUp")]
    [SerializeField] private GameObject movedDmgText;
    [SerializeField] TMPro.TMP_Text damagePopTxt;
    [SerializeField] TMPro.TMP_Text critDamagePopTxt;
    [SerializeField] private Animation damagePopUpAnim;
    [SerializeField] private AnimationClip[] damagePopUps;

    //All Sprites
    private List<SpriteRenderer> allSprites = new List<SpriteRenderer>();
    private List<Color> allColors = new List<Color>();

    //Main Stats
    private int health;
    private int armor;
    private int baseAttackDamage;
    private float attackDurability;
    private float readyAtckCoolDown;

    //Changed Stats
    private int currHealth;
    private float currReadyAtckCd;
    private float currAtckCd;
    private float currTakeDamageCd;
    private float currDeathCd;

    //bools
    private bool isFight = false;
    private bool isAttack = false;
    private bool isReadyToAttack = false;
    private bool isPause = false;
    private bool isDead = false;
    private bool isTakeDamage = false;
    private bool isRange = false;
    private bool isLoot = false;

    public static Action<int> onEnemieDamageHit;
    public static Action onEnemieDied;
    public static Action onEnemieDiedToPlayer;
    public static Action<int> onEnemieAddExp;


    private void OnEnable()
    {
        GameController.onFightEnded += FightEndClick;
        PlayerController.onDamageHit += TakeDamage;
        GameController.onPaused += SetPause;
    }

    private void OnDisable()
    {
        GameController.onFightEnded -= FightEndClick;
        PlayerController.onDamageHit -= TakeDamage;
        GameController.onPaused -= SetPause;
    }

    void Start()
    {
        TakeAllSprites(transform);

        //ResetStats();
    }

    //��������� ��������������
    public void ResetStats()
    {
        SetStats();
        UpdateHealthBar();

        loot.gameObject.SetActive(false);
        movedDmgText.SetActive(false);

        isFight = GameController.instance.IsFight;

        if (isFight) 
        {
            isAttack = false;
            isReadyToAttack = false;
            isPause = false;
            isDead = false;
            isTakeDamage = false;

            ResumeAnimation(enemieAnimator);

            enemieAnimator.SetBool("Death", false);
            enemieAnimator.SetBool("TakeDamage", false);

            UpdateUI();
            enemieVisual.SetActive(true);

            IsStartAttack();
        }
        else
        {
            Dead();
        }
    }

    //������������� ��������� ��������������
    private void SetStats()
    {
        health = MainEnemieStats.baseHealth;
        armor = MainEnemieStats.baseArmor;
        baseAttackDamage = MainEnemieStats.baseAttackDamage;
        attackDurability = MainEnemieStats.attackDurability;
        readyAtckCoolDown = MainEnemieStats.readyAtckCoolDown;
        isRange = MainEnemieStats.rangeAttack;

        currHealth = health;
    }

    void FixedUpdate()
    {
        ReadyToAttackCD();
        AttackCD();
        TakeDamageCD();
        DeathCD();

        UpdateUI();
    }

    //�������� �� �����
    private void IsStartAttack()
    {
        if (isFight&& !isPause && !isAttack && !isReadyToAttack && !isDead)
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
            isTakeDamage = false;
            enemieAnimator.SetBool("Attack", false); 
            enemieAnimator.SetBool("TakeDamage", false);

            currReadyAtckCd = 0f;
            currAtckCd = 0f;
        }
        else
        {
            IsStartAttack();
        }
    }

    //������� ��� ���� ��� ���������� � �����
    private void ReadyToAttackCD()
    {
        if (isReadyToAttack && !isAttack && isFight && !isDead && !isPause)
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
                SetAttack();
            }
        }
    }

    //������������ �������� ��� �����
    private void SetAttack()
    {
        //������������� �������� �����
        enemieAnimator.SetFloat("AttackSpeed", attackAnim.length / attackDurability);
        currAtckCd = attackDurability;
        //��������� ��������
        enemieAnimator.SetBool("Attack", true);

        isAttack = true;
    }

    //������� ��� ���� ��� �����
    private void AttackCD()
    {
        if (isAttack && !isReadyToAttack && isFight && !isDead && !isPause)
        {
            if (currAtckCd > 0f)
            {
                currAtckCd -= Time.fixedDeltaTime;
            }
            else
            {
                currAtckCd = 0f;

                //��������� ��������
                enemieAnimator.SetBool("Attack", false);
                isAttack = false;
                //����
                TakeHit();
            }
        }
    }

    //����
    public void TakeHit()
    {
        int damage = baseAttackDamage;
        onEnemieDamageHit?.Invoke(damage);

        IsStartAttack();
    }

    //�������� ����
    private void TakeDamage(int damage, bool isCrit)
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

        //������ ��������
        SetDamagePopUp(currentDamage, isCrit);

        isTakeDamage = true;
        currTakeDamageCd = takeDamageAnim.length;

        //���������� ��������� � ������� ���� ���� ������ 0
        if(currentDamage > 0) StartCoroutine(Utils.ChangeColorCoroutine(allSprites, allColors));

        if (currHealth <= 0)
        {
            currHealth = 0;
            SetDead();
        }

        UpdateHealthBar();

        enemieAnimator.SetBool("TakeDamage", true);
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
        int randAnim = UnityEngine.Random.Range(0,damagePopUps.Length);
        //������ �������� �������� ��� �����
        damagePopUpAnim.Play(damagePopUps[randAnim].name);
    }

    //������� ��� ���� ��� ��������� �����
    private void TakeDamageCD()
    {
        if (isTakeDamage && !isAttack && isFight && !isDead && !isPause)
        {
            if (currTakeDamageCd > 0f)
            {
                currTakeDamageCd -= Time.fixedDeltaTime;
            }
            else
            {
                currTakeDamageCd = 0f;

                //��������� ��������
                enemieAnimator.SetBool("TakeDamage", false);
                isTakeDamage = false;
            }
        }
        if (isAttack && isTakeDamage)
        {
            enemieAnimator.SetBool("TakeDamage", false);
            isTakeDamage = false;
        }
    }

    //�������
    private void SetDead()
    {
        isDead = true;
        currDeathCd = deathAnim.length;
        enemieAnimator.SetBool("Death", true);

        //������ ����
        onEnemieAddExp?.Invoke(MainEnemieStats.experienceAmount);

        //������ �� ���
        IsDropLoot();

        //�������� ������� ��� ������
        if (isFight) onEnemieDiedToPlayer?.Invoke();
    }

    //������� ��� ���� ��� ������
    private void DeathCD()
    {
        if (isDead && isFight && !isPause && !isLoot)
        {
            if (currDeathCd > 0f)
            {
                currDeathCd -= Time.fixedDeltaTime;
            }
            else
            {
                currDeathCd = 0f;

                //��������� ��������
                enemieAnimator.SetBool("Death", false);
                enemieAnimator.SetBool("Attack", false);
                enemieAnimator.SetBool("TakeDamage", false);
                enemieVisual.SetActive(true);
                if(!isLoot) Dead();
            }
        }
    }

    //������� �� ���
    private void IsDropLoot()
    {
        int lootChance = UnityEngine.Random.Range(0, 101);

        if (lootChance <= MainEnemieStats.anyLootCahance)
        {
            isLoot = true;
            //���� ��������� � ��������� ����� �������, �� ������
            loot.gameObject.SetActive(true);
            loot.LootSpawn(MainEnemieStats.allLoots, this);
        }
        else
        {
            //����� ��� ����
            isLoot = false;
        }
    }

    //������
    public void Dead()
    {
        isDead = false;
        isLoot = false;

        //��������� ��� ��������
        enemieAnimator.SetBool("Death", false);
        enemieAnimator.SetBool("Attack", false);
        enemieAnimator.SetBool("TakeDamage", false);
        enemieVisual.SetActive(true);

        //�������� �������
        if (isFight) onEnemieDied?.Invoke();
        //���������� � ����������� ����
        Utils.BackToStartColor(allSprites, allColors);
        //��������� �����
        gameObject.SetActive(false);
    }

    //��������� ����������� UI ���������
    private void UpdateUI()
    {
        if (isAttack && !isDead)
        {
            atckBg.SetActive(true);
            atckFill.fillAmount = (float)(attackDurability - currAtckCd) / attackDurability;
        }
        else
        {
            atckBg.SetActive(false);
        }

        if (isReadyToAttack && !isDead)
        {
            readyToAtckBg.SetActive(true);
            readyToAtckFill.fillAmount = (float)(readyAtckCoolDown - currReadyAtckCd) / readyAtckCoolDown;
        }
        else
        {
            readyToAtckBg.SetActive(false);
        }
        if (isRange)
        {
            rangeAttackIcon.SetActive(true);
        }
        else
        {
            rangeAttackIcon.SetActive(false);
        }
    }

    //��������� �� ��� �����
    private void UpdateHealthBar()
    {
        healthBar.value = (float)currHealth / health;
        healthBarText.text = $"{currHealth}/{health}";
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

    //������ �� ����� �� ���
    private void FightEndClick()
    {
        isFight = false;
        IsFight();
        Dead();
    }

    //�����
    private void SetPause(bool state)
    {
        isPause = state;

        //���� ����� � �����
        if (!isPause)
        {
            //���������� ��������
            ResumeAnimation(enemieAnimator);
        }
        //���� ����� � �����
        else
        {
            //���������������� ��������
            PauseAnimation(enemieAnimator);
        }
    }

    //������ ����
    public void PlayHitSound()
    {
        AudioController.instance.PlayEnemieHitSound(MainEnemieStats.enemieHitSound);
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


    public bool IsRange
    {
        get
        {
            return isRange;
        }
    }
}
