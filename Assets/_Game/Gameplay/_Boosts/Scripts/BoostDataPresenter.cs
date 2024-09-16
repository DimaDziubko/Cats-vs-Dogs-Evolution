using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Debugger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.UI._BoostPopup;
using _Game.Utils.Extensions;

namespace _Game.Gameplay._Boosts.Scripts
{
    public interface IBoostDataPresenter
    {
        event Action<BoostType> BoostModelChanged;
        void ShowBoosts(BoostSource source);
        BoostInfoItemModel TryGetBoostFor(BoostSource source, BoostType itemBoostType);
        BoostUpgradeInfoItemModel TryGetBoostUpgradeInfoFor(BoostType itemBoostType);
    }

    public class BoostDataPresenter : IBoostDataPresenter, IDisposable
    {
        public event Action<BoostType> BoostModelChanged;

        private const float MIN_BOOST_VALUE = 1;

        private readonly IGeneralDataPool _dataPool;
        private readonly IGameInitializer _gameInitializer;
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;

        private readonly Dictionary<BoostSource, BoostPanelModel> _boosts = new Dictionary<BoostSource, BoostPanelModel>();
        private readonly BoostUpgradePanelModel _boostUpgrade = new BoostUpgradePanelModel();
        
        public BoostDataPresenter(
            IGeneralDataPool dataPool,
            IGameInitializer gameInitializer,
            IConfigRepositoryFacade configRepositoryFacade,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyDebugger debugger,
            IMyLogger logger)
        {
            _dataPool = dataPool;
            _gameInitializer = gameInitializer;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _cameraService = cameraService;
            _audioService = audioService;
            _gameInitializer.OnMainInitialization += Init;
            _logger = logger;

            debugger.Boosts = _boosts;
            debugger.BoostUpgrades = _boostUpgrade;
        }

        private void Init()
        {
            BoostData.Changed += OnBoostDataChanged;
            PrepareBoostPanelModel();
            PrepareUpgradePanelModel();
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            BoostData.Changed -= OnBoostDataChanged;
        }

        private void PrepareUpgradePanelModel()
        {
            _boostUpgrade.BoostUpgradeItemModels = new Dictionary<BoostType, BoostUpgradeInfoItemModel>(5);
            foreach (BoostType type in Enum.GetValues(typeof(BoostType)))
            {
                if (type == BoostType.None) continue;
                float value = BoostData.GetBoost(BoostSource.TotalBoosts, type);
                bool isActive = false;
                BoostUpgradeInfoItemModel upgradeInfoItemModel = new BoostUpgradeInfoItemModel()
                {
                    Type = type,
                    CurrentValue = value,
                    Delta = 0f.ToFormattedString(),
                    DisplayValue = value.ToFormattedString(),
                    Icon = _commonConfig.ForBoostIcon(type),
                    IsUpgraded = false
                };
                
                _boostUpgrade.BoostUpgradeItemModels.Add(type, upgradeInfoItemModel);
            }
        }

        private void PrepareBoostPanelModel()
        {
            foreach (BoostSource source in Enum.GetValues(typeof(BoostSource)))
            {
                BoostPanelModel model = new BoostPanelModel();
                model.BoostItemModels = new Dictionary<BoostType, BoostInfoItemModel>(5);

                model.Name = source.ToName();

                foreach (BoostType type in Enum.GetValues(typeof(BoostType)))
                {
                    if (type == BoostType.None) continue;

                    float value = BoostData.GetBoost(source, type);

                    BoostInfoItemModel infoItemModel = new BoostInfoItemModel()
                    {
                        Type = type,
                        Icon = _commonConfig.ForBoostIcon(type),
                        DisplayValue = $"x{value}",
                        IsActive = IsActiveBoost(source, type, value)
                    };

                    model.BoostItemModels.Add(type, infoItemModel);
                }

                _boosts.Add(source, model);
            }
        }

        private void OnBoostDataChanged(BoostSource source, BoostType type, float newValue)
        {
            UpdateBoostPanelModel(source, type, newValue);
            UpdateBoostPanelModel(BoostSource.TotalBoosts, type, BoostData.GetBoost(BoostSource.TotalBoosts, type));
            UpdateUpgradePanelModel(type, newValue);
            BoostModelChanged?.Invoke(type);
        }
        
        private void UpdateUpgradePanelModel(BoostType type, float newValue)
        {
            if (_boostUpgrade.BoostUpgradeItemModels.ContainsKey(type))
            {
                var item = _boostUpgrade.BoostUpgradeItemModels[type];
                if (item != null)
                {
                    item.Delta = (newValue - item.CurrentValue).ToFormattedString();
                    item.DisplayValue = newValue.ToFormattedString();
                    item.IsUpgraded = newValue - item.CurrentValue > 0;
                    item.CurrentValue = newValue;
                }
            }
        }

        private void UpdateBoostPanelModel(BoostSource source, BoostType type, float newValue)
        {
            if (_boosts.ContainsKey(source))
            {
                BoostPanelModel boostModel = _boosts[source];
                if (boostModel != null)
                {
                    if (boostModel.BoostItemModels.ContainsKey(type))
                    {
                        var boostItem = boostModel.BoostItemModels[type];
                        if (boostItem != null)
                        {
                            boostItem.DisplayValue = newValue.ToFormattedString();
                            boostItem.IsActive = IsActiveBoost(source, type, newValue);
                        }
                    }
                }
            }
        }

        private bool IsActiveBoost(BoostSource source, BoostType type, float newValue)
        {
            bool isActive;

            if (IsAlwaysShownBoosts(source, type))
            {
                isActive = true;
            }
            else
            {
                isActive = newValue > MIN_BOOST_VALUE;
            }

            return isActive;
        }


        private bool IsAlwaysShownBoosts(BoostSource source, BoostType type) => 
            source == BoostSource.TotalBoosts && (type == BoostType.AllUnitHealth || type == BoostType.AllUnitDamage);

        async void IBoostDataPresenter.ShowBoosts(BoostSource source)
        {
            IBoostPopupProvider boostPopupProvider = new BoostPopupProvider(_cameraService, _audioService);
            var popup = await boostPopupProvider.Load();

            bool isConfirm = true;
            switch (source)
            {
                case BoostSource.TotalBoosts:
                    isConfirm = await popup.Value.ShowBoosts(_boosts[BoostSource.TotalBoosts]);
                    break;
                case BoostSource.Cards:
                    isConfirm = await popup.Value.ShowBoosts(_boosts[BoostSource.Cards], _boosts[BoostSource.TotalBoosts]);
                    break;
            }
            
            if (isConfirm)
            {
                popup.Dispose();
            }
        }

        public BoostInfoItemModel TryGetBoostFor(BoostSource source, BoostType itemBoostType)
        {
            if (_boosts.ContainsKey(source))
            {
                if(_boosts[source].BoostItemModels.ContainsKey(itemBoostType))
                    return _boosts[source].BoostItemModels[itemBoostType];
                return new BoostInfoItemModel() {IsActive = false};
            }
            
            return new BoostInfoItemModel() {IsActive = false};
        }

        public BoostUpgradeInfoItemModel TryGetBoostUpgradeInfoFor(BoostType type)
        {
            if (_boostUpgrade.BoostUpgradeItemModels.ContainsKey(type))
                return _boostUpgrade.BoostUpgradeItemModels[type];
            return new BoostUpgradeInfoItemModel();
        }
    }
}