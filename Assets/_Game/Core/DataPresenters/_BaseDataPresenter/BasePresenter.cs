using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Utils;
using Assets._Game.Core.Data;
using Assets._Game.Core.DataPresenters._RaceChanger;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using Assets._Game.Utils;

namespace _Game.Core.DataPresenters._BaseDataPresenter
{
 class BasePresenter : IBasePresenter, IDisposable
    {
        public event Action<Faction> BaseUpdated;
        
        //TODO Check later
        public event Action<BaseModel> PlayerBaseDataUpdated;

        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IRaceChanger _raceChanger;
        private IUpgradeItemsReadonly UpgradeItems => _dataPool.AgeDynamicData.UpgradeItems;

        public BasePresenter(
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
            UpgradeItems.Changed += OnUpgradeItemChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _raceChanger.RaceChanged += OnRaceChanged;
        }

        void IDisposable.Dispose()
        {
            UpgradeItems.Changed -= OnUpgradeItemChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _raceChanger.RaceChanged -= OnRaceChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnAgeChanged()
        {
            BaseUpdated?.Invoke(Faction.Player);
            PlayerBaseDataUpdated?.Invoke(GetBaseModel(Constants.CacheContext.AGE));
        }

        private void OnRaceChanged()
        {
            BaseUpdated?.Invoke(Faction.Player);
            BaseUpdated?.Invoke(Faction.Enemy);
            PlayerBaseDataUpdated?.Invoke(GetBaseModel(Constants.CacheContext.AGE));
        }

        private void OnUpgradeItemChanged(UpgradeItemType type, UpgradeItemDynamicData data)
        {
            if(type == UpgradeItemType.BaseHealth)
                PlayerBaseDataUpdated?.Invoke(GetBaseModel(Constants.CacheContext.AGE));
        }

        public BaseModel GetBaseModel(int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                var model = new BaseModel()
                {
                    StaticData = _dataPool.AgeStaticData.ForBase(),
                    Health = GetBaseHealth(Faction.Player)
                };
                
                return model;
            }
            else if(context == Constants.CacheContext.BATTLE)
            {
                var model = new BaseModel()
                {
                    StaticData = _dataPool.BattleStaticData.ForBase(_navigator.CurrentBattle),
                    Health = GetBaseHealth(Faction.Enemy)
                };
                
                return model;
            }
            else
            {
                _logger.LogError("BaseModel GetBaseData There is no such context");
                return null;
            }
        }

        public float GetBaseHealth(Faction faction)
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