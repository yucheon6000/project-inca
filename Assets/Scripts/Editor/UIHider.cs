using System.Collections.Generic;
using UnityEngine;

public class UIHider : MonoBehaviour
{
    [SerializeField]
    private bool show;
    [SerializeField]
    private List<GameObject> targetGameObjects;

    private int prevCount = -1;

    private void Update()
    {
        if (targetGameObjects.Count != prevCount)
        {
            prevCount = targetGameObjects.Count;
            UpdateActivation();
        }
    }

    private void UpdateActivation()
    {
        foreach (var target in targetGameObjects)
            target.gameObject.SetActive(show);
    }
}
