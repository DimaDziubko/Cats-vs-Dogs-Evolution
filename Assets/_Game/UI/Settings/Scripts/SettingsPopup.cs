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

        [SerializeField] private Slider _soundsSlider;
        [SerializeField] private Slider _musicSlider;

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

            UpdateSlidersView();
            
            _soundsSlider.onValueChanged.AddListener(OnSoundsSliderChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        public void UpdateSlidersView()
        {
            _soundsSlider.value = _audioService.GetSFXVolume();
            _musicSlider.value = _audioService.GetMusicVolume();
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
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }
        
        private void OnSoundsSliderChanged(float value)
        {
            _audioService.SetSFXVolume(value);
        }

        private void OnMusicSliderChanged(float value)
        {
            _audioService.SetMusicVolume(value);
        }
    }
}