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

    public bool ShouldStop => IsWaitingUserOfNextLanePoint || HasSafetyDistanceProblem;

    public Vector3 MiddlePosition => transform.position + Vector3.up * 1;

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

        stateMachine.Setup(this, states[(int)CarStates.Drive]);

        if (states[(int)CarStates.Global] != null)
            stateMachine.SetGlobalState(states[(int)CarStates.Global]);

        ChangeState(CarStates.Drive);
    }

    private void Update()
    {
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
