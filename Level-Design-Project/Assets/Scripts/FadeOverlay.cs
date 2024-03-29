﻿using System.Collections;
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

    public Coroutine Fade(int direction, bool startFullFade = true)
    {
        return (direction < 0) ? StartCoroutine("FadeOut", startFullFade) : StartCoroutine("FadeIn", startFullFade);
    }

    public void SetFadeDuration(float duration)
    {
        fadeDuration = duration;
    }

    private IEnumerator FadeOut(bool startFullFade = true)
    {
        Color color = overlayImg.color;
        if(startFullFade)
            color.a = 0;
        overlayImg.color = color;

        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            color = overlayImg.color;
            color.a = fadeOutCurve.Evaluate(1 - i / fadeDuration);
            overlayImg.color = color;

            yield return null;
        }

        SetFade(1);
    }

    public void SetFade(float alpha)
    {
        Color color = overlayImg.color;
        color.a = alpha;
        overlayImg.color = color;
    }

    private IEnumerator FadeIn(bool startFullFade = true)
    {
        Color color = overlayImg.color;
        if(startFullFade)
        color.a = 1;
        overlayImg.color = color;

        for (float i = 0; i < fadeDuration; i += Time.deltaTime)
        {
            color = overlayImg.color;
            color.a = fadeOutCurve.Evaluate(i / fadeDuration);
            overlayImg.color = color;

            yield return null;
        }

        SetFade(0);
    }
}
