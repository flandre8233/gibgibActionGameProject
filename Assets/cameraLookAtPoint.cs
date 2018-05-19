using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLookAtPoint : SingletonMonoBehavior<cameraLookAtPoint> {
    [SerializeField]
    public Transform lockDownTarget;
    public bool lockDown = false;

    Vector3 targetPoint ;

    float orlLerpSpeed = 2;
    float lerpSpeed;

	// Use this for initialization
	void Start () {
		
	}

    public Vector3 lockDownTargetlookAtEuler;


    // Update is called once per frame
    void Update () {
        if (lockDown) {
            Quaternion lookAtTarget = gameModel.instance.getVector3ToVector3LookAtRotation(playermovement.instance.transform.position, lockDownTarget.transform.position);
            lockDownTargetlookAtEuler = lookAtTarget.eulerAngles;
            gameObject.transform.rotation = Quaternion.Euler(lockDownTargetlookAtEuler.x, lockDownTargetlookAtEuler.y + playermovement.instance.moveHorizontal * 53 * Time.deltaTime, lockDownTargetlookAtEuler.z);
            lerpSpeed = orlLerpSpeed * 4;
        } else {
            lerpSpeed = orlLerpSpeed;
            gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y + playermovement.instance.moveHorizontal * 53 * Time.deltaTime, gameObject.transform.rotation.eulerAngles.z);
        }
        forceFollowTargetCentre();

        if (playermovement.instance.moveVertical < 0 || playermovement.instance.moveVertical > 0) {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * lerpSpeed * 2);
        } else {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * lerpSpeed);
        }
        if (Input.GetKeyDown("c")) {
            lockDown = !lockDown;
        }

    }

    void forceFollowTargetCentre() {
        if (lockDown) {
            targetPoint = playermovement.instance.gameObject.transform.position;
        } else {
            targetPoint = playermovement.instance.gameObject.transform.position;

        }
    }
}
