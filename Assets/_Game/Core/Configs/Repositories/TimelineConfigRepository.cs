using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;
using Assets._Game.Core.UserState;

namespace Assets._Game.Core.Configs.Repositories
{
    public interface ITimelineConfigRepository
    {
        TimelineConfig GetCurrentTimeline();
        IEnumerable<AgeConfig> GetAgeConfigs();
        IEnumerable<BattleConfig> GetBattleConfigs();
        int LastBattle();
        int LastAge();
    }

    public class TimelineConfigRepository : ITimelineConfigRepository
    {
        private readonly IUserContainer _persistentData;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;

        public TimelineConfigRepository(IUserContainer persistentData)
        {
            _persistentData = persistentData;
        }

        public TimelineConfig GetCurrentTimeline()
        {
            int currentTimelineId = TimelineState.TimelineId;
            return _persistentData.GameConfig.Timeline.Id == currentTimelineId ? _persistentData.GameConfig.Timeline : null;
        }

        public IEnumerable<AgeConfig> GetAgeConfigs() => GetCurrentTimeline()?.Ages;
        public IEnumerable<BattleConfig> GetBattleConfigs() => GetCurrentTimeline()?.Battles;
        public int LastBattle() => GetCurrentTimeline().Battles.Count - 1;
        public int LastAge() => GetCurrentTimeline().Ages.Count - 1;
    }
}