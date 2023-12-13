using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject testCubePrefab;

    private void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            Instantiate(
                testCubePrefab,
                new Vector3(Random.Range(-500, 500), 1f, Random.Range(-500, 500)),
                Quaternion.identity
            );
        }
    }
}
