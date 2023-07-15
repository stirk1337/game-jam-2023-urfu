using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class Ability : MonoBehaviour
{
    enum AbilityType
    {
        Melee,
        Splash,
        Dash,
        Range,
        Skip
    }
    public enum AbilityState
    {
        Ready,
        Active,
        Cooldown
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DashEnemies.Add(collision.gameObject);
    }

    [SerializeField] public int damage;
    [SerializeField] public int range;
    [SerializeField] public int cooldown;
    [SerializeField] public int currentCooldown;
    [SerializeField] AbilityType abilityType;
    [SerializeField] public AbilityState abilityState;
    [SerializeField] Player player;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] AbilitiesUIManager abilitiesUIManager;
    [SerializeField] GameObject buttonGameObject;
    [SerializeField] DiceManager diceManager;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] BoxCollider2D abilityCollider;
    [SerializeField] int abilityStateCooldown;
    [SerializeField] Animator animator;
    [SerializeField] public TextMeshProUGUI cooldownText;
    [SerializeField] public GameObject buttonCanvas;
    [SerializeField] public GameObject cooldownImage;
    [SerializeField] public Sprite DefaultImage;
    [SerializeField] public Sprite ElectroImage;
    [SerializeField] public Sprite FireImage;
    [SerializeField] public Sprite WindImage;
    [SerializeField] public GameObject showDamagePrefab;

    [SerializeField] AudioSource abilitySound;


    EnemiesManager enemiesManager;
    List<GameObject> DashEnemies;
   
    Tilemap tilemap;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        DashEnemies = new List<GameObject>();
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



    void DisplayAttackRange(UnityEngine.Color color, UnityEngine.Color middleColor, AbilityType abilityType)
    {
        List<Vector2> attackPoints = new List<Vector2>();
        int currentRange = range;
        List<Vector2> middlePoints = new List<Vector2>() { new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f) };
        if (abilityType == AbilityType.Melee)
        {
            currentRange = player.range;
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

        if (abilityType == AbilityType.Dash && player.abilityElement == Player.AbilityElement.Electro)
        {
            currentRange = range + 2;
            attackPoints = FindAttackRangeForwardShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Dash && player.abilityElement == Player.AbilityElement.Wind)
        {
            currentRange = range;
            attackPoints = FindAttackRangeForwardShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }

        if (abilityType == AbilityType.Range && (player.abilityElement == Player.AbilityElement.Default || player.abilityElement == Player.AbilityElement.Fire || player.abilityElement == Player.AbilityElement.Electro))
        {
            currentRange = range;
            middlePoints = new List<Vector2>()
            {
                new Vector2(playerMove.targetPos.x - 0.5f + range, playerMove.targetPos.y - 0.5f),
                new Vector2(playerMove.targetPos.x - 0.5f - range, playerMove.targetPos.y - 0.5f),
                new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f + range),
                new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f - range),

            };
            foreach (Vector2 point in middlePoints)
            {
                attackPoints.AddRange(FindAttackRangeSquareShape(point, range - 2));
            }
        }

        if (abilityType == AbilityType.Range && player.abilityElement == Player.AbilityElement.Wind)
        {
            currentRange = range + 2;
            attackPoints = FindAttackRangeDiamondShape(new Vector2(playerMove.targetPos.x - 0.5f, playerMove.targetPos.y - 0.5f), currentRange);
        }


        foreach (Vector2 point in attackPoints)
        {
            Vector3Int cellPos = new Vector3Int((int)point.x, (int)point.y, 0);
            tilemap.SetTileFlags(cellPos, TileFlags.None);
            tilemap.SetColor(cellPos, color);
        }
        
        foreach (Vector2 point in middlePoints)
        {
            Vector3Int cellPos = new Vector3Int((int)point.x, (int)point.y, 0);
            tilemap.SetTileFlags(cellPos, TileFlags.None);
            tilemap.SetColor(cellPos, middleColor);
        }
    }

    IEnumerator InSelectTurner(float time)
    {
        yield return new WaitForSeconds(time);
        playerMove.InSelect = !playerMove.InSelect;
    }

    public void Select()
    {
        if (!State.Instance.IsPlayerTurn)
            return;

        playerMove.startTouchPostitionX = 0f;
        playerMove.startTouchPostitionY = 0f;
        playerMove.endTouchPositionX = 0f;
        playerMove.endTouchPositionY = 0f;


        abilitiesUIManager.TurnInteractable(buttonGameObject);
        StartCoroutine(InSelectTurner(0.1f));
        switch (abilityState)
        {
            case AbilityState.Ready:
                abilityState = AbilityState.Active;
                DisplayAttackRange(UnityEngine.Color.green, UnityEngine.Color.yellow, abilityType);
                break;

            case AbilityState.Active:
                abilityState = AbilityState.Ready;
                DisplayAttackRange(UnityEngine.Color.white, UnityEngine.Color.white, abilityType);
                break;
        }

    }

    public void SkipTurn()
    {
        if (State.Instance.IsPlayerTurn)
        {
            NextTurn();
            player.shield += 5;
            player.needToDeleteShieldFromZZZ = true;
        }
    }

    public void NextTurn()
    {
        State.Instance.IsPlayerTurn = false;
        abilitySound.Play();
        tilemap.color = UnityEngine.Color.white;
    }

    void HandleCooldown()
    {
        Select();
        abilitySound.Play();
        player.abilityElement = Player.AbilityElement.Default;
        abilityState = AbilityState.Cooldown;
        currentCooldown = player.currentTurn + cooldown;
        //State.Instance.IsPlayerTurn = false;
        tilemap.color = UnityEngine.Color.white;
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

        if (player.abilityElement == Player.AbilityElement.Wind)
        {
            diceDamage = player.damage;
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
        int random = UnityEngine.Random.Range(0, 6);
        DiceManager.DiceState diceState = DiceManager.DiceState.Hit;
        switch (player.diceElement)
        {
            case Player.AbilityElement.Default:
                diceState = diceManager.playerDice[random];
                if (diceState == DiceManager.DiceState.Miss)
                {
                    if (player.lastAttackWasMiss)
                    {
                        player.lastAttackWasMiss = false;
                        diceState = DiceManager.DiceState.Hit;
                    }
                    else
                    {
                        int random2 = UnityEngine.Random.Range(0, 2);
                        if (random2 == 0)
                        {
                            diceState = DiceManager.DiceState.Hit;
                        }
                        else
                        {
                            player.lastAttackWasMiss = true;
                        }
                    }

                }
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
        if (player.diceElement != Player.AbilityElement.Default)
        {
            player.currentDices[player.diceElement].SetActive(false);
        }
        player.diceElement = Player.AbilityElement.Default;

        var calculatedDamage = CalculateDamage(diceState);
        GameObject canvasDamage = Instantiate(showDamagePrefab, player.transform.position, transform.rotation);
        DamageShowUI damageShowUI = canvasDamage.GetComponent<DamageShowUI>();
        damageShowUI.transform.parent = player.transform;
        damageShowUI.Init(calculatedDamage);
        return calculatedDamage;
    }


    void SplashAttack(int plusDamage, Vector2 position, Enemy.ElementState enemyState)
    {
        int dmg = damage + plusDamage;
        foreach (Enemy enemy in enemiesManager.enemies)
        {

            float distance = ManhattanDistance(position, enemy.transform.position);

            if (distance <= range + 0.1)
            {
                enemy.TakeDamageWithoutCube(dmg, enemyState, player.abilityElement);
            }
        }
    }

    void RangeSplashAttack(Vector2 position, Enemy.ElementState enemyState)
    {
        foreach (Enemy enemy in enemiesManager.enemies)
        {
            if (enemy.IsDead)
                continue;

            float distance = Vector2.Distance(position, enemy.transform.position);
            //Debug.Log(distance);
            if (distance <= range - 2 + 0.1 + 0.5)
            {          
                enemy.TakeDamageWithoutCube(damage, enemyState, player.abilityElement);
            }
        }
    }

    IEnumerator ElectroSplash(float time)
    {
        
        yield return new WaitForSeconds(time);
        Select();
        yield return new WaitForSeconds(time);
        SplashAttack(0, player.transform.position, Enemy.ElementState.Default);
        HandleCooldown();
        foreach (GameObject bu in abilitiesUIManager.buttons)
        {
            var button = bu.GetComponent<UnityEngine.UI.Button>();
            button.interactable = true;
        }
        boxCollider.enabled = true;
    }

    IEnumerator WindSplash(float time)
    {
        yield return new WaitForSeconds(time);
        HandleCooldown();
    }

    IEnumerator FireDash(float time, Enemy.ElementState enemyState)
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject fireDash in DashEnemies)
        {
            if (fireDash.tag == "Enemy")
            {
                Enemy enemy = fireDash.GetComponent<Enemy>();
                enemy.TakeDamageWithoutCube(damage, enemyState, player.abilityElement);
            }
        }
        DashEnemies.Clear();
        abilityCollider.enabled = false;
        boxCollider.enabled = true;
        playerMove.onDash = false;
    }

    IEnumerator StartDash(float time)
    {
        yield return new WaitForSeconds(time);
        playerMove.onDash = false;
    }

    IEnumerator ElectroDash(float time)
    {
        yield return new WaitForSeconds(time);
        boxCollider.enabled = true;
        playerMove.onDash = false;
    }

    IEnumerator WindDash(float time)
    {
        yield return new WaitForSeconds(time);
        foreach (GameObject windDash in DashEnemies)
        {
            if (windDash.tag == "Enemy")
            {
                Enemy enemy = windDash.GetComponent<Enemy>();
                enemy.elementState[Enemy.ElementState.Wind] = abilityStateCooldown;
            }
        }
        abilityCollider.enabled = false;
        abilityCollider.size = new Vector2(0.9f, 0.9f);
        boxCollider.enabled = true;
        playerMove.onDash = false;

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
                    //Debug.Log(distance);
                    damage = player.damage;
                    if (gameObj.tag == "Enemy" && distance <= player.range + 0.1)
                    {
                        Select();
                        animator.SetTrigger("Attack");
                        //if (gameObj.transform.position.x - player.transform.position.x > 0)
                        //{
                        //    if (playerMove.spriteRenderer.flipX)
                        //    {
                        //        playerMove.spriteRenderer.flipX = false;
                        //        //playerMove.spriteRenderer.transform.position = new Vector2(playerMove.spriteRenderer.transform.position.x + 0.32f, playerMove.spriteRenderer.transform.position.y);
                        //    }
                        //}
                        //else
                        //{
                        //    if (!playerMove.spriteRenderer.flipX)
                        //    {
                        //        playerMove.spriteRenderer.flipX = true;
                        //        //playerMove.spriteRenderer.transform.position = new Vector2(playerMove.spriteRenderer.transform.position.x - 0.32f, playerMove.spriteRenderer.transform.position.y);
                        //    }
                        //}
                        tilemap.color = UnityEngine.Color.white;
                        Enemy enemy = gameObj.GetComponent<Enemy>();
                        enemy.TakeDamageWithCube(ThrowDice());
                        NextTurn();
                    } 
                    break;

                case AbilityType.Splash:
                    if (player.abilityElement == Player.AbilityElement.Default)
                    {
                        if (gameObj.tag == "Player" || (gameObj.tag == "Tile" || gameObj.tag == "Rune"))
                        {
                            SplashAttack(0, player.transform.position, Enemy.ElementState.Default);
                            animator.SetTrigger("Splash");
                            HandleCooldown();
                        }
                    }

                    if (player.abilityElement == Player.AbilityElement.Fire)
                    {
                        if (gameObj.tag == "Player" || (gameObj.tag == "Tile" || gameObj.tag == "Rune"))
                        {
                            SplashAttack(10, player.transform.position, Enemy.ElementState.Default);
                            animator.SetTrigger("Splash");
                            HandleCooldown();
                        }
                    }

                    if (player.abilityElement == Player.AbilityElement.Electro)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if ((gameObj.tag == "Tile" || gameObj.tag == "Rune" || gameObj.tag == "Player") && distance <= range + 0.1)
                        {
                            foreach (GameObject bu in abilitiesUIManager.buttons)
                            {
                                var button = bu.GetComponent<UnityEngine.UI.Button>();
                                button.interactable = false;
                            }
                            Select();
                            animator.SetTrigger("Splash");
                            boxCollider.enabled = false;
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 10;
                            playerMove.targetPos = cell;
                        }
                        StartCoroutine(ElectroSplash(0.5f));
                    }

                    if (player.abilityElement == Player.AbilityElement.Wind)
                    {
                        foreach (Enemy enemy in enemiesManager.enemies)
                        {
                            distance = ManhattanDistance(player.transform.position, enemy.transform.position);

                            if (distance <= 1 + 0.1)
                            {
                                float force = 6f;
                                animator.SetTrigger("Splash");
                                float offsetX = Mathf.Abs(enemy.transform.position.x - player.transform.position.x);
                                float offsetY = Mathf.Abs(enemy.transform.position.y - player.transform.position.y);
                                float totalForce = force / (offsetX + offsetY + 1f);
                                Vector3 direction = (enemy.transform.position - player.transform.position).normalized;
                                Vector3 newCoords = enemy.transform.position + direction * totalForce;
                                enemy.speed = 11;
                                enemy.TakeDamageWithoutCube(damage, Enemy.ElementState.Wind, player.abilityElement);
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
                        if ((gameObj.tag == "Tile" || gameObj.tag == "Rune") && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 10;
                            playerMove.onDash = true;
                            animator.SetTrigger("Dash");
                            playerMove.targetPos = cell;
                            StartCoroutine(StartDash(0.5f));
                        }
                    }

                    if (player.abilityElement == Player.AbilityElement.Fire)
                    {
                        boxCollider.enabled = false;
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if ((gameObj.tag == "Tile" || gameObj.tag == "Rune") && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            animator.SetTrigger("Dash");
                            playerMove.onDash = true;
                            abilityCollider.enabled = true;
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 20;
                            playerMove.targetPos = cell;
                            StartCoroutine(FireDash(0.5f, Enemy.ElementState.Default));
                        }
                        
                    }

                    if (player.abilityElement == Player.AbilityElement.Electro)
                    {
                        boxCollider.enabled = false;
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if ((gameObj.tag == "Tile" || gameObj.tag == "Rune") && distance <= range + 2 + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            animator.SetTrigger("Dash");
                            playerMove.lastPos = player.transform.position;
                            playerMove.onDash = true;
                            playerMove.speed = 20;
                            playerMove.targetPos = cell;
                            StartCoroutine(ElectroDash(0.5f));
                            
                        }

                    }

                    if (player.abilityElement == Player.AbilityElement.Wind)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        distance = ManhattanDistance(player.transform.position, cell);
                        if ((gameObj.tag == "Tile" || gameObj.tag == "Rune")  && distance <= range + 0.1 && !(player.transform.position.x != cell.x && player.transform.position.y != cell.y))
                        {
                            HandleCooldown();
                            animator.SetTrigger("Dash");
                            abilityCollider.enabled = true;
                            if (Mathf.Abs(cell.x - player.transform.position.x) > Mathf.Abs(cell.y - player.transform.position.y))
                            {
                                abilityCollider.size = new Vector2(abilityCollider.size.x, abilityCollider.size.y + 0.3f);
                            }
                            else
                            {
                                abilityCollider.size = new Vector2(abilityCollider.size.x + 0.3f, abilityCollider.size.y);
                            }
                            playerMove.lastPos = player.transform.position;
                            playerMove.speed = 20;
                            playerMove.onDash = true;
                            playerMove.targetPos = cell;
                            StartCoroutine(WindDash(0.5f));
                        }

                    }

                    break;
                case AbilityType.Range:
                    if (player.abilityElement == Player.AbilityElement.Default)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        var tpos = tilemap.WorldToCell(cell);
                        // Debug.Log(tpos);
                        if (tilemap.GetColor(tpos) == UnityEngine.Color.yellow)
                        {
                            RangeSplashAttack(cell, Enemy.ElementState.Default);
                            HandleCooldown();
                        }

                    }

                    if (player.abilityElement == Player.AbilityElement.Fire)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        var tpos = tilemap.WorldToCell(cell);
                        // Debug.Log(tpos);
                        if (tilemap.GetColor(tpos) == UnityEngine.Color.yellow)
                        {
                            RangeSplashAttack(cell, Enemy.ElementState.Fire);
                            HandleCooldown();
                        }

                    }

                    if (player.abilityElement == Player.AbilityElement.Electro)
                    {
                        cell = new Vector2(MathF.Floor(mouseHit.x) + 0.5f, MathF.Floor(mouseHit.y) + 0.5f);
                        var tpos = tilemap.WorldToCell(cell);
                        // Debug.Log(tpos);
                        if (tilemap.GetColor(tpos) == UnityEngine.Color.yellow)
                        {
                            RangeSplashAttack(cell, Enemy.ElementState.Electro);

                            HandleCooldown();
                        }

                    }

                    if (player.abilityElement == Player.AbilityElement.Wind)
                    {
                        distance = ManhattanDistance(player.transform.position, gameObj.transform.position);
                        Debug.Log(distance);

                        if ((gameObj.tag == "Enemy") && distance <= range + 0.1 + 2)
                        { 
                            Enemy enemy = gameObj.GetComponent<Enemy>();
                            enemy.TakeDamageWithCube(ThrowDice());
                            HandleCooldown();
                        }
                        if (gameObj.tag == "Rune" && distance <= range + 0.1 + 2)
                        {
                            gameObj.transform.position = player.transform.position;
                            HandleCooldown();
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
                        //Debug.Log(hit.transform.gameObject);
                    }
                    else
                    {
                        Activate(tilemap.transform.gameObject, mouseHit);
                        //Debug.Log("Tilemap");
                    }
                }
            }
        }

        if (abilityState == AbilityState.Cooldown && player.currentTurn >=  currentCooldown)
        {
            abilityState = AbilityState.Ready;
        }
    }
}
