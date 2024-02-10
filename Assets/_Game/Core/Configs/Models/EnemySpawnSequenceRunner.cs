using System;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay._BattleField.Scripts;

namespace _Game.Core.Configs.Models
{
    public class EnemySpawnSequenceRunner
    {
        private UnitType _type;
        private int _amount;
        private float _cooldown;

        public void Init(EnemySpawnSequence sequence)
        {
            _type = sequence.Type;
            _amount = sequence.Amount;
            _cooldown = sequence.Cooldown;
        }

        public State Begin(BattleField battleField) => new State(this, battleField);

        [Serializable]
        public struct State
        {
            private BattleField _battleField;
            
            private EnemySpawnSequenceRunner _sequenceRunner;
            private int _count;
            private float _cooldown;

            public State(EnemySpawnSequenceRunner sequenceRunner, BattleField battleField)
            {
                _sequenceRunner = sequenceRunner;
                _count = 0;
                _cooldown = sequenceRunner._cooldown;
                _battleField = battleField;
            }

            public float Progress(float deltaTime)
            {
                _cooldown += deltaTime;
                while (_cooldown >= _sequenceRunner._cooldown)
                {
                    _cooldown -= _sequenceRunner._cooldown;
                    if (_count >= _sequenceRunner._amount)
                    {
                        return _cooldown;
                    }
                    _count += 1;
                    
                    _battleField.SpawnEnemy(_sequenceRunner._type);
                }
                return -1f;
            }
        }
    }
}