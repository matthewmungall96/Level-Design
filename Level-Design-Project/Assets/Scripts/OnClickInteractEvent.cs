using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnClickInteractEvent : MonoBehaviour, IInteractable
{
    [SerializeField]
    UnityEvent onInteract;
    [SerializeField]
    bool triggerOnce = true;

    bool isTriggered = false;

    public void OnInteract(bool isLeftClick)
    {
        if (triggerOnce && isTriggered)
            return;

        if(onInteract != null)
        {
            onInteract.Invoke();
            isTriggered = true;
        }
    }
}
