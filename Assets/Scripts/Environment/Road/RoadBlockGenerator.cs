using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RoadBlock))]
public class RoadBlockGenerator : MonoBehaviour
{
    [Header("Raod Block Information")]
    public Vector3 roadBlockSize = new Vector3(500, 1, 500);

    [Header("Prefab")]
    public GameObject lanePointPrefab;

    public RoadInformation roadInformation = new RoadInformation();

    private RoadInformation ri => roadInformation;

    public void GenerateRoadBlock()
    {
        List<List<LanePoint>> lanes = new List<List<LanePoint>>();

        for (int i = 0; i < ri.laneCount; ++i)
        {
            int reverse = ri.reverse ? -1 : 1;
            int laneIndex = (i + 1) * reverse;  // idx starts from 1 or -1

            GameObject lane = new GameObject($"Lane {laneIndex}");

            lane.transform.SetParent(gameObject.transform);
            lane.transform.localPosition = new Vector3(ri.laneInitialGapX + ri.laneGapX * i, 0, 0);

            float length = roadBlockSize.z - ri.lanePointInitialGapZ;
            float lanePointGap = length / (ri.lanePointCount - 1);

            List<LanePoint> points = new List<LanePoint>();

            for (int j = 0; j < ri.lanePointCount; ++j)
            {
                GameObject clone = PrefabUtility.InstantiatePrefab(lanePointPrefab) as GameObject;
                LanePoint point = clone.GetComponent<LanePoint>();

                point.LaneIndex = laneIndex;

                if (j == 0) point.IsStartPoint = true;
                else if (j == ri.lanePointCount - 1) point.IsEndPoint = true;

                point.transform.SetParent(lane.transform);
                point.transform.localPosition = new Vector3(0, 0, -(length - lanePointGap * j));

                points.Add(point);
            }

            lanes.Add(points);

            if (ri.reverse)
            {
                lane.transform.rotation = Quaternion.Euler(0, 180, 0);
                lane.transform.localPosition = new Vector3(-ri.laneInitialGapX + ri.laneGapX * -i, 0, -roadBlockSize.z);
            }
        }

        LinkEachLanePoint(lanes);

        GetComponent<RoadBlock>().SyncLanePoints();
    }

    private void LinkEachLanePoint(List<List<LanePoint>> lanes)
    {
        for (int i = 0; i < lanes.Count; ++i)
        {
            List<LanePoint> points = lanes[i];

            for (int j = 0; j < points.Count - 1; ++j)
            {
                LanePoint point = points[j];

                // to straight
                point.AddAccessibleLanePoint(lanes[i][j + 1]);

                // to right 
                if (i != lanes.Count - 1)
                    point.AddAccessibleLanePoint(lanes[i + 1][j + 1]);

                // to left   
                if (i != 0)
                    point.AddAccessibleLanePoint(lanes[i - 1][j + 1]);
            }
        }
    }

    [Serializable]
    public class RoadInformation
    {
        [Header("Lane")]
        [Min(1)]
        public int laneCount = 1;
        [Min(0)]
        public float laneGapX;
        [Min(0)]
        public float laneInitialGapX;

        [Header("Lane Point")]
        [Min(2)]
        public int lanePointCount = 2;
        [Min(0)]
        public float lanePointInitialGapZ;

        [Header("Reverse")]
        public bool reverse;
    }
}
