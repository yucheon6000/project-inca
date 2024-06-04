using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;
using UnityEngine;

public class ViveControllerBasedGameController : GameController
{
    [Header("Controller")]
    [SerializeField]
    private Transform controllerTransform;
    [SerializeField]
    private float aaa;
    [SerializeField]
    private float aaaa;
    [SerializeField]
    private Transform rayT;

    float prevScale = 0;
    float startScale = 0;
    float targetScale = 1;
    float timer = 0;

    private void Awake()
    {
        ViveInput.AddListenerEx(HandRole.RightHand, ControllerButton.FullTrigger, ButtonEventType.Down, () =>
        {
            TriggerShoot();
        });
    }

    private void LateUpdate()
    {
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        if (CheckTarget(ray))
        {
            rectTransform.position = Vector3.Lerp(rectTransform.position, targetCamera.WorldToScreenPoint(hitPoint), Time.deltaTime * aaaa);

            float dist = Vector3.Distance(controllerTransform.position, hitPoint);
            targetScale = Mathf.Lerp(0.1f, 2.4f, 5f / dist);

            if (prevScale == 1)
            {
                startScale = 1;
                timer = 0;
            }
            image.material = materialRed;
        }
        else
        {
            target = null;
            rayT.localScale = new Vector3(1, 1, aaa);
            rectTransform.position = Vector3.Lerp(rectTransform.position, targetCamera.WorldToScreenPoint(controllerTransform.position + controllerTransform.forward * aaa), Time.deltaTime * aaaa);
            targetScale = 1;
            if (prevScale < 0.95)
            {
                startScale = prevScale;
                timer = 0;
            }
            image.material = materialGreen;
        }

        timer += Time.deltaTime;
        rectTransform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, sizeChangingCurve.Evaluate(timer / 0.5f));
        prevScale = rectTransform.localScale.x;

    }

    protected override void SpawnShootEffect()
    {
        GameObject cloneEffect = Instantiate(shootEffect, shootEffectSpawnTransform.position, Quaternion.identity);
        cloneEffect.transform.rotation = controllerTransform.rotation;
    }

    // protected override void SpawnShootEffect()
    // {
    //     GameObject cloneEffect = Instantiate(shootEffect, PlayerPosition, Quaternion.identity);
    //     cloneEffect.transform.SetParent(transform);

    //     // Target position that the effect should look at
    //     Vector3 targetPosition = target != null ? hitPoint : mouseWorldPosition;
    //     cloneEffect.transform.LookAt(targetPosition, Vector3.up);
    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(controllerTransform.position, controllerTransform.position + (controllerTransform.forward * 100));
    }
}
