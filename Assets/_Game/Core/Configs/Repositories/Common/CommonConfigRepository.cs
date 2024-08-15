using System;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core._Logger;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Repositories.Common
{
    public class CommonItemsConfigRepository : ICommonItemsConfigRepository
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        public CommonItemsConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public string GetFoodIconKey(Race race)
        {
            switch (race)
            {
                case Race.None:
                    return GetCommonFoodIconReference();
                case Race.Cat:
                    return GetCatFoodIconReference();
                case Race.Dog:
                    return GetDogFoodIconReference();
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }

        private string GetDogFoodIconReference()
        {
            _logger.Log("Try to GetDogFoodIconReference");
            return _userContainer
                .GameConfig
                .CommonConfig
                .DogFoodIconKey;
        }

        private string GetCatFoodIconReference()
        {
            _logger.Log("Try to GetCatFoodIconReference");
            return _userContainer
                .GameConfig
                .CommonConfig
                .CatFoodIconKey;
        }

        public string GetBaseIconKey()
        {
            _logger.Log("Try to GetBaseIconReference");
            return _userContainer
                .GameConfig
                .CommonConfig
                .BaseIconKey;
        }

        private string GetCommonFoodIconReference()
        {
            _logger.Log("Try to GetCommonFoodIconReference");
            return _userContainer
                .GameConfig
                .CommonConfig
                .FoodIconKey;
        }
    }
}