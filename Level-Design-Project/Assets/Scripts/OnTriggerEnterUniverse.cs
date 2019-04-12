using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class OnTriggerEnterUniverse : MonoBehaviour
{
    [SerializeField]
    UnityEvent onTriggered;
    [SerializeField]
    bool isTriggeredOnce;
    [SerializeField]
    Universes triggeredUniverse;

    static UniverseController universeController;

    bool isTriggered;

    private void Start()
    {
        universeController = GameObject.FindObjectOfType<UniverseController>();
    }

    private void OnValidate()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggeredOnce && isTriggered)
            return;

        if(other.tag == "Player" && triggeredUniverse == universeController.currentUniverse)
        {
            onTriggered.Invoke();
        }
    }
}
