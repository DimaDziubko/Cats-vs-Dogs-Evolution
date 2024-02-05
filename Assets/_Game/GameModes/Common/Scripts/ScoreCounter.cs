using System;

namespace _Game.GameModes.Common.Scripts
{
    public class ScoreCounter
    {
        public event Action<int, int> ScoreChanged;
        public event Action ObjectiveAchieved;
        
        private int _score;
        private readonly int _objective;
        public int Score => _score;

        private int _factor;

        public ScoreCounter(
            int factor,
            int objective)
        {
            _factor = factor;
            _objective = objective;
        }
        
        public void UpdateScore(int score)
        {
            _score += score * _factor;
            
            if (_score >= _objective)
            {
                _score = _objective;
                ObjectiveAchieved?.Invoke();
            }

            ScoreChanged?.Invoke(_score, _objective);
        }

        public void ChangeFactor(int factor)
        {
            _factor = factor;
        }
        
        public void Cleanup()
        {
            _score = 0;
        }
    }
}