using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;
    [SerializeField] float turnWaitTime;
    bool onCooldown;
    Player player;

    void Start()
    {
        onCooldown = false;
        player = FindObjectOfType<Player>();

    }

    IEnumerator EnemiesTurn()
    {
        foreach(Enemy enemy in enemies)
        {
            yield return new WaitForSeconds(turnWaitTime);

            enemy.Move(player.transform);
        }
        State.Instance.IsPlayerTurn = true;
        onCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!State.Instance.IsPlayerTurn && !onCooldown)
        {
            StartCoroutine(EnemiesTurn());
            onCooldown = true;
        }
    }
}
