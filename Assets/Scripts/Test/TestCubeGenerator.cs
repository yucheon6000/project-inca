using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject testCubePrefab;

    private void Awake()
    {
        for (int i = 0; i < 300; i++)
        {
            Instantiate(
                testCubePrefab,
                new Vector3(Random.Range(-300, 300), 1f, Random.Range(-300, 300)),
                Quaternion.identity
            );
        }
    }
}
