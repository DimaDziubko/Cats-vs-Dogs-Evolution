using System;
using UnityEngine;

namespace _Game.Creatives.Creative_1.Scenario
{
    
    [CreateAssetMenu(fileName = "EnemyWave", menuName = "Core/Enemy Wave")]
    public class CrWave : ScriptableObject
    {
        [SerializeField] private CrSpawnSequence[] _spawnSequences;

        public State Begin() => new State(this);

        [Serializable]
        public struct State
        {
            private CrWave _wave;
            private int _index;
            private CrSpawnSequence.State _sequence;

            public State(CrWave wave)
            {
                _wave = wave;
                _index = 0;
                _sequence = _wave._spawnSequences[0].Begin();
            }

            public float Progress(float deltaTime)
            {
                deltaTime = _sequence.Progress(deltaTime);
                while (deltaTime >= 0f)
                {
                    if (++_index >= _wave._spawnSequences.Length)
                    {
                        return deltaTime;
                    }

                    _sequence = _wave._spawnSequences[_index].Begin();
                    deltaTime = _sequence.Progress(deltaTime);
                }

                return -1;
            }
        }
    }
}