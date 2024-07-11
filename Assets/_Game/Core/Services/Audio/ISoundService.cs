﻿using System.Collections.Generic;
using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.Core.Services.Audio
{
    public interface ISoundService
    {
        Transform Transform { get; }
        bool CanPlaySound(SoundData soundData);
        void ReturnToPool(SoundEmitter soundEmitter);
        Dictionary<AudioClip, Queue<SoundEmitter>> FrequentSoundEmitters { get; }
        SoundEmitter Get(AudioClip clip);
        SoundBuilder CreateSound();
        void StopAll();
        void Cleanup();
    }
}