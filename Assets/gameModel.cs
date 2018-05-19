using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameModel : SingletonMonoBehavior<gameModel> {
    public Quaternion getVector3ToVector3LookAtRotation(Vector3 lookAtPointX, Vector3 lookAtPointY) {
        Vector3 relativePos = lookAtPointY - lookAtPointX;
        relativePos.y = 0;
        return Quaternion.LookRotation(relativePos);
    }
}
