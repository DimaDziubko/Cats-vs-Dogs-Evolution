using System.Collections.Generic;
using Unity.Usercentrics;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class UserCentricsService : MonoBehaviour
    {
        //[SerializeField] private LoadingSceneContext _loadingSceneContext;
        //[SerializeField] private ApplovinMaxController _applovinMaxController;


        private void Start()
        {
#if UNITY_EDITOR
            InitGame();
            return;
#endif

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Usercentrics.Instance.Initialize((status) =>
                {
                    if (status.shouldCollectConsent)
                    {
                        ShowFirstLayer();
                    }
                    else
                    {
                        ApplyConsent(status.consents);
                    }
                },
                (errorMessage) =>
                {
                    Debug.Log("[USERCENTRICS] AutoInitialize is " + errorMessage);
                    //ApplyConsent(new List<UsercentricsServiceConsent>());
                    InitGame();
                });
            }
            else
            {
                //ApplyConsent(new List<UsercentricsServiceConsent>());
                InitGame();
            }
        }
        private void ShowSecondLayer()
        {
            Usercentrics.Instance.ShowSecondLayer(bannerSettings: new BannerSettings(), (userResponse) =>
            {
                // Handle userResponse
                ApplyConsent(userResponse.consents);
            });
        }

        private void InitGame()
        {
            //Mb Start Game
            //_applovinMaxController.InitSdk();
            //_loadingSceneContext.StartInit();
        }

        private void ApplyConsent(List<UsercentricsServiceConsent> consents)
        {

            foreach (var serviceConsent in consents)
            {
                switch (serviceConsent.templateId)
                {
                    case "H1Vl5NidjWX": // Usercentrics Consent Management Platform
                                        // Apply specific logic if needed
                        break;
                    case "42vRvlulK96R-F": // Firebase
                                           // Firebase specific logic
                        break;
                    case "fHczTMzX8": // AppLovin
                        MaxSdk.SetHasUserConsent(serviceConsent.status);
                        Debug.Log("MaxSdk SetHasUserConsent " + serviceConsent.status);
                        break;
                    case "XFAdcSj-7Jc-JJ": // Appodeal
                                           // Appodeal specific logic
                        break;
                    case "Gx9iMF__f": // AppsFlyer
                                      // AppsFlyer specific logic
                        break;
                    case "ax0Nljnj2szF_r": // Facebook Audience Network
                                           // Facebook Audience Network specific logic
                        break;
                    case "bQTbuxnTb": // GameAnalytics SDK
                                      // GameAnalytics SDK specific logic
                        break;
                    case "9dchbL797": // ironSource
                                      // ironSource specific logic
                        break;
                    case "E6AgqirYV": // Mintegral SDK
                                      // Mintegral SDK specific logic
                        break;
                    case "hpb62D82I": // Unity Ads
                                      // Unity Ads specific logic
                        break;
                    case "xLgL2b0bmU-RT_": // Supersonic
                                           // Supersonic specific logic
                        break;
                    case "HWSNU_Ll1": // Pangle SDK
                                      // Pangle SDK specific logic
                        break;
                    case "HyeqVsdjWX": // Criteo
                                       // Criteo specific logic
                        break;
                    case "H17alcVo_iZ7": // Fyber
                                         // Fyber specific logic
                        break;
                    case "S1_9Vsuj-Q": // Google Ads
                                       // Google Ads specific logic
                        break;
                    case "ykdq8J5a9MExGT": // InMobi
                                           // InMobi specific logic
                        break;
                    default:
                        Debug.Log($"Unknown consent template ID: {serviceConsent.templateId}");
                        break;
                }
            }

            //bool isMaxContest = 
            //MaxSdk.SetHasUserConsent(consents.Find();

            InitGame();
        }

        private void ShowFirstLayer()
        {
            Usercentrics.Instance.ShowFirstLayer((userResponse) =>
            {
                // Handle userResponse
                ApplyConsent(userResponse.consents);
            });
        }
    }
}