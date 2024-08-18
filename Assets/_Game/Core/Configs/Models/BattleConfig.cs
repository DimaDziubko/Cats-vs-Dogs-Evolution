using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "Configs/Battle")]
    [Serializable]
    public class BattleConfig : ScriptableObject
    {
        public int Id;
        public BattleScenario Scenario;
        public List<WarriorConfig> Warriors;
        public List<int> WarriorsId;
        public string EnvironmentKey;
        public float EnemyBaseHealth;
        public string AmbienceKey;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
        public AssetReferenceGameObject BasePrefab;
        public int Level;
    }
    
    public class RemoteBattleConfig
    {
        public int Id;
        public BattleScenario Scenario;
        public List<int> WarriorsId;
        public float EnemyBaseHealth;
        public float CoinsPerBase;
        public float MaxCoinsPerBattle;
    }
}