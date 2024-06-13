using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Data;
using _Game.Core.Data.Age.Dynamic._UpgradeItem;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Gameplay._Bases.Scripts;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;
using _Game.Utils;

namespace _Game.Core.DataPresenters._BaseDataPresenter
{
    class BasePresenter : IBasePresenter, IDisposable
    {
        public event Action PlayerBaseUpdated;
        public event Action<BaseModel> PlayerBaseDataUpdated;

        private readonly IGeneralDataPool _dataPool;
        private readonly IBattleNavigator _navigator;
        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private IUpgradeItemsReadonly UpgradeItems => _dataPool.AgeDynamicData.UpgradeItems;

        public BasePresenter(
            IGeneralDataPool dataPool,
            IBattleNavigator navigator,
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator)
        {
            _dataPool = dataPool;
            _navigator = navigator;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _gameInitializer = gameInitializer;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            UpgradeItems.Changed += OnUpgradeItemChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
        }

        void IDisposable.Dispose()
        {
            UpgradeItems.Changed -= OnUpgradeItemChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnAgeChanged()
        {
            PlayerBaseUpdated?.Invoke();
            PlayerBaseDataUpdated?.Invoke(GetTowerData(Constants.CacheContext.AGE));
        }

        private void OnUpgradeItemChanged(UpgradeItemType type, UpgradeItemDynamicData obj)
        {
            if(type == UpgradeItemType.BaseHealth)
                PlayerBaseDataUpdated?.Invoke(GetTowerData(Constants.CacheContext.AGE));
        }
        
        public BaseModel GetTowerData(int context)
        {
            if (context == Constants.CacheContext.AGE)
            {
                var model = new BaseModel()
                {
                    StaticData = _dataPool.AgeStaticData.ForBase(),
                    Health = UpgradeItems.GetItemData(UpgradeItemType.BaseHealth).Amount,
                };
                
                return model;
            }
            else if(context == Constants.CacheContext.BATTLE)
            {
                var model = new BaseModel()
                {
                    StaticData = _dataPool.BattleStaticData.ForBase(_navigator.CurrentBattle),
                    Health = _dataPool.BattleStaticData.ForBaseHealth(_navigator.CurrentBattle),
                };
                
                return model;
            }
            else
            {
                _logger.LogError("TowerModel GetTowerData There is no such context");
                return null;
            }
        }
    }
}