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
    [SerializeField] Vector2 mapSize;
    [SerializeField] Player.AbilityElement runeElement;
    [SerializeField] int currentCooldown;
    [SerializeField] RuneState runeState;
    [SerializeField] AudioSource collectSound;
    Player player;
    public BoxCollider2D boxCollider;
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
        transform.position = new Vector2(-10000, -10000);
        collectSound.Play();
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

    public bool IsPointWithinMap(Vector2 point, Vector2 mapSize)
    {
        float halfMapWidth = mapSize.x / 2f;
        float halfMapHeight = mapSize.y / 2f;

        if (point.x < -halfMapWidth || point.x > halfMapWidth)
            return false;

        if (point.y < -halfMapHeight || point.y > halfMapHeight)
            return false;

        return true;
    }

    Vector2 GetRandomSpawnPoint()
    {
        List<Vector2> spawnPoints = GetPointsAroundPlayer(player.transform.position);
        Vector2 randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        if (IsPointWithinMap(randomPoint, mapSize))
        {
            return randomPoint;
        }
        else
        {
            return GetRandomSpawnPoint();
        }
    }

    void Spawn()
    {
        
        runeState = RuneState.Active;
        int random = Random.Range(1, 4);
        //Debug.Log(random);
        runeElement = (Player.AbilityElement)random;
        //runeElement = (Player.AbilityElement)3;
        
        player = FindAnyObjectByType<Player>();
        sprite.color = Color.white;
        boxCollider.enabled = true;
        
        gameObject.transform.position = GetRandomSpawnPoint();
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
