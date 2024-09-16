using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core._DataProviders._BaseDataProvider
{
    class BaseDataProvider : IBaseDataProvider, IDisposable
    {
        public event Action<Faction> BaseUpdated;

        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IRaceChanger _raceChanger;
        private IUpgradeItemsReadonly UpgradeItems => _dataPool.AgeDynamicData.UpgradeItems;
        private IBoostsDataReadonly BoostData => _dataPool.AgeDynamicData.BoostsData;

        public BaseDataProvider(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            IRaceChanger raceChanger)
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _gameInitializer = gameInitializer;
            _raceChanger = raceChanger;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            _ageNavigator.AgeChanged += OnAgeChanged;
            _raceChanger.RaceChanged += OnRaceChanged;
        }

        void IDisposable.Dispose()
        {
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _raceChanger.RaceChanged -= OnRaceChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }
        
        private void OnAgeChanged()
        {
            BaseUpdated?.Invoke(Faction.Player);
        }

        private void OnRaceChanged()
        {
            BaseUpdated?.Invoke(Faction.Player);
            BaseUpdated?.Invoke(Faction.Enemy);
        }
        
        public IBaseData GetBaseData(int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                var model = new BaseData(_dataPool.AgeStaticData.ForBase())
                {
                    Health = GetBaseHealth(Faction.Player)
                };

                BaseHealthBoostDecorator healthBoostDecorator
                    = new BaseHealthBoostDecorator(model,
                        BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.BaseHealth));
                
                return healthBoostDecorator;
            }
            else if (context == Constants.CacheContext.BATTLE)
            {
                var model = new BaseData(_dataPool.BattleStaticData.ForBase(_navigator.CurrentBattle))
                {
                    Health = GetBaseHealth(Faction.Enemy)
                };

                BaseLootBoostDecorator lootBoostDecorator
                    = new BaseLootBoostDecorator(model,
                        BoostData.GetBoost(BoostSource.TotalBoosts, BoostType.BaseHealth));

                return lootBoostDecorator;
            }
            else
            {
                _logger.LogError("BaseModel GetBaseData There is no such context");
                return null;
            }
        }

        private float GetBaseHealth(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return UpgradeItems.GetItemData(UpgradeItemType.BaseHealth).Amount;
                case Faction.Enemy:
                    return _dataPool.BattleStaticData.ForBaseHealth(_navigator.CurrentBattle);
                default:
                    return 0;
            }
        }
    }
}