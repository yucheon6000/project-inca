using Inca;
using UnityEngine;

public class Bullet_BearHoneyBall : DamagableEnemy
{
    [SerializeField]
    private int bounceCount = 4;
    private int currentBounceCount = 0;

    [SerializeField]
    private float bounceProgressSpeed;

    private Vector3 initialPosition;
    private float initialY;

    private Vector3 finalPosition;
    private float finalY;

    private float distance;

    private float gapDist;     // Distance of one bounce
    private float gapY;

    private Vector3 bounceStartZ;
    private Vector3 bounceEndZ;

    private float bounceProgress = 0;

    [SerializeField]
    private float hitPower = 10;

    private Rigidbody rigidbody;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        currentBounceCount = 0;

        gameObject.transform.SetParent(IncaData.PlayerCarTransform);
        transform.LookAt(IncaData.PlayerCarTransform);

        initialY = transform.localPosition.y;
        initialPosition = transform.localPosition;

        Vector3 userPos = transform.parent.InverseTransformPoint(IncaData.PlayerPosition);
        finalY = userPos.y;
        finalPosition = userPos;

        initialPosition.y = finalY;
        finalPosition.y = finalY;

        distance = Vector3.Distance(initialPosition, finalPosition);

        gapY = (initialY - finalY) / bounceCount;
        gapDist = distance / bounceCount;

        bounceStartZ = initialPosition - transform.forward * gapDist / 2;
        bounceEndZ = initialPosition + transform.forward * gapDist * (currentBounceCount + 1);

        bounceProgress = 0.5f;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (currentBounceCount >= bounceCount) return;

        bounceProgress += bounceProgressSpeed * Time.fixedDeltaTime;

        if (bounceProgress >= 1)
        {
            bounceProgress = 0;
            currentBounceCount++;
            bounceStartZ = bounceEndZ;
            bounceEndZ = initialPosition + transform.forward * gapDist * (currentBounceCount + 1);

            if (currentBounceCount == bounceCount)
            {
                Player.Instance.TakeDamage(status.CurrentAttack);
                DeactivateGameObject();
            }
        }

        float s = Mathf.Sin(bounceProgress * 180 * Mathf.Deg2Rad);
        Vector3 newPos = Vector3.Lerp(bounceStartZ, bounceEndZ, bounceProgress);
        newPos.y = s * (initialY - gapY * currentBounceCount);

        transform.localPosition = newPos;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        currentBounceCount = int.MaxValue;

        transform.SetParent(null);

        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        Vector3 dir = transform.position - IncaData.PlayerPosition;
        dir = dir.normalized * 4 + Random.onUnitSphere * 2;
        dir.Normalize();
        dir.y = Mathf.Abs(dir.y);

        rigidbody.AddForce(dir * hitPower, ForceMode.Impulse);

        Invoke(nameof(DeactivateGameObject), 3f);
    }
}
