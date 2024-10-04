using System;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitSpawner
    {
        event Action<Faction, UnitType> UnitSpawned;
        event Action<Faction, UnitType> UnitDead;
        void SpawnEnemy(UnitType type);
        void SpawnPlayerUnit(UnitType type);
    }
}