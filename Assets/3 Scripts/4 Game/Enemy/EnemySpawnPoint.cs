using System.Collections;
using System.Collections.Generic;
using Inca;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private string gizmosIconName;

    [Header("Method")]
    [SerializeField]
    private bool spawnsOwnPosition = false;
    [SerializeField]
    private bool spawnsAsPlayerChildren = false;
    [SerializeField]
    private Vector3 localPositionInPlayer = Vector3.zero;
    [SerializeField]
    private Vector3 localRotationInPlayer = Vector3.zero;
    [SerializeField]
    private bool spawnsOnCars = false;
    [SerializeField]
    private bool sameLaneIndex = false;
    [SerializeField]
    private bool closestCar = false;

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
        GameObject enemy = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab, transform.position, transform.rotation);

        enemy.GetComponent<Enemy>().Init();
    }

    public void SpawnAsPlayerChildren()
    {
        GameObject enemy = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);

        enemy.transform.SetParent(IncaData.UserCarTransform.transform);

        enemy.transform.localPosition = localPositionInPlayer;
        enemy.transform.localRotation = Quaternion.Euler(localRotationInPlayer);

        enemy.GetComponent<Enemy>().Init();
    }

    public void SpawnOnCars()
    {
        List<DetectedObject> detectedObjects = IncaDetectManager.GetAllDetectedObjects();

        if (sameLaneIndex && closestCar)
        {
            SpawnEnemyOnClosestCar(detectedObjects);
            return;
        }

        foreach (DetectedObject detectedObject in detectedObjects)
            SpawnEnemyOnCar(detectedObject);
    }

    private void SpawnEnemyOnClosestCar(List<DetectedObject> detectedObjects)
    {
        DetectedObject targetCar = null;

        float minDist = Mathf.Infinity;

        foreach (DetectedObject detectedObject in detectedObjects)
        {
            if (detectedObject == null) continue;
            if (detectedObject.ObjectType != DetectedObjectType.Car) continue;
            if (detectedObject.EnvironmentObject.TryGetComponent<Car>(out Car car) == false) continue;
            if (car.CurrentLanePoint.LaneIndex != IncaData.UserCarLaneIndex) continue;

            float dist = Vector3.Distance(IncaData.UserCarPosition, detectedObject.Position);

            if (dist < minDist)
            {
                targetCar = detectedObject;
                minDist = dist;
            }
        }

        if (targetCar == null) return;

        Enemy[] enemies = targetCar.AvailableTransform.GetComponentsInChildren<Enemy>();
        for (int i = 0; i < enemies.Length; ++i)
        {
            Enemy enemy = enemies[i];
            if (enemy == null) continue;

            enemy.ForceKill();
        }

        GameObject clone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);

        Vector3 pos = targetCar.Position;
        pos.y += targetCar.Scale.y;
        clone.transform.position = pos;

        clone.transform.SetParent(targetCar.AvailableTransform);

        clone.GetComponent<Enemy>().Init(targetCar);
    }

    private void SpawnEnemyOnCar(DetectedObject detectedObject)
    {
        if (detectedObject == null) return;
        if (detectedObject.ObjectType != DetectedObjectType.Car) return;

        // The detected object already has an enemy.
        if (detectedObject.AvailableTransform.GetComponentsInChildren<Enemy>().Length > 0) return;

        GameObject clone = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);
        Vector3 pos = detectedObject.Position;
        pos.y += detectedObject.Scale.y;
        clone.transform.position = pos;

        clone.transform.SetParent(detectedObject.AvailableTransform);

        clone.GetComponent<Enemy>().Init(detectedObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, gizmosIconName);
    }
}
