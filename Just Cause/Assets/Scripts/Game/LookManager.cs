using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Liebert, Jasper
// 05/01/2024
// This script will manage the look direction of the player and let the camera go to the correct position
// without messing with the player model

public class LookManager : MonoBehaviour
{
    public Quaternion test;
    private void Update()
    {
        test = this.transform.rotation;
        //this.transform.position = PlayerController.Instance.transform.position;
        this.transform.localRotation = Quaternion.Euler(PlayerController.Instance.lookVector.y, 0, 0);
    }
}
