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

    // To check which vehicles want to come to this point.
    private GameObject user = null;
    public GameObject User => user;
    public bool HasUser => user ? true : false;

    private List<GameObject> users = new List<GameObject>();

    public LanePoint GetNextLanePoint(int targetLaneIndex = 0)
    {
        if (targetLaneIndex == 0)
            targetLaneIndex = this.laneIndex;

        foreach (var nextPoint in accessibleLanePoints)
        {
            if (nextPoint.LaneIndex == targetLaneIndex)
                return nextPoint;
        }

        return accessibleLanePoints.Count == 0 ? null : accessibleLanePoints[0];
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
