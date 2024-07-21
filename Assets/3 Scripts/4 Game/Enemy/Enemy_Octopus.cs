using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Octopus : DamagableEnemy
{
    [Header("Attack_Bubble")]
    [SerializeField]
    private GameObject bubbleBulletPrefab;
    [SerializeField]
    private float bubbleAttackDelay;
    private float bubbleAttackTimer;
    [SerializeField]
    private int minBubbleAttackSpawnCount;
    [SerializeField]
    private int maxBubbleAttackSpawnCount;
    [SerializeField]
    private Transform[] bulletAttackSpawnTransforms;


    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) return;

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_IDLE);

        // Attack
        bubbleAttackTimer += Time.deltaTime;
        if (bubbleAttackTimer > bubbleAttackDelay)
            PlayAttackAnimation_Bubble();
    }

    public void PlayAttackAnimation_Bubble()
    {
        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_ATTACK);
    }

    public override void Attack()
    {
        base.Attack();

        foreach (var tf in bulletAttackSpawnTransforms)
        {
            int spawnCnt = Random.Range(minBubbleAttackSpawnCount, maxBubbleAttackSpawnCount);
            for (int i = 0; i < spawnCnt; ++i)
            {
                GameObject bullet = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(bubbleBulletPrefab, tf.position);
                bullet.transform.SetParent(IncaData.PlayerTransform);
                bullet.GetComponent<Enemy>().Init();
            }
        }

        bubbleAttackTimer = 0;
    }

    public override int TakeDamage(int attckAmount)
    {
        return base.TakeDamage(attckAmount);
    }
}
