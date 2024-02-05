using _Game.Audio.Scripts;
using _Game.Core.Prefabs;
using UnityEngine;
using UnityEngine.Audio;

namespace _Game.Core.Services.Audio
{
    public class AudioService : IAudioService
    {
        private const float MIN_VOLUME_DB = -80f;
        private const string SFX_VOLUME = "SFX";
        private const string MUSIC_VOLUME = "MUSIC";
        
        private readonly AudioMixer _mixer;
        
        private readonly AudioSource[] _sFXSources;
        private readonly AudioSource _musicSource;
        
        private int _freeSource;
        
        private readonly SoundsHolder _soundsHolder;
        
        private int _tossSoundIndex;

        public AudioService(
            AudioMixer mixer,
            SFXSourcesHolder sfxSourcesHolder,
            AudioSource musicSource,
            SoundsHolder soundsHolder)
        {
            _mixer = mixer;
            _sFXSources = sfxSourcesHolder.SfxSources;
            _musicSource = musicSource;
            _soundsHolder = soundsHolder;
        }
        
        public void PlayOneShot(AudioClip audioClip)
        {
            if (_freeSource >= _sFXSources.Length - 1)
            {
                _freeSource = 0;
            }
            
            _sFXSources[_freeSource].PlayOneShot(audioClip);

            _freeSource += 1;
        }

        public void Play(AudioClip musicClip)
        {
            _musicSource.clip = musicClip;
            _musicSource.Play();
        }

        public void PlayButtonSound()
        {
            var buttonSound = _soundsHolder.ButtonSound;
            PlayOneShot(buttonSound);
        }
        
        public void SetSFXVolume(in float volume)
        {
            float correctVolume = MIN_VOLUME_DB - (MIN_VOLUME_DB * volume);
            _mixer.SetFloat(SFX_VOLUME, correctVolume);
        }

        public void SetMusicVolume(in float volume)
        {
            float correctVolume = MIN_VOLUME_DB - (MIN_VOLUME_DB * volume);
            _mixer.SetFloat(MUSIC_VOLUME, correctVolume);
        }
        
        public float GetSFXVolume()
        {
            _mixer.GetFloat(SFX_VOLUME, out var volume);
            
            return (volume - MIN_VOLUME_DB) / -MIN_VOLUME_DB;
        }

        public float GetMusicVolume()
        {
            _mixer.GetFloat(MUSIC_VOLUME, out var volume);
            
            return (volume - MIN_VOLUME_DB) / -MIN_VOLUME_DB;
        }
    }
}