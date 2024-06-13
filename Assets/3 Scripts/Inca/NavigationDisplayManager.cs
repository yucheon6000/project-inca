using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;
using System.Reflection;

public class NavigationDisplayManager : IncaManager
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private GameObject buildingPrefab;

    [Header("Line Renderer")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Car userCar;
    [SerializeField]
    private int lanePointCount = 10;



    [SerializeField]
    private float scale;

    private Dictionary<DetectedObject, Transform> detectedObjects = new Dictionary<DetectedObject, Transform>();

    private void Awake()
    {
        transform.localScale *= scale;

        IncaDetectManager.AddOnTriggerEnterDetectedObject((DetectedObject detObj, bool first) =>
        {
            GameObject clone = null;

            if (detObj.ObjectType == DetectedObjectType.Car)
                clone = Instantiate(carPrefab, new Vector3(0, -300, 0), Quaternion.identity);
            else if (detObj.ObjectType == DetectedObjectType.Building)
            {
                clone = Instantiate(buildingPrefab, new Vector3(0, -300, 0), Quaternion.identity);
                clone.transform.localScale = detObj.Scale * scale;
            }
            clone.transform.rotation = detObj.Rotation;

            detectedObjects.Add(detObj, clone.transform);
        });
    }

    private void Update()
    {
        List<DetectedObject> removedDetectedObjs = new List<DetectedObject>();

        transform.rotation = IncaData.PlayerCarTransform.rotation;

        // Move objects to their position
        foreach (var obj in detectedObjects.Keys)
        {
            if (obj == null || !obj.IsVisible())
            {
                removedDetectedObjs.Add(obj);
                continue;
            }

            Transform objTransform = detectedObjects[obj];

            try
            {
                objTransform.position = CovertToOurCoordinate(obj.Position);
                objTransform.rotation = obj.Rotation;
            }
            catch (MissingReferenceException)
            {
                removedDetectedObjs.Add(obj);
            }
        }


        // Delete objects which don't use anymore.
        foreach (var obj in removedDetectedObjs)
        {
            Transform objTransform = detectedObjects[obj];
            detectedObjects.Remove(obj);
            Destroy(objTransform.gameObject);
        }

        // Update line renderer.
        UpdateLineRenderer();
    }

    List<Vector3> points = new List<Vector3>();     // Points the LineRenderer uses
    private void UpdateLineRenderer()
    {
        // Remove all items from the points list.
        points.Clear();

        LanePoint lanePoint = userCar.NextLanePoint;

        Vector3 nextLanePointPos = CovertToOurCoordinate(lanePoint.Position);
        Vector3 startPoint = transform.position;

        startPoint.y = nextLanePointPos.y;

        points.Add(startPoint);

        for (int i = 0; i < lanePointCount; ++i)
        {
            nextLanePointPos = CovertToOurCoordinate(lanePoint.Position);
            points.Add(nextLanePointPos);

            lanePoint = lanePoint.GetNextLanePoint(userCar.TargetLaneIndex);
            if (lanePoint == null) break;
        }

        // Set points to the points of the LineRenderer.
        lineRenderer.SetPositions(points.ToArray());
    }

    private Vector3 CovertToOurCoordinate(Vector3 point)
    {
        Vector3 toObj = point - IncaData.PlayerPosition;            // A direction vector

        Vector3 result = transform.position + (toObj * scale);

        return result;
    }
}
