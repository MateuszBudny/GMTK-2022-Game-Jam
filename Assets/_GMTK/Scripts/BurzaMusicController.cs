using AetherEvents;
using DG.Tweening;
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

    private float currentBurzaVolumeFromBombsDropping = 0f;
    private float currentBurzaVolumeFromGunAngle = 0f;

    protected override void Awake()
    {
        base.Awake();
        BombsDropped.AddListener(eventData => TryToAdjustBurzaMusicVolume(eventData.WhichDroppingIsThis));
    }

    private void Update()
    {
        if(GameplayManager.Instance.player.IsPlayerHoldingGun)
        {
            currentBurzaVolumeFromGunAngle = CalculateBurzaVolumeFromGunAngle(GameplayManager.Instance.player.gunHolder.localRotation.eulerAngles.y);
            UpdateBurzaVolume();
        }
        else
        {
            if(!Mathf.Approximately(currentBurzaVolumeFromGunAngle, 0f))
            {
                currentBurzaVolumeFromGunAngle = 0f;
                UpdateBurzaVolume();
            }
        }
    }

    public void TryToAdjustBurzaMusicVolume(int currentDropsDone)
    {
        if(currentDropsDone >= startPlayingBurzaMusicAfterThisNumberOfDrops)
        {
            float newTargetVolume = currentBurzaVolumeFromBombsDropping + burzaMusicVolumeAmountToAdjustOnBombsDrop();
            DOTween.To(() => currentBurzaVolumeFromBombsDropping, (value) =>
            {
                currentBurzaVolumeFromBombsDropping = value;
                UpdateBurzaVolume();

            }, newTargetVolume, burzaMusicIncreaseDuration);
        }

        float burzaMusicVolumeAmountToAdjustOnBombsDrop() => 1f / (GameplayManager.Instance.player.droppingBombsNumToGoIntoMadness - startPlayingBurzaMusicAfterThisNumberOfDrops);
    }

    protected override void DoOnAudioSource(Action<AudioSource> action)
    {
        action(burzaEndingMusicSource);
    }

    private void UpdateBurzaVolume()
    {
        // volumeFromBombsDropping is a minimum base volume
        // volumeFromGunAngle is an additional volume, which is proportionally added to the base volume, in a way, that every small rotation adds to the base volume and max rotation always means max volume
        float gunAngleActualVolumeIncrease = (1f - currentBurzaVolumeFromBombsDropping) * currentBurzaVolumeFromGunAngle; // volumeFromGunAngle impacts only the volume that is not impacted by volumeFromBombsDropping
        burzaEndingMusicSource.volume = currentBurzaVolumeFromBombsDropping + gunAngleActualVolumeIncrease;
    }

    private float CalculateBurzaVolumeFromGunAngle(float gunAngle)
    {
        float gunAngleBetweenMinus180And180 = MathUtils.RecalculateAngleToBetweenMinus180And180(gunAngle);
        return Mathf.InverseLerp(0f, 180f, Mathf.Abs(gunAngleBetweenMinus180And180));
    }
}
