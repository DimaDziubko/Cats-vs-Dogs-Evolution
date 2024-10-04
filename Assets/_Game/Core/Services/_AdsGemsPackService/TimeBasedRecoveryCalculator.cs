using System;
using UnityEngine;

namespace _Game.Core.Services._AdsGemsPackService
{
    public class TimeBasedRecoveryCalculator
    {
        public bool CalculateRecoveredUnits(int currentCount, int maxCount, int recoverTimeMinutes, DateTime lastUseTime, out int recoveredUnits)
        {
            recoveredUnits = 0;

            if (currentCount >= maxCount)
            {
                return true;
            }

            DateTime now = DateTime.UtcNow;
            TimeSpan timeSinceLastUse = now - lastUseTime;

            
            Debug.Log($"lastUseTime: {lastUseTime:yyyy-MM-dd HH:mm:ss}");
            Debug.Log($"<color=green>timeSinceLastUse: {timeSinceLastUse.TotalMinutes} minutes</color>");
            
            if (timeSinceLastUse.TotalMinutes < 0)
            {
                timeSinceLastUse = TimeSpan.Zero;
            }

            int recoverableUnits = Math.Max(0, (int)(timeSinceLastUse.TotalMinutes / recoverTimeMinutes));
            recoveredUnits = Math.Min(recoverableUnits, maxCount - currentCount);

            Debug.Log($"<color=green>recoverableUnits: {recoverableUnits}</color>");
            Debug.Log($"<color=green>recoveredUnits: {recoveredUnits}</color>");

            return currentCount + recoveredUnits >= maxCount;
        }
        
        public float CalculateTimeUntilNextRecoverySeconds(int recoveringTimeMinutes, DateTime lastUseTime)
        {
            DateTime now = DateTime.UtcNow;
            
            TimeSpan recoveryInterval = TimeSpan.FromMinutes(recoveringTimeMinutes);
            
            TimeSpan timeForNextUnit = recoveryInterval - (now - lastUseTime);
            
            float secondsUntilNextRecovery = (float)Math.Max(0, timeForNextUnit.TotalSeconds);
            
            Debug.Log($"<color=green>Time Until Next Recovery: {secondsUntilNextRecovery} seconds</color>");
    
            return secondsUntilNextRecovery;
        }
        
        public DateTime CalculateNewLastUseTime(DateTime lastUseTime, int recoveredUnits, int recoverTimeMinutes) => 
            lastUseTime.AddMinutes(recoveredUnits * recoverTimeMinutes);
    }
}