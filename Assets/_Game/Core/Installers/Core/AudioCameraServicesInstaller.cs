using _Game.Audio.Scripts;
using _Game.Core.Prefabs;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class AudioCameraServicesInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uICameraOverlay;
        [SerializeField] private SFXSourcesHolder _sfxSourcesHolder;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SoundsHolder _soundsHolder;

        public override void InstallBindings()
        {
            BindCameraService();
            BindAudioService();
        }

        private void BindCameraService()
        {
            var cameraService = new WorldCameraService(_mainCamera, _uICameraOverlay);
            
            cameraService.DisableCamera();
            
            Container
                .Bind<IWorldCameraService>()
                .FromInstance(cameraService)
                .AsSingle();
        }

        private void BindAudioService()
        {
            var audioService = new AudioService(
                _audioMixer, 
                _sfxSourcesHolder,
                _musicSource,
                _soundsHolder);
            
            
            Container
                .Bind<IAudioService>()
                .FromInstance(audioService)
                .AsSingle(); 
        }
    }
}