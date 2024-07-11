using _Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._Units.Factory
{
    public interface IUnitFactory
    {
        Unit Get(Faction faction, UnitType type);
        public void Reclaim(Unit unit);
    }
}