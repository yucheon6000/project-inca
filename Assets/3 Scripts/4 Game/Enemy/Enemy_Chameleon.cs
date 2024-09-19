using Inca;
using UnityEngine;

public class Enemy_Chameleon : DamagableEnemy
{
    [Header("[[Chameleon]]")]
    [Header("[Move]")]
    [SerializeField]
    private float startMoveDistance = 3f;

    [Header("[Attack]")]
    [SerializeField]
    private float attackDelayTime;
    private float attackTimer;

    [Header("[Aniamtion]")]
    [SerializeField]
    private float animationSpeedMps;

    private Enemy_Chameleon_Tongue tongue;

    protected override void GetMyComponents()
    {
        base.GetMyComponents();
        tongue = GetComponentInChildren<Enemy_Chameleon_Tongue>();
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Attack] = new State_Attack(this);

        states[EnemyState.Global] = new State_Move(this);
    }

    private bool IsWithinMoveDistance() => Vector3.Distance(transform.position, IncaData.UserCarPosition) <= startMoveDistance;

    private void UpdateVelocity()
    {
        if (IsWithinMoveDistance() == false)
            rigidbody.velocity = Vector3.zero;
        else
            rigidbody.velocity = transform.forward * Vector3.Dot(IncaData.UserCarVelocity, transform.forward);
    }

    protected override void PlayAnimationByName(string animationName)
    {
        base.PlayAnimationByName(animationName);

        animator.Play(animationName, 1, 0);
    }

    private void UpdateAnimationSpeed()
    {
        float animSpeed = rigidbody.velocity.magnitude * animationSpeedMps;
        animator.SetFloat("MoveSpeedMultiplier", animSpeed);
    }

    private bool CanPlayAttackAnimation()
    {
        attackTimer += Time.deltaTime;
        return attackTimer > attackDelayTime;
    }

    protected override void Attack()
    {
        base.Attack();
        tongue.Init(null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startMoveDistance);
    }

    private class State_Idle : EnemyState_Idle
    {
        Enemy_Chameleon owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation() && owner.IsWithinMoveDistance())
                owner.ChangeState(EnemyState.Attack);
        }
    }

    private class State_Attack : EnemyState_Attack
    {
        Enemy_Chameleon owner;
        public State_Attack(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon)entity;

        public override void Attack(Enemy entity)
        {
            base.Attack(entity);
        }

        public override void Execute(Enemy entity)
        {
            // base.Execute(entity)     // prevent to check animation in layer 0.

            if (owner.CanAttack())
                Attack(owner);
            if (owner.IsAnimationFinished(EnemyAnimation.Attack0, 1))
                OnFinishAttackAnimation(owner);
        }

        public override void OnFinishAttackAnimation(Enemy entity)
        {
            base.OnFinishAttackAnimation(entity);
            owner.attackTimer = 0;
        }
    }

    private class State_Move : EnemyState_Global
    {
        Enemy_Chameleon owner;
        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.animator.Play(EnemyAnimation.Move);
        }

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            owner.UpdateVelocity();
            owner.UpdateAnimationSpeed();
        }
    }
}
