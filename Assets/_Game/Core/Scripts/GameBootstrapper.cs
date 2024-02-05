using _Game.Core.GameState;
using UnityEngine;
using Zenject;

namespace _Game.Core.Scripts
{
    public class GameBootstrapper : MonoBehaviour
    {
        private GameStateMachine _gameStateMachine;
    
        [Inject]
        public void Construct(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        
        private void Start()
        {
            _gameStateMachine.Initialize();
            _gameStateMachine.Enter<BootstrapState>();
        }
    }
}