using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : SingletonMonoBehavior<cameraScript> {
    [SerializeField]
    float orlCameraDisantce;
    float saveCameraDisantce;
    [SerializeField]
    float cameraDisantce;

    [SerializeField]
    float cameraYOffset;

    Transform cameraTransform;
    [SerializeField]
    Transform lookingObject;
    [SerializeField]
    Vector3 lookingAngles;

    [SerializeField]
    Transform orlCameraTransform;
    Vector3 orlCameraPosition;

    [SerializeField]
    bool isCameraCollising = false;

    private void Start() {
        cameraTransform = transform;
        saveCameraDisantce = orlCameraDisantce;
    }
    // Update is called once per frame
    public void cameraScriptUpdate() {
        if (playerController.instance.isLockDown) {
            cameraDisantce = orlCameraDisantce / 1.2f;
        } else {
            cameraDisantce = orlCameraDisantce;
        }

        //Vector3 newPos = (-lookingObject.forward) * cameraDisantce;
        //newPos.y += YOffset ;
        //cameraTransform.localPosition = newPos;

        cameraCollision();
        orlCameraTransform.localPosition = new Vector3(0, cameraYOffset, -cameraDisantce);
        orlCameraPosition = orlCameraTransform.position;
        if (isCameraCollising) {
            Vector3 startPoint = new Vector3(cameraLookAtPoint.instance.transform.position.x, cameraLookAtPoint.instance.transform.position.y + (cameraYOffset / 2), cameraLookAtPoint.instance.transform.position.z);
            Quaternion rotate = gameModel.instance.getVector3ToVector3LookAtRotation3D(startPoint, orlCameraPosition);
            //cameraTransform.position = hitPoint;

            Vector3 finalPoint = hitPoint - (rotate * (Vector3.forward * 0.5f));
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, finalPoint, Time.deltaTime * 12);
            //orlCameraDisantce = newCameraDisantce;
        } else {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, orlCameraPosition, Time.deltaTime * 3);
            //orlCameraDisantce = saveCameraDisantce;
        }

    }

    void followPlayer() {
        //Vector3 cameraPos = playermovement.instance.gameObject.transform.position;

    }

    Vector3 hitPoint;
    public float test;
    float newCameraDisantce;
    public bool cameraCollision() {
        Vector3 startPoint = new Vector3(cameraLookAtPoint.instance.transform.position.x, cameraLookAtPoint.instance.transform.position.y + (cameraYOffset / 2), cameraLookAtPoint.instance.transform.position.z);
        Quaternion rotate = gameModel.instance.getVector3ToVector3LookAtRotation3D(startPoint, orlCameraPosition);
        int layer_mask = LayerMask.GetMask("Default");
        Ray ray = new Ray(startPoint, rotate * Vector3.forward);
        RaycastHit hit;

        //Debug.DrawRay(startPoint, rotate * Vector3.forward, Color.red);
        if (Physics.Raycast(ray, out hit, 1.7f, layer_mask) && hit.transform.tag == "background") {
            hitPoint = hit.point;
            isCameraCollising = true;
        } else {
            isCameraCollising = false;
        }
        return isCameraCollising;
    }

}
