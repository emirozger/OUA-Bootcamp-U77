using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public float timeBetweenEnemies = 1f;
    public int[] enemiesPerWave;
    private int currentWave = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private bool waveInProgress = false;
    public float healthMultiplierPerWave = 1.1f;
    public float attackRangeMultiplierPerWave = 1.1f;
    public float sightRangeMultiplierPerWave = 1.1f;




    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        while (currentWave < enemiesPerWave.Length)
        {
            waveInProgress = true;
            enemiesSpawned = 0;
            enemiesKilled = 0;

            // Düşmanların özelliklerini zorlaştır
            float healthMultiplier = Mathf.Pow(healthMultiplierPerWave, currentWave);
            float attackRangeMultiplier = Mathf.Pow(attackRangeMultiplierPerWave, currentWave);
            float sightRangeMultiplier = Mathf.Pow(sightRangeMultiplierPerWave, currentWave);

            while (enemiesSpawned < enemiesPerWave[currentWave])
            {
                SpawnEnemy(healthMultiplier, attackRangeMultiplier, sightRangeMultiplier);
                enemiesSpawned++;
                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            while (enemiesKilled < enemiesPerWave[currentWave])
            {
                yield return null;
            }

            currentWave++;
            waveInProgress = false;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // Bütün waveler bitti, win ekranı.
    }




    private void SpawnEnemy(float healthMultiplier, float attackRangeMultiplier, float sightRangeMultiplier)
{
    int randomIndex = Random.Range(0, enemyPrefabs.Length);
    GameObject enemyPrefab = enemyPrefabs[randomIndex];
    Transform spawnPoint = GetRandomSpawnPoint();
    GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

    Enemy enemy = enemyObject.GetComponent<Enemy>();
    if (enemy != null)
    {
        enemy.OnDeath += EnemyDeathHandler;
        enemy.health *= healthMultiplier;
        enemy.attackRange *= attackRangeMultiplier;
        enemy.sightRange *= sightRangeMultiplier;
    }
}




    private Transform GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }

    private void EnemyDeathHandler(Enemy enemy)
    {
        enemiesKilled++;
        enemy.OnDeath -= EnemyDeathHandler;
    }

    public bool IsWaveInProgress()
    {
        return waveInProgress;
    }
}
