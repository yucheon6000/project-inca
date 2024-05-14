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
        List<DetectedObject> detectedObjects = IncaDetectionManager.GetAllDetectedObjects();
        foreach (DetectedObject detectedObject in detectedObjects)
            SpawnEnemy(detectedObject);

        IncaDetectionManager.AddOnTriggerEnterDetectedObject((DetectedObject detectedObject, bool first) =>
        {
            if (first) SpawnEnemy(detectedObject);
        });
    }

    private void SpawnEnemy(DetectedObject detectedObject)
    {
        if (detectedObject.ObjectType != DetectedObjectType.Car)
            return;

        GameObject clone = Instantiate(sampleEnemyPrefab);
        Vector3 pos = detectedObject.Position;
        pos.y += detectedObject.Scale.y;
        clone.transform.position = pos;

        clone.transform.SetParent(detectedObject.transform);

        clone.GetComponent<Enemy>().Setup(detectedObject);
    }
}
