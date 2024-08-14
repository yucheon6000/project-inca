using System.Collections;
using System.Collections.Generic;
using Inca;
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

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);
        SetDefaultAnimation(Constants.Animation.ENEMY_ANIMATION_IDLE);
    }

    private void Update()
    {
        if (IsDead) return;

        shootTimer += Time.deltaTime;

        // 공격 애니메이션 공격하는 시점말고, 공격 애니메이션 끝나는 시점에서 이 분기에서 참이 되어서
        // 공격 안하는 경우 있음 (shootDelay를 좀 길게 주면 해결 됨)
        if (shootTimer >= shootDelay)
            PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        // This attack animation calls 'this.Attack();'
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_ATTACK);
    }

    protected override void Attack()
    {
        GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bulletPrefab, transform.position);
        bullet.transform.position = transform.position;
        bullet.GetComponent<Enemy>().Init();
        shootTimer = 0;
    }

    public override float TakeDamage(float attackAmount)
    {
        hoenycomb.TakeDamage(attackAmount);
        shootTimer = Mathf.Min(shootTimer, shootDelay - 1f);

        return base.TakeDamage(attackAmount);
    }

    protected override void OnDeath()
    {
        transform.SetParent(null);
        base.OnDeath();
        // gameObject.SetActive(false);
    }
}
