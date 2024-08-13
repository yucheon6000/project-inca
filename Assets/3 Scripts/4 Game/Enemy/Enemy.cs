using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : Character
{
    public enum EnemyState { Spawn, Idle, Move, Attack, TakeDamage, Die }

    private DetectedObject detectedObject;

    [SerializeField]
    private EnemyState currentState = EnemyState.Spawn;

    [Header("[Attack]")]
    [SerializeField]
    private bool canAttack = false;
    [SerializeField]
    private bool canTakeDamage = true;

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
    protected Dictionary<EnemyState, IState<Enemy>> states;

    /*---------------- Components ----------------*/
    protected ScaleEffector scaleEffector;
    protected EmissionEffector emissionEffector;
    private LookAtPlayer lookAtPlayer;

    protected override void Awake()
    {
        base.Awake();

        GetMyComponents();

        if (initOnAwake)
            Init(null);
    }

    protected virtual void GetMyComponents()
    {
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
        // If the detectedObject is hiden, call OnHideDetectedObject method.
        // Basically, OnHideDetectedObject call ForceKill method.
        if (detectedObject != null)
        {
            this.detectedObject = detectedObject;
            detectedObject.OnHideDetectedObject.AddListener(OnHideDetectedObject);
        }
    }

    private void ResetAnimator()
    {
        if (animator == null) return;

        animator.Rebind();
        animator.Update(0);
    }

    protected virtual void InitStateMachine()
    {
        stateMachine = new StateMachine<Enemy>();
        stateMachine.Setup(this, null);
        states = new Dictionary<EnemyState, IState<Enemy>>
        {
            { EnemyState.Spawn, new EnemyState_Spawn<Enemy>(this) },
            { EnemyState.Idle, new EnemyState_Idle<Enemy>(this) },
            { EnemyState.Move, new EnemyState_Move<Enemy>(this) },
            { EnemyState.Attack, new EnemyState_Attack<Enemy>(this) },
            { EnemyState.TakeDamage, new EnemyState_TakeDamage<Enemy>(this) },
            { EnemyState.Die, new EnemyState_Die<Enemy>(this) }
        };

        stateMachine.SetGlobalState(new EnemyState_Global<Enemy>(this));
    }

    protected virtual void StartStateMachine()
        => ChangeState(EnemyState.Spawn);

    /*---------------- FixedUpdate ----------------*/
    protected virtual void FixedUpdate() => stateMachine?.Execute();

    /*-------------- Methods for FSM --------------*/
    public void LookAtPlayer(bool value)
    {
        if (lookAtPlayer == null) return;
        lookAtPlayer.Look(value);
    }

    public bool CanAttack()
        => canAttack;

    public void CanAttack(bool value)
        => canAttack = value;

    public bool CanTakeDamage()
        => canTakeDamage;

    public void CanTakeDamage(bool value) { }

    public override float TakeDamage(float attckAmount)
    {
        float curHp = base.TakeDamage(attckAmount);

        if (IsAlive)
            ChangeState(EnemyState.TakeDamage);

        return curHp;
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
        print("defaultAnimationId: " + defaultAnimationId);
        PlayAnimationByValue(defaultAnimationId);
    }

    protected void PlayAnimationByName(string animationName)
    {
        if (animator == null) return;

        animator.Play(animationName, 0, 0);
    }

    protected bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(animationName))
            // 애니메이션 진행도가 1이면 애니메이션이 끝난 것
            if (stateInfo.normalizedTime >= 1.0f)
                return true;

        return false;
    }

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }

    /*--------------------- FSM ---------------------*/
    public void ChangeState(EnemyState newState)
    {
        if (stateMachine == null || states == null) return;
        if (states.ContainsKey(newState) == false) return;

        currentState = newState;
        stateMachine.ChangeState(states[newState]);
    }

    public class EnemyState_Spawn<T> : IState<T> where T : Enemy
    {
        public EnemyState_Spawn(T entity) { }

        /// <summary>
        /// Spawn Effect, Appear Effect, Spawn Animation, Spawn Audio
        /// After appear effect, call the OnAppear method.
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.SpawnSpawnEffect();
            entity.PlayAppearEffect(() => OnAppear(entity));

            entity.PlayAnimationByName("Spawn");
            entity.PlayAudioClip(AudioType.Spawn);
        }
        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }

        /// <summary>
        /// Change state to Idle.
        /// </summary>
        public virtual void OnAppear(T entity)
        {
            entity.ChangeState(EnemyState.Idle);
        }
    }

    public class EnemyState_Idle<T> : IState<T> where T : Enemy
    {
        public EnemyState_Idle(T entity) { }

        /// <summary>
        /// Idle Animation
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.PlayAnimationByName("Idle");
        }

        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }
    }

    public class EnemyState_Move<T> : IState<T> where T : Enemy
    {
        public EnemyState_Move(T entity) { }

        /// <summary>
        /// Move Animation
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.PlayAnimationByName("Move");
        }
        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }
    }

    public class EnemyState_Attack<T> : IState<T> where T : Enemy
    {
        public EnemyState_Attack(T entity) { }

        /// <summary>
        /// Attack Animation
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.PlayAnimationByName("Attack");
        }
        public virtual void Execute(T entity)
        {
            if (entity.IsAnimationFinished("Attack"))
                OnFinishAttackAnimation(entity);

        }
        public virtual void Exit(T entity) { }

        /// <summary>
        /// Call entity's Attack method, Make CanAttack false, Attack Audio
        /// </summary>
        public virtual void Attack(T entity)
        {
            entity.Attack();
            entity.CanAttack(false);
            entity.PlayAudioClip(AudioType.Attack0);
        }

        public virtual void OnFinishAttackAnimation(T entity)
        {
            entity.ChangeState(EnemyState.Idle);
        }
    }

    public class EnemyState_TakeDamage<T> : IState<T> where T : Enemy
    {
        public EnemyState_TakeDamage(T entity) { }

        /// <summary>
        /// Take Damage Animation, TakeDamage Audio
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.PlayAnimationByName("Take Damage");
            entity.PlayAudioClip(AudioType.TakeDamage0);
        }

        public virtual void Execute(T entity)
        {
            if (entity.IsAnimationFinished("Take Damage"))
                OnFinishTakeDamageAnimation(entity);

            if (entity.CanTakeDamage())
                OnCanTakeDamage(entity);
        }

        public virtual void Exit(T entity) { }

        public virtual void OnCanTakeDamage(T entity) { }

        public virtual void OnFinishTakeDamageAnimation(T entity)
        {
            entity.ChangeState(EnemyState.Idle);
        }
    }

    public class EnemyState_Die<T> : IState<T> where T : Enemy
    {
        public EnemyState_Die(T entity) { }

        /// <summary>
        /// Disappear Effect, Die Animation, Die Audio
        /// After disappear effect, call the OnDisappear method.
        /// </summary>
        public virtual void Enter(T entity)
        {
            entity.PlayDisappearEffect(() => OnDisappear(entity));

            entity.PlayAnimationByName("Die");
            entity.PlayAudioClip(AudioType.Die);
        }
        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }

        /// <summary>
        /// Die Effect, Deactivate Game Object
        /// </summary>
        public virtual void OnDisappear(T entity)
        {
            entity.SpawnDieEffect();
            entity.DeactivateGameObject();
        }
    }

    public class EnemyState_Global<T> : IState<T> where T : Enemy
    {
        public EnemyState_Global(T entity) { }
        public virtual void Enter(T entity) { }
        public virtual void Execute(T entity) { }
        public virtual void Exit(T entity) { }
    }
}
