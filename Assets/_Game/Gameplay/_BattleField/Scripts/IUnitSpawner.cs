using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._BattleField.Scripts
{
    public interface IUnitSpawner
    {
        void SpawnEnemy(UnitType type);
        void SpawnPlayerUnit(UnitType type);
    }
}