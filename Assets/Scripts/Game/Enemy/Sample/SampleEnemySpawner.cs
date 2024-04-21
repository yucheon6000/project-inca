using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;
using HTC.UnityPlugin.Vive;

public class SampleEnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject sampleEnemyPrefab;

    private void Start()
    {
        IncaDetectionManager.AddOnTriggerEnterDetectedObject((DetectedObject detectedObject, bool first) =>
        {
            if (detectedObject.ObjectType != DetectedObjectType.Car)
                return;

            GameObject clone = Instantiate(sampleEnemyPrefab);
            Vector3 pos = detectedObject.Position;
            pos.y += detectedObject.Scale.y;
            clone.transform.position = pos;

            clone.transform.SetParent(detectedObject.transform);

            clone.GetComponent<Enemy>().Setup(detectedObject);
        });
    }
}
