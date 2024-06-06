using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModeSwitch)), CanEditMultipleObjects]
public class ModeSwitchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ModeSwitch modeSwitch = (ModeSwitch)target;

        if (GUILayout.Button("VR Mode", new GUILayoutOption[] { GUILayout.Height(50) }))
            modeSwitch.ChangeToVrMode();
        if (GUILayout.Button("Monitor Mode", new GUILayoutOption[] { GUILayout.Height(50) }))
            modeSwitch.ChangeToMonitorMode();
    }
}