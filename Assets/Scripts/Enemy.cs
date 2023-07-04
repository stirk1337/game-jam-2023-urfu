using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int health;
    [SerializeField] int attackRange;
    [SerializeField] int MoveSpeed;
    [SerializeField] float moveVelocity;
    Vector2 lastPos;
    Vector2 targetPos;

    void Start()
    {
        lastPos = transform.position;
        targetPos = transform.position;
    }

    public void Move(Transform target)
    {
        float moveX = target.position.x - transform.position.x;
        float moveY = target.position.y - transform.position.y;

        if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
        {
            if (moveX > 0)
            {
                lastPos = transform.position;
                targetPos = new Vector2(transform.position.x + 1, transform.position.y);
            }
            else
            {
                lastPos = transform.position;
                targetPos = new Vector2(transform.position.x - 1, transform.position.y);
            }
        }
        else
        {
            if (moveY > 0)
            {
                lastPos = transform.position;
                targetPos = new Vector2(transform.position.x, transform.position.y + 1);
            }
            else
            {
                lastPos = transform.position;
                targetPos = new Vector2(transform.position.x, transform.position.y - 1);
            }
        }
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveVelocity);
    }
}
