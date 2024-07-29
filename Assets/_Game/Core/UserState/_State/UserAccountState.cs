using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;
using _Game.Core.Services.Random;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.Core.UserState._State
{
    public class UserAccountState
    {
        public string Version;
        public int Id;

        public RaceState RaceState;
        public TimelineState TimelineState;
        public UserCurrenciesState Currencies;
        public FoodBoostState FoodBoost;
        public BattleStatistics BattleStatistics;
        public TutorialState TutorialState;
        public BattleSpeedState BattleSpeedState;
        public AdsStatistics AdsStatistics;
        public RetentionState RetentionState;
        public PurchaseDataState PurchaseDataState;
        public FreeGemsPackState FreeGemsPackState;
        
        public static UserAccountState GetInitial(
            IRandomService random)
        {
            return new UserAccountState()
            {
                Version = Application.version,
                Id = random.Next(1, int.MaxValue),
                
                RaceState = new RaceState()
                {
                    CurrentRace = Race.None
                },
                
                TimelineState = new TimelineState()
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
                },
                
                BattleStatistics = new BattleStatistics()
                {
                    BattlesCompleted = 0
                },
                
                TutorialState = new TutorialState()
                {
                    StepsCompleted = -1
                },
                
                BattleSpeedState = new BattleSpeedState()
                {
                    IsNormalSpeedActive = true,
                    PermanentSpeedId = 0,
                    DurationLeft = 0.0f
                },
                
                AdsStatistics = new AdsStatistics()
                {
                    AdsReviewed = 0
                },
                
                RetentionState = new RetentionState()
                {
                    FirstOpenTime = DateTime.UtcNow,
                    FirstDayRetentionEventSent = false,
                    SecondDayRetentionEventSent = false,
                },
                
                PurchaseDataState = new PurchaseDataState()
                {
                    BoudhtIAPs = new List<BoughtIAP>()
                },
                
                FreeGemsPackState = new FreeGemsPackState()
                {
                    FreeGemPackCount = 2,
                    LastFreeGemPackDay = DateTime.UtcNow
                },
            };
        }

        public bool IsValid() => Id > 0;
    }
}