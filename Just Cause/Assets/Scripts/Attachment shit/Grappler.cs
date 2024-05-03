using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : Attachment
{
    public bool isGrappling = false;
    public bool isWallSticking = false;
    public bool isWallHanging = false;
    public bool grappled = false; // when the grappling hook is stuck to a wall

    public Transform gunTip;

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
            Ray raycast = new Ray(gunTip.position, CameraControl.Instance.transform.forward);
            if (Physics.Raycast(raycast, out RaycastHit hit, maxGrappleDist))
            {
                //Debug.Log(hit.point);
                PlayerController.Instance.test.transform.position = hit.point;
                grapplePoint = hit.point;
                StartCoroutine(GrappleHookHit());
            }
            else
            {
                Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2 + grappleYOffset, maxGrappleDist);
                PlayerController.Instance.test.transform.position = CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(centerScreen);
                grapplePoint = CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(centerScreen);
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
            gunTipStartPos = gunTip.position;
            grappleTimeStart = Time.time;
            grappleTimeDuration = (grapplePoint - gunTipStartPos).magnitude/8f;
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
            Vector3 grappleGunTipOffset = gunTip.position - this.transform.position;

            if (PlayerController.Instance.IsGrounded() && grapplePos.y < gunTip.position.y)
            {
                grapplePos.y = gunTip.position.y;
            }
            //Debug.Log(grapplePos);
            this.transform.position = grapplePos - grappleGunTipOffset;
        }

    }

    private void StopGrapple()
    {

        isGrappling = false;
        grappled = false; //for is wall hanging

        //gunTipStartPos = Vector3.zero;
        //grapplePoint = Vector3.zero;
    }
    // Update is called once per frame
   private void Update()
    {
        Grappling();
    }

    private IEnumerator GrappleHookHit()
    {
        PlayerController.Instance.movementForce = 0f;
        grappled = true;
        yield return new WaitForSeconds(0.2f);
        PlayerController.Instance.movementForce = tempMoveForce;
    }
}
