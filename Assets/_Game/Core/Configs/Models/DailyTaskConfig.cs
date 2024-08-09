using _Game.Gameplay._DailyTasks.Scripts;

namespace _Game.Core.Configs.Models
{
    public class DailyTaskConfig
    {
        public int Id;
        public DailyTaskType Type;
        public LinearFunction LinearFunction;
        public int DropChance;
        public string Description;
        public int Reward;
    }
}