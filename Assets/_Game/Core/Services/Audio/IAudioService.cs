﻿using UnityEngine;

namespace _Game.Core.Services.Audio
{
    public interface IAudioService 
    {
        void PlayOneShot(AudioClip audioClip);
        void Play(AudioClip musicClip);
        void SetSFXVolume(in float value);
        void SetMusicVolume(in float value);
        float GetSFXVolume();
        float GetMusicVolume();
        void PlayButtonSound();
        void PlayCoinDropSound();
        void PlayCoinCollectSound();
        void Stop();
        void PlayBaseDestructionSFX();
    }
}