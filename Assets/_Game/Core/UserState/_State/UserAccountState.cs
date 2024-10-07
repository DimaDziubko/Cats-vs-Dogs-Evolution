using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;
using _Game.Core.Services.Random;
using _Game.Gameplay._Units.Scripts;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay.Common.Scripts;
using Sirenix.OdinInspector;
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
        public TasksState TasksState;
        public DailyTasksState DailyTasksState;
        public AdsWeeklyWatchState AdsWeeklyWatchState;
        public CardsCollectionState CardsCollectionState;
        
        [ShowInInspector]
        public FreeGemsPackContainer FreeGemsPackContainer;
        
        [ShowInInspector]
        public AdsGemsPackContainer AdsGemsPackContainer;
        
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
                    Coins = 0,
                    Gems = 1000
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
                    CompletedSteps = new List<int>()
                    {
                        -1
                    }
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

                DailyTasksState = new DailyTasksState()
                {
                    ProgressOnTask = 0,
                    CompletedTasks = new List<int>(),
                    CurrentTaskIdx = -1,
                    LastTimeGenerated = DateTime.Now
                },
                
                TasksState = new TasksState()
                {
                    TotalCompletedTasks = 0
                },
                
                AdsWeeklyWatchState = new AdsWeeklyWatchState()
                {
                    LastWeekAdsWatched = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 },
                    LastDay = DateTime.Today
                },
                
                CardsCollectionState = new CardsCollectionState()
                {
                    CardSummoningLevel = 1,
                    CardsSummoningProgressCount = 0,
                    Cards = new List<Card>(),
                },
                
                FreeGemsPackContainer =  new FreeGemsPackContainer(),
                AdsGemsPackContainer = new AdsGemsPackContainer()
            };
        }

        public bool IsValid() => Id > 0;
    }
}