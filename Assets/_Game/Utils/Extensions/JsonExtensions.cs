using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Configs.Models;
using Newtonsoft.Json.Linq;

namespace _Game.Utils.Extensions
{
    public static class JsonExtensions
    {
        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig();
            
            var timelineToken = jsonData[Constants.ConfigKeys.TIMELINE]?
                .FirstOrDefault(t => (int)t[Constants.ConfigKeys.ID] == timelineId);
            
            if (timelineToken != null)
            {
                config.Timeline = ParseTimeline(timelineToken, jsonData);
            }

            return config;
        }

        private static TimelineConfig ParseTimeline(JToken timelineToken, JObject jsonData)
        {
            var timelineConfig = new TimelineConfig
            {
                Id = (int)timelineToken[Constants.ConfigKeys.ID],
                Ages = ParseAges(timelineToken[Constants.ConfigKeys.AGES], jsonData),
                Battles = ParseBattles(timelineToken[Constants.ConfigKeys.BATTLES], jsonData),
            };

            return timelineConfig;
        }

        private static List<AgeConfig> ParseAges(JToken agesToken, JObject jsonData)
        {
            var ages = new List<AgeConfig>();
            foreach (var ageId in agesToken)
            {
                var ageToken = jsonData[Constants.ConfigKeys.AGE]?
                    .FirstOrDefault(a => (int)a[Constants.ConfigKeys.ID] == (int)ageId);
                if (ageToken != null)
                {
                    var ageConfig = new AgeConfig
                    {
                        Id = (int)ageToken[Constants.ConfigKeys.ID],
                        Price = (float)ageToken[Constants.ConfigKeys.PRICE],
                        GemsPerAge = (float)ageToken[Constants.ConfigKeys.GEMS_PER_AGE],
                        Economy = ParseEconomy((int)ageToken[Constants.ConfigKeys.ECONOMY], jsonData),
                        Warriors = ParseWarriors(ageToken[Constants.ConfigKeys.WARRIORS], jsonData),
                        FoodIconKey = ageToken[Constants.ConfigKeys.FOOD_ICON_KEY]?.ToString(),
                    };
                    ages.Add(ageConfig);
                }
            }
            return ages;
        }

       private static EconomyConfig ParseEconomy(int economyId, JObject jsonData)
        {
            var economyToken = jsonData[Constants.ConfigKeys.ECONOMY]?
                .FirstOrDefault(e => (int)e[Constants.ConfigKeys.ID] == economyId);
            if (economyToken != null)
            {
                return new EconomyConfig
                {
                    Id = (int)economyToken[Constants.ConfigKeys.ID],
                    CoinPerBattle = (int)economyToken[Constants.ConfigKeys.COINS_PER_BATTLE],
                    FoodProduction = ParseFoodProduction((int)economyToken[Constants.ConfigKeys.FOOD_PRODUCTION], jsonData),
                    BaseHealth = ParseBaseHealth((int)economyToken[Constants.ConfigKeys.BASE_HEALTH], jsonData),
                };
            }
            return null;
        }

        private static FoodProductionConfig ParseFoodProduction(int foodProductionId, JObject jsonData)
        {
            var foodProductionToken = jsonData[Constants.ConfigKeys.FOOD_PRODUCTION]?
                .FirstOrDefault(f => (int)f[Constants.ConfigKeys.ID] == foodProductionId);
            if (foodProductionToken != null)
            {
                return new FoodProductionConfig
                {
                    Id = (int)foodProductionToken[Constants.ConfigKeys.ID],
                    Price = (float)foodProductionToken[Constants.ConfigKeys.PRICE],
                    PriceFactor = (float)foodProductionToken[Constants.ConfigKeys.PRICE_FACTOR],
                    Speed = (float)foodProductionToken[Constants.ConfigKeys.SPEED],
                    SpeedFactor = (float)foodProductionToken[Constants.ConfigKeys.SPEED_FACTOR]
                };
            }
            return null;
        }

        private static BaseHealthConfig ParseBaseHealth(int baseHealthId, JObject jsonData)
        {
            var baseHealthToken = jsonData[Constants.ConfigKeys.BASE_HEALTH]?
                .FirstOrDefault(b => (int)b[Constants.ConfigKeys.ID] == baseHealthId);
            if (baseHealthToken != null)
            {
                return new BaseHealthConfig
                {
                    Id = (int)baseHealthToken[Constants.ConfigKeys.ID],
                    Price = (float)baseHealthToken[Constants.ConfigKeys.PRICE],
                    PriceFactor = (float)baseHealthToken[Constants.ConfigKeys.PRICE_FACTOR],
                    Health = (float)baseHealthToken[Constants.ConfigKeys.HEALTH],
                    HealthFactor = (float)baseHealthToken[Constants.ConfigKeys.HEALTH_FACTOR]
                };
            }
            return null;
        }

        private static List<BattleConfig> ParseBattles(JToken battlesToken, JObject jsonData)
        {
            var battles = new List<BattleConfig>();
            foreach (var battleId in battlesToken)
            {
                var battleToken = jsonData[Constants.ConfigKeys.BATTLE]?
                    .FirstOrDefault(b => (int)b[Constants.ConfigKeys.ID] == (int)battleId);
                
                if (battleToken != null)
                {
                    var battleConfig = new BattleConfig
                    {
                        Id = (int)battleToken[Constants.ConfigKeys.ID],
                        Scenario = ParseBattleScenario((int)battleToken[Constants.ConfigKeys.BATTLE_SCENARIO], jsonData),
                        Enemies = ParseWarriors(battleToken[Constants.ConfigKeys.WARRIORS], jsonData),
                        BackgroundKey = battleToken[Constants.ConfigKeys.BACKGROUND_KEY]?.ToString()
                        
                    };
                    battles.Add(battleConfig);
                }
            }
            return battles;
        }

        private static BattleScenario ParseBattleScenario(int battleScenarioId, JObject jsonData)
        {
            var battleScenarioToken = jsonData[Constants.ConfigKeys.BATTLE_SCENARIO]?
                .FirstOrDefault(f => (int)f[Constants.ConfigKeys.ID] == battleScenarioId);
            if (battleScenarioToken != null)
            {
                return new BattleScenario()
                {
                    Id = (int)battleScenarioToken[Constants.ConfigKeys.ID],
                    Waves = ParseWaves(battleScenarioToken[Constants.ConfigKeys.WAVES], jsonData),
                };
            }
            return null;
        }

        private static List<EnemyWave> ParseWaves(JToken wavesToken, JObject jsonData)
        {
            var waves = new List<EnemyWave>();

            foreach (var waveId in wavesToken)
            {
                var waveToken = jsonData[Constants.ConfigKeys.WAVE]?
                    .FirstOrDefault(w => (int)w[Constants.ConfigKeys.ID] == (int)waveId);
                if (waveToken != null)
                {
                    var wave = new EnemyWave()
                    {
                        Id = (int) waveToken[Constants.ConfigKeys.ID],
                        SpawnSequences = ParseSequences(waveToken[Constants.ConfigKeys.ENEMY_SPAWN_SEQUENCES], jsonData),
                    };
                    waves.Add(wave);
                }
            }

            return waves;
        }

        private static List<EnemySpawnSequence> ParseSequences(JToken spawnSequencesToken, JObject jsonData)
        {
            var waves = new List<EnemySpawnSequence>();

            foreach (var spawnSequenceId in spawnSequencesToken)
            {
                var spawnSequenceToken = jsonData[Constants.ConfigKeys.ENEMY_SPAWN_SEQUENCE]?
                    .FirstOrDefault(w => (int)w[Constants.ConfigKeys.ID] == (int)spawnSequenceId);
                if (spawnSequenceToken != null)
                {
                    var wave = new EnemySpawnSequence()
                    {
                        Id = (int)spawnSequenceToken[Constants.ConfigKeys.ID],
                        Type = spawnSequenceToken[Constants.ConfigKeys.UNIT_TYPE] != null ?
                            (UnitType)Enum.Parse(typeof(UnitType),
                                spawnSequenceToken[Constants.ConfigKeys.UNIT_TYPE].ToString(), 
                                true) : default(UnitType),
                        Amount = (int)spawnSequenceToken[Constants.ConfigKeys.AMOUNT],
                        Cooldown = (float)spawnSequenceToken[Constants.ConfigKeys.COOLDOWN]
                    };
                    waves.Add(wave);
                }
            }

            return waves;
        }

        private static List<WarriorConfig> ParseWarriors(JToken warriorsToken, JObject jsonData)
        {
            var warriors = new List<WarriorConfig>();
            foreach (var warriorId in warriorsToken)
            {
                var warriorToken = jsonData[Constants.ConfigKeys.WARRIOR]?
                    .FirstOrDefault(w => (int)w[Constants.ConfigKeys.ID] == (int)warriorId);
                if (warriorToken != null)
                {
                    var warriorConfig = new WarriorConfig
                    {
                        Id = (int)warriorToken[Constants.ConfigKeys.ID],
                        Health = (float)warriorToken[Constants.ConfigKeys.HEALTH],
                        Speed = (float)warriorToken[Constants.ConfigKeys.SPEED],
                        Damage = (float)warriorToken[Constants.ConfigKeys.DAMAGE],
                        Name = warriorToken[Constants.ConfigKeys.NAME]?.ToString(),
                        IconKey = warriorToken[Constants.ConfigKeys.ICON_KEY]?.ToString(),
                        Price = (float)warriorToken[Constants.ConfigKeys.PRICE],
                        EnemyKey = warriorToken[Constants.ConfigKeys.ENEMY_KEY]?.ToString(),
                        PlayerKey = warriorToken[Constants.ConfigKeys.PLAYER_KEY]?.ToString(),
                        FoodPrice = (int)warriorToken[Constants.ConfigKeys.FOOD_PRICE],
                        Type = warriorToken[Constants.ConfigKeys.UNIT_TYPE] != null ?
                            (UnitType)Enum.Parse(typeof(UnitType),
                                warriorToken[Constants.ConfigKeys.UNIT_TYPE].ToString(), 
                                true) : default(UnitType),
                    };
                    warriors.Add(warriorConfig);
                }
            }
            return warriors;
        }

    }
}