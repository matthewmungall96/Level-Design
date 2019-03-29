// DoorTriggerEditor.cs
// Created by Alexander Ameye
// Version 1.2.0

using UnityEngine;
using UnityEditor;
using StylesHelper;

[CustomEditor(typeof(DoorTrigger)), CanEditMultipleObjects]
public class DoorTriggerEditor : Editor//, IHierarchyIcon
{
    private int _toolBarIndex;

    private DoorDetection _doordetection;
    private DoorTrigger _doortrigger;

    private SerializedProperty _colliderTypeProp;

    private SerializedProperty _hasTagProp, _hasNameProp, _isLookingAtProp, _hasPressedProp, _hasScriptProp, _isGameObjectProp;
    private SerializedProperty _tagProp, _nameProp, _charProp;
    private SerializedProperty _lookObjectProp, _objectProp;
    private SerializedProperty _scriptProp;

    //private SerializedProperty lockProp, keyPadProp;

    private void OnEnable()
    {
        _colliderTypeProp = serializedObject.FindProperty("ColliderType");

        _hasTagProp = serializedObject.FindProperty("HasTag");
        _hasNameProp = serializedObject.FindProperty("HasName");
        _isLookingAtProp = serializedObject.FindProperty("IsLookingAt");
        _hasPressedProp = serializedObject.FindProperty("HasPressed");
        _hasScriptProp = serializedObject.FindProperty("HasScript");
        _isGameObjectProp = serializedObject.FindProperty("IsGameObject");

        _tagProp = serializedObject.FindProperty("PlayerTag");
        _nameProp = serializedObject.FindProperty("PlayerName");
        _charProp = serializedObject.FindProperty("Character");
        _lookObjectProp = serializedObject.FindProperty("LookObject");
        _scriptProp = serializedObject.FindProperty("Script");
        _objectProp = serializedObject.FindProperty("IsObject");

        //lockProp = serializedObject.FindProperty("RequiresPadLockUnlock");
        //keyPadProp = serializedObject.FindProperty("RequiresKeyPadUnlock");
    }

    public override void OnInspectorGUI()
    {   /*CaptureGroup.ShowDebug = true;
        CaptureGroup.Begin("Assets/test.png");
        {*/
            serializedObject.Update();
            _doortrigger = target as DoorTrigger;
            _doordetection = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>();

            GUIStyle style = new GUIStyle()
            {
                richText = true
            };

            string[] menuOptions = new string[2];
            menuOptions[0] = "Trigger";
            menuOptions[1] = "Gizmo";

            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = 70;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
            EditorGUILayout.PropertyField(_colliderTypeProp, new GUIContent("Shape"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<b>Player Requirements</b>", style);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_hasTagProp,
                new GUIContent("Tag", "Does the object that entered the trigger zone have a certain tag?"));
            if (_doortrigger != null && _doortrigger.HasTag)
                EditorGUILayout.PropertyField(_tagProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_hasNameProp,
                new GUIContent("Name", "Does the object that entered the trigger zone has a certain name?"));
            if (_doortrigger != null && _doortrigger.HasName)
                EditorGUILayout.PropertyField(_nameProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_isLookingAtProp,
                new GUIContent("Looking At", "Is the player looking at a certain object?"));
            if (_doortrigger != null && _doortrigger.IsLookingAt)
                EditorGUILayout.PropertyField(_lookObjectProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_hasPressedProp,
                new GUIContent("Pressed", "Has the user pressed a certain key?"));
            if (_doortrigger != null && _doortrigger.HasPressed)
                EditorGUILayout.PropertyField(_charProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_hasScriptProp,
                new GUIContent("Script", "Does the player have a certain script component?"));
            if (_doortrigger != null && _doortrigger.HasScript)
                EditorGUILayout.PropertyField(_scriptProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_isGameObjectProp,
                new GUIContent("Is Object", "Is there a certain object in the trigger zone?"));
            if (_doortrigger != null && _doortrigger.IsGameObject)
                EditorGUILayout.PropertyField(_objectProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            /*if (EditorPrefs.GetBool("PadLockKey"))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lockProp, new GUIContent("Lock", "Does the player have to unlock the door?"));
                EditorGUILayout.EndHorizontal();
            }
    
            else doortrigger.RequiresPadLockUnlock = false;
    
            if (EditorPrefs.GetBool("KeyPadKey"))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(keyPadProp, new GUIContent("Keypad", "Does the player have to enter a keycode?"));
                EditorGUILayout.EndHorizontal();
            }
    
            else doortrigger.RequiresKeyPadUnlock = false;*/

            if (AnyEmptyFields())
                EditorGUILayout.HelpBox("One or more fields have been left empty.", MessageType.Warning);

            if (_doortrigger.IsLookingAt && _doordetection.Reach == 0 && _doortrigger.IsLookingAt &&
                _doortrigger.LookObject != null)
                EditorGUILayout.HelpBox("The reach of your player is zero.", MessageType.Warning);

            EditorGUILayout.Space();

            if (EditorPrefs.GetBool("ColorModeKey")) GUI.color = Color.green;

            if (GUILayout.Button("Add " + _doortrigger.transform.gameObject.name))
            {
                GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trigger.name = _doortrigger.transform.gameObject.name;

                GameObject rotationParent = _doortrigger.transform.parent.gameObject;

                trigger.transform.position = _doortrigger.transform.position;
                trigger.transform.localScale = _doortrigger.transform.localScale;
                trigger.transform.rotation = _doortrigger.transform.rotation;

                SetParentChild(rotationParent, trigger);


                trigger.AddComponent<DoorTrigger>();
                trigger.GetComponent<DoorTrigger>().Id = _doortrigger.Id;
                trigger.GetComponent<DoorTrigger>().ColliderType = _doortrigger.ColliderType;

                string material = "";

                switch (trigger.name)
                {
                    case "Move Trigger":
                        material = "Assets/Ameye/Alex's Door System/Resources/Move_Trigger_Zone.mat";
                        break;
                    case "Open Trigger":
                        material = "Assets/Ameye/Alex's Door System/Resources/Open_Trigger_Zone.mat";
                        break;
                    case "Close Trigger":
                        material = "Assets/Ameye/Alex's Door System/Resources/Close_Trigger_Zone.mat";
                        break;
                    case "Front Trigger":
                        material = "Assets/Ameye/Alex's Door System/Resources/Front_Trigger_Zone.mat";
                        break;
                    case "Back Trigger":
                        material = "Assets/Ameye/Alex's Door System/Resources/Back_Trigger_Zone.mat";
                        break;
                }

                Material mat = (Material) AssetDatabase.LoadAssetAtPath(material, typeof(Material));
                trigger.GetComponent<Renderer>().material = mat;
            }

            GUI.color = Color.white;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Styles.VersionLabel, Styles.centeredVersionLabel);

            if (Event.current.type == EventType.Repaint)
            {
                if (_doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Cubic)
                {
                    if (_doortrigger.gameObject.GetComponent<BoxCollider>() == null)
                        _doortrigger.gameObject.AddComponent<BoxCollider>();
                    _doortrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                }

                else if (_doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Spherical)
                {
                    if (_doortrigger.gameObject.GetComponent<SphereCollider>() == null)
                        _doortrigger.gameObject.AddComponent<SphereCollider>();
                    _doortrigger.gameObject.GetComponent<SphereCollider>().isTrigger = true;
                }
            }

            if (_doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Cubic)
            {
                _doortrigger.GetComponent<BoxCollider>().enabled = true;
                _doortrigger.GetComponent<SphereCollider>().enabled = false;

                _doortrigger.GetComponent<MeshFilter>().mesh =
                    CreatePrimitiveMesh(PrimitiveType.Cube); //PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Cube);
            }

            else if (_doortrigger.ColliderType == DoorTrigger.TypeOfCollider.Spherical)
            {
                _doortrigger.GetComponent<BoxCollider>().enabled = false;
                _doortrigger.GetComponent<SphereCollider>().enabled = true;
                _doortrigger.GetComponent<MeshFilter>().mesh =
                    CreatePrimitiveMesh(PrimitiveType.Sphere); //PrimitiveHelper.GetPrimitiveMesh(PrimitiveType.Sphere);
            }

            serializedObject.ApplyModifiedProperties();
        /*}
        CaptureGroup.End();
            if(GUILayout.Button("Preform Capture"))
            {
                // Start the capture.
                CaptureGroup.PreformCapture(this);
            }*/
    }

    private static void SetParentChild(GameObject parent, GameObject trigger)
    {
        parent.transform.parent = Selection.activeGameObject.transform.parent.transform;
        trigger.transform.parent = parent.transform;
    }

    private bool AnyEmptyFields()
    {
        DoorTrigger doortrigger = target as DoorTrigger;

        if (doortrigger != null && (doortrigger.HasName && doortrigger.PlayerName == ""))
            return true;
        if (doortrigger != null && (doortrigger.IsLookingAt && doortrigger.LookObject == null))
            return true;
        if (doortrigger != null && (doortrigger.HasPressed && doortrigger.Character == ""))
            return true;
        if (doortrigger != null && (doortrigger.HasScript && doortrigger.Script.script.Name == ""))
            return true;
        return doortrigger != null && (doortrigger.IsGameObject && doortrigger.IsObject == null);
    }
    
    private static Mesh CreatePrimitiveMesh(PrimitiveType type)
    {
        GameObject gameObject = GameObject.CreatePrimitive(type);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(gameObject);
        return mesh;
    }
}