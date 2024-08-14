using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories.Timeline
{
    public class TimelineConfigRepository : ITimelineConfigRepository
    {
        private readonly IUserContainer _userContainer;
        public TimelineConfigRepository(IUserContainer userContainer) => _userContainer = userContainer;

        public TimelineConfig GetCurrentTimeline() => _userContainer.GameConfig.CurrentTimeline;

        public AgeConfig[] GetAgeConfigs() => GetCurrentTimeline()?.Ages.ToArray();
        public List<BattleConfig> GetBattleConfigs() => GetCurrentTimeline()?.Battles;
        public int LastBattle() => GetCurrentTimeline().Battles.Count - 1;
        public int LastAge() => GetCurrentTimeline().Ages.Count - 1;
    }
}