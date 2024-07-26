using System.Collections.Generic;
using _Game.Gameplay._Battle.Scripts;
using _Game.UI._Environment;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Core.Data.Battle
{
    public class BattleStaticData
    {
        public Dictionary<int, DataPool<UnitType, UnitData>> UnitDataPools;
        public Dictionary<int, DataPool<int, WeaponData>> WeaponDataPools;
        public Dictionary<int, BattleScenarioData> BattleDataPools;
        public Dictionary<int, BaseStaticData> BasePool;
        public Dictionary<int, float> BaseHealthPool;
        public Dictionary<int, EnvironmentData> EnvironmentPool;
        public Dictionary<int, AudioClip> AmbiencePool;
        
        public UnitData ForUnit(int battle, UnitType type) => UnitDataPools[battle].ForType(type);
        public WeaponData ForWeapon(int battle, int weaponId) => WeaponDataPools[battle].ForType(weaponId);
        public BaseStaticData ForBase(int battle) => BasePool[battle];
        public AudioClip ForAmbience(int battle) => AmbiencePool[battle];
        public EnvironmentData ForEnvironment(int battle) => EnvironmentPool[battle];
        public BattleScenarioData ForBattleScenario(int battle) => BattleDataPools[battle];
        public float ForBaseHealth(int battle) => BaseHealthPool[battle];

        public void Cleanup()
        {
            foreach (var pair in UnitDataPools) pair.Value.Cleanup();
            UnitDataPools.Clear();
            foreach (var pair in WeaponDataPools) pair.Value.Cleanup();
            WeaponDataPools.Clear();
            BattleDataPools.Clear();
            BasePool.Clear();
            BaseHealthPool.Clear();
            EnvironmentPool.Clear();
            AmbiencePool.Clear();
        }
    }
}