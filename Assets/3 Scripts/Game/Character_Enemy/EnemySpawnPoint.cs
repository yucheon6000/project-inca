using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [Header("Method")]
    [SerializeField]
    private bool spawnsOwnPosition = false;
    [SerializeField]
    private bool spawnsAsPlayerChildren = false;
    [SerializeField]
    private bool spawnsOnCars = false;

    private const string ENEMY_SPAWN_RANGE_TAG = "Game_Enemy Spawn Range";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ENEMY_SPAWN_RANGE_TAG)) return;

        if (spawnsOwnPosition) SpawnOwnPosition();
        if (spawnsAsPlayerChildren) SpawnAsPlayerChildren();
        if (spawnsOnCars) SpawnOnCars();
    }

    public void SpawnOwnPosition()
    {
        GameObject enemy = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;

        enemy.GetComponent<Enemy>().Init();
    }

    public void SpawnAsPlayerChildren()
    {
        GameObject enemy = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);

        enemy.transform.SetParent(IncaData.PlayerCarTransform.transform);

        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.localRotation = Quaternion.identity;

        enemy.GetComponent<Enemy>().Init();
    }

    public void SpawnOnCars()
    {
        List<DetectedObject> detectedObjects = IncaDetectManager.GetAllDetectedObjects();
        foreach (DetectedObject detectedObject in detectedObjects)
            SpawnEnemyOnCar(detectedObject);
    }

    private void SpawnEnemyOnCar(DetectedObject detectedObject)
    {
        if (detectedObject.ObjectType != DetectedObjectType.Car)
            return;

        // The detected object already has an enemy.
        if (detectedObject.transform.GetComponentsInChildren<Enemy>().Length > 0) return;

        GameObject clone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);
        Vector3 pos = detectedObject.Position;
        pos.y += detectedObject.Scale.y;
        clone.transform.position = pos;

        clone.transform.SetParent(detectedObject.transform);

        clone.GetComponent<Enemy>().Init(detectedObject);
    }
}
