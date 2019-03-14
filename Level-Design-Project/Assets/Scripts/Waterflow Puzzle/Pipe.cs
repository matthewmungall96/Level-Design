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

    private float targetRotation;

    private int rotationDir = 1;

    private float rotationSpeed = 1;
    public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }

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

        targetRotation = transform.rotation.eulerAngles.z;
    }

    public void Rotate(int dir)
    {
        StopAllCoroutines();
        StartCoroutine("RotateCoroutine", dir);
    }

    IEnumerator RotateCoroutine(int dir)
    {
        // Calculate rotations
        targetRotation += 90;
        Quaternion fromAngle = transform.rotation;
        Quaternion toAngle = Quaternion.Euler(new Vector3(0, 0, targetRotation * dir));

        // Calculate adjusted speed (a = (90 / (target - current)) * 5
        float adjustedRotSpeed = ((targetRotation - fromAngle.eulerAngles.z) / 90) * rotationSpeed;

        for (var t = 0f; t < 1f; t += Time.deltaTime)
        {
            yield return null;
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t * rotationSpeed);
        }

        // Snap to final rotation
        transform.rotation = toAngle;

        onRotateCompleted.Invoke();
    }

    public void OnInteract(bool leftClick)
    {
        Rotate((leftClick) ? 1 : -1);
    }
}
