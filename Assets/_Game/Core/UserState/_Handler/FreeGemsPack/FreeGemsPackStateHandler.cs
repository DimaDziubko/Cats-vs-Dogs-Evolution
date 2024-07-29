using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler.FreeGemsPack
{
    public class FreeGemsPackStateHandler : IFreeGemsPackStateHandler
    {
        private readonly IUserContainer _userContainer;

        public FreeGemsPackStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void RecoverFreeGemsPack(int packsToAdd, DateTime newLastDailyFreePackSpent) => 
            ChangeGemsPack(packsToAdd, true, newLastDailyFreePackSpent);

        public void SpendGemsPack(DateTime lastDailyGemsPack) => 
            ChangeGemsPack(1, false, lastDailyGemsPack);

        private void ChangeGemsPack(int delta, bool isPositive, DateTime lastDailyGemsPack)
        {
            delta = isPositive ? delta : (delta * -1);
            _userContainer.State.FreeGemsPackState.ChangeFreeGemPackCount(delta, lastDailyGemsPack);
        }
    }
}