using System;
using System.Collections.Generic;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Pin.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradesWindow : MonoBehaviour, IUIWindow
    {
        public event Action Opened;
        public Window Window => Window.Upgrades;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradeUnitItemView[] _unitItems;
        [SerializeField] private UpgradeItemView _foodProduction, _baseHealth;

        [SerializeField] private AudioClip _unitUpgradeSFX;

        [SerializeField] private TutorialStep _foodProductionStep;

        private IEconomyUpgradesService _economyUpgradesService;
        private IUnitUpgradesService _unitUpgradesService;
        private IHeader _header;
        private IAudioService _audioService;
        private ITutorialManager _tutorialManager;
        private IUpgradesAvailabilityChecker _upgradesChecker;


        public void Construct(
            IHeader header,
            IEconomyUpgradesService economyUpgradesService,
            IUnitUpgradesService unitUpgradesService,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker)
        {
            _economyUpgradesService = economyUpgradesService;
            _header = header;
            _unitUpgradesService = unitUpgradesService;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
        }

        public void Show()
        {
            _canvas.enabled = true;
            _header.ShowWindowName(Window.ToString());

            Unsubscribe();
            Subscribe();
            
            _tutorialManager.Register(_foodProductionStep);

            InitItems();
        
            Opened?.Invoke();

            _upgradesChecker.MarkAsReviewed(Window);
            _upgradesChecker.MarkAsReviewed(Window.UpgradesAndEvolution);
        }

        private void Subscribe()
        {
            foreach (var unitItem in _unitItems)
            {
                unitItem.Upgrade += OnUnitItemUpgrade;
            }
            
            _foodProduction.Upgrade += OnItemUpgrade;
            _foodProduction.ButtonStateChanged += OnFoodProductionButtonStateChanged;
            _baseHealth.Upgrade += OnItemUpgrade;
            
            _unitUpgradesService.UpgradeUnitItemsUpdated += UpdateUnitItems;
            _economyUpgradesService.UpgradeItemUpdated += UpdateEconomyUpgradeItem;

            Opened += OnUpgradesWindowOpened;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            
            Unsubscribe();

            _foodProduction.Cleanup();
            _baseHealth.Cleanup();
            
            _foodProductionStep.CancelStep();
            _tutorialManager.UnRegister(_foodProductionStep);
        }

        private void Unsubscribe()
        {
            _unitUpgradesService.UpgradeUnitItemsUpdated -= UpdateUnitItems;
            _economyUpgradesService.UpgradeItemUpdated -= UpdateEconomyUpgradeItem;

            foreach (var item in _unitItems)
            {
                item.Upgrade -= OnUnitItemUpgrade;
                item.Cleanup();
            }

            _baseHealth.Upgrade -= OnItemUpgrade;
            _foodProduction.Upgrade -= OnItemUpgrade;
            _foodProduction.ButtonStateChanged -= OnFoodProductionButtonStateChanged;

            Opened -= OnUpgradesWindowOpened;
        }

        private void OnFoodProductionButtonStateChanged(ButtonState state)
        {
            if (state == ButtonState.Active)
            {
                _foodProductionStep.ShowStep();
            }
        }

        private void OnUnitItemUpgrade(UnitType type)
        {
            _unitUpgradesService.PurchaseUnit(type);
            PlayUpgradeSound();
        }

        private void OnItemUpgrade(UpgradeItemType type)
        {
            if(type == UpgradeItemType.FoodProduction)
                _foodProductionStep.CompleteStep();

            _economyUpgradesService.UpgradeItem(type);
        }

        private void UpdateUnitItems(IEnumerable<UpgradeUnitItemViewModel> models)
        {
            foreach (var model in models)
            {
               _unitItems[(int)model.Type].UpdateUI(model); 
            }
        }

        private void OnUpgradesWindowOpened()
        {
            _unitUpgradesService.OnUpgradesWindowOpened();
            _economyUpgradesService.OnUpgradesWindowOpened();
        }

        private void InitItems()
        {
            foreach (var unitItem in _unitItems)
            {
                unitItem.Init(_audioService);
            }
            
            _foodProduction.Init(_audioService);
            _baseHealth.Init(_audioService);
        }

        private void UpdateEconomyUpgradeItem(UpgradeItemViewModel model)
        {
            switch (model.Type)
            {
                case UpgradeItemType.FoodProduction:
                    _foodProduction.UpdateUI(model);
                    break;
                case UpgradeItemType.BaseHealth:
                    _baseHealth.UpdateUI(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Type), model.Type, null);
            }
        }

        private void PlayUpgradeSound()
        {
            if (_audioService != null && _unitUpgradeSFX != null)
            {
                _audioService.PlayOneShot(_unitUpgradeSFX);
            }
        }
    }
}