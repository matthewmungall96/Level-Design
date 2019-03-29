using UnityEngine;
using UnityEditor;

public class SceneOverlay : EditorWindow
{
    private const float ButtonWidth = 50f;
    private const float ButtonHeight = 18f;
    private const float HeightOffset = 5f;
    private const float WidthOffset = 5f;

    private static bool _triggerZonesVisible = true;

    private static string _text;

    private static Texture2D _hingesIcon;
    private static GUIContent _showHinges;
    private static GUIContent _showTriggerZones;
    private static GUIContent _showRotationAngles;

    [InitializeOnLoadMethod]
    public static void Enable()
    {
        SceneView.onSceneGUIDelegate += OnScene;
       // Debug.Log("Scene GUI : Enabled");

        //Texture2D HingesIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Ameye/Doors+ V1.2.0/Icons/hinge.png", typeof(Texture2D));
        Texture2D TriggerIcon = (Texture2D)Resources.Load("Icons/triggerzonesblue");  
        Texture2D AnglesIcon =  (Texture2D)Resources.Load("Icons/errorchecking");  

        //showHinges = new GUIContent(HingesIcon, "Show/hide the door hinges.");
        _showTriggerZones = new GUIContent(TriggerIcon, "Show/hide the trigger zones.");
        _showRotationAngles = new GUIContent(AnglesIcon, "Detect Errors");
    }

    private static void OnScene(SceneView sceneview)
    {
        GUI.skin.button.name = "New GUISkin";
        Handles.BeginGUI();

        if (EditorPrefs.GetBool("ShowSceneOverlayKey"))
        {
            if (GUI.Button(new Rect(new Vector2(WidthOffset, HeightOffset), new Vector2(ButtonWidth, ButtonHeight)), _showTriggerZones, EditorStyles.miniButtonLeft))
            {
                var triggers = FindObjectsOfType<DoorTrigger>();

                foreach (DoorTrigger trigger in triggers)
                {
                    trigger.transform.GetComponent<MeshRenderer>().enabled = !_triggerZonesVisible;
                }
                _triggerZonesVisible = !_triggerZonesVisible;
            }

            if (GUI.Button(new Rect(new Vector2(WidthOffset + ButtonWidth, HeightOffset), new Vector2(ButtonWidth, ButtonHeight)), _showRotationAngles, EditorStyles.miniButtonRight))
            {
                GetWindow(typeof(ErrorWindow));
                GetWindow(typeof(ErrorWindow)).ShowUtility();
                GetWindow(typeof(ErrorWindow)).titleContent = new GUIContent("Error Detection");
                GetWindow(typeof(ErrorWindow)).minSize = new Vector2(200, 190);
                GetWindow(typeof(ErrorWindow)).maxSize = GetWindow(typeof(ErrorWindow)).minSize;
            }
        }
        Handles.EndGUI();
    }
}