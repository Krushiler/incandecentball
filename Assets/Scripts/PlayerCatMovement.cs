using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCatMovement : MonoBehaviour
{
    [SerializeField] CharacterController2D controller;
    [SerializeField] float jumpHolingTime = 0.2f;
    CanvasController canvasController;

    [SerializeField] Sprite sitImage;
    [SerializeField] Sprite standImage;
    [SerializeField] Sprite sitCatImage;
    [SerializeField] Sprite standCatImage;


    //[SerializeField] float speed;
    bool jump = false;
    bool crouch = false;
    bool interact = false;
    Joystick joystick;
    ButtonControl jumpButton;
    ButtonControl crouchButton;
    ButtonControl interactButton;
    Image stateImage;

    void setSitImage()
    {
        crouchButton.gameObject.GetComponent<Image>().sprite = sitImage;
        stateImage.sprite = standCatImage;
    }

    void setStandImage()
    {
        crouchButton.gameObject.GetComponent<Image>().sprite = standImage;
        stateImage.sprite = sitCatImage;
    }

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        canvasController = controller.GetCanvasController();
        joystick = canvasController.getJoystick();
        jumpButton = canvasController.getJumpButton().GetComponent<ButtonControl>();
        crouchButton = canvasController.getCrouchButton().GetComponent<ButtonControl>();
        interactButton = canvasController.getInteractButton().GetComponent<ButtonControl>();
        stateImage = canvasController.getStateImage();
        setSitImage();
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
        if (horizontalMove == 0)
        {
            float joyHor = joystick.Horizontal;
            if (joyHor > 0.2f)
            {
                joyHor = 1;
            }else if (joyHor < -0.2f)
            {
                joyHor = -1;
            }
            else
            {
                joyHor = 0;
            }
            horizontalMove = joyHor;
        }
        verticalMove = Input.GetAxisRaw("Vertical");
        if (verticalMove == 0)
        {
            float joyVer = joystick.Vertical;
            if (joyVer > 0.2f)
            {
                joyVer = 1;
            }
            else if (joyVer < -0.2f)
            {
                joyVer = -1;
            }
            else
            {
                joyVer = 0;
            }
            verticalMove = joyVer;
        }
        if (Input.GetButtonDown("Jump") | jumpButton.buttonPressedDown)
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

        if (Input.GetButtonDown("Interact") || interactButton.buttonPressedDown)
        {
            interact = true;
        }

        if (crouch)
        {
            setStandImage();
        }
        else
        {
            setSitImage();
        }

        if (Input.GetButtonDown("Crouch") || crouchButton.buttonPressedDown)
        {
            if (!crouch)
            {
                crouch = true;
                setStandImage();
            }
            else
            {
                crouch = false;
                setStandImage();
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