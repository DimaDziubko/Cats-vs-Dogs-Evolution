using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Configs.Controllers;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay.UpgradesAndEvolution.Scripts
{
    public class UpgradesAndEvolutionService : IUpgradesAndEvolutionService
    {
        private readonly IPersistentDataService _persistentData;
        private readonly IGameConfigController _gameConfigController;
        private readonly IAssetProvider _assetProvider;
        private IUserTimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IUserCurrenciesStateReadonly Currency => _persistentData.State.Currencies;
        
        public UpgradesAndEvolutionService(
            IPersistentDataService persistentData,
            IGameConfigController gameConfigController,
            IAssetProvider assetProvider)
        {
            _persistentData = persistentData;
            _gameConfigController = gameConfigController;
            _assetProvider = assetProvider;
        }
        
        public void MoveToNextAge()
        {
            throw new System.NotImplementedException();
        }

        public bool IsNextAgeAvailable()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId) <= Currency.Coins;
        }

        public int GetTimelineNumber() => TimelineState.TimelineId;
        

        public float GetEvolutionPrice()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId);
        }
        
        
        public async UniTask<UpgradeItemModel[]> GetUpgradeItems()
        {
            List<WarriorConfig> warriorConfigs = _gameConfigController.GetCurrentAgeUnits();
            UpgradeItemModel[] models = new UpgradeItemModel[warriorConfigs.Count];

            for (int i = 0; i < warriorConfigs.Count; i++)
            {
                var config = warriorConfigs[i];
                Sprite icon = await _assetProvider.Load<Sprite>(config.IconKey);

                models[i] = new UpgradeItemModel
                {
                    Icon = icon, 
                    Name = config.Name,
                    Price = config.Price,
                    IsBought = TimelineState.OpenUnits.Contains(warriorConfigs[i].Type),
                    CanAfford = Currency.Coins >= config.Price
                };
            }

            return models;
        }

        public void PurchaseUnit(in UnitType type)
        {
            var unitPrice = _gameConfigController.GetUnitPrice(type);
            
            if (Currency.Coins >= unitPrice)
            {
                _persistentData.PurchaseUnit(type, unitPrice);
            }
            else
            {
                Debug.LogError($"Cannot purchase unit {type}. Either already opened or not enough coins.");
            }
        }

        public float GetFoodProductionSpeed()
        {
            var foodProduction = _gameConfigController.GetFoodProduction();

            float baseSpeed = foodProduction.Speed;

            float totalSpeed = baseSpeed * Mathf.Pow(foodProduction.SpeedFactor, TimelineState.FoodProductionLevel);
    
            return totalSpeed;
        }
    }

    public interface IUpgradesAndEvolutionService
    {
        void MoveToNextAge();
        float GetEvolutionPrice();
        bool IsNextAgeAvailable();
        int GetTimelineNumber();
        UniTask<UpgradeItemModel[]> GetUpgradeItems();
        void PurchaseUnit(in UnitType type);
        float GetFoodProductionSpeed();
    }
}