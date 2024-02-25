using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanePoint : MonoBehaviour
{
    [SerializeField]
    private int laneIndex;

    [SerializeField]
    private bool isStartPoint = false;
    [SerializeField]
    private bool isEndPoint = false;

    [SerializeField]
    private List<LanePoint> accessibleLanePoints = new List<LanePoint>();

    public int LaneIndex => laneIndex;
    public bool IsStartPoint => isStartPoint;
    public bool IsEndPoint => isEndPoint;
    public Vector3 Position => transform.position;

    public LanePoint GetNextLanePoint(int targetLaneIndex = 0)
    {
        if (targetLaneIndex == 0)
            targetLaneIndex = this.laneIndex;

        foreach (var nextPoint in accessibleLanePoints)
        {
            if (nextPoint.LaneIndex == targetLaneIndex)
                return nextPoint;
        }

        return accessibleLanePoints[0];
    }

    public void AddAccessibleLanePoint(LanePoint accessibleLanePoint)
    {
        accessibleLanePoints.Add(accessibleLanePoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var nextPoint in accessibleLanePoints)
        {
            if (!nextPoint) continue;
            Gizmos.DrawLine(this.transform.position, nextPoint.transform.position);
        }
    }
}
