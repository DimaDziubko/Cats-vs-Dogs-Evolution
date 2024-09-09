using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class StatsPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
    
        [SerializeField] private TMP_Text _currentWarriorLabel;
        [SerializeField] private Button _previousStatsButton;
        [SerializeField] private Button _nextStatsButton;

        [SerializeField] private UnitInfoItem _playerUnitInfoItem;
        [SerializeField] private UnitInfoItem _enemyUnitInfoItem;
    
        [SerializeField] private Button[] _cancelButtons;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private IAudioService _audioService;
        private IStatsPopupPresenter _statsPopupPresenter;

        private UnitType _typeToShow;
    
        private UnitType TypeToShow
        {
            get => _typeToShow;
            set
            {
                if (value > UnitType.Heavy)
                {
                    _typeToShow = UnitType.Heavy;
                }
                else if(value < UnitType.Light)
                {
                    _typeToShow = UnitType.Light;
                }
                else
                {
                    _typeToShow = value;
                }
               
                UpdateView(_typeToShow);
            }
        }
    
        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IStatsPopupPresenter statsPopupPresenter)
        {
            _audioService = audioService;
            _canvas.worldCamera = uICamera;
            _statsPopupPresenter = statsPopupPresenter;
            Init();
        }

    
        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
            
            _previousStatsButton.onClick.AddListener(OnPreviousButtonClicked);
            _nextStatsButton.onClick.AddListener(OnNextButtonClicked);

            _canvas.enabled = false;
        }

        public void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
            }
            
            _previousStatsButton.onClick.RemoveListener(OnPreviousButtonClicked);
            _nextStatsButton.onClick.RemoveListener(OnNextButtonClicked);
        }
    
        public async UniTask<bool> ShowStatsAndAwaitForExit(UnitType type)
        {
            UpdateView(type);
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void UpdateView(UnitType type)
        {
            var model = _statsPopupPresenter.GetStatsPopupModelFor(type);
        
            if (!model.IsStatsUnlocked)
            {
                model = _statsPopupPresenter.GetStatsPopupModelFor(UnitType.Light);
            }

            _statsPopupPresenter.FindNextAvailableModel(type, false, out bool isPreviousModelAvailable);
            _statsPopupPresenter.FindNextAvailableModel(type, true, out bool isNextModelAvailable);
        
            _previousStatsButton.gameObject.SetActive(isPreviousModelAvailable);
            _nextStatsButton.gameObject.SetActive(isNextModelAvailable);
        
            _currentWarriorLabel.text = model.WarriorName;
            _playerUnitInfoItem.UpdateView(model.PlayerWarriorWarriorInfoItemModel, false);
            _enemyUnitInfoItem.UpdateView(model.EnemyWarriorWarriorInfoItemModel, true);
        }
    
        private void OnNextButtonClicked()
        {
            PlayButtonSound();
            TypeToShow = _statsPopupPresenter
                .FindNextAvailableModel(TypeToShow, true, out _);
        }

        private void OnPreviousButtonClicked()
        {
            PlayButtonSound();
            TypeToShow = _statsPopupPresenter
                .FindNextAvailableModel(TypeToShow, false, out _);
        }

        private void OnCancelled()
        {
            PlayButtonSound();
            _taskCompletion.TrySetResult(false);
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}
