using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glider : Attachment
{
    public bool isGliding = false;

    private float glideSpeed = 12.5f;
    private float glideDrag = 6f;

    private float anglePercentage;

    private Vector3 glideRotation;

    // Start is called before the first frame update
    private void Start()
    {
        attachmentGameObject = PlayerController.Instance.transform.Find("Glider").gameObject;
        attachmentGameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Gliding();
    }

    public void ActivateGlider()
    {
        if (collected && !isGliding)
        {
            isGliding = true;
            this.attachmentGameObject.SetActive(false);
            this.GetComponent<Grappler>().attachmentGameObject.SetActive(false);
            this.GetComponent<Parachute>().attachmentGameObject.SetActive(false);
            this.GetComponent<Weapon>().attachmentGameObject.SetActive(false);
            glideRotation = this.transform.eulerAngles;
            //PlayerController.Instance.playerRB.drag
            PlayerController.Instance.act = PlayerController.Instance.Gliding;
        }
    }

    private void Gliding()
    {
        if (isGliding)
        {
            // Rotate
            glideRotation.x += 20 * PlayerController.Instance.move.ReadValue<Vector2>().y * Time.fixedDeltaTime;
            glideRotation.x = Mathf.Clamp(glideRotation.x, -5, 45);
            glideRotation.y += 50 * PlayerController.Instance.move.ReadValue<Vector2>().x * Time.fixedDeltaTime;
            glideRotation.z = -30 * PlayerController.Instance.move.ReadValue<Vector2>().x * Time.fixedDeltaTime;
            glideRotation.z = Mathf.Clamp(glideRotation.z, -30, 30);
            transform.rotation = Quaternion.Euler(glideRotation);

            anglePercentage = (glideRotation.x + 5) / 50;
            float dragMod = (anglePercentage * -2) + 6;
            Debug.Log("angle" + anglePercentage);
            float speedMod = (anglePercentage * 20) + 12.5f;
            Debug.Log("speed" + speedMod);

            PlayerController.Instance.playerRB.drag = dragMod;
            Vector3 localVel = this.transform.InverseTransformDirection(PlayerController.Instance.playerRB.velocity);
            localVel.z = speedMod;
            PlayerController.Instance.playerRB.velocity = this.transform.TransformDirection(localVel);

            if (PlayerController.Instance.IsGrounded())
            {
                this.GetComponent<Glider>().StopGliding();
            }

        }
    }

    public void StopGliding()
    {
        if (isGliding)
        {
            this.attachmentGameObject.SetActive(true);
            this.GetComponent<Grappler>().attachmentGameObject.SetActive(true);
            this.GetComponent<Parachute>().attachmentGameObject.SetActive(true);
            this.GetComponent<Weapon>().attachmentGameObject.SetActive(true);

            glideRotation = new Vector3(0, glideRotation.y, 0);
            transform.rotation = Quaternion.Euler(glideRotation);
            Debug.Log("stopped gliding");
            isGliding = false;
            PlayerController.Instance.playerRB.drag = 3.5f;
            PlayerController.Instance.act = PlayerController.Instance.Falling;
        }
    }
}
