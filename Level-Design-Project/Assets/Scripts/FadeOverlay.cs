using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOverlay : MonoBehaviour
{
    [SerializeField] Image overlayImg;
    [SerializeField] Color overlayColour = new Color(0, 0, 0);
    [SerializeField] AnimationCurve fadeInCurve;
    [SerializeField] AnimationCurve fadeOutCurve;
    [SerializeField] float fadeDuration = 1;
    [SerializeField] bool fadedOnLoad = true;

    public void Start()
    {
        overlayImg = GetComponentInChildren<Image>();
        overlayImg.color = overlayColour;

        Color col = overlayImg.color;
        col.a = (fadedOnLoad) ? 1 : 0;
        overlayImg.color = col;
    }

    public Coroutine Fade(int direction)
    {
        return (direction < 0) ? StartCoroutine("FadeOut") : StartCoroutine("FadeIn");
    }

    public IEnumerator FadeOut()
    {
        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            Color color = overlayImg.color;
            color.a = fadeOutCurve.Evaluate(1 - i / fadeDuration);
            overlayImg.color = color;

            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            Color color = overlayImg.color;
            color.a = fadeOutCurve.Evaluate(i / fadeDuration);
            overlayImg.color = color;

            yield return null;
        }
    }
}
