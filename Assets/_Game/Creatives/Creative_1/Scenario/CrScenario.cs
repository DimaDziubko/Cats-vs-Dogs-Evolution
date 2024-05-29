using UnityEngine;

namespace _Game.Creatives.Creative_1.Scenario
{
    [CreateAssetMenu(fileName = "GameScenario", menuName = "Core/Game Scenario")]
    public class CrScenario : ScriptableObject
    {
        [SerializeField] private CrWave[] _waves;

        public State Begin()
        {
            return new State(this);
        }

        public struct State
        {
            private readonly CrScenario _scenario;
            private int _index;
            private CrWave.State _wave;

            public (int currentWave, int wavesCount) GetWaves()
            {
                int wavesCount = _scenario._waves.Length;
                if (_index + 1 > wavesCount)
                {
                    return (wavesCount, wavesCount);
                }

                return (_index + 1, wavesCount);
            }

            public State(CrScenario scenario)
            {
                _scenario = scenario;
                _index = 0;
                _wave = scenario._waves[0].Begin();
            }

            public bool Progress()
            {
                float deltaTime = _wave.Progress(Time.deltaTime);
                while (deltaTime >= 0)
                {
                    if (++_index >= _scenario._waves.Length)
                    {
                        return false;
                    }

                    _wave = _scenario._waves[_index].Begin();
                    deltaTime = _wave.Progress(deltaTime);
                }

                return true;
            }
        }
    }
}
