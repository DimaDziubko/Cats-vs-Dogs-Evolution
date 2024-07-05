using System.Collections.Generic;
using System.Linq;
using Assets._Game.Core.Configs.Models;

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

        public AgeConfigRepository(ITimelineConfigRepository timelineConfigRepository)
        {
            _timelineConfigRepository = timelineConfigRepository;
        }

        public AgeConfig GetAgeConfig(int ageId) =>
            _timelineConfigRepository.GetAgeConfigs().FirstOrDefault(age => age.Id == ageId);

        public float GetAgePrice(int ageId) =>
            GetAgeConfig(ageId)?.Price ?? 0;

        public IEnumerable<WarriorConfig> GetAgeUnits(int ageId) =>
            GetAgeConfig(ageId)?.Warriors;
        
    }
}