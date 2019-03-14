using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PipeSystem : MonoBehaviour {

    public Pipe start, finish;
    Pipe[] pipes;

    public float pipeRotationSpeed = 1f;

    public UnityEvent onComplete;

    private void Start()
    {
        pipes = GetComponentsInChildren<Pipe>();
        
        for(int i = 0; i < pipes.Length; i++)
        {
            pipes[i].onRotateCompleted = () => UpdateConnections(start, null);
        }

        SetPipeRotationSpeed(pipeRotationSpeed, pipes);
    }

    private void OnValidate()
    {
        SetPipeRotationSpeed(pipeRotationSpeed);
    }

    private void SetPipeRotationSpeed(float speed, Pipe[] pipes = null)
    {
        pipes = pipes ?? GetComponentsInChildren<Pipe>();

        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i].RotationSpeed = pipeRotationSpeed;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ClearPower();
            UpdateConnections(start, null);

            if(finish.HasPower)
            {
                Debug.Log("Rad");
            }
        }
    }

    private void UpdateConnections(Pipe pipe, PipeConnector connectedPoint)
    {
        int connectionsNo = pipe.connections.Length;

        PipeConnector currentConnection, connected;

        pipe.HasPower = true;

        for (int i = 0; i < connectionsNo; i++)
        {
            currentConnection = pipe.connections[i];

            // Skip Connects if it's one that was passed in 
            if (connectedPoint == currentConnection)
                continue;

            connected = currentConnection.connected;

            // Process the connected pipes connections only if it doesn't have power already
            if (connected != null && !connected.parent.HasPower)
            {
                UpdateConnections(connected.parent, connected);
            }
        }
    }   
    
    void ClearPower()
    {
        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i].HasPower = false;
        }
    }
}
