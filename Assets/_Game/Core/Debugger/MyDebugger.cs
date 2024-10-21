using _Game.Core._Logger;
using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.UserContainer;
using _Game.Gameplay._Timer.Scripts;
using _Game.UI._Hud;
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

        [Inject, ShowInInspector]
        private Hud _hud;
        
        [Inject, ShowInInspector] private MaxAdsService _maxAdsService;

        [Inject, ShowInInspector]
        private MyLogger _logger;
    
    }
}