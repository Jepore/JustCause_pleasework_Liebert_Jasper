using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : Attachment
{
    public bool isGrappling = false;
    public bool isWallSticking = false;
    public bool isWallHanging = false;

    public Transform gunTip;

    private Vector3 gunTipStartPos;
    public Vector3 grapplePoint;
    private float grappleU;
    private float maxGrappleDist = 30f;
    private float grappleYOffset = 10f;



    // Start is called before the first frame update
    private void Start()
    {
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
        if (collected)
        {
            //gunTipStartPos = gunTip.position;
            Ray raycast = new Ray(gunTip.position, CameraControl.Instance.transform.forward);
            if (Physics.Raycast(raycast, out RaycastHit hit, maxGrappleDist))
            {
                Debug.Log(hit.point);
                PlayerController.Instance.test.transform.position = hit.point;
                    //test.transform.position = hit.point;
                    //grapplePoint = hit.point;
            }
            else
            {
                Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2 + grappleYOffset, maxGrappleDist);
                PlayerController.Instance.test.transform.position = CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(centerScreen);
                Debug.Log(CameraControl.Instance.gameObject.GetComponent<Camera>().ScreenToWorldPoint(Vector3.forward * maxGrappleDist));
                //grapplePoint = CameraControl.Instance.ScreenToWorldPoint(Vector3.forward * maxGrappleDist);
            }
        }
        else
        {
            Debug.Log("no grappling hook");
        }
    }

    public void StartGrapple()
    {
        Debug.Log("yo");
        isGrappling = true;
    }

    private void Grappling()
    {
        // interpolation with Vector3 of origional gun tip position (grapplePoint) and grapplePoint from GrappleStart()
    }

    private void StopGrapple()
    {
        gunTipStartPos = Vector3.zero;
        if (!isWallHanging || !isWallSticking)
        {

        }
    }
    // Update is called once per frame
   private void Update()
    {

    }
}
