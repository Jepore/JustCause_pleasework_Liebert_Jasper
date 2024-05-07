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
        if (PlayerController.Instance.animateSprint)
        {
            anim.SetBool("sprinting", true);
        }
        else
        {
            anim.SetBool("sprinting", false);
        }

        if (PlayerController.Instance.animateJump)
        {
            anim.SetBool("jumping", true);
        }
        else
        {
            anim.SetBool("jumping", false);
        }

        if(this.GetComponent<Grappler>().isGrappling)
        {
            anim.SetBool("grappling", true);
        }
        else
        {
            anim.SetBool("grappling", false);
        }

        if (this.GetComponent<Parachute>().isParachuting)
        {
            anim.SetBool("parachuting", true);
        }
        else
        {
            anim.SetBool("parachuting", false);
        }

        anim.SetFloat("strafe", PlayerController.Instance.move.ReadValue<Vector2>().x);

    }
}
