using System;

namespace _Game.Core.UserState._Handler.FreeGemsPack
{
    public interface IFreeGemsPackStateHandler
    {
        void RecoverFreeGemsPack(int packsToAdd, DateTime newLastDailyFreePackSpent);
        void SpendGemsPack(DateTime lastDailyGemsPack);
    }
}