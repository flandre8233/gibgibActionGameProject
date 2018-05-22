using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : SingletonMonoBehavior<cameraScript> {
    [SerializeField]
    float orlCameraDisantce;
    float saveCameraDisantce;
    float cameraDisantce;

    [SerializeField]
    float cameraYOffset;

    Transform cameraTransform;

    [SerializeField]
    Transform orlCameraTransform;
    Vector3 orlCameraPosition;

    [SerializeField]
    bool isCameraCollising = false;

    Vector3 hitPoint;
    float newCameraDisantce;

    private void Start() {
        cameraTransform = transform;
        saveCameraDisantce = orlCameraDisantce;
    }
    // Update is called once per frame
    public void cameraScriptUpdate() {
        if (playerController.instance.isLockDown) {
            //如果鎖定了就拉近鏡頭距離
            cameraDisantce = orlCameraDisantce / 1.2f;
        } else {
            cameraDisantce = orlCameraDisantce;
        }
        cameraCollision();
        //將鏡頭距離數值寫進另一個拿來暫存位置的transform裡
        orlCameraTransform.localPosition = new Vector3(0, cameraYOffset, -cameraDisantce);
        //把暫存位置的transform中的vector3拿出來
        orlCameraPosition = orlCameraTransform.position;

        //如果鏡頭碰撞中
        if (isCameraCollising) {
            //求得鏡頭對著的位置
            Vector3 startPoint = new Vector3(cameraLookAtPoint.instance.transform.position.x, cameraLookAtPoint.instance.transform.position.y + (cameraYOffset / 2), cameraLookAtPoint.instance.transform.position.z);
            //與角度
            Quaternion rotate = gameModel.instance.getVector3ToVector3LookAtRotation3D(startPoint, orlCameraPosition);
            //求得最後鏡頭應在什麼地方(會再近玩家多點點，防止過份穿牆)
            Vector3 finalPoint = hitPoint - (rotate * (Vector3.forward * 0.5f));
            //以lerp方式把camera位置同步至修正穿牆後的鏡頭新位置
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, finalPoint, Time.deltaTime * 12);
        } else {
            //以lerp方式把camera位置同步至暫存位置
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, orlCameraPosition, Time.deltaTime * 3);
        }

    }

    //鏡頭穿牆判定
    public bool cameraCollision() {
        Vector3 startPoint = new Vector3(cameraLookAtPoint.instance.transform.position.x, cameraLookAtPoint.instance.transform.position.y + (cameraYOffset / 2), cameraLookAtPoint.instance.transform.position.z);
        Quaternion rotate = gameModel.instance.getVector3ToVector3LookAtRotation3D(startPoint, orlCameraPosition);
        //目標Layer
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
