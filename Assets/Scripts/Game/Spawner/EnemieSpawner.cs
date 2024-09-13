using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public struct RandomEnemy
{
    [Tooltip("������ �����")]
    public GameObject enemiePref;
    [Tooltip("�������������� ������� �����")]
    public EnemieStats enemieStats;
    [Tooltip("��� ������������ ������")]
    public List<GameObject> enemiePool;
}

public class EnemieSpawner : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private SpawnerStats spawnerStats;

    [Header("Enemie Settings")]
    [Tooltip("������ ��������� ������ � �� ����������������")]
    private RandomEnemy[] randomEnemies;

    [Header("Enemie Positions Settings")]
    [Tooltip("������� ������ ������ � ������� ����� �����")]
    [SerializeField] private Transform meleeEnemiePosition;
    [Tooltip("������� ������ ������ � ������� ����� �����")]
    [SerializeField] private Transform rangeEnemiePosition;

    //Main Stats
    private Vector2 randomSpawnDelay;

    //Changed Stats
    [SerializeField]
    private float currSpawnDelay;

    //bools
    private bool isSpawn = false;
    private bool isEnemieSpawn = false;
    private bool isPause = false;

    public static Action<bool> onEnemieSpawned;
    public static Action onStopFinded;
    public static Action onStartFinded;

    public static EnemieSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameController.onFightEnded += StopSpawn;
        GameController.onFightStarted += StartSpawn;
        EnemieController.onEnemieDied += StartSpawn;
        GameController.onPaused += SetPause;
    }

    private void OnDisable()
    {
        GameController.onFightEnded -= StopSpawn;
        GameController.onFightStarted -= StartSpawn;
        EnemieController.onEnemieDied -= StartSpawn;
        GameController.onPaused -= SetPause;
    }

    void Start()
    {
        SetStats();
    }

    //������������� ��������������
    private void SetStats()
    {
        randomSpawnDelay = spawnerStats.randomSpawnDelay;
    }

    //������������� ������ � ����������� �� �������
    public void SetRandomEnemies(int locIndex = 0)
    {
        randomEnemies = spawnerStats.spawnerLocations[locIndex].randomEnemies;

        InitializeEnemiePools();
    }

    private void FixedUpdate()
    {
        SpawnCD();
    }

    //������������� �������� ��� ������ ������
    private void SetSpawnEnemie()
    {
        //�������� ��������� �������� ����� ���������� �����
        currSpawnDelay = Random.Range(randomSpawnDelay.x, randomSpawnDelay.y);
        isEnemieSpawn = true;
    }
    
    //������� ��� ���� �������� ����� �������
    private void SpawnCD()
    {
        if (isSpawn && isEnemieSpawn && !isPause)
        {
            if (currSpawnDelay > 0f)
            {
                currSpawnDelay -= Time.fixedDeltaTime;
            }
            else
            {
                currSpawnDelay = 0f;

                //������� �����
                EnemieSpawn();
                isEnemieSpawn = false;
            }
        }
    }

    //������� �����
    private void EnemieSpawn()
    {
        //�������� ���������� �����
        GameObject en = TakeRandomEnemie();
        //������ ��������� ��������������
        en.GetComponent<EnemieController>().ResetStats();
        //��������
        en.SetActive(true);

        //�������� �������
        onEnemieSpawned?.Invoke(en.GetComponent<EnemieController>().IsRange);
        onStopFinded?.Invoke();
    }

    //����� ���������� ����� � ����������� �� �� ������
    private GameObject TakeRandomEnemie()
    {
        //�������� �������� ���� �����
        int randInd = Random.Range(0, randomEnemies.Length);
        //���������� ���� ��������� �����
        float chance = Random.Range(0f, 1f);
        if (chance <= randomEnemies[randInd].enemieStats.spawnChance / 100f)
        {
            //����������
            return TakeEnemie(randInd);
        }
        else
        {
            //���� ������
            return TakeRandomEnemie();
        }
    }

    //����������� �����, ���� �� ����, ���� ������
    private GameObject TakeEnemie(int enemieInd)
    {
        //���� �� ��������� � ����
        for (int i = 0; i < randomEnemies[enemieInd].enemiePool.Count; i++)
        {
            //���� ���� (���������) �� ����������
            if (!randomEnemies[enemieInd].enemiePool[i].activeSelf)
            {
                return randomEnemies[enemieInd].enemiePool[i];
            }
        }

        //���� �� ����� ��������� �� ������� � ��������� � ���
        GameObject obj = Instantiate(randomEnemies[enemieInd].enemiePref, 
            GetEnemiePos(randomEnemies[enemieInd].enemieStats), Quaternion.identity).gameObject;

        obj.SetActive(false);

        //��������� � ���
        randomEnemies[enemieInd].enemiePool.Add(obj);
        return obj;
    }

    //���������� ������� ����� � ����������� �� ��� ���� �����
    private Vector3 GetEnemiePos(EnemieStats stats)
    {
        if (!stats.rangeAttack)
        {
            //�������
            return meleeEnemiePosition.position;
        }
        else
        {
            //�������
            return rangeEnemiePosition.position;
        }
    }

    //�������� ����� ������
    public void StartSpawn()
    {
        isSpawn = true;
        isEnemieSpawn = false;
        currSpawnDelay = 0f;

        SetSpawnEnemie();
        onStartFinded?.Invoke();
    }

    //����������� ����� ������
    public void StopSpawn()
    {
        isSpawn = false;
        isEnemieSpawn = false;
        currSpawnDelay = 0f;
    }

    //�������������� ���� ��� ������
    private void InitializeEnemiePools()
    {
        for (int i = 0; i < randomEnemies.Length; i++) 
        {
            randomEnemies[i].enemiePool = new List<GameObject>();
        }
    }

    //�����
    private void SetPause(bool state)
    {
        isPause = state;
    }
}
