using System.Collections;
using System.Collections.Generic;
using UniProject;
using UnityEngine;
using UnityEngine.Events;

public class Monument : MonoBehaviour
{
    // Material List
    // 0. Base Material
    // 1. Waterworks Powered
    // 2. Observatory Powered
    // 3. Waterworks & Observatory Powered
    // 4. All Powered
    Material[] materials;

    [SerializeField]
    Material poweredMat;
    [SerializeField]
    Material unpoweredMat;

    [SerializeField]
    bool isWaterworksPowered;
    [SerializeField]
    bool isObservatoryPowered;

    [SerializeField]
    UnityEvent onPowered;

    [SerializeField]
    UnityEvent onUnpowered;

    [SerializeField]
    UnityEvent onObservatoryPowered;

    [SerializeField]
    UnityEvent onWaterworksPowered;

    private void OnValidate()
    {
        materials = GetComponent<MeshRenderer>().sharedMaterials;
        SetComplete(isWaterworksPowered, isObservatoryPowered, true);
    }

    private void Awake()
    {
        materials = GetComponent<MeshRenderer>().sharedMaterials;
    }

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;

        // Load persisted data
        if (GameManager.Instance != null)
        {
            PersistedLevelData levelData = GameManager.Instance.GetPersistedLevelData;
            SetComplete(levelData.WaterworksPowered, levelData.ObservatoryPowered, true);
        }
    }

    public void SetComplete(bool waterworks, bool observatory, bool triggerEvents = false)
    {
        // Choose what sections should have the powered material
        if(waterworks && observatory)
        {
            materials[1] = poweredMat;
            materials[2] = poweredMat;
            materials[3] = poweredMat;
            materials[4] = poweredMat;

            if(triggerEvents && onPowered != null)
                onPowered.Invoke();

            if (onWaterworksPowered != null)
                onWaterworksPowered.Invoke();

            if (onObservatoryPowered != null)
                onObservatoryPowered.Invoke();
        }
        else if(waterworks)
        {
            if (triggerEvents && onUnpowered != null)
                onUnpowered.Invoke();

            if (onWaterworksPowered != null)
                onWaterworksPowered.Invoke();

            materials[1] = poweredMat;
            materials[2] = unpoweredMat;
            materials[3] = unpoweredMat;
            materials[4] = unpoweredMat;
        }
        else if (observatory)
        {
            if (triggerEvents && onUnpowered != null)
                onUnpowered.Invoke();

            if (onObservatoryPowered != null)
                onObservatoryPowered.Invoke();

            materials[1] = unpoweredMat;
            materials[2] = poweredMat;
            materials[3] = unpoweredMat;
            materials[4] = unpoweredMat;
        }
        else
        {
            if (triggerEvents && onUnpowered != null)
                onUnpowered.Invoke();

            materials[1] = unpoweredMat;
            materials[2] = unpoweredMat;
            materials[3] = unpoweredMat;
            materials[4] = unpoweredMat;
        }

        GetComponent<MeshRenderer>().sharedMaterials = materials;
    }
}
