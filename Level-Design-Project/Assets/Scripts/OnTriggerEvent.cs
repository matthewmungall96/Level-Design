using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent onTriggered;
    public bool trigger_once;
    private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (trigger_once && isTriggered)
            return;

        if (other.tag == "Player")
        {
                onTriggered.Invoke();
        }
    }
}
