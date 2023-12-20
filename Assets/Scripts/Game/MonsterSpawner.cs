using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class MonsterSpawner : MonoBehaviour
{
    private void Start()
    {
        IncaDetectionManager.AddOnTriggerEnterDetectedEnvironmentObject((DetectedEnvironmentObject obj, bool first) =>
        {
            print("EnterDetectedEnvironmentObject: " + obj.GUID.ToString() + ", " + first.ToString());
        });

        IncaDetectionManager.AddOnTriggerExitDetectedEnvironmentObject((DetectedEnvironmentObject obj) =>
        {
            print("ExitDetectedEnvironmentObject: " + obj.GUID.ToString());
        });
    }
}
