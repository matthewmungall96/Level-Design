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

        public static bool loadCalled = false;
        public static bool loadFromStart = true;

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
            GetPersistedLevelData = GetComponent<PersistedLevelData>();
        }

        public CustomSceneManager GetSceneManager { get; private set; }
        public FadeOverlay GetFadeOverlay { get; private set; }
        public PersistedLevelData GetPersistedLevelData { get; private set; }

        private void Start()
        {
            // Load in the starting level
            if (loadFromStart)
            {
                GetSceneManager.AsyncLoadSceneAdditive(1);
            }
            else
            {
                var areaData = GameObject.FindObjectOfType<AreaData>();

                // Load alternative time scene if specified in an AreaData object
                if (areaData != null)
                {
                    GetSceneManager.AsyncLoadAlternate(areaData, () =>
                    {
                        //GetFadeOverlay.Fade(1);
                    });
                }
                else
                    GetFadeOverlay.Fade(1);
            }
        }

        public static void StartGameManagementScene()
        {
            if (loadCalled)
                return;

            loadFromStart = false;
            loadCalled = true;

            if(!SceneManager.GetSceneByName("GameManagement").isLoaded)
                SceneManager.LoadSceneAsync("GameManagement", LoadSceneMode.Additive);
        }
    }


}
