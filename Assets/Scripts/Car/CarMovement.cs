using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Car Status")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private AnimationCurve rotateCurve;

    [Header("Road Information")]
    [SerializeField]
    private LanePoint startLanePoint;
    [SerializeField]
    private LanePoint nextLanePoint;

    [SerializeField]
    private int targetLaneIndex = 1;

    private void Start()
    {
        transform.position = startLanePoint.Position;
        StartCoroutine(MoveRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Test_ChangeLane();
    }

    public void Test_ChangeLane()
    {
        targetLaneIndex = targetLaneIndex == 1 ? 2 : 1;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            nextLanePoint = startLanePoint.GetNextLanePoint(targetLaneIndex);
            yield return StartCoroutine(LanePointToLanePointRoutine(nextLanePoint));
            startLanePoint = nextLanePoint;
            // transform.position = startLanePoint.Position;
        }
    }

    private IEnumerator LanePointToLanePointRoutine(LanePoint nextLanePoint)
    {
        Vector3 moveDir = nextLanePoint.Position - startLanePoint.Position;
        moveDir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
            transform.position = newPosition;

            float rSpeed = rotateCurve.Evaluate(Mathf.Lerp(0, 1, timer / 1)) * rotateSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rSpeed * Time.deltaTime);

            // 정확히 계산해서 넘어가면 최대 위치로 지정해주는 것도 필요
            if (Vector3.Distance(transform.position, nextLanePoint.Position) > 1)
                yield return null;
            else
                break;
        }

        print(timer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (nextLanePoint)
            Gizmos.DrawSphere(nextLanePoint.Position, 0.3f);
    }
}
