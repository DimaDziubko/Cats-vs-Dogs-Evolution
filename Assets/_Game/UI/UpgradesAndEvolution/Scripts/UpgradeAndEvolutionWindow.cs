using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Evolution.Scripts;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionWindow : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradesWindow _upgradesWindow;
        [SerializeField] private EvolutionWindow _evolutionWindow;
        [SerializeField] private Button _upgradesButton;
        [SerializeField] private Button _evolutionButton;
        
        private IAudioService _audioService;
        private IPersistentDataService _persistentData;
        private IUserStateCommunicator _communicator;
        private IAlertPopupProvider _alertPopupProvider;
        
        public void Construct(
            Camera uICamera, 
            IPersistentDataService persistentData, 
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider,
            
            IHeader header,
            
            IUpgradesService upgradesService,
            IEvolutionService evolutionService)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _persistentData = persistentData;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;
            
            _upgradesWindow.Construct(header, upgradesService);
            _evolutionWindow.Construct(header, evolutionService);

            _upgradesButton.onClick.AddListener(OnUpgradesButtonClick);
            _evolutionButton.onClick.AddListener(OnEvolutionButtonClick);

            OnUpgradesButtonClick();
        }

        private void OnEvolutionButtonClick()
        {
            //TODO Play sound
            
            _evolutionWindow.Show();
            _upgradesWindow.Hide();
        }

        private void OnUpgradesButtonClick()
        {
            //TODO Play sound
            
            _evolutionWindow.Hide();
            _upgradesWindow.Show();
        }

        private void OnDisable()
        {
            _upgradesButton.onClick.RemoveAllListeners();
            _evolutionButton.onClick.RemoveAllListeners();
        }
    }
}
