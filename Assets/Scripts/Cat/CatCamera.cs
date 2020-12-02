using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCamera : MonoBehaviour
{
    [SerializeField] PlayerCatMovement catPlayer;
    [SerializeField] float cameraSpeed;

    void Start()
    {

    }

    void Update()
    {
        Vector3 targetPos = new Vector3(catPlayer.transform.position.x, catPlayer.transform.position.y + 1, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, cameraSpeed * Time.deltaTime);
    }
}