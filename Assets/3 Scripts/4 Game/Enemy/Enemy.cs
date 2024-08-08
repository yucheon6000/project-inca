using System;
using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : Character
{
    private DetectedObject detectedObject;

    [Header("Effects")]
    [SerializeField]
    protected GameObject spawnEffectPrefab;
    [SerializeField]
    protected GameObject dieEffectPrefab;

    // Appear And Disappear
    protected Vector3 originalScale;
    protected bool hasOriginalScaleVariable = false;

    [Header("Effect - Appear/Disappear")]
    [SerializeField]
    private Transform appearTransform;
    protected Transform AppearTransform => appearTransform != null ? appearTransform : transform;

    [Space]
    [SerializeField]
    private Color emissionEffectColor;
    [SerializeField]
    private float emissionEffectIntensity = 1;
    [SerializeField]
    private float emissionEffectTime = 2;

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

    private Dictionary<Material, Color> originalEmissions;

    protected override void Awake()
    {
        base.Awake();

        InitMaterials();

        if (initOnAwake)
            Init(null);
    }

    private void InitMaterials()
    {
        List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>(GetComponents<SkinnedMeshRenderer>());
        meshRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());

        List<Material> materials = new List<Material>();
        foreach (var meshRenderer in meshRenderers)
            materials.AddRange(meshRenderer.materials);

        print(materials.Count);

        originalEmissions = new Dictionary<Material, Color>();
        foreach (var mat in materials)
            originalEmissions.Add(mat, mat.GetColor("_EmissionColor"));
    }

    [ContextMenu("Init")]
    private void InitWithoutDetectedObject()
    {
        Init(null);
    }

    /// <summary>
    /// When this enemy is spawned by EnemySpawner, this method is called firstly.
    /// </summary>
    /// <param name="detectedObject"></param>
    public virtual void Init(DetectedObject detectedObject = null)
    {
        // If the detectedObject is hiden, call OnHideDetectedObject method.
        // Basically, OnHideDetectedObject call ForceKill method.
        if (detectedObject != null)
        {
            this.detectedObject = detectedObject;
            detectedObject.OnHideDetectedObject.AddListener(OnHideDetectedObject);
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0);
        }

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        SpawnEffect(spawnEffectPrefab, detectedObject);

        if (!hasOriginalScaleVariable)
        {
            originalScale = AppearTransform.localScale;
            hasOriginalScaleVariable = true;
        }

        if (useAppearEffect)
            AppearEffect();

        base.Init();
    }

    public override void Attack()
    {
        base.Attack();

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_ATTACK);
    }

    public override float TakeDamage(float attckAmount)
    {
        float curHp = base.TakeDamage(attckAmount);

        if (IsDead)
        {
            return 0;
        }
        else
        {
            PlayAudioClip(AudioType.TakeDamage0);
            PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_TAKE_DAMAGE);
        }

        return curHp;
    }

    public void ForceKill()
    {
        base.TakeDamage(status.CurrentHp);     // => Call OnDeath method
    }

    /// <summary>
    /// Play audio clip and animation. 
    /// Start disappear coroutine.
    /// </summary>
    protected override void OnDeath()
    {
        PlayAudioClip(AudioType.Die);

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_DIE);

        if (useDisappearEffect)
            DisappearEffect();
    }

    protected void AppearEffect()
    {
        StartCoroutine(ScaleEffectRoutine(appearTime, Vector3.zero, originalScale, appearScaleCurve, OnAppear));
        if (useAppearEmissionEffect)
            StartCoroutine(EmissionEffectRoutine(Mathf.Max(appearTime, emissionEffectTime), emissionEffectIntensity, 0));
    }

    protected virtual void OnAppear() { }

    protected void DisappearEffect()
    {
        StartCoroutine(ScaleEffectRoutine(disappearTime, originalScale, Vector3.zero, disappearScaleCurve, OnDisappear));
        if (useDisappearEmissionEffect)
            StartCoroutine(EmissionEffectRoutine(Mathf.Max(disappearTime, emissionEffectTime), 0, emissionEffectIntensity));
    }

    protected virtual void OnDisappear() { }

    private IEnumerator ScaleEffectRoutine(float time, Vector3 startScale, Vector3 endScale, AnimationCurve scaleCurve, UnityAction onFinish)
    {
        float timer = 0;
        float progress = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / time;

            AppearTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(progress));

            yield return null;
        }

        onFinish.Invoke();
    }

    private IEnumerator EmissionEffectRoutine(float time, float startIntensity, float endIntensity)
    {
        float timer = 0;
        float progress = 0;

        foreach (var mat in originalEmissions.Keys)
            mat.EnableKeyword("_EMISSION");

        AnimationCurve easeOutCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 1),  // 시작점 (시간 0, 값 0, 입구 기울기 0, 출구 기울기 1)
            new Keyframe(1, 1, 1, 0)   // 끝점 (시간 1, 값 1, 입구 기울기 1, 출구 기울기 0)
        );

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / time;

            foreach (var mat in originalEmissions.Keys)
            {
                Color finalColor = emissionEffectColor * Mathf.LinearToGammaSpace(Mathf.Lerp(startIntensity, endIntensity, easeOutCurve.Evaluate(progress)));
                mat.SetColor("_EmissionColor", finalColor);
            }

            yield return null;
        }

        foreach (var mat in originalEmissions.Keys)
            mat.DisableKeyword("_EMISSION");
    }

    int defaultAnimationId = 1;
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

    private IEnumerator PlayDefaultAnimationRoutine()
    {
        yield return null;
        print("defaultAnimationId: " + defaultAnimationId);
        PlayAnimationByValue(defaultAnimationId);
    }

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

    public virtual void OnHoverStart() { }
    public virtual void OnHoverEnd() { }
}
