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

using Android.Util;


namespace LearningApp1
{
    [Activity(Label = "AccelerometerTest")]
    public class AccelerometerTest : Activity, ISensorEventListener, ILocationListener
    {
        private SensorManager mSensorManager;
        private Sensor mSensor;
        private Sensor mLinearSensor;
        private static readonly object _syncLock = new object();

        //UI Elements
        private TextView accelStatus, accelStrength, accelX, accelY, accelZ;
        private TextView linearStatus, linearStrength, linearX, linearY, linearZ;
        private TextView gpsStatus, gpsProvider, gpsLatitude, gpsLongitude, gpsSpeed;

        //GPS
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AccelerometerMain);

            //Normal Accelerometer (Pure Hardware)
            accelStatus = (TextView)FindViewById(Resource.Id.accelStatus);
            accelStrength = (TextView)FindViewById(Resource.Id.accelStrength);
            accelX = (TextView)FindViewById(Resource.Id.accelX);
            accelY = (TextView)FindViewById(Resource.Id.accelY);
            accelZ = (TextView)FindViewById(Resource.Id.accelZ);

            //Linear Accelerometer (Software Adjusted)
            linearStatus = (TextView)FindViewById(Resource.Id.linearStatus);
            linearStrength = (TextView)FindViewById(Resource.Id.linearStrength);
            linearX = (TextView)FindViewById(Resource.Id.linearX);
            linearY = (TextView)FindViewById(Resource.Id.linearY);
            linearZ = (TextView)FindViewById(Resource.Id.linearZ);

            //GPS
            gpsStatus = (TextView)FindViewById(Resource.Id.gpsStatus);
            gpsProvider = (TextView)FindViewById(Resource.Id.gpsProvider);
            gpsLatitude = (TextView)FindViewById(Resource.Id.gpsLatitude);
            gpsLongitude = (TextView)FindViewById(Resource.Id.gpsLongitude);
            gpsSpeed = (TextView)FindViewById(Resource.Id.gpsSpeed);
            InitalizeLocationManager();

            //List<Sensor> deviceSensors = new List<Sensor>(mSensorManager.GetSensorList(SensorType.All));

            //Sensor Initializations
            mSensorManager = (SensorManager)GetSystemService(Context.SensorService);               
            if(mSensorManager.GetDefaultSensor(SensorType.Accelerometer) != null)
            {
                accelStatus.Text = "EXISTS";
                mSensor = mSensorManager.GetDefaultSensor(SensorType.Accelerometer);
            }
            else
            {
                accelStatus.Text = "NOT AVAILABLE";
            }

            if (mSensorManager.GetDefaultSensor(SensorType.LinearAcceleration) != null)
            {
                linearStatus.Text = "EXISTS";
                mLinearSensor = mSensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            }
            else
            {
                linearStatus.Text = "NOT AVAILABLE";
            }

        }

        protected override void OnResume()
        {
            base.OnResume();

            mSensorManager.RegisterListener(this, mSensor, SensorDelay.Ui);
            mSensorManager.RegisterListener(this, mLinearSensor, SensorDelay.Ui);

            //GPS
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            mSensorManager.UnregisterListener(this);
            
            //GPS
            locationManager.RemoveUpdates(this);
        }
        //===============================================ACCELEROMETER INTERFACE METHODS===================
        public void OnSensorChanged(SensorEvent e)
        {
            lock(_syncLock)
            {
                if(e.Sensor == mSensor)
                {
                    accelStrength.Text = e.Accuracy.ToString();
                    accelX.Text = e.Values[0].ToString();
                    accelY.Text = e.Values[1].ToString();
                    accelZ.Text = e.Values[2].ToString();
                }
                else if(e.Sensor == mLinearSensor)
                {
                    linearStrength.Text = e.Accuracy.ToString();
                    linearX.Text = e.Values[0].ToString();
                    linearY.Text = e.Values[1].ToString();
                    linearZ.Text = e.Values[2].ToString();
                }
                
            }
        }

        public void OnAccuracyChanged(Sensor Sensor, SensorStatus accuracy)
        {
            accelStrength = (TextView)FindViewById(Resource.Id.accelStrength);
            //Correlate to ENUMS later... for now just display strong...
        }
        //===============================================GPS INTERFACE METHODS===============================
        private void InitalizeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            if(locationManager != null)
            {
                gpsStatus.Text = "Location Manager Found";
            }

            Criteria criteriaForLocation = new Criteria { Accuracy = Accuracy.Low };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocation, true);
            if(acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
                gpsProvider.Text = locationProvider;
            }
            else
            {
                locationProvider = "";
                
            }
        }

        public void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if(currentLocation != null)
            {
                gpsLatitude.Text = currentLocation.Latitude.ToString();
                gpsLongitude.Text = currentLocation.Longitude.ToString();
                gpsSpeed.Text = currentLocation.Speed.ToString();
            }
        }
        public void OnProviderDisabled(string provider)
        { }
        public void OnProviderEnabled(string provider)
        {
            gpsStatus.Text = "Provider Enabled";
        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        { }
    }
}