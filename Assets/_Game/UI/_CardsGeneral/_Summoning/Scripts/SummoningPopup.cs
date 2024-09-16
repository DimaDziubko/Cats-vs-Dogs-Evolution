using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils._Static;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class SummoningPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Button _previousLevelButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button[] _cancelButtons;
        
        [SerializeField] private CardSummoningView[] _cardSummoningViewes;
        
        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;
        private IUserContainer _userContainer;
        private CardsSummoningModel _cardsSummoningModel;

        private int _levelToShow;

        private int LevelToShow
        {
            get => _levelToShow;
            set
            {
                _levelToShow = value;
                UpdateViews();
            }
        }
        
        public void Construct(
            Camera cameraServiceUICameraOverlay, 
            IAudioService audioService,
            CardsSummoningModel cardsSummoningModel)
        {
            _canvas.worldCamera = cameraServiceUICameraOverlay;
            _audioService = audioService;
            _cardsSummoningModel = cardsSummoningModel;
            Init();
        }

        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
            
            _previousLevelButton.onClick.AddListener(OnPreviousButtonClicked);
            _nextLevelButton.onClick.AddListener(OnNextButtonClicked);

            _canvas.enabled = false;
        }

        public async UniTask<bool> AwaitForExit()
        {
            LevelToShow = _cardsSummoningModel.CurrentLevel;
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void UpdateViews()
        {
            _previousLevelButton.interactable = LevelToShow > _cardsSummoningModel.MinSummoningLevel;
            _nextLevelButton.interactable = LevelToShow < _cardsSummoningModel.MaxSummoningLevel;
            
            _levelLabel.text = $"Level {LevelToShow}";
            
            var cardsSummoning = _cardsSummoningModel.AllCardSummonings[LevelToShow];
            
            foreach (var view in _cardSummoningViewes)
            {
                var model = new CardSummoningModel()
                {
                    Color = CardColorMapper.GetColorForType(view.CardType),
                    SummoningValue = cardsSummoning.ForType(view.CardType)
                };
                
                view.UpdateView(model);
            }
        }

        public void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            
            _previousLevelButton.onClick.RemoveListener(OnPreviousButtonClicked);
            _nextLevelButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        private void OnNextButtonClicked()
        {
            PlayButtonSound();
            if(LevelToShow < _cardsSummoningModel.MaxSummoningLevel)
                LevelToShow++;
        }

        private void OnPreviousButtonClicked()
        {
            PlayButtonSound();
            if(LevelToShow > _cardsSummoningModel.MinSummoningLevel)
                LevelToShow--;
        }

        private void OnCancelled()
        {
            PlayButtonSound();
            _taskCompletion.TrySetResult(false);
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
    }
}
