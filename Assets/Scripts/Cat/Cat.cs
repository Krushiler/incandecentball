using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] CharacterController2D playerCat;
    [SerializeField] float speed = 40;
    [SerializeField] Animator animator;
    [SerializeField] int jumpsPerJump = 1;
    [SerializeField] float jumpForce = 600;
    [SerializeField] float ladderSpeed = 30;
    [SerializeField] bool canLadder = true;
    [Range(0, 1)] [SerializeField] float m_CrouchSpeed = .65f;
    [SerializeField] bool canJumpFromWalls = false;

    void Start()
    {
        playerCat.SetAnimator(animator);
        playerCat.SetJumpForce(jumpForce);
        playerCat.SetSpeed(speed);
        playerCat.SetJumpsPerJump(jumpsPerJump);
        playerCat.SetCrouchSpeed(m_CrouchSpeed);
        playerCat.SetLadderSpeed(ladderSpeed);
        playerCat.SetCanLadder(canLadder);
        playerCat.SetCanJumpFromWalls(canJumpFromWalls);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}