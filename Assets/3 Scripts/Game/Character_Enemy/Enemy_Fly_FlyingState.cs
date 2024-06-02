using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Fly_FlyingState : State<Enemy_Fly>
{
    private Vector3 velocity;       // 속력
    private Vector3 acceleration;   // 가속도
    private Vector3 steerForce;     // 조종 힘

    [SerializeField]
    public int maxSteerForce = 20;
    [SerializeField]
    public int maxVelocity = 10;
    [SerializeField]
    public float mass = 100;
    [Range(0, 1)]
    [SerializeField]
    public float rotateSpeed = 0.05f;

    [Header("Wander")]
    private List<Vector3> wanderPositions;
    [SerializeField]
    private int wanderPositionIndex = 0;
    [SerializeField]
    private Vector3 currentWanderPosition;

    [Space]
    [SerializeField]
    private int wanderPositionCount = 5;
    [SerializeField]
    private Vector3 wanderPositionRangeMin;
    [SerializeField]
    private Vector3 wanderPositionRangeMax;
    [SerializeField]
    private bool flipY = false;

    [Header("Children")]
    [SerializeField]
    private bool isChild = false;
    private bool IsParent => !isChild;
    [SerializeField]
    private List<Enemy_Fly_FlyingState> children = new List<Enemy_Fly_FlyingState>();

    private Enemy_Fly owner;
    private Vector3 OwnerPosition
    {
        get => owner.transform.localPosition;
        set => owner.transform.localPosition = value;
    }

    public override void Enter(Enemy_Fly entity)
    {
        owner = entity;

        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        steerForce = Vector3.zero;

        if (IsParent)
        {
            SetWanderPositions(GetWanderPositions(wanderPositionCount));

            // Set children's wanderPositions.
            // Children have to pass parent's start position.
            List<Vector3> wanderPositionsForChildren = new List<Vector3>(wanderPositions);
            wanderPositionsForChildren.Insert(0, this.transform.localPosition);

            foreach (var child in children)
                child.SetWanderPositions(wanderPositionsForChildren);
        }
    }

    public void SetWanderPositions(List<Vector3> wanderPositions)
    {
        this.wanderPositions = wanderPositions;
        currentWanderPosition = wanderPositions[wanderPositionIndex];
    }

    public override void Execute(Enemy_Fly entity)
    {
        steerForce = Wander();
        Truncate(ref steerForce, maxSteerForce);

        acceleration = steerForce / 10;

        velocity += acceleration;
        Truncate(ref velocity, maxVelocity);

        transform.localPosition += velocity * Time.fixedDeltaTime;

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), 0.05f);
        }
    }

    private void Truncate(ref Vector3 vector, int max)
    {
        if (vector.magnitude > max)
        {
            vector = vector.normalized;
            vector *= max;
        }
    }

    private Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = (targetPosition - OwnerPosition).normalized * maxVelocity;

        Vector3 steerForce = desiredVelocity - velocity;

        Debug.DrawLine(OwnerPosition, OwnerPosition + velocity, Color.green);
        Debug.DrawLine(OwnerPosition, OwnerPosition + desiredVelocity, Color.blue);

        return steerForce;
    }

    private List<Vector3> GetWanderPositions(int count)
    {
        List<Vector3> result = new List<Vector3>();

        float gapZ = (wanderPositionRangeMax.z - wanderPositionRangeMin.z) / (count - 1);

        for (int i = 0; i < count - 1; ++i)    // count-1: The last point is player's position.
        {
            float x = Random.Range(wanderPositionRangeMin.x, wanderPositionRangeMax.x);

            float y = 0;
            if (flipY) y = i % 2 == 0 ? wanderPositionRangeMax.y : wanderPositionRangeMin.y + Random.Range(-0.5f, 0.5f);
            else y = i % 2 == 0 ? wanderPositionRangeMin.y : wanderPositionRangeMax.y + Random.Range(-0.5f, 0.5f);

            float z = wanderPositionRangeMax.z - gapZ * i;

            Vector3 wanderPos = new Vector3(x, y, z);

            result.Add(wanderPos);
        }

        // Add player's local position.
        result.Add(IncaData.PlayerPosition - transform.parent.transform.position);

        return result;
    }

    private Vector3 Wander()
    {
        // If this enemy didn't arrive currentWanderPosition, it has to move to currentWanderPosition.
        if (Vector3.Distance(OwnerPosition, currentWanderPosition) > 0.5f) return Seek(currentWanderPosition);

        // If it is last point, (If you arrived player's position)
        // Attack player
        if (wanderPositionIndex == wanderPositions.Count)
        {
            Player.Instance.Hit(1);
            owner.Hit(100);
        }
        else
        {
            currentWanderPosition = wanderPositions[wanderPositionIndex];
            wanderPositionIndex++;
        }

        return Seek(currentWanderPosition);
    }

    public override void Exit(Enemy_Fly entity) { }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentWanderPosition, 0.2f);
    }
}
