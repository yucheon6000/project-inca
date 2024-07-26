using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Bullet_OctopusBubble : Enemy_Fly
{
    [SerializeField]
    private float rangeY = 5;
    [SerializeField]
    private float minMass;
    [SerializeField]
    private float maxMoveSpeed;

    private float distanceOnSpawn;

    [Header("Scale")]
    [SerializeField]
    private float minInitialScale;
    [SerializeField]
    private float maxInitialScale;
    [SerializeField]
    private float minFinalScale;
    [SerializeField]
    private float maxFinalScale;
    [SerializeField]
    private float maxPopScale;
    [SerializeField]
    private float scaleUpSpeed;

    private float initialScale;
    private float finalScale;
    private float targetScale;
    private float currentScale => transform.localScale.x;

    [SerializeField]
    private AnimationCurve scaleCurve;

    public override void Init(DetectedObject detectedObject = null)
    {
        // 범위를 생성 위치 기준으로 변경
        wanderPositionRangeMin.y = transform.localPosition.y - rangeY / 2;
        wanderPositionRangeMax.y = transform.localPosition.y + rangeY / 2;

        base.Init(detectedObject);

        distanceOnSpawn = Vector3.Distance(GGData.PlayerPosition, this.transform.position);

        initialScale = Random.Range(minInitialScale, maxInitialScale);
        targetScale = initialScale;
        finalScale = Random.Range(minFinalScale, maxFinalScale);

        transform.localScale = Vector3.zero;

        status.OnChangeCurrentHp.AddListener((curHp, _) =>
        {
            float newScale = Mathf.Lerp(maxPopScale, initialScale, (float)curHp / status.DefaultHp);
            targetScale = Mathf.Max(targetScale, newScale);
        });
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateTargetScale();
    }

    private void UpdateTargetScale()
    {
        float dist = Vector3.Distance(GGData.PlayerPosition, this.transform.position);

        float newScale = Mathf.Lerp(finalScale, initialScale, scaleCurve.Evaluate(dist / distanceOnSpawn));
        targetScale = Mathf.Max(targetScale, newScale);

        transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, targetScale, Time.fixedDeltaTime * scaleUpSpeed);
    }
}
