using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    EnemiesManager enemiesManager;
    [SerializeField] int currentWave;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int swordSpawnCount;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI enemiesCountText;
    int waveEnemiesCount;

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
        waveEnemiesCount = enemiesManager.enemies.Count;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        waveText.text = currentWave.ToString() + ":Волна";
        enemiesCountText.text = enemiesManager.enemies.Count.ToString() + "/" + waveEnemiesCount.ToString();
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
