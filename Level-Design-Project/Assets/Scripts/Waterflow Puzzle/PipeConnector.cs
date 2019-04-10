﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnector : MonoBehaviour
{
    [HideInInspector]
    public Pipe parent;
    [HideInInspector] 
    public PipeConnector connected;
    [HideInInspector]
    public LayerMask collisionMask;

    private void OnTriggerStay(Collider other)
    {
        if (collisionMask == (collisionMask | (1 << other.gameObject.layer)))
        {
            connected = other.GetComponent<PipeConnector>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        connected = null;
    }
}