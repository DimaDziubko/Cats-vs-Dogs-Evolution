using _Game.Core.Services.Evolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionWindow : MonoBehaviour, IUIWindow
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private TransactionButton _evolveButton;
        [SerializeField] private Image _currentAgeImage; 
        [SerializeField] private Image _nextAgeImage;
        [SerializeField] private TMP_Text _timelineLabel;

        private IEvolutionService _evolutionService;
        private IHeader _header;
        public string Name => "Evolution";

        public void Construct(
            IHeader header,
            IEvolutionService evolutionService)
        {
            _evolutionService = evolutionService;
            _header = header;

            _evolveButton.Init();
            _evolveButton.Click += OnEvolveButtonClick;
            
            UpdateUIElements();
        }


        public void Show()
        {
            _canvas.enabled = true;
            _header.ShowWindowName(Name);
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        private void OnEvolveButtonClick()
        {
            _evolutionService.MoveToNextAge();
        }

        private void UpdateUIElements()
        {
            UpdateEvolveButton();
            UpdateTimelineLabel();
        }

        private void UpdateTimelineLabel()
        {
            var id = _evolutionService.GetTimelineNumber();
            _timelineLabel.text = $"Timeline {id + 1}";
        }

        private void UpdateEvolveButton()
        {
            bool canAfford = _evolutionService.IsNextAgeAvailable();
            float evolutionPrice = _evolutionService.GetEvolutionPrice();
            _evolveButton.UpdateButtonState(canAfford, evolutionPrice);
        }

        private void OnDisable()
        {
            _evolveButton.Click -= OnEvolveButtonClick;
        }
    }
}