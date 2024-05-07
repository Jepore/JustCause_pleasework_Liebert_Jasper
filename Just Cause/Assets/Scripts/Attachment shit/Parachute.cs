using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : Attachment
{
    public bool isParachuting = false;

    public GameObject parachuteObject;

    // Start is called before the first frame update
    private void Start()
    {
        attachmentGameObject = PlayerController.Instance.transform.Find("Parachute").gameObject;
        parachuteObject = PlayerController.Instance.transform.Find("Parachuter").gameObject;
        attachmentGameObject.SetActive(false);
        parachuteObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnParachute();
    }

    public void ActivateParachute()
    {
        if (collected && !isParachuting && !this.GetComponent<Grappler>().isGrappling)
        {
            isParachuting = true;
            PlayerController.Instance.forceDirection += Vector3.up * PlayerController.Instance.jumpForce;
            Debug.Log("parachute activate");
        }
        else
        {
            Debug.Log(isParachuting);
        }
    }

    public void SpawnParachute()
    {
        if (isParachuting)
        {
            parachuteObject.SetActive(true);
        }
        else
        {
            parachuteObject.SetActive(false);
        }
    }
}
