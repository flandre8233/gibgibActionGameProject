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
    public float gravity = 5f;
    public float jump = 3f;
    public float turnAroundSpeed;
    public float turnAroundDetectAngles;
    float heightY = 0;
    
    public customizeMovement[] keycode;
    public AnimationCurve raiseMovement;
    public AnimationCurve dropMovement;
    public float passTime;

    public CompassPosition playerCompassPosition;
    FloatLerp HraiseLerpSystem = new FloatLerp();
    FloatLerp HdropLerpSystem = new FloatLerp();
    FloatLerp VraiseLerpSystem = new FloatLerp();
    FloatLerp VdropLerpSystem = new FloatLerp();

    [SerializeField]
    Animator playerModelAnimator;

    // Use this for initialization
    void Start() {
        speed = orl_speed;
        playerCharController = GetComponent<CharacterController>();
    }

   public float moveHorizontal;
    public float moveVertical;
   public void playermovementUpdate() {
        int inputCount = 0;
        float angle = transform.rotation.eulerAngles.y;
        playerCompassPosition = getCompassPosition(angle);


        if (playerController.instance.isKeyboard) {
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

            Vector3 moveDir = new Vector3(moveHorizontal, 0.0f, moveVertical) * (speed * Time.deltaTime);
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
            moveDir = Quaternion.Euler(0, cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y, 0) * moveDir;
            playerCharController.Move(moveDir);
        } else {

            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
            if (moveHorizontal < 0.1F && moveHorizontal > -0.1F) {
                moveHorizontal = 0;
            }
            if (moveVertical < 0.1F && moveVertical > -0.1F) {
                moveVertical = 0;
            }
            /*
            if (Input.GetAxis("Horizontal") > moveHorizontal) {
                HdropLerpSystem = new FloatLerp();
                HraiseLerpSystem.startLerp(moveHorizontal, Input.GetAxis("Horizontal"), raiseMovement, passTime);
                if (HraiseLerpSystem != null && HraiseLerpSystem.isLerping) {
                        moveHorizontal = HraiseLerpSystem.update();
                }
            } else {
                HraiseLerpSystem = new FloatLerp();
                HdropLerpSystem.startLerp(moveHorizontal, Input.GetAxis("Horizontal"), dropMovement, passTime);
                if (HdropLerpSystem != null && HdropLerpSystem.isLerping) {
                        moveHorizontal = HdropLerpSystem.update();
                }
            }

            if (Input.GetAxis("Horizontal") > moveVertical) {
                VdropLerpSystem = new FloatLerp();
                VraiseLerpSystem.startLerp(moveVertical, Input.GetAxis("Vertical"), raiseMovement, passTime);
                if (VraiseLerpSystem != null && VraiseLerpSystem.isLerping) {
                        moveVertical = VraiseLerpSystem.update();
                }
            } else {
                VraiseLerpSystem = new FloatLerp();
                VdropLerpSystem.startLerp(moveVertical,  Input.GetAxis("Vertical"), dropMovement, passTime);
                if (VdropLerpSystem != null && VdropLerpSystem.isLerping) {
                        moveVertical = VdropLerpSystem.update();
                }
            }
            */
            
            float inputAngles = 0;
            if (!playerController.instance.isLockDown) {
                if (moveHorizontal == 0 && moveVertical == 0) {
                } else {
                    inputAngles = Mathf.Atan2(moveVertical, moveHorizontal) * Mathf.Rad2Deg;
                    inputAngles -= 90 - 180;
                    inputAngles += cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y;
                    if (inputAngles >= 360) {
                        inputAngles -= 360;
                    } else if (inputAngles < 0) {
                        inputAngles += 360;
                    }
                    //changeFaceDir(inputAngles + cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, inputAngles, transform.rotation.eulerAngles.z),Time.deltaTime* turnAroundSpeed);
                    if (Mathf.Abs(transform.rotation.eulerAngles.y - inputAngles) >= turnAroundDetectAngles && Mathf.Abs(transform.rotation.eulerAngles.y - inputAngles) <= 210) {
                        moveHorizontal = 0;
                        moveVertical = 0;
                    }
                }
            } else {
                changeFaceDir(cameraLookAtPoint.instance.lockDownTargetlookAtEuler.y);
            }

            Vector3 moveDir = new Vector3(moveHorizontal, 0.0f, -moveVertical) * (speed * Time.deltaTime);
            heightY -= gravity * Time.deltaTime;
            moveDir.y = heightY;
            moveDir = Quaternion.Euler(0, cameraLookAtPoint.instance.gameObject.transform.rotation.eulerAngles.y, 0) * moveDir;
            playerCharController.Move(moveDir);
        }
        playerModelAnimator.SetFloat("HInput", (Mathf.Abs(moveHorizontal )));
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