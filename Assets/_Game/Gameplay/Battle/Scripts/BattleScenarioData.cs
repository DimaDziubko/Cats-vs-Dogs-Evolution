using Assets._Game.Core.Configs.Models;
using Assets._Game.UI._Environment;
using UnityEngine;

namespace Assets._Game.Gameplay.Battle.Scripts
{
    public class BattleData
    {
        public int Battle;
        public AudioClip Ambience;
        public EnvironmentData EnvironmentData;
        public BattleScenarioData ScenarioData;
    }
    
    public class BattleScenarioData
    {
        public BattleScenario Scenario;
        public float MaxCoinsPerBattle;
        public BattleAnalyticsData AnalyticsData;
    }

    public struct BattleAnalyticsData
    {
        public int TimelineNumber;
        public int AgeNumber;
        public int BattleNumber;
    }
}