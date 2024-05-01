using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;

    // input
    public ThirdPersonInputActions playerInputActions;
    private InputAction move;
    public InputAction lookDelta;

    // movement
    private Vector2 moveVector;
    private Rigidbody playerRB;
    public float movementForce = 1f;
    public float jumpForce = 5f;
    public float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    // look
    public float sensitivity = 2.0f;
    private float upDownRange = 80f;
    private float verticalRot;
    public Transform orientation;
    public Vector2 lookVector;
    private Vector3 charLook; // for only the player (no x value in the euler)

    // animator
    public bool sprinting;

    // singleton
    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    private void Awake()
    {
        playerRB = this.GetComponent<Rigidbody>();
        playerInputActions = new ThirdPersonInputActions();
        if (_instance != null && _instance != this)
        {
            Debug.Log("tried to make 2 players");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    /// <summary>
    /// will be called whenever SetActive(true)
    /// instead of start
    /// </summary>
    private void OnEnable()
    {
        playerInputActions.Player.Jump.started += DoJump;
        move = playerInputActions.Player.Move;
        lookDelta = playerInputActions.Player.Look;
        playerInputActions.Player.Enable();
    }

    /// <summary>
    /// will be called whenever SetActive(false)
    /// </summary>
    private void OnDisable()
    {
        playerInputActions.Player.Jump.started -= DoJump;
        playerInputActions.Player.Disable();
    }

    private void Update()
    {
        LookAt();
    }

    private void FixedUpdate()
    {
        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);

        playerRB.AddForce(forceDirection, ForceMode.Impulse);
        //forceDirection = Vector3.zero;

        // increasing the acceleration when going down so they don't float
        // (player falls faster when jumping or falling)
        if (playerRB.velocity.y < 0f)
        {
            playerRB.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
        }


        //YOUTUBE VIDEO THAT USES CINEMATICS BUT STILL COOL (ill prolly do it later)
        //Vector3 horizontalVelocity = playerRB.velocity;
        //horizontalVelocity.y = 0;
        // allegedly faster than normal magnitude since it doesn't have to do a sqr root
        //if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        //{
        //    playerRB.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * playerRB.velocity.y;
        //}

    }

    private void LookAt()
    {
        lookVector = lookDelta.ReadValue<Vector2>();

        float mouseX = lookVector.x * sensitivity;
        transform.Rotate(0, mouseX, 0);

        verticalRot -= lookVector.y * sensitivity;
        verticalRot = Mathf.Clamp(verticalRot, -upDownRange, upDownRange);
        orientation.localRotation = Quaternion.Euler(verticalRot, 0, 0);
    }


    private void DoJump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
        }
    }

    public bool IsGrounded()
    {
        // raycast starts above the object by 0.25 units
        Ray raycast = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        // if raycast hits anything
        // ray is 0.05f units longer than the 0.25 we set it above the player
        if (Physics.Raycast(raycast, out RaycastHit hit, 0.3f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
