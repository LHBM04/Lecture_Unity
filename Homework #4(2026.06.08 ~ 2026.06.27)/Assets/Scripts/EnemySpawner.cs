using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _enemyPrefabs;

    [SerializeField]
    private float _spawnRadius;

    [SerializeField]
    private float _minSpawnDistance;

    private PlayerInputActions _inputActions;

    private void Reset()
    {
        _spawnRadius = 20.0f;
        _minSpawnDistance = 5.0f;
    }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions.Player.Spawn.performed += OnSpawnPerformed;
    }
    
    private void OnDisable()
    {
        _inputActions.Player.Spawn.performed -= OnSpawnPerformed;
        _inputActions.Disable();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        Gizmos.DrawWireSphere(transform.position, _minSpawnDistance);
    }
#endif

    private void OnSpawnPerformed(InputAction.CallbackContext context)
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("스폰할 적 프리팹이 없습니다.", this);
#endif
            return;
        }

        for (int count = 0; count < 5; ++count)
        {
            float radius = Random.Range(_minSpawnDistance, _spawnRadius);
            float angle = Random.Range(0.0f, Mathf.PI * 2.0f);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0.0f,
                Mathf.Sin(angle) * radius
            );
            Vector3 spawnPosition = transform.position + offset;

            GameObject prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)];
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}