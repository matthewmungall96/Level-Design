using System.Collections;
using System.Collections.Generic;
using UniProject;
using UnityEngine;

public class MenuOptions : MonoBehaviour
{
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        // Toggle Menu
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#endif
            Debug.Log("P Pressed");
            var menu = transform.GetChild(0).gameObject;
            menu.SetActive(!menu.activeSelf);
            UniProject.Player.Instance.GetFirstPersonController.enabled = !menu.activeSelf;
            Cursor.visible = menu.activeSelf;
            Cursor.lockState = menu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void Resume()
    {
        var menu = transform.GetChild(0).gameObject;
        menu.SetActive(!menu.activeSelf);
        UniProject.Player.Instance.GetFirstPersonController.enabled = !menu.activeSelf;
        Cursor.visible = menu.activeSelf;
        Cursor.lockState = menu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void HideOverlay()
    {
        GameManager.Instance.GetFadeOverlay.gameObject.SetActive(false);
    }

    public void ShowOverlay()
    {
        GameManager.Instance.GetFadeOverlay.gameObject.SetActive(true);
    }

    public void Respawn()
    {
        ShowOverlay();
        CheckpointManager.Instance.Respawn();
        Resume();
    }
}
