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

        while (true)
        {
            Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;
            transform.position = newPosition;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // 정확히 계산해서 넘어가면 최대 위치로 지정해주는 것도 필요
            if (Vector3.Distance(transform.position, nextLanePoint.Position) > 1)
                yield return null;
            else
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(nextLanePoint.Position, 1);
    }
}
