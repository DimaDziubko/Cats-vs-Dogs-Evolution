using _Game.Core._Logger;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories._Ads;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories.Age;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public class ConfigRepositoryFacade : IConfigRepositoryFacade
    {
        public IAdsConfigRepository AdsConfigRepository { get; }
        public ICardsConfigRepository CardsConfigRepository { get; }
        public ITimelineConfigRepository TimelineConfigRepository { get; }
        public ICommonItemsConfigRepository CommonItemsConfigRepository { get; }
        public IAgeConfigRepository AgeConfigRepository { get; }
        public IBattleSpeedConfigRepository BattleSpeedConfigRepository { get; }
        public IEconomyConfigRepository EconomyConfigRepository { get; }
        public IDailyTaskConfigRepository DailyTaskConfigRepository { get; }
        public IShopConfigRepository ShopConfigRepository { get; }
        public IDifficultyConfigRepository DifficultyConfigRepository { get; }

        public ConfigRepositoryFacade(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            AdsConfigRepository = new AdsConfigRepository(userContainer);
            CardsConfigRepository = new CardsConfigRepository(userContainer);
            TimelineConfigRepository = new TimelineConfigRepository(userContainer);
            AgeConfigRepository = new AgeConfigRepository(TimelineConfigRepository, logger);
            CommonItemsConfigRepository = new CommonItemsConfigRepository(userContainer, logger);
            BattleSpeedConfigRepository = new BattleSpeedConfigRepository(userContainer);
            EconomyConfigRepository = new EconomyConfigRepository(userContainer, AgeConfigRepository);
            DailyTaskConfigRepository = new DailyTaskConfigRepository(userContainer);
            ShopConfigRepository = new ShopConfigRepository(userContainer);
            DifficultyConfigRepository = new DifficultyConfigRepository(userContainer);
        }
        
    }
}