// DoorRotationEditor.cs
// Created by Alexander Ameye
// Version 1.2.0

using System;
using UnityEngine;
using UnityEditor;
using Malee.Editor;
using StylesHelper;

[CustomEditor(typeof(DoorRotation)), CanEditMultipleObjects]
public class DoorRotationEditor : Editor
{
    private static bool prefsLoaded = false;

    [Tooltip("Automatically add a sound component when adding a rotation component to a door.")]
    private static bool _autoAddSound = false;
    private static bool _colorMode = false;
    private static bool _showSceneOverlay = false;
    private static bool _padLock = false;
    private static bool _keyPad = false;
    private static bool _hierarchyIcons = true;

    private ReorderableList _rotationTimeline;

    private int _numberOfTriggers, _numberOfRotationBlocks, _toolBarIndex;

    private SerializedProperty _hingePositionProp, _doorScaleProp, _pivotPositionProp, _rotationWayProp, _angleConventionProp, _resetOnLeaveProp;

    public GameObject MoveTrigger, OpenTrigger, CloseTrigger, FrontTrigger, BackTrigger;
    public GameObject DoorParent, RotationParent;

    readonly Color _orange = new Color32(255, 139, 0, 255);

    public void OnEnable()
    {
        DoorRotation doorRotation = target as DoorRotation;

        _rotationTimeline = new ReorderableList(serializedObject.FindProperty("RotationTimeline"), true, true, true, ReorderableList.ElementDisplayType.Expandable, "RotationIndex", null);

        _hingePositionProp = serializedObject.FindProperty("HingePosition");
        _doorScaleProp = serializedObject.FindProperty("DoorScale");
        _pivotPositionProp = serializedObject.FindProperty("PivotPosition");
        _rotationWayProp = serializedObject.FindProperty("RotationWay");
        _resetOnLeaveProp = serializedObject.FindProperty("ResetOnLeave");

        if (doorRotation != null && doorRotation.transform.parent == null)
        {
            // Create a parent with the same name as the door itself and reset it
            DoorParent = new GameObject(doorRotation.gameObject.name);
            DoorParent.transform.localRotation = Quaternion.identity;
            DoorParent.transform.localPosition = Vector3.zero;
            DoorParent.transform.localScale = Vector3.one;
            doorRotation.transform.SetParent(DoorParent.transform);
        }

        if (doorRotation != null && (doorRotation.transform.parent != null && doorRotation.transform.parent.transform.name != doorRotation.gameObject.name))
        {
            int siblingIndex = doorRotation.transform.GetSiblingIndex();

            DoorParent = new GameObject(doorRotation.gameObject.name);
            DoorParent.transform.localRotation = Quaternion.identity;
            DoorParent.transform.localPosition = Vector3.zero;
            DoorParent.transform.localScale = Vector3.one;
            DoorParent.transform.SetParent(doorRotation.transform.parent);
            DoorParent.transform.SetSiblingIndex(siblingIndex);
            doorRotation.transform.SetParent(DoorParent.transform);
        }

        // Loop through all the children of the parent object and check for triggers
        _numberOfTriggers = 0;

        if (doorRotation.transform.parent != null)
            for (int x = 0; x < doorRotation.transform.parent.childCount; x++)
            {
                if (doorRotation.transform.parent.Find("Rotation " + x + " (Single)") != null ||
                    doorRotation.transform.parent.Find("Rotation " + x + " (Looped)") != null ||
                    doorRotation.transform.parent.Find("Rotation " + x + " (Swing)") != null) _numberOfTriggers += 1;
            }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DoorRotation doorRotation = target as DoorRotation;

        EditorGUILayout.Space();

        _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, new string[] {"Door", "Rotations"});

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };
        
        switch (_toolBarIndex)
        {
            //Door and hinge settings
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Door Settings</b>", style);
                EditorGUILayout.PropertyField(_doorScaleProp, new GUIContent("Scale"));
                EditorGUILayout.PropertyField(_pivotPositionProp, new GUIContent("Pivot Position"));
                EditorGUILayout.Space();
                if (doorRotation != null && (doorRotation.DoorScale == DoorRotation.ScaleOfDoor.Unity3DUnits && doorRotation.PivotPosition == DoorRotation.PositionOfPivot.Centered))
                {
                    EditorGUILayout.LabelField("<b>Hinge Settings</b>", style);
                    EditorGUILayout.PropertyField(_hingePositionProp, new GUIContent("Position"));
                    EditorGUILayout.Space();
                }

                if (doorRotation != null && (doorRotation.DoorScale == DoorRotation.ScaleOfDoor.Other && doorRotation.PivotPosition == DoorRotation.PositionOfPivot.Centered))
                    EditorGUILayout.HelpBox("If your door is not scaled in Unity3D units and the pivot position is not already positioned correctly, the hinge algorithm will not work as expected.", MessageType.Error);

                else if (Tools.pivotMode == PivotMode.Center)
                    EditorGUILayout.HelpBox("Make sure the tool handle is placed at the active object's pivot point.", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                /*EditorGUILayout.Space();
                EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                serializedObject.ApplyModifiedProperties();*/
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Rotation Settings</b>", style);
                EditorGUILayout.PropertyField(_rotationWayProp, new GUIContent("Rotation"));
                if (doorRotation != null)
                {
                    doorRotation.AngleConvention = EditorGUILayout.IntPopup("Convention", doorRotation.AngleConvention,
                        doorRotation.AngleConventionNames, doorRotation.AngleConventionValues);
                    EditorGUILayout.PropertyField(_resetOnLeaveProp, new GUIContent("Reset On Leave"));

                    _rotationTimeline.DoLayoutList();

                    if (doorRotation.transform.gameObject.GetComponent(typeof(DoorSound)) == null)
                    {
                        if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = Color.green;
                        if (GUILayout.Button("Add Audio Component"))
                            doorRotation.transform.gameObject.AddComponent(typeof(DoorSound));
                        GUI.color = Color.white;
                    }

                    else
                    {
                        if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = _orange;
                        if (GUILayout.Button("Remove Audio Component"))
                            DestroyImmediate(doorRotation.transform.gameObject.GetComponent(typeof(DoorSound)));
                        GUI.color = Color.white;
                    }

                    if (doorRotation.RotationTimeline != null)
                    {
                        for (int x = 0; x < doorRotation.RotationTimeline.Count; x++)
                        {
                            if ((doorRotation.RotationTimeline[x].FinalAngle -
                                 doorRotation.RotationTimeline[x].InitialAngle) >= 360)
                                EditorGUILayout.HelpBox(
                                    "The difference between FinalAngle and InitialAngle should not exceed 360°. (See rotation " +
                                    (x + 1) + ")", MessageType.Warning);
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                   // EditorGUILayout.Space();
                    //EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
                    
                }
               // serializedObject.ApplyModifiedProperties();
                break;
            default: break;
            
         
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);
        

        _numberOfRotationBlocks = 0;
        if (doorRotation != null && doorRotation.RotationTimeline != null) _numberOfRotationBlocks = doorRotation.RotationTimeline.Count;

        if (Application.isPlaying) return;

        // Adding rotation blocks
        if (_numberOfTriggers < _numberOfRotationBlocks)
        {
            for (int index = 0; index < _numberOfRotationBlocks; index++)
            {
                if (doorRotation.RotationTimeline != null && (doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)") == null && doorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SingleRotation))
                    CreateMoveTrigger(true, index);

                if (doorRotation.RotationTimeline != null && (doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)") == null && doorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.LoopedRotation))
                    CreateOpenCloseTriggers(true, index);

                if (doorRotation.RotationTimeline != null && (doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)") == null && doorRotation.RotationTimeline[index].RotationType == DoorRotation.RotationTimelineData.TypeOfRotation.SwingRotation))
                    CreateFrontBackTriggers(true, index);
            }
        }

        // Changing rotation blocks
        for (int index = 0; index < _numberOfRotationBlocks; index++)
        {
            //if(DoorRotation.RotationTimeline[index].RotationIndex == "")
            if (doorRotation == null) continue;
            if (doorRotation.RotationTimeline == null) continue;
            
            doorRotation.RotationTimeline[index].RotationIndex = "Rotation Block " + (index + 1).ToString();

            if (doorRotation.RotationTimeline[index].RotationType ==
                DoorRotation.RotationTimelineData.TypeOfRotation.LoopedRotation)
            {
                if (doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)") &&
                    !Application.isPlaying)
                {
                    DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)")
                        .gameObject);
                    CreateOpenCloseTriggers(false, index);
                }

                if (doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Swing)") &&
                    !Application.isPlaying)
                {
                    DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)")
                        .gameObject);
                    CreateOpenCloseTriggers(false, index);
                }
            }

            if (doorRotation.RotationTimeline[index].RotationType ==
                DoorRotation.RotationTimelineData.TypeOfRotation.SingleRotation)
            {
                if (doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)") &&
                    !Application.isPlaying)
                {
                    DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)")
                        .gameObject);
                    CreateMoveTrigger(false, index);
                }

                if (doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Swing)") &&
                    !Application.isPlaying)
                {
                    DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Swing)")
                        .gameObject);
                    CreateMoveTrigger(false, index);
                }
            }

            if (doorRotation.RotationTimeline[index].RotationType != DoorRotation.RotationTimelineData.TypeOfRotation.SwingRotation) continue;
            
            if (doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)") &&
                !Application.isPlaying)
            {
                DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Single)")
                    .gameObject);
                CreateFrontBackTriggers(false, index);
            }

            if (!doorRotation.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)") || Application.isPlaying) continue;
            DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + (index + 1) + " (Looped)").gameObject);
            CreateFrontBackTriggers(false, index);
        }

        if (_numberOfTriggers <= _numberOfRotationBlocks) return;

        // Removing Rotation blocks
        while (_numberOfTriggers > _numberOfRotationBlocks)
        {
            if (doorRotation != null && doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Single)") != null)
            {
                DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Single)").gameObject);
                _numberOfTriggers--;
            }

            else if (doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Swing)") != null)
            {
                DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Swing)").gameObject);
                _numberOfTriggers--;
            }

            else if (doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Looped)") != null)
            {
                DestroyImmediate(doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Looped)").gameObject);
                _numberOfTriggers--;
            }

            else if (doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Looped)") == null || doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Single)") == null || doorRotation.transform.parent.Find("Rotation " + _numberOfTriggers + " (Swing)") == null) continue;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateMoveTrigger(bool addToNumberOfTriggers, int index)
    {

        DoorRotation doorRotation = target as DoorRotation;
        if (doorRotation == null) throw new ArgumentNullException("DoorRotation");

        GameObject MoveTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MoveTrigger.name = "Move Trigger";

        RotationParent = new GameObject("Rotation " + (index + 1) + " (Single)");
        ResetTransform(MoveTrigger, doorRotation);
        SetParentChild(RotationParent, MoveTrigger);
        AddTriggerScript(MoveTrigger, index);
        SetColor(MoveTrigger);

        if (addToNumberOfTriggers) _numberOfTriggers += 1;
    }

    private void CreateOpenCloseTriggers(bool AddToNumberOfTriggers, int index)
    {
        DoorRotation doorRotation = target as DoorRotation;

        GameObject OpenTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        OpenTrigger.name = "Open Trigger";

        GameObject CloseTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        CloseTrigger.name = "Close Trigger";

        RotationParent = new GameObject("Rotation " + (index + 1) + " (Looped)");

        ResetTransform(OpenTrigger, doorRotation);
        SetParentChild(RotationParent, OpenTrigger);
        AddTriggerScript(OpenTrigger, index);

        ResetTransform(CloseTrigger, doorRotation);
        SetParentChild(RotationParent, CloseTrigger);
        AddTriggerScript(CloseTrigger, index);
        SetColor(OpenTrigger);
        SetColor(CloseTrigger);

        if (AddToNumberOfTriggers) _numberOfTriggers += 1;
    }

    private void CreateFrontBackTriggers(bool AddToNumberOfTriggers, int index)
    {
        DoorRotation doorRotation = target as DoorRotation;

        GameObject FrontTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        FrontTrigger.name = "Front Trigger";


        GameObject BackTrigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        BackTrigger.name = "Back Trigger";


        RotationParent = new GameObject("Rotation " + (index + 1) + " (Swing)");

        ResetTransform(FrontTrigger, doorRotation);
        SetParentChild(RotationParent, FrontTrigger);
        AddTriggerScript(FrontTrigger, index);


        ResetTransform(BackTrigger, doorRotation);
        SetParentChild(RotationParent, BackTrigger);
        AddTriggerScript(BackTrigger, index);

        SetColor(FrontTrigger);
        SetColor(BackTrigger);

        if (AddToNumberOfTriggers) _numberOfTriggers += 1;
    }

    private static void SetParentChild(GameObject Parent, GameObject Trigger)
    {
        Parent.transform.parent = Selection.activeGameObject.transform.parent.transform;
        Trigger.transform.parent = Parent.transform;
    }

    private static void ResetTransform(GameObject obj, DoorRotation DoorRotation)
    {
        if (obj.name == "Move Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = Vector3.one;
            //obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }

        if (obj.name == "Open Trigger" || obj.name == "Front Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position - new Vector3(1, 0, 0) + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = Vector3.one;
            //obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }

        if (obj.name == "Close Trigger" || obj.name == "Back Trigger")
        {
            obj.transform.position = DoorRotation.gameObject.transform.position + new Vector3(1, 0, 0) + new Vector3(0, 0.125f, 0);
            obj.transform.localScale = Vector3.one;
            //obj.transform.localScale = new Vector3(2, DoorRotation.gameObject.transform.localScale.y + 0.25f, 2);
            obj.transform.rotation = DoorRotation.gameObject.transform.rotation;
        }
    }

    private static void AddTriggerScript(GameObject Trigger, int index)
    {
        Trigger.AddComponent<DoorTrigger>();
        Trigger.GetComponent<DoorTrigger>().Id = index;
    }

    private static void SetColor(GameObject Trigger)
    {
        string material = "";

        switch (Trigger.name)
        {
            case "Move Trigger":
                material = "Assets/Ameye/Doors+ V1.2.0/Resources/Move_Trigger_Zone.mat";
                break;
            case "Open Trigger":
                material = "Assets/Ameye/Doors+ V1.2.0/Resources/Open_Trigger_Zone.mat";
                break;
            case "Close Trigger":
                material = "Assets/Ameye/Doors+ V1.2.0/Resources/Close_Trigger_Zone.mat";
                break;
            case "Front Trigger":
                material = "Assets/Ameye/Doors+ V1.2.0/Resources/Front_Trigger_Zone.mat";
                break;
            case "Back Trigger":
                material = "Assets/Ameye/Doors+ V1.2.0/Resources/Back_Trigger_Zone.mat";
                break;
        }

        Material mat = (Material)AssetDatabase.LoadAssetAtPath(material, typeof(Material));
        Trigger.GetComponent<Renderer>().material = mat;
    }

    [PreferenceItem("Doors+ V1.2.0")]
    public static void PreferencesGui()
    {
        GUIStyle centeredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
    
        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        if (!prefsLoaded)
        {
            _autoAddSound = EditorPrefs.GetBool("AutoAddSoundKey", false);
            _colorMode = EditorPrefs.GetBool("ColorModeKey", false);
            _showSceneOverlay = EditorPrefs.GetBool("ShowSceneOverlayKey", false);
            _padLock = EditorPrefs.GetBool("PadLockKey", false);
            _keyPad = EditorPrefs.GetBool("KeyPadKey", false);
            _hierarchyIcons = EditorPrefs.GetBool("HierarchyIconsKey", false);
            prefsLoaded = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>Door System Settings</b>", style);
        _colorMode = EditorGUILayout.Toggle(new GUIContent("Colorful Mode", "Use a colorful UI mode."), _colorMode);
        _showSceneOverlay = EditorGUILayout.Toggle(new GUIContent("Scene Overlay", "Show the scene overlay toolbar."), _showSceneOverlay);
        _hierarchyIcons =
            EditorGUILayout.Toggle(
                new GUIContent("Hierarchy Icons", "Show Hierarchy Icons for a structured overview of your doors."),
                _hierarchyIcons);
        
        EditorGUILayout.Space();
        
        /*EditorGUILayout.LabelField("<b>3rd Party Assets</b>", style);
        EditorGUILayout.BeginHorizontal();
        PadLock = EditorGUILayout.Toggle(new GUIContent("Padlock Puzzle System", "Enable the integration of the padlock puzzle system."), PadLock);
        GUI.color = Color.green;
        if (GUILayout.Button("Get", EditorStyles.miniButton)) Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/80482");
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        KeyPad = EditorGUILayout.Toggle(new GUIContent("Digital Input Keypad System", "Enable the integration of the digital input keypad system."), KeyPad);
        GUI.color = Color.green;
        if (GUILayout.Button("Get", EditorStyles.miniButton)) Application.OpenURL("   https://www.assetstore.unity3d.com/en/#!/content/71659");
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();*/
        //AutoAddSound = EditorGUILayout.Toggle(new GUIContent("Auto add sound component", "Automatically add a sound component when adding a rotation component to a door."), AutoAddSound);

        //EditorGUILayout.LabelField("Coming soon!", EditorStyles.centeredGreyMiniLabel);

        if (GUI.changed)
        {
            EditorPrefs.SetBool("AutoAddSoundKey", _autoAddSound);
            EditorPrefs.SetBool("ColorModeKey", _colorMode);
            EditorPrefs.SetBool("ShowSceneOverlayKey", _showSceneOverlay);
            EditorPrefs.SetBool("PadLockKey", _padLock);
            EditorPrefs.SetBool("KeyPadKey", _keyPad);
            EditorPrefs.SetBool("HierarchyIconsKey", _hierarchyIcons);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(new GUIContent("Version 1.2.0"), centeredVersionLabel);
    }
}