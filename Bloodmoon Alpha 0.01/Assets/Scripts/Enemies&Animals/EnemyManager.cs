using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private EnemySpawn[] enemyPrefabs;
    [SerializeField] private int maxEntities = 20;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float groundRayHeight = 200f;
    [SerializeField] private float groundRayDistance = 400f;

    private int currentEntityCount;

    private void OnEnable()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        int spawnPerType = Mathf.Max(1, maxEntities / enemyPrefabs.Length);

        foreach (var spawn in enemyPrefabs)
        {
            if (spawn.prefab == null) continue;

            for (int i = 0; i < Mathf.Min(spawnPerType, spawn.count); i++)
            {
                if (currentEntityCount >= maxEntities) return;

                if (TryGetGroundPosition(out Vector3 spawnPos))
                {
                    Instantiate(spawn.prefab, spawnPos, Quaternion.identity);
                    currentEntityCount++;
                }
            }
        }

        Zombie zombie = FindObjectOfType<Zombie>();
        zombie.TakeDamage(10f, Vector3.zero);
    }

    private bool TryGetGroundPosition(out Vector3 position)
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPoint.y = transform.position.y + groundRayHeight;

        if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, groundRayDistance))
        {
            position = hit.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }
}

[System.Serializable]
public class EnemySpawn
{
    public string enemyName;
    public GameObject prefab;
    [Min(1)] public int count = 5;
}