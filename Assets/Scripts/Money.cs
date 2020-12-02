using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] int currency = 1;

    bool picked = false;
    CharacterController2D whoPicked;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (picked)
        {
            Vector2 targetPos = whoPicked.GetTargetMoneyPos();
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 30f*Time.deltaTime);
            if (Vector2.Distance((Vector2)transform.position, targetPos) < 0.2f) 
            {
                whoPicked.UpdateMoney();
                Destroy(gameObject);
            }
        }
    }

    public int PickUp(GameObject _whoPicked)
    {
        whoPicked = _whoPicked.GetComponent<CharacterController2D>(); ;
        picked = true;
        GetComponent<BoxCollider2D>().enabled = false;
        return currency;
    }
}
