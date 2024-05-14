using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    private int increaesCount = 10;     // Number of objects created at once
    private int maxCount = 0;           // Number of objects created so far
    private int activeCount = 0;        // Number of active objects

    private GameObject poolGameObject;
    private List<PoolItem> poolItems;

    public int MaxCount => maxCount;
    public int ActiveCount => activeCount;

    public MemoryPool(GameObject poolGameObject)
    {
        this.poolGameObject = poolGameObject;
    }

    public MemoryPool(GameObject poolGameObject, int increaesCount) : this(poolGameObject)
    {
        this.increaesCount = increaesCount;
    }

    /// <summary>
    /// Create as many pool objects as increaseCount
    /// </summary>
    private void InstatiateObjects()
    {
        maxCount += increaesCount;

        for (int i = 0; i < increaesCount; ++i)
        {
            PoolItem item = new PoolItem();

            item.isActive = false;
            item.gameObject = GameObject.Instantiate(poolGameObject);
            item.gameObject.SetActive(false);

            poolItems.Add(item);
        }
    }

    /// <summary>
    /// Destory all pool objects
    /// </summary>
    private void DestroyAllObjects()
    {
        if (poolItems == null) return;

        foreach (var item in poolItems)
            GameObject.Destroy(item.gameObject);

        poolItems.Clear();
    }

    /// <summary>
    /// Get a game object that you can use.
    /// If you don't use the game object anymore, Call 'DeactivatePoolItem' method with the game object.
    /// </summary>
    public GameObject ActivatePoolItem()
    {
        if (poolItems == null) return null;

        if (maxCount == activeCount) InstatiateObjects();

        foreach (var item in poolItems)
        {
            if (!item.isActive)
            {
                activeCount++;

                item.isActive = true;
                item.gameObject.SetActive(true);

                return item.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// It makes the game object (parameter) inactive
    /// </summary>
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItems == null || removeObject == null) return;

        foreach (var item in poolItems)
        {
            if (item.isActive && item.gameObject == removeObject)
            {
                activeCount--;

                item.isActive = false;
                item.gameObject.SetActive(false);

                return;
            }
        }
    }

    private class PoolItem
    {
        public bool isActive;
        public GameObject gameObject;
    }
}
