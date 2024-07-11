using _Game.Core.Services.Audio;
using Assets._Game.Audio.Scripts;
using Assets._Game.Core.Prefabs;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Assets._Game.Core.Installers.Core
{
    public class AudioCameraServicesInstaller : MonoInstaller
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uICameraOverlay;
        [SerializeField] private SFXSourcesHolder _sfxSourcesHolder;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private SoundsHolder _soundsHolder;
        [SerializeField] private SoundService _soundService;

        public override void InstallBindings()
        {
            BindCameraService();
            BindAudioService();
            BindSoundService();
        }

        private void BindCameraService()
        {
            var cameraService = new WorldCameraService(_mainCamera, _uICameraOverlay);
            
            cameraService.DisableMainCamera();
            
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

        private void BindSoundService()
        {
            //_soundService.Init();
            Container
                .Bind<ISoundService>()
                .FromInstance(_soundService)
                .AsSingle()
                .NonLazy(); 
        }
    }
}