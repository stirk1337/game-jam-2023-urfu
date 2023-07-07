using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Default,
        Fire,
        Electro,
        Wind
    }

    public enum EnemyType
    {
        Sword,
        Mage,
        Knight,
        FireKnight,
        ElectroMage
    }

    [SerializeField] public int health;
    [SerializeField] int damage;
    [SerializeField] int range;
    [SerializeField] float speedField;
    [SerializeField] public float speed;
    [SerializeField] public bool IsDead;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int swordDamage;
    [SerializeField] int swordHealth;
    [SerializeField] int swordRange;
    [SerializeField] int swordMoveSpeed;
    [SerializeField] public Dictionary<EnemyState, int> enemyState;

    public Vector2 lastPos;
    public Vector2 targetPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("enemy collision");
            if (speed >= 10)
            {
                if (Mathf.Abs(lastPos.x - targetPos.x) < Mathf.Abs(lastPos.y - targetPos.y))
                {
                    int offset = targetPos.y - lastPos.y > 0 ? 1 : -1;
                    Enemy collisionEnemy = collision.gameObject.GetComponent<Enemy>();
                    collisionEnemy.lastPos = collisionEnemy.transform.position;
                    collisionEnemy.targetPos = new Vector2(targetPos.x, targetPos.y + offset);
                    collisionEnemy.speed = speed + 10;
                }
                else
                {
                    int offset = targetPos.x - lastPos.x > 0 ? 1 : -1;
                    Enemy collisionEnemy = collision.gameObject.GetComponent<Enemy>();
                    collisionEnemy.lastPos = collisionEnemy.transform.position;
                    collisionEnemy.targetPos = new Vector2(targetPos.x + offset, targetPos.y);
                    collisionEnemy.speed = speed + 10;
                }
            }
            else
            {
                targetPos = new Vector2(MathF.Floor(transform.position.x) + 0.5f, MathF.Floor(transform.position.y) + 0.5f);
            }
        }
    }

    public void Init()
    {
        enemyState = new Dictionary<EnemyState, int>()
        {
            [EnemyState.Electro] = 0,
            [EnemyState.Fire] = 0,
            [EnemyState.Wind] = 0,
            [EnemyState.Default] = 0,
        };
        lastPos = transform.position;
        targetPos = transform.position;
        IsDead = false;
        speed = speedField;
        switch (enemyType)
        {
            case EnemyType.Sword:
                damage = swordDamage;
                health = swordHealth;
                range = swordRange;
                break;
        }
    }

    bool inRange(Transform target)
    {
        float distance = Vector2.Distance(gameObject.transform.position, target.position);
        //Debug.Log(distance);
        return distance <= range + 0.1;
    }

    void AutoAttack(Transform target)
    {
        Player player = target.gameObject.GetComponent<Player>();
        player.health -= damage;
        if (player.health <= 0)
        {
            player.Die();
        }
    }

    public void Turn(Transform target)
    {
        speed = speedField;
        switch (enemyType)
        {
            case EnemyType.Sword:
                if (inRange(target))
                {
                    AutoAttack(target);
                }
                else
                {
                    Move(target);
                }
                
                break;

            case EnemyType.Mage:
                break;

            case EnemyType.Knight:
                break;

            case EnemyType.FireKnight:
                break;

            case EnemyType.ElectroMage:
                break;
        }
    }

    bool IsCollision(Vector2 target)
    {
        RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero);
        if (hit.transform != null && hit.transform.gameObject.tag == "Enemy")
        {
            //Debug.Log("Selected object: " + hit.transform.name + " From: " + gameObject.transform.name);
            return true;
        }
        return false;
        
    }

    bool TryToMove(float x, float y)
    {
        lastPos = transform.position;
        Vector2 moveTo = new Vector2(transform.position.x + x, transform.position.y + y);
        if (!IsCollision(moveTo))
        {
            targetPos = moveTo;
            return false;
        }
        return true;
    }

    public void Move(Transform target)
    {
        float moveX = target.position.x - transform.position.x;
        float moveY = target.position.y - transform.position.y;
        bool cantMove = false;
 
        
        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
        {
            if (moveX > 0)
            {
                cantMove = TryToMove(1, 0);
            }
            else
            {
                cantMove = TryToMove(-1, 0);
            }
        }
        if (Mathf.Abs(moveX) <= Mathf.Abs(moveY) || cantMove)
        {
            if (moveY > 0)
            {
                cantMove = TryToMove(0, 1);
            }
            else
            {
                cantMove = TryToMove(0, -1);
            }
        }
    }

    public void TakeDamageWithoutCube()
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    public void TakeDamageWithCube(Tuple<int, DiceManager.DiceState> tuple)
    {
        health -= tuple.Item1;
        Debug.Log(tuple.Item1 + tuple.Item2.ToString());
        switch (tuple.Item2)
        {
            case DiceManager.DiceState.Electro:
                enemyState[EnemyState.Electro] = 5;
                break;
            case DiceManager.DiceState.Wind:
                State.Instance.FreeMove = true;          
                break;
        }
        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
        IsDead = true;
    }

    void Update()
    {
        foreach(var state in enemyState)
        {
            if (state.Value != 0)
                Debug.Log(state.Key.ToString() + state.Value.ToString());
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }
}
