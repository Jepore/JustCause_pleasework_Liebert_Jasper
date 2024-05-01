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
    private float movementForce = 1f;
    private float jumpForce = 10f;
    private float maxSpeed = 5f;
    public Vector3 forceDirection = Vector3.zero;

    // look
    private float sensitivity = 0.2f;
    private float upDownRange = 80f;
    private float verticalRot;
    public Transform orientation;
    public Vector2 lookVector;
    private Vector3 charLook; // for only the player (no x value in the euler)

    // animator
    public bool animateSprint;
    public bool animateJump;

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
        //orientation.Find("Camera Target").transform.LookAt(this.gameObject);

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
        Look();
    }

    private void FixedUpdate()
    {
        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce, forceDirection.y, move.ReadValue<Vector2>().y * movementForce);

        playerRB.AddForce(forceDirection, ForceMode.Impulse);

        forceDirection = Vector3.zero;

        if (IsGrounded())
        {
            Debug.Log("not");
            animateJump = false;
        }
        else
        {
            Debug.Log("Jumping");
            animateJump = true;
        }

        // increase the acceleration when going down so they don't float

    }

    private void Look()
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
            Debug.Log("jumped");
            forceDirection += Vector3.up * jumpForce;
        }
        Debug.Log(forceDirection);

    }

    public bool IsGrounded()
    {
        Ray raycast = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        // if raycast hits anything
        if (Physics.Raycast(raycast, out RaycastHit hit, .3f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
