using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLookAtPoint : SingletonMonoBehavior<cameraLookAtPoint> {
    [SerializeField]
    public Transform lockDownTarget;

    Vector3 targetPoint ;

    [SerializeField]
    float orlLerpSpeed;
    float lerpSpeed;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    Camera mycam; 

    void Start () {
        mycam = Camera.main;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    public Vector3 lockDownTargetlookAtEuler;

    // Update is called once per frame
    public void cameraLookAtPointUpdate () {
        if (playerController.instance.isLockDown) {
            Quaternion lookAtTarget = gameModel.instance.getVector3ToVector3LookAtRotation(playermovement.instance.transform.position, lockDownTarget.transform.position);
            lockDownTargetlookAtEuler = lookAtTarget.eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(lockDownTargetlookAtEuler.x, lockDownTargetlookAtEuler.y + playermovement.instance.moveHorizontal * 53 * Time.deltaTime, lockDownTargetlookAtEuler.z);
            lerpSpeed = orlLerpSpeed * 4;
        } else {
            lerpSpeed = orlLerpSpeed;
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y + playermovement.instance.moveHorizontal * 53 * Time.deltaTime, gameObject.transform.rotation.eulerAngles.z);
        }
        forceFollowTargetCentre();

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPoint.x, targetPoint.y+0.5f, targetPoint.z), Time.deltaTime * lerpSpeed * (1+(3f* Mathf.Abs(playermovement.instance.moveVertical) )));

        //不在鎖定目標時
        if (!playerController.instance.isLockDown) {
            //控制鏡頭角度
            ControllCameraAngles();
        }

    }


    //控制鏡頭角度
    void ControllCameraAngles() {
        transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);
        float mouseX = 0;
        float mouseY = 0;
        //使用鍵盤
        if (playerController.instance.isKeyboard) {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = -Input.GetAxis("Mouse Y");
        } else {
            //使用手把
            mouseX = Input.GetAxis("joystick X");
            mouseY = -Input.GetAxis("joystick Y");
            //防止手把輸入出現餘數
            gameModel.instance.joystickInputResidueFixer(ref mouseX);
            gameModel.instance.joystickInputResidueFixer(ref mouseY);
            float absMouseX = Mathf.Abs(mouseX);
            float absMouseY = Mathf.Abs(mouseY);

            if (absMouseX > 0.8f) {
                mouseX *= 4f;
            } else if(absMouseX > 0.35f) {
                mouseX *= 2.2f;
            } else {
                mouseX *= 1.5f;
            }
            if (absMouseY > 0.8f) {
                mouseY *= 4f;
            } else if (absMouseY > 0.35f) {
                mouseY *= 2.2f;
            } else {
                mouseY *= 1.5f;
            }
        }
        float moveHVal = playermovement.instance.moveHorizontal;
        float absMoveHVal = Mathf.Abs(moveHVal);
        if (absMoveHVal > 0.8f) {
            moveHVal *= 1.2f;
        } else if (absMoveHVal > 0.35f) {
            moveHVal *= 0.75f;
        } else {
            moveHVal *= 0.35f;
        }
        mouseX += (moveHVal * 0.7f);

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX , rotY, 0.0f);
        transform.rotation = localRotation;
    }

    void forceFollowTargetCentre() {
        if (playerController.instance.isLockDown) {
            targetPoint = playermovement.instance.gameObject.transform.position;
        } else {
            targetPoint = playermovement.instance.gameObject.transform.position;

        }
    }
}
