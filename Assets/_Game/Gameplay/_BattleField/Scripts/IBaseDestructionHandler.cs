using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IBaseDestructionHandler
    {
        void OnBaseDestructionStarted(Faction faction, Base @base);
        void OnBaseDestructionCompleted(Faction faction, Base @base);
    }
}