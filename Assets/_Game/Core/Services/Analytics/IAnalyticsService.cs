﻿using _Game.Gameplay._Battle.Scripts;

namespace _Game.Core.Services.Analytics
{
    public interface IAnalyticsService
    {
        void OnBattleStarted(BattleAnalyticsData battleAnalyticsData);
        void SendEvent(string eventName);
        void SendWave(string wave, BattleAnalyticsData battleAnalyticsData);
    }
}