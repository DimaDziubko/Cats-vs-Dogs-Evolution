using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.Communication;
using _Game.Core.GameState;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Shop.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using UnityEngine;

namespace _Game.UI._MainMenu.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ToggleButton _dungeonButton;
        [SerializeField] private ToggleButton _upgradeButton;
        [SerializeField] private ToggleButton _battleButton;
        [SerializeField] private ToggleButton _cardsButton;
        [SerializeField] private ToggleButton _shopButton;

        private ToggleButton _activeButton;
        
        private ToggleButton ActiveButton
        {
            get => _activeButton;
            set 
            {
                if (_activeButton != null)
                {
                    _activeButton.UnHighlightBtn();
                }
                _activeButton = value;
                _activeButton.HighlightBtn();
            }
        }
        
        private IGameStateMachine _stateMachine;
        private IShopPopupProvider _shopPopupProvider;
        private IWorldCameraService _cameraService;

        private IAudioService _audioService;
        private IUserStateCommunicator _communicator;

        private IStartBattleWindowProvider _startBattleWindowProvider;
        private IUpgradeAndEvolutionWindowProvider _upgradeAndEvolutionWindowProvider;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private ITutorialManager _tutorialManager;


        //TODO Change step by step
        private bool IsDungeonUnlocked => false;
        private bool IsUpgradesUnlocked => _featureUnlockSystem.IsFeatureUnlocked(_upgradeButton);
        private bool IsBattleUnlocked => true;
        private bool IsCardsUnlocked => false;
        private bool IsShopUnlocked => false;


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
            IUpgradeAndEvolutionWindowProvider upgradeAndEvolutionWindowProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager
        )
        {

            _audioService = audioService;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            
            _startBattleWindowProvider = startBattleWindowProvider;
            _upgradeAndEvolutionWindowProvider = upgradeAndEvolutionWindowProvider;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
        }

        public void Show()
        {
            _tutorialManager.Register(_upgradeButton);
            
            _dungeonButton.Initialize(IsDungeonUnlocked, OnDungeonClick, PlayButtonSound);
            _upgradeButton.Initialize(IsUpgradesUnlocked, OnUpgradeButtonClick, PlayButtonSound);
            _battleButton.Initialize(IsBattleUnlocked, OnBattleButtonClick, PlayButtonSound);
            _cardsButton.Initialize(IsCardsUnlocked, OnCardsButtonClick, PlayButtonSound);
            _shopButton.Initialize(IsCardsUnlocked, OnShopButtonClick, PlayButtonSound);


            OnBattleButtonClick(_battleButton);
        }

        private void OnShopButtonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        private void OnCardsButtonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        private void OnDungeonClick(ToggleButton obj)
        {
            //TODO Implement later
        }

        public void Hide()
        {
            //TODO Delete later
            Debug.Log("MainMenu HIDE");
            
            _upgradeButton.Cleanup();
            _battleButton.Cleanup();
            _dungeonButton.Cleanup();
            _cardsButton.Cleanup();
            _shopButton.Cleanup();
            
            if (_startBattleWindow != null)
            {
                //TODO Delete later
                Debug.Log("StartBattleWindow HIDE");
                
                _startBattleWindow.Value.Hide();
                _startBattleWindow?.Dispose();
            }

            if (_upgradeAndEvolutionWindow != null)
            {
                //TODO Delete later
                Debug.Log("UpgradeAndEvolution window HIDE");
                
                _upgradeAndEvolutionWindow.Value.Hide();
                _upgradeAndEvolutionWindow?.Dispose();
            }
            
            _tutorialManager.UnRegister(_upgradeButton);
        }

        private async void OnBattleButtonClick(ToggleButton button)
        {
            if (_activeWindow != ActiveWindow.StartBattleWindow)
            {
                _startBattleWindow
                    = await _startBattleWindowProvider.Load();
                _activeWindow = ActiveWindow.StartBattleWindow;
                _startBattleWindow.Value.Show();

                ActiveButton = button;
                
                if (_upgradeAndEvolutionWindow != null)
                {
                    _upgradeAndEvolutionWindow.Value.Hide();
                    _upgradeAndEvolutionWindow?.Dispose();
                    _upgradeAndEvolutionWindow = null;
                }
            }
        }

        private async void OnUpgradeButtonClick(ToggleButton button)
        {
            if (_activeWindow != ActiveWindow.UpgradeAndEvolutionWindow)
            {
                _upgradeAndEvolutionWindow 
                    = await _upgradeAndEvolutionWindowProvider.Load();
                
                _upgradeAndEvolutionWindow.Value.Show();
                _activeWindow = ActiveWindow.UpgradeAndEvolutionWindow;

                ActiveButton = button;

                if (_startBattleWindow != null)
                {
                    _startBattleWindow.Value.Hide();
                    _startBattleWindow?.Dispose();
                    _startBattleWindow = null;
                }
            }
            
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }

        private async void OnShopBtnClicked()
        {
            var shop = await _shopPopupProvider.Load();
            var isExit = await shop.Value.AwaitForExit();
            if (isExit) shop.Dispose();
        }
    }
}
