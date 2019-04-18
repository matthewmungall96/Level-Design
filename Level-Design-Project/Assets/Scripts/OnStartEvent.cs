using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStartEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onStartEvent;

    void Start()
    {
        if(onStartEvent != null)
        {
            onStartEvent.Invoke();
        }
    }
}
