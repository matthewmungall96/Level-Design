using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollidingPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            Debug.Log("With Colliding.");

            collider.transform.SetParent(transform);
        }
        Debug.Log("Colliding.");
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            collider.transform.SetParent(null);
        }
    }
}
