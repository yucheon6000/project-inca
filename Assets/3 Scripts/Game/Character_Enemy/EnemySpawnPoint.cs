using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    public void Spawn()
    {
        GameObject enemy = MemoryPool.Instance(MemoryPoolType.Enemy).ActivatePoolItem(enemyPrefab);
        enemy.transform.position = transform.position;
    }
}
