using System.Collections;
using System.Collections.Generic;
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

    [Header("Wander")]
    [SerializeField]
    private float currentWanderTime = 0;
    [SerializeField]
    private float maxWanderTime = 5;
    [SerializeField]
    private float wanderRadius = 3;
    [SerializeField]
    private Vector3 wanderPosition;
    [SerializeField]
    private Vector3 wanderCircleCenter;
    [SerializeField]
    private float wanderPosMinX;
    [SerializeField]
    private float wanderPosMaxX;

    private Enemy_Fly owner;
    private Vector3 OwnerPosition
    {
        get => owner.transform.position;
        set => owner.transform.position = value;
    }

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

        transform.position += velocity * Time.fixedDeltaTime;

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

    private int prevSignY = -1;

    [SerializeField]
    private float wanderDistance = 10;
    [SerializeField]
    private int wanderJitter = 0;
    [SerializeField]
    private int wanderJitterMin = -30;
    [SerializeField]
    private int wanderJitterMax = 30;
    private Vector3 Wander()
    {

        currentWanderTime += Time.fixedDeltaTime;

        wanderCircleCenter = OwnerPosition + velocity.normalized * wanderDistance;

        // Set new wanderPosition;
        if (Vector3.Distance(OwnerPosition, wanderPosition) < 0.5f || currentWanderTime >= maxWanderTime)
        {
            currentWanderTime = 0;

            if (velocity.x > 0)
                wanderJitter = Random.Range(wanderJitterMin - (int)(transform.rotation.eulerAngles.x - 360), wanderJitterMax - (int)(transform.rotation.eulerAngles.x - 360));
            else
                wanderJitter = Random.Range(wanderJitterMin - (int)transform.rotation.eulerAngles.x, wanderJitterMax - (int)transform.rotation.eulerAngles.x);

            if (transform.position.x < wanderPosMinX)
            {
                wanderJitterMin = -30;
                wanderJitterMax = 30;
                velocity.x = Mathf.Abs(velocity.x);
                velocity.y = 0;
            }
            else if (transform.position.x > wanderPosMaxX)
            {
                wanderJitterMin = 180 - 30;
                wanderJitterMax = 180 + 30;
                velocity.x = -Mathf.Abs(velocity.x);
                velocity.y = 0;
            }
            velocity.z = -Mathf.Abs(velocity.z);

            Vector3 vel = velocity;
            vel.y *= -1;

            wanderCircleCenter = OwnerPosition + vel.normalized * wanderDistance;

            prevSignY *= -1;

            Debug.DrawLine(wanderCircleCenter, wanderCircleCenter + SetAngle(wanderRadius, wanderJitterMin));
            Debug.DrawLine(wanderCircleCenter, wanderCircleCenter + SetAngle(wanderRadius, wanderJitterMax));

            wanderPosition = wanderCircleCenter + SetAngle(wanderRadius, wanderJitter);
            wanderPosition.y = Mathf.Clamp(wanderPosition.y, 1.77f, 2.9f);
            wanderPosition.x = Mathf.Clamp(wanderPosition.x, wanderPosMinX - 0.5f, wanderPosMaxX + 0.5f);
            wanderPosition.z -= Random.Range(0, 5);
        }

        return Seek(wanderPosition);
    }

    private Vector3 SetAngle(float radius, float angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        position.y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        return position;
    }

    public override void Exit(Enemy_Fly entity)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(wanderCircleCenter, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wanderPosition, 0.2f);
    }
}
