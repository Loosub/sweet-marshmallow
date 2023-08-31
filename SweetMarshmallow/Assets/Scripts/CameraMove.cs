using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target; // 플레이어
    public float smoothing = 0.05f;
    Vector3 offset;
    
    void Start()
    {
        offset = transform.position - target.position;
        // Screen.SetResolution(540, 960, true);
    }

    void LateUpdate()
    {
        Vector3 CameraPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, CameraPosition, smoothing);
    }
}


