using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static CheckpointManager Instance { get { return instance; } }

    [SerializeField]
    Transform[] checkpoints;

    int currentCheckpointIdx = 0;
    bool respawning;

    public void MovePlayerToCheckpoint()
    {
        Transform playerTransfrom = UniProject.Player.Instance.transform;

        if (checkpoints.Length >= 0 && currentCheckpointIdx < checkpoints.Length)
        {
            Transform checkpoint = checkpoints[currentCheckpointIdx];
            playerTransfrom.SetParent(null);
            playerTransfrom.position = checkpoint.position;
            playerTransfrom.rotation = checkpoint.rotation;
        }
    }

    public void Respawn()
    {
        if (respawning)
            return;

        respawning = true;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {

        // Fade out
        UniProject.GameManager.Instance.GetFadeOverlay.SetFadeDuration(.3f);
        yield return UniProject.GameManager.Instance.GetFadeOverlay.Fade(-1);

        // Lock movement
        UniProject.Player.Instance.GetFirstPersonController.enabled = false;

        MovePlayerToCheckpoint();

        // FadeIn
        UniProject.GameManager.Instance.GetFadeOverlay.SetFadeDuration(.6f);
        yield return UniProject.GameManager.Instance.GetFadeOverlay.Fade(1);

        // Unlock movement
        UniProject.Player.Instance.GetFirstPersonController.enabled = true;

        respawning = false;
    }

    public void SetCheckpointIndex(int index)
    {
        currentCheckpointIdx = index;
    }

    public void IncrementCheckpointIndex()
    {
        currentCheckpointIdx++;
    }
}
