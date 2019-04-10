using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Pipe))]
public class PipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Pipe pipe = (Pipe)target;
        if (GUILayout.Button("Create child connector"))
        {
            CreateConnector(pipe.transform);
        }
    }

    void CreateConnector(Transform parent)
    {
        GameObject connector = new GameObject();
        connector.tag = "Pipe-Connector";
        connector.name = "Connector";
        connector.transform.SetParent(parent);
        connector.transform.localPosition = Vector3.zero;
        connector.transform.localRotation = Quaternion.identity;

        SphereCollider connectorTrigger = connector.AddComponent<SphereCollider>();
        connectorTrigger.isTrigger = true;
        connectorTrigger.radius = .02f;

        Rigidbody connectorRB = connector.AddComponent<Rigidbody>();
        connectorRB.isKinematic = true;

        connector.AddComponent<PipeConnector>();
        
    }
}