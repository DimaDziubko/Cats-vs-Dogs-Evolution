using Assets._Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay.Common.Scripts;
using Assets._Game.UI._RaceSelectionWindow.Scripts;
using Assets._Game.Utils.Disposable;

namespace Assets._Game.Gameplay._Race
{
    public class RaceSelectionController
    {
        private readonly IRaceSelectionWindowProvider _raceSelectionWindowProvider;
        private readonly IUserContainer _persistentData;
        
        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;
        
        public RaceSelectionController(
            IUserContainer persistentData,
            IRaceSelectionWindowProvider raceSelectionWindowProvider)
        {
            _persistentData = persistentData;
            _raceSelectionWindowProvider = raceSelectionWindowProvider;
        }

        public void Init()
        {
            if (RaceState.CurrentRace == Race.None)
            {
                AskForRace();
            }
        }

        private async void AskForRace()
        {
            Disposable<RaceSelectionWindow> factionSelectionWindow = await _raceSelectionWindowProvider.Load();
            var result = await factionSelectionWindow.Value.AwaitForDecision();
            if(result)
                factionSelectionWindow.Dispose();
        }
    }
}