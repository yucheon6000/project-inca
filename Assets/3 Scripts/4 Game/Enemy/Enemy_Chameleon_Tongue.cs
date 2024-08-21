using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dalak.LineRenderer3D;
using Inca;
using UnityEngine;

public class Enemy_Chameleon_Tongue : NonDamagableEnemy
{
    [Header("[[Chameleon_Tongue]]")]
    [Header("[Attack]")]
    [SerializeField]
    private int attackEnemyCount;
    [SerializeField]
    private BoxCollider targetBoxCollider;

    [Header("[Move]")]
    [SerializeField]
    private float moveTime = 1f;
    private float moveTimer;
    [SerializeField]
    private AnimationCurve moveCurve;

    private List<Transform> targets;

    private bool hasHit = false;

    private LineRenderer3D lineRenderer;

    protected override void GetMyComponents()
    {
        base.GetMyComponents();
        lineRenderer = GetComponentInChildren<LineRenderer3D>();
    }

    public override void Init(DetectedObject detectedObject = null)
    {
        moveTimer = 0;
        targets = new List<Transform>();
        hasHit = false;

        gameObject.SetActive(true);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        SetStartState(EnemyState.Move);

        states[EnemyState.Move] = new State_Move(this);
    }

    private void InitTargets()
    {
        List<GameObject> enemies = new List<GameObject>(MemoryPool.Instance(MemoryPoolType.Enemy).GetAllActivatedItems());

        enemies.RemoveAll(item => item == null || !EnemyIsInFrontOfPlayer(item.transform));

        int count = Mathf.Min(attackEnemyCount, enemies.Count);

        // Sort enemies by distance.
        enemies.Sort((a, b) =>
        {
            float distA = Vector3.Distance(transform.position, a.transform.position);
            float distB = Vector3.Distance(transform.position, b.transform.position);
            return distA.CompareTo(distB);
        });

        // Select enemies at evenly spaced intervals
        targets = new List<Transform>();
        int stepSize = enemies.Count / count;

        for (int i = 0; i < count; i++)
        {
            int index = i * stepSize;
            if (index < enemies.Count)
                targets.Add(enemies[index].transform);
        }
    }

    bool EnemyIsInFrontOfPlayer(Transform enemy)
    {
        return targetBoxCollider.bounds.Contains(enemy.transform.position);
    }

    private void UpdateStickOut()
    {
        moveTimer += Time.deltaTime;
        float progress = moveCurve.Evaluate(moveTimer / moveTime);

        UpdateLineRenderer(progress);

        if (moveTimer >= moveTime)
            Attack();
    }

    protected override void Attack()
    {
        base.Attack();
        Player.Instance.TakeDamage(status.CurrentAttack);
        hasHit = true;
        moveTimer = moveTime;
    }

    private void UpdateStickIn()
    {
        moveTimer -= Time.deltaTime;
        float progress = moveCurve.Evaluate(moveTimer / moveTime);

        if (progress <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        UpdateLineRenderer(progress);
    }

    private void UpdateLineRenderer(float progress)
    {
        targets.RemoveAll(item => item == null || !EnemyIsInFrontOfPlayer(item));

        List<Transform> validTargets = new List<Transform>(targets);
        validTargets.Insert(0, transform);
        validTargets.Add(GGData.PlayerTransform);

        // Calculate total distance.
        float totalDist = 0;
        Vector3 prevPoint = transform.position;

        Vector3[] enemyPositions = validTargets.Select(i =>
        {
            totalDist += Vector3.Distance(prevPoint, i.position);
            prevPoint = i.position;
            return i.position;
        }).ToArray();

        // Calculate target distance this tongue should move.
        float targetDist = Mathf.Lerp(0, totalDist, progress);
        if (targetDist == 0) return;

        // Colect points to give to LineRenderer.
        List<Vector3> points = new List<Vector3>();
        points.Add(transform.position);

        Vector3 startPoint = transform.position;
        for (int i = 1; i < enemyPositions.Length; i++)
        {
            Vector3 endPoint = enemyPositions[i];
            float distToEndPosition = Vector3.Distance(startPoint, endPoint);

            if (targetDist < distToEndPosition)
            {
                points.Add(startPoint + (endPoint - startPoint).normalized * targetDist);
                break;
            }
            else
            {
                points.Add(endPoint);
                startPoint = endPoint;
                targetDist -= distToEndPosition;
            }
        }

        Vector3[] result = points.ToArray();
        transform.InverseTransformPoints(result);

        // Update the LineRenderer's points.
        lineRenderer.pathData.positions.Clear();
        lineRenderer.pathData.positions = new List<Vector3>(result);
        lineRenderer.UpdateMesh();
    }

    private class State_Move : EnemyState_Move
    {
        private Enemy_Chameleon_Tongue owner;

        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_Chameleon_Tongue)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.InitTargets();
        }

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.hasHit == false)
                owner.UpdateStickOut();
            else
                owner.UpdateStickIn();
        }
    }
}
