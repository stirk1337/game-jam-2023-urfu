using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    EnemiesManager enemiesManager;
    [SerializeField] int currentWave;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int swordSpawnCount;

    void Start()
    {
        enemiesManager = FindObjectOfType<EnemiesManager>();
        NextWave();
    }

    void NextWave()
    {
        for(int i = 0; i < swordSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.Sword);
        }
        currentWave += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesManager.enemies.Count == 0)
        {
            NextWave();
        }
    }
}
