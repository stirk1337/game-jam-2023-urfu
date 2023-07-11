using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using static Enemy;
using TMPro;
using static Ability;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] public List<Enemy> enemies;
    [SerializeField] public float turnWaitTime;
    [SerializeField] float afterPlayerWait;
    [SerializeField] int minSpawnDistance;
    [SerializeField] int maxSpawnDistance;
    [SerializeField] GameObject enemyHudCanvas;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI shieldText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] TextMeshProUGUI moveSpeedText;
    RuneManager runeManager;
    ExpRuneManager expRuneManager;
    WaveManager waveManager;
    bool onCooldown;
    Player player;


    void Start()
    {
        onCooldown = false;
        player = FindObjectOfType<Player>();
        waveManager = FindObjectOfType<WaveManager>();
        runeManager = FindObjectOfType<RuneManager>();
        expRuneManager = FindObjectOfType<ExpRuneManager>();
    }

    Vector2 GetRandomSpawnPoint()
    {
        Vector2 position = new Vector2(0.5f , 0.5f);

        int offsetX = Random.Range(minSpawnDistance, maxSpawnDistance + 1) * (Random.value < 0.5f ? -1 : 1);
        int offsetY = Random.Range(minSpawnDistance, maxSpawnDistance + 1) * (Random.value < 0.5f ? -1 : 1);

        return position + new Vector2(offsetX, offsetY);
    }

    public void Spawn(Enemy.EnemyType enemyType, GameObject prefab)
    {
        Vector2 spawnPoint = GetRandomSpawnPoint();
        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(spawnPoint, Vector2.zero);
            if (hit.transform == null)
            {
                break;
            }
            spawnPoint = GetRandomSpawnPoint();
        }
        GameObject gameObj = Instantiate(prefab, spawnPoint, transform.rotation);
        gameObj.SetActive(true);
        Enemy enemy = gameObj.GetComponent<Enemy>();
        
        enemy.enemyType = enemyType;
        enemy.Init();
        enemies.Add(enemy);   
    }

    public void DeleteEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {

            if (enemies[i].IsDead)
            {
                GameObject enemy = enemies[i].gameObject;
                enemies.RemoveAt(i);
                Destroy(enemy.gameObject);
            }
        }
    }

    IEnumerator EnemiesTurn()
    {
        enemies = enemies.OrderByDescending(x => x.distanceToPlayer).ToList();
        
        yield return new WaitForSeconds(afterPlayerWait);
        if (!State.Instance.FreeMove)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {

                if (enemies[i].IsDead)
                {
                    Destroy(enemies[i].gameObject);
                    enemies.RemoveAt(i);
                }
                else
                {
                    enemies[i].Turn(player.transform);
                    enemies[i].currentAbilityCooldown -= 1;
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
        waveManager.UpdateVisual();
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hit.transform != null && hit.transform.gameObject.tag == "Enemy")
            {
                Enemy enemy = hit.transform.transform.gameObject.GetComponent<Enemy>();
                enemyHudCanvas.SetActive(true);
                healthText.text = enemy.health.ToString();
                shieldText.text = enemy.shield.ToString();
                damageText.text = enemy.damage.ToString();
                rangeText.text = enemy.range.ToString();
                moveSpeedText.text = enemy.MoveSpeed.ToString();
            }
            else
            {
                enemyHudCanvas.SetActive(false);
            }
        }

        if (!State.Instance.IsPlayerTurn && !onCooldown)
        {
            StartCoroutine(EnemiesTurn());
            player.currentTurn += 1;
            onCooldown = true;
            if (player.state[Enemy.ElementState.Fire] > 0)
            {
                player.health -= 5;
                if (player.health <= 0)
                {
                    player.Die();
                }
            }
            foreach (var key in player.state.Keys.ToList())
            {
                if (player.state[key] > 0)
                    player.state[key] = player.state[key] - 1;
            }
        }
    }
}
