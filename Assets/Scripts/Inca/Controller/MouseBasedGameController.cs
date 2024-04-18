using Inca;
using UnityEngine;
using UnityEngine.UI;

public class MouseBasedGameController : GameController
{
    [SerializeField]
    private Transform mouseBallTransfrom;
    [SerializeField]
    private Transform hitBallTransfrom;

    private Vector3 mouseScreenPosition;
    private Vector3 mouseWorldPosition;

    Vector3 dir = Vector3.zero;

    // Aim Scale
    float prevScale = 0;
    float startScale = 0;
    float targetScale = 1;
    float scaleTimer = 0;

    private Vector3 CameraPosition => targetCamera.transform.position;
    private Vector3 PlayerPosition => IncaData.PlayerPosition;

    private void Update()
    {
        // Update mouse's positions
        mouseScreenPosition = Input.mousePosition + new Vector3(0, 0, 10f);
        mouseWorldPosition = targetCamera.ScreenToWorldPoint(mouseScreenPosition);

        // for Debug
        mouseBallTransfrom.position = mouseWorldPosition;

        dir = mouseWorldPosition - CameraPosition;
        dir.Normalize();

        Ray ray = new Ray(CameraPosition, dir);

        if (CheckTarget(ray))
        {
            // for Debug
            hitBallTransfrom.position = hitPoint;

            // Set aim's target scale
            float dist = Vector3.Distance(mouseWorldPosition, hitPoint);
            targetScale = Mathf.Lerp(0.1f, 2.4f, 5f / dist);

            if (prevScale == 1)
            {
                startScale = 1;
                scaleTimer = 0;
            }

            // Change aim's color
            image.material = materialRed;
        }
        else
        {
            targetScale = 1;
            if (prevScale < 0.95)
            {
                startScale = prevScale;
                scaleTimer = 0;
            }
            image.material = materialGreen;
        }

        // Update aim UI
        rectTransform.position = Input.mousePosition;
        scaleTimer += Time.deltaTime;
        rectTransform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, sizeChangingCurve.Evaluate(scaleTimer / 0.5f));
        prevScale = rectTransform.localScale.x;

        // Check trigger button down
        CheckTriggerDown();
    }

    private void CheckTriggerDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        TriggerShoot();
    }

    protected override void SpawnShootEffect()
    {
        GameObject cloneEffect = Instantiate(shootEffect, PlayerPosition, Quaternion.identity);
        cloneEffect.transform.SetParent(transform);

        // Target position that the effect should look at
        Vector3 targetPosition = target != null ? hitPoint : mouseWorldPosition;
        cloneEffect.transform.LookAt(targetPosition, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(CameraPosition, CameraPosition + (dir * 100));
    }
}
