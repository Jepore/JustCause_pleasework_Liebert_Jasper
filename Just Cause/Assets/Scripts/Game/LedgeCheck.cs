using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCheck : MonoBehaviour

{
    public Collision collision;
    public string collisionTag;

    void Update()
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            Debug.Log("Colliding with Player");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        this.collision = collision;
        collisionTag = collision.gameObject.tag;
    }

    void OnCollisionExit(Collision collision)
    {
        this.collision = null;
        collisionTag = null;
    }

}
