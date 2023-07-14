using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;

    private void Start()
    {
        
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].transform.position, Quaternion.identity);
            Rigidbody enemyRB = enemy.AddComponent<Rigidbody>();
            enemy.GetComponent<NavMeshAgent>().speed = 5f;
            enemyRB.freezeRotation = true;
            enemyRB.mass = 10f;
        }
        
    }
    
}
