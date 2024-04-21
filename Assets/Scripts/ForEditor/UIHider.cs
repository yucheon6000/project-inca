using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIHider : MonoBehaviour
{
    private bool preShow = false;
    [SerializeField]
    private bool show;
    [SerializeField]
    private List<GameObject> targetGameObjects = new List<GameObject>();

    private int prevCount = -1;

    private void Awake()
    {
        UpdateActivation(true);
    }

    private void Update()
    {
        if (targetGameObjects.Count != prevCount)
        {
            prevCount = targetGameObjects.Count;
            UpdateActivation();
        }

        if (preShow != show)
        {
            preShow = show;
            UpdateActivation();
        }
    }

    private void UpdateActivation(bool value)
    {
        show = value;
        UpdateActivation();
    }

    private void UpdateActivation()
    {
        foreach (var target in targetGameObjects)
            target.gameObject.SetActive(show);
    }
}
