using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Shotput : DamagableEnemy
{
    [Header("[[Shotput]]")]
    [Header("[Attack]")]
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float attackTimeMin;
    [SerializeField]
    private float attackTimeMax;
    [SerializeField]
    private float attackTime;
    private float attackTimer;

    [Space]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpawnTransform;

    public override void Init(DetectedObject detectedObject = null)
    {
        attackTime = Random.Range(attackTimeMin, attackTimeMax);
        attackTimer = 0;

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Attack] = new State_Attack(this);
        states[EnemyState.Global] = new State_Global(this);
    }

    private void LookAtPlayer()
    {
        transform.LookAt(GGData.PlayerPosition, Vector3.up);
    }

    private bool CanPlayAttackAnimation()
    {
        if (Vector3.Distance(transform.position, GGData.PlayerPosition) > attackDistance) return false;

        attackTimer += Time.deltaTime;

        return attackTimer > attackTime;
    }

    protected override void Attack()
    {
        if (IsDead) return;
        base.Attack();

        GameObject bulletClone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab);
        bulletClone.transform.SetPositionAndRotation(bulletSpawnTransform.position, Quaternion.LookRotation(GGData.PlayerPosition));
        Vector3 dir = (GGData.PlayerPosition - bulletSpawnTransform.position);
        bulletClone.GetComponent<Bullet>().Setup(dir);

        attackTime = Random.Range(attackTimeMin, attackTimeMax);
        attackTimer = 0;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    /*--------------------- FSM ---------------------*/
    public class State_Idle : EnemyState_Idle
    {
        Enemy_Shotput owner;

        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Shotput)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation())
                owner.ChangeState(EnemyState.Attack);
        }
    }

    public class State_Attack : EnemyState_Attack
    {
        Enemy_Shotput owner;

        public State_Attack(Enemy entity) : base(entity)
            => owner = (Enemy_Shotput)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.CanAttack(false);
        }

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (entity.CanAttack())
                Attack(entity);
        }
    }

    public class State_Global : EnemyState_Global
    {
        Enemy_Shotput owner;

        public State_Global(Enemy entity) : base(entity)
            => owner = (Enemy_Shotput)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            owner.LookAtPlayer();
        }
    }
}
