using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private PlayerCurrentStats currentStats;

    [Header("Locations")]
    [Tooltip("������ �� ����� ���������� ���������")]
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

    //��������� ������� �������
    public void UpdateLocation(int locInd)
    {
        currentStats.CurrentLocationIndex = locInd;

        //��������� ������� �����
        AudioController.instance.DefineAmbient();

        UpdateCurrentLocation();
        MapClose();
    }

    //��������� ��� ������� ������ �� ������� ��������� �������
    private void UpdateCurrentLocation()
    {
        int locInd = currentStats.CurrentLocationIndex;

        if (locInd < allLocations.Length)
        {
            //�������� ������ �������
            OpenCurrentLoc(locInd);
            //��������� �������
            enemieSpawner.SetRandomEnemies(locInd);
        }
        else
        {
            //������� ������
            Debug.LogError("�� ���������� ������� � ����� ��������");
        }
    }

    //�������� ������ ������� �� �������
    private void OpenCurrentLoc(int currInd)
    {
        for (int i = 0; i < allLocations.Length; i++) 
        {
            if (i == currInd)
            {
                //���� ������ - ��������
                allLocations[i].SetActive(true);
            }
            else
            {
                //���� �� - ���������
                allLocations[i].SetActive(false);
            }
        }
    }

    //��������� ������
    private void UpdateLocBtns()
    {
        foreach (LocationBtn btn in locationBtns)
        {
            btn.UpdateBtnVisual();
        }
    }

    //������� �������
    public int GetCurrLoc
    {
        get
        {
            return currentStats.CurrentLocationIndex;
        }
    }

    //��������� �����
    private void MapClose()
    {
        UpdateCurrentLocation();
        mapView.SetActive(false);
        //GameController.instance.SetPause(false);
    }

    //��������� �����
    public void MapOpen()
    {
        mapView.SetActive(true);
        UpdateLocBtns();
    }
}
