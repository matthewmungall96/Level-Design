// DoorSoundEditor.cs
// Created by Alexander Ameye
// Version 1.2.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorSound)), CanEditMultipleObjects]
public class DoorSoundEditor : Editor
{
    private int _toolBarIndex;
    private DoorSound _doorsound;
    readonly Color blue = new Color(0,0.715f,0.93f);

    private SerializedProperty _openingClipProp, _openingVolumeProp, _openingPitchProp, _openingOffsetProp;
    private SerializedProperty _openedClipProp, _openedVolumeProp, _openedPitchProp, _openedOffsetProp;
    private SerializedProperty _closingClipProp, _closingVolumeProp, _closingPitchProp, _closingOffsetProp;
    private SerializedProperty _closedClipProp, _closedVolumeProp, _closedPitchProp, _closedOffsetProp;
    private SerializedProperty _lockedCLipProp, _lockedVolumeProp, _lockedPitchProp, _lockedOffsetProp;
   // private SerializedProperty mixerProp;

    public void OnEnable()
    {
        _openingClipProp = serializedObject.FindProperty("OpeningClip");
        _openingVolumeProp = serializedObject.FindProperty("OpeningVolume");
        _openingPitchProp = serializedObject.FindProperty("OpeningPitch");
        _openingOffsetProp = serializedObject.FindProperty("OpeningOffset");
       // mixerProp = serializedObject.FindProperty("mixer");
        _openedClipProp = serializedObject.FindProperty("OpenedClip");
        _openedVolumeProp = serializedObject.FindProperty("OpenedVolume");
        _openedPitchProp = serializedObject.FindProperty("OpenedPitch");
        _openedOffsetProp = serializedObject.FindProperty("OpenedOffset");

        _closingClipProp = serializedObject.FindProperty("ClosingClip");
        _closingVolumeProp = serializedObject.FindProperty("ClosingVolume");
        _closingPitchProp = serializedObject.FindProperty("ClosingPitch");
        _closingOffsetProp = serializedObject.FindProperty("ClosingOffset");

        _closedClipProp = serializedObject.FindProperty("ClosedClip");
        _closedVolumeProp = serializedObject.FindProperty("ClosedVolume");
        _closedPitchProp = serializedObject.FindProperty("ClosedPitch");
        _closedOffsetProp = serializedObject.FindProperty("ClosedOffset");

        _lockedCLipProp = serializedObject.FindProperty("LockedClip");
        _lockedVolumeProp = serializedObject.FindProperty("LockedVolume");
        _lockedPitchProp = serializedObject.FindProperty("LockedPitch");
        _lockedOffsetProp = serializedObject.FindProperty("LockedOffset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _doorsound = target as DoorSound;

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        GUIContent[] menuOptions = new GUIContent[3];
        menuOptions[0] = new GUIContent("Open");
        menuOptions[1] = new GUIContent("Close");
        menuOptions[2] = new GUIContent("Locked");

        EditorGUILayout.Space();
        _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, menuOptions);

        switch (_toolBarIndex)
        {
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Opening</b>", style);
                EditorGUILayout.PropertyField(_openingClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(_openingVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(_openingPitchProp, new GUIContent("Playback Speed"));
                EditorGUILayout.PropertyField(_openingOffsetProp, new GUIContent("Delay"));
               // EditorGUILayout.PropertyField(mixerProp, new GUIContent("Mixer Group"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Opened</b>", style);
                EditorGUILayout.PropertyField(_openedClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(_openedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(_openedPitchProp, new GUIContent("Playback Speed"));
                EditorGUILayout.PropertyField(_openedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = blue;
                if (GUILayout.Button("Preview Audio"))
                    if (_doorsound != null) _doorsound.Preview("Open");

                GUI.color = Color.white;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Closing</b>", style);
                EditorGUILayout.PropertyField(_closingClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(_closingVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(_closingPitchProp, new GUIContent("Playback Speed"));
                EditorGUILayout.PropertyField(_closingOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Closed</b>", style);
                EditorGUILayout.PropertyField(_closedClipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(_closedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(_closedPitchProp, new GUIContent("Playback Speed"));
                EditorGUILayout.PropertyField(_closedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = blue;
                if (GUILayout.Button("Preview Audio")) _doorsound.Preview("Close");
     
                GUI.color = Color.white;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;

            case 2:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Locked</b>", style);
                EditorGUILayout.PropertyField(_lockedCLipProp, new GUIContent("Clip"));
                EditorGUILayout.PropertyField(_lockedVolumeProp, new GUIContent("Volume"));
                EditorGUILayout.PropertyField(_lockedPitchProp, new GUIContent("Playback Speed"));
                EditorGUILayout.PropertyField(_lockedOffsetProp, new GUIContent("Delay"));

                EditorGUILayout.Space();
                if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = blue;
                if (GUILayout.Button("Preview Audio")) _doorsound.Preview("Lock");

                GUI.color = Color.white;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}