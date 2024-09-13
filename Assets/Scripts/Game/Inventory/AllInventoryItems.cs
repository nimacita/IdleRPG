using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ArmorSet
{
    public string SetName;
    public MyInventoryItem Sword;
    public MyInventoryItem Bow;
    public MyInventoryItem Helmet;
    public MyInventoryItem Torso;
    public MyInventoryItem Gloves;
    public MyInventoryItem Boots;
}

[CreateAssetMenu(fileName = "AllItems", menuName = "ScriptableObjects/AllInventoryItems")]
public class AllInventoryItems : ScriptableObject
{
    [Header("All Inventory Items")]
    [Tooltip("Все предметы, которые могут быть доступны в инвентаре")]
    public List<MyInventoryItem> AllInvenoryItems;
    //public MyInventoryItem[] AllInvenoryItems;

    [Header("Сеты брони и оружия")]
    [Tooltip("Все полные сеты брони и оружия")]
    public List<ArmorSet> AllArmorSets;

    [Header("Обычные предметы")]
    [Tooltip("Все возможные обычне предметы типа Item")]
    public List<MyInventoryItem> allItems;

    [Header("Зелья исцеления")]
    [Tooltip("Все зелья исцеления")]
    public List<MyInventoryItem> allPoition;

    [Header("Начальные предметы")]
    public MyInventoryItem defaultSword;
    public MyInventoryItem defaultBow;
}
