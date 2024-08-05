using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.Gameplay._Battle.Scripts
{
    public class BattleAmbienceController
    {
        private AudioClip _ambience;
        
        private readonly IAudioService _audioService;

        public BattleAmbienceController(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public void PlayAmbience()
        {
            if (_audioService != null && _ambience != null)
            {
                _audioService.Play(_ambience);
            }
        }

        public void UpdateAmbience(AudioClip ambience)
        {
            _ambience = ambience;
        }


        public void StopAmbience()
        {
            if (_audioService != null && _ambience != null)
            {
                _audioService.Stop();
            }
        }
    }
}