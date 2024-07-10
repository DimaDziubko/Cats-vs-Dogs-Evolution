using System;
using System.Collections.Generic;
using _Game.Core.DataPresenters.BattlePresenter;
using _Game.Gameplay._Battle.Scripts;
using Assets._Game.UI._Environment;
using Assets._Game.UI._Environment.Factory;

namespace _Game.UI._Environment
{
    public class EnvironmentController : IDisposable
    {
        private readonly IEnvironmentFactory _factory;
        private readonly IBattlePresenter _battlePresenter;

        private readonly Dictionary<string, BattleEnvironment> _environmentCache = new Dictionary<string, BattleEnvironment>();

        private BattleEnvironment _currentBattleEnvironment;

        public EnvironmentController(
            IEnvironmentFactory factory,
            IBattlePresenter battlePresenter)
        {
            _factory = factory;
            _battlePresenter = battlePresenter;
        }

        public void Init()
        {
            ShowEnvironment(_battlePresenter.BattleData.EnvironmentData);
            _battlePresenter.BattleDataUpdated += OnBattleDataUpdated;
        }

        private void OnBattleDataUpdated(BattleData data, bool needClearCache)
        {
            if(needClearCache) Cleanup();
            ShowEnvironment(data.EnvironmentData);
        }

        public void Dispose()
        {
            _battlePresenter.BattleDataUpdated -= OnBattleDataUpdated;
        }

        private void ShowEnvironment(EnvironmentData environmentData)
        {
            if(_currentBattleEnvironment) _currentBattleEnvironment.Hide();
            
            if (_environmentCache.ContainsKey(environmentData.Key))
            {
                _currentBattleEnvironment = _environmentCache[environmentData.Key];
                _currentBattleEnvironment.Show();
                return;
            }
            
            BattleEnvironment newEnvironment = _factory.Get(environmentData.Prefab);
            _currentBattleEnvironment = newEnvironment;
            _currentBattleEnvironment.Show();
            
            _environmentCache.Add(environmentData.Key, newEnvironment);
        }


        private void Cleanup()
        {
            foreach (var environment in _environmentCache)
            {
                environment.Value.Recycle();
            }
            
            _environmentCache.Clear();
        }
    }
}