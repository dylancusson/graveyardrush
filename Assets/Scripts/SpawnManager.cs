using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameSettings gameSettings;
    public List<Transform> spawnPoints; 

    private int _zombiesToSpawn;

    void Start()
    {

        _zombiesToSpawn = (int)(4.0 - gameSettings.difficultyLevel) * 3;

        InvokeRepeating(nameof(SpawnWave), 2f, 15f);
    }

    void SpawnWave()
    {
        for (int i = 0; i < _zombiesToSpawn; i++)
        {
            // Pick a random spawn point from your list
            int randomIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[randomIndex];

            // Create the zombie at that position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 2.0f, NavMesh.AllAreas)) {
                Instantiate(zombiePrefab, hit.position, spawnPoint.rotation);
            }
        }
        
        Debug.Log($"Spawned {_zombiesToSpawn} zombies for difficulty: {gameSettings.difficultyLevel}");
    }
}