using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLookAtPoint : SingletonMonoBehavior<cameraLookAtPoint> {
    Vector3 targetPoint ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y  + playermovement.instance.moveHorizontal * 53 * Time.deltaTime, gameObject.transform.rotation.eulerAngles.z);
        forceFollowPlayerCentre();

        if (playermovement.instance.moveVertical < 0 || playermovement.instance.moveVertical > 0) {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 4);
        } else {
            transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 2);
        }
        /*
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            forceFollowPlayerCentre();
        }
        */

    }

    void forceFollowPlayerCentre() {
        targetPoint = playermovement.instance.gameObject.transform.position;
    }
}
