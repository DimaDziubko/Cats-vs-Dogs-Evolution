using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.PersistentData;

namespace _Game.Core.Configs.Repositories
{
    public interface IBattleSpeedConfigRepository
    {
        List<BattleSpeedConfig> GetBattleSpeedConfigs();
        BattleSpeedConfig GetBattleSpeedConfig(int battleSpeedId);
    }

    public class BattleSpeedConfigRepository : IBattleSpeedConfigRepository
    {
        private readonly IPersistentDataService _persistentData;

        public BattleSpeedConfigRepository(
            IPersistentDataService persistentData)
        {
            _persistentData = persistentData;
        }
        public List<BattleSpeedConfig> GetBattleSpeedConfigs() => 
            _persistentData.GameConfig.BattleSpeedConfigs;

        public BattleSpeedConfig GetBattleSpeedConfig(int battleSpeedId) => 
            _persistentData.GameConfig.BattleSpeedConfigs[battleSpeedId];
    }
}