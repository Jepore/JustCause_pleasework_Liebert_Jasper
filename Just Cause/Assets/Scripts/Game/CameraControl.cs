using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private static CameraControl _instance; 
    public static CameraControl Instance { get {return _instance;}}

    public Transform camTarget;
    // speed of interpolation for position and rotation (values are just trial/error) 
    private float posU = 0.13f;
    private float rotU = 0.09f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("tried to make 2 cameras");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
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
