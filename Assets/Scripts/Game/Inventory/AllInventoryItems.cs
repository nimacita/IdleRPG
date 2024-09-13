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
    [Tooltip("��� ��������, ������� ����� ���� �������� � ���������")]
    public List<MyInventoryItem> AllInvenoryItems;
    //public MyInventoryItem[] AllInvenoryItems;

    [Header("���� ����� � ������")]
    [Tooltip("��� ������ ���� ����� � ������")]
    public List<ArmorSet> AllArmorSets;

    [Header("������� ��������")]
    [Tooltip("��� ��������� ������ �������� ���� Item")]
    public List<MyInventoryItem> allItems;

    [Header("����� ���������")]
    [Tooltip("��� ����� ���������")]
    public List<MyInventoryItem> allPoition;

    [Header("��������� ��������")]
    public MyInventoryItem defaultSword;
    public MyInventoryItem defaultBow;
}
