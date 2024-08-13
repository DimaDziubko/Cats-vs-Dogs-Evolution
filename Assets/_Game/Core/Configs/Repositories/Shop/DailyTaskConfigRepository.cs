using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories.Shop
{
    public interface IDailyTaskConfigRepository
    {
        int MaxDailyCountPerDay { get; }
        public float RecoverTimeMinutes { get; }
        List<DailyTaskConfig> GetDailyTaskConfigs();
    }

    public class DailyTaskConfigRepository : IDailyTaskConfigRepository
    {
        private readonly IUserContainer _userContainer;
        
        public DailyTaskConfigRepository(IUserContainer userContainer) =>
            _userContainer = userContainer;

        public List<DailyTaskConfig> GetDailyTaskConfigs() => 
            _userContainer.GameConfig.GeneralDailyTaskConfig.DailyTaskConfigs;

        public int MaxDailyCountPerDay => _userContainer.GameConfig.GeneralDailyTaskConfig.MaxCountPerDay;
        public float RecoverTimeMinutes => _userContainer.GameConfig.GeneralDailyTaskConfig.RecoverTimeMinutes;
    }
}