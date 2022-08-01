using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [SerializeField]
    private Volume volume;

    private Vignette vignette;

    private void Start()
    {
        volume.profile.TryGet(out vignette);
    }

    public void SetVignette(float value)
    {
        vignette.intensity.value = value;
    }

    public void SetVignetteSmoothly(float endValue, float duration)
    {
        DOTween.To(() => vignette.intensity.value, SetVignette, endValue, duration);
    }
}
