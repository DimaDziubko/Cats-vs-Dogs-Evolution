using System.Collections.Generic;
using Assets._Game.Gameplay._Bases.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Gameplay._BattleField.Scripts
{
    public interface IBaseDestructionManager
    {
        void BaseDestructionStarted(Faction faction, Base @base);
        void BaseDestructionCompleted(Faction faction, Base @base);
        void Register(IBaseDestructionHandler handler);
        void UnRegister(IBaseDestructionHandler handler);
    }

    public class BaseDestructionManager : IBaseDestructionManager
    {
        private readonly List<IBaseDestructionHandler> _handlers = new List<IBaseDestructionHandler>();

        public void Register(IBaseDestructionHandler handler)
        {
            _handlers.Add(handler);
        }

        public void UnRegister(IBaseDestructionHandler handler)
        {
            _handlers.Remove(handler);
        }
        
        public void BaseDestructionStarted(Faction faction, Base @base)
        {
            foreach (var handler in _handlers)
            {
                handler.OnBaseDestructionStarted(faction, @base);
            }
        }

        public void BaseDestructionCompleted(Faction faction, Base @base)
        {
            foreach (var handler in _handlers)
            {
                handler.OnBaseDestructionCompleted(faction, @base);
            }
        }
    }
}