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
    private bool isFall = false;

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
    [SerializeField]
    private float exMo = 1;

    private Rigidbody rigidbody;


    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) return;

        float dist = Vector3.Distance(transform.position, IncaData.PlayerPosition);

        if (state == EnemyState.Idle && dist < followDistance)
        {
            state = EnemyState.Move;
            lookAtPlayer.Look(true);
            animator.SetInteger("animation", 2);
            StartCoroutine(MoveRoutine());
        }

        else if (state == EnemyState.Move && dist < attackDistance)
        {
            state = EnemyState.Attack;
            lookAtPlayer.Look(false);
            animator.SetInteger("animation", 1);
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator MoveRoutine()
    {
        float followTimer = 0;
        float progress = 0;

        Vector3 originPos = transform.position;
        Vector3 targetPos = transformByLaneIndex[IncaData.PlayerLaneIndex].position;

        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        while (progress < 1)
        {
            followTimer += Time.deltaTime;
            progress = followTimer / followTime;

            transform.position = Vector3.Lerp(originPos, targetPos, progress);

            yield return wait;
        }
    }

    private IEnumerator FallRoutine()
    {
        float fallTimer = 0;
        float progress = 0;
        isFall = true;
        while (progress < 1)
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            fallTimer += Time.deltaTime;
            progress = fallTimer / fallTime;

            float angleZ = Mathf.Lerp(0, 90, fallCurve.Evaluate(progress));

            transform.rotation = Quaternion.Euler(0, 0, angleZ);

            yield return wait;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if (isFall)
        {
            StopAllCoroutines();
            transform.rotation = Quaternion.Euler(0, 0, 90);
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            // rigidbody.AddForce((transform.position - explosionTf.position).normalized * exFor, ForceMode.Impulse);
            rigidbody.AddExplosionForce(exFor, explosionTf.position, exRa, exMo);
        }
        else
        {
            animator.SetInteger("animation", 5);
        }

        // Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == EnemyState.Die || IsDead) return;

        if (other.TryGetComponent<UserCar>(out UserCar car))
        {
            state = EnemyState.Die;

            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            // rigidbody.AddForce((transform.position - explosionTf.position).normalized * exFor, ForceMode.Impulse);
            rigidbody.AddExplosionForce(exFor, explosionTf.position, exRa, 1f);

            Player.Instance.Hit(2);
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
