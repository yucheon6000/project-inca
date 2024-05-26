using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Kindergartner : Enemy
{
    [SerializeField]
    private float moveSpeed;

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;
    }

    public override int Hit(int attckAmount)
    {
        if (isDead) return 0;

        return base.Hit(attckAmount);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        isDead = true;

        Destroy(this.gameObject);
    }
}
