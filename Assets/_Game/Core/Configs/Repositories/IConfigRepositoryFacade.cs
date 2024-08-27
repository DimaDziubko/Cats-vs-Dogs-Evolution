using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Configs.Repositories.Timeline;

namespace _Game.Core.Configs.Repositories
{
    public interface IConfigRepositoryFacade
    {
        IAdsConfigRepository AdsConfigRepository { get; }
        ICardsConfigRepository CardsConfigRepository { get; }
        ITimelineConfigRepository TimelineConfigRepository { get; }
        ICommonItemsConfigRepository CommonItemsConfigRepository { get; }
        IAgeConfigRepository AgeConfigRepository { get; }
        IBattleSpeedConfigRepository BattleSpeedConfigRepository { get; }
        IEconomyConfigRepository EconomyConfigRepository { get; }
        IDailyTaskConfigRepository DailyTaskConfigRepository { get; }
        IShopConfigRepository ShopConfigRepository { get; }
    }
}