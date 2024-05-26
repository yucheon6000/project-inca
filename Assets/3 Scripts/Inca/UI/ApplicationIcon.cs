using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public class ApplicationIcon : Enemy, InteractableObject
{
    [SerializeField]
    private UnityEvent onClick = new UnityEvent();

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

    protected override void Awake()
    {
        originScale = transform.localScale.x;
    }

    public override void Init(DetectedObject detectedObject)
    {
        base.Init(detectedObject);
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

    public override int Hit(int attckAmount)
    {
        onClick.Invoke();
        print("aa");
        return base.Hit(attckAmount);
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
}
