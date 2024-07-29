﻿using System;
using _Game.Core.Services.UserContainer;
using Assets._Game.Gameplay.Common.Scripts;

namespace _Game.Core.Configs.Repositories.Common
{
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
                    return GetCatFoodIconKey();
                case Race.Dog:
                    return GetDogFoodIconKey();
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }
        }

        private string GetDogFoodIconKey()
        {
            return _userContainer
                .GameConfig
                .CommonConfig
                .DogFoodIconKey;
        }

        private string GetCatFoodIconKey()
        {
            return _userContainer
                .GameConfig
                .CommonConfig
                .CatFoodIconKey;
        }

        public string GetBaseIconKey() => 
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