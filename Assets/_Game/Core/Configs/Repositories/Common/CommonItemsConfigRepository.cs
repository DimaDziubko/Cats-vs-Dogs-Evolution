using System;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Boosts.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

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

        public string  GetFoodIconKey(Race race)
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

        private string GetDogFoodIconReference() =>
            _userContainer
                .GameConfig
                .CommonConfig
                .DogFoodIconKey;

        private string GetCatFoodIconReference() =>
            _userContainer
                .GameConfig
                .CommonConfig
                .CatFoodIconKey;

        public string GetBaseIconKey() =>
            _userContainer
                .GameConfig
                .CommonConfig
                .BaseIconKey;

        public Sprite ForFoodIcon(Race race) => 
            _userContainer.GameConfig.CommonConfig.GetFoodIconForRace(race);

        public Sprite ForBaseIcon() => 
            _userContainer.GameConfig.CommonConfig.BaseIcon;

        public Sprite ForBoostIcon(BoostType boostType) => 
            _userContainer.GameConfig.CommonConfig.GetBoostIconFor(boostType);

        public Sprite GetUnitAttackIconFor(Race race) => 
            _userContainer.GameConfig.CommonConfig.GetAttackIconForRace(race);

        public Sprite GetUnitHealthIconFor(Race race) => 
            _userContainer.GameConfig.CommonConfig.GetHealthIconForRace(race);


        private string  GetCommonFoodIconReference()
        {
            _logger.Log("Try to GetCommonFoodIconReference");
            return _userContainer
                .GameConfig
                .CommonConfig
                .FoodIconKey;
        }
    }
}