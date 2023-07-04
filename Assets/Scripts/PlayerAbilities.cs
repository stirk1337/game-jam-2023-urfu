using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> currentObjectsInRange;
    void Start()
    {
        BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(3, 3);
        boxCollider.isTrigger = true;
        currentObjectsInRange = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentObjectsInRange.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        currentObjectsInRange.Remove(collision.gameObject);
    }

    void SelectAttack()
    {
        foreach (GameObject gameObj in currentObjectsInRange)
        {
            if (gameObj.tag == "Tile" || gameObj.tag == "Enemy")
            {
                gameObj.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    void Discard()
    {
        foreach (GameObject gameObj in currentObjectsInRange)
        {
            if (gameObj.tag == "Tile" || gameObj.tag == "Enemy")
            {
                gameObj.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SelectAttack();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Discard();
        }
    }
}
