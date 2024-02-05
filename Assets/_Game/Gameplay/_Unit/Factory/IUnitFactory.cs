using _Game.Gameplay._Unit.Scripts;
using _Game.Gameplay.Common;

namespace _Game.Gameplay._Unit.Factory
{
    public interface IUnitFactory
    {
        Unit GetForPlayer(UnitType type);
        Unit GetForEnemy(int index);
        public void Reclaim(Unit unit);
    }
}