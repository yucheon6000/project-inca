using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaleEffector : Effector
{
    // Target transform
    [SerializeField]
    private Transform targetTransform;
    public Transform TargetTransform => targetTransform ? targetTransform : transform;

    // Original scale
    protected Vector3 originalScale;
    protected bool hasOriginalScaleVariable = false;
    public Vector3 OriginalScale;

    private void Awake()
    {
        SaveOriginalScale();
    }

    protected void SaveOriginalScale()
    {
        // Save original scale.
        if (!hasOriginalScaleVariable)
        {
            originalScale = TargetTransform.localScale;
            hasOriginalScaleVariable = true;
        }
    }

    public void PlayFromCurrentScaleToZero(float time, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
        => PlayToZero(time, TargetTransform.localScale, scaleCurve, onFinishEffect);

    public void PlayToZero(float time, Vector3 startScale, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
        => Play(time, startScale, Vector3.zero, scaleCurve, onFinishEffect);

    public void PlayFromZeroToOriginalScale(float time, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
        => PlayToOriginalScale(time, Vector3.zero, scaleCurve, onFinishEffect);

    public void PlayFromOriginalScale(float time, Vector3 endScale, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
        => Play(time, originalScale, endScale, scaleCurve, onFinishEffect);

    public void PlayToOriginalScale(float time, Vector3 startScale, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
        => Play(time, startScale, originalScale, scaleCurve, onFinishEffect);

    public void Play(float time, Vector3 startScale, Vector3 endScale, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
    {
        coroutine = StartCoroutine(ScaleEffectRoutine(time, startScale, endScale, scaleCurve, onFinishEffect));
    }

    private IEnumerator ScaleEffectRoutine(float time, Vector3 startScale, Vector3 endScale, AnimationCurve scaleCurve, UnityAction onFinishEffect = null)
    {
        float timer = 0;
        float progress = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / time;

            TargetTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleCurve.Evaluate(progress));

            yield return null;
        }

        coroutine = null;

        if (onFinishEffect != null)
            onFinishEffect.Invoke();
    }
}
