using _Game.Gameplay.UpgradesAndEvolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using TMPro;
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

        private IUpgradesAndEvolutionService _upgradesAndEvolutionService;
        private IHeader _header;
        public string Name => "Evolution";

        public void Construct(
            IHeader header,
            IUpgradesAndEvolutionService upgradesAndEvolutionService)
        {
            _upgradesAndEvolutionService = upgradesAndEvolutionService;
            _header = header;

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
            _upgradesAndEvolutionService.MoveToNextAge();
        }

        private void UpdateUIElements()
        {
            UpdateEvolveButton();
            UpdateTimelineLabel();
        }

        private void UpdateTimelineLabel()
        {
            var number = _upgradesAndEvolutionService.GetTimelineNumber();
            _timelineLabel.text = $"Timeline {number}";
        }

        private void UpdateEvolveButton()
        {
            bool canAfford = _upgradesAndEvolutionService.IsNextAgeAvailable();
            float evolutionPrice = _upgradesAndEvolutionService.GetEvolutionPrice();
            _evolveButton.UpdateButtonState(canAfford, evolutionPrice);
        }

        private void OnDisable()
        {
            _evolveButton.Click -= OnEvolveButtonClick;
        }
    }
}