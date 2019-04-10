using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PipeSystem))]
public class PipeSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PipeSystem pipeSystem = (PipeSystem)target;
        if (GUILayout.Button("Set Universe A"))
        {
            SetUniverse(pipeSystem.transform, "UniverseA", "PipeConnector-a");
        }

        if (GUILayout.Button("Set Universe B"))
        {
            SetUniverse(pipeSystem.transform, "UniverseB", "PipeConnector-b");
        }   
    }

    private void SetUniverse(Transform rootTransform, string universeLayer, string universeConnectorLayer)
    {
        rootTransform.gameObject.layer = LayerMask.NameToLayer(universeLayer);

        var connectors = rootTransform.gameObject.GetComponentsInChildren<PipeConnector>();

        // Set all objects layers to Universe A layer
        RecursiveSetChildLayer(rootTransform.transform, universeLayer);

        // Set only connector objects layers to Universe B layer
        for (int i = 0; i < connectors.Length; i++)
        {
            connectors[i].gameObject.layer = LayerMask.NameToLayer(universeConnectorLayer);
            connectors[i].collisionMask = LayerMask.GetMask(universeConnectorLayer);
        }
    }

    private void RecursiveSetChildLayer(Transform parent, string layer)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layer);

            RecursiveSetChildLayer(child, layer);
        }
    }
}