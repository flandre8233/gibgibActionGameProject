using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameModel : SingletonMonoBehavior<gameModel> {
    public Quaternion getVector3ToVector3LookAtRotation(Vector3 lookAtPointX, Vector3 lookAtPointY) {
        Vector3 relativePos = lookAtPointY - lookAtPointX;
        relativePos.y = 0;
        return Quaternion.LookRotation(relativePos);
    }
    public Quaternion getVector3ToVector3LookAtRotation3D(Vector3 lookAtPointX, Vector3 lookAtPointY) {
        Vector3 relativePos = lookAtPointY - lookAtPointX;
        return Quaternion.LookRotation(relativePos);
    }
    public void joystickInputResidueFixer(ref float number) {
        if (number < 0.1F && number > -0.1F) {
            number = 0;
        }
    }

}
