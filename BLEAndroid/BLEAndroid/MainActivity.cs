﻿using Android.App;
using Android.Widget;
using Android.OS;
using Java;
using Java.Util;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Views;
using Java.Lang;
using System;
using Android.Content;
using Android.Graphics;
using Android.Bluetooth.LE;
using Android.Bluetooth;
using Android.Util;
using System.Threading.Tasks;

namespace BLEAndroid
{
    [Activity(Label = "BLEAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // about _Y_listview
        private ListView mListView;
        private List<ListItem> mList;
        private bool isSearching = false;
        protected ListView _listView;
        protected ScanButton _scanButton;
        protected DevicesAdapter _listAdapter;
        protected ProgressDialog _progress;
        protected BluetoothDevice _deviceToConnect; //not using State.SelectedDevice because it may not be connected yet

        // external handlers
        EventHandler<BluetoothLEManager.DeviceDiscoveredEventArgs> deviceDiscoveredHandler;
        EventHandler<BluetoothLEManager.DeviceConnectionEventArgs> deviceConnectedHandler;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this._listView = FindViewById<ListView>(Resource.Id.searchList);
            this._scanButton = FindViewById<ScanButton>(Resource.Id.searchbutton);

            // create our list adapter
            this._listAdapter = new DevicesAdapter(this, BluetoothLEManager.Current.DiscoveredDevices);
            this._listView.Adapter = this._listAdapter;
            // about _Z_listview 
            mListView = (ListView)FindViewById(Resource.Id.deviceList);
            mList = new List<ListItem>();
            //MainLi
        }

        public void button1_onClick()
        {
            EditText et = (EditText)FindViewById(Resource.Id.text1);
            //////
        }

        // about _Y_BLE


        protected override void OnResume()
        {
            base.OnResume();

            this.WireupLocalHandlers();
            this.WireupExternalHandlers();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // stop our scanning (does a check, and also runs async)
            this.StopScanning();

            // unwire external event handlers (memory leaks)
            this.RemoveExternalHandlers();
        }

        protected void WireupLocalHandlers()
        {
            this._scanButton.Click += (object sender, EventArgs e) => {
                if (!BluetoothLEManager.Current.IsScanning)
                {
                    BluetoothLEManager.Current.BeginScanningForDevices();
                    mListView.Visibility = ViewStates.Invisible;
                    _listView.Visibility = ViewStates.Visible;
                }
                else
                {
                    BluetoothLEManager.Current.StopScanningForDevices();
                    mListView.Visibility = ViewStates.Visible;
                    _listView.Visibility = ViewStates.Invisible;
                }
            };

            this._listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
                Console.Write("ItemClick: " + this._listAdapter.Items[e.Position]);

                // stop scanning
                this.StopScanning();

                // select the item
                this._listView.ClearFocus();
                this._listView.Post(() => {
                    this._listView.SetSelection(e.Position);
                });
                //this._listView.SetItemChecked (e.Position, true);
                // todo: for some reason, we're losing the selection, so i have to cache it
                // think i know the issue, see the note in the GenericObjectAdapter class
                this._deviceToConnect = this._listAdapter.Items[e.Position];

                // show a connecting overlay
                // TODO: make this conform to lifecycle, see: https://github.com/xamarin/private-samples/blob/master/EvolveCurriculum/Advanced/10%20-%20Advanced%20Android%20Application%20Lifecycle/ActivityLifecycle/MainActivity.cs
                this.RunOnUiThread(() => {
                    //TODO: we need to save a ref to the device when click
                    this._progress = ProgressDialog.Show(this, "Connecting", "Connecting to " + this._deviceToConnect.Name, true);
                });

                // try and connect
                BluetoothLEManager.Current.ConnectToDevice(this._listAdapter[e.Position]);

            };
        }

        protected void WireupExternalHandlers()
        {
            this.deviceDiscoveredHandler = (object sender, BluetoothLEManager.DeviceDiscoveredEventArgs e) => {
                Console.WriteLine("Discovered device: " + e.Device.Name);

                // reload the list view
                //TODO: why doens't NotifyDataSetChanged work? is it because i'm replacing the reference?
                this.RunOnUiThread(() => {
                    this._listAdapter = new DevicesAdapter(this, BluetoothLEManager.Current.DiscoveredDevices);
                    this._listView.Adapter = this._listAdapter;
                });
            };
            BluetoothLEManager.Current.DeviceDiscovered += this.deviceDiscoveredHandler;

            this.deviceConnectedHandler = (object sender, BluetoothLEManager.DeviceConnectionEventArgs e) => {
                this.RunOnUiThread(() => {
                    this._progress.Hide();
                });
                // now that we're connected, save it
                App.Current.State.SelectedDevice = e.Device;

                // launch the details screen
                this.StartActivity(typeof(DeviceDetailsScreen));
            };
            BluetoothLEManager.Current.DeviceConnected += this.deviceConnectedHandler;
        }

        protected void RemoveExternalHandlers()
        {
            BluetoothLEManager.Current.DeviceDiscovered -= this.deviceDiscoveredHandler;
            BluetoothLEManager.Current.DeviceConnected -= this.deviceConnectedHandler;
        }

        protected void StopScanning()
        {
            // stop scanning
            new Task(() => {
                if (BluetoothLEManager.Current.IsScanning)
                {
                    Console.WriteLine("Still scanning, stopping the scan and reseting the right button");
                    BluetoothLEManager.Current.StopScanningForDevices();
                    this._scanButton.SetState(ScanButton.ScanButtonState.Normal);
                }
            }).Start();
        }
    


    class MainListViewAdapter : BaseAdapter
        {
            private List<ListItem> mList;
            private Context context;
            private ListItemView listItemView;
            public MainListViewAdapter(Context mcontext,List<ListItem> list)
            {
                mList = list;
                context = mcontext;
            }
            public override int Count => mList.Count;


            public override Java.Lang.Object GetItem(int position)
            {
                throw new NotImplementedException();
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {

                convertView = LayoutInflater.From(context).Inflate(Resource.Layout.CardiographView, null);
                listItemView = new ListItemView();
                listItemView.cardiographview = (CardiographView)convertView.FindViewById(Resource.Id.background);
                listItemView.pathview = (PathView)convertView.FindViewById(Resource.Id.path);
                //?Path path=mList[position]
                return convertView;
            }
        }

        class ListItemView
        {
            public CardiographView cardiographview;
            public PathView pathview;
        }

        class ListItem
        {
           
        }
    }
}

