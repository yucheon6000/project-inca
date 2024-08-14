using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Kindergartner : DamagableEnemy
{
    [Header("[[Kindergartner]]")]
    [Header("[Move]")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float moveDistance;
    private Vector3 startPosition;

    [Header("[Scale]")]
    [SerializeField]
    private float minScale;
    [SerializeField]
    private float maxScale;

    public override void Init(DetectedObject detectedObject = null)
    {
        startPosition = transform.position;
        transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        states[EnemyState.Spawn] = new State_Spawn(this);
        states[EnemyState.Move] = new State_Move(this);
        states[EnemyState.Die] = new State_Die(this);
    }

    private void PlayAnimation()
    {
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_MOVE);
    }

    private void Move()
    {
        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
    }

    private bool CanMove()
    {
        return Vector3.Distance(transform.position, startPosition) < moveDistance;
    }

    public class State_Spawn : EnemyState_Spawn
    {
        private Enemy_Kindergartner owner;

        public State_Spawn(Enemy entity) : base(entity)
            => owner = (Enemy_Kindergartner)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);

            entity.LookAtPlayer(false);
            entity.Invoke(nameof(PlayAnimation), Random.Range(0f, 1f));
        }

        public override void OnAppear(Enemy entity)
        {
            // base.OnAppear(entity);
            entity.ChangeState(EnemyState.Move);
        }
    }

    public class State_Move : EnemyState_Move
    {
        private Enemy_Kindergartner owner;

        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_Kindergartner)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanMove())
                owner.Move();

            else
                owner.DeactivateGameObject();
        }
    }

    public class State_Die : EnemyState_Die
    {
        private Enemy_Kindergartner owner;

        public State_Die(Enemy entity) : base(entity)
            => owner = (Enemy_Kindergartner)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);

            entity.LookAtPlayer(true);
            entity.transform.Rotate(0, Random.Range(-30f, 30f), 0);
        }
    }
}
