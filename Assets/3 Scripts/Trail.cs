using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class Trail : MonoBehaviour
{
    LineRenderer lineRenderer;

    [SerializeField]
    private float trailLength = 50f;
    private bool isFinished = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(Projectile target)
    {
        isFinished = false;
        StartCoroutine(TrailRoutine(target));
    }

    public void Finish()
    {
        isFinished = true;
    }

    private IEnumerator TrailRoutine(Projectile target)
    {
        Vector3 startLocalPoint = IncaData.UserCarTransform.InverseTransformPoint(IncaData.UserRightHandPosition);
        Vector3 targetWorldPoint = target.transform.position;
        float speed = 0;

        while (trailLength > 0)
        {
            if (!isFinished)
            {
                targetWorldPoint = target.transform.position;
                speed = target.Speed;
            }
            else
            {
                trailLength -= speed * Time.deltaTime;
            }

            Vector3 startWorldPoint = IncaData.UserCarTransform.TransformPoint(startLocalPoint);

            Vector3 dir = (startWorldPoint - targetWorldPoint).normalized;
            float length = Mathf.Min(trailLength, Vector3.Distance(startWorldPoint, targetWorldPoint));

            Vector3 trailStartPoint = targetWorldPoint + dir * length;

            UpdateTrail(trailStartPoint, targetWorldPoint);

            yield return null;
        }

        lineRenderer.enabled = false;
        Destroy(this.gameObject);

        // 근데 중력이 있으면 자연스럽지가 않네 곡선으로 떨어지면
        // 직선은 좋은 것 같아
    }

    private void UpdateTrail(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
