using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour, IInteractable
{
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private UnityEvent OnInteraction;

    private bool isTriggered = false;

    public void OnInteract(bool isLeftClick)
    {
        Debug.Log("Interact");
        // Stop processing if set to trigger only once and has been triggered
        if (triggerOnce && isTriggered)
            return;

        // Call the passed in behavioour
        OnInteraction.Invoke();
    }
}
