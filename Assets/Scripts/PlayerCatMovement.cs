using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerCatMovement : MonoBehaviour
{
    [SerializeField] CharacterController2D controller;
    [SerializeField] float jumpHolingTime = 0.2f;

    //[SerializeField] float speed;
    bool jump = false;
    bool crouch = false;
    bool interact = false;

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
    }

    float horizontalMove = 0;
    float verticalMove = 0;
    float jumpHolingTimer = 0;
    bool countJumpTimer = false;
    bool jumped = false; 

    // Update is called once per frame
    void Update()
    {
       /* if (countJumpTimer)
        {
            jumpHolingTimer += Time.deltaTime;
        }*/
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            //jumpHolingTimer = jumpHolingTime/2;
            // countJumpTimer = true;
            //jumped = false;
            jump = true;
            crouch = false;
        }
        /*if ((Input.GetButtonUp("Jump") || jumpHolingTimer >= jumpHolingTime) && countJumpTimer && !jumped)
        {
            if (jumpHolingTimer > jumpHolingTime)
            {
                jumpHolingTimer = jumpHolingTime;
            }
            jumpHolingTimer *= 1 / jumpHolingTime;
            jumped = true;
            jump = true;
            crouch = false;
        }*/

        if (Input.GetButtonDown("Interact"))
        {
            interact = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            if (!crouch)
            {
                crouch = true;
            }
            else
            {
                crouch = false;
            }
        }
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.deltaTime, crouch, jump, 1, interact);
        jump = false;
        interact = false;
    }
}