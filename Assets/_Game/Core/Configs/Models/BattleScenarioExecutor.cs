using System;
using System.Collections.Generic;
using _Game.Gameplay._BattleField.Scripts;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    public class BattleScenarioExecutor 
    {
        private List<EnemyWaveScheduler> _waves;
        
        public State Begin(BattleField battleField) => new State(this, battleField);

        public void Init(BattleScenario scenarioData)
        {
            //TODO Delete
            Debug.Log($"BattleScenarioExecutor init with Id {scenarioData.Id}");
            
            if (_waves == null) _waves = new List<EnemyWaveScheduler>();
            
            for (int i = 0; i < scenarioData.Waves.Count; i++)
            {
                if (i < _waves.Count)
                {
                    _waves[i].Init(scenarioData.Waves[i]);
                }
                else
                {
                    EnemyWaveScheduler waveScheduler = new EnemyWaveScheduler();
                    waveScheduler.Init(scenarioData.Waves[i]);
                    _waves.Add(waveScheduler);
                }
            }
            
            if (_waves.Count > scenarioData.Waves.Count)
            {
                _waves.RemoveRange(scenarioData.Waves.Count, _waves.Count - scenarioData.Waves.Count);
            }
        }

        [Serializable]
        public struct State
        {
            private BattleField _battleField;
            
            private BattleScenarioExecutor _scenarioExecutor;
            private int _index;
            private EnemyWaveScheduler.State _wave;

            public (int currentWave, int wavesCount) GetWaves()
            {
                return (_index + 1, _scenarioExecutor._waves.Count + 1);
            }

            public State(BattleScenarioExecutor scenarioExecutor, BattleField battleField)
            {
                _scenarioExecutor = scenarioExecutor;
                _index = 0;
                _wave = _scenarioExecutor._waves[0].Begin(battleField);
                _battleField = battleField;
            }

            public bool Progress()
            {
                float deltaTime = _wave.Progress(Time.deltaTime);
                while (deltaTime >= 0f)
                {
                    if (++_index >= _scenarioExecutor._waves.Count)
                    {
                        return false;
                    }

                    _wave = _scenarioExecutor._waves[_index].Begin(_battleField);
                    deltaTime = _wave.Progress(deltaTime);
                }

                return true;
            }
        }
    }
}