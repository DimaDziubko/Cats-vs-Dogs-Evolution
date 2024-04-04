using _Game.Core.Services.Audio;
using _Game.Core.Services.PersistentData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Settings.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class SettingsPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _closeButton;

        [SerializeField] private CustomToggle _sfxToggle, _ambienceToggle;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;
        private IPersistentDataService _persistentData;

        public void Construct(
            Camera uICamera,
            IAudioService audioService)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            
            _closeButton.onClick.AddListener(OnCloseBtnClick);

            InitializeUIElements();
            
        }

        private void InitializeUIElements()
        {
            Unsubscribe();
            Subscribe();
            _sfxToggle.Initialize(_audioService.IsOnSFX(), _audioService);
            _ambienceToggle.Initialize(_audioService.IsOnAmbience(), _audioService);
        }


        public async UniTask<bool> AwaitForExit()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void OnCloseBtnClick()
        {
            Unsubscribe();
            _sfxToggle.Cleanup();
            _ambienceToggle.Cleanup();
            
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        private void Subscribe()
        {
            _sfxToggle.ValueChanged += _audioService.SwitchSFX;
            _ambienceToggle.ValueChanged += _audioService.SwitchAmbience;
        }

        private void Unsubscribe()
        {
            _sfxToggle.ValueChanged -= _audioService.SwitchSFX;
            _ambienceToggle.ValueChanged -= _audioService.SwitchAmbience;
        }
    }
}