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

    private void Awake()
    {
        originScale = transform.localScale.x;
    }

    public override void Setup(DetectedObject detectedObject)
    {
        base.Setup(detectedObject);
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

    public override void Hit(int power)
    {
        // base.Hit(power);
        onClick.Invoke();
        print("aa");
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
