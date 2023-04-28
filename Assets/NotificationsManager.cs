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

        AndroidNotification two_day_notif= new AndroidNotification();

        int r = UnityEngine.Random.Range(0, 7);

        switch(r) 
        {
        case 0:
            two_day_notif.Title = "Train your brain 🧠 🧠";
        two_day_notif.Text = "Solve today's puzzle now!";
            break;
        case 1:
            two_day_notif.Title = "Start brain yoga 🧘";
        two_day_notif.Text = "...in 3..2..1. Today's puzzle is live 🎲🎲";
            break;
        case 2:
            two_day_notif.Title = "🧠 🏋️🧠 🏋️🧠 🏋️";
        two_day_notif.Text = "Time for a brain workout";
            break;
        case 3:
            two_day_notif.Title = "Can you hear the applause?! 👏🎇";
        two_day_notif.Text = "That's us cheering for you to solve today's puzzle";
            break;
        case 4:
            two_day_notif.Title = "Today's Dicele is Live 💡";
        two_day_notif.Text = "Can you solve it? 🤯";
            break;
        case 5:
            two_day_notif.Title = "☀️☀️ Good Morning ☀️☀️";
        two_day_notif.Text = "☕ or Dicele? Play New Puzzle Daily";
            break;
        case 6:
            two_day_notif.Title = "Become a Global celebrity! 🤝";
        two_day_notif.Text = "Solve the puzzle and feature in leaderboard 🌎🏆";
            break;
        }

        two_day_notif.ShowTimestamp = true;
        two_day_notif.RepeatInterval = new TimeSpan(48,0,0);
        two_day_notif.FireTime = System.DateTime.Now.AddHours(2);

        var identifier0 = AndroidNotificationCenter.SendNotification(two_day_notif, "example_channel");

        if(AndroidNotificationCenter.CheckScheduledNotificationStatus (identifier0) == NotificationStatus.Scheduled) {
            AndroidNotificationCenter.CancelAllNotifications();

            r = UnityEngine.Random.Range(0, 7);

            switch(r) 
            {
            case 0:
                two_day_notif.Title = "Train your brain 🧠 🧠";
            two_day_notif.Text = "Solve today's puzzle now!";
                break;
            case 1:
                two_day_notif.Title = "Start brain yoga 🧘";
            two_day_notif.Text = "...in 3..2..1. Today's puzzle is live 🎲🎲";
                break;
            case 2:
                two_day_notif.Title = "🧠 🏋️🧠 🏋️🧠 🏋️";
            two_day_notif.Text = "Time for a brain workout";
                break;
            case 3:
                two_day_notif.Title = "Can you hear the applause?! 👏🎇";
            two_day_notif.Text = "That's us cheering for you to solve today's puzzle";
                break;
            case 4:
                two_day_notif.Title = "Today's Dicele is Live 💡";
            two_day_notif.Text = "Can you solve it? 🤯";
                break;
            case 5:
                two_day_notif.Title = "☀️☀️ Good Morning ☀️☀️";
            two_day_notif.Text = "☕ or Dicele? Play New Puzzle Daily";
                break;
            case 6:
                two_day_notif.Title = "Become a Global celebrity! 🤝";
            two_day_notif.Text = "Solve the puzzle and feature in leaderboard 🌎🏆";
                break;
            }

            AndroidNotificationCenter.SendNotification(two_day_notif,  "example_channel");
        }


        AndroidNotification trophy_notif = new AndroidNotification();

        trophy_notif.ShowTimestamp = true;
        var date = DateTime.Now;
        var firstDay = new DateTime(date.Year, date.Month, 1);
        trophy_notif.FireTime = firstDay.AddMonths(1);
        trophy_notif.Title= "New Trophies Unlocked 🏆🏆🏆";
        trophy_notif.Text = "Bronze! Silver! or Gold! Which trophy are you aiming for this month? ";

        var identifier1 = AndroidNotificationCenter.SendNotification(trophy_notif, "example_channel");
        if(AndroidNotificationCenter.CheckScheduledNotificationStatus (identifier1) == NotificationStatus.Scheduled) {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(trophy_notif,  "example_channel");
        }




        AndroidNotification daily_notif = new AndroidNotification();

        daily_notif.ShowTimestamp = true;
        var startDay = new DateTime(date.Year, date.Month, date.Day);
        daily_notif.FireTime = startDay.AddHours(4);
        daily_notif.RepeatInterval = new TimeSpan(24,0,0);
        daily_notif.Title= "🎲 Daily Dicele Updated!! 🎲";
        daily_notif.Text = "Solve to find where you stand in the 🌍";

        var identifier2 = AndroidNotificationCenter.SendNotification(daily_notif, "example_channel");
        if(AndroidNotificationCenter.CheckScheduledNotificationStatus (identifier2) == NotificationStatus.Scheduled) {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(daily_notif,  "example_channel");
        }



        

    }


}
