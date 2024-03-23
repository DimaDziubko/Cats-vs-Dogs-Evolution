using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitSpawner
    {
        void SpawnEnemy(UnitType type);
        void SpawnPlayerUnit(UnitType type);
    }
}