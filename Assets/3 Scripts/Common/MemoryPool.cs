using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MemoryPoolType
{
    Enviroments, RoadBlocks,
    DetectedObject,
    Enemy
}
public class MemoryPool : MonoBehaviour
{
    private static Dictionary<MemoryPoolType, MemoryPool> instances = null;

    public static MemoryPool Instance(MemoryPoolType poolType)
    {
        if (instances.ContainsKey(poolType) == false)
        {
            Debug.LogError($"MemoryPool: You have to set '{poolType.ToString()}' memory pool");
            return null;
        }

        return instances[poolType];
    }

    [SerializeField]
    private MemoryPoolType memoryPoolType;

    [SerializeField]
    private List<MemoryPoolPrefab> prefabs;
    protected Dictionary<GameObject, MemoryPoolBase> pools = new Dictionary<GameObject, MemoryPoolBase>();
    private Dictionary<GameObject, GameObject> cloneToPrefab = new Dictionary<GameObject, GameObject>();

    [SerializeField]
    private int defaultIncreaesCount = 1;

    private void Awake()
    {
        if (instances == null)
            instances = new Dictionary<MemoryPoolType, MemoryPool>();

        if (instances.ContainsKey(memoryPoolType) == false)
            Setup();
    }

    private void Setup()
    {
        foreach (var prefab in prefabs)
            AddMemeoryPoolBase(prefab.prefab, prefab.increaesCount, transform);

        instances.Add(memoryPoolType, this);
    }

    private void AddMemeoryPoolBase(GameObject prefab, int increaesCount, Transform transform)
    {
        MemoryPoolBase memoryPool = new MemoryPoolBase(prefab, increaesCount, transform);
        memoryPool.InstatiateObjects();

        pools.Add(prefab, memoryPool);
    }

    public GameObject ActivatePoolItem(GameObject prefab)
    {
        if (pools.ContainsKey(prefab) == false)
        {
            AddMemeoryPoolBase(prefab, defaultIncreaesCount, transform);
            return ActivatePoolItem(prefab);
        }

        GameObject result = pools[prefab].ActivatePoolItem();
        cloneToPrefab.Add(result, prefab);

        return result;
    }

    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (cloneToPrefab.ContainsKey(removeObject) == false) return;

        GameObject prefab = cloneToPrefab[removeObject];
        pools[prefab].DeactivatePoolItem(removeObject);

        cloneToPrefab.Remove(removeObject);
    }

    [Serializable]
    private class MemoryPoolPrefab
    {
        public GameObject prefab;
        public int increaesCount = 10;
    }
}
