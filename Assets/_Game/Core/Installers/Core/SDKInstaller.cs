using _Game.Core._Facebook;
using _Game.Core.Ads.ApplovinMaxAds;
using _Game.Core.Services.Analytics;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class SDKInstaller : MonoInstaller
    {
        [SerializeField] private AppsFlyerSettings _appsFlyerSettings;
        public override void InstallBindings()
        {
            BindAdsService();
            BindAppsFlyerAnalyticsService();
            BindAnalyticsService();
            BindFacebookInstaller();
        }
        
        private void BindAppsFlyerAnalyticsService() =>
            Container.BindInterfacesAndSelfTo<AppsFlyerAnalyticsService>()
                .AsSingle()
                .WithArguments(_appsFlyerSettings)
                .NonLazy();
        private void BindAnalyticsService() =>
            Container.BindInterfacesAndSelfTo<FirebaseAnalyticsService>().AsSingle();
        private void BindAdsService() =>
            Container
                .BindInterfacesAndSelfTo<MaxAdsService>()
                .AsSingle();
        private void BindFacebookInstaller() => 
            Container.BindInterfacesAndSelfTo<FacebookInitializer>().AsSingle().NonLazy();
    }
}