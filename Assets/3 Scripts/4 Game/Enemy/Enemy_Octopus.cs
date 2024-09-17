using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Octopus : DamagableEnemy
{
    private bool isAttacking = false;

    [Header("[[Octopus]]")]
    [Header("[Attack]")]
    [SerializeField]
    private float AttackDelay;
    private float AttackTimer;

    [Header("[Attack: Mini Octopus]")]
    [SerializeField]
    private Enemy_Octopus_Mini miniOctopusPrefab;
    [SerializeField]
    private int minMiniOctopusSpawnCount;
    [SerializeField]
    private int maxMiniOctopusSpawnCount;
    [SerializeField]
    private Transform miniOctopusAttackSpawnTransforms;

    [Header("[Attack: Bubble]")]
    [SerializeField]
    private GameObject bubbleBulletPrefab;
    [SerializeField]
    private int minBubbleAttackSpawnCount;
    [SerializeField]
    private int maxBubbleAttackSpawnCount;
    [SerializeField]
    private Transform[] bulletAttackSpawnTransforms;

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Attack] = new State_Attack_MiniOctopus(this);
        states[EnemyState.Attack1] = new State_Attack_Bubble(this);
    }

    private bool CanPlayAttackAnimation()
    {
        AttackTimer += Time.deltaTime;
        return AttackTimer >= AttackDelay;
    }

    private void Attack_MiniOctopus()
    {
        base.Attack();

        int cnt = Random.Range(minMiniOctopusSpawnCount, maxMiniOctopusSpawnCount + 1);

        for (int i = 0; i < cnt; i++)
        {
            Enemy_Octopus_Mini clone
                = Instantiate(miniOctopusPrefab, miniOctopusAttackSpawnTransforms.position, Quaternion.identity);

            clone.Init(null);
        }

        AttackTimer = 0;
    }

    private void Attack_Bubble()
    {
        base.Attack();

        foreach (var tf in bulletAttackSpawnTransforms)
        {
            int spawnCnt = Random.Range(minBubbleAttackSpawnCount, maxBubbleAttackSpawnCount + 1);
            for (int i = 0; i < spawnCnt; ++i)
            {
                GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bubbleBulletPrefab, tf.position);
                bullet.transform.SetParent(IncaData.UserCarTransform);
                bullet.GetComponent<Enemy>().Init();
            }
        }

        AttackTimer = 0;
    }

    private class State_Idle : EnemyState_Idle
    {
        private Enemy_Octopus owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);
            if (owner.CanPlayAttackAnimation())
            {
                if (Random.Range(0, 2) == 0)
                    owner.ChangeState(EnemyState.Attack);
                else
                    owner.ChangeState(EnemyState.Attack1);
            }
        }
    }

    private class State_Attack_MiniOctopus : EnemyState_Attack
    {
        private Enemy_Octopus owner;
        public State_Attack_MiniOctopus(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus)entity;

        public override void Attack(Enemy entity)
        {
            owner.Attack_MiniOctopus();
            owner.CanAttack(false);
            owner.PlayAudioClip(AudioTypeForChracter.Attack1);
        }
    }

    private class State_Attack_Bubble : EnemyState_Attack
    {
        private Enemy_Octopus owner;
        public State_Attack_Bubble(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus)entity;

        public override void Enter(Enemy entity)
        {
            owner.CanAttack(false);
            owner.PlayAnimationByName(EnemyAnimation.Attack1);
        }

        public override void Execute(Enemy entity)
        {
            if (owner.CanAttack())
                Attack(owner);
            if (owner.IsAnimationFinished(EnemyAnimation.Attack1))
                OnFinishAttackAnimation(owner);

        }

        public override void Attack(Enemy entity)
        {
            owner.Attack_Bubble();
            owner.CanAttack(false);
            owner.PlayAudioClip(AudioTypeForChracter.Attack1);
        }
    }
}
