using System;
using Unity.Notifications.Android;

namespace _Game.Core.Notifications
{
    public class NotificationService : IDisposable
    {
        private const string ID_DAILYTASK = "daily_task";

        public NotificationService()
        {
            CreateChannelDailyTask();
        }

        public void SendDailyTaskAvalivableNotification(float minutes)
        {
#if UNITY_ANDROID

            //AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.CancelAllNotifications();

            var notification = new AndroidNotification();
            notification.Title = "Don’t miss out!";
            notification.Text = "You have new daily tasks available!";
            notification.FireTime = System.DateTime.Now.AddMinutes(minutes);
            //notification.FireTime = System.DateTime.Now.AddMinutes(3);

            notification.SmallIcon = "icon_small";
            notification.LargeIcon = "icon_big";

            var id = AndroidNotificationCenter.SendNotification(notification, ID_DAILYTASK);

            //if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
            //{
            //    AndroidNotificationCenter.CancelAllNotifications();
            //    AndroidNotificationCenter.SendNotification(notification, "offline_earn");
            //}

#endif
        }

        private void CreateChannelDailyTask()
        {
#if UNITY_ANDROID
            var channel = new AndroidNotificationChannel()
            {
                Id = ID_DAILYTASK,
                Name = "Daily Task",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif

            //var channel = new GameNotificationChannel(ChannelId, "Default Game Channel", "Generic notifications");
            //manager.Initialize(channel);
        }




        public void Dispose()
        {

        }


    }
}
