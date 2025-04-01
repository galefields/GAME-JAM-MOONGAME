using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 1.25f;

    private List<GameObject> meleePool;
    private List<GameObject> rangedPool;
    private Dictionary<Transform, GameObject> activeEnemies;

    void Start()
    {
        meleePool = new List<GameObject>();
        rangedPool = new List<GameObject>();
        activeEnemies = new Dictionary<Transform, GameObject>();

        foreach (Transform spawn in spawnPoints)
        {
            activeEnemies[spawn] = null;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject meleeEnemy = Instantiate(meleeEnemyPrefab);
            meleeEnemy.SetActive(false);
            meleePool.Add(meleeEnemy);

            GameObject rangedEnemy = Instantiate(rangedEnemyPrefab);
            rangedEnemy.SetActive(false);
            rangedPool.Add(rangedEnemy);
        }

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        List<Transform> availableSpawnPoints = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            if (activeEnemies[spawn] == null)
            {
                availableSpawnPoints.Add(spawn);
            }
        }

        if (availableSpawnPoints.Count == 0) return;

        Transform selectedSpawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];

        GameObject enemy = Random.value > 0.5f ? ActivateEnemyFromPool(meleePool) : ActivateEnemyFromPool(rangedPool);

        if (enemy != null)
        {
            enemy.transform.position = selectedSpawnPoint.position;
            enemy.SetActive(true);
            activeEnemies[selectedSpawnPoint] = enemy;
        }
    }

    GameObject ActivateEnemyFromPool(List<GameObject> pool)
    {
        foreach (GameObject enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }
        return null;
    }

    public void EnemyDestroyed(GameObject enemy)
    {
        foreach (var pair in activeEnemies)
        {
            if (pair.Value == enemy)
            {
                activeEnemies[pair.Key] = null;
                break;
            }
        }
        enemy.SetActive(false);
        
        if (meleePool.Contains(enemy) || rangedPool.Contains(enemy))
        {
            return;
        }
        else if (enemy.CompareTag("MeleeEnemy"))
        {
            meleePool.Add(enemy);
        }
        else if (enemy.CompareTag("RangedEnemy"))
        {
            rangedPool.Add(enemy);
        }
    }
}
