using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bear : DamagableEnemy
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float shootDelay;
    private float shootTimer;

    [SerializeField]
    private Enemy hoenycomb;

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) return;

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootDelay)
            PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        // This attack animation calls 'this.Attack();'
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_ATTACK);
    }

    public override void Attack()
    {
        GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab, transform.position);
        bullet.transform.position = transform.position;
        bullet.GetComponent<Enemy>().Init();
        shootTimer = 0;
    }

    public override int TakeDamage(int attckAmount)
    {
        hoenycomb.TakeDamage(1);
        shootTimer = Mathf.Min(shootTimer, shootDelay - 1f);

        return base.TakeDamage(attckAmount);
    }

    protected override void OnDeath()
    {
        transform.SetParent(null);
        base.OnDeath();
        // gameObject.SetActive(false);
    }
}
