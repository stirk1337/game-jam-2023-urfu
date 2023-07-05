using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;
    [SerializeField] float cameraSpeed;

    void Update()
    {
        float camDistX = player.transform.position.x - transform.position.x;
        float camDistY = player.transform.position.y - transform.position.y;
        transform.position = new Vector3(transform.position.x + offset.x + (camDistX / cameraSpeed), transform.position.y + offset.y + (camDistY / cameraSpeed), transform.position.z);
    }
}
