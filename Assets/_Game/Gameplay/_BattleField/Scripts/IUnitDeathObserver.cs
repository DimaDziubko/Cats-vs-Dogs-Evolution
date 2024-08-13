using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitDeathObserver
    {
        void Notify(Faction faction, UnitType type);
    }
}