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
        forceFollowPlayerCentre();


        gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y  + playermovement.instance.moveHorizontal * 25 * Time.deltaTime, gameObject.transform.rotation.eulerAngles.z);

        /*
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            forceFollowPlayerCentre();
        }
        */
        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * 2);

    }

    void forceFollowPlayerCentre() {
        targetPoint = playermovement.instance.gameObject.transform.position;
    }
}
