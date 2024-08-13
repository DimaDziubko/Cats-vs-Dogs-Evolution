using _Game.Core.Configs.Models;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTask
    {
        public DailyTaskConfig Config;
        public float Progress;
        public float Target;
        public bool IsCompleted;
        public int CompletedCount;
        public int MaxCountPerDay;
        public bool IsRunOut;
    }
}