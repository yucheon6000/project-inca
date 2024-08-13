using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmissionEffector : Effector
{
    private Dictionary<Material, Color> originalEmissions;

    [SerializeField]
    private Color emissionColor;

    private void Awake()
    {
        SaveMaterials();
    }

    private void SaveMaterials()
    {
        // Find all mesh renderers.
        List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>(GetComponents<SkinnedMeshRenderer>());
        meshRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());

        // Find all materials.
        List<Material> materials = new List<Material>();
        foreach (var meshRenderer in meshRenderers)
            materials.AddRange(meshRenderer.materials);

        // Save original emission values.
        originalEmissions = new Dictionary<Material, Color>();
        foreach (var mat in materials)
            originalEmissions.Add(mat, mat.GetColor("_EmissionColor"));
    }

    public void Play(float time, float startIntensity, float endIntensity, UnityAction onFinishEffect = null)
        => Play(time, startIntensity, endIntensity, emissionColor, onFinishEffect);

    public void Play(float time, float startIntensity, float endIntensity, Color emissionColor, UnityAction onFinishEffect = null)
    {
        coroutine = StartCoroutine(EmissionEffectRoutine(time, startIntensity, endIntensity, emissionColor, onFinishEffect));
    }

    private IEnumerator EmissionEffectRoutine(float time, float startIntensity, float endIntensity, Color emissionColor, UnityAction onFinishEffect = null)
    {
        float timer = 0;
        float progress = 0;

        AnimationCurve easeOutCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 1),  // 시작점 (시간 0, 값 0, 입구 기울기 0, 출구 기울기 1)
            new Keyframe(1, 1, 1, 0)   // 끝점 (시간 1, 값 1, 입구 기울기 1, 출구 기울기 0)
        );

        // Enable emission.
        foreach (var mat in originalEmissions.Keys)
            mat.EnableKeyword("_EMISSION");

        // Change emission value.
        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / time;

            float value = Mathf.Lerp(startIntensity, endIntensity, easeOutCurve.Evaluate(progress));
            Color color = emissionColor * Mathf.LinearToGammaSpace(value);

            foreach (var mat in originalEmissions.Keys)
                mat.SetColor("_EmissionColor", color);

            yield return null;
        }

        // Disable emission.
        foreach (var mat in originalEmissions.Keys)
            mat.DisableKeyword("_EMISSION");

        coroutine = null;

        // Call the callback method.
        if (onFinishEffect != null)
            onFinishEffect.Invoke();
    }
}
