using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniProject;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

public class LevelManager : MonoBehaviour
{
    UnityEvent onStart;

    [SerializeField]
    TextMeshProUGUI openingText;

    [SerializeField]
    string[] openingTextSequence;

    [SerializeField]
    float timeBeforeOpeningSequence = 2;
    [SerializeField]
    float textDuration = 5;
    [SerializeField]
    float alphaFadeInDuration = 1;
    [SerializeField]
    float alphaFadeOutDuration = 1;

    void Start()
    {
        if(onStart != null)
            onStart.Invoke();

        if (GameManager.Instance != null)
        {
            PersistedLevelData levelData = GameManager.Instance.GetPersistedLevelData;

            if (!levelData.GameStarted)
            {
                levelData.GameStarted = true;
                StartCoroutine(LateStart());
            }

            if(levelData.HasTimePiece)
            {
                UniProject.Player.Instance.EnableTimePiece();
            }
        }
        else
        {
            StartCoroutine(LateStart());
        }
    }

    IEnumerator LateStart()
    {
        yield return null;

        yield return StartCoroutine(PlayOpeningSequenceCoroutine());
    }

    IEnumerator PlayOpeningSequenceCoroutine()
    {
        //GameManager.Instance.GetFadeOverlay.SetFade(1);

        FirstPersonController fpc = UniProject.Player.Instance.GetFirstPersonController;
        fpc.enabled = false;

        yield return new WaitForSeconds(timeBeforeOpeningSequence);

        openingText.CrossFadeAlpha(0, 0, false);

        for (int i = 0; i < openingTextSequence.Length; i++)
        {
            string poemText = openingTextSequence[i];
            poemText = poemText.Replace("\\n", "\n");
            openingText.text = poemText;

            openingText.CrossFadeAlpha(1, alphaFadeInDuration, false);

            yield return new WaitForSeconds(alphaFadeInDuration + textDuration);

            openingText.CrossFadeAlpha(0, alphaFadeOutDuration, false);

            yield return new WaitForSeconds(alphaFadeOutDuration);
        }

        yield return GameManager.Instance.GetFadeOverlay.Fade(1);

        fpc.enabled = true;
    }
}
