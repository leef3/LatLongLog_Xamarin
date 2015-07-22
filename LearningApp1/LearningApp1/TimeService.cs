using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LearningApp1
{
    [Service]
    public class TimeService : Service
    {
        System.Threading.Timer timer;

        public override void OnStart(Android.Content.Intent intent, int startId)
        {
            base.OnStart(intent, startId);
            ShowNotification();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            timer.Dispose();
        }

        protected void ShowNotification()
        {
            /*
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle("Break Time!")
                .SetContentText("You have passed a 3.5 hours and should take your 10 minute break if possible")
                .SetSmallIcon(Resource.Drawable.coffee);

            Notification notification = builder.Build();
            NotificationManager notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
             * 
             * */
            Notification notification;

            RemoteViews bigView = new RemoteViews(ApplicationContext.PackageName, Resource.Layout.notification_large);

            //PendingIntent testIntent = PendingIntent.GetActivities

            Notification.Builder builder = new Notification.Builder(this);
            notification = builder.SetContentTitle("Accrued Break Time")
                .SetContentText("You have reached a certain threshold to take a required break. Slide down for more details")
                .SetSmallIcon(Resource.Drawable.bussmall)
                .SetLargeIcon(Android.Graphics.BitmapFactory.DecodeResource(Resources, Resource.Drawable.bus))
                .Build();

            notification.BigContentView = bigView;

            NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService) as NotificationManager;
            manager.Notify(0, notification);

        }


        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }
    }
         
}