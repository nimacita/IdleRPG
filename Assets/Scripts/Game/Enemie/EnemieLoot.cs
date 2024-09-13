using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieLoot : MonoBehaviour
{

    [Header("Components")]
    public GameObject lootVisual;
    public SpriteRenderer lootObjSprite;
    public Animation lootAnim;
    public AnimationClip lootDrop;

    private MyInventoryItem currLoot;
    private EnemieController currEnemie;

    void Start()
    {
        lootVisual.SetActive(false);
    }

    //спавним лут
    public void LootSpawn(Loot[] allLoots, EnemieController enemie)
    {
        currEnemie = enemie;

        lootVisual.SetActive(false);
        currLoot = null;
        currLoot = GetRandomLoot(allLoots);

        //включаем объект
        lootVisual.SetActive(true);
        //ставим нужную иконку лута
        lootObjSprite.sprite = currLoot.itemIcon;
        //включаем анимацию
        lootAnim.Play(lootDrop.name);
    }

    //генерируем случайный лут из списка
    private MyInventoryItem GetRandomLoot(Loot[] allLoots)
    {
        if (allLoots.Length > 0) {
            int randLoot = Random.Range(0, allLoots.Length);
            int randProcent = Random.Range(0, 101);
            //если процент находится в диапазоне выбранного, то возвращаем его
            if (randProcent <= allLoots[randLoot].lootChance)
            {
                return allLoots[randLoot].lootObj;
            } else
            {
                //если нет, то пробуем заново
                return GetRandomLoot(allLoots);
            }
        }
        else
        {
            return null;
        }
    }

    //устанавливаем полученный лут после анимации
    public void SetLoot()
    {
        //выдаем лут
        InventoryController.instance.SetToSlots(currLoot);

        //выключаем лут
        lootVisual.SetActive(false);
        currLoot = null;

        //вызываем событие смерти врага
        currEnemie.Dead();

        //выключаемся
        gameObject.SetActive(false);
    }
}
