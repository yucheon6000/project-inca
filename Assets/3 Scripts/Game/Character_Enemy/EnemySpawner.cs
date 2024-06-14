using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.activeSelf || !other.gameObject.activeInHierarchy) return;

        // other.Tr
    }
}
