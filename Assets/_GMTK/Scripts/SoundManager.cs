using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingleBehaviour<SoundManager>
{
    public AudioSource ambientSource;
    public List<AudioSource> soundsSources;

    [Header("Rare ambient")]
    public AudioSource rareAmbientSource;
    public float firstPlayDelay = 60f;
    public float anotherPlayDelayAfterFirst = 300f;

    private float rareAmbientPlayedTimestamp = -1f;
    private bool rareAmbientPlayedAlready = false;

    [Header("Other sounds")]
    [SerializeField]
    private List<AudioRecord> sounds;
    [SerializeField]
    private List<AudioRecord> satanTalking;

    protected override void Awake()
    {
        base.Awake();
        rareAmbientPlayedTimestamp = Time.time;
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

    public void StopAllMusicAndSounds()
    {
        ambientSource.Stop();
        rareAmbientSource.Stop();
        soundsSources.ForEach(source => source.Stop());
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