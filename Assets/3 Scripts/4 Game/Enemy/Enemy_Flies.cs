using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Flies : Enemy
{
    [SerializeField]
    private List<Enemy_Fly> flies;

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        foreach (Enemy_Fly fly in flies)
            fly.Init();
    }

    private void Update()
    {
        foreach (Enemy_Fly fly in flies)
            if (fly.IsAlive) return;

        ForceKill();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
    }
}
