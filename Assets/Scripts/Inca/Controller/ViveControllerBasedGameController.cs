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

    [SerializeField]
    private Camera camera;

    float prevScale = 0;
    float startScale = 0;
    float targetScale = 1;
    float timer = 0;

    private Enemy pointedEnemy;

    private void Awake()
    {
        ViveInput.AddListenerEx(HandRole.RightHand, ControllerButton.FullTrigger, ButtonEventType.Down, () =>
        {
            if (pointedEnemy == null) return;

            pointedEnemy.Hit(1);
            targetScale = 1;
        });
    }

    private void Update()
    {
        // print(Input.mousePosition);

        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100000, targetLayermask)
            && hitInfo.collider.TryGetComponent<Enemy>(out Enemy enemy)
            && enemy.IsInteractableType(InteractableType.Hitable))
        {
            pointedEnemy = enemy;
            rectTransform.position = Vector3.Lerp(rectTransform.position, camera.WorldToScreenPoint(hitInfo.point), Time.deltaTime * aaaa);
            float dist = Vector3.Distance(controllerTransform.position, hitInfo.point);
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
            pointedEnemy = null;
            rayT.localScale = new Vector3(1, 1, aaa);
            rectTransform.position = Vector3.Lerp(rectTransform.position, camera.WorldToScreenPoint(controllerTransform.position + controllerTransform.forward * aaa), Time.deltaTime * aaaa);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(controllerTransform.position, controllerTransform.position + (controllerTransform.forward * 100));
    }
}
