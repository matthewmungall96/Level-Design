using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniProject
{
    [RequireComponent(typeof(BoxCollider))]
    public class SceneTransitionTrigger : MonoBehaviour
    {
        [SerializeField] string baseTransitionScene;

        private void OnTriggerEnter(Collider other)
        {
            GameManager.Instance.GetSceneManager.AsyncFadeToScene(baseTransitionScene);
        }
    }
}