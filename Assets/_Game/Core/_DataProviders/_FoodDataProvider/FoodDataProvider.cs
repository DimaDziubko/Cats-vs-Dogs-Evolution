using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Boosts.Scripts;
 using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
 using Assets._Game.Core.UserState;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public class FoodProductionDataProvider : IFoodProductionDataProvider
    {
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IEconomyConfigRepository _economyConfig;
        private readonly IGeneralDataPool _generalDataPool;
        private readonly IUserContainer _userContainer;

        private IRaceStateReadonly RaceState => _userContainer.State.RaceState;
        private IUpgradeItemsReadonly UpgradeItems => _generalDataPool.AgeDynamicData.UpgradeItems;

        private IBoostsDataReadonly BoostsData => _generalDataPool.AgeDynamicData.BoostsData;

        public FoodProductionDataProvider(
            IConfigRepositoryFacade configRepositoryFacade,
            IGeneralDataPool generalDataPool,
            IUserContainer userContainer)
        {
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _economyConfig = configRepositoryFacade.EconomyConfigRepository;
            _generalDataPool = generalDataPool;
            _userContainer = userContainer;
        }


        public IFoodProductionData GetData()
        {
            var data = new FoodProductionData()
            {
                FoodIcon = _commonConfig.ForFoodIcon(RaceState.CurrentRace),
                ProductionSpeed = UpgradeItems.GetItemData(UpgradeItemType.FoodProduction).Amount,
                InitialFoodAmount = _economyConfig.GetInitialFoodAmount()
            };

            FoodProductionBoostDecorator productionBoostDecorator 
                = new FoodProductionBoostDecorator(data, BoostsData.GetBoost(BoostSource.TotalBoosts, BoostType.FoodProduction));
            return productionBoostDecorator;
        }
    }
}