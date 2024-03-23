using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using Newtonsoft.Json.Linq;

namespace _Game.Utils.Extensions
{
    public static class JsonExtensions
    {
        private const int ID_OFFSET = 1;
        
        //TODO Check magic number
        public static GameConfig ToGameConfig(this JObject jsonData, int timelineId = 0)
        {
            var config = new GameConfig();
            
            var timelineToken = jsonData[Constants.ConfigKeys.TIMELINE]?
                .FirstOrDefault(t => ((int)t[Constants.ConfigKeys.ID] - ID_OFFSET) == timelineId);
            
            if (timelineToken != null)
            {
                config.Timeline = ParseTimeline(timelineToken, jsonData);
            }

            var commonToken = jsonData[Constants.ConfigKeys.COMMON]?
                .FirstOrDefault(t => ((int)t[Constants.ConfigKeys.ID] - ID_OFFSET) == 0);
            
            if (commonToken != null)
            {
                config.CommonConfig = new CommonConfig()
                {
                    Id = ((int)commonToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                    FoodIconKey = commonToken[Constants.ConfigKeys.FOOD_ICON_KEY]?.ToString(),
                    BaseIconKey = commonToken[Constants.ConfigKeys.BASE_ICON_KEY]?.ToString(),
                };
            }
            
            return config;
        }

        private static TimelineConfig ParseTimeline(JToken timelineToken, JObject jsonData)
        {
            var timelineConfig = new TimelineConfig
            {
                Id = ((int)timelineToken[Constants.ConfigKeys.ID] - ID_OFFSET),
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
                        Id = ((int)ageToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                        Price = (float)ageToken[Constants.ConfigKeys.PRICE],
                        GemsPerAge = (float)ageToken[Constants.ConfigKeys.GEMS_PER_AGE],
                        Economy = ParseEconomy((int)ageToken[Constants.ConfigKeys.ECONOMY], jsonData),
                        Warriors = ParseWarriors(ageToken[Constants.ConfigKeys.WARRIORS], jsonData),
                        PlayerBaseKey = ageToken[Constants.ConfigKeys.PLAYER_BASE_KEY]?.ToString(),
                        Name = ageToken[Constants.ConfigKeys.NAME]?.ToString(),
                        AgeIconKey = ageToken[Constants.ConfigKeys.AGE_ICON_KEY]?.ToString(),
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
                    Id = ((int)economyToken[Constants.ConfigKeys.ID] - ID_OFFSET),
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
                    Id = ((int)foodProductionToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                    Price = (float)foodProductionToken[Constants.ConfigKeys.PRICE],
                    PriceExponential = ParsePolynomial((int)foodProductionToken[Constants.ConfigKeys.PRICE_EXPONENTIAL], jsonData),
                    Speed = (float)foodProductionToken[Constants.ConfigKeys.SPEED],
                    SpeedStep = (float)foodProductionToken[Constants.ConfigKeys.SPEED_STEP],
                    InitialFoodAmount = (int)foodProductionToken[Constants.ConfigKeys.INITIAL_FOOD_AMOUNT],
                };
            }
            return null;
        }

        private static Exponential ParsePolynomial(int polynomialId, JObject jsonData)
        {
            var polynomialToken = jsonData[Constants.ConfigKeys.EXPONENTIAL]?
                .FirstOrDefault(e => (int)e[Constants.ConfigKeys.ID] == polynomialId);
            if (polynomialToken != null)
            {
                return new Exponential()
                {
                    Id = ((int) polynomialToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                    A = ((float) polynomialToken[Constants.ConfigKeys.COEFFICIENT_A]),
                    B = ((float) polynomialToken[Constants.ConfigKeys.COEFFICIENT_B]),
                    C = ((float) polynomialToken[Constants.ConfigKeys.COEFFICIENT_C]),
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
                    Id = ((int)baseHealthToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                    Price = (float)baseHealthToken[Constants.ConfigKeys.PRICE],
                    PriceExponential = ParsePolynomial((int)baseHealthToken[Constants.ConfigKeys.PRICE_EXPONENTIAL], jsonData),
                    Health = (float)baseHealthToken[Constants.ConfigKeys.HEALTH],
                    HealthStep = (float)baseHealthToken[Constants.ConfigKeys.HEALTH_STEP]
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
                        Id = ((int)battleToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                        Scenario = ParseBattleScenario((int)battleToken[Constants.ConfigKeys.BATTLE_SCENARIO], jsonData),
                        Enemies = ParseWarriors(battleToken[Constants.ConfigKeys.WARRIORS], jsonData),
                        EnvironmentKey = battleToken[Constants.ConfigKeys.BACKGROUND_KEY]?.ToString(),
                        EnemyBaseHealth = (float)battleToken[Constants.ConfigKeys.ENEMY_BASE_HEALTH],
                        EnemyBaseKey = battleToken[Constants.ConfigKeys.ENEMY_BASE_KEY]?.ToString(),
                        BGMKey = battleToken[Constants.ConfigKeys.BGM_KEY]?.ToString(),
                        CoinsPerBase = (float)battleToken[Constants.ConfigKeys.COINS_PER_BASE],
                        
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
                    Id = ((int)battleScenarioToken[Constants.ConfigKeys.ID] - ID_OFFSET),
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
                        Id = ((int) waveToken[Constants.ConfigKeys.ID] - ID_OFFSET),
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
                        Id = ((int)spawnSequenceToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                        Type = spawnSequenceToken[Constants.ConfigKeys.UNIT_TYPE] != null ?
                            (UnitType)Enum.Parse(typeof(UnitType),
                                spawnSequenceToken[Constants.ConfigKeys.UNIT_TYPE].ToString(), 
                                true) : default(UnitType),
                        Amount = (int)spawnSequenceToken[Constants.ConfigKeys.AMOUNT],
                        Cooldown = (float)spawnSequenceToken[Constants.ConfigKeys.COOLDOWN],
                        StartDelay = (float)spawnSequenceToken[Constants.ConfigKeys.SEQUENCE_START_DELAY],
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
                        Id = ((int)warriorToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                        Health = (float)warriorToken[Constants.ConfigKeys.HEALTH],
                        Speed = (float)warriorToken[Constants.ConfigKeys.SPEED],
                        WeaponConfig = ParseWeapon((int)warriorToken[Constants.ConfigKeys.WEAPON], jsonData),
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
                        CoinsPerKill = (int)warriorToken[Constants.ConfigKeys.COINS_PER_KILL],
                    };
                    warriors.Add(warriorConfig);
                }
            }
            return warriors;
        }

        private static WeaponConfig ParseWeapon(int weaponId, JObject jsonData)
        {
            var weaponConfigToken = jsonData[Constants.ConfigKeys.WEAPON]?
                .FirstOrDefault(f => (int)f[Constants.ConfigKeys.ID] == weaponId);
            if (weaponConfigToken != null)
            {
                return new WeaponConfig()
                {
                    Id = ((int)weaponConfigToken[Constants.ConfigKeys.ID] - ID_OFFSET),
                    
                    WeaponType = weaponConfigToken[Constants.ConfigKeys.WEAPON_TYPE] != null ?
                        (WeaponType)Enum.Parse(typeof(WeaponType),
                            weaponConfigToken[Constants.ConfigKeys.WEAPON_TYPE].ToString(), 
                            true) : default(WeaponType),
                    
                    Damage = (float)weaponConfigToken[Constants.ConfigKeys.DAMAGE],
                    
                    ProjectileSpeed = (float)weaponConfigToken[Constants.ConfigKeys.PROJECTILE_SPEED],
                    
                    ProjectileKey = weaponConfigToken[Constants.ConfigKeys.PROJECTILE_KEY]?.ToString(),
                    
                    TrajectoryWarpFactor = (float)weaponConfigToken[Constants.ConfigKeys.TRAJECTORY_WARP_FACTOR],
                    MuzzleKey = weaponConfigToken[Constants.ConfigKeys.MUZZLE_KEY]?.ToString(),
                    ProjectileExplosionKey = weaponConfigToken[Constants.ConfigKeys.PROJECTILE_EXPLOSION_KEY]?.ToString(),
                    
                    SplashRadius = (float)weaponConfigToken[Constants.ConfigKeys.SPLASH_RADIUS],
                };
            }

            return null;
        }
        
    }
}