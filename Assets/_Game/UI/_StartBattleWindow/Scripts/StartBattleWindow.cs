using System;
using _Game.Core._Logger;
using _Game.Core.Navigation;
using _Game.Core.Navigation.Battle;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Settings.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public class StartBattleWindow : MonoBehaviour, IUIWindow
    {
        public event Action Opened;
        public Window Window => Window.Battle;

        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _startBattleButton;
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _previousBattleButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private Button _cheatBtn;
        
        private IAudioService _audioService;

        private IBattleLaunchManager _battleLaunchManager;
        private IHeader _header;
        private IMyLogger _logger;
        private IUserContainer _persistentData;
        private ISettingsPopupProvider _settingsPopupProvider;
        private IBattleNavigator _battleNavigator;


        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IHeader header,
            IBattleLaunchManager battleLaunchManager,
            IMyLogger logger,
            IUserContainer persistentData,
            ISettingsPopupProvider settingsPopupProvider,
            IBattleNavigator battleNavigator)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;

            _battleLaunchManager = battleLaunchManager;

            _header = header;
            _logger = logger;

            _persistentData = persistentData;
            _settingsPopupProvider = settingsPopupProvider;
            _battleNavigator = battleNavigator;
        }

        public void Show()
        {
            ShowName();

            Unsubscribe();
            Subscribe();

            _canvas.enabled = true;
            
            Opened?.Invoke();
        }

        private void ShowName()
        {
            var fullName = $"{Window} {_battleNavigator.CurrentBattle + 1}";
            _header.ShowWindowName(fullName);
        }

        public void Hide()
        {
            Unsubscribe();

            _canvas.enabled = false;
        }

        private void Subscribe()
        {
            _battleNavigator.NavigationUpdated += UpdateNavigationButtons;
            Opened += _battleNavigator.OnStartBattleWindowOpened;
            
            _previousBattleButton.onClick.AddListener(OnPreviousBattleButtonClick);
            _nextBattleButton.onClick.AddListener(OnNextBattleButtonClick);
            _startBattleButton.onClick.AddListener(OnStartButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsBtnClick);
            
            _cheatBtn.onClick.AddListener(OnCheatBtnClicked);
            
            _quitButton.onClick.AddListener(OnQuitBtnClick);
        }

        private void Unsubscribe()
        {
            _startBattleButton.onClick.RemoveAllListeners();
            _nextBattleButton.onClick.RemoveAllListeners();
            _previousBattleButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
            
            _cheatBtn.onClick.RemoveAllListeners();

            _battleNavigator.NavigationUpdated -= UpdateNavigationButtons;
            Opened -= _battleNavigator.OnStartBattleWindowOpened;
        }

        private void OnCheatBtnClicked()
        {
            PlayButtonSound();
            _persistentData.AddCoins(10_000_000);
        }

        private void OnQuitBtnClick()
        {
            PlayButtonSound();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_ANDROID || UNITY_STANDALONE || UNITY_IOS
            Application.Quit();
#endif
        }

        private async void OnSettingsBtnClick()
        {
            PlayButtonSound();
            var popup = await _settingsPopupProvider.Load();
            await popup.Value.AwaitForExit();
            popup.Dispose();
        }

        private void UpdateNavigationButtons(BattleNavigationModel model)
        {
            _previousBattleButton.gameObject.SetActive(model.CanMoveToPreviousBattle);
            _nextBattleButton.gameObject.SetActive(model.CanMoveToNextBattle);
            _startBattleButton.interactable = model.IsPrepared;
            
            ShowName();
        }

        private void OnStartButtonClick()
        {
            PlayButtonSound();
            _battleLaunchManager.TriggerLaunchBattle();
        }

        private void OnPreviousBattleButtonClick()
        {
            PlayButtonSound();
            _battleNavigator.MoveToPreviousBattle();
        }

        private void OnNextBattleButtonClick()
        {
            PlayButtonSound();
            _battleNavigator.MoveToNextBattle();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}
