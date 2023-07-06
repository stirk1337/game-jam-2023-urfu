using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Ability;

public class Enemy : MonoBehaviour
{
    enum EnemyState
    {
        Default,
        Fire,
        Electro,
        Wind
    }

    enum EnemyType
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
    [SerializeField] int MoveSpeed;
    [SerializeField] float moveVelocity;
    [SerializeField] public bool IsDead;
    [SerializeField] EnemyState enemyState;
    [SerializeField] EnemyType enemyType;
    Vector2 lastPos;
    Vector2 targetPos;

    void Start()
    {
        lastPos = transform.position;
        targetPos = transform.position;
        IsDead = false;
    }

    bool inRange(Transform target)
    {
        float distance = Vector2.Distance(gameObject.transform.position, target.position);
        Debug.Log(distance);
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
            Debug.Log("Selected object: " + hit.transform.name + " From: " + gameObject.transform.name);
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

    public void TakeDamage(int damage)
    {
        health -= damage;
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
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveVelocity);
    }
}
