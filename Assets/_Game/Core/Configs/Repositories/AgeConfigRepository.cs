using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Repositories;
using Assets._Game.Core._Logger;

namespace Assets._Game.Core.Configs.Repositories
{
    public interface IAgeConfigRepository
    {
        AgeConfig GetAgeConfig(int ageId);
        float GetAgePrice(int ageId);
        IEnumerable<WarriorConfig> GetAgeUnits(int ageId);
    }

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