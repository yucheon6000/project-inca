using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Kindergartner : Enemy
{
    [SerializeField]
    private float moveSpeed;

    private void Start()
    {
        Init();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);
        Invoke(nameof(PlayAnimation), Random.Range(0f, 1f));
    }

    private void PlayAnimation()
    {
        animator.SetInteger("animation", 2);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
    }

    protected override void OnDeath()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(150f, 210f), 0);
        animator.SetInteger("animation", 5);

        base.OnDeath();

        // Destroy(this.gameObject);
    }
}
