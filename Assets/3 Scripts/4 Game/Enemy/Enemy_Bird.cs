using System.Collections;
using Inca;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Bird : DamagableEnemy
{
    [Header("[Move]")]
    [SerializeField]
    private Vector3 originLocalPosition;
    [SerializeField]
    private float minLocalX;
    [SerializeField]
    private float maxLocalX;

    private float maxY = 0;

    private int currentMoveDirection = 1;
    [SerializeField]
    float moveTime = 2f;
    [SerializeField]
    private float moveDepthMin = 1f;
    [SerializeField]
    private float moveDepthMax = 1f;
    [SerializeField]
    private float moveDepth = 1f;
    [SerializeField]
    private float moveLengthMin = 1f;
    [SerializeField]
    private float moveLengthMax = 1f;
    [SerializeField]
    private float moveLength = 1f;
    [SerializeField]
    private float nextMoveDelayTime = 3f;

    [Header("[Attack]")]
    [SerializeField]
    private float attackTime;
    private float attackTimer;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpawnTransform;

    public override void Init(DetectedObject detectedObject = null)
    {
        Vector3 startPos = originLocalPosition;
        startPos.x = Random.Range(minLocalX, minLocalX + ((maxLocalX - minLocalX) / 2));
        transform.localPosition = startPos;
        maxY = transform.localPosition.y;

        attackTimer = 0;

        EnableRigidbody(false);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Attack] = new State_Attack(this);
        states[EnemyState.TakeDamage] = new State_TakeDamage(this);
        states[EnemyState.Die] = new State_Die(this);
        states[EnemyState.Global] = new State_Global(this);
    }

    /*-------------------- Move --------------------*/
    // This is called by the global state.
    private IEnumerator UpdateMove()
    {
        ChangeState(EnemyState.Move);

        float moveTimer = 0;

        Vector3 startPos = transform.localPosition;

        moveLength = Random.Range(moveLengthMin, moveLengthMax);
        if (currentMoveDirection > 0 && startPos.x + moveLength > maxLocalX)
            moveLength = maxLocalX - startPos.x;
        else if (currentMoveDirection < 0 && startPos.x + moveLength < minLocalX)
            moveLength = minLocalX - startPos.x;
        moveLength = Mathf.Abs(moveLength);

        moveDepth = Random.Range(moveDepthMin, moveDepthMax);

        while (moveTimer < moveTime)
        {
            moveTimer += Time.deltaTime;

            float progress = moveTimer / moveTime;

            Vector3 newPos = startPos;
            newPos.x += moveLength * progress * currentMoveDirection;
            newPos.y += -Mathf.Sin(progress * Mathf.PI) * moveDepth;

            if (newPos.x > maxLocalX) newPos.x = maxLocalX;
            else if (newPos.x < minLocalX) newPos.x = minLocalX;

            if (newPos.y > maxY) newPos.y = maxY;

            transform.localPosition = newPos;

            yield return new WaitForFixedUpdate();
        }

        currentMoveDirection *= -1;

        ChangeState(EnemyState.Idle);

        yield return new WaitForSeconds(nextMoveDelayTime);

        StartCoroutine(UpdateMove());
    }


    /*------------------- Attack -------------------*/
    private bool CanPlayAttackAnimation()
    {
        attackTimer += Time.deltaTime;
        return attackTimer > attackTime;
    }

    protected override void Attack()
    {
        base.Attack();

        GameObject clone = Instantiate(bulletPrefab, bulletSpawnTransform.transform.position, Quaternion.LookRotation(GGData.PlayerPosition));

        Bullet bullet = clone.GetComponent<Bullet>();
        bullet.Init();
        bullet.SetAttack(status.CurrentAttack);

        clone.transform.SetParent(IncaData.UserCarTransform);

        attackTimer = 0;
    }

    /*--------------------- FSM ---------------------*/
    public class State_Idle : EnemyState_Idle
    {
        Enemy_Bird owner;

        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Bird)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation())
                owner.ChangeState(EnemyState.Attack);
        }
    }

    public class State_Attack : EnemyState_Attack
    {
        Enemy_Bird owner;

        public State_Attack(Enemy entity) : base(entity)
            => owner = (Enemy_Bird)entity;

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

    public class State_TakeDamage : EnemyState_TakeDamage
    {
        Enemy_Bird owner;

        public State_TakeDamage(Enemy entity) : base(entity)
            => owner = (Enemy_Bird)entity;

        public override void OnFinishTakeDamageAnimation(Enemy entity)
        {
            // base.OnFinishTakeDamageAnimation(entity);
            owner.ChangeState(EnemyState.Move);
        }
    }

    public class State_Die : EnemyState_Die
    {
        Enemy_Bird owner;

        public State_Die(Enemy entity) : base(entity)
            => owner = (Enemy_Bird)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.StopAllCoroutines();
            owner.EnableRigidbody(true);
            owner.LookAtPlayer(false);
        }
    }

    public class State_Global : EnemyState_Global
    {
        Enemy_Bird owner;

        public State_Global(Enemy entity) : base(entity)
            => owner = (Enemy_Bird)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.StartCoroutine(owner.UpdateMove());
            owner.LookAtPlayer(true);
        }
    }
}
