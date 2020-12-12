using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shar : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] public ParticleSystem destroyParticle;
    // Start is called before the first frame update

    private Rigidbody2D m_Rigidbody2d;

    public void Setup(float speed)
    {
        this.speed = speed;
    }

    void Start()
    {
        m_Rigidbody2d = GetComponent<Rigidbody2D>();
        
        m_Rigidbody2d.velocity = -transform.up * speed;
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(destroyParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
