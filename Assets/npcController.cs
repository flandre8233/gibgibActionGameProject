using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class npcController : MonoBehaviour {
    public Transform attackTarget;
    public float attackDesire;
    public float detectRange;

    NavMeshAgent navMeshAgent;

    Vector3 navAgentNextLocalPos;
    float targetDistance;

    //strollAround
    float timerCD = 3.0f;
    float time = 0;
    Vector3 newGoToTarget = new Vector3();
    Vector3 orlV3 = new Vector3();
    Vector3 newOrlV3 = new Vector3();

   // Use this for initialization
   void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;

        //strollAround
        orlV3 = transform.position;
        newOrlV3 = orlV3;

        //Vector3 localPos = worldPosToLocalPos(testObject.transform, testObject2.transform.position);
        //print("localPos : " + localPos);
        //print("worldPos : " + localPosToWorldPos(testObject.transform, localPos));
    }
    // Update is called once per frame
    void Update() {

        //detect part
        Transform detectedTarget;
        Vector3 thisNpcOrlPos;
        Vector3 detectedTargetPos;
        if (detectAttackableTarget(out detectedTarget, out thisNpcOrlPos, out detectedTargetPos)) {
            attackDesire = 100;
            attackTarget = detectedTarget;
        }

        if (!attackTarget) {
            return;
        }
        //因為有很多移動方法都需要距離資料，所以拉出來集中處理
        //targetDistance = checkTargetDistance(attackTarget);
        //targetDistance = checkTargetDistance(localPosToWorldPos(transform, newGoToTarget));


        //move to target pos using by nav

        //runAwayFromTarget(attackTarget,3);
        //runAwayFromTarget2(attackTarget,5);
        //MoveTowards(attackTarget);

        //計算移動部份
        //其原理是將各種簡單單一移動方法坐標(LocalPos)都加在一起，就會同時得到幾種不同的移動方法，最後將結果(navAgentNextLocalPos)換回WorldPos給navMeshAgent使用
        resetLocalPosResult();
        //towardAndKeepDis();

        //strollAround
        strollAround();

        //最後運算
        Vector3 navAgentNextWorldPos = localPosToWorldPos(transform, navAgentNextLocalPos);
        navMeshAgent.SetDestination(navAgentNextWorldPos);

        RotateToNavAgentNextPos();
        //RotateTowards(navAgentNextWorldPos);
        //RotateTowards(attackTarget);
    }

    void unlockAttackTarget() {
        attackTarget = null;
    }

    void lockDownAttackTarget(Transform targetTransform) {
        attackTarget = targetTransform;
    }

    void lockDownPlayer() {
        lockDownAttackTarget(playermovement.instance.transform);
    }

    bool detectAttackableTarget(out Transform detectedTarget, out Vector3 thisNpcOrlPos, out Vector3 detectedTargetPos) {
        detectedTarget = null;
        thisNpcOrlPos = new Vector3();
        detectedTargetPos = new Vector3();
        //now detect player only 
        // find player in range

        float distance = checkTargetDistance(playermovement.instance.transform);
        if (distance <= detectRange) {
            //finded!
            detectedTarget = playermovement.instance.transform;
            thisNpcOrlPos = transform.position;
            detectedTargetPos = detectedTarget.position;

            return true;
        }
        return false;
    }


    void resetLocalPosResult() {
        //resetLocalPosResult
        navAgentNextLocalPos = new Vector3();
    }

    //base ai moving method
    private void MoveTowards(Transform target) {
        Vector3 localPos = worldPosToLocalPos(transform,target.position);
        navAgentNextLocalPos += localPos;
    }

    //行走指定角度
    private void walkAngles(float walkAngles) {
        //正數是逆時針 負數是順時針
        float val = -1 *(walkAngles % 360); 
        Vector3 localPos = worldPosToLocalPos(transform, transform.position + (transform.rotation * Quaternion.Euler(0,val,0) * new Vector3(0, 0, 1)));
        navAgentNextLocalPos += localPos;
    }

    //對著目標打轉
    private void keepCircle(bool isClockwise) {
        //正數是逆時針 負數是順時針
        int val = isClockwise ? -1 : 1;
        Vector3 localPos = worldPosToLocalPos(transform, transform.position + (transform.rotation * new Vector3(val, 0, 0)));
        navAgentNextLocalPos += localPos;
    }

    //當走進要保持距離的範圍時 會自動拉開一段距離
    private void runAwayFromTarget(Transform target, float keepDistance) {
        if (targetDistance >= keepDistance) {
            return;
        }
        Vector3 dir = transform.position - target.position;
        Vector3 newPos = transform.position + dir;
        Vector3 localPos = worldPosToLocalPos(transform, newPos);
        navAgentNextLocalPos += localPos ;
    }
    //當走進要保持距離的範圍時 會自動拉開一段距離 其距離另明顯
    private void runAwayFromTarget2(Transform target, float keepDistance) {
        if (targetDistance >= keepDistance) {
            return;
        }
        Vector3 newPos = (getLookVectorRotation(target.position, transform.position) * Vector3.forward * keepDistance) + target.position;
        Debug.DrawRay(target.position, newPos, Color.red);
        print(newPos);
        Vector3 localPos = worldPosToLocalPos(transform, newPos);
        navAgentNextLocalPos += localPos;
    }

    //adv ai move method
    void towardAndKeepDis() {
        if (targetDistance <= 5) {
            runAwayFromTarget(attackTarget, 4);
        } else {
            MoveTowards(attackTarget);
        }
    }

    void towardAndKeepDisCircle() {
        if (targetDistance <= 5) {
            keepCircle(true);
            runAwayFromTarget2(attackTarget, 4);
        } else {
            MoveTowards(attackTarget);
        }
    }

    //strollAround
    void strollAround() {
        //navMeshAgent.updateRotation = true;
        if (time >= timerCD) {
            //enter next move
            time = 0;
            timerCD = Random.Range(3, 6);
            newOrlV3 = (new Vector3(orlV3.x + Random.Range(-3, 3), transform.position.y, orlV3.z + Random.Range(-3, 3)));
        }

        //cant use distance
        float velocityFloat = Mathf.Abs(navMeshAgent.velocity.x) + Mathf.Abs(navMeshAgent.velocity.y) + Mathf.Abs(navMeshAgent.velocity.z);
        if (velocityFloat <= 0.0f) {
            time += Time.deltaTime;
        }
        newGoToTarget = worldPosToLocalPos(transform, newOrlV3);
        navAgentNextLocalPos += newGoToTarget;
    }

    private float rotationSpeed = 1;
    private void RotateTowards(Transform target) {
        RotateTowards(target.position);
    }
    private void RotateTowards(Vector3 target) {
        //transform.rotation = Quaternion.Slerp(transform.rotation, getLookVectorRotation(transform.position, target), Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, getLookVectorRotation(transform.position, target), Time.deltaTime * rotationSpeed);
    }

    //效果跟navAgent內建的旋轉功能差不多
    private void RotateToNavAgentNextPos() {
        Vector3 navAgentNextPos = addV3( transform.position , navMeshAgent.velocity);
        if (checkTargetDistance(navAgentNextPos) > 0.5f) {
            RotateTowards(navAgentNextPos);
        }
    }

    //some method

    float checkTargetDistance(Transform target) {
        return checkTargetDistance(target.position);
    }
    float checkTargetDistance(Vector3 position) {
        return Vector3.Distance(transform.position, new Vector3(position.x, transform.position.y, position.z) );
    }

    Vector3 worldPosToLocalPos(Transform masterTransform,Vector3 worldPos) {
        Vector3 ret = masterTransform.position;
        ret.x -= worldPos.x;
        ret.y -= worldPos.y;
        ret.z -= worldPos.z;
        ret.x *= -1;
        ret.y *= -1;
        ret.z *= -1;
        //changed to localPos
        return ret;
    }

    Vector3 localPosToWorldPos(Transform masterTransform,Vector3 localPos) {
        Vector3 ret = masterTransform.position;
        ret.x += localPos.x;
        ret.y += localPos.y;
        ret.z += localPos.z;
        return ret;
    }

    Quaternion getLookVectorRotation(Vector3 from, Vector3 to) {
        Vector3 direction = (to - from).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3

        return lookRotation;
    }

    Vector3 addV3(Vector3 var1,Vector3 var2) {
        Vector3 ret = var1;
        ret.x += var2.x;
        ret.y += var2.y;
        ret.z += var2.z;
        return ret;
    }

}
