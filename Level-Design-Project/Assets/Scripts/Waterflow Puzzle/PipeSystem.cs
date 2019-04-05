using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PipeSystem : MonoBehaviour {

    // The start and end pipes
    public Pipe start, finish;

    // All pipes in the system - Retrieved in start
    Pipe[] pipes;

    // All pipes in the system that have power
    private List<Pipe> poweredPipes = new List<Pipe>();

    // Controls the rotation speed of all pipes in the system
    public float pipeRotationSpeed = 1f;

    // Event is called when connected from start to finish
    public UnityEvent onComplete;

    // Controls whether the system has any power to it
    public bool isPowered = true;

    // Locks interaction on complete
    public bool lockOnComplete = true;

    private void Start()
    {
        pipes = GetComponentsInChildren<Pipe>();

        for(int i = 0; i < pipes.Length; i++)
        {
            pipes[i].onRotate = UpdatePipesStates;
            pipes[i].onRotateCompleted = ()=> UpdatePipesStates();
        }

        SetPipeRotationSpeed(pipeRotationSpeed, pipes);

        // Update powered state
        SetPowered(isPowered);
    }

    public void SetPowered(bool powered)
    {
        isPowered = powered;

        if (isPowered)
        {
            start.HasPower = true;
        }
        else
        {
            start.HasPower = false;
        }

        UpdatePipesStates();
    }

    private void OnValidate()
    {
        SetPipeRotationSpeed(pipeRotationSpeed);
    }

    // Set rotation speed for all pipes in the system
    private void SetPipeRotationSpeed(float speed, Pipe[] pipes = null)
    {
        pipes = pipes ?? GetComponentsInChildren<Pipe>();

        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i].RotationSpeed = pipeRotationSpeed;
        }
    }

    // Recursive method for retrieving all powered pipes
    private void UpdateConnections(Pipe pipe, PipeConnector connectedPoint, Pipe skipPipe = null)
    {
        // Skip the pipe if told to or if the system has no power anyway
        if (skipPipe == pipe || isPowered == false)
            return;

        int connectionsNo = pipe.connections.Length;

        PipeConnector currentConnection, connected;

        poweredPipes.Add(pipe);

        for (int i = 0; i < connectionsNo; i++)
        {
            currentConnection = pipe.connections[i];

            // Skip Connects if it's one that was passed in 
            if (connectedPoint == currentConnection)
                continue;

            connected = currentConnection.connected;

            // Process the connected pipes connections only if it doesn't have power already
            if (connected != null && !poweredPipes.Contains(connected.parent))
            {
                UpdateConnections(connected.parent, connected, skipPipe);
            }
        }
    }

    private void UpdatePipesStates(Pipe skipPipe = null)
    {
        // Empty powered pipes list
        poweredPipes.Clear();

        // Update list of powered pipes
        UpdateConnections(start, null, skipPipe);

        // Activate powered pipes and deactivate non-powered pipes
        for (int i = 0; i < pipes.Length; i++)
        {
            if (poweredPipes.Contains(pipes[i]))
            {
                pipes[i].HasPower = true;
            }
            else
            {
                pipes[i].HasPower = false;
            }
        }

        if (finish.HasPower)
        {
            onComplete.Invoke();

            // Lock interaction on complete if told to
            if (lockOnComplete)
            {
                for (int i = 0; i < pipes.Length; i++)
                {
                    pipes[i].IsLocked = true;
                }
            }
        }
    }

    public void Shuffle()
    {
        //SetPowered(false);
        ////// Clear power from pipes
        ////for (int i = 0; i < pipes.Length; i++)
        ////{
        ////    pipes[i].HasPower = false;
        ////}

        //// Shuffle pipes - skipping the start and finish
        //for (int i = 0; i < pipes.Length; i++)
        //{
        //    if (pipes[i] == start || pipes[i] == finish)
        //        continue;

        //    pipes[i].RotateRandomly(false, false);
        //}

        //UpdatePipesStates();
        StartCoroutine("ShuffleCoroutine");
    }

    IEnumerator ShuffleCoroutine()
    {
        SetPowered(false);

        yield return null;

        // Shuffle pipes - skipping the start and finish
        for (int i = 0; i < pipes.Length; i++)
        {
            if (pipes[i] == start || pipes[i] == finish)
                continue;

            pipes[i].RotateRandomly(false, false);
        }

        while (pipes[0].IsRotating)
        {
            yield return null;
        }

        SetPowered(true);

        UpdatePipesStates();
    }
}
