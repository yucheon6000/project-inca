using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingPrefab;

    private void Start()
    {
        IncaDetectionManager.AddOnTriggerEnterDetectedEnvironmentObject((DetectedEnvironmentObject obj, bool first) =>
        {
            print("EnterDetectedEnvironmentObject: " + obj.GUID.ToString() + ", " + first.ToString());

            if (first)
            {
                GameObject clone = Instantiate(buildingPrefab, obj.Position, obj.Rotation);
                clone.transform.localScale = obj.Scale;
            }
        });

        IncaDetectionManager.AddOnTriggerExitDetectedEnvironmentObject((DetectedEnvironmentObject obj) =>
        {
            print("ExitDetectedEnvironmentObject: " + obj.GUID.ToString());
        });
    }
}
