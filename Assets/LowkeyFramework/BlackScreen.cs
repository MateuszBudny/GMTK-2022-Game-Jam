using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : SingleBehaviour<BlackScreen>
{
    private Image blackImage;

    protected override void Awake()
    {
        base.Awake();
        blackImage = GetComponent<Image>();
    }

    public void FadeIn(float duration, bool forceStartFromBeginning = false, Action onComplete = null)
    {
        Fade(duration, 1f, forceStartFromBeginning, onComplete);
    }

    public void FadeOut(float duration, bool forceStartFromBeginning = false, Action onComplete = null)
    {
        Fade(duration, 0f, forceStartFromBeginning, onComplete);
    }

    private void Fade(float duration, float toAlphaValue, bool forceStartFromBeginning, Action onComplete)
    {
        DOTween.Kill(this);

        if(forceStartFromBeginning)
        {
            blackImage.color = new Color(0f, 0f, 0f, 1f - toAlphaValue);
        }

        DOTween.To(() => blackImage.color.a, setter => blackImage.color = new Color(0f, 0f, 0f, setter), toAlphaValue, duration)
            .SetEase(Ease.InOutSine)
            .SetTarget(this)
            .OnComplete(() => onComplete?.Invoke());
    }
}
