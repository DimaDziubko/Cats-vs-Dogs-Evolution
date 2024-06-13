using System;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Core.Configs.Repositories
{
    public interface ICommonItemsConfigRepository
    {
        public string GetFoodIconKey(Race race);
        public string GetTowerIconKey();
    }

    public class CommonItemsConfigRepository : ICommonItemsConfigRepository
    {
        private readonly IUserContainer _userContainer;

        public CommonItemsConfigRepository(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public string GetFoodIconKey(Race race)
        {
            switch (race)
            {
                case Race.None:
                    return GetCommonFoodIconKey();
                case Race.Cat:
                    return GetCommonFoodIconKey();
                case Race.Dog:
                    return GetCommonFoodIconKey();
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }

        public string GetTowerIconKey() => 
            _userContainer
                .GameConfig
                .CommonConfig
                .BaseIconKey;

        private string GetCommonFoodIconKey()
        {
            return _userContainer
                .GameConfig
                .CommonConfig
                .FoodIconKey;
        }
    }
}