using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {
    private void Start() {

    }
    // Update is called once per frame
    void Update () {
        //getVector3ToVector3LookAtRotation();
    }

    void followPlayer() {
        //Vector3 cameraPos = playermovement.instance.gameObject.transform.position;

    }

    public Quaternion getVector3ToVector3LookAtRotation(Vector3 lookAtPointX, Vector3 lookAtPointY) {
        Vector3 relativePos = lookAtPointY - lookAtPointX;
        relativePos.y = 0;
        return Quaternion.LookRotation(relativePos);
    }

}
