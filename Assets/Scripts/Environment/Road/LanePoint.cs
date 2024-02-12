using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanePoint : MonoBehaviour
{
    [SerializeField]
    private int laneIndex;

    [SerializeField]
    private List<LanePoint> accessibleLanePoints;

    public int LaneIndex => laneIndex;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var nextPoint in accessibleLanePoints)
        {
            Gizmos.DrawLine(this.transform.position, nextPoint.transform.position);
        }
    }
}
