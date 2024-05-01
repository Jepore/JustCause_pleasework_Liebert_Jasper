using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator anim;
    private float maxSpeed = 5f;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetFloat("speed", PlayerController.Instance.GetComponent<Rigidbody>().velocity.magnitude / maxSpeed);
        //Debug.Log(player.GetComponent<Rigidbody>().velocity.magnitude / maxSpeed);
        if (PlayerController.Instance.sprinting)
        {
            anim.SetBool("sprinting", true);
        }
    }
}
