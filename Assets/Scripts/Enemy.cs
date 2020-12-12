using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType{
    JumpOnHead
}

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] float speed;
    [SerializeField] float distance;
    [SerializeField] float stayTime;
    [SerializeField] Transform groundCheckTransform;

    private bool movingRight = true;
    private float stayTimer = 0;
    private Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    private void FixedUpdate()
    {
        stayTimer += Time.fixedDeltaTime;
        if (stayTimer >= stayTime)
        {
            stayTimer = stayTime;
            if (movingRight)
            {
                rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
            }
            else
            {
                rigidbody2D.velocity = new Vector2(-speed, rigidbody2D.velocity.y);
            }
            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, distance);
            if (groundInfo.collider == false || groundInfo.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                movingRight = !movingRight;
                stayTimer = 0;
            }
        }
        else
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            
    }

}
