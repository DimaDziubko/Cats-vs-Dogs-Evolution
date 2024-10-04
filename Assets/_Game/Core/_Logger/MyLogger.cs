using UnityEngine;

namespace _Game.Core._Logger
{
    public enum DebugStatus
    {
        Success,
        Warning
    }
    public class MyLogger : IMyLogger
    {
        public void Log(string message, DebugStatus status)
        {
#if UNITY_EDITOR
            switch (status)
            {
                case DebugStatus.Success:
                    Debug.Log($"<color=green>{message}</color>");
                    break;
                case DebugStatus.Warning:
                    Debug.Log($"<color=yellow>{message}</color>");
                    break;
            }
#endif
        }
        
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