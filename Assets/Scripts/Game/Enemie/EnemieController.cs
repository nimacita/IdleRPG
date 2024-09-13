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

    //начальные характеристики
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

    //устанавливаем начальные характеристики
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

    //начинаем ли атаку
    private void IsStartAttack()
    {
        if (isFight&& !isPause && !isAttack && !isReadyToAttack && !isDead)
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

    //считаем кул даун для подготовки к атаке
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

                //закончили подготовку, начинаем атаку
                SetAttack();
            }
        }
    }

    //устанавливем значения для атаки
    private void SetAttack()
    {
        //устанавливаем скорость атаки
        enemieAnimator.SetFloat("AttackSpeed", attackAnim.length / attackDurability);
        currAtckCd = attackDurability;
        //запускаем анимацию
        enemieAnimator.SetBool("Attack", true);

        isAttack = true;
    }

    //считаем кул даун для атаки
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

                //выключаем анимацию
                enemieAnimator.SetBool("Attack", false);
                isAttack = false;
                //бьем
                TakeHit();
            }
        }
    }

    //бьем
    public void TakeHit()
    {
        int damage = baseAttackDamage;
        onEnemieDamageHit?.Invoke(damage);

        IsStartAttack();
    }

    //получаем урон
    private void TakeDamage(int damage, bool isCrit)
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

        //играем анимацию
        SetDamagePopUp(currentDamage, isCrit);

        isTakeDamage = true;
        currTakeDamageCd = takeDamageAnim.length;

        //окрашиваем персонажа в красный если урон больше 0
        if(currentDamage > 0) StartCoroutine(Utils.ChangeColorCoroutine(allSprites, allColors));

        if (currHealth <= 0)
        {
            currHealth = 0;
            SetDead();
        }

        UpdateHealthBar();

        enemieAnimator.SetBool("TakeDamage", true);
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
        int randAnim = UnityEngine.Random.Range(0,damagePopUps.Length);
        //играем случанйю анимацию для урона
        damagePopUpAnim.Play(damagePopUps[randAnim].name);
    }

    //считаем кул даун для получения урона
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

                //выключаем анимацию
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

    //умераем
    private void SetDead()
    {
        isDead = true;
        currDeathCd = deathAnim.length;
        enemieAnimator.SetBool("Death", true);

        //выдаем опыт
        onEnemieAddExp?.Invoke(MainEnemieStats.experienceAmount);

        //падает ли лут
        IsDropLoot();

        //вызываем событие для плеера
        if (isFight) onEnemieDiedToPlayer?.Invoke();
    }

    //считаем кул даун для смерти
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

                //выключаем анимации
                enemieAnimator.SetBool("Death", false);
                enemieAnimator.SetBool("Attack", false);
                enemieAnimator.SetBool("TakeDamage", false);
                enemieVisual.SetActive(true);
                if(!isLoot) Dead();
            }
        }
    }

    //спавним ли лут
    private void IsDropLoot()
    {
        int lootChance = UnityEngine.Random.Range(0, 101);

        if (lootChance <= MainEnemieStats.anyLootCahance)
        {
            isLoot = true;
            //если находится в диапазоне шансы выпасть, то выдаем
            loot.gameObject.SetActive(true);
            loot.LootSpawn(MainEnemieStats.allLoots, this);
        }
        else
        {
            //иначе нет лута
            isLoot = false;
        }
    }

    //умерли
    public void Dead()
    {
        isDead = false;
        isLoot = false;

        //выключаем все амниации
        enemieAnimator.SetBool("Death", false);
        enemieAnimator.SetBool("Attack", false);
        enemieAnimator.SetBool("TakeDamage", false);
        enemieVisual.SetActive(true);

        //вызываем событие
        if (isFight) onEnemieDied?.Invoke();
        //окрашиваем в изначальный цвет
        Utils.BackToStartColor(allSprites, allColors);
        //выключаем врага
        gameObject.SetActive(false);
    }

    //обновляем отображение UI элементов
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

    //обновляем хп бар врага
    private void UpdateHealthBar()
    {
        healthBar.value = (float)currHealth / health;
        healthBarText.text = $"{currHealth}/{health}";
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

    //нажали на выход из боя
    private void FightEndClick()
    {
        isFight = false;
        IsFight();
        Dead();
    }

    //пауза
    private void SetPause(bool state)
    {
        isPause = state;

        //если вышли с паузы
        if (!isPause)
        {
            //продолжаем анимации
            ResumeAnimation(enemieAnimator);
        }
        //если зашли в паузу
        else
        {
            //приостанавливаем анимации
            PauseAnimation(enemieAnimator);
        }
    }

    //играем звук
    public void PlayHitSound()
    {
        AudioController.instance.PlayEnemieHitSound(MainEnemieStats.enemieHitSound);
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


    public bool IsRange
    {
        get
        {
            return isRange;
        }
    }
}
