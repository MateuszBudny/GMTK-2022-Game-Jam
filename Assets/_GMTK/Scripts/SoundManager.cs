using AetherEvents;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleBehaviour<SoundManager>
{
    public AudioSource ambientSource;
    public List<AudioSource> soundsSources;
    [SerializeField]
    private float audioFadeInDuration = 3f;

    [Header("Rare ambient")]
    public AudioSource rareAmbientSource;
    public float firstPlayDelay = 60f;
    public float anotherPlayDelayAfterFirst = 300f;

    private float rareAmbientPlayedTimestamp = -1f;
    private bool rareAmbientPlayedAlready = false;

    [Header("Burza ending music")]
    [SerializeField]
    private AudioSource burzaEndingMusicSource;
    [SerializeField]
    private int startPlayingBurzaMusicAfterThisNumberOfDrops = 3;
    [SerializeField]
    private float burzaMusicIncreaseDuration = 3f;

    [Header("Other sounds")]
    [SerializeField]
    private List<AudioRecord> sounds;
    [SerializeField]
    private List<AudioRecord> satanTalking;

    protected override void Awake()
    {
        base.Awake();
        rareAmbientPlayedTimestamp = Time.time;
        BombsDropped.AddListener(OnBombsDrop);
    }

    private void Start()
    {
        AudioFadeIn();
    }

    private void Update()
    {
        if(GameplayManager.Instance.State != GameState.GameOver)
        {
            if(!rareAmbientPlayedAlready)
            {
                if(Time.time - rareAmbientPlayedTimestamp > firstPlayDelay)
                {
                    PlayRareAmbient();
                }
            }
            else
            {
                if(Time.time - rareAmbientPlayedTimestamp > anotherPlayDelayAfterFirst)
                {
                    PlayRareAmbient();
                }
            }
        }
    }

    public void Play(Audio audioEnum)
    {
        AudioRecord audioRecordToPlay = sounds.Find(sound => sound.audioEnum == audioEnum);
        soundsSources.GetRandomElement().PlayOneShot(audioRecordToPlay.audioClip, audioRecordToPlay.volume);
    }

    public void PlaySatanTalking()
    {
        AudioRecord satanTalkingChosen = satanTalking.GetRandomElement();
        soundsSources.GetRandomElement().PlayOneShot(satanTalkingChosen.audioClip, satanTalkingChosen.volume);
    }
    
    private void PlayRareAmbient()
    {
        rareAmbientSource.Play();
        rareAmbientPlayedTimestamp = Time.time;
    }

    private void OnBombsDrop(BombsDropped eventData)
    {
        TryToAdjustBurzaMusicVolume(eventData.WhichDroppingIsThis);
    }

    private void TryToAdjustBurzaMusicVolume(int currentDropsDone)
    {
        if(currentDropsDone == startPlayingBurzaMusicAfterThisNumberOfDrops)
        {
            burzaEndingMusicSource.volume = 0f;
            burzaEndingMusicSource.Play();
        }

        if(currentDropsDone >= startPlayingBurzaMusicAfterThisNumberOfDrops)
        {
            float newTargetVolume = burzaEndingMusicSource.volume + burzaMusicVolumeAmountToAdjustOnBombsDrop();
            AdjustVolumeByTweening(burzaEndingMusicSource, newTargetVolume, burzaMusicIncreaseDuration);
        }

        float burzaMusicVolumeAmountToAdjustOnBombsDrop() => 1f / (GameplayManager.Instance.player.droppingBombsNumToGoIntoMadness - startPlayingBurzaMusicAfterThisNumberOfDrops + 1);
    }

    private void AudioFadeIn()
    {
        DoOnAllSoundsSources(source =>
        {
            source.volume = 0f;
            AdjustVolumeByTweening(source, 1f, audioFadeInDuration);
        });
    }

    public void StopAllMusicAndSounds() => DoOnAllSoundsSources(source => source.Stop());

    private void DoOnAllSoundsSources(Action<AudioSource> action)
    {
        action(ambientSource);
        action(rareAmbientSource);
        action(burzaEndingMusicSource);
        soundsSources.ForEach(source => action(source));
    }

    private void AdjustVolumeByTweening(AudioSource source, float targetVolume, float duration)
    {
        DOTween.To(() => source.volume, value => source.volume = value, targetVolume, duration).SetEase(Ease.Linear);
    }
}

public enum Audio
{
    SatanSetFaceShorter,
    SatanSetFaceLonger,
    DiceBounce,
    NoAmmo,
    Gunshot,
    CrankHullOpening,
    BombsFalling,
}

[Serializable]
public class AudioRecord
{
    public AudioClip audioClip;
    public Audio audioEnum;
    // TODO:
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;
    public float volume = 1f;
}