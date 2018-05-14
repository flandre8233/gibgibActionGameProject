using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : SingletonMonoBehavior<playermovement> {

    CharacterController playerController;
    public float speed = 0f;
    float orl_speed = 5f;
    public float gravity = 5f;
    public float jump = 3f;
    float heightY = 0;

    [SerializeField]
    customizeMovement[] keycode;
    public AnimationCurve raiseMovement;
    public AnimationCurve dropMovement;
    public float passTime;



    // Use this for initialization
    void Start() {
        speed = orl_speed;
        playerController = GetComponent<CharacterController>();
    }

    float moveHorizontal;
    float moveVertical;
    void FixedUpdate() {
        for (int i = 0; i < keycode.Length; i++) {
            customizeMovement item = keycode[ i ];
            item.update(raiseMovement, dropMovement, passTime);
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

        playerController.Move(moveDir);
        //print(playerController.velocity);
       

    }

}


[System.Serializable]
class customizeMovement {
    public float val;
    public KeyCode keyCode;

    FloatLerp raiseLerpSystem;
    FloatLerp dropLerpSystem;
    bool isRaise = false;
    public float update(AnimationCurve raiseMovement, AnimationCurve dropMovement,float passTime) {
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
                }
            }
        }
        if (Input.GetKeyDown(keyCode)) {
            raiseLerpSystem = new FloatLerp();
            isRaise = true;
            raiseLerpSystem.startLerp(0, 1, raiseMovement, passTime);
        }
        if (Input.GetKeyUp(keyCode)) {
            dropLerpSystem = new FloatLerp();
            isRaise = false;
            dropLerpSystem.startLerp(1, 0, dropMovement, passTime);
        }

        return val;
    }
}