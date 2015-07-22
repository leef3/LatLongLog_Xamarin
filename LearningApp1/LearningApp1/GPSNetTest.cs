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
using System.Timers;

using Android.Locations;

namespace LearningApp1
{
    [Activity(Label = "GPSNetTest")]
    public class GPSNetTest : Activity, ILocationListener
    {
        //Same variables without the 2 are in the Accelerometer Test page
        private TextView gpsStatus2, gpsAccuracy2, gpsLatitude2, gpsLongitude2, gpsSpeed2, gpsTime2, gpsContact2, gpsLastUpdate2;
        private TextView networkStatus, networkAccuracy, networkLatitude, networkLongitude, networkSpeed, networkTime, networkContact, networkLastUpdate;
        private TextView gpsCount2, networkCount;

        //GPS
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;

        //Timer and Counters
        System.Timers.Timer timeElapsed;
        DateTime timeStart;
        DateTime currentTime;
        int numGPS, numNetwork;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.GPSNetMain);

            gpsStatus2 = (TextView)FindViewById(Resource.Id.gpsStatus2);
            gpsAccuracy2 = (TextView)FindViewById(Resource.Id.gpsAccuracy2);
            gpsLatitude2 = (TextView)FindViewById(Resource.Id.gpsLatitude2);
            gpsLongitude2 = (TextView)FindViewById(Resource.Id.gpsLongitude2);
            gpsSpeed2 = (TextView)FindViewById(Resource.Id.gpsSpeed2);
            gpsTime2 = (TextView)FindViewById(Resource.Id.gpsTime2);
            gpsContact2 = (TextView)FindViewById(Resource.Id.gpsContact2);
            gpsLastUpdate2 = (TextView)FindViewById(Resource.Id.gpsLastUpdate2);
            gpsCount2 = (TextView)FindViewById(Resource.Id.gpsCount2);

            networkStatus = (TextView)FindViewById(Resource.Id.networkStatus);
            networkAccuracy = (TextView)FindViewById(Resource.Id.networkAccuracy);
            networkLatitude = (TextView)FindViewById(Resource.Id.networkLatitude);
            networkLongitude = (TextView)FindViewById(Resource.Id.networkLongitude);
            networkSpeed = (TextView)FindViewById(Resource.Id.networkSpeed);
            networkTime = (TextView)FindViewById(Resource.Id.networkTime);
            networkContact = (TextView)FindViewById(Resource.Id.networkContact);
            networkLastUpdate = (TextView)FindViewById(Resource.Id.networkLastUpdate);
            networkCount = (TextView)FindViewById(Resource.Id.networkCount);

            numGPS = 0;
            numNetwork = 0;
            gpsCount2.Text = numGPS.ToString();
            networkCount.Text = numNetwork.ToString();

            InitalizeLocationManager();
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            //GPS
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);
            locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 0, this);

            timeStart = DateTime.Now.ToLocalTime();

            timeElapsed = new System.Timers.Timer(1000);
            timeElapsed.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            timeElapsed.Enabled = true;
            timeElapsed.Start();

            

        }
        protected override void OnPause()
        {
            base.OnPause();
            //GPS
            locationManager.RemoveUpdates(this);
            
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
            currentLocation = location;
            if (currentLocation != null)
            {
                if(currentLocation.Provider == LocationManager.GpsProvider)
                {
                    gpsLatitude2.Text = currentLocation.Latitude.ToString();
                    gpsLongitude2.Text = currentLocation.Longitude.ToString();
                    gpsSpeed2.Text = currentLocation.Speed.ToString();
                    gpsAccuracy2.Text = currentLocation.Accuracy.ToString() + " Meters";

                    if (numGPS == 0)
                    {
                        gpsContact2.Text = (currentTime - timeStart).TotalSeconds.ToString();
                        gpsLastUpdate2.Text = (currentTime - timeStart).TotalSeconds.ToString();
                    }
                    else
                    {
                        gpsLastUpdate2.Text = (currentTime - timeStart).TotalSeconds.ToString();
                    }

                    ++numGPS;
                    gpsCount2.Text = numGPS.ToString();
                }
                else if(currentLocation.Provider == LocationManager.NetworkProvider)
                {
                    networkLatitude.Text = currentLocation.Latitude.ToString();
                    networkLongitude.Text = currentLocation.Longitude.ToString();
                    networkSpeed.Text = currentLocation.Speed.ToString();
                    networkAccuracy.Text = currentLocation.Accuracy.ToString() + " Meters";

                    if(numNetwork == 0)
                    {
                        networkContact.Text = (currentTime - timeStart).TotalSeconds.ToString();
                        networkLastUpdate.Text = (currentTime - timeStart).TotalSeconds.ToString();
                    }
                    else
                    {
                        networkLastUpdate.Text = (currentTime - timeStart).TotalSeconds.ToString();
                    }

                    ++numNetwork;
                    networkCount.Text = numNetwork.ToString();
                }
                
            } 

        }
        public void OnProviderDisabled(string provider)
        { }
        public void OnProviderEnabled(string provider)
        {
            gpsStatus2.Text = "Provider Enabled";
        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if(provider == LocationManager.GpsProvider)
            {
                gpsStatus2.Text = status.ToString();
            }
            else if(provider == LocationManager.NetworkProvider)
            {
                networkStatus.Text = status.ToString();
            }
        }
        //======================================Timer==========================
        private void OnTimeEvent(object source, ElapsedEventArgs e)
        {
            RunOnUiThread(delegate
            {
                currentTime = DateTime.Now.ToLocalTime();
                gpsTime2.Text = (DateTime.Now.ToLocalTime() - timeStart).TotalSeconds.ToString();
                networkTime.Text = (DateTime.Now.ToLocalTime() - timeStart).TotalSeconds.ToString();
            });
        }
    }

}