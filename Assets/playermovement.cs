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
        if (Input.GetKey(KeyCode.W)) {
            changeFaceDir(0 + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);
        } else if (Input.GetKey(KeyCode.S)) {
            changeFaceDir(180 + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);
        } else if (Input.GetKey(KeyCode.A)) {
            changeFaceDir(270 + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);
        } else if (Input.GetKey(KeyCode.D)) {
            changeFaceDir(90 + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);

        }


        float angle = transform.rotation.eulerAngles.y;
        playerCompassPosition = getCompassPosition(angle);

        for (int i = 0; i < keycode.Length; i++) {
            keycode[ i ].update(raiseMovement, dropMovement, passTime);
        }

        moveHorizontal = (-keycode[ 2 ].val) + keycode[ 3 ].val;
        moveVertical = (-keycode[ 1 ].val) + keycode[ 0 ].val;

        //moveHorizontal = Input.GetAxis("Horizontal");
        //moveVertical = Input.GetAxis("Vertical");

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
    bool isRaise = false;
    public float update(AnimationCurve raiseMovement, AnimationCurve dropMovement,float passTime ) {
      
        if (Input.GetKeyDown(keyCode)) {
            raiseLerpSystem = new FloatLerp();
            isRaise = true;
            raiseLerpSystem.startLerp(val, 1, raiseMovement, passTime);
        }
        if (Input.GetKeyUp(keyCode)) {
            dropLerpSystem = new FloatLerp();
            isRaise = false;
            dropLerpSystem.var = val;
            dropLerpSystem.startLerp(val, 0, dropMovement, passTime);
        }
        if (!Input.GetKey(keyCode)) {
            isRaise = false;
        }
        if (isRaise) {
            if (raiseLerpSystem != null) {
                if (raiseLerpSystem.isLerping) {
                    val = raiseLerpSystem.update();
                }
            }
        } else {
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