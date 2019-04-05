// DoorDetectionEditor.cs
// Created by Alexander Ameye
// Version 1.2.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorDetection))]
public class DoorDetectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DoorDetection doorDetection = target as DoorDetection;

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>UI Settings</b>", style);
        if (doorDetection != null)
        {
            doorDetection.LookingAtPrefab = (GameObject) EditorGUILayout.ObjectField("Looking at", doorDetection.LookingAtPrefab, typeof(GameObject), true);
            doorDetection.InTriggerZoneLookingAtPrefab = (GameObject) EditorGUILayout.ObjectField("In zone", doorDetection.InTriggerZoneLookingAtPrefab, typeof(GameObject), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Raycast Settings</b>", style);
            doorDetection.Reach = EditorGUILayout.FloatField("Reach", doorDetection.Reach);
            doorDetection.DebugRay = EditorGUILayout.Toggle("Debug Ray", doorDetection.DebugRay);
            if (doorDetection.DebugRay)
            {
                doorDetection.DebugRayColor = EditorGUILayout.ColorField("Color", doorDetection.DebugRayColor);
                doorDetection.DebugRayColorAlpha =
                    EditorGUILayout.Slider("Opacity", doorDetection.DebugRayColorAlpha, 0, 1);
                doorDetection.DebugRayColor.a = doorDetection.DebugRayColorAlpha;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
    }
}