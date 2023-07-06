using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum AbilityElement
    {
        Default,
        Fire,
        Electro,
        Wind
    }

    public enum DiceElement
    {
        Default,
        Fire,
        Electro,
        Wind
    }

    

    [SerializeField] public int health;
    [SerializeField] public int currentTurn;
    [SerializeField] PlayerMove playerMove;
    [SerializeField] public AbilityElement abilityElement;
    [SerializeField] public DiceElement diceElement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            playerMove.targetPos = new Vector2(MathF.Floor(playerMove.transform.position.x) + 0.5f, MathF.Floor(playerMove.transform.position.y) + 0.5f);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
