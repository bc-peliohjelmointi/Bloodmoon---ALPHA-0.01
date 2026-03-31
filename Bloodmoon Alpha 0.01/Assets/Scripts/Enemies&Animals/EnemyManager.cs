using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    [SerializeField] private EnemySpawn[] enemyPrefabs;
    [SerializeField] private int maxEntities = 128;
    [SerializeField] private float spawnRadius = 50f;

    public void OnEnable() {
        foreach (var spawn in enemyPrefabs) {
            for (int i = 0; i < 10 / enemyPrefabs.Length; i++) {
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
                spawnPos.y = transform.position.y + 200f;

                if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 400f)) {
                    spawnPos.y = hit.point.y;
                    Debug.Log($"Spawned {spawn.enemyName} at {spawnPos}");
                }
                else {
                    spawnPos.y = 0f;
                }

                Instantiate(spawn.prefab, spawnPos, Quaternion.identity);
            }
        }
    }
}

[System.Serializable]
public class EnemySpawn
{
    public string enemyName;
    public GameObject prefab;
}