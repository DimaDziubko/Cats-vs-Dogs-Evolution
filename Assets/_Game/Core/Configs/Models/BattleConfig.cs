using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class BattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Enemies;
        public string EnvironmentKey;
        public float EnemyBaseHealth;
        public string AmbienceKey;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
        public string BaseKey;
    }
}