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

    //������� ���
    public void LootSpawn(Loot[] allLoots, EnemieController enemie)
    {
        currEnemie = enemie;

        lootVisual.SetActive(false);
        currLoot = null;
        currLoot = GetRandomLoot(allLoots);

        //�������� ������
        lootVisual.SetActive(true);
        //������ ������ ������ ����
        lootObjSprite.sprite = currLoot.itemIcon;
        //�������� ��������
        lootAnim.Play(lootDrop.name);
    }

    //���������� ��������� ��� �� ������
    private MyInventoryItem GetRandomLoot(Loot[] allLoots)
    {
        if (allLoots.Length > 0) {
            int randLoot = Random.Range(0, allLoots.Length);
            int randProcent = Random.Range(0, 101);
            //���� ������� ��������� � ��������� ����������, �� ���������� ���
            if (randProcent <= allLoots[randLoot].lootChance)
            {
                return allLoots[randLoot].lootObj;
            } else
            {
                //���� ���, �� ������� ������
                return GetRandomLoot(allLoots);
            }
        }
        else
        {
            return null;
        }
    }

    //������������� ���������� ��� ����� ��������
    public void SetLoot()
    {
        //������ ���
        InventoryController.instance.SetToSlots(currLoot);

        //��������� ���
        lootVisual.SetActive(false);
        currLoot = null;

        //�������� ������� ������ �����
        currEnemie.Dead();

        //�����������
        gameObject.SetActive(false);
    }
}
