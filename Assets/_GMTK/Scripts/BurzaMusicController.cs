using AetherEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurzaMusicController : AudioController
{
    [SerializeField]
    private AudioSource burzaEndingMusicSource;
    [SerializeField]
    private int startPlayingBurzaMusicAfterThisNumberOfDrops = 2;
    [SerializeField]
    private float burzaMusicIncreaseDuration = 5f;

    protected override void Awake()
    {
        base.Awake();
        BombsDropped.AddListener(eventData => TryToAdjustBurzaMusicVolume(eventData.WhichDroppingIsThis));
    }

    public void TryToAdjustBurzaMusicVolume(int currentDropsDone)
    {
        if(currentDropsDone >= startPlayingBurzaMusicAfterThisNumberOfDrops)
        {
            float newTargetVolume = burzaEndingMusicSource.volume + burzaMusicVolumeAmountToAdjustOnBombsDrop();
            SoundManager.Instance.AdjustVolumeByTweening(burzaEndingMusicSource, newTargetVolume, burzaMusicIncreaseDuration);
        }

        float burzaMusicVolumeAmountToAdjustOnBombsDrop() => 1f / (GameplayManager.Instance.player.droppingBombsNumToGoIntoMadness - startPlayingBurzaMusicAfterThisNumberOfDrops);
    }

    protected override void DoOnAudioSource(Action<AudioSource> action)
    {
        action(burzaEndingMusicSource);
    }
}
