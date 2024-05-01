using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform camTarget;
    public float posU = 0.02f;
    public float rotU = 0.01f;

    private void Start()
    {
        camTarget = PlayerController.Instance.transform.Find("Camera Target").transform;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, camTarget.position, posU);
        transform.rotation = Quaternion.Lerp(transform.rotation, camTarget.rotation, rotU);
    }
}
