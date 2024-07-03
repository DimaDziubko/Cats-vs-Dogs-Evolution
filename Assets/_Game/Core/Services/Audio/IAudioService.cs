using UnityEngine;

namespace Assets._Game.Core.Services.Audio
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

        public void PlayCoinAppearanceSFX();
        public void PlayFillingWalletSFX();

        bool IsOnSFX();
        bool IsOnAmbience();
        void SwitchSFX(bool isOn);
        void SwitchAmbience(bool isOn);
        void PlayVictorySound();
        void PlayStartBattleSound();
    }
}