using _Game.Core.Configs.Models;
using _Game.Gameplay._Bases.Scripts;
using _Game.UI._Environment;
using UnityEngine;

namespace _Game.Gameplay.Battle.Scripts
{
    public class BattleData
    {
        public int Battle;
        public BattleScenario Scenario;
        public BaseData EnemyBaseData;
        public AudioClip BGM;
        public float MaxCoinsPerBattle;
        public EnvironmentData EnvironmentData;
    }
}