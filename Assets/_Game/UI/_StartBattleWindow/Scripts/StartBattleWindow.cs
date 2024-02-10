using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Battle;
using _Game.Gameplay.Battle.Scripts;
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

        private IBeginGameManager _beginGameManager;
        private IBattleStateService _battleState;
        private IHeader _header;
        private IMyLogger _logger;


        public void Construct(
            Camera uICamera,
            IAudioService audioService,

            IHeader header,
            IBeginGameManager beginGameManager,

            IBattleStateService battleState,
            IMyLogger logger)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;

            _beginGameManager = beginGameManager;

            _header = header;

            _battleState = battleState;
            _logger = logger;
            
            UpdateNavigationButtons(_battleState.NavigationModel);
            
            _header.ShowWindowName(Name);

            _battleState.BattleChange += UpdateNavigationButtons;
            _battleState.BattlePrepared += OnBattlePrepared;

            _previousBattleButton.onClick.AddListener(OnPreviousBattleButtonClick);
            _nextBattleButton.onClick.AddListener(OnNextBattleButtonClick);
            _startBattleButton.onClick.AddListener(OnStartButtonClick);
        }

        private void OnBattlePrepared(BattleData obj)
        {
            _logger.Log("OnBattle prepared (UI)");
            _startBattleButton.interactable = true;
        }

        private void UpdateNavigationButtons(BattleNavigationModel model)
        {
            _previousBattleButton.gameObject.SetActive(model.IsFirstBattle);
            _nextBattleButton.gameObject.SetActive(!model.IsLastBattle);
            _previousBattleButton.interactable = model.CanMoveToPreviousBattle;
            _nextBattleButton.interactable = model.CanMoveToNextBattle;
            
            _startBattleButton.interactable = _battleState.IsBattlePrepared;
            _header.ShowWindowName(Name);
        }
        
        private void OnStartButtonClick() => _beginGameManager.TriggerBeginGame();

        private void OnPreviousBattleButtonClick() => _battleState.MoveToPreviousBattle();

        private void OnNextBattleButtonClick() => _battleState.MoveToNextBattle();

        private void OnDisable()
        {
            _startBattleButton.onClick.RemoveAllListeners();
            _nextBattleButton.onClick.RemoveAllListeners();
            _previousBattleButton.onClick.RemoveAllListeners();

            _battleState.BattlePrepared -= OnBattlePrepared;
            _battleState.BattleChange -= UpdateNavigationButtons;
        }
    }
}
