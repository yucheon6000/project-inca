using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Octopus_Mini : DamagableEnemy
{
    [Header("[[Mini Octopus]]")]
    [Header("[Fly]")]
    [SerializeField]
    private float projectileSpeed = 10f; // 투사체 속도
    private float gravity = -9.81f; // 중력 값
    [SerializeField]
    private float maxTargettingJitter = 1f;
    [SerializeField]
    private Transform modelTransform;

    [Header("[Attck]")]
    [SerializeField]
    private float attackDelayMin;
    [SerializeField]
    private float attackDelayMax;
    private float attackDelay;
    private float attackTimer;

    [Header("[Ink]")]
    [SerializeField]
    private ScaleEffector inkScaleEffector;
    [SerializeField]
    private float inkScalePerAttack = 0.2f;

    private new SphereCollider collider;

    protected override void GetMyComponents()
    {
        base.GetMyComponents();
        collider = GetComponent<SphereCollider>();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        collider.isTrigger = true;
        inkScaleEffector.transform.localScale = Vector3.zero;

        ResetAttackTimer();

        LookAtPlayer(false);

        StartCoroutine(ColliderTrigger());

        base.Init(detectedObject);
    }

    private IEnumerator ColliderTrigger()
    {
        yield return new WaitForSeconds(1f);
        collider.isTrigger = false;
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();

        SetStartState(EnemyState.Fly);

        states[EnemyState.Fly] = new State_Fly(this);
        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Attack] = new State_Attack(this);
    }

    private void StartFly()
    {
        // 플레이어의 현재 위치와 속도
        Vector3 playerPosition = IncaData.UserCarPosition;
        Vector3 playerVelocity = IncaData.UserCarVelocity;
        Vector3 startPosition = transform.position;

        // 예측 사격을 위한 시간 계산
        float timeToImpact = CalculateTimeToImpact(playerPosition, startPosition, projectileSpeed);

        // 예측된 플레이어 위치 계산
        Vector3 predictedPlayerPosition = playerPosition + playerVelocity * timeToImpact;
        predictedPlayerPosition += Random.insideUnitSphere * maxTargettingJitter;

        // 포물선을 그리는 발사 속도와 각도 계산
        Vector3 launchVelocity = CalculateLaunchVelocity(startPosition, predictedPlayerPosition, timeToImpact);

        // 투사체 발사
        rigidbody.velocity = launchVelocity; // 계산된 속도를 적용
    }

    private void Update()
    {
        if (CurrentState != EnemyState.Fly) return;

        // 물체의 down 방향(-Y)을 속도 벡터를 향하도록 회전시킴
        Quaternion targetRotation = Quaternion.LookRotation(-rigidbody.velocity.normalized);
        // 회전을 부드럽게 적용하려면 Lerp 또는 Slerp를 사용할 수 있습니다.
        modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // 충돌 시점까지 걸리는 시간 계산
    float CalculateTimeToImpact(Vector3 playerPosition, Vector3 firePointPosition, float projectileSpeed)
    {
        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(playerPosition, firePointPosition);

        // 투사체가 목표에 도달할 시간을 추정 (간단히 거리/속력으로 계산)
        float timeToImpact = distance / projectileSpeed;

        return timeToImpact;
    }

    // 포물선 경로로 목표 위치에 도달할 수 있도록 속도 계산
    Vector3 CalculateLaunchVelocity(Vector3 startPoint, Vector3 targetPoint, float timeToImpact)
    {
        // 시작 위치에서 목표 위치로의 xz 방향 벡터
        Vector3 direction = targetPoint - startPoint;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        // xz축 속도 계산 (단순 거리/시간)
        float distanceXZ = directionXZ.magnitude;
        float velocityXZ = distanceXZ / timeToImpact;

        // y축 속도 계산 (포물선의 최고점까지 올라가는 속도 + 중력에 의한 속도)
        float heightDifference = targetPoint.y - startPoint.y;
        float velocityY = (heightDifference / timeToImpact) - (0.5f * gravity * timeToImpact);

        // 최종 발사 속도 벡터 (xz 평면 속도 + y축 속도)
        Vector3 launchVelocity = directionXZ.normalized * velocityXZ;
        launchVelocity.y = velocityY;

        return launchVelocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (CurrentState != EnemyState.Fly) return;
        if (other.gameObject.CompareTag("Windshield") == false) return;

        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        // rigidbody.velocity = Vector3.zero;

        collider.isTrigger = true;

        // transform.rotation = Quaternion.LookRotation(-other.contacts[0].normal, Vector3.down);

        transform.SetParent(IncaData.UserCarTransform);

        LookAtPlayer(true);

        ChangeState(EnemyState.Idle);
    }

    private bool CanPlayAttackAnimation()
    {
        attackTimer += Time.deltaTime;
        return attackTimer >= attackDelay;
    }

    protected override void Attack()
    {
        base.Attack();
        inkScaleEffector.PlayFromCurrentScale(0.5f, inkScaleEffector.CurrentScale + Vector3.one * inkScalePerAttack, ScaleEffector.EaseOutCurve);


    }

    private void ResetAttackTimer()
    {
        attackDelay = Random.Range(attackDelayMin, attackDelayMax);
        attackTimer = 0;
    }

    private class State_Fly : EnemyState_Move
    {
        Enemy_Octopus_Mini owner;
        public State_Fly(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus_Mini)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.PlayAnimationByName(EnemyAnimation.Idle);
            owner.PlayAppearEffect();
            owner.StartFly();
        }
    }

    private class State_Idle : EnemyState_Idle
    {
        Enemy_Octopus_Mini owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus_Mini)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanPlayAttackAnimation())
                owner.ChangeState(EnemyState.Attack);
        }
    }

    private class State_Attack : EnemyState_Attack
    {
        Enemy_Octopus_Mini owner;
        public State_Attack(Enemy entity) : base(entity)
            => owner = (Enemy_Octopus_Mini)entity;

        public override void OnFinishAttackAnimation(Enemy entity)
        {
            base.OnFinishAttackAnimation(entity);
            owner.ResetAttackTimer();
        }
    }
}
