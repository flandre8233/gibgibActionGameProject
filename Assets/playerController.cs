using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : SingletonMonoBehavior<playerController> {
    public bool isLockDown = false;
    public bool isKeyboard = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isKeyboard) {
            if (Input.GetKeyDown("c")) {
                isLockDown = !isLockDown;
            }
        } else {
            if (Input.GetButtonDown("lockDownTarget")) {
                print("hit");
                isLockDown = !isLockDown;
            }
        }
      
        cameraLookAtPoint.instance.cameraLookAtPointUpdate();
        cameraScript.instance.cameraScriptUpdate();
        playermovement.instance.playermovementUpdate();

    }
    
}
