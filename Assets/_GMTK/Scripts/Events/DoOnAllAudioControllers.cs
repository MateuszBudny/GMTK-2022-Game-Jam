using Aether;
using System;
using UnityEngine;

namespace AetherEvents
{
    public class DoOnAllAudioControllers : Event<DoOnAllAudioControllers>
    {
        public  Action<AudioSource> DoOnAudioSource { get; }

        public DoOnAllAudioControllers(Action<AudioSource> doOnAudioSource)
        {
            DoOnAudioSource = doOnAudioSource;
        }
    }
}