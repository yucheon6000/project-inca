using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedEnvironmentObject
{
    private EnvironmentObject environmentObject;
    private Transform originalTransform;

    public Guid GUID => environmentObject.GUID;

    public Vector3 Position => originalTransform.position;
    public Quaternion Rotation => originalTransform.rotation;
    public Vector3 Scale => originalTransform.lossyScale;

    private bool isVisible = false;

    public DetectedEnvironmentObject(EnvironmentObject environmentObject)
    {
        this.environmentObject = environmentObject;
        this.originalTransform = environmentObject.transform;
    }

    public bool IsVisible()
    {
        return isVisible;
    }

    public void IsVisible(bool value)
    {
        isVisible = value;
    }
}
