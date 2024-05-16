using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadBlockDirection { North = 0, South = 180, East = 90, West = 270 }
public class RoadBlock : MonoBehaviour
{
    public static readonly Vector3 road_block_size = new Vector3(500, 1, 500);

    [SerializeField]
    private List<LanePoint> lanePoints = new List<LanePoint>();
    [SerializeField]
    private List<LanePoint> startLanePoints = new List<LanePoint>();
    [SerializeField]
    private List<LanePoint> endLanePoints = new List<LanePoint>();

    [SerializeField]
    private RoadBlock test_nextRoadBlock = null;
    private RoadBlock nextRoadBlock = null;

    [Space]
    [SerializeField]
    private RoadBlockDirection currentDirection = RoadBlockDirection.North;     // 절대 방향
    public RoadBlockDirection CurrentDirection => currentDirection;
    [SerializeField]
    private RoadBlockDirection nextDirection = RoadBlockDirection.North;        // 상대 방향
    public RoadBlockDirection NextDirection => nextDirection;

    /// <summary>
    /// Find start lane points and end lane points. And it is added to each lists.
    /// It is a feature for only Inspector or only RoadBlockGenerator!
    /// You must not call this method inside normal methods.
    /// </summary>
    [ContextMenu("Sync Lane Points")]
    public void SyncLanePoints()
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

    [ContextMenu("Set Next Road Block")]
    public void Test_SetNextRoadBlock()
    {
        SetNextRoadBlock(test_nextRoadBlock);
    }

    public void Setup(RoadBlockDirection currentDirection)
    {
        this.currentDirection = currentDirection;
        transform.rotation = Quaternion.Euler(0, (int)currentDirection, 0);
        triggerEnter = false;
    }

    private bool triggerEnter = false;
    public bool OnTriggerEnterCar(Vector3 targetWorldPosition, float triggerGap)
    {
        if (triggerEnter) return false;

        float a = 0, b = 0;

        if (currentDirection == RoadBlockDirection.North)
        {
            a = transform.position.z + (road_block_size.z / 2);
            b = targetWorldPosition.z;
        }
        else if (currentDirection == RoadBlockDirection.South)
        {
            a = transform.position.z - (road_block_size.z / 2);
            b = targetWorldPosition.z;
        }
        else if (currentDirection == RoadBlockDirection.East)
        {
            a = transform.position.x + (road_block_size.z / 2);
            b = targetWorldPosition.x;
        }
        else if (currentDirection == RoadBlockDirection.West)
        {
            a = transform.position.x - (road_block_size.z / 2);
            b = targetWorldPosition.x;
        }

        if (Mathf.Abs(a - b) < triggerGap)
        {
            triggerEnter = true;
        }

        if (currentDirection == RoadBlockDirection.North || currentDirection == RoadBlockDirection.South)
        {
            Debug.DrawLine(new Vector3(transform.position.x, 1, a), new Vector3(transform.position.x, 1, b), Color.red);
        }
        else
        {
            Debug.DrawLine(new Vector3(a, 1, transform.position.z), new Vector3(b, 1, transform.position.z), Color.red);
        }

        return triggerEnter;
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

    private void OnDisable()
    {
        // For next using this road block, It remove linking between this road block and next road blcok.
        foreach (var point in endLanePoints)
            point.ClearAccessibleLanePoints();

        foreach (var point in nextRoadBlock.endLanePoints)
            point.ClearAccessibleLanePoints();
    }
}
