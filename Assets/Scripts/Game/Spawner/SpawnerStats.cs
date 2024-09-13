using UnityEngine;

[System.Serializable]
public struct SpawnerLocation
{
    [Tooltip("�������� ������ �������")]
    public string locationName;
    [Tooltip("������ ��������� ������ � �� ���������������� �� ������� �������")]
    public RandomEnemy[] randomEnemies;
}

[CreateAssetMenu(fileName = "SpawnerStats", menuName = "ScriptableObjects/Spawner")]
public class SpawnerStats : ScriptableObject
{
    [Header("Main Stats")]
    [Tooltip("�������� ��� ��������� �������� ����� ������� �����, (X - �� ; Y - ��)")]
    public Vector2 randomSpawnDelay = new Vector2(0.5f, 1.5f);

    [Header("Spawner Location Stats")]
    [Tooltip("��� ������� � ������ ������ �� �������")]
    public SpawnerLocation[] spawnerLocations;
}
