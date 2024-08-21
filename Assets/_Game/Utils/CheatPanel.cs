using _Game.Core._GameMode;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.UI._Currencies;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.UserState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils
{
    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timelineLabel;
        [SerializeField] private TMP_Text _ageLabel;
        [SerializeField] private TMP_Text _battleLabel;

        [SerializeField] private Button _nextTimelineBtn;
        [SerializeField] private Button _previousTimelineBtn;

        [SerializeField] private Button _nextAgeBtn;
        [SerializeField] private Button _previousAgeBtn;
        
        [SerializeField] private Button _nextBattleBtn;
        [SerializeField] private Button _previousBattleBtn;

        [SerializeField] private Button _cheatButton;
        
        private ITimelineNavigator _timelineNavigator;
        private IAgeNavigator _ageNavigator;
        private IBattleNavigator _battleNavigator;
        private IUserContainer _userContainer;
        private ITimelineConfigRepository _timelineConfigRepository;
        private IAudioService _audioService;
        
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public void Construct(
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            IBattleNavigator battleNavigator,
            IUserContainer userContainer,
            ITimelineConfigRepository timelineConfigRepository,
            IAudioService audioService)
        {
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _battleNavigator = battleNavigator;
            _userContainer = userContainer;
            _timelineConfigRepository = timelineConfigRepository;
            _audioService = audioService;
            
            gameObject.SetActive(GameMode.I.IsCheatEnabled);
        }

        public void Init()
        {
            Unsubscribe();
            Subscribe();
            UpdateNavigations();
        }

        public void Cleanup()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _battleNavigator.BattleChanged += OnBattleChanged;
            
            _previousBattleBtn.onClick.AddListener(OnPreviousBattleBtnClicked);
            _nextBattleBtn.onClick.AddListener(OnNextBattleBtnClicked);

            _previousTimelineBtn.onClick.AddListener(OnPreviousTimelineBtnClicked);
            _nextTimelineBtn.onClick.AddListener(OnNextTimelineBtnClicked);

            _previousAgeBtn.onClick.AddListener(OnPreviousAgeBtnClicked);
            _nextAgeBtn.onClick.AddListener(OnNextAgeBtnClicked);

            _cheatButton.onClick.AddListener(OnCheatButtonClick);
        }

        private void Unsubscribe()
        {
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _battleNavigator.BattleChanged -= OnBattleChanged;
            
            _previousBattleBtn.onClick.RemoveAllListeners();
            _nextBattleBtn.onClick.RemoveAllListeners();
            
            _previousTimelineBtn.onClick.RemoveAllListeners();
            _nextTimelineBtn.onClick.RemoveAllListeners();
            
            _previousAgeBtn.onClick.RemoveAllListeners();
            _nextAgeBtn.onClick.RemoveAllListeners();
            
            _cheatButton.onClick.RemoveAllListeners();
        }

        private void OnCheatButtonClick()
        {
            PlayButtonSound();
            _userContainer.CurrenciesHandler.AddCoins(10_000_000, CurrenciesSource.None);
        }

        private void OnTimelineChanged()
        {
            UpdateNavigations();
        }

        private void OnAgeChanged()
        {
            UpdateNavigations();
        }

        private void OnBattleChanged() => UpdateNavigations();

        private void UpdateNavigations()
        {
            UpdateTimelineNavigation();
            UpdateAgeNavigation();
            UpdateBattleNavigation();
        }

        private void UpdateTimelineNavigation()
        {
            _timelineLabel.text = $"Timeline {TimelineState.TimelineId + 1}";
            _previousTimelineBtn.interactable = TimelineState.TimelineId > 0;
            _nextTimelineBtn.interactable = true;
        }

        private void UpdateAgeNavigation()
        {
            _ageLabel.text = $"Age {TimelineState.AgeId + 1}";
            _previousAgeBtn.interactable = TimelineState.AgeId > 0;
            _nextAgeBtn.interactable = TimelineState.AgeId < _timelineConfigRepository.LastAge();
        }

        private void UpdateBattleNavigation()
        {
            _battleLabel.text = $"Battle {_battleNavigator.CurrentBattle + 1}";
            _previousBattleBtn.interactable = _battleNavigator.CanMoveToPreviousBattle();
            _nextBattleBtn.interactable = _battleNavigator.CurrentBattle < _timelineConfigRepository.LastBattle();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();

        private void OnPreviousBattleBtnClicked()
        {
            PlayButtonSound();
            _battleNavigator.MoveToPreviousBattle();

        }
        
        private void OnNextBattleBtnClicked()
        {
            PlayButtonSound();
            _battleNavigator.ForceMoveToNextBattle();
        }
        
        private void OnPreviousAgeBtnClicked()
        {
            PlayButtonSound();
            DisableButtons();
            _userContainer.TimelineStateHandler.OpenNewAge(false);
        }
        
        private void OnNextAgeBtnClicked()
        {
            PlayButtonSound();
            DisableButtons();
            _userContainer.TimelineStateHandler.OpenNewAge();
        }
        
        private void OnPreviousTimelineBtnClicked()
        {
            PlayButtonSound();
            DisableButtons();
            _userContainer.TimelineStateHandler.OpenNewTimeline(false);
        }
        
        private void OnNextTimelineBtnClicked()
        {
            PlayButtonSound();
            DisableButtons();
            _userContainer.TimelineStateHandler.OpenNewTimeline();
        }

        private void DisableButtons()
        {
            _previousTimelineBtn.interactable = false;
            _nextTimelineBtn.interactable = false;
            
            _previousAgeBtn.interactable = false;
            _nextAgeBtn.interactable = false;
            
            _previousBattleBtn.interactable = false;
            _nextBattleBtn.interactable = false;
        }
    }
}
