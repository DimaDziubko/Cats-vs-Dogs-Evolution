using _Game.Core.Configs.Models;
using _Game.Core.UserState._Handler._Analytics;
using _Game.Core.UserState._Handler._BattleSpeed;
using _Game.Core.UserState._Handler._DailyTask;
using _Game.Core.UserState._Handler._FoodBoost;
using _Game.Core.UserState._Handler._Purchase;
using _Game.Core.UserState._Handler._Timeline;
using _Game.Core.UserState._Handler._Tutorial;
using _Game.Core.UserState._Handler._Upgrade;
using _Game.Core.UserState._Handler.Currencies;
using _Game.Core.UserState._Handler.FreeGemsPack;
using _Game.Core.UserState._State;

namespace _Game.Core.Services.UserContainer
{
    public interface IUserContainer 
    {
        UserAccountState State { get; set; }
        GameConfig GameConfig { get; set; }
        ICurrenciesHandler CurrenciesHandler { get; }
        ITimelineStateHandler TimelineStateHandler { get; }
        IAnalyticsStateHandler AnalyticsStateHandler { get; }
        IUpgradeStateHandler UpgradeStateHandler { get; }
        IPurchaseStateHandler PurchaseStateHandler  { get; }
        IFreeGemsPackStateHandler FreeGemsPackStateHandler  { get; }
        IFoodBoostStateHandler FoodBoostStateHandler { get; }
        IBattleSpeedStateHandler BattleSpeedStateHandler { get; }
        ITutorialStateHandler TutorialStateHandler { get; }
        IDailyTaskStateHandler DailyTaskStateHandler { get;}
    }
}