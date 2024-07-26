using _Game.Core.Services.UserContainer;
using _Game.UI._RaceSelectionWindow.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI.Settings.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class SettingsPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _closeButton;

        [SerializeField] private CustomToggle _sfxToggle, _ambienceToggle;
        [SerializeField] private Button _changeRaceBtn;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;
        private IUserContainer _userContainer;
        private IRaceSelectionWindowProvider _raceSelectionWindowProvider;

        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IRaceSelectionWindowProvider raceSelectionWindowProvider)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _raceSelectionWindowProvider = raceSelectionWindowProvider;
            
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
            _changeRaceBtn.onClick.AddListener(OnChangeRaceBtnClicked);
        }

        private void Unsubscribe()
        {
            _sfxToggle.ValueChanged -= _audioService.SwitchSFX;
            _ambienceToggle.ValueChanged -= _audioService.SwitchAmbience;
            _changeRaceBtn.onClick.RemoveAllListeners();
        }

        private async void OnChangeRaceBtnClicked()
        {
            Disposable<RaceSelectionWindow> factionSelectionWindow = await _raceSelectionWindowProvider.Load();
            var result = await factionSelectionWindow.Value.AwaitForDecision();
            if(result)
                factionSelectionWindow.Dispose();
        }
    }
}