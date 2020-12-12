using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sharoeb : MonoBehaviour
{

    [SerializeField] float shootInteraval;
    [SerializeField] Shar spawningShar;
    [SerializeField] Transform pivot;
    [SerializeField] float startInterval = 0;
    [SerializeField] float speed = 5f;

    private float timer = 0;
    private float timerz = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        timerz += Time.fixedDeltaTime;
        if (timerz >= startInterval)
        {
            timerz = startInterval;
            timer += Time.fixedDeltaTime;
            if (timer >= shootInteraval)
            {
                timer = 0;
                spawningShar.Setup(speed);
                Instantiate(spawningShar, pivot.position, pivot.rotation);
            }
        }
    }
}
