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
    CharacterController playerCharController;
    public float speed = 0f;
    public float orl_speed = 7f;
    public float gravity = 9.8f;
    public float jump = 3f;
    public float turnAroundSpeed;
    public float turnAroundDetectAngles;
    float heightY = 0;
    
    public customizeMovement[] keycode;
    [SerializeField]
    public AnimationCurve raiseMovement;
    [SerializeField]
    public AnimationCurve dropMovement;
    public float passTime;

    public CompassPosition playerCompassPosition;
    FloatLerp HraiseLerpSystem = new FloatLerp();
    FloatLerp HdropLerpSystem = new FloatLerp();
    FloatLerp VraiseLerpSystem = new FloatLerp();
    FloatLerp VdropLerpSystem = new FloatLerp();

    [SerializeField]
    Animator playerModelAnimator;

    public Vector3 slidingDirection;
    public bool isSliding;

    [SerializeField]
    bool isSprint;

    FloatLerp sprintSpeedRaiseLerpSystem = new FloatLerp();

    [SerializeField]
    float orlSprintSpeedPercentage;
    [SerializeField]
    float sprintTime;
    [SerializeField]
    float SprintSpeedPercentage;
    [SerializeField]
     AnimationCurve sprintSpeedRaiseMovement;
    // Use this for initialization
    void Start() {
        speed = orl_speed;
        playerCharController = GetComponent<CharacterController>();
        SprintSpeedPercentage = 100;
    }

   public float moveHorizontal;
    public float moveVertical;
    float inAirTime = 0;
    public void playermovementUpdate() {
        float angle = transform.rotation.eulerAngles.y;
        playerCompassPosition = getCompassPosition(angle);

        gravityControll();

        if (playerController.instance.isKeyboard) {
            int inputCount = 0;

            for (int i = 0; i < keycode.Length; i++) {
                keycode[ i ].update(raiseMovement, dropMovement, passTime);
                if (keycode[ i ].isRaise) {
                    inputCount++;
                }
            }
            moveHorizontal = (-keycode[ 2 ].val) + keycode[ 3 ].val;
            moveVertical = (-keycode[ 1 ].val) + keycode[ 0 ].val;

            float inputAngles = 0;

            if (!playerController.instance.isLockDown) {
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

            if (Input.GetKeyDown(KeyCode.Space)) {
                heightY = jump;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                speed *= 1.5f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift)) {
                speed = orl_speed;
            }

        } else {
            //手把輸入
                moveHorizontal = Input.GetAxis("Horizontal");
                moveVertical = Input.GetAxis("Vertical");
            if (isSliding) {
                moveHorizontal = 0;
                moveVertical = 0;
            }

            sprintControll();

            //防止手把輸入出現餘數
            gameModel.instance.joystickInputResidueFixer(ref moveHorizontal);
            gameModel.instance.joystickInputResidueFixer(ref moveVertical);
            float inputAngles = 0;
            if (!playerController.instance.isLockDown) {
                if (moveHorizontal == 0 && moveVertical == 0) {
                } else {
                    //HV輸入都沒有0時就令camera角度同步至inputAngles
                    //手把camera角度處理
                    inputAngles = Mathf.Atan2(moveVertical, moveHorizontal) * Mathf.Rad2Deg;
                    inputAngles -= 90 - 180;
                    inputAngles += cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y;
                    if (inputAngles >= 360) {
                        inputAngles -= 360;
                    } else if (inputAngles < 0) {
                        inputAngles += 360;
                    }
                    //以Slerp方式同式
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, inputAngles, transform.rotation.eulerAngles.z),Time.deltaTime* turnAroundSpeed);
                    //假如旋轉角度過大時就馬上急停HV速度
                    if (Mathf.Abs(transform.rotation.eulerAngles.y - inputAngles) >= turnAroundDetectAngles && Mathf.Abs(transform.rotation.eulerAngles.y - inputAngles) <= 210) {
                        //playerModelAnimator.SetTrigger("isTurn");
                        moveHorizontal = 0;
                        moveVertical = 0;
                        resetSprint();
                    }
                }
            } else {
                //在鎖定時，強制看著鎖定對象
                changeFaceDir(cameraLookAtPoint.instance.lockDownTargetlookAtEuler.y);
            }
            
        }
        //移動處理
        moveControll();

        //設定動畫
        animatorSendVal();
    }

    public void sprintControll() {
        if (Input.GetButtonDown("Sprint")) {
            isSprint = true;
            sprintSpeedRaiseLerpSystem.startLerp(100, orlSprintSpeedPercentage, sprintSpeedRaiseMovement, sprintTime);
        }
        if (Input.GetButtonUp("Sprint")) {
            isSprint = false;
            sprintSpeedRaiseLerpSystem = new FloatLerp();
            SprintSpeedPercentage = 100;
        }
        if (isSprint && sprintSpeedRaiseLerpSystem.isLerping) {
            SprintSpeedPercentage = sprintSpeedRaiseLerpSystem.update();
        }
    }
    public void resetSprint() {
        if (isSprint) {
            sprintSpeedRaiseLerpSystem = new FloatLerp();
            SprintSpeedPercentage = 100;
            sprintSpeedRaiseLerpSystem.startLerp(100, orlSprintSpeedPercentage, sprintSpeedRaiseMovement, sprintTime);
        }
    }


    public void gravityControll() {
        heightY = -(gravity );
        if (playerCharController.isGrounded) {
            inAirTime = 0;
            heightY = -(gravity);
        } else {
            inAirTime += Time.deltaTime;
            heightY -= gravity * 2 * ((inAirTime + 1));
        }
    }

    public void moveControll() {
        Vector3 moveDir = new Vector3(moveHorizontal, 0.0f, -moveVertical) * (speed) * (SprintSpeedPercentage / 100f);
        moveDir.y = heightY;
        moveDir = Quaternion.Euler(0, cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y, 0) * moveDir;
        moveDir += slidingDirection;
        playerCharController.Move(moveDir * Time.deltaTime);
    }

    //設定動畫
    public void animatorSendVal() {
        playerModelAnimator.SetFloat("HInput", (Mathf.Abs(moveHorizontal)));
        playerModelAnimator.SetFloat("VInput", (Mathf.Abs(moveVertical)));
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

    void OnControllerColliderHit(ControllerColliderHit hit) {
        float myAng = Vector3.Angle(Vector3.up, hit.normal); //Calc angle between normal and character
        if (myAng > playerCharController.slopeLimit) {
            isSliding = true;

            Vector3 normal = hit.normal;
            Vector3 c = Vector3.Cross(Vector3.up, normal);
            Vector3 u = Vector3.Cross(c, normal);
            slidingDirection = u * 4f;

        } else {
            isSliding = false;
            slidingDirection = Vector3.zero;
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