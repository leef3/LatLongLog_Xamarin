using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Collections.Generic;

namespace LearningApp1
{
    [Activity(Label = "LearningApp1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        Button LoginUser1Button;
        Button LoginUser2Button;
        Button LogoutUser1Button;
        Button LogoutUser2Button;
        Button toAccelerometerButton;
        Button toMotionStopButton;
        Button toGPSNetButton;

        User driver;
        User aid;

        List<string> eventLog;
        ListView eventList;
        ArrayAdapter<string> adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //To the next feature
            toAccelerometerButton = (Button)FindViewById(Resource.Id.toAccelerometer);
            toAccelerometerButton.Click += delegate
            {
                StartActivity(typeof(AccelerometerTest));
            };

            toMotionStopButton = (Button)FindViewById(Resource.Id.toMotionStop);
            toMotionStopButton.Click += delegate
            {
                StartActivity(typeof(MotionStopTest));
            };

            toGPSNetButton = (Button)FindViewById(Resource.Id.toGPSNetTest);
            toGPSNetButton.Click += delegate
            {
                StartActivity(typeof(GPSNetTest));
            };

            /*
            eventLog = new List<string>();
            eventLog.Add("TEST");
            eventList = (ListView)FindViewById(Resource.Id.listView1);
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, eventLog);
            eventList.Adapter = adapter;
             * */

            LoginUser1Button = (Button)FindViewById(Resource.Id.loginUser1Button);
            LoginUser1Button.Click += delegate
            {
                driver = new User("William Hunter", "DRIVER", 3);
                Update(driver);
                LoginUser1Button.Enabled = false;
                TestAlarm();
            };

            LoginUser2Button = (Button)FindViewById(Resource.Id.loginUser2Button);
            LoginUser2Button.Click += delegate
            {
                aid = new User("Joseph Pratt", "AID", 4);
                Update(aid);
                LoginUser2Button.Enabled = false;
                TestAlarm();
            };


            LogoutUser1Button = (Button)FindViewById(Resource.Id.logoutUser1Button);
            LogoutUser1Button.Click += delegate
            {
                if(LoginUser1Button.Enabled == false)
                {
                    EditText timeWorked1 = (EditText)FindViewById(Resource.Id.timeWorked1);
                    if (timeWorked1.Text != null)
                    {
                        driver.SetTimeWorked(Int32.Parse(timeWorked1.Text));
                        Update(driver);
                    }
                    else
                    {
                        driver.SetTimeWorked(0);
                        Update(driver);
                    }

                    ShowNotification(driver);
                    LoginUser1Button.Enabled = true;
                }
                
            };

            LogoutUser2Button = (Button)FindViewById(Resource.Id.logoutUser2Button);
            LogoutUser2Button.Click += delegate
            {
                if (LoginUser2Button.Enabled == false)
                {
                    EditText timeWorked2 = (EditText)FindViewById(Resource.Id.timeWorked2);
                    if (timeWorked2.Text != null)
                    {
                        aid.SetTimeWorked(Int32.Parse(timeWorked2.Text));
                        Update(aid);
                    }
                    else
                    {
                        aid.SetTimeWorked(0);
                        Update(aid);
                    }

                    ShowNotification(aid);
                    LoginUser2Button.Enabled = true;
                }
            };

        }

        protected void Update(User toUpdate)
        {
            TextView username = (TextView)FindViewById(Resource.Id.username);
            TextView logintime = (TextView)FindViewById(Resource.Id.logintime);
            TextView timeworked = (TextView)FindViewById(Resource.Id.timeworked);
            TextView breaktime = (TextView)FindViewById(Resource.Id.breaktime);

            username.Text = toUpdate.GetName();
            logintime.Text = toUpdate.GetSignOn().ToString();
            timeworked.Text = toUpdate.GetTimeWorked().ToString();
            breaktime.Text = toUpdate.GetBreakTime().ToString();

            TextView acknowledged = (TextView)FindViewById(Resource.Id.acknowledged);
            acknowledged.Text = "Not Acknowledged";

        }

        protected void TestAlarm()
        {
            Intent i = new Intent(this, typeof(TimeService));
            PendingIntent pendingIntent = PendingIntent.GetService(this, 0, i, 0);
            AlarmManager alarmManager = (AlarmManager)this.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime()+10000, pendingIntent);
        }

        public void ShowNotification(User employee)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Break Time for: " + employee.GetName());
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage("You have been working " + employee.GetTimeWorked() + " hours, You are entitled to a " + employee.GetBreakTime() + " minute break");

            alertDialog.SetButton("OK", (s, ev) =>
            {
                //Record Acknowledgement
                Toast.MakeText(this, "Break Acknowledged", ToastLength.Short).Show();

                TextView acknowledged = (TextView)FindViewById(Resource.Id.acknowledged);
                acknowledged.Text = "Acknowledged at " + DateTime.Now.ToLocalTime().ToShortTimeString();

            });

            alertDialog.Show();
        }


    }
}

