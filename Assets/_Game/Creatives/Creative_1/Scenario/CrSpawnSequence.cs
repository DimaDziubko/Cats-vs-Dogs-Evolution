using System;
using Assets._Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace Assets._Game.Creatives.Creative_1.Scenario
{
    [Serializable]
    public class CrSpawnSequence
    {
        [SerializeField] private UnitType _type;
        [SerializeField, Range(1, 300)] private int _amount = 1;
        [SerializeField, Range(0.1f, 10f)] private float _cooldown = 1f;

        public State Begin() => new State(this);
        
        [Serializable]
        public struct State
        {
            private CrSpawnSequence _sequence;
            private int _count;
            private float _cooldown;
            public State(CrSpawnSequence sequence)
            {
                _sequence = sequence;
                _count = 0;
                _cooldown = sequence._cooldown;
            }
            
            public float Progress(float deltaTime)
            {
                _cooldown += deltaTime;
                while (_cooldown >= _sequence._cooldown)
                {
                    _cooldown -= _sequence._cooldown;
                    if (_count >= _sequence._amount)
                    {
                        return _cooldown;
                    }

                    _count++;
                    
                    CrQuickGame.I.SpawnEnemy(_sequence._type);
                }

                return -1f;
            }
        }
    }
}