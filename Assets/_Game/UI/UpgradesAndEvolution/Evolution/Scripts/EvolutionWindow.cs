using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Evolution.Scripts
{
    public class EvolutionWindow : MonoBehaviour, IUIWindow
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private EvolutionTab _evolutionTab;
        [SerializeField] private TravelTab _travelTab;
        
        private IEvolutionService _evolutionService;
        private IHeader _header;
        public string Name => "Evolution";
        
        public void Construct(
            IHeader header,
            IEvolutionService evolutionService,
            IAudioService audioService)
        {
            _evolutionService = evolutionService;
            _header = header;

            _evolutionTab.Construct(evolutionService, audioService);
            _travelTab.Construct(evolutionService, audioService);
        }


        public void Show()
        {
            _header.ShowWindowName(Name);
            
            _canvas.enabled = true;

            if (IsTimeToTravel())
            {
                _travelTab.Show();
            }
            else
            {
                _evolutionTab.Show();
            }

            Unsubscribe();
            Subscribe();
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _travelTab.Hide();
            _evolutionTab.Hide();
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            _evolutionService.LastAgeOpened += OnLastAgeOpened;
        }

        private void Unsubscribe()
        {
            _evolutionService.LastAgeOpened -= OnLastAgeOpened;
        }

        private void OnLastAgeOpened()
        {
            _travelTab.Show();
            _evolutionTab.Hide();
        }

        private bool IsTimeToTravel()
        {
            return _evolutionService.IsTimeToTravel();
        }
        
    }
}