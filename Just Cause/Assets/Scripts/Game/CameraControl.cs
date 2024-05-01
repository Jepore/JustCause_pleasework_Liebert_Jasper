using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform camTarget;
    // speed of interpolation for position and rotation (values are just trial/error) 
    private float posU = 0.13f;
    private float rotU = 0.09f;

    private void Start()
    {
        camTarget = PlayerController.Instance.transform.Find("Orientation").transform.Find("Camera Target").transform;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, camTarget.position, posU);
        transform.rotation = Quaternion.Lerp(transform.rotation, camTarget.rotation, rotU);

    }
}
