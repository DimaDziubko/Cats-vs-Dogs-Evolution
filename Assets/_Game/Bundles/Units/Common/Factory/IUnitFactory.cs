using _Game.Bundles.Units.Common.Scripts;

namespace _Game.Bundles.Units.Common.Factory
{
    public interface IUnitFactory
    {
        Unit GetForPlayer(UnitType type);
        Unit GetForEnemy(UnitType type);
        public void Reclaim(Unit unit);
    }
}