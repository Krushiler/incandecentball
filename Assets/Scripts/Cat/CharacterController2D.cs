using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{                     // Amount of force added when the player jumps.
	[Space(10)]
	[Header("Level Varibables")]
	[SerializeField] private string levelSection;
	[SerializeField] private int levelNumber;
	[SerializeField] private bool isFinalLevel = false;
	[SerializeField] private int moneyToEnd;
	[Space(10)]
	[Header("Cat Characteristics")]
	[SerializeField] CanvasController canvasController;
	[SerializeField] Camera playerCamera;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGroundDef;							// A mask determining what is ground to the character
	[SerializeField] private LayerMask m_WhatIsLadder;							// A mask determining what is ladder to the character
	[SerializeField] private LayerMask m_WhatIsWallJump;							// A mask determining what is ladder to the character
	[SerializeField] private LayerMask m_WhatIsInteractable;							// A mask determining what is ladder to the character
	[SerializeField] private LayerMask MoneyMask;							
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_CeilingCheckLT;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_CeilingCheckRB;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_GroundCheckRB;                         // A position marking where to check for ceilings
	[SerializeField] private Transform m_WallCheckRB;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_WallCheckRU;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_WallCheckLB;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_WallCheckLU;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_LadderCheckLT;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_LadderCheckRB;							// A position marking where to check for ceilings
	[SerializeField] private Transform m_ClipCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] float CoyoteTime;

	float levelTimer = 0f;
	bool countTime = true;

	private UIManager uiManager;
	private float m_JumpForce = 400f;
	private float speed = 40f;
	private int jumpsPerJump = 1;
	private float m_CrouchSpeed = .36f;
	private float ladderSpeed = 30;
	private int money = 0;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded = false;            // Whether or not the player is grounded.
	private bool m_Laddered = false;            // Whether or not the player is grounded.
	private bool m_CanLadder = false;         // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool canLadder = true;
	private bool canJumpFromWalls = true;
	private float wallJumpX = 1;
	private bool onWall = false;
	private bool inInteractZone = false;
	private Animator catAnimator;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;
	private float gravityScale;

	string key_time, key_money;

	LayerMask m_WhatIsGround;



	private void Awake()
	{
		m_WhatIsGround = m_WhatIsGroundDef | m_WhatIsWallJump;
		Time.timeScale = 1;
		uiManager = GetComponent<UIManager>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		gravityScale = m_Rigidbody2D.gravityScale;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}
	Vector3 rbPos, oldRbPos, clipPos, oldClipPos;
	private void Start()
	{
		key_money = levelSection + levelNumber.ToString() + "Money";
		key_time = levelSection + levelNumber.ToString() + "Time";
		rbPos = m_Rigidbody2D.position;
		oldRbPos = m_Rigidbody2D.position;
		clipPos = m_ClipCheck.position;
		oldClipPos = m_ClipCheck.position;
		UpdateMoney();
	}

	private float coyoteTimer = 0;
	bool jumped = false;
	int jumpCounter = 0;
	bool escapeFromLadder = false;
	bool escapeFromWall = false;
	float antiBugJumpTime = 0.3f;
	float antiBugJumpTimer = 0f;
	bool countAntiBugJumpTime = true;

	bool die = false;

	GameObject interactableGameObject = null;
	bool interacting = false;

	Vector2 colPos = Vector2.zero;

	bool paused = false;

	Vector2 pauseVel = Vector2.zero;

	public void PauseGame()
	{
		interacting = false;
		pauseVel = m_Rigidbody2D.velocity;
		m_Rigidbody2D.bodyType = RigidbodyType2D.Static;
		paused = true;
		canvasController.pauseGame();
	}
	public void UnPauseGame()
	{
		canvasController.unPauseGame();
		paused = false;
		m_Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
		m_Rigidbody2D.velocity = pauseVel;
	}

	private void Update()
	{
		if (countTime)
		{
			levelTimer += Time.deltaTime;
		}
		if (Input.GetButtonDown("Pause"))
		{
			if (!paused)
			{
				PauseGame();
			}
			else
			{
				UnPauseGame();
			}
		}
	}

	public void FinishLevel()
	{
		countTime = false;
		canvasController.levelFinishCanvasActive();
		interacting = true;
		if (levelNumber != 0)
		{
			if (!PlayerPrefs.HasKey(key_time))
			{
				PlayerPrefs.SetFloat(key_time, levelTimer);
				canvasController.setTime(levelTimer, levelTimer);
			}
			else
			{
				if (levelTimer < PlayerPrefs.GetFloat(key_time))
				{
					PlayerPrefs.SetFloat(key_time, levelTimer);
					canvasController.setTime(levelTimer, levelTimer);
				}
				else
				{
					canvasController.setTime(levelTimer, PlayerPrefs.GetFloat(key_time));
				}
			}
		}
		else
		{
			PlayerPrefs.SetInt("tutorial", 1);
		}
		if (!PlayerPrefs.HasKey("completedLevels"))
		{
			PlayerPrefs.SetInt("completedLevels", 1);
		}
		else
		{
			if (PlayerPrefs.GetInt("completedLevels") < levelNumber)
			{
				PlayerPrefs.SetInt("completedLevels", levelNumber);
			}
		}
		
	}

	public void UpdateMoney()
	{
		uiManager.UpdateMoney(money, moneyToEnd, levelSection);
	}
	float moveHor = 0;
	private void FixedUpdate()
	{
		if (!paused && !die)
		{
			if (antiBugJumpTimer < antiBugJumpTime && countAntiBugJumpTime)
			{
				antiBugJumpTimer += Time.fixedDeltaTime;
			}
			bool wasGrounded = m_Grounded;
			bool wasLaddered = m_Laddered;
			bool wasWalled = onWall;
			bool wasInteractedZone = inInteractZone;
			inInteractZone = false;
			m_Grounded = false;
			m_Laddered = false;
			onWall = false;

			Collider2D[] colliders = Physics2D.OverlapAreaAll(m_GroundCheck.position, m_GroundCheckRB.position, m_WhatIsGround);

			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					m_Grounded = true;
					if (!wasGrounded)
						OnLandEvent.Invoke();
				}
			}

			bool isLastLaddered = false;
			colliders = Physics2D.OverlapAreaAll(m_LadderCheckLT.position, m_LadderCheckRB.position, MoneyMask);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					money += colliders[i].gameObject.GetComponent<Money>().PickUp(gameObject);

				}
			}
			colliders = Physics2D.OverlapAreaAll(m_LadderCheckLT.position, m_LadderCheckRB.position, m_WhatIsLadder);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					isLastLaddered = true;
				}
			}

			if (!isLastLaddered)
			{
				escapeFromLadder = false;
			}


			colliders = Physics2D.OverlapAreaAll(m_LadderCheckLT.position, m_LadderCheckRB.position, m_WhatIsLadder);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject && !escapeFromLadder)
				{
					m_Laddered = true;
				}
			}

			interactableGameObject = null;

			Collider2D collider = Physics2D.OverlapArea(m_CeilingCheckLT.position, m_GroundCheckRB.position, m_WhatIsInteractable);
			if (collider != null && collider.gameObject != gameObject)
			{
				interactableGameObject = collider.gameObject;
				inInteractZone = true;
				if (!wasInteractedZone)
				{
					if (interactableGameObject.GetComponent<Interactable>().getType() == InteractableType.LevelEnder)
					{
						int bestMoney = 0;
						int playerMoney = 0;

						if (PlayerPrefs.HasKey(levelSection + levelNumber.ToString() + "Money"))
						{
							bestMoney = PlayerPrefs.GetInt(levelSection + levelNumber.ToString() + "Money");
						}

						if (PlayerPrefs.HasKey("Money"))
						{
							playerMoney = PlayerPrefs.GetInt("Money");
						}

						if (bestMoney < money)
						{
							if (money >= moneyToEnd)
							{
								PlayerPrefs.SetInt(levelSection + levelNumber.ToString() + "AllMoney", 1);
							}
							PlayerPrefs.SetInt(levelSection + levelNumber.ToString() + "Money", money);
							PlayerPrefs.SetInt("Money", playerMoney + money - bestMoney);
						}
						
						FinishLevel();
					}
					else
					{
						canvasController.EnablePrompt(interactableGameObject.GetComponent<Interactable>().getText());
					}
				}
			}
			if (wasInteractedZone && !inInteractZone)
			{
				canvasController.DisablePrompt();
			}
			bool isLastWalled = false;

			colliders = Physics2D.OverlapAreaAll(m_WallCheckLU.position, m_WallCheckRB.position, m_WhatIsWallJump);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					isLastWalled = true;
				}
			}

			if (!isLastWalled)
			{
				escapeFromWall = false;
			}

			colliders = Physics2D.OverlapAreaAll(m_WallCheckRU.position, m_WallCheckRB.position, m_WhatIsWallJump);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject && !escapeFromWall)
				{
					onWall = true;
					if (m_FacingRight)
					{
						wallJumpX = -1;
					}
					else
					{
						wallJumpX = 1;
					}
				}
			}



			if (jumpCounter == 0 && !m_Grounded)
			{
				jumpCounter = 1;
			}

			if (!canLadder)
			{
				m_Laddered = false;
			}

			if (!canJumpFromWalls)
			{
				onWall = false;
			}

			if (m_Grounded)
			{
				onWall = false;
			}

			if (m_Grounded && (!wasGrounded || antiBugJumpTimer >= antiBugJumpTime))
			{
				jumped = false;
				jumpCounter = 0;
				countAntiBugJumpTime = false;
			}
			if (wasGrounded && !m_Grounded)
			{
				coyoteTimer = 0;
				catAnimator.SetBool("Grounded", false);
			}
			if (!wasGrounded && m_Grounded)
			{
				catAnimator.SetBool("Grounded", true);
			}
			if (coyoteTimer <= CoyoteTime)
			{
				coyoteTimer += Time.fixedDeltaTime;
			}

			rbPos = m_Rigidbody2D.position;
			clipPos = m_ClipCheck.position;

			RaycastHit2D[] hit2D = Physics2D.LinecastAll(oldClipPos, clipPos, m_WhatIsGround);
			for (int i = 0; i < hit2D.Length; i++)
			{
				if (hit2D[i].collider != null)
				{
					m_Rigidbody2D.position = oldRbPos;
					m_Rigidbody2D.velocity = Vector2.zero;
				}
			}
			oldClipPos = m_ClipCheck.position;
			oldRbPos = m_Rigidbody2D.position;
		}
	}


	public void Move(float move, float verticalMove, bool crouch, bool jump, float jumpHolding, bool interact)
	{
		if ((interacting || die) && m_Rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
		{
			if (m_Grounded)
			{
				m_Rigidbody2D.velocity = Vector2.zero;
			}
		}
		if (!interacting && !paused && !die)
		{
			if (move != 0)
			{
				catAnimator.SetBool("Running", true);
			}
			else
			{
				catAnimator.SetBool("Running", false);
			}
			if (interact && interactableGameObject != null)
			{
				InteractableType interType;
				interType = interactableGameObject.GetComponent<Interactable>().getType();
				string interText;
				interText = interactableGameObject.GetComponent<Interactable>().getText();
				if (interType == InteractableType.LevelPicker)
				{
					canvasController.levelCanvasActive();
				}
			}

			moveHor = move;
			if (m_Laddered)
			{
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				move *= ladderSpeed;
				verticalMove *= ladderSpeed;
				m_Rigidbody2D.gravityScale = 0;
				m_Rigidbody2D.velocity = Vector2.zero;
				Vector3 targetVelocity = new Vector2(move * 10f, verticalMove * 10f);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
				if (jump)
				{
					jumpCounter++;
					// Add a vertical force to the player.
					escapeFromLadder = true;
					m_Laddered = false;
					jumped = true;
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
					m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * jumpHolding));
					antiBugJumpTimer = 0;
					countAntiBugJumpTime = true;
				}
			}
			else if (onWall)
			{
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
				m_Rigidbody2D.velocity = new Vector2(0, -1);
				if (wallJumpX < 0 && !m_FacingRight || wallJumpX > 0 && m_FacingRight)
				{
					Flip();
				}
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}

				if (jump)
				{
					jumpCounter++;
					// Add a vertical force to the player.
					escapeFromWall = true;
					onWall = false;
					jumped = true;
					m_Rigidbody2D.velocity = new Vector2(wallJumpX * 10f, 0);
					m_Rigidbody2D.AddForce(new Vector2(0, 1.2f * 550 * jumpHolding));
					antiBugJumpTimer = 0;
					countAntiBugJumpTime = true;
					if (move == 0)
					{
						Flip();
					}
				}
			}
			else
			{
				m_Rigidbody2D.gravityScale = gravityScale;
				move *= speed;
				if (m_wasCrouching && !crouch)
				{
					// If the character has a ceiling preventing them from standing up, keep them crouching 

					if (Physics2D.OverlapArea(m_CeilingCheckLT.position, m_CeilingCheckRB.position, m_WhatIsGround))
					{
						crouch = true;
					}

				}
				//only control the player if grounded or airControl is turned on
				if (m_Grounded || m_AirControl)
				{
					// If crouching
					if (crouch)
					{
						if (!m_wasCrouching)
						{
							m_wasCrouching = true;
							OnCrouchEvent.Invoke(true);
						}

						// Reduce the speed by the crouchSpeed multiplier
						move *= m_CrouchSpeed;

						// Disable one of the colliders when crouching
						if (m_CrouchDisableCollider != null)
							m_CrouchDisableCollider.enabled = false;
					}
					else
					{
						// Enable the collider when not crouching
						if (m_CrouchDisableCollider != null)
							m_CrouchDisableCollider.enabled = true;

						if (m_wasCrouching)
						{
							m_wasCrouching = false;
							OnCrouchEvent.Invoke(false);
						}
					}

					// Move the character by finding the target velocity
					Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
					// And then smoothing it out and applying it to the character
					if (m_Grounded)
					{
						m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
					}
					else
					{
						if (!(m_Rigidbody2D.velocity.x >= Mathf.Abs(move) * 10f && move > 0) && !(m_Rigidbody2D.velocity.x <= -Mathf.Abs(move) * 10f && move < 0))
						{
							if (Mathf.Abs(move * 4f + m_Rigidbody2D.velocity.x) > Mathf.Abs(move * 10f))
							{
								targetVelocity = Vector2.zero;
							}
							else
							{
								targetVelocity = new Vector2(move * 4f, 0);
							}

							//m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, m_Rigidbody2D.velocity+(Vector2)targetVelocity, ref m_Velocity, m_MovementSmoothing);
							m_Rigidbody2D.AddForce(targetVelocity * 10);
						}
					}

					// If the input is moving the player right and the player is facing left...
					if (move > 0 && !m_FacingRight)
					{
						// ... flip the player.
						Flip();
					}
					// Otherwise if the input is moving the player left and the player is facing right...
					else if (move < 0 && m_FacingRight)
					{
						// ... flip the player.
						Flip();
					}
				}
				// If the player should jump...
				if (((m_Grounded || coyoteTimer < CoyoteTime) && jump && !jumped) || ((jumpCounter < jumpsPerJump) && jump))
				{
					if (!Physics2D.OverlapCircle(m_CeilingCheck.transform.position, 0.5f, m_WhatIsGroundDef))
					{
						if (m_CrouchDisableCollider != null)
							m_CrouchDisableCollider.enabled = true;

						if (m_wasCrouching)
						{
							m_wasCrouching = false;
							OnCrouchEvent.Invoke(false);
						}
						// Add a vertical force to the player.
						jumpCounter++;
						jumped = true;
						m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
						m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * jumpHolding));
						antiBugJumpTimer = 0;
						countAntiBugJumpTime = true;
					}
					
				}
			}
		}
	}
	
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
		{
			canvasController.endScreenCanvasActive();
			die = true;
		}
	}

	public void SetSpeed(float _speed)
	{
		speed = _speed;
	}
	public void SetJumpForce(float _jumpForce)
	{
		m_JumpForce = _jumpForce;
	}
	public void SetJumpsPerJump(int _jumpsPerJump)
	{
		jumpsPerJump = _jumpsPerJump;
	}
	public void SetCrouchSpeed(float _crouchSpeed)
	{
		m_CrouchSpeed = _crouchSpeed;
	}
	public void SetLadderSpeed(float _ladderSpeed)
	{
		ladderSpeed = _ladderSpeed;
	}
	public void SetCanLadder(bool _canLadder)
	{
		canLadder = _canLadder;
	}
	public void SetCanJumpFromWalls(bool _canJumpFromWalls)
	{
		canJumpFromWalls = _canJumpFromWalls;
	}

	public void SetAnimator(Animator animator)
	{
		catAnimator = animator;
	}

	public Vector3 GetTargetMoneyPos()
	{
		return uiManager.GetMoneyPos(playerCamera);
	}

	public void SetInteracting(bool _interacting)
	{
		interacting = _interacting;
	}

	public bool GetLevelIsFinal()
	{
		return isFinalLevel;
	}
	public string GetLevelSection()
	{
		return levelSection;
	}
	public int GetLevelNumber()
	{
		return levelNumber;
	}

}
