using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CompassPosition {
    N,
    S,
    W,
    E
}
public class playermovement : SingletonMonoBehavior<playermovement> {

    CharacterController playerController;
    public float speed = 0f;
    public float orl_speed = 7f;
    public float gravity = 5f;
    public float jump = 3f;
    float heightY = 0;

    
    public customizeMovement[] keycode;
    public AnimationCurve raiseMovement;
    public AnimationCurve dropMovement;
    public float passTime;

    public CompassPosition playerCompassPosition;

    // Use this for initialization
    void Start() {
        speed = orl_speed;
        playerController = GetComponent<CharacterController>();
    }

   public float moveHorizontal;
    public float moveVertical;
    void FixedUpdate() {
        int inputCount = 0;
        float angle = transform.rotation.eulerAngles.y;
        playerCompassPosition = getCompassPosition(angle);

        for (int i = 0; i < keycode.Length; i++) {
            keycode[ i ].update(raiseMovement, dropMovement, passTime);
            if (keycode[i].isRaise) {
                inputCount++;
            }
        }

        moveHorizontal = (-keycode[ 2 ].val) + keycode[ 3 ].val;
        moveVertical = (-keycode[ 1 ].val) + keycode[ 0 ].val;

        float inputAngles = 0;

        if (!cameraLookAtPoint.instance.lockDown) {
            if (inputCount > 0) {
                if (keycode[ 0 ].isRaise) {
                    inputAngles += 0;
                }
                if (keycode[ 1 ].isRaise) {
                    inputAngles += 180;
                }
                if (keycode[ 2 ].isRaise) {
                    inputAngles += 270;
                }
                if (keycode[ 3 ].isRaise) {
                    inputAngles += 90;
                }
                if (inputCount > 1 && keycode[ 0 ].isRaise && keycode[ 2 ].isRaise) {
                    inputAngles += 360;
                }
                changeFaceDir((inputAngles / inputCount) + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);
            }
        } else {
            changeFaceDir(cameraLookAtPoint.instance.lockDownTargetlookAtEuler.y);
        }

        Vector3 moveDir = new Vector3(moveHorizontal, 0.0f , moveVertical)*(speed * Time.deltaTime);
        heightY -= gravity * Time.deltaTime;
        moveDir.y = heightY;
        if (Input.GetKeyDown(KeyCode.Space)) {
            heightY = jump;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            speed *= 1.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            speed = orl_speed;
        }
        moveDir = Quaternion.Euler(0, cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y,0)*moveDir;
        playerController.Move(moveDir);
        //print(playerController.velocity);
    }

    public void changeFaceDir(float anglesY) {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,anglesY, transform.rotation.z);
    }

    CompassPosition getCompassPosition(float anglesY) {
        if (anglesY >= 315 || anglesY < 45) {
            return CompassPosition.N;
        } else if (anglesY >= 45 && anglesY < 135) {
            return CompassPosition.E;
        } else if (anglesY >= 135 && anglesY < 225) {
            return CompassPosition.S;
        } else {
            return CompassPosition.W;
        }
    }

}


[System.Serializable]
public class customizeMovement {
    public float val;
    public KeyCode keyCode;

    FloatLerp raiseLerpSystem;
    FloatLerp dropLerpSystem;
   public bool isRaise = false;
    public float update(AnimationCurve raiseMovement, AnimationCurve dropMovement,float passTime ) {
        if (Input.GetKey(keyCode)) {
            if (!isRaise) {
                isRaise = true;
                raiseLerpSystem = new FloatLerp();
                dropLerpSystem = new FloatLerp();
                raiseLerpSystem.startLerp(val, 1, raiseMovement, passTime);
            }
            if (raiseLerpSystem != null) {
                if (raiseLerpSystem.isLerping) {
                    val = raiseLerpSystem.update();
                }
            }
        } else {
            if (isRaise) {
                isRaise = false;
                raiseLerpSystem = new FloatLerp();
                dropLerpSystem = new FloatLerp();
                dropLerpSystem.var = val;
                dropLerpSystem.startLerp(val, 0, dropMovement, passTime);
            }



            if (dropLerpSystem != null) {
                if (dropLerpSystem.isLerping) {
                    val = dropLerpSystem.update();
                } else {
                    val = 0;
                }
            }
        }

        return val;
    }

    public void reset() {
        raiseLerpSystem = new FloatLerp();
        dropLerpSystem = new FloatLerp();
        val = 0;
        isRaise = false;
    }
}