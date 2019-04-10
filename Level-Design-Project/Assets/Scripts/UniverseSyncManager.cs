using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniProject;

public class UniverseSyncManager : MonoBehaviour
{
    List<UniverseTransformSync> universeASyncs;
    List<UniverseTransformSync> universeBSyncs;

    bool linkEstablished = false;

    private void Awake()
    {
        UniverseController.onUniverseChanged = UniverseController.onUniverseChanged ?? new OnUniverseChanged();
        UniverseController.onUniverseChanged.AddListener(OnUniverseChanged);
    }

    private void OnDestroy()
    {
        UniverseController.onUniverseChanged.RemoveListener(OnUniverseChanged);
    }

    private void Start()
    {
        StartCoroutine(LateStart());        
    }

    IEnumerator LateStart()
    {
        yield return null;

        SceneManager.sceneLoaded -= OnNewSceneLoaded;
        SceneManager.sceneLoaded += OnNewSceneLoaded;
    }

    void OnNewSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        // Do not update for Game management scene
        if (scene.name == "GameManagement")
            return;

        TryEstablishConnections();
    }

    // Tries to establish connections between objects
    void TryEstablishConnections()
    {
        universeASyncs = new List<UniverseTransformSync>();
        universeBSyncs = new List<UniverseTransformSync>();

        GameObject[] syncsGO = GameObject.FindGameObjectsWithTag("UniverseTransformSyncer");

        // Extract all sync components
        for(int i = 0; i < syncsGO.Length; i++)
        {
            var syncComponent = syncsGO[i].GetComponent<UniverseTransformSync>();

            if (syncComponent != null)
            {
                if (syncsGO[i].layer == LayerMask.NameToLayer("UniverseA"))
                    universeASyncs.Add(syncComponent);
                else if (syncsGO[i].layer == LayerMask.NameToLayer("UniverseB"))
                    universeBSyncs.Add(syncComponent);
                else
                    Debug.LogWarning("Unable to Sync Universe transform: " + syncsGO[i].name + "\nObject not assigned to a universe layer.", syncsGO[i]);
            }
        }

        // Inefficiently establish links
        for(int i = 0; i < universeASyncs.Count; i++)
        {
            for(int j = 0; j < universeBSyncs.Count; j++)
            {
                // Skip empty sync ids or is already synced
                if (string.IsNullOrEmpty(universeASyncs[i].SyncID) || universeASyncs[i].syncedTransform != null)
                    return;

                // Establish link if syncIDs match
                if(universeASyncs[i].SyncID == universeBSyncs[j].SyncID)
                {
                    universeASyncs[i].syncedTransform = universeBSyncs[j].transform;
                    universeBSyncs[j].syncedTransform = universeASyncs[i].transform;

                    universeASyncs[i].OnConnectionMade();
                    Debug.Log("Item A", universeASyncs[i]);
                    universeBSyncs[j].OnConnectionMade();
                    Debug.Log("Item B", universeBSyncs[j]);

                    break;
                }
            }
        }

        linkEstablished = true;
    }

    public void OnUniverseChanged(int universe)
    {
        SetDominantUniverse((Universes)universe);
    }

    public void SetDominantUniverse(Universes universe)
    {
        bool isDominantUniverse = (universe == Universes.A);

        for(int i = 0; i < universeASyncs.Count; i++)
        {
            universeASyncs[i].IsDominantTransform = isDominantUniverse; 
        }

        isDominantUniverse = (universe == Universes.B);

        for (int i = 0; i < universeBSyncs.Count; i++)
        {
            universeBSyncs[i].IsDominantTransform = isDominantUniverse;
        }
    }

    private void FixedUpdate()
    {
        if (!linkEstablished)
            return;

        for (int i = 0; i < universeASyncs.Count; i++)
        {
            if (universeASyncs[i].syncedTransform == null)
                continue;

            bool universeAChanged = universeASyncs[i].transform.hasChanged;
            bool universeBChanged = universeASyncs[i].syncedTransform.hasChanged;

            // Sync Transforms
            if (universeAChanged && universeBChanged)
            {
                // if both transforms update then update dominant one 
                if(universeASyncs[i].IsDominantTransform)
                {
                    universeASyncs[i].syncedTransform.localRotation = universeASyncs[i].transform.localRotation;
                    universeASyncs[i].syncedTransform.localPosition = universeASyncs[i].transform.localPosition;
                    universeASyncs[i].syncedTransform.hasChanged = false;
                    universeASyncs[i].transform.hasChanged = false;
                }
                else
                {
                    universeASyncs[i].transform.localPosition = universeASyncs[i].syncedTransform.localPosition;
                    universeASyncs[i].transform.localRotation = universeASyncs[i].syncedTransform.localRotation;
                    universeASyncs[i].syncedTransform.hasChanged = false;
                    universeASyncs[i].transform.hasChanged = false;
                }
            }
            else if(universeAChanged)
            {
                universeASyncs[i].syncedTransform.localRotation = universeASyncs[i].transform.localRotation;
                universeASyncs[i].syncedTransform.localPosition = universeASyncs[i].transform.localPosition;
                universeASyncs[i].syncedTransform.hasChanged = false;
                universeASyncs[i].transform.hasChanged = false;
            }
            else if(universeBChanged)
            {
                universeASyncs[i].transform.localPosition = universeASyncs[i].syncedTransform.localPosition;
                universeASyncs[i].transform.localRotation = universeASyncs[i].syncedTransform.localRotation;
                universeASyncs[i].syncedTransform.hasChanged = false;
                universeASyncs[i].transform.hasChanged = false;
            }
        }
    }
}
