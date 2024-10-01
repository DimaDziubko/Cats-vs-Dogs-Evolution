using UnityEngine;

namespace _Game.Core._GameMode
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] private bool _testMode;
        [SerializeField] private bool _isCheatEnabled;

        public static GameMode I;

        public bool TestMode => _testMode;
        public bool IsCheatEnabled => _isCheatEnabled;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }

        //Todo Change Mb Save UserID
        public static string GetUniqUserID()
        {
            return UnityEngine.Device.SystemInfo.deviceUniqueIdentifier;
        }
    }
}
