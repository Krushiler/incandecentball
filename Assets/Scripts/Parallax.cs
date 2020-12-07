using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Start is called before the first frame update
    private float length, height, startpos, startposy;
    [SerializeField] GameObject cam;
    [SerializeField] float parallaxEffect;
    void Start()
    {
        
        startpos = transform.position.x;
        startposy = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log(startpos);
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1- parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);
        float disty = (cam.transform.position.y * parallaxEffect);

        transform.position = new Vector3(startpos + dist, startposy + disty, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
