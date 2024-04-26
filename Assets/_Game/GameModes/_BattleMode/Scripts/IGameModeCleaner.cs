using System.Collections.Generic;
using _Game.Core.Factory;

namespace _Game.GameModes._BattleMode.Scripts
{
    public interface IGameModeCleaner
    {
        IEnumerable<GameObjectFactory> Factories { get; }
        string SceneName { get; }
        void Cleanup();
        void ResetGame();
    }
}