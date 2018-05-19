using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {
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
    void Update () {
        //getVector3ToVector3LookAtRotation();
        if (cameraLookAtPoint.instance.lockDown) {
            cameraDisantce = orlCameraDisantce/1.2f;

            //cameraDisantce = orlCameraDisantce + Vector3.Distance(playermovement.instance.transform.position,cameraLookAtPoint.instance.lockDownTarget.position);
        } else {
            cameraDisantce = orlCameraDisantce;
        }

        //cameraTransform.localPosition = Quaternion.Euler(lookingAngles) * -(Vector3.forward * cameraDisantce);
        cameraTransform.localPosition = new Vector3(0,cameraYOffset, -cameraDisantce);


        //Vector3 newPos = (-lookingObject.forward) * cameraDisantce;
        //newPos.y += YOffset ;
        //cameraTransform.localPosition = newPos;


    }

    void followPlayer() {
        //Vector3 cameraPos = playermovement.instance.gameObject.transform.position;

    }



}
