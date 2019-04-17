using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnector : MonoBehaviour
{
    [HideInInspector]
    public Pipe parent;
    //[HideInInspector] 
    public PipeConnector connected;
    private int connectedID;

    [SerializeField]
    public LayerMask collisionMask;

    private void OnTriggerStay(Collider other)
    {
        if(connected != null && other.GetInstanceID() == connectedID)
        {
            return;
        }

        if (collisionMask == (collisionMask | (1 << other.gameObject.layer)))
        {
            connected = other.GetComponent<PipeConnector>();
            connectedID = other.GetInstanceID();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        connected = null;
    }
}