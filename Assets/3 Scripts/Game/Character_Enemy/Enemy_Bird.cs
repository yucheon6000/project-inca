using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Bird : Enemy
{
    [SerializeField]
    private float attackTime;
    private float attackTimer;

    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform bulletSpawnTransform;

    [Space]
    [SerializeField]
    private UnityEvent onInit = new UnityEvent();

    private float maxY = 0;

    private void Start()
    {
        Init();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        attackTimer = 0;
        maxY = transform.localPosition.y;
        StartCoroutine(UpdateMove());
        onInit.Invoke();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        attackTimer += Time.deltaTime;
        if (attackTimer > attackTime)
        {
            Attack();
            attackTimer = 0;
            animator.Play("Attack");
        }
    }

    private int currentMoveDirection = 1;
    [SerializeField]
    float moveTime = 2f;
    [SerializeField]
    private float moveDepth = 1f;
    [SerializeField]
    private float moveLength = 1f;
    [SerializeField]
    private float nextMoveDelayTime = 3f;


    public bool parent = true;
    private IEnumerator UpdateMove()
    {
        animator.SetInteger("animation", 2);

        float moveTimer = 0;

        Vector3 originalPos = transform.localPosition;

        while (moveTimer < moveTime)
        {
            moveTimer += Time.deltaTime;

            float progress = moveTimer / moveTime;

            Vector3 newPos = originalPos;
            newPos.x += moveLength * progress * currentMoveDirection;
            newPos.y += -Mathf.Sin(progress * Mathf.PI) * moveDepth;

            if (newPos.y >= maxY) newPos.y = maxY;

            transform.localPosition = newPos;

            yield return new WaitForFixedUpdate();
        }

        currentMoveDirection *= -1;

        animator.SetInteger("animation", 1);

        yield return new WaitForSeconds(nextMoveDelayTime);

        StartCoroutine(UpdateMove());
    }

    public override int Hit(int attckAmount)
    {
        int curHp = base.Hit(attckAmount);

        if (IsDead) return curHp;

        animator.Play("Damage");

        return curHp;
    }

    public void Attack()
    {
        GameObject clone = Instantiate(bulletPrefab, bulletSpawnTransform.transform.position, Quaternion.LookRotation(IncaData.PlayerPosition));
        if (parent) clone.transform.SetParent(IncaData.PlayerTransform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        animator.SetInteger("animation", 5);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(DeactivateGameObject), 1);
    }
}
