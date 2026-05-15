using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> enemyPrefabs;

    [SerializeField]
    private float spawnRadius;

    [SerializeField]
    private float minSpawnDistance;

    [SerializeField]
    private float spawnTime;
    private float spawnTimer;

    private void Reset()
    {
        spawnRadius = 20.0f;
        minSpawnDistance = 5.0f;
        spawnTime = 1.0f;
    }

    private void Start()
    {
        spawnTimer = spawnTime;
    }

    private void Update()
    {
        if ((spawnTimer -= Time.deltaTime) > 0.0f)
        {
            return;
        }

        SpawnEnemy();
        spawnTimer = spawnTime;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("스폰할 적 프리팹이 없습니다.", this);
#endif
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float radius = Random.Range(minSpawnDistance, spawnRadius);
        float angle = Random.Range(0.0f, Mathf.PI * 2.0f);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            0.0f,
            Mathf.Sin(angle) * radius
        );

        return transform.position + offset;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.DrawWireSphere(transform.position, minSpawnDistance);
    }
#endif
}