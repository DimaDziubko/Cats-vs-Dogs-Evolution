using UnityEngine;

namespace _Game.Core._Logger
{
    public class MyLogger : IMyLogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}