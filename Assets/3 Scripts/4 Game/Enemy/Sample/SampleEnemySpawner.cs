using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class SampleEnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject sampleEnemyPrefab;

    private void Start()
    {
        List<DetectedObject> detectedObjects = IncaDetectManager.GetAllDetectedObjects();
        foreach (DetectedObject detectedObject in detectedObjects)
            SpawnEnemy(detectedObject);

        IncaDetectManager.Instance.OnTriggerEnterDetectedObject.AddListener((DetectedObject detectedObject, bool first) =>
        {
            if (first) SpawnEnemy(detectedObject);
        });
    }

    private void SpawnEnemy(DetectedObject detectedObject)
    {
        if (detectedObject.ObjectType != DetectedObjectType.Car)
            return;

        GameObject clone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(sampleEnemyPrefab);
        Vector3 pos = detectedObject.Position;
        pos.y += detectedObject.Scale.y;
        clone.transform.position = pos;

        clone.transform.SetParent(detectedObject.transform);

        clone.GetComponent<Enemy>().Init(detectedObject);
    }
}
