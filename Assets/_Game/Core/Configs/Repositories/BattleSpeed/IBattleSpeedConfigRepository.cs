using System.Collections.Generic;
using _Game.Core.Configs.Models;

namespace _Game.Core.Configs.Repositories.BattleSpeed
{
    public interface IBattleSpeedConfigRepository
    {
        List<BattleSpeedConfig> GetBattleSpeedConfigs();
        BattleSpeedConfig GetBattleSpeedConfig(int battleSpeedId);
    }
}