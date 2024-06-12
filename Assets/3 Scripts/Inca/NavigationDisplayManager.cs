using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;
using System.Reflection;

public class NavigationDisplayManager : IncaManager
{
    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private float scale;

    private Dictionary<DetectedObject, Transform> detectedObjects = new Dictionary<DetectedObject, Transform>();

    private void Awake()
    {
        transform.localScale *= scale;

        IncaDetectManager.GetAllDetectedObjects().ForEach(detObj =>
        {
            GameObject clone = Instantiate(cubePrefab);
            clone.transform.localScale = detObj.Scale * scale;
            clone.transform.rotation = detObj.Rotation;

            detectedObjects.Add(detObj, clone.transform);
        });

        IncaDetectManager.AddOnTriggerEnterDetectedObject((DetectedObject detObj, bool first) =>
        {
            GameObject clone = Instantiate(cubePrefab);
            clone.transform.localScale = detObj.Scale * scale;
            clone.transform.rotation = detObj.Rotation;

            detectedObjects.Add(detObj, clone.transform);
        });


        IncaDetectManager.AddOnTriggerExitDetectedObject((DetectedObject detectedObject) =>
        {
            // GameObject clone = detectedObjects[detectedObject].gameObject;
            // Destroy(clone);
            // detectedObjects.Remove(detectedObject);

        });
    }

    private void Update()
    {
        List<DetectedObject> removedDetectedObjs = new List<DetectedObject>();

        transform.rotation = IncaData.PlayerCarTransform.rotation;

        foreach (var obj in detectedObjects.Keys)
        {

            if (obj == null) continue;
            Transform objTransform = detectedObjects[obj];

            if (!obj.IsVisible())
            {
                removedDetectedObjs.Add(obj);
                continue;
            }

            try
            {
                Vector3 toObj = obj.Position - IncaData.PlayerPosition;
                Vector3 rToObj = toObj * scale;
                rToObj = transform.position + rToObj;

                objTransform.position = rToObj;
                objTransform.rotation = obj.Rotation;
            }
            catch (MissingReferenceException)
            {
                removedDetectedObjs.Add(obj);
            }
        }


        foreach (var obj in removedDetectedObjs)
        {
            Transform objTransform = detectedObjects[obj];
            detectedObjects.Remove(obj);
            Destroy(objTransform.gameObject);
        }
    }
}
