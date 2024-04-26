using System.Collections.Generic;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._Environment.Factory;

namespace _Game.UI._Environment
{
    public class EnvironmentController
    {
        private readonly IEnvironmentFactory _factory;

        private readonly Dictionary<string, BattleEnvironment> _environmentCache = new Dictionary<string, BattleEnvironment>();

        private BattleEnvironment _currentBattleEnvironment;
        
        public EnvironmentController(IEnvironmentFactory factory)
        {
            _factory = factory;
        }

        public void ShowEnvironment(EnvironmentData dataEnvironmentData)
        {
            if(_currentBattleEnvironment) _currentBattleEnvironment.Hide();
            
            if (_environmentCache.ContainsKey(dataEnvironmentData.Key))
            {
                _currentBattleEnvironment = _environmentCache[dataEnvironmentData.Key];
                _currentBattleEnvironment.Show();
                return;
            }
            
            BattleEnvironment newEnvironment = _factory.Get(dataEnvironmentData.Prefab);
            _currentBattleEnvironment = newEnvironment;
            _currentBattleEnvironment.Show();
            
            _environmentCache.Add(dataEnvironmentData.Key, newEnvironment);
        }


        public void Cleanup()
        {
            foreach (var environment in _environmentCache)
            {
                environment.Value.Recycle();
            }
            
            _environmentCache.Clear();
        }
    }
}