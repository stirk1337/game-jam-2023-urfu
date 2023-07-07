using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.EventSystems.EventTrigger;

public class Ability : MonoBehaviour
{
    enum AbilityType
    {
        Melee,
        Splash,
        Dash
    }
    public enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        fireDashEnemies.Add(collision.gameObject);
    }

    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int cooldown;
    [SerializeField] int currentCooldown;
    [SerializeField] AbilityType abilityType;
    [SerializeField] public AbilityState abilityState;
    [SerializeField] Player player;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] AbilitiesUIManager abilitiesUIManager;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] DiceManager diceManager;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] BoxCollider2D abilityCollider;
    List<Enemy> enemies;
    List<GameObject> fireDashEnemies;
   
    Tilemap tilemap;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        enemies = FindObjectOfType<EnemiesManager>().enemies;
        fireDashEnemies = new List<GameObject>();
    }

    List<Vector2> FindAttackRangeDiamondShape(Vector2 position, int attackRange)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        for (int i = (int)position.x - attackRange; i <= (int)position.x + attackRange; i++)
        {
            for (int j = (int)position.y - attackRange; j <= (int)position.y + attackRange; j++)
            {
                if (Mathf.Abs(i - position.x) + Mathf.Abs(j - position.y) <= attackRange)
                {
                    attackPoints.Add(new Vector2(i, j));
                }
            }
        }
        return attackPoints;
    }

    List<Vector2> FindAttackRangeForwardShape(Vector2 position, float attackRange)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        for (float i = position.x - attackRange; i <= position.x + attackRange; i += 0.5f)
        {
            for (float j = position.y - attackRange; j <= position.y + attackRange; j += 0.5f)
            {
                if (new Vector2(i, j) != position && (i == position.x || j == position.y))
                {
                    attackPoints.Add(new Vector2(i, j));
                }
            }
        }
        return attackPoints;
    }

    List<Vector2> FindAttackRangeSquareShape(Vector2 position, float attackRange)
    {
        List<Vector2> attackPoints = new List<Vector2>();

        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int range = Mathf.RoundToInt(attackRange);

        for (int i = x - range; i <= x + range; i++)
        {
            for (int j = y - range; j <= y + range; j++)
            {
                if (i != x || j != y)
                {
                    attackPoints.Add(new Vector2(i, j));
                }
            }
        }

        return attackPoints;
    }



    void DisplayAttackRange(Color color, AbilityType abilityType)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        int currentRange = range;

        if (abilityType == AbilityType.Melee)
        {
            currentRange = range;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Splash && player.abilityElement == Player.AbilityElement.Default)
        {
            currentRange = range;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Splash && player.abilityElement == Player.AbilityElement.Fire)
        {
            currentRange = range + 1;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Splash && player.abilityElement == Player.AbilityElement.Electro)
        {
            currentRange = range;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Splash && player.abilityElement == Player.AbilityElement.Wind)
        {
            currentRange = 1;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Dash && player.abilityElement == Player.AbilityElement.Default)
        {
            currentRange = range;
            attackPoints = FindAttackRangeForwardShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Dash && player.abilityElement == Player.AbilityElement.Fire)
        {
            currentRange = range;
            attackPoints = FindAttackRangeForwardShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        foreach (Vector2 point in attackPoints)
        {
            Vector3Int cellPos = new Vector3Int((int)point.x, (int)point.y, 0);
            tilemap.SetTileFlags(cellPos, TileFlags.None);
            tilemap.SetColor(cellPos, color);
        }
    }

    public void Select()
    {
        if (!State.Instance.IsPlayerTurn)
            return;

        abilitiesUIManager.TurnInteractable(buttonGameObject);
        playerMove.InSelect = !playerMove.InSelect;
        switch (abilityState)
        {
            case AbilityState.Ready:
                abilityState = AbilityState.Active;
                DisplayAttackRange(Color.green, abilityType);
                break;

            case AbilityState.Active:
                abilityState = AbilityState.Ready;
                DisplayAttackRange(Color.white, abilityType);
                break;
        }

    }

    IEnumerator waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }

    void HandleCooldown()
    {
        Select();
        player.abilityElement = Player.AbilityElement.Default;
        abilityState = AbilityState.Cooldown;
        currentCooldown = player.currentTurn + cooldown;
        State.Instance.IsPlayerTurn = false;
        tilemap.color = Color.white;
    }

    float ManhattanDistance(Vector3 a, Vector3 b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    Tuple<int, DiceManager.DiceState> CalculateDamage(DiceManager.DiceState diceState)
    {
        int diceDamage = damage;

        if (abilityType == AbilityType.Splash && player.abilityElement == Player.AbilityElement.Fire)
        {
            diceDamage += 5;
        }

        switch (diceState)
        {
            case DiceManager.DiceState.Crit:
                diceDamage *= 2;
                break;

            case DiceManager.DiceState.Miss:
                diceDamage = 0;
                break;
        }
        return Tuple.Create(diceDamage, diceState);
    }

    Tuple<int, DiceManager.DiceState> ThrowDice()
    {
        int random = UnityEngine.Random.Range(0, 5);
        DiceManager.DiceState diceState = DiceManager.DiceState.Hit;
        switch (player.diceElement)
        {
            case Player.DiceElement.Default:
                diceState = diceManager.defaultDice[random];
                break;
            case Player.DiceElement.Fire:
                diceState = diceManager.fireDice[random];
                break;
            case Player.DiceElement.Electro:
                diceState = diceManager.electroDice[random];
                break;
            case Player.DiceElement.Wind:
                diceState = diceManager.windDice[random];
                break;
        }
        return CalculateDamage(diceState);
    }


    void SplashAttack()
    {
        foreach (Enemy enemy in enemies)
        {
            float distance = ManhattanDistance(player.transform.position, enemy.transform.position);

            if (distance <= range + 0.1)
            {
                enemy.TakeDamage(ThrowDice());
            }
        }
    }

    IEnumerator ElectroSplash(float time)
    {
        
        yield return new WaitForSeconds(time);
        Select();
        yield return new WaitForSeconds(time);
        SplashAttack();
        HandleCooldown();
        boxCollider.enabled = true;
    }

    IEnumerator WindSplash(float time)
    {
        yield return new WaitForSeconds(time);
        HandleCooldown();
    }

    IEnumerator FireDash(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject fireDash in fireDashEnemies)
        {
            if (fireDash.tag == "Enemy")
            {
                Enemy enemy = fireDash.GetComponent<Enemy>();
                enemy.TakeDamage(ThrowDice());
            }
        }
        fireDashEnemies.Clear();
        abilityCollider.enabled = false;
        boxCollider.enabled = true;
    }

    void Activate(GameObject gameObj, Vector2 mouseHit)
    {
        Vector2 cell = new Vector2();
        if (State.Instance.IsPlayerTurn)
        {
            switch (abilityType)
            {
                case AbilityType.Melee:
                    float distance = ManhattanDistance(player.transform.position, gameObj.transform.position);
                    Debug.Log(distance);

                    if (gameObj.tag == "Enemy" && distance <= range + 0.1)
                    {
                        HandleCooldown();
                        Enemy enemy = gameObj.GetComponent<Enemy>();
                        enemy.TakeDamage(ThrowDice());
                    }
                    break;

                case AbilityType.Splash:
                    if (player.abilityElement == Player.AbilityElement.Default)
                    {
                        if (gameObj.tag == "Player" || gameObj.tag == "Tile")
                        {
                            SplashAttack();
                            HandleCooldown();
                        }
                    }

                    if (player.abilityElement == Player.AbilityElement.Electro)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if (gameObj.tag == "Tile" && distance <= range + 0.1)
                        {
                            Select();
                            boxCollider.enabled = false;
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 10;
                            playerMove.targetPos = cell;
                        }
                        StartCoroutine(ElectroSplash(0.5f));
                    }

                    if (player.abilityElement == Player.AbilityElement.Wind)
                    {
                        foreach (Enemy enemy in enemies)
                        {
                            distance = ManhattanDistance(player.transform.position, enemy.transform.position);

                            if (distance <= 1 + 0.1)
                            {
                                float force = 6f;
                                float offsetX = Mathf.Abs(enemy.transform.position.x - player.transform.position.x);
                                float offsetY = Mathf.Abs(enemy.transform.position.y - player.transform.position.y);
                                float totalForce = force / (offsetX + offsetY + 1f);
                                Vector3 direction = (enemy.transform.position - player.transform.position).normalized;
                                Vector3 newCoords = enemy.transform.position + direction * totalForce;
                                enemy.speed = 10;
                                enemy.lastPos = enemy.transform.position;
                                enemy.targetPos = newCoords;
                                StartCoroutine(WindSplash(1f));
                            }
                        }
                    }
                    break;

                case AbilityType.Dash:
                    if (player.abilityElement == Player.AbilityElement.Default)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if (gameObj.tag == "Tile" && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 10;
                            playerMove.targetPos = cell;
                        }
                    }
                    if (player.abilityElement == Player.AbilityElement.Fire)
                    {
                        boxCollider.enabled = false;
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if (gameObj.tag == "Tile" && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            abilityCollider.enabled = true;
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 10;
                            playerMove.targetPos = cell;
                            StartCoroutine(FireDash(0.5f));
                        }
                        
                    }

                    break;
            }

        }
    }


    void Update()
    {

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Vector2 mouseHit = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (abilityState == AbilityState.Active)
            {
                {
                    if (hit.transform != null)
                    {
                        Activate(hit.transform.gameObject, mouseHit);
                        Debug.Log(hit.transform.gameObject);
                    }
                    else
                    {
                        Activate(tilemap.transform.gameObject, mouseHit);
                        Debug.Log("Tilemap");
                    }
                }
            }
        }

        if (abilityState == AbilityState.Cooldown && player.currentTurn == currentCooldown)
        {
            abilityState = AbilityState.Ready;
        }
    }
}
