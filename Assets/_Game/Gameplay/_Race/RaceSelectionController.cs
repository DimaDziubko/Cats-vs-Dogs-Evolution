using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Gameplay.Common.Scripts;
using _Game.UI._RaceSelectionWindow.Scripts;
using _Game.Utils.Disposable;

namespace _Game.Gameplay._Race
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