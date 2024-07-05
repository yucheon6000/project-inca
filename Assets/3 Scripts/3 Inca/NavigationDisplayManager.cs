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

    /// <typeparam name="DetectedObject">Detected Object</typeparam>
    /// <typeparam name="Transform">Model Transform</typeparam>
    private Dictionary<DetectedObject, Transform> detectedObjects = new Dictionary<DetectedObject, Transform>();

    private void Awake()
    {
        transform.localScale *= scale;

        IncaDetectManager.AddOnTriggerEnterDetectedObject((DetectedObject detObj, bool first) =>
        {
            GameObject clone = null;

            // Create a model based on the type of the detected object.
            if (detObj.ObjectType == DetectedObjectType.Car)
                clone = MemoryPool.Instance(MemoryPoolType.Inca_Navigation).ActivatePoolItem(carPrefab);
            else if (detObj.ObjectType == DetectedObjectType.Building)
            {
                clone = MemoryPool.Instance(MemoryPoolType.Inca_Navigation).ActivatePoolItem(buildingPrefab);
                clone.transform.localScale = detObj.Scale * scale;
            }

            clone.transform.position = new Vector3(0, -300, 0);

            // Add the model to the dictionary.
            detectedObjects.Add(detObj, clone.transform);

            // When the detected object is hiden, remove it and model object.
            detObj.RegisterOnHideAction(() => RemoveDetectedObject(detObj));
        });
    }

    private void Update()
    {
        transform.rotation = IncaData.PlayerCarTransform.rotation;

        List<DetectedObject> removedDetectedObjs = new List<DetectedObject>();

        // Move objects to their position
        foreach (var detObj in detectedObjects.Keys)
            UpdateModelPositionAndRotation(detObj, ref removedDetectedObjs);


        // Delete objects which don't use anymore.
        foreach (var detObj in removedDetectedObjs)
            RemoveDetectedObject(detObj);


        // Update line renderer.
        UpdateLineRenderer();
    }

    private void UpdateModelPositionAndRotation(DetectedObject detectedObject, ref List<DetectedObject> removedDetectedObejcts)
    {
        // If the detected object is not visible, remove it.
        if (detectedObject == null || !detectedObject.IsVisible())
        {
            removedDetectedObejcts.Add(detectedObject);
            return;
        }

        // Update position of the model and rotation of the model.
        Transform modelTf = detectedObjects[detectedObject];

        try
        {
            modelTf.position = CovertToOurCoordinate(detectedObject.Position);
            modelTf.rotation = detectedObject.Rotation;
        }

        // If the model can't be moved by this script, remove the detected obejct.
        catch (MissingReferenceException)
        {
            removedDetectedObejcts.Add(detectedObject);
        }
    }

    private void RemoveDetectedObject(DetectedObject detectedObject)
    {
        if (!detectedObjects.ContainsKey(detectedObject)) return;

        Transform clone = detectedObjects[detectedObject];
        MemoryPool.Instance(MemoryPoolType.Inca_Navigation).DeactivatePoolItem(clone.gameObject);

        detectedObjects.Remove(detectedObject);
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
