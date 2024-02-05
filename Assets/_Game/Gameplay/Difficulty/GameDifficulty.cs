using _Game.Core.UserState;

namespace _Game.Gameplay.Difficulty
{
    public class GameDifficulty
    {
        public int LevelDuration
        {
            get => _currentLevelDuration;
        }
        
        public int CurrentDifficultyChangeTime
        {
            get => _currentDifficultyChangeTime;
        }

        private readonly IUserTimelineStateReadonly _timelineState;

        private readonly int _initialLevelDuration;
        private readonly int _levelDurationModifier;
        
        private int _currentLevelDuration;
        private int _currentDifficultyChangeTime;
        
        private readonly int _timeInitialCount;

        private readonly int _initialDifficultyChangeTime;
        private readonly int _levelDifficultyChangeTimeModifier;


        public GameDifficulty(
            IUserTimelineStateReadonly timelineState,
            DifficultyConfig config)
        {
            _initialLevelDuration = config.InitialLevelDuration;
            _levelDurationModifier = config.LevelDurationModyfier;

            _initialDifficultyChangeTime = config.InitialDifficultyChangeTime;

            _timelineState = timelineState;
            CalculateLevelDuration();
            DifficultyChangeTime();
        }
        

        private void CalculateLevelDuration()
        {
            _currentLevelDuration = _initialLevelDuration + _levelDurationModifier * _timelineState.TimelineId;
        }

        private void DifficultyChangeTime()
        {
            _currentDifficultyChangeTime = _initialDifficultyChangeTime;
        }
    }
}
