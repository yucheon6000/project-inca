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
        IncaDetectManager.AddOnTriggerEnterDetectedObject((DetectedObject obj, bool first) =>
        {
            print("EnterDetectedEnvironmentObject: " + obj.GUID.ToString() + ", " + first.ToString());

            if (first && obj.ObjectType == DetectedObjectType.Building)
            {
                GameObject clone = Instantiate(buildingPrefab, obj.Position, obj.Rotation);
                clone.transform.localScale = obj.Scale;
            }
        });

        IncaDetectManager.AddOnTriggerExitDetectedObject((DetectedObject obj) =>
        {
            print("ExitDetectedEnvironmentObject: " + obj.GUID.ToString());
        });
    }
}
