using UnityEngine;

[System.Serializable]
public struct SpawnerLocation
{
    [Tooltip("Название текщей локации")]
    public string locationName;
    [Tooltip("Список случайных врагов с их харакетристиками на текущей локации")]
    public RandomEnemy[] randomEnemies;
}

[CreateAssetMenu(fileName = "SpawnerStats", menuName = "ScriptableObjects/Spawner")]
public class SpawnerStats : ScriptableObject
{
    [Header("Main Stats")]
    [Tooltip("Значения для случайной задержки перед спавном врага, (X - от ; Y - до)")]
    public Vector2 randomSpawnDelay = new Vector2(0.5f, 1.5f);

    [Header("Spawner Location Stats")]
    [Tooltip("Все локации и список врагов на локации")]
    public SpawnerLocation[] spawnerLocations;
}
