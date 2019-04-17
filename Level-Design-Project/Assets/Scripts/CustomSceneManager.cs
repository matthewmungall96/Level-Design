using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UniProject
{
    public class CustomSceneManager : MonoBehaviour
    {
        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene();
            AreaData.onSpawn = (sceneData) => { SetAreaData(sceneData); };
        }

        private void Start()
        {
            StartCoroutine("LateStart");
        }

        IEnumerator LateStart()
        {
            yield return null;
        }

        Scene currentScene;
        public static string currentAlternativeTimeScene;

        public UnityEvent onSceneMerged;

        bool isTransitioning = false;

        public void SetAreaData(string alternativeScene)
        {
            currentAlternativeTimeScene = alternativeScene;
        }

        public void AsyncLoadSceneAdditive(int sceneIndex)
        {
            if (isTransitioning)
                return;

            StartCoroutine("AsyncLoadSceneAdditiveCoroutine", sceneIndex);
        }

        IEnumerator AsyncLoadSceneAdditiveCoroutine(int sceneBuildIndex)
        {
            isTransitioning = true;

            AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

            yield return StartCoroutine("WaitTillAsyncFinished", asyncSceneLoad);

            // Load alternative time scene if updated
            if (!string.IsNullOrEmpty(currentAlternativeTimeScene))
            {
                asyncSceneLoad = SceneManager.LoadSceneAsync(currentAlternativeTimeScene, LoadSceneMode.Additive);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

                SceneManager.MergeScenes(SceneManager.GetSceneByName(currentAlternativeTimeScene), SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
            }

            currentScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);

            //yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            isTransitioning = false;
        }

        public void AsyncFadeToScene(string sceneName)
        {
            if (isTransitioning || string.IsNullOrEmpty(sceneName))
                return;
            Debug.Log("Fading to scene.");
            StartCoroutine("FadeToSceneCoroutine", sceneName);
        }

        public void AsyncFadeToScene(int sceneIndex)
        {
            if (isTransitioning || sceneIndex > SceneManager.sceneCountInBuildSettings - 1)
                return;

            StartCoroutine("FadeToSceneCoroutine", sceneIndex);
        }

        IEnumerator WaitTillAsyncFinished(AsyncOperation asyncOperation)
        {
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

        IEnumerator FadeToSceneCoroutine(string sceneName)
        {
            isTransitioning = true;

            // Fade the scene out
            yield return GameManager.Instance.GetFadeOverlay.Fade(-1);

            // Disable lights to remove dual direction light exception
            var lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                light.enabled = false;
            }

            string oldAlternativeTimeScene = currentAlternativeTimeScene;

            // Begin loading the scene asynchronously
            AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Wait for scene to load
            yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

            // Load alternative time scene if updated
            if (!string.IsNullOrEmpty(currentAlternativeTimeScene) && oldAlternativeTimeScene != currentAlternativeTimeScene)
            {
                asyncSceneLoad = SceneManager.LoadSceneAsync(currentAlternativeTimeScene, LoadSceneMode.Additive);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

                SceneManager.MergeScenes(SceneManager.GetSceneByName(currentAlternativeTimeScene), SceneManager.GetSceneByName(sceneName));
            }

            // Unload new scene if it's not the same as current
            if (currentScene.IsValid() && currentScene.isLoaded)
            {
                asyncSceneLoad = SceneManager.UnloadSceneAsync(currentScene.buildIndex);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            }

            // Unload alternative scene if loaded
            //if (!string.IsNullOrEmpty(oldAlternativeTimeScene))
            //{
            //    asyncSceneLoad = SceneManager.UnloadSceneAsync(oldAlternativeTimeScene);
            //    yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            //}

            yield return new WaitForSeconds(1);

            // Fade the scene back in
            yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            // Cache a reference to the newly loaded scene
            currentScene = SceneManager.GetSceneByName(sceneName);

            isTransitioning = false;
        }

        IEnumerator FadeToSceneCoroutine(int sceneBuildIndex)
        {
            isTransitioning = true;

            // Fade the scene out
            yield return GameManager.Instance.GetFadeOverlay.Fade(-1);

            // Disable lights to remove dual direction light exception
            var lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                light.enabled = false;
            }

            string oldAlternativeTimeScene = currentAlternativeTimeScene;

            // Begin loading the scene asynchronously
            AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

            // Wait for scene to load
            yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

            // Load alternative time scene if updated
            if (!string.IsNullOrEmpty(currentAlternativeTimeScene) && oldAlternativeTimeScene != currentAlternativeTimeScene)
            {
                asyncSceneLoad = SceneManager.LoadSceneAsync(currentAlternativeTimeScene, LoadSceneMode.Additive);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

                SceneManager.MergeScenes(SceneManager.GetSceneByName(currentAlternativeTimeScene), SceneManager.GetSceneByBuildIndex(sceneBuildIndex));
            }

            // Unload old scene and wait for it to finish
            if (currentScene.IsValid() && currentScene.isLoaded)
            {
                asyncSceneLoad = SceneManager.UnloadSceneAsync(currentScene.buildIndex);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            }

            // Unload alternative scene if loaded
            //if (!string.IsNullOrEmpty(oldAlternativeTimeScene))
            //{
            //    asyncSceneLoad = SceneManager.UnloadSceneAsync(oldAlternativeTimeScene);
            //    yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            //}

            yield return new WaitForSeconds(1);

            // Fade the scene back in
            yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            // Cache a reference to the newly loaded scene
            currentScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);

            isTransitioning = false;
        }

        public void AsyncLoadAlternate(AreaData areaData, Action onComplete)
        {
            StartCoroutine(LoadAlternateCoroutine(areaData, onComplete));
        }

        private IEnumerator LoadAlternateCoroutine(AreaData areaData, Action onComplete)
        {
            if (string.IsNullOrEmpty(areaData.alterativeTimeSceneName))
            {
                onComplete.Invoke();

                yield break;
            }

            yield return null;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(areaData.alterativeTimeSceneName, LoadSceneMode.Additive);

            yield return StartCoroutine(WaitTillAsyncFinished(asyncLoad));

            SceneManager.MergeScenes(SceneManager.GetSceneByName(areaData.alterativeTimeSceneName), currentScene);

            yield return null;

            onSceneMerged.Invoke();

            yield return null;

            onComplete.Invoke();
        }
    }
}
