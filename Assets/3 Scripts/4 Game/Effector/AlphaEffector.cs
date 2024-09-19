using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;
using UnityEngine.Events;

public class AlphaEffector : Effector
{
    private Dictionary<MeshRenderer, Material[]> materials;

    private Dictionary<Material, float> originalAlphas;

    private void Awake()
    {
        SaveMaterials();
    }

    private void SaveMaterials()
    {
        // Find all mesh renderers.
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        meshRenderers.AddRange(GetComponentsInChildren<MeshRenderer>());


        // Find all materials.
        materials = new Dictionary<MeshRenderer, Material[]>();
        foreach (var renderer in meshRenderers)
            materials.Add(renderer, renderer.materials);

        /*
        // Save original emission values.
        originalAlphas = new Dictionary<Material, float>();
        foreach (var materials in materials.Values)
            foreach (var mat in materials)
                originalAlphas.Add(mat, mat.color.a);
        */
    }

    public void Play(float time, float startValue, float endValue, UnityAction onFinishEffect = null)
    {
        if (gameObject.activeSelf == false) return;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(AlphaEffectRoutine(time, startValue, endValue, onFinishEffect));
    }

    private IEnumerator AlphaEffectRoutine(float time, float startValue, float endValue, UnityAction onFinishEffect = null)
    {
        float timer = 0;
        float progress = 0;

        AnimationCurve easeOutCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 1),  // 시작점 (시간 0, 값 0, 입구 기울기 0, 출구 기울기 1)
            new Keyframe(1, 1, 1, 0)   // 끝점 (시간 1, 값 1, 입구 기울기 1, 출구 기울기 0)
        );

        // Change emission value.
        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / time;

            float valueA = Mathf.Lerp(startValue, endValue, easeOutCurve.Evaluate(progress));

            foreach (var renderer in materials.Keys)
            {
                List<Material> mats = new List<Material>();
                foreach (var mat in renderer.materials)
                {
                    Color color = mat.color;
                    color.a = valueA;
                    mat.color = color;
                    mats.Add(mat);
                }
                renderer.GetMaterials(mats);
            }

            /*
            foreach (var mat in originalAlphas.Keys)
            {
                Color color = mat.color;
                color.a = valueA;
                mat.color = color;
            }
            */

            yield return null;
        }

        // Reset alpha value to original value.
        /*
        foreach (var mat in originalAlphas.Keys)
        {
            float valueA = originalAlphas[mat];
            Color color = mat.color;
            color.a = valueA;
            mat.color = color;
        }
        */

        coroutine = null;

        // Call the callback method.
        if (onFinishEffect != null)
            onFinishEffect.Invoke();
    }
}
