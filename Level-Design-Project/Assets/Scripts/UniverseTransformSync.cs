using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UniverseTransformSync : MonoBehaviour
{
    string parentSceneName = "";

    //[HideInInspector]
    public Transform syncedTransform;

    [SerializeField] string syncID;

    public string SyncID { get { return syncID; } }
    public bool IsDominantTransform { get; set; }

    protected UnityEvent onControlledMovementComplete;
    
    private void Awake()
    {
        parentSceneName = gameObject.scene.name;
    }

    public virtual void OnConnectionMade() { }
}
