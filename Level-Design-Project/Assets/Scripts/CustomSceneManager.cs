using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UniProject
{
    public class CustomSceneManager : MonoBehaviour
    {
        private void Awake()
        {
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

            currentScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
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
            }

            yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            isTransitioning = false;
        }

        public void AsyncFadeToScene(string sceneName)
        {
            if (isTransitioning)
                return;

            StartCoroutine("FadeToSceneCoroutine", sceneName);
        }

        public void AsyncFadeToScene(int sceneIndex)
        {
            if (isTransitioning)
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
            }

            // Unload new scene if it's not the same as current
            asyncSceneLoad = SceneManager.UnloadSceneAsync(currentScene.buildIndex);
            
            // Wait for currentScene to unload
            yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

            // Unload alternative scene if loaded
            if(!string.IsNullOrEmpty(oldAlternativeTimeScene))
            {
                asyncSceneLoad = SceneManager.UnloadSceneAsync(oldAlternativeTimeScene);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            }

            // Fade the scene back in
            yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            // Cache a reference to the newly loaded scene
            currentScene = SceneManager.GetSceneByName("sceneName");

            isTransitioning = false;
        }

        IEnumerator FadeToSceneCoroutine(int sceneBuildIndex)
        {
            isTransitioning = true;

            // Fade the scene out
            yield return GameManager.Instance.GetFadeOverlay.Fade(-1);

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
            }

            // Unload old scene and wait for it to finish
            asyncSceneLoad = SceneManager.UnloadSceneAsync(currentScene.buildIndex);
            yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));

            // Unload alternative scene if loaded
            if (!string.IsNullOrEmpty(oldAlternativeTimeScene))
            {
                asyncSceneLoad = SceneManager.UnloadSceneAsync(oldAlternativeTimeScene);
                yield return StartCoroutine(WaitTillAsyncFinished(asyncSceneLoad));
            }

            // Fade the scene back in
            yield return GameManager.Instance.GetFadeOverlay.Fade(1);

            // Cache a reference to the newly loaded scene
            currentScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);

            isTransitioning = false;
        }
    }
}
