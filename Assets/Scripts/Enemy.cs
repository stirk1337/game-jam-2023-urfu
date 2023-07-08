using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Ability;

public class Enemy : MonoBehaviour
{
    public enum ElementState
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
    [SerializeField] public int shield;
    [SerializeField] public int damage;
    [SerializeField] public int range;
    [SerializeField] public int MoveSpeed;
    [SerializeField] float speedField;
    [SerializeField] public float speed;
    [SerializeField] public bool IsDead;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] public Dictionary<ElementState, int> elementState;
    [SerializeField] public float distanceToPlayer;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject fireImage;
    [SerializeField] GameObject electroImage;
    [SerializeField] GameObject windImage;
    [SerializeField] Player.AbilityElement diceElement;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    Player player;
    DiceManager diceManager;

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

    void UpdateVisual()
    {
        healthText.text = health.ToString();
        var stateImages = new Dictionary<ElementState, GameObject>()
        {
            [ElementState.Fire] = fireImage,
            [ElementState.Electro] = electroImage,
            [ElementState.Wind] = windImage,
        };
        foreach (var state in elementState)
        {
            if (state.Key == ElementState.Default)
                continue;
            if (state.Value > 0)
            {
                stateImages[state.Key].SetActive(true);
            }
            else
            {
                stateImages[state.Key].SetActive(false);
            }
        }
    }

    public void Init()
    {
        player = FindObjectOfType<Player>();
        diceManager = FindAnyObjectByType<DiceManager>();
        elementState = new Dictionary<ElementState, int>()
        {
            [ElementState.Electro] = 0,
            [ElementState.Fire] = 0,
            [ElementState.Wind] = 0,
            [ElementState.Default] = 0,
        };
        lastPos = transform.position;
        targetPos = transform.position;
        IsDead = false;
        speed = speedField;
        switch (enemyType)
        {
            case EnemyType.Sword:
                
                break;
        }
    }

    bool inRange(Transform target)
    {
        float distance = Vector2.Distance(gameObject.transform.position, target.position);
        //Debug.Log(distance);
        return distance <= range + 0.1;
    }

    void Attack(Transform target)
    {
        Player player = target.gameObject.GetComponent<Player>();
        animator.SetTrigger("Attack");
        player.TakeDamageWithCube(ThrowDice());
    }

    Tuple<int, DiceManager.DiceState> CalculateDamage(DiceManager.DiceState diceState)
    {
        int diceDamage = damage;

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
        switch (diceElement)
        {
            case Player.AbilityElement.Default:
                diceState = diceManager.defaultDice[random];
                break;
            case Player.AbilityElement.Fire:
                diceState = diceManager.fireDice[random];
                break;
            case Player.AbilityElement.Electro:
                diceState = diceManager.electroDice[random];
                break;
            case Player.AbilityElement.Wind:
                diceState = diceManager.windDice[random];
                break;
        }
        return CalculateDamage(diceState);
    }


    public void Turn(Transform target)
    {
        speed = speedField;
        switch (enemyType)
        {
            case EnemyType.Sword:
                if (inRange(target))
                {
                    Attack(target);
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
        foreach (var key in elementState.Keys.ToList())
        {
            if (elementState[key] > 0)
                elementState[key] = elementState[key] - 1;
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
            if (x == -1)
                spriteRenderer.flipX = true;
            if (x == 1)
                spriteRenderer.flipX = false;
            //spriteRenderer.flipX = true;
            return false;
        }
        return true;
    }

    public void Move(Transform target)
    {
        float moveX = target.position.x - transform.position.x;
        float moveY = target.position.y - transform.position.y;
        bool cantMove = false;


        Debug.Log(moveX.ToString() + "  " + moveY.ToString());
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
        if (Mathf.Abs(moveX) < Mathf.Abs(moveY) || cantMove)
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
        if (Math.Abs(moveX) == Math.Abs(moveY) || cantMove)
        {
            Debug.Log("xd");
            char path = "xy"[UnityEngine.Random.Range(0, 1)];
            if (path == 'x')
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
            if (path == 'y' || cantMove)
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
    }

    public void TakeDamageWithoutCube(int dmg)
    {
        health -= dmg;
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
                elementState[ElementState.Electro] = 5;
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

    float ManhattanDistance(Vector3 a, Vector3 b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    void Update()
    {
        UpdateVisual();
        foreach(var state in elementState)
        {
            if (state.Value != 0)
                Debug.Log(state.Key.ToString() + state.Value.ToString());
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        distanceToPlayer = ManhattanDistance(player.transform.position, transform.position);
    }
}
