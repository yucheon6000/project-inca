using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CarStates { Stop = 0, Drive, Global }

public class Car : MonoBehaviour
{
    [Header("States")]
    [SerializeField]
    private List<State<Car>> states = new List<State<Car>>();
    private StateMachine<Car> stateMachine = new StateMachine<Car>();
    private CarStates currentState = CarStates.Stop;
    public CarStates CurrentState => currentState;

    [Header("Lane Information")]
    [Space]
    [SerializeField]
    private LanePoint currentLanePoint = null;
    public LanePoint CurrentLanePoint
    {
        set => currentLanePoint = value;
        get => currentLanePoint;
    }
    [SerializeField]
    private LanePoint nextLanePoint = null;
    public LanePoint NextLanePoint
    {
        set => nextLanePoint = value;
        get => nextLanePoint;
    }
    [SerializeField]
    private LanePoint previousLanePoint = null;
    public LanePoint PreviousLanePoint
    {
        set => previousLanePoint = value;
        get => previousLanePoint;
    }

    [SerializeField]
    private int targetLaneIndex;
    public int TargetLaneIndex => targetLaneIndex;

    [Header("Safety")]
    [SerializeField]
    private float safetyDistance = 10f;
    public float SafetyDistance => safetyDistance;

    [SerializeField]
    private bool isWaitingUserOfNextLanePoint = false;
    public bool IsWaitingUserOfNextLanePoint
    {
        set => isWaitingUserOfNextLanePoint = value;
        get => isWaitingUserOfNextLanePoint;
    }

    [SerializeField]
    private bool hasSafetyDistanceProblem = false;
    public bool HasSafetyDistanceProblem
    {
        set => hasSafetyDistanceProblem = value;
        get => hasSafetyDistanceProblem;
    }

    [SerializeField]
    private bool isWaitingTrafficLight = false;
    public bool IsWaitingTrafficLight
    {
        set => isWaitingTrafficLight = value;
        get => isWaitingTrafficLight;
    }

    public bool ShouldStop => IsWaitingUserOfNextLanePoint || HasSafetyDistanceProblem || isWaitingTrafficLight;

    public Vector3 MiddlePosition => transform.position + Vector3.up * 1;

    public float CurrentMoveSpeed
    {
        get
        {
            CarStateDrive d = (CarStateDrive)states[(int)CarStates.Drive];

            if (d == null) return 0;
            return d.CurrentMoveSpeed;
        }
    }

    private IEnumerator Start()
    {
        while (true)
        {
            if (currentLanePoint != null) break;
            yield return null;
        }

        Setup();
    }

    private bool setup = false;

    [SerializeField]
    private bool startWithPositioning = true;
    private void Setup()
    {
        NextLanePoint = CurrentLanePoint.GetNextLanePoint(TargetLaneIndex);
        NextLanePoint.RegisterUser(this.gameObject);

        transform.SetParent(null);
        if (startWithPositioning)
        {
            transform.position = CurrentLanePoint.Position;
            transform.rotation = Quaternion.LookRotation(NextLanePoint.Position - CurrentLanePoint.Position);
        }

        stateMachine.Setup(this, states[(int)CarStates.Drive]);

        if (states[(int)CarStates.Global] != null)
            stateMachine.SetGlobalState(states[(int)CarStates.Global]);

        ChangeState(CarStates.Drive);

        setup = true;
    }

    private void FixedUpdate()
    {
        if (!setup) return;

        stateMachine.Execute();
    }

    public void ChangeState(CarStates newState)
    {
        currentState = newState;
        stateMachine.ChangeState(states[(int)newState]);
    }

    public void ChangeTargetLane(int targetLaneIndex)
    {
        this.targetLaneIndex = targetLaneIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentLanePoint != null) return;

        if (other.TryGetComponent<LanePoint>(out LanePoint lanePoint))
            CurrentLanePoint = lanePoint;
    }

    /* Gizmo */
    private void OnDrawGizmos()
    {
        Gizmos.color = hasSafetyDistanceProblem ? Color.red : Color.green;
        Gizmos.DrawLine(MiddlePosition, MiddlePosition + (transform.forward * safetyDistance));

        Gizmos.color = isWaitingUserOfNextLanePoint ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position + transform.up * 3 + transform.forward * 2, 0.3f);

        if (nextLanePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(nextLanePoint.Position, 0.3f);
    }
}
