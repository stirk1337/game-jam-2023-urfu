using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float speed;

    private void Start()
    {

    }

    void Move(float x, float y)
    {
        player.position = new Vector3(player.position.x + x, player.position.y + y, 0);
        State.Instance.IsPlayerTurn = false;
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(1, 0);
        }
    }

    void Update()
    {
        if (State.Instance.IsPlayerTurn)
        {
            CheckInput();
        }
;   }
}
