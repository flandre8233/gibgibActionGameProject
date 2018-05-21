﻿using System.Collections;
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

                               // Use this for initialization
    void Start () {
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

        /*
        if ( Mathf.Abs(playermovement.instance.moveHorizontal)<=0.1f && Mathf.Abs(playermovement.instance.moveVertical) >= 0.1f ) {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * lerpSpeed * 3f);
        } else {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * lerpSpeed);
        }
        */

        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * lerpSpeed * (1+(3f* Mathf.Abs(playermovement.instance.moveVertical) )));

        if (!playerController.instance.isLockDown) {
            mouseControllCameraAngles();
        }

    }

    void mouseControllCameraAngles() {
        Camera mycam = Camera.main;

        transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);

        float mouseX = 0;
        float mouseY = 0;
        if (playerController.instance.isKeyboard) {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = -Input.GetAxis("Mouse Y");
        } else {
            mouseX = Input.GetAxis("joystick X");
            mouseY = -Input.GetAxis("joystick Y");
            if (mouseX < 0.15F && mouseX > -0.15F) {
                mouseX = 0;
            }
            if (mouseY < 0.15F && mouseY > -0.15F) {
                mouseY = 0;
            }
            mouseX *= 2.2f;
            mouseY *= 2.2f;
        }
   

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
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
