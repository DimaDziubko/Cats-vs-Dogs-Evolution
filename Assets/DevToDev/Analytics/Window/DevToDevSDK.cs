﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToDev;
using System;
using System.Linq;
using DevToDev.Analytics;
using UnityEngine.Serialization;

#if UNITY_METRO && !UNITY_EDITOR
using System.Reflection;
#endif

namespace DTDEditor
{
    public class DevToDevSDK : MonoBehaviour
    {
        public DTDLogLevel logLevel;
        public DTDCredentials[] credentials;
        public bool isAnalyticsEnabled;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            DTDAnalytics.SetLogLevel(logLevel);
#if UNITY_ANDROID
            InitializeAnalytics(DTDPlatform.Android);
#elif UNITY_IOS
            InitializeAnalytics(DTDPlatform.IOS);
#elif UNITY_METRO || UNITY_WSA
			InitializeAnalytics(DTDPlatform.UWP);
#elif UNITY_STANDALONE_OSX
			InitializeAnalytics(DTDPlatform.MacOS);
#elif UNITY_STANDALONE_WIN
			InitializeAnalytics(DTDPlatform.WindowsStandalone);
#endif
        }

        private void InitializeAnalytics(DTDPlatform platform)
        {
            var targetCredential = credentials.FirstOrDefault(item => item.platform == platform);
            if (targetCredential != null)
            {
                DTDAnalytics.Initialize(targetCredential.key);
            }
        }
    }
}