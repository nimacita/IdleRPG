using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstruments : MonoBehaviour
{
    [SerializeField]
    private InventoryController inventoryController = InventoryController.instance;
    [SerializeField]
    private LevelController levelController = LevelController.instance;

    //выдаем полный сет экипировки
    public void GiveFullEquipSet(string setName)
    {
        inventoryController.GiveFullSet(setName);
    }

    //повышаем уровень
    public void GiveNewLevel()
    {
        levelController.SetNewLevel();
    }

    //сбрасывае все настройки
    public void ResetAllStats()
    {
        PlayerPrefs.DeleteAll();
    }
}
