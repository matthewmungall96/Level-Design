using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pipe : MonoBehaviour, IInteractable
{
    public PipeConnector[] connections;

    public delegate void OnRotateCompleted();
    public OnRotateCompleted onRotateCompleted;

    public UnityEvent onPowered, onNotPowered;

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

    public float rotationTime = 1;

    void Start()
    {
        connections = GetComponentsInChildren<PipeConnector>();

        for (int i = 0; i < connections.Length; i++)
        {
            connections[i].parent = this;
        }

        onPowered.AddListener(() => {
            MeshRenderer[] meshRend = GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRend.Length; i++) {
                meshRend[i].material.color = Color.green;
            }
        });

        onNotPowered.AddListener(() => {
            MeshRenderer[] meshRend = GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRend.Length; i++)
            {
                meshRend[i].material.color = Color.blue;
            }
        });
    }

    public void Rotate()
    {
        StartCoroutine("RotateCoroutine");
    }

    IEnumerator RotateCoroutine()
    {
        Quaternion fromAngle = transform.rotation;
        Quaternion toAngle = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90));

        for (var t = 0f; t < 1; t += Time.deltaTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, Time.deltaTime);
            Debug.Log("Rotation: " + transform.rotation.eulerAngles);
        }

        // Snap to final rotation
        transform.rotation = toAngle;

        onRotateCompleted.Invoke();
    }

    public void OnInteract()
    {
        Rotate();
    }
}
