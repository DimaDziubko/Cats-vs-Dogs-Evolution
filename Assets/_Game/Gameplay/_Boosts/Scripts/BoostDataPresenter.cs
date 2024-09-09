using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.UI._BoostPopup;
using _Game.Utils.Extensions;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;

namespace _Game.Gameplay._Boosts.Scripts
{
    public interface IBoostDataPresenter
    {
        event Action<BoostType> BoostModelChanged;
        void ShowBoosts(BoostSource source);
        BoostInfoItemModel TryGetBoostFor(BoostSource source, BoostType itemBoostType);
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
        
        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;

        private readonly Dictionary<BoostSource, BoostPanelModel> _boosts = new Dictionary<BoostSource, BoostPanelModel>();

        public BoostDataPresenter(
            IGeneralDataPool dataPool,
            IGameInitializer gameInitializer,
            IConfigRepositoryFacade configRepositoryFacade,
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _dataPool = dataPool;
            _gameInitializer = gameInitializer;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _cameraService = cameraService;
            _audioService = audioService;
            _gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            BoostData.Changed += OnBoostDataChanged;
            
            foreach (BoostSource source in Enum.GetValues(typeof(BoostSource)))
            {
                BoostPanelModel model = new BoostPanelModel();
                model.BoostItemModels = new Dictionary<BoostType, BoostInfoItemModel>(5);

                model.Name = source.ToName();
                
                foreach (BoostType type in Enum.GetValues(typeof(BoostType)))
                {
                    if(type == BoostType.None) continue;
                    
                    float value = BoostData.GetBoost(source, type);

                    bool isActive;
                    
                    if(IsAlwaysShownBoosts(source, type))
                    {
                        isActive = true;
                    }
                    else
                    {
                        isActive = value > MIN_BOOST_VALUE;
                    }
                     
                    BoostInfoItemModel infoItemModel = new BoostInfoItemModel()
                    {
                        Type = type,
                        Icon = _commonConfig.ForBoostIcon(type),
                        Value = $"x{value}",
                        IsActive = isActive
                    };
                    
                    model.BoostItemModels.Add(type, infoItemModel);
                }
                
                _boosts.Add(source, model);
            }
        }

        private void OnBoostDataChanged(BoostSource source, BoostType type, float newValue)
        {
            UpdateInfoItemModel(source, type, newValue);
            UpdateInfoItemModel(BoostSource.TotalBoosts, type, newValue);
            BoostModelChanged?.Invoke(type);
        }

        private void UpdateInfoItemModel(BoostSource source, BoostType type, float newValue)
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
                            boostItem.Value = newValue.ToFormattedString();
                            boostItem.IsActive = newValue > MIN_BOOST_VALUE;
                        }
                    }
                }
            }
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
        
        public void Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            BoostData.Changed -= OnBoostDataChanged;
        }
    }
}