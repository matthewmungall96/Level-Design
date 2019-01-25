using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnector : MonoBehaviour
{
    //[HideInInspector]
    public Pipe parent;
    //[HideInInspector] 
    public PipeConnector connected;

    private void OnTriggerEnter(Collider other)
    {
        connected = other.GetComponent<PipeConnector>();
    }

    private void OnTriggerExit(Collider other)
    {
        connected = null;
    }
}