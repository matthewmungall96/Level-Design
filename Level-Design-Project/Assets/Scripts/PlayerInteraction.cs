using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

    Ray ray;
    RaycastHit hit;

    Camera rayCam;

    float halfScreenWidth, halfScreenHeight;

    public float interactionDistance = 1f;
    public LayerMask layerMask;

	void Start () {
        rayCam = Camera.main;	

        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;
    }
	
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            ray = rayCam.ScreenPointToRay(new Vector3(halfScreenWidth, halfScreenHeight, 0));

            Debug.Log("Boop");
            if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
            {

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                Debug.Log("Other collider: " + hit.collider.name);
                if (interactable != null)
                {
                    Debug.Log("Beep");

                    interactable.OnInteract();
                }
            }
        }
	}
}
