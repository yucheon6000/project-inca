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
    [SerializeField]
    private float minRadius = 1;
    [SerializeField]
    private float maxRadius = 1.5f;
    private List<Enemy_MorningGlory_Speaker> speakers;

    [Header("[Take Damage]")]
    [SerializeField]
    private float takeDamageTime = 0.5f;

    public override void Init(DetectedObject detectedObject = null)
    {
        previousBoxIndex = -1;
        speakers = new List<Enemy_MorningGlory_Speaker>();

        transform.localPosition = GetRandomPosition();

        LookAtPlayer(true);

        base.Init(detectedObject);
    }

    protected override void InitStateMachine()
    {
        base.InitStateMachine();
        states[EnemyState.Idle] = new State_Idle(this);
        states[EnemyState.Move] = new State_Move(this);
        states[EnemyState.Die] = new State_Die(this);
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

    private Vector3 GetRandomPosition()
    {
        int boxIdx = -1;
        do
        {
            boxIdx = Random.Range(0, spawnSpaceBoxes.Count);

        } while (boxIdx == previousBoxIndex);

        LocalSpaceBox box = spawnSpaceBoxes[boxIdx];
        previousBoxIndex = boxIdx;

        return box.GetLocalPointInBox();
    }

    private void RemoveNearSpeakers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var col in colliders)
            if (col.TryGetComponent<Enemy_MorningGlory_Speaker>(out Enemy_MorningGlory_Speaker spaeker))
                spaeker.ForceKill();
    }

    private Vector3 GetRandomSpeakerPoint()
    {
        // Get a random radius between minimum and maximum.
        float radius = Random.Range(minRadius, maxRadius);

        // Get a random angle between 0 and 360.
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Set local point.
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        Vector3 localPoint = new Vector3(x, y, 0f);

        // Convert this local point to a world point.
        Vector3 worldPoint = transform.TransformPoint(localPoint);

        return worldPoint;
    }

    private void SpawnSpeaker()
    {
        int cnt = Random.Range(3, 6);

        for (int i = 0; i < cnt; ++i)
        {
            Vector3 pos = GetRandomSpeakerPoint();

            GameObject clone = MemoryPool.Instance(MemoryPoolType.Enemy)
                                .ActivatePoolItem(speakerPrefab, pos, quaternion.identity);
            clone.GetComponent<Enemy_MorningGlory_Speaker>().Init();
            clone.transform.SetParent(IncaData.UserCarTransform);

            var speaker = clone.GetComponent<Enemy_MorningGlory_Speaker>();

            speakers.Add(speaker);
        }
    }

    protected override void OnTakeDamage()
    {
        base.OnTakeDamage();
        SortSpeakersByDistance();
        PlaySpeakersTakeDamageEffect();
    }

    private void SortSpeakersByDistance()
    {
        speakers.Sort((a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
    }

    private void PlaySpeakersTakeDamageEffect()
    {
        AnimationCurve easeOutCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 1),  // 시작점 (시간 0, 값 0, 입구 기울기 0, 출구 기울기 1)
            new Keyframe(1, 1, 1, 0)   // 끝점 (시간 1, 값 1, 입구 기울기 1, 출구 기울기 0)
        );

        foreach (var speaker in speakers)
        {
            if (speaker.gameObject.activeSelf == false) continue;

            float dist = Vector3.Distance(transform.position, speaker.transform.position);
            float v = easeOutCurve.Evaluate(dist / 5f);
            float time = Mathf.LerpUnclamped(0, 1f, v);
            speaker.PlayTakeDamageEffect(time);
        }
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
                owner.ChangeState(EnemyState.Move);
        }
    }

    private class State_Move : EnemyState_Move
    {
        private Enemy_MorningGlory owner;
        public State_Move(Enemy entity) : base(entity)
            => owner = (Enemy_MorningGlory)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);

            owner.InitMoveDelayTimeAndTimer();

            owner.scaleEffector.PlayToZero(0.5f, owner.transform.localScale, AnimationCurve.EaseInOut(0, 0, 1, 1), () =>
            {
                owner.transform.localPosition = owner.GetRandomPosition();
                owner.LookAtPlayerImmediate();
                owner.RemoveNearSpeakers();
                owner.PlayAnimationByName(EnemyAnimation.Move);

                owner.scaleEffector.PlayFromZeroToOriginalScale(0.5f, AnimationCurve.EaseInOut(0, 0, 1, 1), () =>
                {
                    owner.InitMoveDelayTimeAndTimer();
                    owner.SpawnSpeaker();
                    owner.ChangeState(EnemyState.Idle);
                });
            });
        }
    }

    private class State_Die : EnemyState_Die
    {
        private Enemy_MorningGlory owner;
        public State_Die(Enemy entity) : base(entity)
            => owner = (Enemy_MorningGlory)entity;

        public override void Enter(Enemy entity)
        {
            base.Enter(entity);
            owner.SortSpeakersByDistance();
            ForceKillAllSpeakers();
        }

        private void ForceKillAllSpeakers()
        {
            AnimationCurve easeOutCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 1),  // 시작점 (시간 0, 값 0, 입구 기울기 0, 출구 기울기 1)
                new Keyframe(1, 1, 1, 0)   // 끝점 (시간 1, 값 1, 입구 기울기 1, 출구 기울기 0)
            );

            foreach (var speaker in owner.speakers)
            {
                if (speaker.gameObject.activeSelf == false) continue;

                float dist = Vector3.Distance(owner.transform.position, speaker.transform.position);
                float v = easeOutCurve.Evaluate(dist / 5f);
                float time = Mathf.LerpUnclamped(0, 1f, v);
                speaker.ForceKill(time);
            }
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