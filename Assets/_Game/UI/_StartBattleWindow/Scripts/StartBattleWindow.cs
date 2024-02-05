using _Game.Core.Communication;
using _Game.Core.Configs.Controllers;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay.GamePlayManager;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StartBattleWindow.Scripts
{
    public class StartBattleWindow : MonoBehaviour, IUIWindow
    {
        public string Name => $"Battle {_battleState.CurrentBattleIndex + 1}";

        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _startBattleButton;
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _previousBattleButton;
        
        
        private IAudioService _audioService;
        private IPersistentDataService _persistentData;
        private IUserStateCommunicator _communicator;
        private IBeginGameManager _beginGameManager;
        private IGameConfigController _gameConfigController;
        private IBattleStateService _battleState;
        private IHeader _header;

        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        

        public void Construct(
            Camera uICamera, 
            IPersistentDataService persistentData, 
            IAudioService audioService,
            IUserStateCommunicator communicator,
            
            IHeader header,
            IBeginGameManager beginGameManager,
            
            IGameConfigController gameConfigController,

            IBattleStateService battleState)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _persistentData = persistentData;
            _communicator = communicator;

            _beginGameManager = beginGameManager;

            _gameConfigController = gameConfigController;

            _header = header;

            _battleState = battleState;
            
            UpdateNavigationButtons();
            
            
            _header.ShowWindowName(Name);
            
            _previousBattleButton.onClick.AddListener(OnPreviousBattleButtonClick);
            _nextBattleButton.onClick.AddListener(OnNextBattleButtonClick);
            _startBattleButton.onClick.AddListener(OnStartButtonClick);
        }
        
        private void UpdateNavigationButtons()
        {
            _previousBattleButton.gameObject.SetActive(_battleState.IsFirstBattle());
            _nextBattleButton.gameObject.SetActive(!_battleState.IsLastBattle());
            _previousBattleButton.interactable = _battleState.CanMoveToPreviousBattle();
            _nextBattleButton.interactable = _battleState.CanMoveToNextBattle();
            _header.ShowWindowName(Name);
        }
        
        private void OnStartButtonClick() => _beginGameManager.TriggerBeginGame();

        private void OnPreviousBattleButtonClick()
        {
            _battleState.MoveToPreviousBattle();
            UpdateNavigationButtons();
        }

        private void OnNextBattleButtonClick()
        {
            _battleState.MoveToNextBattle();
            UpdateNavigationButtons();
        }

        private void OnDisable()
        {
            _startBattleButton.onClick.RemoveAllListeners();
            _nextBattleButton.onClick.RemoveAllListeners();
            _previousBattleButton.onClick.RemoveAllListeners();
        }
    }
}
