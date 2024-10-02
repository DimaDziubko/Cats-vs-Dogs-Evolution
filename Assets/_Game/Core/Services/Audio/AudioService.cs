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

        private const string SFX_PREFS_KEY = "SFXOn";
        private const string AMBIENCE_PREFS_KEY = "AmbienceOn";
        
        private readonly AudioMixer _mixer;

        private readonly AudioSource[] _sFXSources;
        private readonly AudioSource _musicSource;

        private int _freeSource;

        private readonly SoundsHolder _soundsHolder;
        
        private bool _isSFXOn = true;
        private bool _isAmbienceOn = true;
        private float _sfxVolumeBeforeMute;
        private float _musicVolumeBeforeMute;
        

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
            
            _isSFXOn = PlayerPrefs.GetInt(SFX_PREFS_KEY, 1) == 1;
            _isAmbienceOn = PlayerPrefs.GetInt(AMBIENCE_PREFS_KEY, 1) == 1;

            if (!_isSFXOn)
            {
                _mixer.SetFloat(SFX_VOLUME, MIN_VOLUME_DB);
            }

            if (!_isAmbienceOn)
            {
                _mixer.SetFloat(MUSIC_VOLUME, MIN_VOLUME_DB);
            }
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
        
        public void PlayStartBattleSound()
        {
            var startBattleSound = _soundsHolder.StartBattrleSound;
            PlayOneShot(startBattleSound);
        }
        
        public void PlayVictorySound()
        {
            var victorySound = _soundsHolder.VictorySound;
            PlayOneShot(victorySound);
        }

        public void PlayBaseDestructionSFX()
        {
            var baseDestructionSFX = _soundsHolder.BaseDestructionSFX;
            PlayOneShot(baseDestructionSFX);
        }
        
        public void PlayCoinCollectSound()
        {
            var index = UnityEngine.Random.Range(0, _soundsHolder.CoinCollectSounds.Length);
            var coinCollectSound = _soundsHolder.CoinCollectSounds[index];
            PlayOneShot(coinCollectSound);
        }

        public void PlayCoinAppearanceSFX()
        {
            var sfx = _soundsHolder.CoinAppearanceSound;
            if (sfx != null)
            {
                PlayOneShot(sfx);
            }
        }

        public void PlayFillingWalletSFX()
        {
            var sfx = _soundsHolder.FillingWalletSound;
            if (sfx != null)
            {
                PlayOneShot(sfx);
            }
        }
        
        public void Stop() => _musicSource.Stop();

        public void PlayCoinDropSound()
        {
            var dropSound = _soundsHolder.DropCoinSound;
            if (dropSound != null)
            {
                PlayOneShot(dropSound);
            }
        }
        
        public void PlayUpgradeSound()
        {
            var upgradeSound = _soundsHolder.UpgradeSound;
            if (upgradeSound != null)
            {
                PlayOneShot(upgradeSound);
            }
        }

        public bool IsOnSFX() => _isSFXOn;

        public bool IsOnAmbience() => _isAmbienceOn;

        public void SwitchSFX(bool isOn)
        {
            _isSFXOn = isOn;
            
            PlayerPrefs.SetInt(SFX_PREFS_KEY, _isSFXOn ? 1 : 0);
            PlayerPrefs.Save();
            
            if (isOn)
            {
                SetSFXVolume(_sfxVolumeBeforeMute);
            }
            else
            {
                _sfxVolumeBeforeMute = GetSFXVolume();
                _mixer.SetFloat(SFX_VOLUME, MIN_VOLUME_DB);
            } 
        }

        public void SwitchAmbience(bool isOn)
        {
            _isAmbienceOn = isOn;
            
            PlayerPrefs.SetInt(AMBIENCE_PREFS_KEY, _isAmbienceOn ? 1 : 0);
            PlayerPrefs.Save();
            
            if (isOn)
            {
                SetMusicVolume(_musicVolumeBeforeMute);
            }
            else
            {
                _musicVolumeBeforeMute = GetMusicVolume();
                _mixer.SetFloat(MUSIC_VOLUME, MIN_VOLUME_DB);
            }
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