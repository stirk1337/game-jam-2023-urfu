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
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            
            if (enemies[i].IsDead)
            {
                enemies.RemoveAt(i);
            }
            else
            {
                enemies[i].Move(player.transform);
            }
            yield return new WaitForSeconds(turnWaitTime);
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
                player.currentTurn += 1;
                onCooldown = true;
            }
        }
}
