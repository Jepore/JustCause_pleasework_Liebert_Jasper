using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glider : Attachment
{


    // Start is called before the first frame update
    private void Start()
    {
        attachmentGameObject = PlayerController.Instance.transform.Find("Glider").gameObject;
        attachmentGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
