using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class MemoryPoolComponentWithEnum<T> : MonoBehaviour where T : Enum
{
    public static MemoryPoolComponentWithEnum<T> Instance { private set; get; }

    [SerializeField]
    private List<MemoryPoolPrefab<T>> prefabs;
    protected Dictionary<T, MemoryPool> pools = new Dictionary<T, MemoryPool>();
    private Dictionary<GameObject, T> cloneToType = new Dictionary<GameObject, T>();

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

            pools.Add(prefab.type, memoryPool);
        }
    }

    public GameObject ActivatePoolItem(T type)
    {
        if (pools.ContainsKey(type) == false) return null;

        GameObject result = pools[type].ActivatePoolItem();
        cloneToType.Add(result, type);

        return result;
    }

    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (cloneToType.ContainsKey(removeObject) == false) return;

        T type = cloneToType[removeObject];
        pools[type].DeactivatePoolItem(removeObject);

        cloneToType.Remove(removeObject);
    }

    [Serializable]
    private class MemoryPoolPrefab<K> where K : Enum
    {
        public T type;
        public GameObject prefab;
        public int increaesCount = 10;
    }
}
