using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Honeycomb : Enemy
{
    [Header("Honeycomb")]
    [SerializeField]
    private GameObject beePrefab;
    [SerializeField]
    private float shootDelay;
    private float shootTimer;

    [SerializeField]
    private Enemy bear;
    [SerializeField]
    private Enemy[] hinges;

    private bool HingesAreFine()
    {
        foreach (Enemy hinge in hinges)
            if (hinge.IsAlive) return true;

        return false;
    }

    public override bool IsAlive => bear.IsDead && HingesAreFine();

    private void Update()
    {
        if (IsDead) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootDelay)
        {
            GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(beePrefab, transform.position);
            bullet.transform.SetParent(IncaData.PlayerCarTransform);
            bullet.GetComponent<Enemy>().Init();
            shootTimer = 0;
        }
    }

    public override bool IsInteractableType(InteractableType type)
    {
        return false;
    }
}
