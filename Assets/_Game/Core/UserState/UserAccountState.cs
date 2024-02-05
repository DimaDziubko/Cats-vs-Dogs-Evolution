using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.Random;

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
                    MaxBattle = 3,
                    OpenUnits = new List<int>(3)
                    {
                        0
                    }
                },
                Currencies = new UserCurrenciesState()
                {
                    Coins = 2000
                },
            };
        }

        public bool IsValid() => Id > 0;
    }
}