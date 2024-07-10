using Assets._Game.Core._Logger;
using UnityEngine;

namespace _Game.Core._Logger
{
    public class MyLogger : IMyLogger
    {
        public void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public void LogWarning(string message)
        {
#if UNITY_EDITOR           
            Debug.LogWarning(message);
#endif            
        }

        public void LogError(string message)
        {
#if UNITY_EDITOR            
            Debug.LogError(message);
#endif           
        }
    }
}