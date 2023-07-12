using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Ability;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] public int abilityDamage;
    [SerializeField] public int range;
    [SerializeField] public int MoveSpeed;
    [SerializeField] float speedField;
    [SerializeField] public float speed;
    [SerializeField] Vector2 fireKnightTarget;
    [SerializeField] public bool IsDead;
    [SerializeField] public EnemyType enemyType;
    [SerializeField] public Dictionary<ElementState, int> elementState;
    [SerializeField] public float distanceToPlayer;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject fireImage;
    [SerializeField] GameObject electroImage;
    [SerializeField] GameObject windImage;
    [SerializeField] Player.AbilityElement diceElement;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int isCharging;
    [SerializeField] public int currentAbilityCooldown;
    [SerializeField] int abilityCooldown;
    [SerializeField] public ElementState resist;

    Player player;
    DiceManager diceManager;
    List<Vector2> electroMageAttackPoints;
    ExpRuneManager expRuneManager;
    EnemiesManager enemiesManager;
    BoxCollider2D boxCollider;

    public Vector2 lastPos;
    public Vector2 targetPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            
            if (speed >= 10)
            {
                if (Mathf.Abs(lastPos.x - targetPos.x) < Mathf.Abs(lastPos.y - targetPos.y))
                {
                    Debug.Log("enemy collision");
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
                //Debug.Log(transform.position);
                targetPos = new Vector2(MathF.Floor(transform.position.x) + 0.5f, MathF.Floor(transform.position.y) + 0.5f);
            }
        }

        if (collision.gameObject.tag == "Player")
        {
            targetPos = new Vector2(MathF.Floor(transform.position.x) + 0.5f, MathF.Floor(transform.position.y) + 0.5f);
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
        boxCollider = GetComponent<BoxCollider2D>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        diceManager = FindObjectOfType<DiceManager>();
        expRuneManager = FindObjectOfType<ExpRuneManager>();
        electroMageAttackPoints = new List<Vector2>();
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
    }

    bool inRange(Transform target, int range)
    {
        float distance = ManhattanDistance(gameObject.transform.position, target.position);
        //Debug.Log(distance);
        return distance <= range + 0.1;
    }

    bool inRangeForward(Transform target, int range)
    {
        float distance = ManhattanDistance(gameObject.transform.position, target.position);
        //Debug.Log(distance);
        return distance <= range + 0.1 && !(gameObject.transform.position.x != target.position.x && gameObject.transform.position.y != target.position.y);
    }

    bool inRangeStrogo(Transform target, int range)
    {
        float distance = ManhattanDistance(gameObject.transform.position, target.position);
        //Debug.Log(distance);
        return distance == range;
    }

    void Attack(Transform target)
    {
        Player player = target.gameObject.GetComponent<Player>();
        animator.SetTrigger("Attack");
        player.TakeDamageWithCube(ThrowDice());
    }

    void AbilityAttack(Transform target, ElementState elementState)
    {
        Player player = target.gameObject.GetComponent<Player>();
        player.state[elementState] = 3;
        animator.SetTrigger("Attack");
        player.TakeDamageWithoutCube(abilityDamage);
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


    IEnumerator KnightTurn(Transform target, float time)
    {
        if (inRange(target, range))
        {
            Attack(target);
        }
        else
        {
            Move(target);
            yield return new WaitForSeconds(time);
            if (inRange(target, range))
            {
                Attack(target);
            }
            else
            {
                Move(target);
                yield return new WaitForSeconds(time);
                if (inRange(target, range))
                {
                    Attack(target);
                }
            }
        }
    }

    IEnumerator KnightTurnElectroState(Transform target, float time)
    {
        Move(target);
        yield return new WaitForSeconds(time);
        Move(target);
        yield return new WaitForSeconds(time);

    }

    public void Turn(Transform target)
    {
        speed = speedField;
        if (elementState[ElementState.Fire] > 0)
        {
            health -= 5;
            if (health <= 0)
            {
                Die();
                return;
            }
        }

        if (elementState[ElementState.Electro] > 0)
        {
            if (enemyType == EnemyType.Knight)
            {
                KnightTurnElectroState(target, 0.45f);
            }
            else
            {
                Move(target);
            }
            return;
        }

        switch (enemyType)
        {
            case EnemyType.Sword:
                if (inRange(target, range))
                {
                    Attack(target);
                }
                else
                {
                    Move(target);
                }

                break;

            case EnemyType.Mage:

                if (currentAbilityCooldown <= 0)
                {
                    if (isCharging == 1)
                    {
                        // animator.SetTrigger("AttackAbility");
                        currentAbilityCooldown = abilityCooldown;
                        if (inRangeStrogo(target, 3))
                        {
                            AbilityAttack(target, ElementState.Default);
                            
                        }
                        isCharging = 0;
                        animator.SetBool("Charging", false);
                    }
                    else if (inRange(target, range))
                    {
                        Attack(target);
                        isCharging = 0;
                    }
                    else if (inRangeStrogo(target, 3))
                    {
                        isCharging = 1;
                        animator.SetBool("Charging", true);
                    }
                    else
                    {
                        Move(target);
                        isCharging = 0;
                    }
                }
                else
                {
                    if (inRange(target, range))
                    {
                        Attack(target);
                    }
                    else
                    {
                        Move(target);
                    }
                }

                break;

            case EnemyType.Knight:
                StartCoroutine(KnightTurn(target, 0.45f));
                break;

            case EnemyType.FireKnight:
                if (currentAbilityCooldown <= 0)
                {
                    if (isCharging == 1)
                    {
                        // animator.SetTrigger("AttackAbility");
                        lastPos = targetPos;
                        targetPos = fireKnightTarget;
                        currentAbilityCooldown = abilityCooldown;
                        if (inRangeForward(target, 3))
                        {

                            AbilityAttack(target, ElementState.Fire);
                        }
                        isCharging = 0;
                    }
                    else if (inRange(target, range))
                    {
                        Attack(target);
                        isCharging = 0;
                    }
                    else if (inRangeForward(target, 3))
                    {
                        isCharging = 1;
                        fireKnightTarget = target.position;
                        //animator.SetTrigger("ChargeAbility");
                    }
                    else
                    {
                        Move(target);
                        isCharging = 0;
                    }
                }
                else
                {
                    if (inRange(target, range))
                    {
                        Attack(target);
                    }
                    else
                    {
                        Move(target);
                    }
                }
                break;

            case EnemyType.ElectroMage:
                if (currentAbilityCooldown <= 0)
                {
                    if (isCharging == 2)
                    {
                        // animator.SetTrigger("AttackAbility");
                        if (electroMageAttackPoints.Contains(player.transform.position))
                        {
                            AbilityAttack(target, ElementState.Electro);
                        }
                        isCharging = 0;
                        currentAbilityCooldown = abilityCooldown;
                    }
                    else if (isCharging == 1)
                        isCharging = 2;
                    else if (isCharging == 0)
                    {
                        if (inRange(target, range))
                        {
                            Attack(target);
                            isCharging = 0;
                        }
                        else if (inRangeStrogo(target, 5))
                        {
                            electroMageAttackPoints = new List<Vector2>()
                            {
                                new Vector2(player.transform.position.x, player.transform.position.y),
                                new Vector2(player.transform.position.x + 1, player.transform.position.y),
                                new Vector2(player.transform.position.x + 1, player.transform.position.y - 1),
                                new Vector2(player.transform.position.x, player.transform.position.y - 1),
                                new Vector2(player.transform.position.x - 1, player.transform.position.y - 1),
                                new Vector2(player.transform.position.x - 1, player.transform.position.y),
                                new Vector2(player.transform.position.x - 1, player.transform.position.y - 1),
                                new Vector2(player.transform.position.x, player.transform.position.y + 1),
                                new Vector2(player.transform.position.x + 1, player.transform.position.y + 1),
                            };
                            isCharging = 1;
                            // animator.SetTrigger("CharingAbility");
                        }
                        else
                        {
                            Move(target);
                            isCharging = 0;
                        }
                    }
                }
                else
                {
                    if (inRange(target, range))
                    {
                        Attack(target);
                    }
                    else
                    {
                        Move(target);
                    }
                }


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
            animator.SetTrigger("Run");
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
        if (elementState[ElementState.Wind] > 0)
        {
            return;
        }

        float moveX = target.position.x - transform.position.x;
        float moveY = target.position.y - transform.position.y;
        bool cantMove = false;


        //Debug.Log(moveX.ToString() + "  " + moveY.ToString());
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
            //Debug.Log("xd");
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


    public void TakeDamageWithoutCube(int dmg, ElementState element, Player.AbilityElement abilityElement)
    {

        Debug.Log((ElementState)abilityElement);
        if ((ElementState)abilityElement == resist && abilityElement != Player.AbilityElement.Default)
        {
            dmg /= 2;

        }
        if (element != resist)
        {
            elementState[element] = 3;
        }

        health -= Mathf.Clamp(dmg, 0, 100000);

        animator.SetTrigger("Damage");
        if (health <= 0)
            Die();
    }

    public void TakeDamageWithCube(Tuple<int, DiceManager.DiceState> tuple)
    {

        //Debug.Log(tuple.Item1 + tuple.Item2.ToString());
        switch (tuple.Item2)
        {
            case DiceManager.DiceState.Electro:
                elementState[ElementState.Electro] = 5;
                break;
            case DiceManager.DiceState.Wind:
                State.Instance.FreeMove = true;
                break;
        }

        if (tuple.Item1 == 0)
            return;

        animator.SetTrigger("Damage");
        health -= Mathf.Clamp(tuple.Item1 - shield, 0, 100000);
        if (health <= 0)
            Die();
    }

    void Die()
    {
        //Destroy(gameObject);
        //SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        //sprite.enabled = false;
        expRuneManager.Spawn(transform);
        IsDead = true;
        enemiesManager.DeleteEnemies();

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
        if (player != null)
        {
            UpdateVisual();
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            distanceToPlayer = ManhattanDistance(player.transform.position, transform.position);
        }


        if (player.playerMove.InSelect)
        {
            //Debug.Log("xd");
            boxCollider.size = new Vector2(1, 1);
        }
        else
        {
            boxCollider.size = new Vector2(0.7f, 0.7f);
        }
    }
}
