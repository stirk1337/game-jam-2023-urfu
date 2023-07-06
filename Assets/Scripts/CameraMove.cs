using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 offset;
    [SerializeField] float cameraSpeed;
    [SerializeField] Player player;

    private void Start()
    {
        transform.parent = null;
    }

    void Update()
    {
        if (player.health > 0)
        {
            float camDistX = playerTransform.transform.position.x - transform.position.x;
            float camDistY = playerTransform.transform.position.y - transform.position.y;
            transform.position = new Vector3(transform.position.x + offset.x + (camDistX / cameraSpeed), transform.position.y + offset.y + (camDistY / cameraSpeed), transform.position.z);
        }
    }
}
