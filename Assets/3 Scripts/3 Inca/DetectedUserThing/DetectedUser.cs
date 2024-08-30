using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class DetectedUser : MonoBehaviour
{
    [SerializeField]
    private UserObjects<DetectedObject> detectedUserObjects;

    private void Start()
    {
        var userEnvObjs = IncaMainManager.Instance.EnvironmentalUserObjects;

        detectedUserObjects.userCar.Initialize(userEnvObjs.userCar);
        detectedUserObjects.userCar.IsVisible(true);
        detectedUserObjects.userHead.Initialize(userEnvObjs.userHead);
        detectedUserObjects.userHead.IsVisible(true);
        detectedUserObjects.userHandRight.Initialize(userEnvObjs.userHandRight);
        detectedUserObjects.userHandRight.IsVisible(true);
        detectedUserObjects.userHandLeft.Initialize(userEnvObjs.userHandLeft);
        detectedUserObjects.userHandLeft.IsVisible(true);

        IncaDetectManager.Instance.SetDetectedUserObjects(detectedUserObjects);
    }
}
