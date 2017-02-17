using Android.App;
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
//using Java.IO;

namespace BLEAndroid
{
    [Activity(Label = "BLEAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // about _Y_listview
        private ListView mListDeviceView;
        private List<mDeviceClass> mListDevice;
        private CardiographAdapter mCardiographAdapter;
        private bool isSearching = false;
        protected ListView _listView;
        protected ScanButton _scanButton;
        protected Button _sendButton;
        protected Button _saveButton;
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
            this._sendButton = FindViewById<Button>(Resource.Id.SendButton);
            _sendButton.Click += _sendButton_Click;
            //_saveButton.Click += _saveButton_Click;
            // create our list adapter
            this._listAdapter = new DevicesAdapter(this, BluetoothLEManager.Current.DiscoveredDevices);
            this._listView.Adapter = this._listAdapter;
            // about _Z_listview 
            this.mListDeviceView = (ListView)FindViewById(Resource.Id.deviceList);
            this.mListDevice = new List<mDeviceClass>();
            this.mCardiographAdapter = new CardiographAdapter(this, mListDevice);
            this.mListDeviceView.Adapter = mCardiographAdapter;
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            ////File file = getFilesDir();
            //Date date = new Date();
            //File file = new File(,date.ToString());
            //OpenFileOutput()
        }

        private void _sendButton_Click(object sender, EventArgs e)
        {
            EditText et = FindViewById<EditText>(Resource.Id.text1);
            string tmp = et.Text;
            foreach (var item in mListDevice)
            {
                item.SendMessage(tmp);
                Console.WriteLine($"send {tmp} to {item.mBluetoothGatt.Device.Name}");
            }
            Console.WriteLine("SendTask Finished");
            et.Text = "";
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
            this._scanButton.Click += (object sender, EventArgs e) =>
            {
                if (!isSearching)
                {
                    mListDeviceView.Visibility = ViewStates.Gone;
                    _listView.Visibility = ViewStates.Visible;
                    BluetoothLEManager.Current.BeginScanningForDevices();
                    isSearching = true;
                }
                else
                {
                    mListDeviceView.Visibility = ViewStates.Visible;
                    _listView.Visibility = ViewStates.Gone;
                    BluetoothLEManager.Current.StopScanningForDevices();
                    isSearching = false;
                }
            };


            this._listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                Console.Write("ItemClick: " + this._listAdapter.Items[e.Position]);
                mListDeviceView.Visibility = ViewStates.Visible;
                isSearching = false;
                _listView.Visibility = ViewStates.Gone;
                // stop scanning
                this.StopScanning();

                // select the item
                this._listView.ClearFocus();
                this._listView.Post(() =>
                {
                    this._listView.SetSelection(e.Position);
                });
                //this._listView.SetItemChecked (e.Position, true);
                // todo: for some reason, we're losing the selection, so i have to cache it
                // think i know the issue, see the note in the GenericObjectAdapter class
                this._deviceToConnect = this._listAdapter.Items[e.Position];

                // show a connecting overlay
                //this.RunOnUiThread(() => {
                //    //TODO: we need to save a ref to the device when click
                //    this._progress = ProgressDialog.Show(this, "Connecting", "Connecting to " + this._deviceToConnect.Name, true);
                //});

                // try and connect
                //BluetoothLEManager.Current.ConnectToDevice(this._listAdapter[e.Position]);
                var newBluetoothDevice = BluetoothLEManager.Current.MConnectToDevice(this._listAdapter[e.Position]);
                if (newBluetoothDevice != null)
                {
                    newBluetoothDevice.DeviceDisconnected += (o, ee) => { mListDevice.Remove(newBluetoothDevice); };
                    newBluetoothDevice.CharacteristicChanged += (o, ee) => { mCardiographAdapter.NotifyDataSetChanged();  };
                    mListDevice.Add(newBluetoothDevice);
                    Console.WriteLine("add a new device");
                    this.mCardiographAdapter = new CardiographAdapter(this, this.mListDevice);
                }
                else
                {
                    Console.WriteLine("return null");
                }
            };
        }
        

        protected void WireupExternalHandlers()
        {
            this.deviceDiscoveredHandler = (object sender, BluetoothLEManager.DeviceDiscoveredEventArgs e) =>
            {
                Console.WriteLine("Discovered device: " + e.Device.Name);

                // reload the list view
                //TODO: why doens't NotifyDataSetChanged work? is it because i'm replacing the reference?
                this.RunOnUiThread(() =>
                {
                    this._listAdapter = new DevicesAdapter(this, BluetoothLEManager.Current.DiscoveredDevices);
                    this._listView.Adapter = this._listAdapter;
                });
            };
            BluetoothLEManager.Current.DeviceDiscovered += this.deviceDiscoveredHandler;

            this.deviceConnectedHandler = (object sender, BluetoothLEManager.DeviceConnectionEventArgs e) =>
            {
                this.RunOnUiThread(() =>
                {
                    this._progress.Hide();
                });
                // now that we're connected, save it
                App.Current.State.SelectedDevice = e.Device;

                // launch the details screen
                //this.StartActivity(typeof(DeviceDetailsScreen));
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
            new Task(() =>
            {
                if (BluetoothLEManager.Current.IsScanning)
                {
                    Console.WriteLine("Still scanning, stopping the scan and reseting the right button");
                    BluetoothLEManager.Current.StopScanningForDevices();
                    this._scanButton.SetState(ScanButton.ScanButtonState.Normal);
                }
            }).Start();
        }


    }



}

