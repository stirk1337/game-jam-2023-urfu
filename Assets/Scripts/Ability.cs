using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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

    enum AbilityElement
    {
        Default,
        Fire,
        Electro,
        Wind
    }

    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] int cooldown;
    [SerializeField] int currentCooldown;
    [SerializeField] AbilityType abilityType;
    [SerializeField] public AbilityState abilityState;
    [SerializeField] AbilityElement abilityElement;
    [SerializeField] Player player;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] AbilitiesUIManager abilitiesUIManager;
    [SerializeField] GameObject buttonGameObject;
    List<Enemy> enemies;

    Tilemap tilemap;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        enemies = FindObjectOfType<EnemiesManager>().enemies;
    }

    List<Vector2> FindAttackRangeDiamondShape(Vector2 playerPosition, int attackRange)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        for (int i = (int)playerPosition.x - attackRange; i <= (int)playerPosition.x + attackRange; i++)
        {
            for (int j = (int)playerPosition.y - attackRange; j <= (int)playerPosition.y + attackRange; j++)
            {
                if (Mathf.Abs(i - playerPosition.x) + Mathf.Abs(j - playerPosition.y) <= attackRange)
                {
                    attackPoints.Add(new Vector2(i, j));
                }
            }
        }
        return attackPoints;
    }

    List<Vector2> FindAttackRangeForwardShape(Vector2 playerPosition, float attackRange)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        for (float i = playerPosition.x - attackRange; i <= playerPosition.x + attackRange; i += 0.5f)
        {
            for (float j = playerPosition.y - attackRange; j <= playerPosition.y + attackRange; j += 0.5f)
            {
                if (new Vector2(i, j) != playerPosition && (i == playerPosition.x || j == playerPosition.y))
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
        if (abilityType == AbilityType.Melee || abilityType == AbilityType.Splash)
        {
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), range);
        }
        else if (abilityType == AbilityType.Dash)
        {
            attackPoints = FindAttackRangeForwardShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), range);
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

    void HandleCooldown()
    {
        Select();
        abilityState = AbilityState.Cooldown;
        currentCooldown = player.currentTurn + cooldown;
        State.Instance.IsPlayerTurn = false;
    }

    float ManhattanDistance(Vector3 a, Vector3 b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    void Activate(GameObject gameObj, Vector2 mouseHit)
    {
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
                        enemy.TakeDamage(damage);
                    }
                    break;

                case AbilityType.Splash:
                    if (gameObj.tag == "Player")
                    {
                        HandleCooldown();
                        foreach (Enemy enemy in enemies)
                        {
                            distance = ManhattanDistance(player.transform.position, enemy.transform.position);
                            Debug.Log("Splash distance: " + distance.ToString() + " to: " + enemy.transform.name);
                            if (distance <= range + 0.1)
                            {
                                enemy.TakeDamage(damage);
                            }
                        }
                    }
                    break;

                case AbilityType.Dash:
                    Vector2 cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                    distance = ManhattanDistance(player.transform.position, cell);
                    if (gameObj.tag == "Tile" && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                    {
                        HandleCooldown();
                        playerMove.lastPos = player.transform.position;
                        playerMove.speed = 10;
                        playerMove.targetPos = cell;
                    }

                    break;
            }
        
    }
}


// Update is called once per frame
void Update()
{

    if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Vector2 mouseHit = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (hit.transform != null)
        {
            Debug.Log("Selected object: " + hit.transform.name);
            if (abilityState == AbilityState.Active)
            {
                Debug.Log(hit.transform.gameObject);
                Activate(hit.transform.gameObject, mouseHit);
            }
        }

    }

    if (abilityState == AbilityState.Cooldown && player.currentTurn == currentCooldown)
    {
        abilityState = AbilityState.Ready;
    }
}
}
