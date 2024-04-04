using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class TrackedPoseDriverRemover : MonoBehaviour
{
    [SerializeField]
    private bool wasRemoved = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (wasRemoved) return;
        if (TryGetComponent<TrackedPoseDriver>(out TrackedPoseDriver component))
        {
            component.enabled = false;
            wasRemoved = true;
        }
    }
}
