using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RuneManager : MonoBehaviour
{

    enum RuneState
    {
        Active,
        Cooldown
    }
    
    [SerializeField] int cooldown;
    [SerializeField] int distance;
    [SerializeField] Player.AbilityElement runeElement;
    [SerializeField] int currentCooldown;
    [SerializeField] RuneState runeState;
    Player player;
    BoxCollider2D boxCollider;
    SpriteRenderer sprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Collect();
        }
    }

    void Collect()
    {
        player.abilityElement = runeElement;
        player.currentDices[runeElement].SetActive(true);
        currentCooldown = player.currentTurn + cooldown;
        runeState = RuneState.Cooldown;
        sprite.color = new Color(1f, 1f, 1f, 0f);
        boxCollider.enabled = false;
    }

    public List<Vector2> GetPointsAroundPlayer(Vector2 playerPosition)
    {
        List<Vector2> points = new List<Vector2>();

        for (float x = playerPosition.x - distance; x <= playerPosition.x + distance; x++)
        {
            for (float y = playerPosition.y - distance; y <= playerPosition.y + distance; y++)
            {
                if (Mathf.Abs(x - playerPosition.x) + Mathf.Abs(y - playerPosition.y) == distance)
                {
                    points.Add(new Vector2(x, y));
                }
            }
        }
        return points;
    }

    void Spawn()
    {
        runeState = RuneState.Active;
        runeElement = (Player.AbilityElement)Random.Range(1, 3);
        List<Vector2> spawnPoints = GetPointsAroundPlayer(player.transform.position);
        player = FindAnyObjectByType<Player>();
        sprite.color = Color.white;
        boxCollider.enabled = true;
        Vector2 randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        gameObject.transform.position = randomPoint;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Spawn();   
    }

    // Update is called once per frame
    void Update()
    {
       if (runeState == RuneState.Cooldown && player.currentTurn == currentCooldown)
        {
            Spawn();
        }
    }
}
