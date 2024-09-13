using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstruments : MonoBehaviour
{
    [SerializeField]
    private InventoryController inventoryController = InventoryController.instance;
    [SerializeField]
    private LevelController levelController = LevelController.instance;

    //������ ������ ��� ����������
    public void GiveFullEquipSet(string setName)
    {
        inventoryController.GiveFullSet(setName);
    }

    //�������� �������
    public void GiveNewLevel()
    {
        levelController.SetNewLevel();
    }

    //��������� ��� ���������
    public void ResetAllStats()
    {
        PlayerPrefs.DeleteAll();
    }
}
