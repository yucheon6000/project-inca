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

        IncaDetectManager.AddOnTriggerEnterDetectedObject((DetectedObject detectedObject, bool first) =>
        {
            GameObject clone = Instantiate(cubePrefab);
            clone.transform.localScale = detectedObject.Scale * scale;
            clone.transform.rotation = detectedObject.Rotation;

            detectedObjects.Add(detectedObject, clone.transform);
        });


        IncaDetectManager.AddOnTriggerExitDetectedObject((DetectedObject detectedObject) =>
        {
            GameObject clone = detectedObjects[detectedObject].gameObject;
            Destroy(clone);
            detectedObjects.Remove(detectedObject);

        });
    }

    private void Update()
    {
        List<DetectedObject> removedDetectedObjs = new List<DetectedObject>();

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
