﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pipe : MonoBehaviour, IInteractable
{
    // Pipe connection points - Colliders
    public PipeConnector[] connections;

    // Event called when rotation has completed
    public delegate void OnRotateCompleted();
    public OnRotateCompleted onRotateCompleted;

    // Event called when rotation begins
    public delegate void OnRotate(Pipe pipe);
    public OnRotate onRotate;

    // Events called when the pipe is powered or not powered
    public UnityEvent onPowered, onNotPowered;

    // Rotation speed for pipe
    private float rotationSpeed = 1;
    public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

    // Flag which indicates whether the pipe is rotating or not
    bool isRotating = false;

    // Command queue for rotations
    Queue<IEnumerator> rotateQueue = new Queue<IEnumerator>();

    // Amount to rotate the pipe by each time
    float rotateByAmount = 90;

    // Holds the pipes state
    bool hasPower;
    public bool HasPower
    {
        get
        {
            return hasPower;
        }

        set
        {
            hasPower = value;

            if (hasPower) onPowered.Invoke(); else onNotPowered.Invoke();
        }
    }

    void Start()
    {
        onNotPowered = onNotPowered ?? new UnityEvent();
        onPowered = onPowered ?? new UnityEvent();

        connections = GetComponentsInChildren<PipeConnector>();

        for (int i = 0; i < connections.Length; i++)
        {
            connections[i].parent = this;
        }
        
        // Temp behaviour on powered
        onPowered.AddListener(() => {
            MeshRenderer[] meshRend = GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRend.Length; i++) {
                meshRend[i].material.SetColor("_BaseColor", Color.green);
            }
        });

        onNotPowered.AddListener(() => {
            MeshRenderer[] meshRend = GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRend.Length; i++)
            {
                meshRend[i].material.SetColor("_BaseColor", Color.blue);
            }
        });
    }

    // Rotates the pipe 90 Degrees
    void Rotate(int dir)
    {
        // Queue the action if a rotation is taking place, else start the rotation
        if(isRotating)
        {
            rotateQueue.Enqueue(RotateCoroutine(dir));
        }
        else
        {
            StartCoroutine("RotateCoroutine", dir);
        }
    }

    IEnumerator RotateCoroutine(int dir)
    {
        isRotating = true;

        onRotate.Invoke(this);

        // Smooth lerp to target rotation
        Quaternion sourceOrientation = transform.rotation;
        Quaternion targetOrientation = sourceOrientation * Quaternion.Euler(0, 0, rotateByAmount * dir);

        for (var t = 0f; t < 1f; t += Time.deltaTime * rotationSpeed)
        {
            yield return null;

            transform.rotation = Quaternion.Slerp(sourceOrientation, targetOrientation, t);
        }

        // Snap to final rotation
        transform.rotation = targetOrientation;     

        isRotating = false;

        // Start the next rotation if one queued, otherwise trigger the on rotate completed event
        if (rotateQueue.Count > 0)
        {
            StartCoroutine(rotateQueue.Dequeue());
        }
        else
        {
            onRotateCompleted.Invoke();
        }
    }

    /// <summary>
    /// Triggered via the inteeraction system
    /// </summary>
    /// <param name="leftClick">True means left click, false means right click</param>
    public void OnInteract(bool leftClick)
    {
        Rotate((leftClick) ? 1 : -1);
    }
}
