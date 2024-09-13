using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;

    [Header("Locations")]
    [Tooltip("ћассив со всеми доступными локаци€ми")]
    [SerializeField] private GameObject[] allLocations;

    [Header("Map View")]
    [SerializeField] private GameObject mapView;
    [SerializeField] private Button closeMapBtn;
    [SerializeField] private LocationBtn[] locationBtns;

    [Header("Spawner")]
    [SerializeField] private EnemieSpawner enemieSpawner;

    public static LocationController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateCurrentLocation();

        mapView.SetActive(false);
        closeMapBtn.onClick.AddListener(MapClose);
    }

    //обновл€ем текущую локацию
    public void UpdateLocation(int locInd)
    {
        currentStats.CurrentLocationIndex = locInd;

        //обнвол€ем фоновые звуки
        AudioController.instance.DefineAmbient();

        UpdateCurrentLocation();
        MapClose();
    }

    //обновл€ем вид локации исход€ из текущей выбранной локации
    private void UpdateCurrentLocation()
    {
        int locInd = currentStats.CurrentLocationIndex;

        if (locInd < allLocations.Length)
        {
            //включаем нужную локацию
            OpenCurrentLoc(locInd);
            //обновл€ем спавнер
            enemieSpawner.SetRandomEnemies(locInd);
        }
        else
        {
            //выводим ошибку
            Debug.LogError("Ќе существует локации с таким индексом");
        }
    }

    //включаем нужную локацию по индексу
    private void OpenCurrentLoc(int currInd)
    {
        for (int i = 0; i < allLocations.Length; i++) 
        {
            if (i == currInd)
            {
                //если нужна€ - включаем
                allLocations[i].SetActive(true);
            }
            else
            {
                //если не - выключаем
                allLocations[i].SetActive(false);
            }
        }
    }

    //обновл€ем кнопки
    private void UpdateLocBtns()
    {
        foreach (LocationBtn btn in locationBtns)
        {
            btn.UpdateBtnVisual();
        }
    }

    //текуща€ локаци€
    public int GetCurrLoc
    {
        get
        {
            return currentStats.CurrentLocationIndex;
        }
    }

    //закрываем карту
    private void MapClose()
    {
        UpdateCurrentLocation();
        mapView.SetActive(false);
        //GameController.instance.SetPause(false);
    }

    //открываем карту
    public void MapOpen()
    {
        mapView.SetActive(true);
        UpdateLocBtns();
    }
}
