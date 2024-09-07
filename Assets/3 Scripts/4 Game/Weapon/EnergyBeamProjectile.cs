using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnergyBeamProjectile : PlayerProjectile
{
    [Header("[[Energy Beam]]")]
    [SerializeField]
    private float attackRange = 100f;
    [SerializeField]
    private float attackTime = 1f;
    [SerializeField]
    private AnimationCurve attackCurve;
    [SerializeField]
    private LayerMask targetLayerMask;

    [SerializeField]
    private Transform explosionEffectTransfrom;
    private MeshRenderer explosionEffectRenderer;

    protected override void Awake()
    {
        base.Awake();
        explosionEffectRenderer = explosionEffectTransfrom.GetComponent<MeshRenderer>();
    }


    public override void Init(Weapon owner, Vector3 moveDirection, Character target = null)
    {

        base.Init(owner, moveDirection, target);

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        List<Collider> colliders = Physics.OverlapSphere(transform.position, attackRange, targetLayerMask).ToList();

        List<DamagableEnemy> enemies = colliders
                                            .Select(col => col.GetComponent<DamagableEnemy>())
                                            .Where(enemy => enemy != null && enemy.CanTakeDamage())
                                            .ToList();

        enemies.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));


        float timer = 0;
        float progess = 0;
        while (progess <= 1)
        {
            timer += Time.deltaTime;
            progess = timer / attackTime;

            float v = attackCurve.Evaluate(progess);
            float range = attackRange * v;

            KillEnemiesWithinRange(ref enemies, range);
            UpdateExplosionEffect(progess);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void KillEnemiesWithinRange(ref List<DamagableEnemy> enemies, float range)
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemies[i];

            if (enemy == null || enemy.IsDead) continue;
            if (Vector3.Distance(transform.position, enemy.transform.position) > range) continue;

            SpawnAttackEffect(enemy.transform.position);

            enemy.ForceKill();
            enemies.RemoveAt(i);
        }
    }

    private void UpdateExplosionEffect(float progess)
    {
        explosionEffectTransfrom.localScale = Vector3.one * Mathf.Lerp(6, 100, progess);

        Color color = explosionEffectRenderer.material.color;
        color.a = Mathf.Lerp(1, 0, progess);
        explosionEffectRenderer.material.color = color;
    }
}
