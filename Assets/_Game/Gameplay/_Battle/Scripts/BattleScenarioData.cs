using _Game.Core.Configs.Models;
using _Game.UI._Environment;
using Assets._Game.UI._Environment;
using UnityEngine;

namespace _Game.Gameplay._Battle.Scripts
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