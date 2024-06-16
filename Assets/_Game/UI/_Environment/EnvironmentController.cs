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
        
        public EnvironmentController(IEnvironmentFactory factory) => 
            _factory = factory;

        public void ShowEnvironment(EnvironmentData environmentData)
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