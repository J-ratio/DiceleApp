using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AndroidNotificationChannel notificationChannel = new AndroidNotificationChannel(){
            Id = "example_channel",
            Name = "example_channel_name",
            Importance = Importance.High,
            Description = "Generic",
        };
    

        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);

        AndroidNotification notification = new AndroidNotification();

        notification.Title = "title";
        notification.Text = "text";
        notification.ShowTimestamp = true;
        notification.RepeatInterval = new TimeSpan();
        notification.FireTime = System.DateTime.Now.AddSeconds(10);
        Invoke("Log",10);
        Log();

        var identifier = AndroidNotificationCenter.SendNotification(notification, "example_channel");

        if(AndroidNotificationCenter.CheckScheduledNotificationStatus (identifier) == NotificationStatus.Scheduled) {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification,  "example_channel");
        }


        void Log()
        {
            Debug.Log("11");
        }


    }


}
