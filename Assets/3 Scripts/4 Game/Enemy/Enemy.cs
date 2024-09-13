using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : Character
{
    protected DetectedObject detectedObject;

    [SerializeField]
    private EnemyState currentState = EnemyState.None;
    protected EnemyState CurrentState => currentState;
    private EnemyState previousState = EnemyState.None;
    protected EnemyState PreviousState => previousState;
    private EnemyState defaultState = EnemyState.Idle;
    protected EnemyState DefaultState => defaultState;
    protected void SetDefaultState(EnemyState state) => defaultState = state;
    private EnemyState startState = EnemyState.Spawn;
    protected EnemyState StartState => startState;
    protected void SetStartState(EnemyState state) => startState = state;

    [Header("[Attack]")]
    [SerializeField]
    private bool canAttack = false;
    [SerializeField]
    private bool canTakeDamage = true;
    protected float DistanceToUserCar => Vector3.Distance(transform.position, IncaData.UserCarPosition);

    /*----------------- Effect -----------------*/
    [Header("[Effect]")]
    [SerializeField]
    protected GameObject spawnEffectPrefab;
    [SerializeField]
    protected GameObject dieEffectPrefab;

    [Header("[Effect - Appear/Disappear]")]
    [SerializeField]
    private float emissionEffectIntensity = 10;

    [Space]
    [SerializeField]
    private bool useAppearEffect = false;
    [SerializeField]
    private float appearTime;
    [SerializeField]
    private AnimationCurve appearScaleCurve;
    [SerializeField]
    private bool useAppearEmissionEffect = false;

    [Space]
    [SerializeField]
    private bool useDisappearEffect = false;
    [SerializeField]
    private float disappearTime;
    [SerializeField]
    private AnimationCurve disappearScaleCurve;
    [SerializeField]
    private bool useDisappearEmissionEffect = false;

    /*------------------- FSM -------------------*/
    protected StateMachine<Enemy> stateMachine;
    protected Dictionary<EnemyState, IEnemyState> states;

    /*---------------- Components ----------------*/
    protected new Rigidbody rigidbody;
    protected ScaleEffector scaleEffector;
    protected EmissionEffector emissionEffector;
    protected LookAtPlayer lookAtPlayer;

    protected override void Awake()
    {
        base.Awake();

        GetMyComponents();

        if (initOnAwake)
            Init(null);
    }

    protected virtual void GetMyComponents()
    {
        rigidbody = GetComponent<Rigidbody>();
        scaleEffector = GetComponent<ScaleEffector>();
        emissionEffector = GetComponent<EmissionEffector>();
        lookAtPlayer = GetComponent<LookAtPlayer>();
        if (lookAtPlayer == null)
            lookAtPlayer = GetComponentInChildren<LookAtPlayer>();
    }

    protected override void Start()
    {
        base.Start();

        if (initOnStart)
            Init(null);
    }

    /*------------------- Init -------------------*/
    /// <summary>
    /// When this enemy is spawned by EnemySpawner, this method is called firstly.
    /// </summary>
    /// <param name="detectedObject"></param>
    public virtual void Init(DetectedObject detectedObject = null)
    {
        RegisterInDetectedObject(detectedObject);
        base.Init();

        canAttack = false;
        canTakeDamage = true;

        ResetAnimator();
        InitStateMachine();
        StartStateMachine();
    }

    private void RegisterInDetectedObject(DetectedObject detectedObject)
    {
        if (detectedObject == null) return;

        // If the detectedObject is hiden, call OnHideDetectedObject method.
        // Basically, OnHideDetectedObject call ForceKill method.
        this.detectedObject = detectedObject;
        detectedObject.OnHideDetectedObject.AddListener(OnHideDetectedObject);
    }

    private void ResetAnimator()
    {
        if (animator == null) return;

        animator.Rebind();
        animator.Update(0);
    }

    protected virtual void InitStateMachine()
    {
        currentState = EnemyState.None;
        previousState = EnemyState.None;

        stateMachine = new StateMachine<Enemy>();
        stateMachine.Setup(this, null);

        states = new Dictionary<EnemyState, IEnemyState>
        {
            { EnemyState.Spawn, new EnemyState_Spawn(this) },
            { EnemyState.Idle, new EnemyState_Idle(this) },
            { EnemyState.Move, new EnemyState_Move(this) },
            { EnemyState.Attack, new EnemyState_Attack(this) },
            // { EnemyState.TakeDamage, new EnemyState_TakeDamage(this) },
            { EnemyState.Die, new EnemyState_Die(this) },
            { EnemyState.Global, new EnemyState_Global(this) }
        };

    }

    private void StartStateMachine()
    {
        ChangeState(StartState);
        stateMachine.SetGlobalState(states[EnemyState.Global]);
    }

    protected void EnableRigidbody(bool enable)
    {
        if (rigidbody == null) return;

        rigidbody.isKinematic = !enable;
        rigidbody.useGravity = enable;

        if (!enable)
            rigidbody.velocity = Vector3.zero;
    }

    /*---------------- FixedUpdate ----------------*/
    protected virtual void FixedUpdate() => stateMachine?.Execute();

    protected override void OnEnable()
    {
        base.OnEnable();

        if (initOnEable)
            Init(null);
    }

    /*-------------- Methods for FSM --------------*/
    public void LookAtPlayer(bool value)
    {
        if (lookAtPlayer == null) return;
        lookAtPlayer.Look(value);
    }

    protected void LookAtPlayerImmediate()
    {
        lookAtPlayer.LookImmediate();
    }

    public virtual bool CanAttack()
        => canAttack;

    public void CanAttack(bool value)
        => canAttack = value;

    public virtual bool CanTakeDamage()
        => canTakeDamage;

    public void CanTakeDamage(bool value) { }

    public override float TakeDamage(float attackAmount)
    {
        if (CanTakeDamage() == false) return Status.CurrentHp;

        return TakeDamageWithChangingState(attackAmount);
    }

    protected float TakeDamageWithChangingState(float attackAmount)
    {
        float curHp = base.TakeDamage(attackAmount);

        if (IsAlive)
        {
            OnTakeDamage();
            // ChangeState(EnemyState.TakeDamage, true);
        }

        return curHp;
    }

    protected virtual void OnTakeDamage()
    {
        if (emissionEffector)
            emissionEffector.Play(0.4f, 10f, 0f, Color.red);

        PlayAudioClip(AudioType.TakeDamage0);

    }

    public void ForceKill()
    {
        if (IsAlive)
            base.TakeDamage(status.CurrentHp * 2);     // => Call OnDeath method
    }

    protected override void OnDeath()
        => ChangeState(EnemyState.Die);

    protected void DeactivateGameObject()
    {
        MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
        SpawnEffect(dieEffectPrefab, detectedObject);
    }

    protected virtual void OnHideDetectedObject()
    {
        ForceKill();
        // MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }

    /*--------------------- Effect ---------------------*/
    protected void PlayAppearEffect(UnityAction onFinishEffect = null)
    {
        if (!useAppearEffect) return;

        if (scaleEffector)
            scaleEffector.PlayFromZeroToOriginalScale(appearTime, appearScaleCurve, onFinishEffect);
        if (emissionEffector)
            emissionEffector.Play(appearTime, emissionEffectIntensity, 0);
    }

    protected void PlayDisappearEffect(UnityAction onFinishEffect = null)
    {
        if (!useDisappearEffect) return;

        if (scaleEffector)
            scaleEffector.PlayFromCurrentScaleToZero(disappearTime, disappearScaleCurve, onFinishEffect);
        if (emissionEffector)
            emissionEffector.Play(disappearTime, 0, emissionEffectIntensity);
    }

    protected virtual void SpawnSpawnEffect()
        => SpawnEffect(spawnEffectPrefab, detectedObject);

    protected virtual void SpawnDieEffect()
        => SpawnEffect(dieEffectPrefab, detectedObject);

    protected void SpawnEffect(GameObject effecPrefab, DetectedObject detectedObject = null)
    {
        if (effecPrefab == null) return;

        if (detectedObject == null)
        {
            if (transform.parent == null)
            {
                Instantiate(effecPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject clone = Instantiate(effecPrefab, transform.position, Quaternion.identity);
                clone.transform.SetParent(transform.parent.transform);
            }
        }
        else
        {
            Vector3 pos = detectedObject.Position;
            pos.y += detectedObject.Scale.y;
            GameObject clone = Instantiate(effecPrefab, pos, Quaternion.identity);
            clone.transform.SetParent(detectedObject.transform);
        }
    }

    /*------------------- Animation -------------------*/
    int defaultAnimationId = 1;
    // 지울 함수
    protected void SetDefaultAnimation(int animationId)
    {
        defaultAnimationId = animationId;
        PlayAnimationByValue(animationId);
    }

    int lastAnimationUpdateFrameCount = -1;
    Coroutine playDefaultAnimationRoutine = null;
    protected override void PlayAnimationByValue(int animationId)
    {
        if (animator == null) return;
        // if (lastAnimationUpdateFrameCount == Time.frameCount && animationId == Constants.Animation.ENEMY_ANIMATION_IDLE) return;

        animator.SetInteger(Constants.Animation.ENEMY_ANIMATION_ID, animationId);

        if (animationId != defaultAnimationId)
        {
            if (playDefaultAnimationRoutine != null)
                StopCoroutine(playDefaultAnimationRoutine);

            playDefaultAnimationRoutine = StartCoroutine(PlayDefaultAnimationRoutine());
        }

        // lastAnimationUpdateFrameCount = Time.frameCount;
    }

    // 지울 함수
    private IEnumerator PlayDefaultAnimationRoutine()
    {
        yield return null;
        PlayAnimationByValue(defaultAnimationId);
    }

    protected virtual void PlayAnimationByName(string animationName)
    {
        if (animator == null) return;

        animator.Play(animationName, 0, 0);
    }

    protected bool IsAnimationFinished(string animationName, int layerIndex = 0)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.IsName(animationName))
            // 애니메이션 진행도가 1이면 애니메이션이 끝난 것
            if (stateInfo.normalizedTime >= 1.0f)
                return true;

        return false;
    }

    // 지울 것
    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }

    /*--------------------- FSM ---------------------*/
    public void ChangeState(EnemyState newState, bool force = false)
    {
        if (stateMachine == null || states == null) return;
        if (states.ContainsKey(newState) == false) return;

        if (currentState == newState && !force) return;

        if (currentState != newState)
            previousState = currentState;

        currentState = newState;
        stateMachine.ChangeState(states[newState]);
    }

    /*  <Preset>
     *                                                                                    ______
     *  Enemy.StartStateMachine()  --(Start State)-->  Spawn       --(Default State)-->  |      |
     *                                                 Attack      --(Default State)-->  | Idle |
     *  Enemy.TakeDamage()                                                               |      |
     *  Enemy.OnDeath()            ----------------->  Die                                ------
     *
     */

    public interface IEnemyState : IState<Enemy> { }

    public class EnemyState_Spawn : IEnemyState
    {
        public EnemyState_Spawn(Enemy entity) { }

        /// <summary>
        /// Spawn Effect, Appear Effect, Spawn Animation, Spawn Audio
        /// After appear effect, call the OnAppear method.
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.SpawnSpawnEffect();
            entity.PlayAppearEffect(() => OnAppear(entity));

            entity.PlayAnimationByName(EnemyAnimation.Spawn);
            entity.PlayAudioClip(AudioType.Spawn);
        }
        public virtual void Execute(Enemy entity) { }
        public virtual void Exit(Enemy entity) { }

        /// <summary>
        /// Change state to Default State.
        /// </summary>
        public virtual void OnAppear(Enemy entity)
        {
            entity.ChangeState(entity.DefaultState);
        }
    }

    public class EnemyState_Idle : IEnemyState
    {
        public EnemyState_Idle(Enemy entity) { }

        /// <summary>
        /// Idle Animation
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.PlayAnimationByName(EnemyAnimation.Idle);
        }

        public virtual void Execute(Enemy entity) { }
        public virtual void Exit(Enemy entity) { }
    }

    public class EnemyState_Move : IEnemyState
    {
        public EnemyState_Move(Enemy entity) { }

        /// <summary>
        /// Move Animation
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.PlayAnimationByName(EnemyAnimation.Move);
            entity.PlayAudioClip(AudioType.Move);
        }
        public virtual void Execute(Enemy entity) { }
        public virtual void Exit(Enemy entity) { }
    }

    public class EnemyState_Attack : IEnemyState
    {
        public EnemyState_Attack(Enemy entity) { }

        /// <summary>
        /// Attack Animation
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.CanAttack(false);
            entity.PlayAnimationByName(EnemyAnimation.Attack0);
        }
        public virtual void Execute(Enemy entity)
        {
            if (entity.CanAttack())
                Attack(entity);
            if (entity.IsAnimationFinished(EnemyAnimation.Attack0))
                OnFinishAttackAnimation(entity);

        }
        public virtual void Exit(Enemy entity) { }

        /// <summary>
        /// Call entity's Attack method, Make CanAttack false, Attack Audio
        /// </summary>
        public virtual void Attack(Enemy entity)
        {
            entity.Attack();
            entity.CanAttack(false);
            entity.PlayAudioClip(AudioType.Attack0);
        }

        public virtual void OnFinishAttackAnimation(Enemy entity)
        {
            entity.ChangeState(entity.DefaultState);
        }
    }

    /*
    public class EnemyState_TakeDamage : IEnemyState
    {
        public EnemyState_TakeDamage(Enemy entity) { }

        /// <summary>
        /// Take Damage Animation, TakeDamage Audio
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.PlayAnimationByName(EnemyAnimation.TakeDamage);
            entity.PlayAudioClip(AudioType.TakeDamage0);
        }

        public virtual void Execute(Enemy entity)
        {
            if (entity.IsAnimationFinished(EnemyAnimation.TakeDamage))
                OnFinishTakeDamageAnimation(entity);

            if (entity.CanTakeDamage())
                OnCanTakeDamage(entity);
        }

        public virtual void Exit(Enemy entity) { }

        public virtual void OnCanTakeDamage(Enemy entity) { }

        public virtual void OnFinishTakeDamageAnimation(Enemy entity)
        {
            entity.ChangeState(entity.DefaultState);
        }
    }
    */

    public class EnemyState_Die : IEnemyState
    {
        public EnemyState_Die(Enemy entity) { }

        /// <summary>
        /// LookAtPlayer(false), Disappear Effect, Die Animation, Die Audio
        /// After disappear effect, call the OnDisappear method.
        /// </summary>
        public virtual void Enter(Enemy entity)
        {
            entity.LookAtPlayer(false);

            entity.PlayDisappearEffect(() => OnDisappear(entity));

            entity.PlayAnimationByName(EnemyAnimation.Die);
            entity.PlayAudioClip(AudioType.Die);
        }
        public virtual void Execute(Enemy entity) { }
        public virtual void Exit(Enemy entity) { }

        /// <summary>
        /// Die Effect, Deactivate Game Object
        /// </summary>
        public virtual void OnDisappear(Enemy entity)
        {
            entity.SpawnDieEffect();
            entity.DeactivateGameObject();
        }
    }

    public class EnemyState_Global : IEnemyState
    {
        public EnemyState_Global(Enemy entity) { }
        public virtual void Enter(Enemy entity) { }
        public virtual void Execute(Enemy entity) { }
        public virtual void Exit(Enemy entity) { }
    }
}
