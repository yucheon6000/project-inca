using System.Collections;
using System.Collections.Generic;
using Inca;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Tree : Enemy
{
    [SerializeField]
    private float followDistance;
    [SerializeField]
    private float attackDistance;

    [Space]
    [SerializeField]
    private float followTime;

    [Space]
    [SerializeField]
    private float fallTime;
    [SerializeField]
    private AnimationCurve fallCurve;

    [SerializeField]
    private LookAtPlayer lookAtPlayer;

    [SerializeField]
    private List<Transform> transformByLaneIndex;
    [SerializeField]
    private Transform explosionTf;
    [SerializeField]
    private float exFor = 1000;
    [SerializeField]
    private float exRa = 10;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isDead) return;

        float dist = Vector3.Distance(transform.position, IncaData.PlayerPosition);

        if (state == EnemyState.Idle && dist < followDistance)
        {
            state = EnemyState.Move;
            lookAtPlayer.Look(true);
            StartCoroutine(MoveRoutine());
        }

        else if (state == EnemyState.Move && dist < attackDistance)
        {
            state = EnemyState.Attack;
            lookAtPlayer.Look(false);
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator MoveRoutine()
    {
        float followTimer = 0;
        float progress = 0;

        Vector3 originPos = transform.position;
        Vector3 targetPos = transformByLaneIndex[IncaData.PlayerLaneIndex].position;

        while (progress < 1)
        {
            followTimer += Time.deltaTime;
            progress = followTimer / followTime;

            transform.position = Vector3.Lerp(originPos, targetPos, progress);

            yield return null;
        }
    }

    private IEnumerator FallRoutine()
    {
        float fallTimer = 0;
        float progress = 0;

        while (progress < 1)
        {
            fallTimer += Time.deltaTime;
            progress = fallTimer / fallTime;

            float angleZ = Mathf.Lerp(0, 89, fallCurve.Evaluate(progress));

            transform.rotation = Quaternion.Euler(0, 0, angleZ);

            yield return null;
        }
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == EnemyState.Die) return;

        if (other.TryGetComponent<UserCar>(out UserCar car))
        {
            state = EnemyState.Die;

            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            // rigidbody.AddForce((transform.position - explosionTf.position).normalized * exFor, ForceMode.Impulse);
            rigidbody.AddExplosionForce(exFor, explosionTf.position, exRa, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
