using Inca;
using UnityEngine;

public class Enemy_Chameleon : DamagableEnemy
{
    [Header("[[Chameleon]]")]
    [Header("[Attack]")]
    [SerializeField]
    private float attackDelayTime;
    private float attackTimer;

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

    private void UpdateVelocity()
    {
        rigidbody.velocity = transform.forward * Vector3.Dot(IncaData.UserCarVelocity, transform.forward);
    }

    private bool CanPlayAttackAnimation()
    {
        attackTimer += Time.deltaTime;
        return attackTimer > attackDelayTime;
    }

    public override bool CanAttack() => true;

    protected override void Attack()
    {
        base.Attack();
        tongue.Init(null);
        attackTimer = 0;
    }

    private class State_Idle : EnemyState_Idle
    {
        Enemy_Chameleon owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation())
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
            owner.ChangeState(EnemyState.Idle);
        }
    }

    private class State_Move : EnemyState_Global
    {
        Enemy_Chameleon owner;
        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            owner.UpdateVelocity();
        }
    }
}
