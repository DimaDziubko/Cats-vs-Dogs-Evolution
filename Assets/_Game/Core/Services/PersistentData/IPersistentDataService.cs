using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Configs.Models;
using _Game.Core.UserState;

namespace _Game.Core.Services.PersistentData
{
    public interface IPersistentDataService 
    {
        UserAccountState State { get; set; }
        AppConfiguration Configuration { get; set; }
        GameConfig GameConfig { get; set; }
        void AddCoins(in int award);
        void PurchaseUnit(UnitType type, float price);
    }
}