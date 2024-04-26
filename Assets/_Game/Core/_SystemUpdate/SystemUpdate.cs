using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core._SystemUpdate
{
    public class SystemUpdate : MonoBehaviour, ISystemUpdate
    {
        private readonly List<IGameUpdate> _updateSystems = new List<IGameUpdate>();
        //private readonly List<IGameInit> _initSystems = new List<IGameInit>();

        public void Register(ISystem system)
        {
            if (system is IGameUpdate updateSystem)
            {
                _updateSystems.Add(updateSystem);
            }
            
            // if (system is IGameInit initSystem)
            // {
            //     _initSystems.Add(initSystem);
            // }
        }

        public void Unregister(ISystem system)
        {
            if (system is IGameUpdate updateSystem)
            {
                _updateSystems.Remove(updateSystem);
            }
            
            // if (system is IGameInit initSystem)
            // {
            //     _initSystems.Remove(initSystem);
            // }
        }
        
        private void Update()
        {
            foreach (var system in _updateSystems)
            {
                system.GameUpdate();
            }
        }
    }

    // internal interface IGameInit : ISystem
    // {
    //     void Init();
    // }

    public interface ISystemUpdate
    {
        void Register(ISystem system);
        void Unregister(ISystem system);
    }
    
    public interface IGameUpdate : ISystem
    {
        void GameUpdate();
    }

    public interface ISystem
    {
    }
}