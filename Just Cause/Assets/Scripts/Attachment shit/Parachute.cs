using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : Attachment
{
    public static bool isParachuting;

    // Start is called before the first frame update
    private void Start()
    {
        attachmentGameObject = PlayerController.Instance.transform.Find("Parachute").gameObject;
        attachmentGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
