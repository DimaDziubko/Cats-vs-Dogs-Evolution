using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._Cards;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Game.Utils.Extensions
{
    public static class JsonExtensions
    {
        public static TimelineConfig ForTimeline(this JObject jsonData, int timelineId = 0) =>
            Generate(jsonData, timelineId);

        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig()
            {
                CurrentTimeline = Generate(jsonData, timelineId),
                BattleSpeedConfigs = ExtractBattleSpeedConfig(jsonData),
                CommonConfig = ExtractCommonConfig(jsonData),
                FoodBoostConfig = ExtractFoodBoostConfig(jsonData),
                ShopConfig = ExtractShopConfig(jsonData),
                AdsConfig = ExtractAdsConfig(jsonData),
                GeneralDailyTaskConfig = ExtractGeneralDailyTaskConfig(jsonData),
                SummoningData = ExtractSummoning(jsonData),
                CardPricingConfig = ExtractCardsPricingConfig(jsonData),
                DifficultyConfig = ExtractDifficultyConfig(jsonData)
            };

            var (cardConfigsByType, cardConfigsById) = ExtractCardsConfig(jsonData);

            config.CardConfigsByType = cardConfigsByType;
            config.CardConfigsById = cardConfigsById;
            return config;
        }

        private static DifficultyConfig ExtractDifficultyConfig(JObject jsonData) => 
            Resources.Load<DifficultyConfig>(Constants.LocalConfigPath.DIFFICULTY_PATH);

        private static (Dictionary<CardType, List<CardConfig>>, Dictionary<int, CardConfig>) ExtractCardsConfig(JObject jsonData)
        {
            var cardsByType = new Dictionary<CardType, List<CardConfig>>();
            var cardsById = new Dictionary<int, CardConfig>();
            var cardsConfig = Resources.Load<CardsConfig>(Constants.LocalConfigPath.CARDS_CONFIG_PATH);

            foreach (var card in cardsConfig.CardConfigs)
            {
                if (!cardsByType.ContainsKey(card.Type))
                {
                    cardsByType[card.Type] = new List<CardConfig>();
                }
                cardsByType[card.Type].Add(card);
                
                cardsById[card.Id] = card;
            }

            return (cardsByType, cardsById);
        }
        
        private static CardsPricingConfig ExtractCardsPricingConfig(JObject jsonData) => 
            Resources.Load<CardsPricingConfig>(Constants.LocalConfigPath.CARDS_PRICING_PATH);

        private static List<RemoteWarriorConfig> ExtractWarriors(JObject jsonData)
        {
            var warriorsToken = jsonData[Constants.ConfigKeys.WARRIORS];
            if (warriorsToken == null)
            {
                Debug.LogError("WarriorsConfig is null");
                return null;
            }

            return warriorsToken.Select(warriorToken => warriorToken.ToObject<RemoteWarriorConfig>()).ToList();
        }

        private static List<RemoteAgeConfig> ExtractAges(JObject jsonData)
        {
            var agesToken = jsonData[Constants.ConfigKeys.AGES];
            if (agesToken == null)
            {
                Debug.LogError("AgeConfig is null");
                return null;
            }

            return agesToken.Select(ageToken => ageToken.ToObject<RemoteAgeConfig>()).ToList();
        }

        private static List<RemoteBattleConfig> ExtractBattles(JObject jsonData)
        {
            var battlesToken = jsonData[Constants.ConfigKeys.BATTLES];
            if (battlesToken == null)
            {
                Debug.LogError("BattleConfig is null");
                return null;
            }

            return battlesToken.Select(battleToken => battleToken.ToObject<RemoteBattleConfig>()).ToList();
        }

        private static AdsConfig ExtractAdsConfig(JObject jsonData)
        {
            var adsConfigToken = jsonData[Constants.ConfigKeys.ADS_CONFIG];
            if (adsConfigToken == null)
            {
                Debug.LogError("Ads config is null");
                return null;
            }

            return adsConfigToken.ToObject<AdsConfig>();
        }

        private static GeneralDailyTaskConfig ExtractGeneralDailyTaskConfig(JObject jsonData)
        {
            var generalDailyTaskToken = jsonData[Constants.ConfigKeys.GENERAL_DAILY_TASK_CONFIG];
            if (generalDailyTaskToken == null)
            {
                Debug.LogError("GeneralDailyTaskConfig is null");
                return null;
            }

            return generalDailyTaskToken.ToObject<GeneralDailyTaskConfig>();
        }
        
        private static ShopConfig ExtractShopConfig(JObject jsonData)
        {
            var shopToken = jsonData[Constants.ConfigKeys.SHOP_CONFIG];
            if (shopToken == null)
            {
                Debug.LogError("ShopConfig is null");
                return null;
            }

            return shopToken.ToObject<ShopConfig>();
        }

        private static FoodBoostConfig ExtractFoodBoostConfig(JObject jsonData)
        {
            var foodBoostToken = jsonData[Constants.ConfigKeys.FOOD_BOOST_CONFIG];
            if (foodBoostToken == null)
            {
                Debug.LogError("FoodBoostConfig is null");
                return null;
            }

            return foodBoostToken.ToObject<FoodBoostConfig>();
        }

        private static CommonConfig ExtractCommonConfig(JObject jsonData)
        {
            var commonConfig = Resources.Load<CommonConfig>(Constants.LocalConfigPath.COMMON_CONFIG_PATH);
            if (commonConfig == null)
            {
                Debug.LogError("CommonConfig is null");
            }
            return commonConfig;
        }

        private static List<BattleSpeedConfig> ExtractBattleSpeedConfig(JObject jsonData)
        {
            var battleSpeedTokens = jsonData[Constants.ConfigKeys.BATTLE_SPEEDS_CONFIGS];
            if (battleSpeedTokens == null)
            {
                Debug.LogError("BattleSpeedConfig is null");
                return null;
            }

            return battleSpeedTokens.Select(battleSpeedToken => battleSpeedToken.ToObject<BattleSpeedConfig>())
                .ToList();
        }

        private static TimelineConfig Generate(JObject jsonData, int timelineId)
        {
            TimelineConfig timelineConfig = new TimelineConfig
            {
                Id = timelineId,
                Ages = new List<AgeConfig>(),
                Battles = new List<BattleConfig>()
            };

            List<RemoteAgeConfig> remoteAgesData = ExtractAges(jsonData);
            GeneralAgesConfig ages = Resources.Load<GeneralAgesConfig>(Constants.LocalConfigPath.GENERAL_AGE_CONFIG_PATH);

            List<RemoteBattleConfig> remoteBattlesData = ExtractBattles(jsonData);
            GeneralBattlesConfig battles =
                Resources.Load<GeneralBattlesConfig>(Constants.LocalConfigPath.GENERAL_BATTLE_CONFIG_PATH);

            List<RemoteWarriorConfig> remoteWarriorsData = ExtractWarriors(jsonData);
            GeneralWarriorsConfig warriors =
                Resources.Load<GeneralWarriorsConfig>(Constants.LocalConfigPath.GENERAL_WARRIOR_CONFIG_PATH);

            for (int level = 1; level <= 6; level++)
            {
                List<AgeConfig> ageLevelGroup = ages.AgeConfigs.Where(x => x.Level == level).ToList();
                List<BattleConfig> battleLevelGroup = battles.BattleConfigs.Where(x => x.Level == level).ToList();

                int index = (timelineId < ageLevelGroup.Count)
                    ? timelineId
                    : (timelineId + level - 1) % ageLevelGroup.Count;

                AgeConfig age = ageLevelGroup[index];
                BattleConfig battle = battleLevelGroup[index];

                age = UpdateAgeConfigWithRemoteData(age, remoteAgesData, remoteWarriorsData, warriors);
                battle = UpdateBattleConfigWithRemoteData(battle, remoteBattlesData, remoteWarriorsData, warriors);

                timelineConfig.Ages.Add(age);
                timelineConfig.Battles.Add(battle);
            }

            return timelineConfig;
        }

        private static SummoningData ExtractSummoning(JObject jsonData)
        {
            var config = Resources.Load<SummoningConfigs>(Constants.LocalConfigPath.SUMMONING_CONFIG_PATH);
            int accumulated = 0;
            foreach (var summoning in config.CardsSummoningConfigs)
            {
                accumulated += summoning.CardsRequiredForLevel;
                summoning.AccumulatedCardsRequiredForLevel = accumulated;
            }
            
            var summoningToken = jsonData[Constants.ConfigKeys.SUMMONING_CONFIGS];
            SummoningConfigs remoteConfig = summoningToken?.ToObject<SummoningConfigs>();

            MergeSummonings(config, remoteConfig); 
            
            var summoningData = new SummoningData()
            {
                DropListsEnabled = config.DropListsEnabled,
                InitialDropList = config.InitialDropList,
                SummoningConfig = config.CardsSummoningConfigs.ToDictionary(x => x.Level, x => x)
                
            };

            return summoningData;
        }

        private static void MergeSummonings(SummoningConfigs local, SummoningConfigs remote)
        {
            local.DropListsEnabled = remote.DropListsEnabled;
            local.InitialDropList = remote.InitialDropList;
            
            var remoteCardSummoningsDict = remote.CardsSummoningConfigs.ToDictionary(x => x.Id);
            
            foreach (var cardsSummoning in local.CardsSummoningConfigs)
            {
                if (remoteCardSummoningsDict.TryGetValue(cardsSummoning.Id, out var remoteConfig))
                {
                    cardsSummoning.DropList = remoteConfig.DropList;
                }
            }
        }

        private static AgeConfig UpdateAgeConfigWithRemoteData(AgeConfig age, List<RemoteAgeConfig> remoteAgesData,
            List<RemoteWarriorConfig> remoteWarriorsData, GeneralWarriorsConfig warriors)
        {
            var remoteAgeData = remoteAgesData.First(x => x.Id == age.Id);
            age.Economy = remoteAgeData.Economy;
            age.WarriorsId = remoteAgeData.WarriorsId;
            age.GemsPerAge = remoteAgeData.GemsPerAge;
            age.Price = remoteAgeData.Price;

            var relevantWarriors = GetRelevantWarriors(age.WarriorsId, remoteWarriorsData, warriors);
            age.Warriors = relevantWarriors;

            return age;
        }

        private static BattleConfig UpdateBattleConfigWithRemoteData(BattleConfig battle,
            List<RemoteBattleConfig> remoteBattlesData, List<RemoteWarriorConfig> remoteWarriorsData,
            GeneralWarriorsConfig warriors)
        {
            var remoteBattleData = remoteBattlesData.First(x => x.Id == battle.Id);
            battle.Scenario = remoteBattleData.Scenario;
            battle.WarriorsId = remoteBattleData.WarriorsId;
            battle.CoinsPerBase = remoteBattleData.CoinsPerBase;
            battle.MaxCoinsPerBattle = remoteBattleData.MaxCoinsPerBattle;
            battle.EnemyBaseHealth = remoteBattleData.EnemyBaseHealth;

            var relevantWarriors = GetRelevantWarriors(battle.WarriorsId, remoteWarriorsData, warriors);
            battle.Warriors = relevantWarriors;

            return battle;
        }

        private static List<WarriorConfig> GetRelevantWarriors(List<int> warriorIds,
            List<RemoteWarriorConfig> remoteWarriorsData, GeneralWarriorsConfig warriors)
        {
            return warriorIds
                .Select(id => warriors.WarriorConfigs.FirstOrDefault(w => w.Id == id))
                .Where(warrior => warrior != null)
                .Select(relevantWarrior => UpdateWarriorConfigWithRemoteData(relevantWarrior, remoteWarriorsData))
                .ToList();
        }

        private static WarriorConfig UpdateWarriorConfigWithRemoteData(WarriorConfig warrior,
            List<RemoteWarriorConfig> remoteWarriorsData)
        {
            var remoteWarriorData = remoteWarriorsData.First(w => w.Id == warrior.Id);
            warrior.Health = remoteWarriorData.Health;
            warrior.Price = remoteWarriorData.Price;
            warrior.FoodPrice = remoteWarriorData.FoodPrice;
            warrior.Speed = remoteWarriorData.Speed;
            warrior.AttackPerSecond = remoteWarriorData.AttackPerSecond;
            warrior.AttackDistance = remoteWarriorData.AttackDistance;
            warrior.CoinsPerKill = remoteWarriorData.CoinsPerKill;
            warrior.EnemyHealthMultiplier = remoteWarriorData.EnemyHealthMultiplier;
            warrior.PlayerHealthMultiplier = remoteWarriorData.PlayerHealthMultiplier;

            warrior.WeaponConfig.Damage = remoteWarriorData.WeaponConfig.Damage;
            warrior.WeaponConfig.ProjectileSpeed = remoteWarriorData.WeaponConfig.ProjectileSpeed;
            warrior.WeaponConfig.SplashRadius = remoteWarriorData.WeaponConfig.SplashRadius;
            warrior.WeaponConfig.EnemyDamageMultiplier = remoteWarriorData.WeaponConfig.EnemyDamageMultiplier;
            warrior.WeaponConfig.PlayerDamageMultiplier = remoteWarriorData.WeaponConfig.PlayerDamageMultiplier;
            warrior.WeaponConfig.TrajectoryWarpFactor = remoteWarriorData.WeaponConfig.TrajectoryWarpFactor;

            return warrior;
        }
    }
}