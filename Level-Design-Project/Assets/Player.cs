﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    public static Player Instance { get { return instance; } }

    public PlayerInteraction GetInteractionSystem { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        GetInteractionSystem = GetComponent<PlayerInteraction>();    
    }
}
