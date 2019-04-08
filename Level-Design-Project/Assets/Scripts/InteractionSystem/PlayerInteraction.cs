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

    public bool UseMousePosition { get; set; }

    void Start () {
        rayCam = Camera.main;	

        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;
    }
	
	void Update () {

        bool leftClicked = Input.GetMouseButtonDown(0);

        if (leftClicked || Input.GetMouseButtonDown(1))
        {
            // Select appropriate ray
            if (!UseMousePosition)
            { 
                ray = rayCam.ScreenPointToRay(new Vector3(halfScreenWidth, halfScreenHeight, 0));
            }
            else
            {
                ray = rayCam.ScreenPointToRay(Input.mousePosition);
            }

            if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.OnInteract(leftClicked);
                }
            }
        }
	}
}
