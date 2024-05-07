using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



enum AttachmentType {rocket, parachute, glider, grappler}

public class PlayerController : MonoBehaviour
{
    public GameObject test;

    public Camera playerCamera;

    // input
    public ThirdPersonInputActions playerInputActions;
    public InputAction move;
    public InputAction lookDelta;

    // movement
    public Rigidbody playerRB;
    public float movementForce = 0.55f;
    public float jumpForce = 10f;
    private float maxSpeed = 5f;
    private float sprintMult = 1.35f;
    public Vector3 forceDirection = Vector3.zero;

    // look
    private float sensitivity = 0.2f;
    private float upDownRange = 80f;
    private float verticalRot;
    public float grappleRot;
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
        playerInputActions.Player.Sprint.started += Sprint;
        playerInputActions.Player.Sprint.canceled += StopSprint;
        playerInputActions.Player.Grapple.started += Grapple;
        playerInputActions.Player.Grapple.canceled += InitiateGrapple;

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
        playerInputActions.Player.Sprint.started -= Sprint;
        playerInputActions.Player.Sprint.canceled -= StopSprint;
        playerInputActions.Player.Grapple.started -= Grapple;
        playerInputActions.Player.Grapple.canceled -= InitiateGrapple;


        playerInputActions.Player.Disable();
    }

    private void Update()
    {
        Look();
    }

    private void FixedUpdate()
    {
        if (move.ReadValue<Vector2>().y < 0 && animateSprint)
        {
            movementForce /= sprintMult;
            animateSprint = false;
        }

        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce * 0.7f, forceDirection.y, move.ReadValue<Vector2>().y * movementForce);

        if (IsGrounded() || this.gameObject.GetComponent<Grappler>().isWallHanging || this.gameObject.GetComponent<Grappler>().isWallSticking) // don't know if it's bad to have 3 or statements but oh well
        {
            animateJump = false;
            this.GetComponent<Parachute>().isParachuting = false;
        }
        else
        {
            //Debug.Log("Jumping");
            forceDirection.x /= 2f;
            forceDirection.z /= 2f;
            animateJump = true;
        }

        if (move.ReadValue<Vector2>().y < 0)
        {
            //Debug.Log(move.ReadValue<Vector2>() + "forceDirection: " + forceDirection);
            forceDirection.z /= 2;
        }

        playerRB.AddForce(forceDirection, ForceMode.Impulse);

        forceDirection = Vector3.zero;

        // increase the acceleration when going down so they don't float (gravity is increased to 16 right now)

    }

    private void Look()
    {
        lookVector = lookDelta.ReadValue<Vector2>();

        float mouseX = lookVector.x * sensitivity;

        verticalRot -= lookVector.y * sensitivity;
        verticalRot = Mathf.Clamp(verticalRot, -upDownRange, upDownRange);

        if(this.gameObject.GetComponent<Grappler>().isGrappling)
        {
            grappleRot += mouseX;
            orientation.localRotation = Quaternion.Euler(verticalRot, grappleRot, 0);
        }
        else
        {
            transform.Rotate(0, mouseX, 0);
            orientation.localRotation = Quaternion.Euler(verticalRot, 0, 0);
        }

    }


    private void DoJump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            //Debug.Log("jumped");
            forceDirection += Vector3.up * jumpForce;
        }
        else
        {
            this.GetComponent<Parachute>().ActivateParachute();
        }
        //Debug.Log(forceDirection);

    }

    private void Grapple(InputAction.CallbackContext obj)
    {
        this.GetComponent<Grappler>().GrappleHook();
    }

    private void InitiateGrapple(InputAction.CallbackContext obj)
    {
        this.GetComponent<Grappler>().StartGrapple();
    }

    private void Sprint(InputAction.CallbackContext obj)
    {
        //Debug.Log("sprint");
        if (IsGrounded())
        {
            movementForce *= sprintMult;
            animateSprint = true;
        }
    }

    private void StopSprint(InputAction.CallbackContext obj)
    {
        //Debug.Log("no sprint");
        if (animateSprint)
        {
            movementForce /= sprintMult;
            animateSprint = false;
        }
    }

    public bool IsGrounded()
    {
        // ray starts above and goes down 0.05f units below the character to see if they are on the ground
        Ray raycast = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        // if raycast hits anything
        if (Physics.Raycast(raycast, out RaycastHit hit, .3f))
        {
            //test.transform.position = hit.point;
            //Debug.Log("IS GROUNDED");
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AttachManager(AttachmentType attachment)
    {
        switch (attachment)
        {
            case AttachmentType.rocket:
                this.gameObject.GetComponent<Weapon>().collected = true;
                this.gameObject.GetComponent<Weapon>().attachmentGameObject.SetActive(true);
                break;
            case AttachmentType.parachute:
                this.gameObject.GetComponent<Parachute>().collected = true;
                this.gameObject.GetComponent<Parachute>().attachmentGameObject.SetActive(true);
                break;
            case AttachmentType.glider:
                this.gameObject.GetComponent<Glider>().collected = true;
                this.gameObject.GetComponent<Glider>().attachmentGameObject.SetActive(true);
                break;
            case AttachmentType.grappler:
                this.gameObject.GetComponent<Grappler>().collected = true;
                this.gameObject.GetComponent<Grappler>().attachmentGameObject.SetActive(true);
                break;
            default:
                Debug.Log("sumn broke with attachmanager");
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "rocketPickup":
                AttachManager(AttachmentType.rocket);
                other.gameObject.SetActive(false);
                break;
            case "parachutePickup":
                AttachManager(AttachmentType.parachute);
                other.gameObject.SetActive(false);
                break;
            case "gliderPickup":
                AttachManager(AttachmentType.glider);
                other.gameObject.SetActive(false);
                break;
            case "grapplerPickup":
                AttachManager(AttachmentType.grappler);
                other.gameObject.SetActive(false);
                break;
            default:
                break;
        }

    }
}
