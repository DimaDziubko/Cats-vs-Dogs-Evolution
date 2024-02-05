using _Game.Core.Communication;
using _Game.Core.GameState;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Settings.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._MainMenu.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _dungeonButton;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _battleButton;
        [SerializeField] private Button _cardsButton;
        [SerializeField] private Button _shopButton;

        private IGameStateMachine _stateMachine;
        private ISettingsPopupProvider _settingsPopupProvider;
        private IShopPopupProvider _shopPopupProvider;
        private IPersistentDataService _persistentData;
        private IWorldCameraService _cameraService;

        private IAudioService _audioService;
        private IUserStateCommunicator _communicator;

        private IStartBattleWindowProvider _startBattleWindowProvider;
        private IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;

        enum ActiveWindow
        {
            None,
            StartBattleWindow,
            UpgradeAndEvolutionWindow,
        }

        private ActiveWindow _activeWindow = ActiveWindow.None;

        private Disposable<StartBattleWindow> _startBattleWindow;
        private Disposable<UpgradeAndEvolutionWindow> _upgradeAndEvolutionWindow;

        public void Construct(
            IWorldCameraService cameraService, 
            IPersistentDataService persistentData,
            IAudioService audioService, 
            IUserStateCommunicator communicator,
            
            IStartBattleWindowProvider startBattleWindowProvider,
            IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider
        )
        {

            _canvas.worldCamera = cameraService.UICameraOverlay;
            
            _startBattleWindowProvider = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;

            _upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
            _battleButton.onClick.AddListener(OnBattleButtonClick);
            
            OnBattleButtonClick();
        }

        private void OnDestroy()
        {
            if (_startBattleWindow != null)
            {
                _startBattleWindow?.Dispose();
            }
            
            _upgradeAndEvolutionWindow?.Dispose();
        }

        private async void OnBattleButtonClick()
        {
            if (_activeWindow != ActiveWindow.StartBattleWindow)
            {
                _startBattleWindow
                    = await _startBattleWindowProvider.Load();
                _activeWindow = ActiveWindow.StartBattleWindow;

                _upgradeAndEvolutionWindow?.Dispose();
            }
        }

        private async void OnUpgradeButtonClick()
        {
            if (_activeWindow != ActiveWindow.UpgradeAndEvolutionWindow)
            {
                _upgradeAndEvolutionWindow 
                    = await _upgradeAndEvolutionWindowProvider.Load();
                _activeWindow = ActiveWindow.UpgradeAndEvolutionWindow;
                _startBattleWindow?.Dispose();
            }
            
        }

        private void PlaySound()
        {
            //TODO Play sound
        }

        private async void OnShopBtnClicked()
        {
            PlaySound();

            var shop = await _shopPopupProvider.Load();
            var isExit = await shop.Value.AwaitForExit();
            if (isExit) shop.Dispose();
        }
        
        private void OnQuitBtnClicked()
        {
            //TODO Check
            //SaveGame();

            PlaySound();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_ANDROID || UNITY_STANDALONE || UNITY_IOS
            Application.Quit();
#endif
        }

        private void SaveGame()
        {
            _communicator.SaveUserState(_persistentData.State);
        }
    }
}
