using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Tree : Enemy
{
    [SerializeField]
    private float attackDistance;

    [SerializeField]
    private float fallTime;
    [SerializeField]
    private AnimationCurve fallCurve;

    [SerializeField]
    private LookAtPlayer lookAtPlayer;

    [SerializeField]
    private List<Transform> targetTransform;

    private void Update()
    {
        if (isDead) return;

        if (state == EnemyState.Idle && Vector3.Distance(transform.position, IncaData.PlayerPosition) < attackDistance)
        {
            state = EnemyState.Attack;
            lookAtPlayer.Look(false);
            StartCoroutine(TreeFallRoutine());
        }
    }

    private IEnumerator TreeFallRoutine()
    {
        float fallTimer = 0;
        float progress = 0;

        while (progress < 1)
        {
            fallTimer += Time.deltaTime;
            progress = fallTimer / fallTime;

            float angleZ = Mathf.Lerp(0, 90, fallCurve.Evaluate(progress));

            transform.rotation = Quaternion.Euler(0, 0, angleZ);

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
