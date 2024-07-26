using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Enemy_Fly : DamagableEnemy
{
    private StateMachine<Enemy_Fly> stateMachine;

    [SerializeField]
    protected StateMonoBehaviour<Enemy_Fly> flyingState;

    [SerializeField]
    private Transform modelTransform;

    [Header("Wander")]
    private List<Vector3> wanderPositions;                      // Local positions where this game object should arrive.
    [SerializeField]
    private int wanderPositionIndex = 0;
    [SerializeField]
    private Vector3 currentWanderPosition = Vector3.zero;       // The local position where where game object is currently targeting.
    public Vector3 CurrentWanderPosition => currentWanderPosition;

    [Space]
    [SerializeField]
    private int wanderPositionCount = 5;
    [SerializeField]
    protected Vector3 wanderPositionRangeMin;     // Local position
    [SerializeField]
    protected Vector3 wanderPositionRangeMax;     // Local position
    [SerializeField]
    private bool flipY = false;
    [SerializeField]
    private bool randomFlipY = false;

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;

    [Header("Children")]
    [SerializeField]
    private bool isChild = false;
    private bool IsParent => !isChild;
    [SerializeField]
    private List<Enemy_Fly> children = new List<Enemy_Fly>();

    [Space]
    [SerializeField]
    private bool isUsedByItself = false;
    [SerializeField]
    private bool usesCurrentPositionZAsRangeZ = false;

    protected override void Awake()
    {
        base.Awake();

        originLocalPosition = transform.localPosition;
        originLocalRotation = transform.localRotation;
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        base.Init(detectedObject);

        if (!isUsedByItself)
        {
            transform.localPosition = originLocalPosition;
            transform.localRotation = originLocalRotation;
        }
        else
        {
            transform.SetParent(GGData.PlayerTransform);
        }

        SetWanderPositionsToMeAndChildren();

        gameObject.SetActive(true);

        stateMachine = new StateMachine<Enemy_Fly>();
        stateMachine.Setup(this, flyingState);

        PlayAnimationByValue(Constants.Animation.ENEMY_ANIMATION_MOVE);
    }

    private void SetWanderPositionsToMeAndChildren()
    {
        if (isChild) return;

        /* If this is parent, this sets children's wanderPotisions. */

        if (randomFlipY)
            flipY = Random.Range(0, 2) == 1 ? true : false;

        if (usesCurrentPositionZAsRangeZ)
            wanderPositionRangeMax.z = transform.localPosition.z;

        SetWanderPositions(GetWanderPositions(wanderPositionCount));

        // Set children's wanderPositions.
        // Children have to pass parent's start position.
        List<Vector3> wanderPositionsForChildren = new List<Vector3>(wanderPositions);
        wanderPositionsForChildren.Insert(0, this.transform.localPosition);

        children.ForEach(
            child => child.SetWanderPositions(wanderPositionsForChildren)
        );
    }

    protected virtual void FixedUpdate()
    {
        stateMachine?.Execute();
    }

    public void HasReachedCurrentWanderPosition()
    {
        if (wanderPositionIndex == wanderPositions.Count)
        {
            AttackPlayer();
        }
        else
        {
            SetCurrentWanderPosition(wanderPositions[wanderPositionIndex]);
            wanderPositionIndex++;
        }
    }

    public void SetWanderPositions(List<Vector3> wanderPositions)
    {
        wanderPositionIndex = 0;
        this.wanderPositions = wanderPositions;
        SetCurrentWanderPosition(wanderPositions[wanderPositionIndex]);
    }

    private List<Vector3> GetWanderPositions(int count)
    {
        List<Vector3> result = new List<Vector3>();

        float gapZ = (wanderPositionRangeMax.z - wanderPositionRangeMin.z) / (count - 1);

        for (int i = 0; i < count - 1; ++i)    // count-1: The last point is player's position.
        {
            float x = Random.Range(wanderPositionRangeMin.x, wanderPositionRangeMax.x);

            float y = 0;
            if (flipY) y = i % 2 == 0 ? wanderPositionRangeMax.y : wanderPositionRangeMin.y + Random.Range(-0.5f, 0.5f);
            else y = i % 2 == 0 ? wanderPositionRangeMin.y : wanderPositionRangeMax.y + Random.Range(-0.5f, 0.5f);

            float z = wanderPositionRangeMax.z - gapZ * i;

            Vector3 wanderPos = new Vector3(x, y, z);

            result.Add(wanderPos);
        }

        // Add player's local position.
        result.Add(transform.parent.InverseTransformPoint(GGData.PlayerPosition));

        return result;
    }

    public void SetCurrentWanderPosition(Vector3 wanderPosition)
    {
        currentWanderPosition = wanderPosition + Random.insideUnitSphere * 0.2f;
    }

    public void AttackPlayer()
    {
        Player.Instance.TakeDamage(Status.CurrentAttack);
        ForceKill();
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if (isUsedByItself)
            DeactivateGameObject();
        else
        {
            gameObject.SetActive(false);
            SpawnEffect(dieEffectPrefab);
        }
    }

    public void LookAt(Vector3 direction)
    {
        modelTransform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnDrawGizmos()
    {
        if (transform.parent == null) return;
        if (CurrentWanderPosition == null) return;

        // Draw current wander position.
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.parent.TransformPoint(CurrentWanderPosition), 0.2f);

        // Draw a range where this game object can be located.
        Vector3 rangeMin = transform.parent.TransformPoint(wanderPositionRangeMin);
        Vector3 rangeMax = transform.parent.TransformPoint(wanderPositionRangeMax);

        Vector3 center = new Vector3(
            (rangeMin.x + rangeMax.x) / 2,
            (rangeMin.y + rangeMax.y) / 2,
            (rangeMin.z + rangeMax.z) / 2
        );

        Vector3 size = new Vector3(
            rangeMax.x - rangeMin.x,
            rangeMax.y - rangeMin.y,
            rangeMax.z - rangeMin.z
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

        // if (wanderPositions != null && wanderPositions.Count > 0)
        //     Gizmos.DrawSphere(wanderPositions[wanderPositions.Count - 1], 1);
    }
}
