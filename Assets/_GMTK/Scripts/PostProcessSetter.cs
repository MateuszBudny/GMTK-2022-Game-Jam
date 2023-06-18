using AetherEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessSetter : MonoBehaviour
{
    [SerializeField]
    private PostProcessController controller;

    private void Awake()
    {
        controller.Init(() => GameplayManager.Instance.CurrentVCam);
        ALittleBitOfSanityLost.AddListener(OnALittleBitOfSanityLost);
    }

    private void OnALittleBitOfSanityLost(ALittleBitOfSanityLost eventData)
    {
        controller.SetVignetteSmoothlyAsMaxPercentage(eventData.instanityProgress);
        controller.SetVCamNoiseMovementProgress(eventData.instanityProgress);
    }
}
