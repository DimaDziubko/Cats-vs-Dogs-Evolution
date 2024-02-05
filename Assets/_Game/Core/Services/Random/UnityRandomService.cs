﻿using UnityEngine;

namespace _Game.Core.Services.Random
{
    public class UnityRandomService : IRandomService
    {
        public int Next(int min, int max) =>
            UnityEngine.Random.Range(min, max);
        
        public float Next(float min, float max) =>
            UnityEngine.Random.Range(min, max);

        public float GetValue() => 
            UnityEngine.Random.value;

        public Vector3 OnUnitSphere()
        {
            return UnityEngine.Random.onUnitSphere;
        }
    }
}