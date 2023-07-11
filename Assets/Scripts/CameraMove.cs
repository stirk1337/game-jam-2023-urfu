using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 offset;
    [SerializeField] float cameraSpeed;
    [SerializeField] Player player;
    [SerializeField] float minFov;
    [SerializeField] float maxFov;
    [SerializeField] float sens;

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

        var fov = Camera.main.orthographicSize;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sens;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.orthographicSize = fov;
    }
}
