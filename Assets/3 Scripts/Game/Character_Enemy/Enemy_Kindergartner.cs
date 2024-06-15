using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Kindergartner : Enemy
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float moveDistance;

    private Vector3 startPosition;

    [SerializeField]
    private float minScale;
    [SerializeField]
    private float maxScale;

    private LookAtPlayer lookAtPlayer;

    protected override void Awake()
    {
        base.Awake();

        lookAtPlayer = GetComponentInChildren<LookAtPlayer>();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        lookAtPlayer.Look(false);

        Invoke(nameof(PlayAnimation), Random.Range(0f, 1f));

        startPosition = transform.position;

        transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
    }

    private void PlayAnimation()
    {
        animator.SetInteger("animation", 2);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(transform.position, startPosition) > moveDistance)
            DeactivateGameObject();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        transform.rotation = Quaternion.Euler(0, Random.Range(150f, 210f), 0);
        animator.SetInteger("animation", 5);

        lookAtPlayer.Look(true);

        Invoke(nameof(DeactivateGameObject), 4);
    }
}
