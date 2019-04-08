using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniProject
{
    [RequireComponent(typeof(BoxCollider))]
    public class SceneTransitionTrigger : MonoBehaviour
    {
        [SerializeField] string baseTransitionScene;

        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                Debug.Log("Loading GameManager..");
                GameManager.StartGameManagementScene();
            }

            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            GameManager.Instance.GetSceneManager.AsyncFadeToScene(baseTransitionScene);          
        }
    }
}