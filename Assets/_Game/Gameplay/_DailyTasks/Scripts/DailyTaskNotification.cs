using _Game.Core.Notifications;
using System;
using UnityEngine;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskNotification : MonoBehaviour, IDisposable
    {
        [Inject]
        private readonly NotificationService _notificationService;
        [Inject]
        private readonly IDailyTaskGenerator _iDailyTaskGenerator;

        private DailyTaskGenerator _dailyTaskGenerator;

        private void Start()
        {
            Initialize();
        }
        public void Initialize()
        {
            _dailyTaskGenerator = _iDailyTaskGenerator as DailyTaskGenerator;
        }

        private void SendNotification()
        {
            var time = _dailyTaskGenerator.GetMinutesToGenerateDailyTask();

            if (time <= 0) return;

            _notificationService.SendDailyTaskAvalivableNotification(time);
        }
        void IDisposable.Dispose()
        {

        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SendNotification();
            }
        }

        private void OnApplicationQuit()
        {
            SendNotification();

        }
    }
}