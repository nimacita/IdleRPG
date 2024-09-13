using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public struct RandomEnemy
{
    [Tooltip("Префаб врага")]
    public GameObject enemiePref;
    [Tooltip("Характеристики данного врага")]
    public EnemieStats enemieStats;
    [Tooltip("Пул заспавненных врагов")]
    public List<GameObject> enemiePool;
}

public class EnemieSpawner : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private SpawnerStats spawnerStats;

    [Header("Enemie Settings")]
    [Tooltip("Список случайных врагов с их харакетристиками")]
    private RandomEnemy[] randomEnemies;

    [Header("Enemie Positions Settings")]
    [Tooltip("Позиция спавна врагов с ближним типом атаки")]
    [SerializeField] private Transform meleeEnemiePosition;
    [Tooltip("Позиция спавна врагов с дальним типом атаки")]
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

    //устанавлвиаем харакетристики
    private void SetStats()
    {
        randomSpawnDelay = spawnerStats.randomSpawnDelay;
    }

    //устанавливаем врагов в зависимости от локации
    public void SetRandomEnemies(int locIndex = 0)
    {
        randomEnemies = spawnerStats.spawnerLocations[locIndex].randomEnemies;

        InitializeEnemiePools();
    }

    private void FixedUpdate()
    {
        SpawnCD();
    }

    //устанавливаем значения для начала спавна
    private void SetSpawnEnemie()
    {
        //выбираем случайную задержку перед поялвением врага
        currSpawnDelay = Random.Range(randomSpawnDelay.x, randomSpawnDelay.y);
        isEnemieSpawn = true;
    }
    
    //считаем кул даун задержки перед спавном
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

                //спавним врага
                EnemieSpawn();
                isEnemieSpawn = false;
            }
        }
    }

    //спавним врага
    private void EnemieSpawn()
    {
        //выбираем случайного врага
        GameObject en = TakeRandomEnemie();
        //Задаем начальные характеристики
        en.GetComponent<EnemieController>().ResetStats();
        //включаем
        en.SetActive(true);

        //вызываем событие
        onEnemieSpawned?.Invoke(en.GetComponent<EnemieController>().IsRange);
        onStopFinded?.Invoke();
    }

    //берем случайного врага в зависимости от их шансов
    private GameObject TakeRandomEnemie()
    {
        //выбираем случайно титп врага
        int randInd = Random.Range(0, randomEnemies.Length);
        //генерируем шанс появления врага
        float chance = Random.Range(0f, 1f);
        if (chance <= randomEnemies[randInd].enemieStats.spawnChance / 100f)
        {
            //возвращаем
            return TakeEnemie(randInd);
        }
        else
        {
            //идем заново
            return TakeRandomEnemie();
        }
    }

    //вовзаращаем врага, либо из пула, либо нового
    private GameObject TakeEnemie(int enemieInd)
    {
        //если ли свободный в пуле
        for (int i = 0; i < randomEnemies[enemieInd].enemiePool.Count; i++)
        {
            //если есть (неактивен) то возвращаем
            if (!randomEnemies[enemieInd].enemiePool[i].activeSelf)
            {
                return randomEnemies[enemieInd].enemiePool[i];
            }
        }

        //если не нашли свободный то спавним и добавляем в пул
        GameObject obj = Instantiate(randomEnemies[enemieInd].enemiePref, 
            GetEnemiePos(randomEnemies[enemieInd].enemieStats), Quaternion.identity).gameObject;

        obj.SetActive(false);

        //добалвяем в пул
        randomEnemies[enemieInd].enemiePool.Add(obj);
        return obj;
    }

    //Возвращаем позицию врага в зависимости от его типа атаки
    private Vector3 GetEnemiePos(EnemieStats stats)
    {
        if (!stats.rangeAttack)
        {
            //ближний
            return meleeEnemiePosition.position;
        }
        else
        {
            //дальний
            return rangeEnemiePosition.position;
        }
    }

    //Начинаем спавн врагов
    public void StartSpawn()
    {
        isSpawn = true;
        isEnemieSpawn = false;
        currSpawnDelay = 0f;

        SetSpawnEnemie();
        onStartFinded?.Invoke();
    }

    //заканчиваем спавн врагов
    public void StopSpawn()
    {
        isSpawn = false;
        isEnemieSpawn = false;
        currSpawnDelay = 0f;
    }

    //инициализируем пулы для врагов
    private void InitializeEnemiePools()
    {
        for (int i = 0; i < randomEnemies.Length; i++) 
        {
            randomEnemies[i].enemiePool = new List<GameObject>();
        }
    }

    //пауза
    private void SetPause(bool state)
    {
        isPause = state;
    }
}
