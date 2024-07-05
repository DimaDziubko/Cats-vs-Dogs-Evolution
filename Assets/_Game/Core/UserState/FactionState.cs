using System;
using Assets._Game.Gameplay.Common.Scripts;

namespace Assets._Game.Core.UserState
{
    public class RaceState : IRaceStateReadonly
    {
        public Race CurrentRace;
        public int Counter;
        
        public event Action Changed;
        
        Race IRaceStateReadonly.CurrentRace => CurrentRace;
        int IRaceStateReadonly.Counter => Counter;

        public void Change(Race race)
        {
            CurrentRace = race;
            Counter++;
            Changed?.Invoke();
        }
    }

    public interface IRaceStateReadonly
    {
        event Action Changed;
        Race CurrentRace { get; }
        int Counter { get; }
    }
}