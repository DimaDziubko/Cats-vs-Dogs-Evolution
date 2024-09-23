using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public class DifficultyConfigRepository : IDifficultyConfigRepository
    {
        private const int DIFFICULTY_TIMELINE_THRESHOLD = 2;
    
        private readonly IUserContainer _userContainer;

        private DifficultyConfig DifficultyConfig => _userContainer.GameConfig.DifficultyConfig;
        public DifficultyConfigRepository(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public float GetDifficultyValue(int timeline)
        {
            if (timeline <= DIFFICULTY_TIMELINE_THRESHOLD)
            {
                return 1; //default difficulty is 1
            }
            
            return DifficultyConfig.DifficultyCurve.GetValue(timeline);
        }

        public float GetEvolutionPrice(int timelineNumber, int ageNumber)
        {
            switch (ageNumber)
            {
                case 1: return DifficultyConfig.InitialEvolutionPrices[0] + DifficultyConfig.DeltaEvolutionPrices[0] * timelineNumber;
                case 2: return DifficultyConfig.InitialEvolutionPrices[1] + DifficultyConfig.DeltaEvolutionPrices[1] * timelineNumber;
                case 3: return DifficultyConfig.InitialEvolutionPrices[2] + DifficultyConfig.DeltaEvolutionPrices[2] * timelineNumber;
                case 4: return DifficultyConfig.InitialEvolutionPrices[3] + DifficultyConfig.DeltaEvolutionPrices[3] * timelineNumber;
                case 5: return DifficultyConfig.InitialEvolutionPrices[4] + DifficultyConfig.DeltaEvolutionPrices[4] * timelineNumber;
                case 6: return DifficultyConfig.InitialEvolutionPrices[5] + DifficultyConfig.DeltaEvolutionPrices[5] * timelineNumber;
                default: return 0;
            }
        }
    }
}