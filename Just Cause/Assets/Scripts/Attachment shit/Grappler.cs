using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : Attachment
{
    public bool isGrappling = false;
    public bool isWallSticking = false;
    public bool isWallHanging = false;
    public bool grappled = false; // when the grappling hook is stuck to a wall
    private bool grapplingLine = false; // when the grappling hook is being rendered

    public Transform gunTip;
    public LineRenderer lr;

    private Vector3 gunTipStartPos;
    public Vector3 grapplePoint = Vector3.zero; 
    public float grappleU; // make all these private below
    public float grappleTimeStart;
    public float grappleTimeDuration;
    private float tempMoveForce;
    private float maxGrappleDist = 30f;
    private float grappleYOffset = 25f;



    // Start is called before the first frame update
    private void Start()
    {
        tempMoveForce = PlayerController.Instance.movementForce;
        attachmentGameObject = PlayerController.Instance.transform.Find("Grappler").gameObject;
        gunTip = attachmentGameObject.transform.Find("Gun Tip").transform;
        lr = this.GetComponent<LineRenderer>();
        attachmentGameObject.SetActive(false);
    }

    private void OnDisable()
    {
        attachmentGameObject.SetActive(false);
    }

    public void GrappleHook()
    {
        if (collected && !grappled)
        {
            Ray raycast = new Ray(CameraControl.Instance.transform.position, CameraControl.Instance.transform.forward);
            if (Physics.Raycast(raycast, out RaycastHit hit, maxGrappleDist) && hit.collider.gameObject != this.gameObject)
            {
                //Debug.Log(hit.point);
                PlayerController.Instance.test.transform.position = hit.point;
                PlayerController.Instance.movementForce /= 2;
                grapplePoint = hit.point;
                StartCoroutine(GrappleHookHit());
            }
            else
            {
                Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, maxGrappleDist);
                PlayerController.Instance.test.transform.position = CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(centerScreen);
                grapplePoint = CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(centerScreen);
                StartCoroutine(GrappleHookMiss());
                //Debug.Log(CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Vector3.forward * maxGrappleDist));
            }
        }
        else
        {
            Debug.Log("no grappling hook");
        }
    }

    public void StartGrapple()
    {
        // make sure we have a grappling point
        if (collected && grappled)
        {
            isGrappling = true;
            this.GetComponent<Parachute>().isParachuting = false; // i could also make the grappling hook pull the parachute in the direction of the grapplePoint for a period of time
            gunTipStartPos = gunTip.position;
            grappleTimeStart = Time.time;
            grappleTimeDuration = (grapplePoint - gunTipStartPos).magnitude/13f;

            //PlayerController.Instance.movementForce = 0f;
            //PlayerController.Instance.lookRot = PlayerController.Instance.transform.forward.y;
            //PlayerController.Instance.playerRB.useGravity = false;

            //grapplePoint = Vector3.zero;
        }
        else
        {
            Debug.Log("still no grappling hook");
        }
    }

    private void Grappling()
    {
        // interpolation with Vector3 of origional gun tip position (grapplePoint) and grapplePoint from GrappleStart()
        Vector3 grapplePos = gunTip.position;
        // Start Position: gunTipStartPos
        // End Position: grapplePoint
        if (isGrappling)
        {
            grappleU = ((grappleTimeStart + grappleTimeDuration) - Time.time)/grappleTimeDuration;
            
            if (grappleU < 0)
            {
                grappleU = 0;
                StopGrapple();
            }

            grapplePos = (1 - grappleU) * grapplePoint + grappleU * gunTipStartPos;
            //grapplePos = 
            Vector3 grappleGunTipOffset = gunTip.position - this.transform.position;

            if (PlayerController.Instance.IsGrounded() && grapplePos.y < gunTip.position.y)
            {
                grapplePos.y = gunTip.position.y;
            }
            //Debug.Log(grapplePos);
            PlayerController.Instance.playerRB.position = grapplePos - grappleGunTipOffset;
        }
    }

    private void RenderGrappleLine()
    {
        if(grapplingLine)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            lr.enabled = false;
            gunTipStartPos = Vector3.zero;
            grapplePoint = Vector3.zero;
        }
    }

    private void StopGrapple()
    { 
        if (!isGrappling) return;
        PlayerController.Instance.lookRot = PlayerController.Instance.transform.forward.y;
        PlayerController.Instance.playerRB.useGravity = true;
        PlayerController.Instance.act = PlayerController.Instance.Falling;

        PlayerController.Instance.movementForce = tempMoveForce;
        isGrappling = false;
        grappled = false; //for is wall hanging
        grapplingLine = false;
        
        // needs to detect edge to climb up or stick to the wall
    }

    // Update is called once per frame
   private void Update()
    {
        Grappling();
        RenderGrappleLine();

        if (PlayerController.Instance.playerInputActions.Player.Jump.triggered && isGrappling)
        {
            grappleU = 0;
            StopGrapple();
            if(!PlayerController.Instance.IsGrounded())
            {
                PlayerController.Instance.forceDirection += Vector3.up * PlayerController.Instance.jumpForce;
            }
        }
    }

    private IEnumerator GrappleHookHit()
    {
        PlayerController.Instance.movementForce = 0f;
        grappled = true;
        grapplingLine = true;
        lr.enabled = true;
        yield return new WaitForSeconds(0.3f);
        PlayerController.Instance.movementForce = tempMoveForce;
    }

    private IEnumerator GrappleHookMiss()
    {
        PlayerController.Instance.movementForce = 0f;
        grapplingLine = true;
        lr.enabled = true;
        yield return new WaitForSeconds(0.3f);
        grapplingLine = false;
        PlayerController.Instance.movementForce = tempMoveForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "floor")
        {
            grappleU = 0;
            StopGrapple();
            Debug.Log("hit something");
        }

    }
}
