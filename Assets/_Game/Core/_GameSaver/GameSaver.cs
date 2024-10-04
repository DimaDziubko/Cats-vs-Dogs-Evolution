using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using _Game.Core.Communication;
using _Game.Core.Services.UserContainer;
using UnityEngine;

namespace _Game.Core._GameSaver
{
    
    public class GameSaver : 
        IGameSaver,
        IStopBattleListener
    {
        private readonly IUserStateCommunicator _communicator;
        private readonly IUserContainer _userContainer;

        private readonly List<ISaveGameTrigger> _triggers = new List<ISaveGameTrigger>();

        private readonly float _debounceTime = 3.0f;
        private float _lastSaveTime;

        public GameSaver(
            IUserStateCommunicator communicator,
            IUserContainer userContainer)
        {
            _communicator = communicator;
            _userContainer = userContainer;
        }

        public void Register(ISaveGameTrigger trigger)
        {
            trigger.SaveGameRequested += SaveGameRequested;
            _triggers.Add(trigger);
        }

        public void Unregister(ISaveGameTrigger trigger)
        {
            trigger.SaveGameRequested -= SaveGameRequested;
            _triggers.Remove(trigger);
        }

        void IStopBattleListener.OnStopBattle() => SaveGame();

        private void SaveGameRequested(bool isDebounced)
        {
            if(isDebounced) DebounceSaveGame();
            else
                SaveGame();
        }

        private void DebounceSaveGame()
        {
            if (Time.time - _lastSaveTime >= _debounceTime)
            {
                SaveGame();
                _lastSaveTime = Time.time;
            }
        }

        private void SaveGame() => 
            _communicator.SaveUserState(_userContainer.State);
        
    }
}