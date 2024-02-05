using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;

namespace _Game.Core.UserState
{
    public class UserContainer : IPersistentDataService
    {
        public UserAccountState State { get; set; }
        public AppConfiguration Configuration { get; set; }

        public GameConfig GameConfig { get; set; }

        public void AddCoins(in int count)
        {
            State.Currencies.ChangeCoins(count);
        }

        public void PurchaseUnit(int unitIndex, float price)
        {
            if (State.Currencies.Coins >= price)
            {
                ChangeAfterPurchase(price, false);
                State.TimelineState.OpenUnit(unitIndex);
            }
        }

        private void ChangeAfterPurchase(float price, bool isPositive)
        {
            price = isPositive ? price : (price * -1);
            State.Currencies.ChangeCoins(price);
        }
    }
}