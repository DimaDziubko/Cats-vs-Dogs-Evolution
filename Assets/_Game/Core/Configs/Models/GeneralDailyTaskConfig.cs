using System.Collections.Generic;

namespace _Game.Core.Configs.Models
{
    public class GeneralDailyTaskConfig
    {
        public int Id;
        public int MaxCountPerDay;
        public int RecoverTimeMinutes;
        public List<DailyTaskConfig> DailyTaskConfigs;
    }
}