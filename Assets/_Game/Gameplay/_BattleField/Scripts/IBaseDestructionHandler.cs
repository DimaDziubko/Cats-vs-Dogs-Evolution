using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._BattleField.Scripts
{
    public interface IBaseDestructionHandler
    {
        void OnBaseDestructionStarted(Faction faction, Base @base);
        void OnBaseDestructionCompleted(Faction faction, Base @base);
    }
}