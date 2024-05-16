using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MemoryPoolComponentWithGameObject : MonoBehaviour
{
    public static MemoryPoolComponentWithGameObject Instance { private set; get; }

    [SerializeField]
    private List<MemoryPoolPrefab> prefabs;
    protected Dictionary<GameObject, MemoryPool> pools = new Dictionary<GameObject, MemoryPool>();
    private Dictionary<GameObject, GameObject> cloneToPrefab = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if (Instance != null) return;

        Instance = this;

        Setup();
    }

    private void Setup()
    {
        foreach (var prefab in prefabs)
        {
            MemoryPool memoryPool = new MemoryPool(prefab.prefab, prefab.increaesCount);
            memoryPool.SetParent(transform);
            memoryPool.InstatiateObjects();

            pools.Add(prefab.prefab, memoryPool);
        }
    }

    public GameObject ActivatePoolItem(GameObject prefab)
    {
        if (pools.ContainsKey(prefab) == false) return null;

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
