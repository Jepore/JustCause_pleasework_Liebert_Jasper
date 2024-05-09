using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Attachment
{
    private Transform gunTip;

    // Start is called before the first frame update
    private void Start()
    {
        attachmentGameObject = PlayerController.Instance.transform.Find("Weapon").gameObject;
        gunTip = attachmentGameObject.transform.Find("Gun Tip").transform;
        attachmentGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
