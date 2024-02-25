using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlock : MonoBehaviour
{
    [SerializeField]
    private List<LanePoint> lanePoints = new List<LanePoint>();
    [SerializeField]
    private List<LanePoint> startLanePoints = new List<LanePoint>();
    [SerializeField]
    private List<LanePoint> endLanePoints = new List<LanePoint>();

    [SerializeField]
    private RoadBlock test_nextRoadBlock = null;

    [ContextMenu("Sync Lane Points")]
    private void SyncLanePoints()
    {
        lanePoints = new List<LanePoint>(GetComponentsInChildren<LanePoint>());

        startLanePoints = new List<LanePoint>();
        endLanePoints = new List<LanePoint>();

        foreach (var lanePoint in lanePoints)
        {
            if (lanePoint.IsStartPoint)
                startLanePoints.Add(lanePoint);
            else if (lanePoint.IsEndPoint)
                endLanePoints.Add(lanePoint);
        }
    }

    [ContextMenu("SetNextRoadBlock")]
    public void Test_SetNextRoadBlock()
    {
        SetNextRoadBlock(test_nextRoadBlock);
    }

    public void SetNextRoadBlock(RoadBlock nextRoadBlock)
    {
        // end lane points in current raod block link start lane points in next road block (positive lanes)
        foreach (var endLanePoint in endLanePoints)
            foreach (var startLanePoint in nextRoadBlock.startLanePoints)
                if (endLanePoint.LaneIndex > 0 && endLanePoint.LaneIndex == startLanePoint.LaneIndex)
                {
                    endLanePoint.AddAccessibleLanePoint(startLanePoint);
                    break;
                }

        // end lane points in next raod block link start lane points in current road block (negative lanes)
        foreach (var endLanePoint in nextRoadBlock.endLanePoints)
            foreach (var startLanePoint in startLanePoints)
                if (endLanePoint.LaneIndex < 0 && endLanePoint.LaneIndex == startLanePoint.LaneIndex)
                {
                    endLanePoint.AddAccessibleLanePoint(startLanePoint);
                    break;
                }
    }
}
