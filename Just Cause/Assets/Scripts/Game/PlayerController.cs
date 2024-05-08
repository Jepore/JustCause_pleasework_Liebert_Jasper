using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public enum AttachmentType {rocket, parachute, glider, grappler}
public enum ActionState {walking, sprinting, falling, grappling, parachuting, gliding}

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
    private float sensitivity = 0.1f;
    private float upDownRange = 80f;
    private float verticalRot;
    public float lookRot;
    public Transform orientation;
    public Vector2 lookVector;
    private Vector3 charLook; // for only the player (no x value in the euler)

    // animator
    public bool animateSprint;
    public bool animateJump;

    // action delegate
    public delegate void ActionDelegate();
    public ActionDelegate act;
    public ActionState acting;

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
    /// will be called whenever SetActive(true) instead of start
    /// just for all the input actions stuff
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

        act = Walking;
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
        ActManager();

        if (act != null)
        {
            act();
        }

        playerRB.AddForce(forceDirection, ForceMode.Impulse);

        forceDirection = Vector3.zero;

        // increase the acceleration when going down so they don't float (gravity is increased to 16 right now)
    }

    private void ActManager()
    {
        if (IsGrounded() && act != Sprinting && act != Grappling)
        {
            switch (acting)
            {
                case ActionState.parachuting:
                    Debug.Log("Ground Hit While Parachuting");
                    this.GetComponent<Parachute>().StopParachuting();
                    //act = Walking;
                    break;
                case ActionState.falling:
                    Debug.Log("Ground Hit While Falling");
                    animateJump = false;
                    //act = Walking;
                    break;
                default:
                    //Debug.Log("no act?");
                    break;

            }
            //if(act == Parachuting)
            //{
            //    Debug.Log("Ground Hit While Parachuting");
            //    this.GetComponent<Parachute>().StopParachuting();
            //    act = Walking;
            //}
            //else if (act == Falling)
            //{
            //    Debug.Log("Ground Hit While Falling");
            //    animateJump = false;
            //    act = Walking;
            //}
                
            act = Walking;
        }
        else if (act == Walking)
        {
            act = Falling;
        }
    }

    public void Walking()
    {
        acting = ActionState.walking;

        animateJump = false;
        animateSprint = false;

        if (move.ReadValue<Vector2>().y < 0)
        {
            //Debug.Log(move.ReadValue<Vector2>() + "forceDirection: " + forceDirection);
            forceDirection.z /= 2;
        }

        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce * 0.7f, forceDirection.y, move.ReadValue<Vector2>().y * movementForce);
    }

    private void Sprinting()
    {
        acting = ActionState.sprinting;

        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce * 0.7f * sprintMult, forceDirection.y, move.ReadValue<Vector2>().y * movementForce * sprintMult);
        if (move.ReadValue<Vector2>().y < 0 && animateSprint)
        {
            act = Walking;
            animateSprint = false;
        }
    }

    public void Falling()
    {
        acting = ActionState.falling;

        animateJump = true;
        if (playerRB.velocity.y < 0)
        {
            forceDirection.y = playerRB.velocity.y * Time.fixedDeltaTime * 3;
            //Debug.Log("doing the thing");
        }
        //Debug.Log("Jumping");
        forceDirection.x /= 2f;
        forceDirection.z /= 2f;

        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce * 0.7f * 0.5f, forceDirection.y, move.ReadValue<Vector2>().y * movementForce * 0.5f);
    }

    public void Grappling()
    {
        acting = ActionState.grappling;
        animateJump = false;

        playerRB.useGravity = false;
        //movementForce = 0f;
    }

    public void Parachuting()
    {
        acting = ActionState.parachuting;

        playerRB.useGravity = false;
        forceDirection = Quaternion.LookRotation(this.transform.forward, this.transform.up) * new Vector3(move.ReadValue<Vector2>().x * movementForce * 0.7f * 0.5f, forceDirection.y, move.ReadValue<Vector2>().y * movementForce * 0.5f);
    }

    public void Gliding()
    {
        acting = ActionState.gliding;

        playerRB.useGravity = false;

        if (IsGrounded())
        {
            this.GetComponent<Glider>().StopGliding();
        }
    }

    private void Look()
    {
        lookVector = lookDelta.ReadValue<Vector2>();

        float mouseX = lookVector.x * sensitivity;

        verticalRot -= lookVector.y * sensitivity;
        verticalRot = Mathf.Clamp(verticalRot, -upDownRange, upDownRange);

        if(this.gameObject.GetComponent<Grappler>().isGrappling)
        {
            lookRot += mouseX;
            orientation.localRotation = Quaternion.Euler(verticalRot, lookRot, 0);
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
            Debug.Log("jumped");
            act = Falling;
            forceDirection += Vector3.up * jumpForce;
        }
        else if (this.GetComponent<Grappler>().isGrappling)
        {
            this.GetComponent<Grappler>().StopGrapple();
            Debug.Log("grapplingjump");
            act = Falling;
            forceDirection = Vector3.up * jumpForce;
        }
        else if (this.GetComponent<Parachute>().isParachuting)
        {
            this.GetComponent<Parachute>().StopParachuting();
            //forceDirection += Vector3.up * jumpForce;
            act = Walking;
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
            //movementForce *= sprintMult;
            act = Sprinting;
            animateSprint = true;
        }
        else if (!GetComponent<Parachute>().isParachuting)
        {
            this.GetComponent<Glider>().ActivateGlider();
        }
    }

    private void StopSprint(InputAction.CallbackContext obj)
    {
        //Debug.Log("no sprint");
        if (animateSprint)
        {
            //movementForce /= sprintMult;
            animateSprint = false;
            act = Walking;
        }
    }

    public bool IsGrounded()
    {
        float frontBackCheckDist = 0.4f;
        float frontBackCeck = -frontBackCheckDist;

        for (int index = 0; index < 3; index++)
        {
            // ray starts above and goes down 0.05f units below the character to see if they are on the ground
            Ray raycast = new Ray(this.transform.position + Vector3.up * 0.25f + this.transform.forward * frontBackCeck, Vector3.down);
            // if raycast hits anything
            if (Physics.Raycast(raycast, out RaycastHit hit, .3f))
            {
                test.transform.position = hit.point;
                //Debug.Log("IS GROUNDED");
                return true;
            }
            else
            {
                frontBackCeck += frontBackCheckDist;
            }
        }
        return false; 
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
