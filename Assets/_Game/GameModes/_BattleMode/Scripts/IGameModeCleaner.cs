using System.Collections.Generic;
using Assets._Game.Core.Factory;

namespace Assets._Game.GameModes._BattleMode.Scripts
{
    public interface IGameModeCleaner
    {
        IEnumerable<GameObjectFactory> Factories { get; }
        string SceneName { get; }
        void Cleanup();
        void ResetGame();
    }
}