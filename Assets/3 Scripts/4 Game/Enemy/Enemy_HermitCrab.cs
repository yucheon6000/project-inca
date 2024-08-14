using System.Collections.Generic;
using UnityEngine;
using Inca;

public class Enemy_HermitCrab : DamagableEnemy
{
    // Move
    [Header("[Move]")]
    [SerializeField]
    private float moveTriggerDistance;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private List<Transform> transformByLaneIndex;
    private bool arrivedTargetLane = false;

    // Attack
    [Header("[Attack (Hide)]")]
    [SerializeField]
    private float attackTriggerDistance;

    // Fly
    [Header("[Fly]")]
    [SerializeField]
    private Transform explosionTf;
    [SerializeField]
    private float exFor = 1000;
    [SerializeField]
    private float exRa = 10;
    [SerializeField]
    private float exMo = 1;

    public override void Init(DetectedObject detectedObject = null)
    {
        arrivedTargetLane = false;

        EnableRigidbody(false);
        LookAtPlayer(true);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Move] = new State_Move(this);
        states[EnemyState.Attack] = new State_Attack(this);
        states.Add(EnemyState.Fly, new State_Fly(this));
    }

    private Vector3 GetMoveTargetPoint()
        => transformByLaneIndex[IncaData.UserCarLaneIndex].position;

    private Vector3 MoveToTargetPoint(Vector3 targetPoint)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.fixedDeltaTime);

        return transform.position;
    }

    protected override void OnDeath()
    {
        // This enemy has attacked.
        if (DistanceToUserCar < attackTriggerDistance)
            ChangeState(EnemyState.Fly);
        else
            ChangeState(EnemyState.Die);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsDead) return;

        if (other.TryGetComponent<DetectedUser>(out DetectedUser car))
        {
            ChangeState(EnemyState.Fly);
            Player.Instance.TakeDamage(2);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, moveTriggerDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDistance);
    }

    private class State_Idle : EnemyState_Idle
    {
        private Enemy_HermitCrab owner;

        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_HermitCrab)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (!owner.arrivedTargetLane && owner.DistanceToUserCar <= owner.moveTriggerDistance)
                owner.ChangeState(EnemyState.Move);
            else if (owner.arrivedTargetLane && owner.DistanceToUserCar <= owner.attackTriggerDistance)
                owner.ChangeState(EnemyState.Attack);
        }
    }

    private class State_Move : EnemyState_Move
    {
        private Enemy_HermitCrab owner;

        Vector3 targetPoint;

        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_HermitCrab)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.LookAtPlayer(true);

            targetPoint = owner.GetMoveTargetPoint();
        }

        public override void Execute(Enemy entity)
        {
            if (Vector3.Distance(entity.transform.position, targetPoint) <= 0.001f)
            {
                owner.ChangeState(EnemyState.Idle);
                owner.arrivedTargetLane = true;
            }
            else
                owner.MoveToTargetPoint(targetPoint);
        }

        public override void Exit(Enemy entity)
        {
            owner.LookAtPlayer(false);
        }
    }

    private class State_Attack : EnemyState_Attack
    {
        private Enemy_HermitCrab owner;

        public State_Attack(Enemy entity) : base(entity)
            => owner = (Enemy_HermitCrab)entity;

        public override void OnFinishAttackAnimation(Enemy entity)
        {
            // base.OnFinishAttackAnimation(entity);    // Prevent the state from changing to idle.
        }
    }
    private class State_Fly : IEnemyState
    {
        private Enemy_HermitCrab owner;

        public State_Fly(Enemy entity)
            => owner = (Enemy_HermitCrab)entity;

        public void Enter(Enemy entity)
        {
            owner.LookAtPlayer(false);

            owner.EnableRigidbody(true);
            owner.rigidbody.AddExplosionForce(owner.exFor, owner.explosionTf.position, owner.exRa, 1f);

            owner.ForceKill();

            owner.PlayAnimationByName("Attack");

            owner.PlayDisappearEffect(owner.DeactivateGameObject);
        }

        public void Execute(Enemy entity) { }

        public void Exit(Enemy entity) { }
    }
}
