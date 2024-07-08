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
    public float maxSteerForce = 20;
    [SerializeField]
    public int maxVelocity = 10;
    [SerializeField]
    public float mass = 100;
    [Range(0, 1)]
    [SerializeField]
    public float rotateSpeed = 0.05f;

    private Enemy_Fly owner;
    private Vector3 OwnerPosition => owner.transform.position;
    private Vector3 OwnerLocalPosition => owner.transform.localPosition;

    public void SetMass(float value) => mass = value;

    public override void Enter(Enemy_Fly entity)
    {
        owner = entity;

        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        steerForce = Vector3.zero;
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
            entity.LookAt(transform.TransformDirection(velocity));
        }
    }

    private void Truncate(ref Vector3 vector, float max)
    {
        if (vector.magnitude > max)
        {
            vector = vector.normalized;
            vector *= max;
        }
    }

    private Vector3 Seek(Vector3 targetLocalPosition)
    {
        Vector3 desiredVelocity = (targetLocalPosition - OwnerLocalPosition).normalized * maxVelocity;

        Vector3 steerForce = desiredVelocity - velocity;

        Debug.DrawLine(OwnerPosition, transform.parent.TransformPoint(OwnerLocalPosition + velocity), Color.green);
        Debug.DrawLine(OwnerPosition, transform.parent.TransformPoint(OwnerLocalPosition + desiredVelocity), Color.blue);

        return steerForce;
    }

    private Vector3 Wander()
    {
        // If this enemy didn't arrive currentWanderPosition, it has to move to currentWanderPosition.
        if (Vector3.Distance(OwnerLocalPosition, owner.CurrentWanderPosition) > 0.5f) return Seek(owner.CurrentWanderPosition);

        // If it is last point, (If you arrived player's position)
        // Attack player
        owner.HasReachedCurrentWanderPosition();

        return Seek(owner.CurrentWanderPosition);
    }

    public override void Exit(Enemy_Fly entity) { }
}
