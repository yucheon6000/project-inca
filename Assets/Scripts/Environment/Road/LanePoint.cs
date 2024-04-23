using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LanePoint : MonoBehaviour
{
    [SerializeField]
    private int laneIndex;
    public int LaneIndex { set { laneIndex = value; } get => laneIndex; }

    [SerializeField]
    private bool isStartPoint = false;
    public bool IsStartPoint { set { isStartPoint = value; } get => isStartPoint; }
    [SerializeField]
    private bool isEndPoint = false;
    public bool IsEndPoint { set { isEndPoint = value; } get => isEndPoint; }

    [SerializeField]
    private RoadBlockDirection roadBlockDirection = RoadBlockDirection.North;
    public RoadBlockDirection RoadBlockDirection => roadBlockDirection;

    [SerializeField]
    private List<LanePoint> accessibleLanePoints = new List<LanePoint>();

    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;

    // To check which vehicles want to come to this point.
    private GameObject user = null;
    public GameObject User => user;
    public bool HasUser => user ? true : false;

    private List<GameObject> users = new List<GameObject>();

    private float allStopTimer = 0;

    private void Update()
    {
        if (users.Count == 0)
        {
            allStopTimer = 0;
            return;
        }

        foreach (var user in users)
        {
            if (user.GetComponent<Car>()?.CurrentState != CarStates.Stop)
            {
                allStopTimer = 0;
                return;
            }
        }

        allStopTimer += Time.deltaTime;

        if (allStopTimer >= 1)
        {
            OrderUsersByDistance();
            print("Ordered");
            allStopTimer = 0;
        }
    }

    public void OrderUsersByDistance()
    {
        users.OrderBy((GameObject x) => (x.transform.position - transform.position).sqrMagnitude);
    }

    public LanePoint GetNextLanePoint(int targetLaneIndex = 0)
    {
        if (targetLaneIndex == 0)
            targetLaneIndex = this.laneIndex;

        LanePoint nextPoint = accessibleLanePoints[0];

        foreach (var point in accessibleLanePoints)
        {
            if (Mathf.Abs(targetLaneIndex - point.laneIndex) <= Mathf.Abs(targetLaneIndex - nextPoint.laneIndex))
                nextPoint = point;
        }

        return nextPoint;
    }

    public void AddAccessibleLanePoint(LanePoint accessibleLanePoint)
    {
        accessibleLanePoints.Add(accessibleLanePoint);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = laneIndex >= 0 ? Color.blue : Color.red;
        foreach (var nextPoint in accessibleLanePoints)
        {
            if (!nextPoint) continue;
            if (nextPoint.laneIndex >= 0) Gizmos.color = Color.blue;
            else Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, nextPoint.transform.position);
        }

        if (isStartPoint)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        if (isEndPoint)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }

    public bool CanICome(GameObject user)
    {
        if (users.Count == 0) return false;
        return users[0] == user;
    }

    public void RegisterUser(GameObject user)
    {
        if (!users.Contains(user))
            users.Add(user);
    }

    public void DeregisterUser(GameObject user)
    {
        if (users.Contains(user))
            users.Remove(user);
    }
}
