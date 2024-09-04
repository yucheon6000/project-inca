using Inca;
using UnityEngine;

public class Bullet_BearHoneyBall : DamagableEnemy
{
    [SerializeField]
    private int bounceCount = 4;
    private int currentBounceCount = 0;

    [SerializeField]
    private float bounceProgressSpeed;

    private Vector3 initialPosition;
    private float initialY;

    private Vector3 finalPosition;
    private float finalY;

    private float distance;

    private float gapDist;     // Distance of one bounce
    private float gapY;

    private Vector3 bounceStartZ;
    private Vector3 bounceEndZ;

    private float bounceProgress = 0;

    [SerializeField]
    private float hitPower = 10;

    [SerializeField]
    private Transform modelTransform;

    public override void Init(DetectedObject detectedObject = null)
    {
        currentBounceCount = 0;

        gameObject.transform.SetParent(IncaData.UserCarTransform);
        transform.LookAt(IncaData.UserCarTransform);

        initialY = transform.localPosition.y;
        initialPosition = transform.localPosition;

        Vector3 userPos = transform.parent.InverseTransformPoint(GGData.PlayerPosition);
        finalY = userPos.y;
        finalPosition = userPos;

        initialPosition.y = finalY;
        finalPosition.y = finalY;

        distance = Vector3.Distance(initialPosition, finalPosition);

        gapY = (initialY - finalY) / bounceCount;
        gapDist = distance / bounceCount;

        bounceStartZ = initialPosition - transform.forward * gapDist / 2;
        bounceEndZ = initialPosition + transform.forward * gapDist * (currentBounceCount + 1);

        bounceProgress = 0.5f;

        EnableRigidbody(false);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        SetStartState(EnemyState.Move);

        base.InitStateMachine();

        states[EnemyState.Move] = new State_Move(this);
        states[EnemyState.Die] = new State_Die(this);
    }

    private bool HasBounceFinished()
        => currentBounceCount >= bounceCount;

    private void Move()
    {
        if (currentBounceCount >= bounceCount) return;

        bounceProgress += bounceProgressSpeed * Time.fixedDeltaTime;

        if (bounceProgress >= 1)
        {
            bounceProgress = 0;
            currentBounceCount++;
            bounceStartZ = bounceEndZ;
            bounceEndZ = initialPosition + transform.forward * gapDist * (currentBounceCount + 1);
        }

        float s = Mathf.Sin(bounceProgress * 180 * Mathf.Deg2Rad);
        Vector3 newPos = Vector3.Lerp(bounceStartZ, bounceEndZ, bounceProgress);
        newPos.y = s * (initialY - gapY * currentBounceCount);

        transform.rotation.SetLookRotation(newPos - transform.localPosition);

        transform.localPosition = newPos;
    }

    private class State_Move : EnemyState_Move
    {
        private Bullet_BearHoneyBall owner;
        public State_Move(Enemy entity) : base(entity)
            => owner = (Bullet_BearHoneyBall)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            owner.Move();

            if (owner.HasBounceFinished())
            {
                Player.Instance.TakeDamage(owner.status.CurrentAttack);
                owner.DeactivateGameObject();
            }
        }
    }

    private class State_Die : EnemyState_Die
    {
        private Bullet_BearHoneyBall owner;
        public State_Die(Enemy entity) : base(entity)
            => owner = (Bullet_BearHoneyBall)entity;

        public override void Enter(Enemy entity)
        {
            owner.transform.SetParent(null);

            // Stop bouncing this ball.
            owner.currentBounceCount = int.MaxValue;

            // Deflect this ball in the direction of the gunshot.
            owner.EnableRigidbody(true);

            Vector3 dir = owner.transform.position - GGData.PlayerPosition;
            dir = dir.normalized * 4 + Random.onUnitSphere * 2;
            dir.Normalize();
            dir.y = Mathf.Abs(dir.y);

            owner.rigidbody.AddForce(dir * owner.hitPower, ForceMode.Impulse);

            owner.modelTransform.rotation.SetLookRotation(dir);

            // Deactivate this gameobject after 3 sec.
            owner.Invoke(nameof(DeactivateGameObject), 3f);

            base.Enter(entity);
        }
    }
}
