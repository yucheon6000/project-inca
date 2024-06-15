using System.Collections;
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

    [SerializeField]
    private Vector3 originLocalPosition;
    [SerializeField]
    private float minLocalX;
    [SerializeField]
    private float maxLocalX;

    [Space]
    [SerializeField]
    private UnityEvent onInit = new UnityEvent();
    [SerializeField]
    private UnityEvent onDeath = new UnityEvent();

    private float maxY = 0;

    Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        Vector3 startPos = originLocalPosition;
        startPos.x = Random.Range(minLocalX, minLocalX + ((maxLocalX - minLocalX) / 2));
        transform.localPosition = startPos;

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
    private float moveDepthMin = 1f;
    [SerializeField]
    private float moveDepthMax = 1f;
    [SerializeField]
    private float moveDepth = 1f;
    [SerializeField]
    private float moveLengthMin = 1f;
    [SerializeField]
    private float moveLengthMax = 1f;
    [SerializeField]
    private float moveLength = 1f;
    [SerializeField]
    private float nextMoveDelayTime = 3f;


    public bool parent = true;
    private IEnumerator UpdateMove()
    {
        animator.SetInteger("animation", 2);

        float moveTimer = 0;

        Vector3 startPos = transform.localPosition;

        moveLength = Random.Range(moveLengthMin, moveLengthMax);
        if (currentMoveDirection > 0 && startPos.x + moveLength > maxLocalX)
            moveLength = maxLocalX - startPos.x;
        else if (currentMoveDirection < 0 && startPos.x + moveLength < minLocalX)
            moveLength = minLocalX - startPos.x;
        moveLength = Mathf.Abs(moveLength);

        moveDepth = Random.Range(moveDepthMin, moveDepthMax);

        while (moveTimer < moveTime)
        {
            moveTimer += Time.deltaTime;

            float progress = moveTimer / moveTime;

            Vector3 newPos = startPos;
            newPos.x += moveLength * progress * currentMoveDirection;
            newPos.y += -Mathf.Sin(progress * Mathf.PI) * moveDepth;

            if (newPos.x > maxLocalX) newPos.x = maxLocalX;
            else if (newPos.x < minLocalX) newPos.x = minLocalX;

            if (newPos.y > maxY) newPos.y = maxY;

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
        clone.GetComponent<Bullet>().SetAttack(status.CurrentAttack);
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

        onDeath.Invoke();

        animator.SetInteger("animation", 5);

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(DeactivateGameObject), 2);
    }
}
