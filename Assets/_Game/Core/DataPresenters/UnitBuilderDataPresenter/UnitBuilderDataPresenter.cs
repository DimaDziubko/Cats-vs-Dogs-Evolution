using System;
using System.Collections.Generic;
using _Game.Core.Data;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.DataPresenters.UnitBuilderDataPresenter
{
    public class UnitBuilderDataPresenter : IUnitBuilderDataPresenter
    {
        public event Action<Dictionary<UnitType, UnitBuilderBtnModel>> BuilderModelUpdated;
        
        private readonly IGeneralDataPool _dataPool;
        private readonly IUserContainer _persistentData;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        
        public UnitBuilderDataPresenter(
            IGeneralDataPool dataPool,
            IUserContainer persistentData)
        {
            _dataPool = dataPool;
            _persistentData = persistentData;
        }
        
        private void UpdateData()
        {
            Dictionary<UnitType, UnitBuilderBtnModel> updatedModels = new Dictionary<UnitType, UnitBuilderBtnModel>(3);
            
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                var staticData = _dataPool.GetBuilderButtonData(type);
                var isUnlocked = TimelineState.OpenUnits.Contains(type);
                var foodIcon = _dataPool.AgeStaticData.ForFoodIcon(RaceState.CurrentRace);

                updatedModels[type] = new UnitBuilderBtnModel
                {
                    StaticData = staticData,
                    DynamicData = new UnitBuilderBtnDynamicData
                    {
                        FoodIcon = foodIcon,
                        IsUnlocked = isUnlocked
                    }
                };
            }

            BuilderModelUpdated?.Invoke(updatedModels);
        }
        
        void IUnitBuilderDataPresenter.OnBuilderStarted() => UpdateData();
    }
}