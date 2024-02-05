using _Game.Core.Configs.Controllers;
using _Game.Core.Services.PersistentData;
using _Game.Core.Services.StaticData;
using _Game.Core.UserState;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
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
        

        private float GetUnitPrice(in int index)
        {
            return _gameConfigController.GetUnitPrice(index);
        }
        

        public float GetEvolutionPrice()
        {
            return _gameConfigController
                .GetAgePrice(TimelineState.AgeId);
        }
        
        
        public UpgradeItemModel[] GetUpgradeItems()
        {
            int capacity = 3;
            
            UpgradeItemModel[] models = new UpgradeItemModel[capacity];
        
            var unitIcons = _assetProvider.ForUnitIcons();
            var unitNames = _gameConfigController.GetUnitNames();
            
            for (int i = 0; i < capacity; i++)
            {
                float price = GetUnitPrice(i);
                
                models[i] = new UpgradeItemModel 
                {
                    Icon = unitIcons[i],
                    Name = unitNames[i],
                    Price = price,
                    IsBought = TimelineState.OpenUnits.Contains(i),
                    CanAfford = Currency.Coins >= price
                };
            }
        
            return models;
        }

        public void PurchaseUnit(in int unitIndex)
        {
            var unitPrice = _gameConfigController.GetUnitPrice(unitIndex);
            if (Currency.Coins >= unitPrice)
            {
                _persistentData.PurchaseUnit(unitIndex, unitPrice);
            }
            else
            {
                Debug.LogError($"Cannot purchase unit {unitIndex}. Either already opened or not enough coins.");
            }
        }
    }

    public interface IUpgradesAndEvolutionService
    {
        void MoveToNextAge();
        float GetEvolutionPrice();
        bool IsNextAgeAvailable();
        int GetTimelineNumber();
        UpgradeItemModel[] GetUpgradeItems();
        void PurchaseUnit(in int unitIndex);
    }
}