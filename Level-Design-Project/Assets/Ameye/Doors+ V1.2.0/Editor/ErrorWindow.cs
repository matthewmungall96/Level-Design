using UnityEditor;
using UnityEngine;
using Tagger;
using StylesHelper;

public class ErrorWindow : EditorWindow
{
    private string _infoString = "\n";

    [MenuItem("Tools/Doors+/Detect Errors")]
    private static void ShowWindow()
    {
        ErrorWindow myWindow = ScriptableObject.CreateInstance<ErrorWindow>();
        myWindow.ShowUtility();
        myWindow.titleContent = new GUIContent("Error Detection");
 
        //myWindow.minSize = new Vector2(200, 190);
        //myWindow.maxSize = myWindow.minSize;
    }

    private void OnGUI()
    {
        bool playerError = GameObject.FindGameObjectWithTag("Player") == null;

        if (!playerError)
        {
            if (GUILayout.Button(Styles.PlayerTagTrue, Styles.helpbox))
                _infoString = "An object with the tag 'Player' was found.";

            var detectionError = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>() == null;

            if (!detectionError)
            {
                if (GUILayout.Button(Styles.DetectionTrue, Styles.helpbox))
                    _infoString = "The detection script component was found attached to the player.";

                var reachError = GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>().Reach == 0;

                if (!reachError)
                {
                    if (GUILayout.Button(Styles.ReachTrue, Styles.helpbox))
                        _infoString = "The reach variable is not 0. \n";
                }

                else
                {
                    if (GUILayout.Button(Styles.ReachFalse, Styles.helpbox))
                    {
                        _infoString = "The reach variable is 0. \n";
                        EditorGUIUtility.PingObject(GameObject.FindGameObjectWithTag("Player").GetComponent<DoorDetection>());
                    }
                }
            }

            else
            {
                if (GUILayout.Button(Styles.DetectionFalse, Styles.helpbox))
                {
                    EditorGUIUtility.PingObject(GameObject.FindGameObjectWithTag("Player"));
                    _infoString = "The player doesn't have the detection script attached to it.";
                }

                if (GUILayout.Button(Styles.ReachUnknown, Styles.helpbox))
                {
                    _infoString = "There is no information on the reach variable.";
                    EditorGUIUtility.PingObject(GameObject.FindGameObjectWithTag("Player"));
                }
            }
        }

        else
        {
            if (GUILayout.Button(Styles.PlayerTagFalse, Styles.helpbox))
                _infoString = "There was no object found with the tag 'Player'.";

            if (GUILayout.Button(Styles.DetectionUnknown, Styles.helpbox))
                _infoString = "There is no information on the detection script.";

            if (GUILayout.Button(Styles.ReachUnknown, Styles.helpbox))
                _infoString = "There is no information on the reach variable.";
        }

        bool layerError = TagHelper.DoesLayerNotExist();

        if(!layerError)
        {
            if (GUILayout.Button(Styles.LayerTrue, Styles.helpbox))
                _infoString = ("The layer 'Trigger Zones' has been created.");
        }

        else if(layerError)
        {
            if (GUILayout.Button(Styles.LayerFalse, Styles.helpbox))
                _infoString = ("The layer 'Trigger Zones' has not yet been created.");
        }

            EditorGUILayout.LabelField(_infoString, Styles.helpbox);

        EditorGUILayout.Space();
        GUILayout.Label(Styles.VersionLabel, Styles.centeredVersionLabel);
        EditorGUILayout.Space();
    }
}