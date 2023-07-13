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
using UnityEngine.UI;

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
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI abilityDamageText;
    [SerializeField] TextMeshProUGUI chargeText;
    [SerializeField] GameObject abilityText;
    [SerializeField] GameObject clock;
    [SerializeField] Image abilityDamageSprite;
    [SerializeField] Image sprite;
    [SerializeField] Image rangeSprite;
    [SerializeField] Image resistSprite;
    [SerializeField] Image abilityEffectSprite;
    [SerializeField] GameObject diceCanvas;

    RuneManager runeManager;
    ExpRuneManager expRuneManager;
    WaveManager waveManager;
    bool onCooldown;
    Player player;
    Enemy lastPickedEnemy;


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
        Vector2 position = new Vector2(0.5f, 0.5f);

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
                enemy.SetActive(false);
                //enemies.RemoveAt(i);
                //Destroy(enemy.gameObject);
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
                    if (player == null)
                        continue;
                    enemies[i].Turn(player.transform);
                    enemies[i].currentAbilityCooldown -= 1;
                }
                yield return new WaitForSeconds(turnWaitTime);
            }

            State.Instance.IsPlayerTurn = true;
            onCooldown = false;
            if (player.needToDeleteShieldFromZZZ)
            {
                player.shield -= 5;
                player.needToDeleteShieldFromZZZ = false;
            }
        
        }
        else
        {
            State.Instance.FreeMove = false;
            State.Instance.IsPlayerTurn = true;
            onCooldown = false;
            if (player.needToDeleteShieldFromZZZ)
            {
                player.shield -= 5;
                player.needToDeleteShieldFromZZZ = false;
            }
        }
        waveManager.UpdateVisual();
    }
    // Update is called once per frame

    void UpdateVisualLiveEnemy()
    {
        if (lastPickedEnemy == null)
            return;
        
        //enemyHudCanvas.SetActive(true);
        healthText.text = lastPickedEnemy.health.ToString();
        shieldText.text = lastPickedEnemy.shield.ToString();
        damageText.text = lastPickedEnemy.damage.ToString();
        rangeText.text = lastPickedEnemy.range.ToString();
        moveSpeedText.text = lastPickedEnemy.MoveSpeed.ToString();

        switch (lastPickedEnemy.enemyType)
        {
            case EnemyType.Sword:
                nameText.text = "Мечник";
                resistSprite.gameObject.SetActive(false);
                sprite.sprite = lastPickedEnemy.GetComponentInChildren<SpriteRenderer>().sprite;
                abilityText.SetActive(false);
                chargeText.gameObject.SetActive(false);
                clock.SetActive(false);
                abilityDamageSprite.gameObject.SetActive(false);
                abilityEffectSprite.gameObject.SetActive(false);
                rangeSprite.gameObject.SetActive(false);
                abilityDamageText.gameObject.SetActive(false);
                break;
            case EnemyType.Mage:
                nameText.text = "Маг";
                resistSprite.gameObject.SetActive(false);
                sprite.sprite = lastPickedEnemy.GetComponentInChildren<SpriteRenderer>().sprite;
                abilityText.SetActive(true);
                chargeText.text = lastPickedEnemy.isCharging.ToString() + "/" + lastPickedEnemy.maxCharge.ToString();
                chargeText.gameObject.SetActive(true);
                clock.SetActive(true);
                abilityDamageSprite.gameObject.SetActive(true);
                abilityEffectSprite.sprite = lastPickedEnemy.abilityDamageSprite;
                abilityEffectSprite.gameObject.SetActive(false);
                rangeSprite.gameObject.SetActive(true);
                abilityDamageText.text = lastPickedEnemy.abilityDamage.ToString();
                abilityDamageText.gameObject.SetActive(true);
                break;
            case EnemyType.Knight:
                nameText.text = "Рыцарь";
                resistSprite.gameObject.SetActive(false);
                sprite.sprite = lastPickedEnemy.GetComponentInChildren<SpriteRenderer>().sprite;
                abilityText.SetActive(false);
                chargeText.gameObject.SetActive(false);
                clock.SetActive(false);
                abilityDamageSprite.gameObject.SetActive(false);
                abilityEffectSprite.gameObject.SetActive(false);
                rangeSprite.gameObject.SetActive(false);
                abilityDamageText.gameObject.SetActive(false);
                break;
            case EnemyType.FireKnight:
                nameText.text = "Огненный мечник";
                resistSprite.sprite = lastPickedEnemy.resistSprite;
                resistSprite.gameObject.SetActive(true);
                sprite.sprite = lastPickedEnemy.GetComponentInChildren<SpriteRenderer>().sprite;
                abilityText.SetActive(true);
                chargeText.text = lastPickedEnemy.isCharging.ToString() + "/" + lastPickedEnemy.maxCharge.ToString();
                chargeText.gameObject.SetActive(true);
                clock.SetActive(true);
                abilityDamageSprite.gameObject.SetActive(true);
                abilityEffectSprite.sprite = lastPickedEnemy.abilityDamageSprite;
                abilityEffectSprite.gameObject.SetActive(true);
                rangeSprite.sprite = lastPickedEnemy.rangeSprite;
                rangeSprite.gameObject.SetActive(true);
                abilityDamageText.text = lastPickedEnemy.abilityDamage.ToString();
                abilityDamageText.gameObject.SetActive(true);
                break;
            case EnemyType.ElectroMage:
                nameText.text = "Маг молнии";
                resistSprite.sprite = lastPickedEnemy.resistSprite;
                resistSprite.gameObject.SetActive(true);
                sprite.sprite = lastPickedEnemy.GetComponentInChildren<SpriteRenderer>().sprite;
                abilityText.SetActive(true);
                chargeText.text = lastPickedEnemy.isCharging.ToString() + "/" + lastPickedEnemy.maxCharge.ToString();
                chargeText.gameObject.SetActive(true);
                clock.SetActive(true);
                abilityDamageSprite.gameObject.SetActive(true);
                abilityEffectSprite.sprite = lastPickedEnemy.abilityDamageSprite;
                abilityEffectSprite.gameObject.SetActive(true);
                rangeSprite.sprite = lastPickedEnemy.rangeSprite;
                rangeSprite.gameObject.SetActive(true);
                abilityDamageText.text = lastPickedEnemy.abilityDamage.ToString();
                abilityDamageText.gameObject.SetActive(true);
                break;
        }
    }


    void UpdateVisualEnemyCanvas()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (hit.transform != null && hit.transform.gameObject.tag == "Enemy")
            {
                lastPickedEnemy = hit.transform.transform.gameObject.GetComponent<Enemy>();
                enemyHudCanvas.SetActive(true);
                diceCanvas.SetActive(false);
            }
            else
            {
                //Debug.Log("xd");
                enemyHudCanvas.SetActive(false);
                diceCanvas.SetActive(false);
            }
        }
    }



    void Update()
    {
        UpdateVisualEnemyCanvas();
        UpdateVisualLiveEnemy();

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


