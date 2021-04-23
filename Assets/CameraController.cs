using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float yaw = 0f, pitch = 0f;
    [SerializeField] Transform player;
    [SerializeField] float distToTarget = 4f;
    [SerializeField] float minPitch = -45f;
    [SerializeField] float maxPitch = 45f;
    [SerializeField] Vector3 cameraOffset;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        transform.position = player.position - transform.forward * distToTarget +
                                    transform.TransformVector(cameraOffset);
    }
}