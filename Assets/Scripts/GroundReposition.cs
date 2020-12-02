using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundReposition : MonoBehaviour
{

    Camera catCamera;

    void Start()
    {
        catCamera = GameObject.Find("Cat Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (catCamera.transform.position.x > transform.position.x + transform.localScale.x)
        {
            transform.position = new Vector2(transform.position.x + transform.localScale.x*3, transform.position.y);
        }
    }
}
