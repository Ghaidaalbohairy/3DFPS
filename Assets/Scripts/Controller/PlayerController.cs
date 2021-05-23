using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which player moves")]
    public float moveSpeed = 2f;
    [Tooltip("The speed at which player rotates to look left and right (calculatee in dgrees)")]
    public float lookSpeed = 60f;
    [Tooltip("The power at which player jumps")]
    public float jumpPower = 8f;
    [Tooltip("The strngth of gravity")]
    public float gravity = 9.81f;

    [Header("Jump Timing")]
    public float jumpTimeLeniency = 0.1f;
    public float timeToStopBeignLenienct = 0;


    [Header("Required Refrences")]
    [Tooltip("The player chooter that fires projectiles")]
    public Shooter playerShooter;
    public Health playerHealth;
    public List<GameObject> disableWhileDead;
    bool doubleJumpAvailable = false;

//The character controller coponent on the player
    private CharacterController controller;
    private InputManager inputManager;

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetUpCharacterController();
        SetUpInputManager();
        ProcessMovement();
    }



    private void SetUpCharacterController() 
    {  
        controller = GetComponent<CharacterController>();
        if(controller == null)
        {
            Debug.LogError("The player controller script does not have a character controller on the same game object!");
        }
    }
    
    void SetUpInputManager ()
    {
        inputManager = InputManager.instance;
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>

    void Update()
    {
        //if Dead turn the gameObject off
        if (playerHealth.currentHealth <= 0)
        {
            foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(false);
            }
            //stop execution the rest of update funtion if the player is dead
            return;
        }
        else 
        {
             foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(true);
            }
        }

        ProcessMovement();
        ProcessRotation();
    }

    Vector3 moveDirection;


    void ProcessMovement () 
    {
        //Get the input from input manager
        float leftRightInput = inputManager.horizontalMoveAxis;
        float forwardBackwardInput = inputManager.verticalMoveAxis;
        bool jumpPressed = inputManager.jumpPressed;

        // Handle the control player while ot os on the ground 
        if(controller.isGrounded)
        {
            doubleJumpAvailable = true;
            timeToStopBeignLenienct = Time.time + jumpTimeLeniency;

            // Set the movment direction to be the received input, set y to 0 since we are on the ground
            moveDirection = new Vector3(leftRightInput,0,forwardBackwardInput);
            // Set the move direction in relation to the transform
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * moveSpeed;

            if (jumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }
        else 
        {
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackwardInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            if (jumpPressed && Time.time < timeToStopBeignLenienct)
            {
                moveDirection.y = jumpPower;
            }

            else if (jumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        if (controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }

        controller.Move(moveDirection * Time.deltaTime);

    }

    void ProcessRotation()
    {
        float HorizontalLookInput = inputManager.horizontalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, playerRotation.y + HorizontalLookInput * lookSpeed *
        Time.deltaTime, playerRotation.z));
    }

}
