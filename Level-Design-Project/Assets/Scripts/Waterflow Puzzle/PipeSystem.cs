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

    // Controls the rotation speed of all pipes in the system
    public float pipeRotationSpeed = 1f;

    public List<Pipe> poweredPipes;

    // Event is called when connected from start to finish
    public UnityEvent onComplete;

    // Event is called when box receives power
    public UnityEvent onPowered;

    // Controls whether the system has any power to it
    public bool isPowered = true;

    // Locks interaction on complete
    public bool lockOnComplete = true;

    [SerializeField] public LayerMask connectorCollisionMask;

    public void SetConnectorCollisionMask(LayerMask collisionMask)
    {
        connectorCollisionMask = collisionMask;
    }

    private void Start()
    {
        poweredPipes = new List<Pipe>();

        pipes = GetComponentsInChildren<Pipe>();

        for(int i = 0; i < pipes.Length; i++)
        {
            pipes[i].onRotate += UpdatePipesStates;
            pipes[i].onRotateCompleted += ()=> UpdatePipesStates();
            pipes[i].SetConnectorCollisionMask(this.connectorCollisionMask);
        }

        SetPipeRotationSpeed(pipeRotationSpeed, pipes);

        AsyncUpdatePipeConnectors();

        // Update powered state
        SetPowered(isPowered);
    }

    private void AsyncUpdatePipeConnectors()
    {
        StartCoroutine(UpdatePipeConnectorsCoroutine());
    }

    private IEnumerator UpdatePipeConnectorsCoroutine()
    {
        Vector3 tempPos;

        // Loop over all pipes
        for (int i = 0; i < pipes.Length; i++)
        {
            // Move Pipe to Narnia
            tempPos = pipes[i].transform.position;
            pipes[i].transform.position = new Vector3(0, 99999, 0);

            // Wait a frame to allow pipe connector collision to update
            yield return null;

            pipes[i].transform.position = tempPos;

            // Wait a frame
            yield return null;
        }
    }

    public void SetPowered(bool powered)
    {
        isPowered = powered;

        if (isPowered)
        {
            start.HasPower = true;

            if(onPowered != null)
                onPowered.Invoke();
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
        SetConnectorCollisionMask(this.connectorCollisionMask);
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
    private IEnumerator UpdateConnections(List<Pipe> poweredPipes, Pipe pipe, PipeConnector connectedPoint, Pipe skipPipe = null)
    {
        if(poweredPipes == null)
        {
            poweredPipes = new List<Pipe>();
        }

        // Skip the pipe if told to or if the system has no power anyway
        if (skipPipe == pipe || isPowered == false)
            yield break;

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
                yield return UpdateConnections(poweredPipes, connected.parent, connected, skipPipe);
            }
        }
    }

    Coroutine updatePipeStatesCoroutine;

    public void UpdatePipesStates(Pipe skipPipe = null)
    {
        // Ensure update pipes state is only run once at a time
        if(updatePipeStatesCoroutine != null)
        {
            //StopCoroutine(updatePipeStatesCoroutine);
        }

        isUpdatingPipeStates = true;
        updatePipeStatesCoroutine = StartCoroutine(UpdatePipeStatesCoroutine(skipPipe));
    }

    bool isUpdatingPipeStates = false;

    public IEnumerator UpdatePipeStatesCoroutine(Pipe skipPipe = null)
    { 
        // Empty powered pipes list
        poweredPipes.Clear();

        // Wait for connectors to update
        yield return new WaitForFixedUpdate();

        // Update list of powered pipe connectors
        yield return StartCoroutine(UpdateConnections(poweredPipes, start, null, skipPipe));

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

        yield return null;

        if (finish != null && finish.HasPower)
        {
            while (isUpdatingPipeStates)
            {
                onComplete.Invoke();
                isUpdatingPipeStates = false;
            }

            // Lock interaction on complete if told to
            if (lockOnComplete)
            {
                Lock();
            }
        }
        else
            isUpdatingPipeStates = false;
    }

    public void Shuffle()
    {
        //UpdatePipesStates();
        StartCoroutine("ShuffleCoroutine");
    }

    public void Lock()
    {
        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i].IsLocked = true;
        }
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
