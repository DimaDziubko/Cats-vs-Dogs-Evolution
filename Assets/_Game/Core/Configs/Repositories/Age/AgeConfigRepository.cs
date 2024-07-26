using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories.Timeline;
using Assets._Game.Core._Logger;

namespace _Game.Core.Configs.Repositories.Age
{
    public class AgeConfigRepository : IAgeConfigRepository
    {
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private readonly IMyLogger _logger;

        public AgeConfigRepository(
            ITimelineConfigRepository timelineConfigRepository,
            IMyLogger logger)
        {
            _timelineConfigRepository = timelineConfigRepository;
            _logger = logger;
        }

        public AgeConfig GetAgeConfig(int ageIndex)
        {
            var configs = _timelineConfigRepository.GetAgeConfigs();
            
            if (configs == null || configs.Length - 1 < ageIndex)
            {
                _logger.LogError("Age config lost");
                return null;
            }

            return configs[ageIndex];
        }

        public float GetAgePrice(int ageId) =>
            GetAgeConfig(ageId)?.Price ?? 0;

        public IEnumerable<WarriorConfig> GetAgeUnits(int ageId) =>
            GetAgeConfig(ageId)?.Warriors;
        
    }
}