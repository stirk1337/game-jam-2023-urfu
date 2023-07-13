using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    EnemiesManager enemiesManager;
    [SerializeField] public int currentWave;
    [SerializeField] GameObject swordPrefab;
    [SerializeField] GameObject magePrefab;
    [SerializeField] GameObject knightPrefab;
    [SerializeField] GameObject electroMagePrefab;
    [SerializeField] GameObject fireSwordPrefab;
    [SerializeField] int swordSpawnCount;
    [SerializeField] int mageSpawnCount;
    [SerializeField] int knightSpawnCount;
    [SerializeField] int electroMageSpawnCount;
    [SerializeField] int fireSwordSpawnCount;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI enemiesCountText;
    int waveEnemiesCount;
    Ability[] abilities;
    Player player;
    [SerializeField] int INFswordSpawnCount;
    [SerializeField] int INFmageSpawnCount;
    [SerializeField] int INFknightSpawnCount;
    [SerializeField] int INFelectroMageSpawnCount;
    [SerializeField] int INFfireSwordSpawnCount;
    List<int> spawnCount;

    void Start()
    {
        enemiesManager = FindObjectOfType<EnemiesManager>();
        abilities = FindObjectsOfType<Ability>();
        player = FindObjectOfType<Player>();
        spawnCount = new List<int>() { 0, 0, 1, 2, 2 };
        NextWave();
    }

    void AbilitiesCooldown()
    {
        foreach (Ability ability in abilities)
        {
            ability.currentCooldown = player.currentTurn;
            ability.abilityState = Ability.AbilityState.Ready;
        }
    }


    void SetEnemiesCount(int sword, int mage, int knight, int electroMage, int fireSword)
    {
        swordSpawnCount = sword;
        mageSpawnCount = mage;
        knightSpawnCount = knight;
        electroMageSpawnCount = electroMage;
        fireSwordSpawnCount = fireSword;
    }

    void CalculateEnemiesCountOnWave()
    {
        switch (currentWave)
        {
            case 1:
                SetEnemiesCount(2, 0, 0, 0, 0);
                break;
            case 2:
                SetEnemiesCount(1, 1, 0, 0, 0);
                break;
            case 3:
                SetEnemiesCount(0, 2, 0, 0, 0);
                break;
            case 4:
                SetEnemiesCount(4, 0, 0, 0, 0);
                break;
            case 5:
                SetEnemiesCount(3, 2, 0, 0, 0);
                break;
            case 6:
                SetEnemiesCount(4, 0, 1, 0, 0);
                break;
            case 7:
                SetEnemiesCount(2, 2, 1, 0, 0);
                break;
            case 8:
                SetEnemiesCount(1, 2, 0, 0, 1);
                break;
            case 9:
                SetEnemiesCount(1, 0, 1, 0, 1);
                break;
            case 10:
                SetEnemiesCount(0, 2, 1, 0, 1);
                break;
            case 11:
                SetEnemiesCount(2, 2, 0, 1, 1);
                break;
            case 12:
                SetEnemiesCount(0, 0, 1, 1, 1);
                break;
            case 13:
                SetEnemiesCount(0, 1, 1, 1, 2);
                break;
            default: 
                int random = Random.Range(0, 6);
                spawnCount[random] += 1;
                SetEnemiesCount(spawnCount[0], spawnCount[1], spawnCount[2], spawnCount[3], spawnCount[4]);
                break;
        }
        
    }

    void NextWave()
    {
        currentWave += 1;
        AbilitiesCooldown();
        CalculateEnemiesCountOnWave();
        for (int i = 0; i < swordSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.Sword, swordPrefab);
        }

        for(int i = 0; i < mageSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.Mage, magePrefab);
        }

        for (int i = 0; i < knightSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.Knight, knightPrefab);
        }

        for (int i = 0; i < electroMageSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.ElectroMage, electroMagePrefab);
        }

        for (int i = 0; i < fireSwordSpawnCount; i++)
        {
            enemiesManager.Spawn(Enemy.EnemyType.FireKnight, fireSwordPrefab);
        }

        waveEnemiesCount = enemiesManager.enemies.Count;
        
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        waveText.text = currentWave.ToString() + " Волна";
        enemiesCountText.text = (waveEnemiesCount - enemiesManager.enemies.Count).ToString() + "/" + waveEnemiesCount.ToString();
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
