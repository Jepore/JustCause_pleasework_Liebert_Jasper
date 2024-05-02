using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public bool collected = false;
    public static bool isGrappling;
    public static bool isWallSticking;


    public GameObject grapplingGun;
    public Transform gunTip;

    private Vector3 grapplePoint;
    private float grappleU;



    // Start is called before the first frame update
    private void Start()
    {
        grapplingGun = PlayerController.Instance.transform.Find("Grappler").gameObject;
        gunTip = grapplingGun.transform.Find("Gun Tip").transform;
        grapplingGun.SetActive(false);
    }

    private void OnDisable()
    {
        grapplingGun.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
