using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    public float timeBetweenEnemies = 1f;
    public int[] enemiesPerWave;
    private int currentWave = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private bool waveInProgress = false;

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

            while (enemiesSpawned < enemiesPerWave[currentWave])
            {
                SpawnEnemy();
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

        
        // bütün waveler bitti win ekranı.
        
    }

    private void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.OnDeath += EnemyDeathHandler;
        }
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
