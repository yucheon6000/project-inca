using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private List<Material> materials;

    private void Awake()
    {
        meshRenderer.material = materials[Random.Range(0, materials.Count)];
    }
}
