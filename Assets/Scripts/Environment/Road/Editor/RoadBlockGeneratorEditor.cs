using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadBlockGenerator))]
public class RoadBlockGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoadBlockGenerator generator = (RoadBlockGenerator)target;

        if (GUILayout.Button("Generate Road Block", new GUILayoutOption[] { GUILayout.Height(50) }))
        {
            generator.GenerateRoadBlock();
        }
    }
}
