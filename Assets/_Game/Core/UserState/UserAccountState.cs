using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Core.UserState
{
    public class UserAccountState
    {
        public int Version;
        public int Id;

        public UserTimelineState TimelineState;
        public UserCurrenciesState Currencies;

        //TODO Check later
        public static UserAccountState GetInitial(
            IRandomService random, 
            GameConfig gameConfig)
        {
            
            //TODO Check index
            return new UserAccountState()
            {
                Version = 0,
                Id = random.Next(1, int.MaxValue),
                
                TimelineState = new UserTimelineState()
                {
                    TimelineId = 0,
                    AgeId = 0,
                    AllBattlesWon = false,
                    BaseHealthLevel = 0,
                    FoodProductionLevel = 0,
                    MaxBattle = 0,
                    OpenUnits = new List<UnitType>(3)
                    {
                        UnitType.Light,
                    }
                },
                Currencies = new UserCurrenciesState()
                {
                    Coins = 0
                },
            };
        }

        public bool IsValid() => Id > 0;
    }
}