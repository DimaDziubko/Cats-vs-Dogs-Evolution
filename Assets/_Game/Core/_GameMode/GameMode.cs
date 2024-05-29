using UnityEngine;

namespace _Game.Core._GameMode
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] private bool _testMode;
        
        public static GameMode I;

        public bool TestMode => _testMode;
        
        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }
    }
}
