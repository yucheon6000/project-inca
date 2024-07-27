using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Honeycomb : NonDamagableEnemy
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

    public override bool IsAlive => bear.IsDead && HingesAreFine();

    private bool HingesAreFine()
    {
        foreach (Enemy hinge in hinges)
            if (hinge.IsAlive) return true;

        return false;
    }

    private void Update()
    {
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        if (IsDead) return;

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootDelay)
            Attack();
    }

    public override void Attack()
    {
        base.Attack();

        GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(beePrefab, transform.position);
        bullet.transform.SetParent(IncaData.UserCarTransform);
        bullet.GetComponent<Enemy>().Init();
        shootTimer = 0;
    }

    public override int TakeDamage(int attckAmount)
    {
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_TAKE_DAMAGE);
        return base.TakeDamage(attckAmount);
    }
}
