using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Car Status")]
    [SerializeField]
    private float currentMoveSpeed;
    [SerializeField]
    private float targetMoveSpeed;
    private bool isStop = false;

    [Space]
    [SerializeField]
    private float moveSpeedAccelerationTime;
    [SerializeField]
    private AnimationCurve moveSpeedAccelerationCurve;
    [SerializeField]
    private float moveSpeedDecelerationTime;
    [SerializeField]
    private AnimationCurve moveSpeedDecelerationCurve;
    private Coroutine changeMoveSpeedCoroutine = null;

    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private AnimationCurve rotateCurve;

    [Header("Safety")]
    [SerializeField]
    private float safetyDistance;
    private bool hasSafetyProblem = false;
    private bool isWaitingForLanePoint = false;

    [Header("Road Information")]
    [SerializeField]
    private LanePoint startLanePoint;
    [SerializeField]
    private LanePoint nextLanePoint;

    [SerializeField]
    private int targetLaneIndex = 1;

    private void Start()
    {

        ChangeMoveSpeed(targetMoveSpeed);
        transform.SetParent(null);
        transform.position = startLanePoint.Position;
        // StartCoroutine(SafetyCheckRoutine());
        StartCoroutine(MoveRoutine());
    }

    public void ChangeTargetLane(int targetLaneIndex)
    {
        this.targetLaneIndex = targetLaneIndex;
    }

    /* Move Speed */
    public void ChangeMoveSpeed(float targetMoveSpeed)
    {
        if (changeMoveSpeedCoroutine != null)
            StopCoroutine(changeMoveSpeedCoroutine);

        this.targetMoveSpeed = targetMoveSpeed;
        changeMoveSpeedCoroutine = StartCoroutine(ChangeMoveSpeedRoutine(currentMoveSpeed, targetMoveSpeed));
    }

    private IEnumerator ChangeMoveSpeedRoutine(float targetMoveSpeed)
    {
        if (changeMoveSpeedCoroutine != null)
            StopCoroutine(changeMoveSpeedCoroutine);

        this.targetMoveSpeed = targetMoveSpeed;
        yield return StartCoroutine(ChangeMoveSpeedRoutine(currentMoveSpeed, targetMoveSpeed));
    }

    private IEnumerator ChangeMoveSpeedRoutine(float startMoveSpeed, float endMoveSpeed)
    {
        float timer = 0;

        float targetTime = startMoveSpeed > endMoveSpeed ? moveSpeedDecelerationTime : moveSpeedAccelerationTime;
        AnimationCurve curve = startMoveSpeed > endMoveSpeed ? moveSpeedDecelerationCurve : moveSpeedAccelerationCurve;

        while (timer < targetTime)
        {
            timer += Time.deltaTime;

            float a = curve.Evaluate(timer / targetTime);
            currentMoveSpeed = Mathf.LerpUnclamped(startMoveSpeed, endMoveSpeed, a);

            yield return null;
        }

        currentMoveSpeed = endMoveSpeed;
    }

    /* Move */
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            nextLanePoint = startLanePoint.GetNextLanePoint(targetLaneIndex);
            if (nextLanePoint == null)
            {
                startLanePoint.DeregisterUser(this.gameObject);
                this.gameObject.SetActive(false);
                break;
            }

            nextLanePoint.RegisterUser(this.gameObject);
            yield return StartCoroutine(LanePointToLanePointRoutine(nextLanePoint));
            nextLanePoint.DeregisterUser(this.gameObject);

            startLanePoint = nextLanePoint;
            // transform.position = startLanePoint.Position;
        }
    }

    private IEnumerator LanePointToLanePointRoutine(LanePoint nextLanePoint)
    {
        Vector3 moveDir = nextLanePoint.Position - startLanePoint.Position;
        float totalDistance = moveDir.magnitude;
        moveDir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        float timer = 0;
        float prevDistance = float.MaxValue;
        while (true)
        {
            while (true)
            {
                /* Safety Check */
                isWaitingForLanePoint = CheckUserOfNextLanePoint();
                hasSafetyProblem = CheckSafetyDistanceProblem();

                bool sholdStop = isWaitingForLanePoint || hasSafetyProblem;

                if (!sholdStop) break;

                yield return new WaitForSeconds(1f);
            }

            timer += Time.deltaTime;
            Vector3 newPosition = transform.position + moveDir * currentMoveSpeed * Time.deltaTime;
            transform.position = newPosition;

            // float leftDistance = (nextLanePoint.Position - transform.position).magnitude;
            // float movedDistance = totalDistance - leftDistance;
            // float rSpeed = rotateCurve.Evaluate(movedDistance / totalDistance) * rotateSpeed;
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rSpeed * Time.deltaTime);

            // 점점 멀어지고 있으면 break
            float distance = Vector3.Distance(transform.position, nextLanePoint.Position);
            if (distance > prevDistance) break;

            // // 정확히 계산해서 넘어가면 최대 위치로 지정해주는 것도 필요
            if (distance > 1)
                yield return null;
            else
                break;

            prevDistance = distance;
        }

        // print(timer);
    }

    /* Safety Check */
    private IEnumerator SafetyCheckRoutine()
    {
        bool stoped = false;
        float originalMoveSpeed = 0;

        while (true)
        {
            isWaitingForLanePoint = CheckUserOfNextLanePoint();
            hasSafetyProblem = CheckSafetyDistanceProblem();

            bool sholdStop = isWaitingForLanePoint || hasSafetyProblem;

            if (!stoped && sholdStop)
            {
                stoped = true;
                originalMoveSpeed = targetMoveSpeed;
                yield return StartCoroutine(ChangeMoveSpeedRoutine(0));
            }

            if (stoped && !sholdStop)
            {
                yield return new WaitForSeconds(2f);
                ChangeMoveSpeed(originalMoveSpeed);
                stoped = false;
            }

            yield return null;
        }
    }

    private bool CheckUserOfNextLanePoint()
    {
        if (nextLanePoint == null) return false;

        return !nextLanePoint.CanICome(this.gameObject);
    }

    private IEnumerator WaitForUserOfNextLanePoint()
    {
        bool stoped = false;
        float originalMoveSpeed = currentMoveSpeed;

        while (CheckUserOfNextLanePoint())
        {
            print("I'm waiting.");
            if (!stoped)
            {
                ChangeMoveSpeed(0);
                stoped = true;
            }
            yield return null;
        }

        if (stoped)
        {
            yield return new WaitForSeconds(1f);
            ChangeMoveSpeed(originalMoveSpeed);
        }
    }

    private bool CheckSafetyDistanceProblem()
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, safetyDistance);

        print(hits.Length);

        if (hits.Length == 0) return false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance > safetyDistance) continue;

            // If it is not a car, continue.
            if (!hit.collider.gameObject.TryGetComponent<CarMovement>(out CarMovement carMovement)) continue;

            // If the car is me, continue.
            if (carMovement == this) continue;

            else return true;
        }

        return false;
    }

    private IEnumerator WaitForSafetyDistance()
    {
        bool stoped = false;
        float originalMoveSpeed = currentMoveSpeed;

        while (true)
        {
            if (!CheckSafetyDistanceProblem()) break;

            if (!stoped)
            {
                ChangeMoveSpeed(0);
                stoped = true;
                hasSafetyProblem = true;
            }

            yield return null;
        }

        if (hasSafetyProblem)
        {
            yield return new WaitForSeconds(2f);
            ChangeMoveSpeed(originalMoveSpeed);
        }

        hasSafetyProblem = false;
    }

    /* Gizmo */
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = hasSafetyProblem ? Color.red : Color.green;
    //     Gizmos.DrawLine(transform.position, transform.position + (transform.forward * safetyDistance));

    //     Gizmos.color = isWaitingForLanePoint ? Color.red : Color.green;
    //     Gizmos.DrawSphere(transform.position + transform.up * 3, 1f);


    //     if (nextLanePoint == null) return;

    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(nextLanePoint.Position, 0.3f);
    // }
}
