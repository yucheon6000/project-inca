using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Bear : DamagableEnemy
{
    [Header("[[Bear]]")]
    [Header("[Attack]")]
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float attackDelayTime;
    private float attackTimer;

    [Header("[Honeycomb]")]
    [SerializeField]
    private Enemy hoenycomb;

    protected override void InitStateMachine()
    {
        SetStartState(EnemyState.Idle);

        base.InitStateMachine();

        states[EnemyState.Idle] = new State_Idle(this);
    }

    private bool CanPlayAttackAnimation()
    {
        attackTimer += Time.deltaTime;
        return attackTimer >= attackDelayTime;
    }

    protected override void Attack()
    {
        GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab, transform.position);
        bullet.transform.position = transform.position;
        bullet.GetComponent<Enemy>().Init();
        attackTimer = 0;
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();
        hoenycomb.TakeDamage(1);
    }

    protected override void OnDeath()
    {
        transform.SetParent(null);
        base.OnDeath();
    }

    private class State_Idle : EnemyState_Idle
    {
        private Enemy_Bear owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Bear)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation())
                owner.ChangeState(EnemyState.Attack);
        }
    }
}
