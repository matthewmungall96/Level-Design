using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniProject
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        [SerializeField] int startingSceneIndex = 1;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }    
            else
            {
                Destroy(gameObject);
            }

            GetSceneManager = GetComponentInChildren<CustomSceneManager>();
            GetFadeOverlay = GetComponentInChildren<FadeOverlay>();
        }

        public CustomSceneManager GetSceneManager { get; private set; }
        public FadeOverlay GetFadeOverlay { get; private set; }

        private void Start()
        {
            // Load in the starting level
            if (!SceneManager.GetSceneByBuildIndex(1).isLoaded)
                GetSceneManager.AsyncLoadSceneAdditive(1);
            else
                GetFadeOverlay.Fade(1);
        }

        public static void StartGameManagementScene()
        {
            SceneManager.LoadSceneAsync("GameManagement", LoadSceneMode.Additive);
        }
    }
}
