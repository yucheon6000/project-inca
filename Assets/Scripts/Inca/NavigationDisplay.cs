using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;
using System.Reflection;

public class NavigationDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private float scale;

    private Dictionary<DetectedObject, Transform> detectedObjects = new Dictionary<DetectedObject, Transform>();

    private void Awake()
    {
        transform.localScale *= scale;

        IncaDetectionManager.AddOnTriggerEnterDetectedObject((DetectedObject detectedObject, bool first) =>
        {
            GameObject clone = Instantiate(cubePrefab);
            clone.transform.localScale = detectedObject.Scale * scale;
            clone.transform.rotation = detectedObject.Rotation;

            detectedObjects.Add(detectedObject, clone.transform);
        });


        IncaDetectionManager.AddOnTriggerExitDetectedObject((DetectedObject detectedObject) =>
        {
            GameObject clone = detectedObjects[detectedObject].gameObject;
            Destroy(clone);
            detectedObjects.Remove(detectedObject);

        });
    }

    private void Update()
    {
        foreach (var obj in detectedObjects.Keys)
        {

            if (obj == null) continue;
            Transform objTransform = detectedObjects[obj];

            if (!obj.IsVisible())
            {
                detectedObjects.Remove(obj);
                Destroy(objTransform.gameObject);
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
                detectedObjects.Remove(obj);
                Destroy(objTransform.gameObject);
            }
        }
    }
}
