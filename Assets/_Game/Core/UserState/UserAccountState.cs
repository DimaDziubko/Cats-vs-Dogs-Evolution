﻿using System;
using System.Collections.Generic;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay.Common.Scripts;

namespace _Game.Core.UserState
{
    public class UserAccountState
    {
        public int Version;
        public int Id;

        public RaceState RaceState;
        
        public UserTimelineState TimelineState;
        public UserCurrenciesState Currencies;
        public FoodBoostState FoodBoost;

        
        //TODO Check later
        public static UserAccountState GetInitial(
            IRandomService random)
        {
            
            //TODO Check index
            return new UserAccountState()
            {
                Version = 0,
                Id = random.Next(1, int.MaxValue),
                
                RaceState = new RaceState()
                {
                    CurrentRace = Race.None
                },
                
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
                
                FoodBoost = new FoodBoostState()
                {
                    DailyFoodBoostCount = 2,
                    LastDailyFoodBoost = DateTime.UtcNow
                }
            };
        }

        public bool IsValid() => Id > 0;
    }
}