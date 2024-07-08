using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Bullet_OctopusBubble : Bullet
{
    [SerializeField]
    private float minMoveSpeed;
    [SerializeField]
    private float maxMoveSpeed;

    [Space]
    [SerializeField]
    private float minUpdateMoveForceTime;
    [SerializeField]
    private float maxUpdateMoveForceTime;
    private float updateMoveForceTime;
    private float updateMoveForceTimer = Mathf.Infinity;

    private Vector3 moveForce = Vector3.zero;

    private float distanceOnSpawn;
    [SerializeField]
    private float minRandomMoveDistance;

    [Range(0f, 1f)]
    [SerializeField]
    private float randomMoveMaxRatio;

    [SerializeField]
    private AnimationCurve moveForceCurve;

    [Header("Scale")]
    [SerializeField]
    private float minInitialScale;
    [SerializeField]
    private float maxInitialScale;
    [SerializeField]
    private float minFinalScale;
    [SerializeField]
    private float maxFinalScale;

    private float initialScale;
    private float finalScale;

    private float startScale;
    private float targetScale;

    [SerializeField]
    private AnimationCurve scaleCurve;

    private void Awake()
    {
        print("Bullet_OctopusBubble: Awake()");
    }

    private void Start()
    {
        print("Bullet_OctopusBubble: OnEnable()");
        base.OnEnable();

        updateMoveForceTimer = Mathf.Infinity;
        distanceOnSpawn = Vector3.Distance(IncaData.PlayerPosition, this.transform.position);
        print(IncaData.PlayerPosition.ToString());

        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        updateMoveForceTime = Random.Range(minUpdateMoveForceTime, maxUpdateMoveForceTime);

        initialScale = Random.Range(minInitialScale, maxInitialScale);
        startScale = initialScale;
        transform.localScale = Vector3.one * initialScale;
        finalScale = Random.Range(minFinalScale, maxFinalScale);
    }

    protected override void FixedUpdate()
    {
        updateMoveForceTimer += Time.fixedDeltaTime;

        float progress = updateMoveForceTimer / updateMoveForceTime;

        if (progress >= 1)
        {
            UpdateMoveForce();
            UpdateTargetScale();
            updateMoveForceTimer = 0;
        }

        float a = moveForceCurve.Evaluate(progress);

        transform.position += a * moveForce * Time.fixedDeltaTime;
        // transform.localScale = Vector3.one * Mathf.Lerp(startScale, targetScale, scaleCurve.Evaluate(progress));

        if (hasReachedPlayer())
            Destroy(this.gameObject);
    }

    private void UpdateMoveForce()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.x *= 2;
        randomDirection.y *= 0.5f;
        randomDirection.z = -Mathf.Abs(randomDirection.z);
        randomDirection.Normalize();

        float dist = Vector3.Distance(IncaData.PlayerPosition, this.transform.position);

        float randomMoveRatio = Mathf.Lerp(randomMoveMaxRatio, 0, (dist - minRandomMoveDistance) / (distanceOnSpawn - minRandomMoveDistance));
        print($"{dist}, {minRandomMoveDistance}, {distanceOnSpawn}");
        print((dist - minRandomMoveDistance) / (distanceOnSpawn - minRandomMoveDistance));
        float straightMoveRatio = 1 - randomMoveMaxRatio;

        moveForce = (IncaData.PlayerPosition - transform.position).normalized * straightMoveRatio + randomDirection * randomMoveMaxRatio;
        moveForce = moveForce.normalized * moveSpeed;
    }

    private void UpdateTargetScale()
    {
        float dist = Vector3.Distance(IncaData.PlayerPosition, this.transform.position);

        startScale = transform.localScale.x;
        targetScale = Mathf.Lerp(finalScale, initialScale, (dist - minRandomMoveDistance) / (distanceOnSpawn - minRandomMoveDistance));
    }
}
