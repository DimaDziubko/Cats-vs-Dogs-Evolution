using System;
using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._UpgradesChecker;
using _Game.Core.DataPresenters._UpgradeItemPresenter;
using _Game.Core.DataPresenters.UnitUpgradePresenter;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Header.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.Common.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradesScreen : MonoBehaviour, IGameScreen
    {
        public event Action Opened;
        public GameScreen GameScreen => GameScreen.Upgrades;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradeUnitItemView[] _unitItems;
        [SerializeField] private UpgradeItemView _foodProduction, _baseHealth;

        [SerializeField] private AudioClip _unitUpgradeSFX;

        [SerializeField] private TutorialStep _foodProductionStep;

        private IUpgradeItemPresenter _upgradeItemPresenter;
        private IUnitUpgradesPresenter _unitUpgradesPresenter;
        private IHeader _header;
        private IAudioService _audioService;
        private ITutorialManager _tutorialManager;
        private IUpgradesAvailabilityChecker _upgradesChecker;
        private IMiniShopProvider _miniShopProvider;


        public void Construct(
            IHeader header,
            IUpgradeItemPresenter upgradeItemPresenter,
            IUnitUpgradesPresenter unitUpgradesPresenter,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            IUpgradesAvailabilityChecker upgradesChecker,
            IMiniShopProvider miniShopProvider)
        {
            _upgradeItemPresenter = upgradeItemPresenter;
            _header = header;
            _unitUpgradesPresenter = unitUpgradesPresenter;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _upgradesChecker = upgradesChecker;
            _miniShopProvider = miniShopProvider;
        }

        public void Show()
        {
            _canvas.enabled = true;
            _header.ShowScreenName(GameScreen.ToString(), Color.white);

            Unsubscribe();
            Subscribe();
            
            _tutorialManager.Register(_foodProductionStep);

            InitItems();
        
            Opened?.Invoke();

            _upgradesChecker.MarkAsReviewed(GameScreen);
            _upgradesChecker.MarkAsReviewed(GameScreen.UpgradesAndEvolution);
        }

        private void Subscribe()
        {
            foreach (var unitItem in _unitItems)
            {
                unitItem.Upgrade += OnUnitItemUpgrade;
                unitItem.TryUpgrade += OnTryUpgrade;
                unitItem.InfoClicked += OnInfoClicked;
            }
            
            _foodProduction.Upgrade += OnItemUpgrade;
            _baseHealth.Upgrade += OnItemUpgrade;
            _foodProduction.TryUpgrade += OnTryUpgrade;
            _baseHealth.TryUpgrade += OnTryUpgrade;
            _foodProduction.ButtonStateChanged += OnFoodProductionButtonStateChanged;

            _unitUpgradesPresenter.UpgradeUnitItemsUpdated += UpdateUnitItems;
            _upgradeItemPresenter.UpgradeItemUpdated += UpdateUpgradeItem;

            Opened += OnUpgradesScreenOpened;
        }

        private void OnInfoClicked(UnitType type)
        {
            _unitUpgradesPresenter.ShowInfoFor(type);
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
            _unitUpgradesPresenter.UpgradeUnitItemsUpdated -= UpdateUnitItems;
            _upgradeItemPresenter.UpgradeItemUpdated -= UpdateUpgradeItem;

            foreach (var item in _unitItems)
            {
                item.Upgrade -= OnUnitItemUpgrade;
                item.TryUpgrade -= OnTryUpgrade;
                item.InfoClicked -= OnInfoClicked;
                item.Cleanup();
            }

            _baseHealth.Upgrade -= OnItemUpgrade;
            _foodProduction.Upgrade -= OnItemUpgrade;
            _foodProduction.TryUpgrade -= OnTryUpgrade;
            _baseHealth.TryUpgrade -= OnTryUpgrade;
            _foodProduction.ButtonStateChanged -= OnFoodProductionButtonStateChanged;

            Opened -= OnUpgradesScreenOpened;
        }

        private async void OnTryUpgrade(float price)
        {
            if(!_miniShopProvider.IsUnlocked) return;
            var popup = await _miniShopProvider.Load();
            var isExit =  await popup.Value.ShowAndAwaitForDecision(price);
            if (isExit)
            {
                popup.Value.Hide();
                popup.Dispose();
            }
        }

        private void OnFoodProductionButtonStateChanged(ButtonState state)
        {
            if (state == ButtonState.Active)
            {
                _foodProductionStep.ShowStep();
            }
        }

        private void OnUnitItemUpgrade(UnitType type, float price)
        {
            _unitUpgradesPresenter.PurchaseUnit(type, price);
            PlayUpgradeSound();
        }

        private void OnItemUpgrade(UpgradeItemType type, float price)
        {
            if(type == UpgradeItemType.FoodProduction)
                _foodProductionStep.CompleteStep();

            _upgradeItemPresenter.UpgradeItem(type, price);
        }

        private void UpdateUnitItems(Dictionary<UnitType, UnitUpgradeItemModel> models)
        {
            foreach (var item in _unitItems)
            {
                var model = models[item.Type];
                if (model != null)
                {
                    item.UpdateUI(model);
                }
            }
        }

        private void OnUpgradesScreenOpened()
        {
            _unitUpgradesPresenter.OnUpgradesWindowOpened();
            _upgradeItemPresenter.OnUpgradesScreenOpened();
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

        private void UpdateUpgradeItem(UpgradeItemModel model)
        {
            switch (model.StaticData.Type)
            {
                case UpgradeItemType.FoodProduction:
                    _foodProduction.UpdateUI(model);
                    break;
                case UpgradeItemType.BaseHealth:
                    _baseHealth.UpdateUI(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.StaticData.Type), model.StaticData.Type, null);
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