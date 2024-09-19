using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoadModelGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> roadModelPrefabs;
    [SerializeField]
    private Vector3 gap;
    [SerializeField]
    private int count;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Vector3 scale;
    [SerializeField]
    private Vector3 rotation;

#if UNITY_EDITOR
    [ContextMenu("Generate Road Model")]
    private void GenerateRoadModel()
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject prefab = roadModelPrefabs[Random.Range(0, roadModelPrefabs.Count)];
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            clone.transform.localScale = scale;
            clone.transform.localEulerAngles = rotation;
            clone.transform.position = startPosition + gap * i;
            clone.transform.SetParent(transform);
        }
    }
#endif
}
