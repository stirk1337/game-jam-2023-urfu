using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] public List<Enemy> enemies;
    [SerializeField] float turnWaitTime;
    [SerializeField] float afterPlayerWait;
    [SerializeField] int spawnSwordCount;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int minSpawnDistance;
    [SerializeField] int maxSpawnDistance;
    bool onCooldown;
    Player player;


    void Start()
    {
        onCooldown = false;
        player = FindObjectOfType<Player>();

    }

    Vector2 GetRandomSpawnPoint()
    {
        Vector2 playerPosition = player.transform.position;

        int offsetX = Random.Range(minSpawnDistance, maxSpawnDistance + 1) * (Random.value < 0.5f ? -1 : 1);
        int offsetY = Random.Range(minSpawnDistance, maxSpawnDistance + 1) * (Random.value < 0.5f ? -1 : 1);

        return playerPosition + new Vector2(offsetX, offsetY);
    }

    public void Spawn(Enemy.EnemyType enemyType)
    {  
        GameObject gameObj = Instantiate(enemyPrefab, GetRandomSpawnPoint(), transform.rotation);
        Enemy enemy = gameObj.GetComponent<Enemy>();
        enemy.enemyType = enemyType;
        enemy.Init();
        enemies.Add(enemy);
    }

    IEnumerator EnemiesTurn()
    {
        yield return new WaitForSeconds(afterPlayerWait);
        if (!State.Instance.FreeMove)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {

                if (enemies[i].IsDead)
                {
                    enemies.RemoveAt(i);
                }
                else
                {
                    enemies[i].Turn(player.transform);
                }
                yield return new WaitForSeconds(turnWaitTime);
            }

            State.Instance.IsPlayerTurn = true;
            onCooldown = false;
        }
        else
        {
            State.Instance.FreeMove = false;
            State.Instance.IsPlayerTurn = true;
            onCooldown = false;
        }
    }
        // Update is called once per frame
        void Update()
        {
            if (!State.Instance.IsPlayerTurn && !onCooldown)
            {
                StartCoroutine(EnemiesTurn());
                player.currentTurn += 1;
                onCooldown = true;
            }
        }
}
