using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public class UnitDataProvider : IUnitDataProvider
    {
        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;
        private readonly IDifficultyConfigRepository _difficultyConfig;
        private readonly ITimelineNavigator _timelineNavigator;

        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;

        public UnitDataProvider(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger,
            IConfigRepositoryFacade configRepositoryFacade,
            ITimelineNavigator timelineNavigator)
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
            _difficultyConfig = configRepositoryFacade.DifficultyConfigRepository;
            _timelineNavigator = timelineNavigator;
        }

        public IUnitData GetDecoratedUnitData(UnitType type, int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                IUnitData vanillaData = _dataPool.AgeStaticData.ForUnit(type);
                DamageBoostDecorator damageDecoratedData = new DamageBoostDecorator(vanillaData,
                    BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.AllUnitDamage));
                HealthBoostDecorator healthBoostDecorator = new HealthBoostDecorator(damageDecoratedData,
                    BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.AllUnitHealth));
                return healthBoostDecorator;
            }
            if (context == Constants.CacheContext.BATTLE)
            {
                var difficulty = _difficultyConfig.GetDifficultyValue(_timelineNavigator.CurrentTimelineNumber);
                
                IUnitData vanillaData = _dataPool.BattleStaticData.ForUnit(_navigator.CurrentBattle, type);
                UnitLootBoostDecorator lootBoostDecorator
                    = new UnitLootBoostDecorator(vanillaData,
                        BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.CoinsGained));
                DamageBoostDecorator damageDecoratedData = new DamageBoostDecorator(lootBoostDecorator,
                    difficulty);
                HealthBoostDecorator healthBoostDecorator = new HealthBoostDecorator(damageDecoratedData,
                    difficulty);
                return healthBoostDecorator;
            }

            _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
            return null;
        }
    }
}