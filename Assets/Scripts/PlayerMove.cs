using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Player playerStats;
    [SerializeField] float speedField;
    [SerializeField] float SwipeDeadZone;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float cooldown;
    [SerializeField] public bool InSelect;
    [SerializeField] Vector2 mapSize;
    [SerializeField] public bool onDash;
    float startTouchPostitionX;
    float endTouchPositionX;
    float startTouchPostitionY;
    float endTouchPositionY;
    bool onCooldown;
    public Vector2 lastPos;
    public Vector2 targetPos;
    public float speed;



    private void Start()
    {
        lastPos = player.position;
        //targetPos = player.position;
        InSelect = false;
        speed = speedField;
    }

    IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        State.Instance.IsPlayerTurn = false;
        onCooldown = false;
    }

    void Move(float x, float y)
    {
        //player.position = new Vector3(player.position.x + x, player.position.y + y, 0);
        //player.position = Vector3.MoveTowards(player.position, new Vector2(player.position.x + x, player.position.y + y) , Time.deltaTime * velocity);
        //Debug.Log("i need to move!");
        lastPos = player.position;
        targetPos = new Vector2(player.position.x + x, player.position.y + y);
        animator.SetTrigger("Jump");
        onCooldown = true;
        StartCoroutine(WaitForCooldown());
    }

    void CheckInput()
    {
        if (InSelect)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(-1, 0);
            spriteRenderer.flipX = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(1, 0);
            spriteRenderer.flipX = false;
        }
    }

    void CheckSwipe()
    {
        if (InSelect)
            return;
        //Debug.Log(startTouchPostitionX.ToString() + endTouchPositionX.ToString());
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            startTouchPostitionX = Input.mousePosition.x;
            startTouchPostitionY = Input.mousePosition.y;
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            endTouchPositionX = Input.mousePosition.x;
            endTouchPositionY = Input.mousePosition.y;
            float distY = Mathf.Abs(endTouchPositionY - startTouchPostitionY);
            float distX = Mathf.Abs(endTouchPositionX - startTouchPostitionX);
            if (distX > SwipeDeadZone || distY > SwipeDeadZone)
            {
                if (distY < distX)
                {
                    if (endTouchPositionX < startTouchPostitionX)
                    {
                        Move(-1, 0);
                        if (!spriteRenderer.flipX)
                        {
                            spriteRenderer.flipX = true;
                            spriteRenderer.transform.position = new Vector2(spriteRenderer.transform.position.x - 0.2f, spriteRenderer.transform.position.y);
                        }
                    }

                    if (endTouchPositionX > startTouchPostitionX)
                    {
                        Move(1, 0);
                        if (spriteRenderer.flipX)
                        {
                            spriteRenderer.flipX = false;
                            spriteRenderer.transform.position = new Vector2(spriteRenderer.transform.position.x + 0.2f, spriteRenderer.transform.position.y);
                        }
                    }
                }
                else
                {
                    if (endTouchPositionY < startTouchPostitionY)
                    {
                        Move(0, -1);
                    }

                    if (endTouchPositionY > startTouchPostitionY)
                    {
                        Move(0, 1);
                    }
                }
            }
        }
    }

    bool IsPointWithinMap(Vector2 point, Vector2 mapSize)
    {
        float halfMapWidth = mapSize.x / 2f;
        float halfMapHeight = mapSize.y / 2f;

        if (point.x < -halfMapWidth || point.x > halfMapWidth)
            return false;

        if (point.y < -halfMapHeight || point.y > halfMapHeight)
            return false;

        return true;
    }

    void Update()
    {
        player.position = Vector3.MoveTowards(player.position, targetPos, Time.deltaTime * speed);

        if (!IsPointWithinMap(targetPos, mapSize))
        {
            targetPos = lastPos;
        }

        if (playerStats.state[Enemy.ElementState.Wind] > 0)
        {
            return;
        }
        if (State.Instance.IsPlayerTurn && !onCooldown && !onDash)
        {
            speed = speedField;
            CheckInput();
            CheckSwipe();
        }
;   }
}
