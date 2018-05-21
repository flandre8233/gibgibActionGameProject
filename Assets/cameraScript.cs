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

        cameraCollision();
    }

    void followPlayer() {
        //Vector3 cameraPos = playermovement.instance.gameObject.transform.position;

    }


    public void cameraCollision() {
        Vector3 startPoint = new Vector3(cameraLookAtPoint.instance.transform.position.x, cameraLookAtPoint.instance.transform.position.y + (cameraYOffset / 2), cameraLookAtPoint.instance.transform.position.z);
        Vector3 endPoint = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
        Quaternion rotate = gameModel.instance.getVector3ToVector3LookAtRotation3D(startPoint, endPoint);
        int layer_mask = LayerMask.GetMask("Default");
        //Vector3 endPoint = new Vector3(cameraTransform.position.x, cameraTransform.position.y - (cameraYOffset / 2), cameraTransform.position.z);
        Ray ray = new Ray(startPoint, rotate * Vector3.forward);
        RaycastHit hit;
        Debug.DrawRay(startPoint, rotate * Vector3.forward, Color.red);
        if (Physics.Raycast(ray, out hit, cameraDisantce*1.5f, layer_mask) && hit.transform.tag == "background") {
            print(hit.collider.gameObject.name);
        }
    }

}
