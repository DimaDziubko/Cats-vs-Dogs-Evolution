using System;
using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using Assets._Game.Core.UserState;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public class UnitBuilderDataPresenter : IUnitBuilderDataPresenter
    {
        public event Action<Dictionary<UnitType, UnitBuilderBtnModel>> BuilderModelUpdated;
        
        private readonly IUserContainer _userContainer;
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IUnitDataProvider _unitDataProvider;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        
        public UnitBuilderDataPresenter(
            IConfigRepositoryFacade configRepositoryFacade,
            IUserContainer userContainer,
            IUnitDataProvider unitDataProvider)
        {
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _userContainer = userContainer;
            _unitDataProvider = unitDataProvider;
        }
        
        private void UpdateData()
        {
            Dictionary<UnitType, UnitBuilderBtnModel> updatedModels = new Dictionary<UnitType, UnitBuilderBtnModel>(3);
            
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                IUnitData unitData = _unitDataProvider.GetDecoratedUnitData(type, Constants.CacheContext.AGE);
                var isUnlocked = TimelineState.OpenUnits.Contains(type);
                var foodIcon = _commonConfig.ForFoodIcon(RaceState.CurrentRace);

                updatedModels[type] = new UnitBuilderBtnModel
                {
                    Type = type,
                    FoodPrice = unitData.FoodPrice,
                    UnitIcon = unitData.Icon,
                    FoodIcon = foodIcon,
                    IsUnlocked = isUnlocked
                };
            }

            BuilderModelUpdated?.Invoke(updatedModels);
        }
        
        void IUnitBuilderDataPresenter.OnBuilderStarted() => UpdateData();
    }
}