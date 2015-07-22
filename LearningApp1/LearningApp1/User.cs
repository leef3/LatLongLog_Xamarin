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
    public class User
    {
        private string type;
        private string username;
        private int baseHours;
        private DateTime signOn, signOff;
        private string status;
        private int breaktime;
        
        public User(string userName, string typeIn, int baseHoursIn)
        {
            type = typeIn;
            baseHours = baseHoursIn;
            signOn = DateTime.Now.ToLocalTime();
            status = "ON";
            breaktime = 0;
            username = userName;
        }
        
        //Must call Singoff before GetTimeWorked()
        public void SignOff()
        {
            signOff = DateTime.Now.ToLocalTime();
            status = "OFF";
        }

        public double GetTimeWorked()
        {
            if(signOff == null)
            {
                return (DateTime.Now.ToLocalTime() - signOn).TotalHours;
            }
            return (signOff - signOn).TotalHours;
        }

        //Breaks base on Calif law
        public double GetBreakTime()
        {
            double timeWorked = GetTimeWorked();
            if(timeWorked < 3.5)
            {
                breaktime = 0;
                return breaktime;
            }
            else if((3.6 < timeWorked) && (timeWorked < 6))
            {
                breaktime = 10;
                return breaktime;
            }
            else if((6 < timeWorked) && (timeWorked < 10))
            {
                breaktime = 20;
                return breaktime;
            }
            else if((10 < timeWorked) && (timeWorked < 14))
            {
                breaktime = 30;
                return breaktime;
            }
            else
            {
                return 0;
            }
        }

        public string GetName()
        {
            return username;
        }

        //Return sign on time
        public string GetSignOn()
        {
            return signOn.ToShortTimeString();
        }

        //Testing Purposes to set the time to test break calculation
        public void SetTimeWorked(int timeToAdd)
        {
            signOff = DateTime.Now.ToLocalTime().AddHours(timeToAdd);
        }
    }
}