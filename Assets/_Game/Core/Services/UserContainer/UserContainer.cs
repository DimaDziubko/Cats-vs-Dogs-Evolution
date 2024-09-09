using System;
using _Game.Core.Configs.Models;
using _Game.Core.Debugger;
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
    public class UserContainer : IUserContainer, ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        
        public UserAccountState State { get; set; }
        public GameConfig GameConfig { get; set; }

        public ITimelineStateHandler TimelineStateHandler { get; }
        public ICurrenciesHandler CurrenciesHandler { get; }
        public IAnalyticsStateHandler AnalyticsStateHandler { get; }
        public IUpgradeStateHandler UpgradeStateHandler { get; }
        public IPurchaseStateHandler PurchaseStateHandler  { get; }
        public IFreeGemsPackStateHandler FreeGemsPackStateHandler  { get; }
        public IFoodBoostStateHandler FoodBoostStateHandler { get; }
        public IBattleSpeedStateHandler BattleSpeedStateHandler { get; }
        public ITutorialStateHandler TutorialStateHandler { get; }
        public IDailyTaskStateHandler DailyTaskStateHandler { get; }


        public UserContainer()
        {
            //debugger.UserContainer = this;
            CurrenciesHandler = new CurrenciesHandler(this);
            TimelineStateHandler = new TimelineStateHandler(this);
            AnalyticsStateHandler = new AnalyticsStateHandler(this);
            UpgradeStateHandler = new UpgradeStateHandler(this);
            PurchaseStateHandler = new PurchaseStateHandler(this);
            FreeGemsPackStateHandler = new FreeGemsPackStateHandler(this);
            FoodBoostStateHandler = new FoodBoostStateHandler(this);
            BattleSpeedStateHandler = new BattleSpeedStateHandler(this);
            TutorialStateHandler = new TutorialStateHandler(this);
            DailyTaskStateHandler = new DailyTaskStateHandler(this);
        }
        
        public void RequestSaveGame(bool isDebounced = false) => SaveGameRequested?.Invoke(isDebounced);
    }
}