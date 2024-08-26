using System;
using System.Collections.Generic;
using Inca;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_MorningGlory : DamagableEnemy
{
    [Header("[[Morning Glory]]")]
    [Header("[Spawn]")]
    [SerializeField]
    private List<LocalSpaceBox> spawnSpaceBoxes;
    private int previousBoxIndex = -1;

    [Header("[Move]")]
    [SerializeField]
    private float moveDelayTimeMin;
    [SerializeField]
    private float moveDelayTimeMax;
    private float moveDelayTime;
    private float moveTimer;

    [Header("[Attack]")]
    [SerializeField]
    private Transform speakerSpawnTransform;
    [SerializeField]
    private GameObject speakerPrefab;
    private float spawnRange;

    public override void Init(DetectedObject detectedObject = null)
    {
        previousBoxIndex = -1;

        Move();

        base.Init(detectedObject);
    }

    private void InitMoveDelayTimeAndTimer()
    {
        moveDelayTime = Random.Range(moveDelayTimeMin, moveDelayTimeMax);
        moveTimer = 0;
    }

    private bool CanMove()
    {
        moveTimer += Time.deltaTime;
        return moveTimer >= moveDelayTime;
    }

    private void Move()
    {
        int boxIdx = -1;
        do
        {
            boxIdx = Random.Range(0, spawnSpaceBoxes.Count);

        } while (boxIdx == previousBoxIndex);

        LocalSpaceBox box = spawnSpaceBoxes[boxIdx];
        transform.localPosition = box.GetLocalPointInBox();

        previousBoxIndex = boxIdx;
        InitMoveDelayTimeAndTimer();

        SpawnSpeaker();
    }

    private void SpawnSpeaker()
    {
        Instantiate(speakerPrefab, speakerSpawnTransform.position, quaternion.identity);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        states[EnemyState.Idle] = new State_Idle(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        spawnSpaceBoxes.ForEach(box => box.DrawGizmos());
    }

    private class State_Idle : EnemyState_Idle
    {
        private Enemy_MorningGlory owner;
        public State_Idle(Enemy entity) : base(entity)
            => owner = (Enemy_MorningGlory)entity;

        public override void Execute(Enemy entity)
        {
            base.Execute(entity);

            if (owner.CanMove())
                owner.Move();

        }
    }
}

[Serializable]
public class LocalSpaceBox
{
    [SerializeField]
    private Transform owner;
    [SerializeField]
    private Vector3 localCenter;
    [SerializeField]
    private Vector3 size;

    private Transform parent => owner.parent;
    private bool hasParent => parent != null;
    private Vector3 worldCenter => hasParent ? parent.TransformPoint(localCenter) : localCenter;

    public LocalSpaceBox(Transform owner, Vector3 center, Vector3 size)
    {
        this.owner = owner;
        this.localCenter = center;
        this.size = size;
    }

    public Vector3 GetWorldPointInBox()
    {
        Vector3 localPoint = GetLocalPointInBox();
        return hasParent ? parent.TransformPoint(localPoint) : localPoint;
    }

    public Vector3 GetLocalPointInBox()
    {
        return new Vector3(
            Random.Range(localCenter.x - size.x * 0.5f, localCenter.x + size.x * 0.5f),
            Random.Range(localCenter.y - size.y * 0.5f, localCenter.y + size.y * 0.5f),
            Random.Range(localCenter.z - size.z * 0.5f, localCenter.z + size.z * 0.5f)
        );
    }

    public void DrawGizmos()
    {
        Gizmos.DrawWireCube(worldCenter, size);
    }
}