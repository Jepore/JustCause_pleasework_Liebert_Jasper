using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : Attachment
{
    public bool isParachuting = false;

    public GameObject parachuteObject;
    private float parachuteGrav = 1.5f;

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
        if (collected && !isParachuting && !this.GetComponent<Grappler>().isGrappling && !this.GetComponent<Glider>().isGliding)
        {
            isParachuting = true;
            
            if (PlayerController.Instance.parachuteJump)
            {
                PlayerController.Instance.parachuteJump = false;
                PlayerController.Instance.forceDirection += Vector3.up * PlayerController.Instance.jumpForce * 2f;
            }

            PlayerController.Instance.act = PlayerController.Instance.Parachuting;
            Debug.Log("parachute activate");
        }
        else
        {
            //Debug.Log(isParachuting);
        }
    }

    public void SpawnParachute()
    {
        if (isParachuting)
        {
            parachuteObject.SetActive(true);
            PlayerController.Instance.playerRB.position -= Vector3.up * parachuteGrav * Time.deltaTime;
        }
        else
        {
            parachuteObject.SetActive(false);
        }
    }

    public void StopParachuting()
    {
        isParachuting = false;
        PlayerController.Instance.playerRB.useGravity = true;
    }
}
