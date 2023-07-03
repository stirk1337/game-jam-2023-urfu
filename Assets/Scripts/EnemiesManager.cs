using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies;


    void Start()
    {
         
    }

    void EnemiesTurn()
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.transform.position = new Vector3(enemy.transform.position.x - 1, enemy.transform.position.y, enemy.transform.position.z);
        }
        State.Instance.IsPlayerTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!State.Instance.IsPlayerTurn)
        {
            EnemiesTurn();
        }
    }
}
