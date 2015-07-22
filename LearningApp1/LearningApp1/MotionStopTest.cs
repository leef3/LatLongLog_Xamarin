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

using Android.Hardware;

using Android.Locations;

namespace LearningApp1
{
    
    [Activity(Label = "MotionStopTest")]
    public class MotionStopTest : Activity, ISensorEventListener, ILocationListener
    {
        private SensorManager mSensorManager;
        private Sensor mSensor;
        private Sensor mLightSensor;

        //GPS
        LocationManager locationManager;
        string locationProvider;
        bool blackoutOn;

        private TextView accelX, accelY, accelZ, threshold, currentSpeed;

        //Ambient Light Sensor
        private TextView ambientLight;

        private Switch blackoutSwitch;

        private LinearLayout blackout;

        private static readonly object _syncLock = new object();
        

        //Changing to Night Mode
        private LinearLayout overallLayout, subLayout1, subLayout2, subLayout3;
        private TextView title1, title2, title3;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MotionStopMain);

            //Speed and Accelerometer UI data
            accelX = (TextView)FindViewById(Resource.Id.blackoutAccelX);
            accelY = (TextView)FindViewById(Resource.Id.blackoutAccelY);
            accelZ = (TextView)FindViewById(Resource.Id.blackoutAccelZ);
            threshold = (TextView)FindViewById(Resource.Id.threshold);
            blackout = (LinearLayout)FindViewById(Resource.Id.blackout);
            currentSpeed = (TextView)FindViewById(Resource.Id.currentSpeed);
            ambientLight = (TextView)FindViewById(Resource.Id.ambientLight);

            //LightSensor Changes
            overallLayout = (LinearLayout)FindViewById(Resource.Id.overallLayout);
            subLayout1 = (LinearLayout)FindViewById(Resource.Id.subLayout1);
            subLayout2 = (LinearLayout)FindViewById(Resource.Id.subLayout2);
            subLayout3 = (LinearLayout)FindViewById(Resource.Id.subLayout3);
            title1 = (TextView)FindViewById(Resource.Id.title1);
            title2 = (TextView)FindViewById(Resource.Id.title2);
            title3 = (TextView)FindViewById(Resource.Id.title3);


            //UI Switch to toggle driver distraction blackout feature
            blackoutOn = false;

            blackoutSwitch = (Switch)FindViewById(Resource.Id.gpsBlackoutSwitch);
            
            blackoutSwitch.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                blackoutOn = blackoutSwitch.Checked;
            };
            
            //Need this to initialize the location listener
            InitalizeLocationManager();

            //Check for sensors
            mSensorManager = (SensorManager)GetSystemService(Context.SensorService);

            if (mSensorManager.GetDefaultSensor(SensorType.LinearAcceleration) != null)
            {
                mSensor = mSensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            }
            if (mSensorManager.GetDefaultSensor(SensorType.Light) != null)
            {
                mLightSensor = mSensorManager.GetDefaultSensor(SensorType.Light);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();

            //Register Sensors
            mSensorManager.RegisterListener(this, mSensor, SensorDelay.Ui);
            mSensorManager.RegisterListener(this, mLightSensor, SensorDelay.Ui);

            //GPS
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);

        }
        protected override void OnPause()
        {
            base.OnPause();
            //Unregister Sensors
            mSensorManager.UnregisterListener(this);

            //GPS
            locationManager.RemoveUpdates(this);
        }

        
        public void OnSensorChanged(SensorEvent e)
        {
            lock (_syncLock)
            {
                if(e.Sensor.Type == SensorType.LinearAcceleration)
                {
                    accelX.Text = e.Values[0].ToString();
                    accelY.Text = e.Values[1].ToString();
                    accelZ.Text = e.Values[2].ToString();
                }
                    //Test for changing Material Dark theme for Night Time mode
                else if(e.Sensor.Type == SensorType.Light)
                {
                    ambientLight.Text = e.Values[0].ToString();

                    if(e.Values[0] > 50.0)
                    {
                        title1.SetTextColor(Android.Graphics.Color.ParseColor("#000000"));
                        title2.SetTextColor(Android.Graphics.Color.ParseColor("#000000"));
                        title3.SetTextColor(Android.Graphics.Color.ParseColor("#000000"));

                        overallLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FFFFFF"));
                        subLayout1.SetBackgroundColor(Android.Graphics.Color.ParseColor("#EEEEEE"));
                        subLayout2.SetBackgroundColor(Android.Graphics.Color.ParseColor("#EEEEEE"));
                        subLayout3.SetBackgroundColor(Android.Graphics.Color.ParseColor("#EEEEEE"));

                        
                    }
                    else
                    {
                        title1.SetTextColor(Android.Graphics.Color.ParseColor("#EEEEEE"));
                        title2.SetTextColor(Android.Graphics.Color.ParseColor("#EEEEEE"));
                        title3.SetTextColor(Android.Graphics.Color.ParseColor("#EEEEEE"));

                        overallLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor("#212121"));
                        subLayout1.SetBackgroundColor(Android.Graphics.Color.ParseColor("#607D8B"));
                        subLayout2.SetBackgroundColor(Android.Graphics.Color.ParseColor("#607D8B"));
                        subLayout3.SetBackgroundColor(Android.Graphics.Color.ParseColor("#607D8B"));
                    }
                }
                

                //Try purely based on GPS for now
                /*
                if(e.Values[1] > 0.5)
                {
                    thresholdReached();
                    threshold.Text = "Reached";
                    blackout.Visibility = ViewStates.Visible;
                }
                else
                {
                    threshold.Text = "Not Reached";
                    blackout.Visibility = ViewStates.Gone;
                }
                 * */
            }
        }

        public void OnAccuracyChanged(Sensor Sensor, SensorStatus accuracy)
        {

        }

        public void thresholdReached()
        {
            //Stuff for reaching threshold... Idk maybe remove the navigation view or something
        }

        //===============================================GPS INTERFACE METHODS===============================
        private void InitalizeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocation = new Criteria { Accuracy = Accuracy.Low };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocation, true);
            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = "";
            }
        }

        public void OnLocationChanged(Location location)
        {
            currentSpeed.Text = location.Speed.ToString();
            if(blackoutOn == true)
            {
                //Blackout LinearLayout pushes out other view if speed is >2m/s
                if (location.Speed > 2.0)
                {
                    blackout.Visibility = ViewStates.Visible;
                }
                else
                {
                    blackout.Visibility = ViewStates.Gone;
                }
            }
            
        }
        public void OnProviderDisabled(string provider)
        { }
        public void OnProviderEnabled(string provider)
        { }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        { 
            if(status != Availability.Available)
            {
                //Screen Freeze, but I think it naturally does that because screen changes with readings..
            }
        }

    }
}