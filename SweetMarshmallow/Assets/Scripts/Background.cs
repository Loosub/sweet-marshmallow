using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{   
    Vector3 offset;
    public GameObject player;
    public Transform cam; // Ä«¸Þ¶ó
    
    void Start()
    {
       
    }

    void Update()
    {
       transform.position = Vector3.Lerp(transform.position, player.transform.position, 0.05f);
    }

    void FixedUpdate()
    {
        GetComponent<Renderer>().material.mainTextureOffset = offset;
        offset = cam.position/1.5f * (Time.deltaTime* 1f);
    }
}