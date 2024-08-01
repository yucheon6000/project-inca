using System.Collections.Generic;
using UnityEngine;
using Inca;

public class Enemy_HermitCrab : DamagableEnemy
{
    public enum State { Idle = 0, Move, Fall, Fly }
    [SerializeField]
    private State currentState = State.Idle;
    private Dictionary<State, IState<Enemy_HermitCrab>> states;
    private StateMachine<Enemy_HermitCrab> stateMachine;

    private Vector3 originPos;

    // Move
    [Space]
    [SerializeField]
    private float followDistance;
    [SerializeField]
    private float followTime;
    private bool arrivedTargetLane = false;
    [SerializeField]
    private List<Transform> transformByLaneIndex;

    // Fall (Attack)
    [Space]
    [SerializeField]
    private float attackDistance;
    [SerializeField]
    private float fallSpeed;
    private bool isFall = false;        // is falling or finished falling

    // Fly
    [Space]
    [SerializeField]
    private Transform explosionTf;
    [SerializeField]
    private float exFor = 1000;
    [SerializeField]
    private float exRa = 10;
    [SerializeField]
    private float exMo = 1;

    [SerializeField]
    private LookAtPlayer lookAtPlayer;
    private Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();

        states = new Dictionary<State, IState<Enemy_HermitCrab>>
        {
            { State.Idle, new IdleState() },
            { State.Move, new MoveState() },
            { State.Fall, new FallState() },
            { State.Fly, new FlyState() }
        };
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        currentState = State.Idle;

        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, 0);

        originPos = transform.position;
        isFall = false;
        arrivedTargetLane = false;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;

        lookAtPlayer.Look(true);

        stateMachine = new StateMachine<Enemy_HermitCrab>();
        stateMachine.Setup(this, states[State.Idle]);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        stateMachine?.Execute();
    }

    protected override void OnDeath()
    {
        lookAtPlayer.Look(false);

        if (isFall)
            ChangeState(State.Fly);
        else
        {
            PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_DIE);
            Invoke(nameof(DeactivateGameObject), 5f);
        }
    }

    public void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        stateMachine.ChangeState(states[newState]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsDead) return;

        if (other.TryGetComponent<DetectedUser>(out DetectedUser car))
        {
            ChangeState(State.Fly);
            Player.Instance.TakeDamage(2);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    private void OnDisable()
    {
        stateMachine = null;
    }

    private class IdleState : IState<Enemy_HermitCrab>
    {
        public void Enter(Enemy_HermitCrab entity)
        {
            entity.SetDefaultAnimation(Constants.Animation.ENEMY_ANIMATION_IDLE);
        }

        public void Execute(Enemy_HermitCrab entity)
        {
            // If entity finished everything.
            if (entity.isFall) return;

            float carDist = Vector3.Distance(entity.transform.position, IncaData.UserCarTransform.position);

            if (!entity.arrivedTargetLane && carDist <= entity.followDistance)
                entity.ChangeState(State.Move);
            else if (entity.arrivedTargetLane && carDist <= entity.attackDistance)
                entity.ChangeState(State.Fall);
        }

        public void Exit(Enemy_HermitCrab entity) { }
    }

    private class MoveState : IState<Enemy_HermitCrab>
    {
        float followTimer = 0;
        float progress = 0;

        Vector3 targetPos;

        public void Enter(Enemy_HermitCrab entity)
        {
            followTimer = 0;
            progress = 0;

            print("Enter Move");
            print(IncaData.UserCarLaneIndex);

            targetPos = entity.transformByLaneIndex[IncaData.UserCarLaneIndex].position;

            entity.lookAtPlayer.Look(true);
            entity.SetDefaultAnimation(Constants.Animation.ENEMY_ANIMATION_MOVE);
        }

        public void Execute(Enemy_HermitCrab entity)
        {
            followTimer += Time.fixedDeltaTime;
            progress = followTimer / entity.followTime;

            entity.transform.position = Vector3.Lerp(entity.originPos, targetPos, progress);

            if (progress >= 1)
                entity.ChangeState(State.Idle);
        }


        public void Exit(Enemy_HermitCrab entity)
        {
            entity.arrivedTargetLane = true;
            entity.lookAtPlayer.Look(false);
        }
    }

    private class FallState : IState<Enemy_HermitCrab>
    {
        float fallDegree = 0;

        public void Enter(Enemy_HermitCrab entity)
        {
            fallDegree = 0;
            entity.isFall = true;
            entity.lookAtPlayer.Look(false);

            entity.SetDefaultAnimation(Constants.Animation.ENEMY_ANIMATION_ATTACK);
        }

        public void Execute(Enemy_HermitCrab entity)
        {
            float amount = entity.fallSpeed * Time.fixedDeltaTime;

            entity.transform.Rotate(0, 0, amount);

            fallDegree += amount;

            if (fallDegree > 90f)
                entity.ChangeState(State.Idle);
        }


        public void Exit(Enemy_HermitCrab entity) { }
    }

    private class FlyState : IState<Enemy_HermitCrab>
    {
        float flyTimer = 0;

        public void Enter(Enemy_HermitCrab entity)
        {
            flyTimer = 0;

            Vector3 rot = entity.transform.localEulerAngles;
            // entity.transform.localEulerAngles = new Vector3(rot.x, rot.y, 90);

            entity.GetComponent<Rigidbody>().isKinematic = false;
            entity.GetComponent<Rigidbody>().useGravity = true;
            entity.GetComponent<Rigidbody>().AddExplosionForce(entity.exFor, entity.explosionTf.position, entity.exRa, 1f);

            entity.ForceKill();

            entity.PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);
        }

        public void Execute(Enemy_HermitCrab entity)
        {
            flyTimer += Time.fixedDeltaTime;
            if (flyTimer > 3f)
                entity.DeactivateGameObject();
        }

        public void Exit(Enemy_HermitCrab entity) { }
    }
}
