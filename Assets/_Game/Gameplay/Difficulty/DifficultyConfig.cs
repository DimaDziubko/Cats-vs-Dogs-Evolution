using System;

namespace _Game.Gameplay.Difficulty
{
    [Serializable]
    public class DifficultyConfig
    {
        public int InitialLevelDuration = 10;
        
        //Seconds
        public int LevelDurationModyfier = 5;

        //Seconds
        public int InitialDifficultyChangeTime = 10;
    }
}