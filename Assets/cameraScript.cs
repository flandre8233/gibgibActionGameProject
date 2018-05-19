using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : SingletonMonoBehavior<cameraScript> {
    [SerializeField]
    float orlCameraDisantce;
    [SerializeField]
    float cameraDisantce;

    [SerializeField]
    float cameraYOffset;

    Transform cameraTransform;
    [SerializeField]
    Transform lookingObject;
    [SerializeField]
    Vector3 lookingAngles;
    private void Start() {
        cameraTransform = transform;
    }
    // Update is called once per frame
    public void cameraScriptUpdate() {
        if (playerController.instance.isLockDown) {
            cameraDisantce = orlCameraDisantce/1.2f;
        } else {
            cameraDisantce = orlCameraDisantce;
        }
        cameraTransform.localPosition = new Vector3(0,cameraYOffset, -cameraDisantce);


        //Vector3 newPos = (-lookingObject.forward) * cameraDisantce;
        //newPos.y += YOffset ;
        //cameraTransform.localPosition = newPos;


    }

    void followPlayer() {
        //Vector3 cameraPos = playermovement.instance.gameObject.transform.position;

    }



}
