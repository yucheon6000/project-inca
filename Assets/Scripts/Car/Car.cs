using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public enum CarStates { Stop = 0, Drive }

    [SerializeField]
    private List<CarState> states = new List<CarState>();
    [SerializeField]
    private CarState currentState = null;

    [Space]
    [SerializeField]
    private LanePoint currentLanePoint = null;
    public LanePoint CurrentLanePoint
    {
        set { currentLanePoint = value; }
        get => currentLanePoint;
    }
    [SerializeField]
    private LanePoint nextLanePoint = null;
    public LanePoint NextLanePoint
    {
        set { nextLanePoint = value; }
        get => nextLanePoint;
    }
    [SerializeField]
    private LanePoint previousLanePoint = null;
    public LanePoint PreviousLanePoint
    {
        set { previousLanePoint = value; }
        get => previousLanePoint;
    }

    [SerializeField]
    private int targetLaneIndex;
    public int TargetLaneIndex => targetLaneIndex;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        transform.SetParent(null);
        transform.position = CurrentLanePoint.Position;

        NextLanePoint = CurrentLanePoint.GetNextLanePoint(TargetLaneIndex);
        NextLanePoint.RegisterUser(this.gameObject);
        // UpdateNextLanePoint();

        ChangeState(CarStates.Drive);
    }

    private void Update()
    {
        if (currentState == null) return;

        UpdateSafety();
        UpdateNextLanePoint();
        currentState.Excute(this);
    }

    public void ChangeState(CarStates newState)
    {
        if (states[(int)newState] == null) return;

        if (currentState != null)
            currentState.Exit(this);

        currentState = states[(int)newState];
        currentState.Enter(this);
    }

    public void ChangeTargetLane(int targetLaneIndex)
    {
        this.targetLaneIndex = targetLaneIndex;
    }

    private float distanceToNextLanePoint = float.MaxValue;
    public void UpdateNextLanePoint()
    {
        float dist = Vector3.Distance(transform.position, NextLanePoint.Position);

        if (dist < 2)
        {
            SetNextLanePoint();
            return;
        }

        if (dist > distanceToNextLanePoint)
        {
            SetNextLanePoint();
            return;
        }

        distanceToNextLanePoint = dist;
    }

    private void SetNextLanePoint()
    {
        PreviousLanePoint?.DeregisterUser(this.gameObject);
        CurrentLanePoint?.DeregisterUser(this.gameObject);
        NextLanePoint?.DeregisterUser(this.gameObject);

        PreviousLanePoint = CurrentLanePoint;
        CurrentLanePoint = NextLanePoint;

        NextLanePoint = CurrentLanePoint.GetNextLanePoint(TargetLaneIndex);
        if (NextLanePoint == null)
        {
            Destroy(gameObject);
            return;
        }
        NextLanePoint.RegisterUser(this.gameObject);
        distanceToNextLanePoint = float.MaxValue;
    }

    /* Safety */
    [Header("Safety")]
    [SerializeField]
    private float safetyDistance = 10f;

    [Space]
    [SerializeField]
    private bool isWaitingUserOfNextLanePoint = false;
    public bool IsWaitingUserOfNextLanePoint => isWaitingUserOfNextLanePoint;
    [SerializeField]
    private bool hasSafetyDistanceProblem = false;
    public bool HasSafetyDistanceProblem => hasSafetyDistanceProblem;
    public bool ShouldStop => IsWaitingUserOfNextLanePoint || HasSafetyDistanceProblem;

    public Vector3 MiddlePosition => transform.position + Vector3.up * 1;

    private void UpdateSafety()
    {
        isWaitingUserOfNextLanePoint = CheckUserOfNextLanePoint();
        hasSafetyDistanceProblem = CheckSafetyDistanceProblem();
    }

    private bool CheckUserOfNextLanePoint()
    {
        if (nextLanePoint == null) return false;

        return !nextLanePoint.CanICome(this.gameObject);
    }

    private bool CheckSafetyDistanceProblem()
    {
        Ray ray = new Ray(MiddlePosition, Vector3.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, safetyDistance);

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

    /* Gizmo */
    private void OnDrawGizmos()
    {
        Gizmos.color = hasSafetyDistanceProblem ? Color.red : Color.green;
        Gizmos.DrawLine(MiddlePosition, MiddlePosition + (transform.forward * safetyDistance));

        Gizmos.color = isWaitingUserOfNextLanePoint ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position + transform.up * 3, 1f);


        if (nextLanePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(nextLanePoint.Position, 0.3f);
    }
}
