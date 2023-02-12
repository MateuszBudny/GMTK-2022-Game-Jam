using AetherEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    protected float defaultVolume;

    protected virtual void Awake()
    {
        DoOnAllAudioControllers.AddListener(eventData => DoOnAudioSource(eventData.DoOnAudioSource));
        DoOnAudioSource(source => defaultVolume = source.volume);
    }

    protected abstract void DoOnAudioSource(Action<AudioSource> action);
}
