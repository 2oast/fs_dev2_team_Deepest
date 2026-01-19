using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMode
    { 
        SingleType,
        RandomType
    }

    static List<EnemySpawner> allSpawners = new List<EnemySpawner>();

    [Header("----- Spawner Settings -----")]
    [SerializeField] SpawnMode spawnMode = SpawnMode.SingleType;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] int singleTypeIndex = 0;

    [Tooltip("How many enemies total should spawn each time the player enters range.")]
    [SerializeField] int enemiesToSpawn = 3;

    [Tooltip("Time between each individual spawn.")]
    [SerializeField] float spawnDelay = 5f;

    [Header("----- Distance Settings -----")]
    [SerializeField] float activateDistance = 25f;
    [SerializeField] float deactivateDistance = 30f;

    [Header("----- Spawn Offsets -----")]
    [SerializeField] float spawnRadius = 3f;

    Transform player;
    bool playerInsideRange = false;

    Coroutine spawnRoutine;
    List<GameObject> spawnedEnemies = new List<GameObject>();

    void OnEnable()
    {
        if (!allSpawners.Contains(this))
        {
            allSpawners.Add(this);
        }
    }

    void OnDisable()
    {
        if (allSpawners.Contains(this))
        {
            allSpawners.Remove(this);
        }
    }

    void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (!playerInsideRange && distance <= activateDistance)
        {
            playerInsideRange = true;
            StartSpawning();
        }
        else if (playerInsideRange && distance > deactivateDistance)
        {
            playerInsideRange = false;
            ResetSpawner();
        }

        CleanupSpawnedList();
    }

    void FindPlayer()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.transform;
        }
        else
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }


    void StartSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        spawnRoutine = StartCoroutine(SpawnEnemiesOneByOne());
    }

    IEnumerator SpawnEnemiesOneByOne()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (!playerInsideRange)
                yield break;

            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSingleEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner on " + name + " has no enemyPrefabs assigned.");
            return;
        }

        GameObject prefabToSpawn = GetPrefabForSpawn();
        if (prefabToSpawn == null)
            return;

        Vector3 offset = Random.insideUnitSphere;
        offset.y = 0f;
        offset = offset.normalized * Random.Range(0f, spawnRadius);

        Vector3 spawnPos = transform.position + offset;

        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }

    GameObject GetPrefabForSpawn()
    {
        if (spawnMode == SpawnMode.SingleType)
        {
            int index = Mathf.Clamp(singleTypeIndex, 0, enemyPrefabs.Length - 1);
            return enemyPrefabs[index];
        }
        else
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }
    }


    void ResetSpawner()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();
    }

    void CleanupSpawnedList()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
                spawnedEnemies.RemoveAt(i);
        }
    }

    public void ForceReset()
    {
        ResetSpawner();
        playerInsideRange = false;
    }

    public static void ResetAllSpawners()
    {
        for (int i = 0; i < allSpawners.Count; i++)
        {
            if (allSpawners[i] != null)
            {
                allSpawners[i].ForceReset();
            }
        }
    }
}
