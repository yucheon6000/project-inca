using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bear : Enemy
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float shootDelay;
    private float shootTimer;

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) return;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootDelay)
        {
            GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab, transform.position);
            bullet.GetComponent<Enemy>().Init();
            shootTimer = 0;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        gameObject.SetActive(false);
    }
}
