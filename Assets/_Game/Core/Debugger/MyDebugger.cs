using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Timer.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Debugger
{
    public class MyDebugger : MonoBehaviour
    {
        [Inject, ShowInInspector]
        private TimerService _timerService;
        
        [Inject, ShowInInspector]
        private UserContainer _userContainer;
        
        [Inject, ShowInInspector]
        private AdsGemsPackService _adsGemsPackService;

    }
}