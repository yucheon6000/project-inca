using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public class ApplicationIcon : DamagableEnemy
{
    [SerializeField]
    private UnityEvent onClick = new UnityEvent();

    [SerializeField]
    private bool canSelect = false;

    private float originScale;
    private float startScale;
    [SerializeField]
    private float hoverScale;
    private bool hover = false;
    [SerializeField]
    private float animTime = 0.5f;
    private float animTimer = 100;

    [SerializeField]
    private AnimationCurve hoverStartScaleCurve;
    [SerializeField]
    private AnimationCurve hoverEndScaleCurve;

    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        originScale = transform.localScale.x;

        spriteRenderer = GetComponent<SpriteRenderer>();

        Init();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);
        CanSelect(canSelect);
    }

    private void Update()
    {
        animTimer += Time.deltaTime;

        float s = 0;
        if (hover)
            s = Mathf.Lerp(startScale, hoverScale, hoverStartScaleCurve.Evaluate(animTimer / animTime));
        else
            s = Mathf.Lerp(startScale, originScale, hoverEndScaleCurve.Evaluate(animTimer / animTime));

        transform.localScale = new Vector3(s, s, s);
    }

    public void CanSelect(bool value)
    {
        return;
        // !!!!!!! 수정 필요

        canSelect = value;

        Color c = spriteRenderer.color;

        if (canSelect) spriteRenderer.color = new Color(1, 1, 1, c.a);
        else spriteRenderer.color = new Color(45f / 255, 45f / 255, 45f / 255, c.a);
    }

    public override float TakeDamage(float attackAmount)
    {
        if (!canSelect) return 0;

        onClick.Invoke();
        return base.TakeDamage(attackAmount);
    }

    public override void OnHoverStart()
    {
        hover = true;
        animTimer = 0;
        startScale = transform.localScale.x;
    }

    public override void OnHoverEnd()
    {
        hover = false;
        animTimer = 0;
        startScale = transform.localScale.x;
    }

    public override bool IsInteractableType(InteractableType type)
    {

        return canSelect && type == InteractableType.Hitable;
    }
}
