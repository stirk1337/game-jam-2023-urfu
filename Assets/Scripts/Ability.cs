using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    Tilemap tilemap;

    void Start()
    { 
        tilemap = FindObjectOfType<Tilemap>();
    }

    List<Vector2> FindAttackRange(Vector2 playerPosition, int attackRange)
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

    public void Select()
    {
        if (!State.Instance.IsPlayerTurn)
            return;

            playerMove.InSelect = !playerMove.InSelect;
        switch (abilityState)
        {
            case AbilityState.Ready:
                abilityState = AbilityState.Active;
                List<Vector2> attackPoints = FindAttackRange(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), range);
                foreach (Vector2 point in attackPoints)
                {
                    //Debug.Log(point);
                    Vector3Int cellPos = new Vector3Int((int)point.x, (int)point.y, 0);
                    tilemap.SetTileFlags(cellPos, TileFlags.None);
                    tilemap.SetColor(cellPos, Color.green);
                }

                break;
            case AbilityState.Active:
                abilityState = AbilityState.Ready;
                attackPoints = FindAttackRange(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), range);
                foreach (Vector2 point in attackPoints)
                {
                    //Debug.Log(point);
                    Vector3Int cellPos = new Vector3Int((int)point.x, (int)point.y, 0);
                    tilemap.SetTileFlags(cellPos, TileFlags.None);
                    tilemap.SetColor(cellPos, Color.white);
                }

                break;
        }
        
    }

    void Activate(GameObject gameObj)
    {
        if (State.Instance.IsPlayerTurn)
        {
            switch (abilityType)
            {
                case AbilityType.Melee:
                    float distance = Vector2.Distance(player.transform.position, gameObj.transform.position);
                    Debug.Log(distance);
                    if (gameObj.tag == "Enemy" && distance <= range + 0.1)
                    {
                        Select();
                        State.Instance.IsPlayerTurn = false;
                        abilityState = AbilityState.Cooldown;
                        currentCooldown = player.currentTurn + cooldown;
                        Enemy enemy = gameObj.GetComponent<Enemy>();
                        enemy.health -= damage;
                        if (enemy.health <= 0)
                        {
                            enemy.Die();
                        }
                    }
                    break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log("Trying to access...");
            if (hit.transform != null)
            {
                Debug.Log("Selected object: " + hit.transform.name);
                if (abilityState == AbilityState.Active)
                {               
                    Activate(hit.transform.gameObject);         
                }
            }

        }

        if (abilityState == AbilityState.Cooldown && player.currentTurn == currentCooldown)
        {
            abilityState = AbilityState.Ready;
        }
    }
}
