using System.Collections.Generic;
using _Game.Core.Configs.Models;

namespace _Game.Core.Configs.Repositories.Timeline
{
    public interface ITimelineConfigRepository
    {
        AgeConfig[] GetAgeConfigs();
        IEnumerable<BattleConfig> GetBattleConfigs();
        int LastBattle();
        int LastAge();
        int LastTimeline();
    }
}