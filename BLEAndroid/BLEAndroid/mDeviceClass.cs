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
using Android.Bluetooth;
using Java.Util;
using Android.Graphics;
using Java.Text;
using static BLEAndroid.BluetoothLEManager;
using System.Threading.Tasks;


namespace BLEAndroid
{
    public class mDeviceClass
    {
        public BluetoothAdapter localAdapter = BluetoothAdapter.DefaultAdapter;
        public BluetoothDevice mBluetoothDevice;
        public BluetoothGatt mBluetoothGatt;
        public mGattCallback _GattCallback;
        public IList<BluetoothGattService> mServices;
        public BluetoothGattService mService;
        public IList<BluetoothGattCharacteristic> mCharacteristics;
        public BluetoothGattCharacteristic mCharacteristic;
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public event EventHandler<ServiceDiscoveredEventArgs> ServiceDiscovered = delegate { };
        public event EventHandler<CharacteristicChangedEventArgs> CharacteristicChanged = delegate { };
        public Path mPath;
        public int count;
        public float distance=(float)1;
        public string mString;
        public string tempString;
        public string saveString;
        public bool correct;
        public mDeviceClass(BluetoothDevice BD)
        {
            correct = false;
            mBluetoothDevice = BD;
            _GattCallback = new mGattCallback(this);
            DeviceConnected += MDeviceClass_DeviceConnected;
            mBluetoothGatt = mBluetoothDevice.ConnectGatt(Application.Context, true, _GattCallback);
            
            //initialize
            
            
            
        }

        private void MDeviceClass_DeviceConnected(object sender, DeviceConnectionEventArgs e)
        {
            ServiceDiscovered += MDeviceClass_ServiceDiscovered;
            mBluetoothGatt.DiscoverServices();
        }

        private void MDeviceClass_ServiceDiscovered(object sender, ServiceDiscoveredEventArgs e)
        {
            mServices = mBluetoothGatt.Services;
            foreach (var item in mServices) Console.WriteLine(item.Uuid);
            foreach (var item in mServices)
            {
                if (item.Uuid.ToString() == "0000ffe0-0000-1000-8000-00805f9b34fb")
                { mService = item; }
            }
            //mService = new BluetoothGattService(UUID.FromString("0000FFE0-0000-1000-8000-00805F9B34FB"), GattServiceType.Primary);
            if (mService != null)
            {
                mCharacteristics = mService.Characteristics;
                foreach (var item in mCharacteristics)
                {
                    if (item.Uuid.ToString() == "0000ffe1-0000-1000-8000-00805f9b34fb")
                    { mCharacteristic = item; }
                }
                if (mCharacteristic != null)
                {
                    mBluetoothGatt.SetCharacteristicNotification(mCharacteristic, true);
                    mPath = new Path();
                    mString = "";
                    tempString = "";
                    saveString = "";
                    count = 0;
                    correct = true;
                }
                else
                {
                    Console.WriteLine("characteristic not found");
                }
            }
            else
            {
                Console.WriteLine("service not found");
            }
        }

        private void AddString(string s)
        {
            tempString += s;
            //Console.WriteLine(tempString);
            mString += s;
            //Console.WriteLine(mString);
            if (mPath.IsEmpty)
            {
                mPath.Reset();
                mPath.MoveTo(0, 300);
            }
            string[] temp = tempString.Split('\n');
            for (int i = 0; i < temp.Length-1; i++)
            {
                //Console.WriteLine(temp[i]);
                try
                {
                    mPath.LineTo(count * distance, Convert.ToInt32(temp[i])/6);
                }
                catch { }
                count += 1;
                saveString += AppendStringForSave(temp[i]);
            }
            tempString = temp.Last();
        }

        private string AppendStringForSave(string s)
        {
            var date = new Date();
            return string.Format($"{mBluetoothDevice.Name},{date.Time},{s}");
        }

        public void SendMessage(string s)
        {
            mCharacteristic.SetValue(Encoding.ASCII.GetBytes(s));
            mBluetoothGatt.WriteCharacteristic(mCharacteristic);
        }

        public class mGattCallback:BluetoothGattCallback
        {
            private mDeviceClass _parent;
            private string _string;
            public mGattCallback(mDeviceClass parent)
            {
                _parent = parent;
            }

            public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
            {
                base.OnCharacteristicChanged(gatt, characteristic);
                Byte[] _bytes= characteristic.GetValue();
                _string = new String(Encoding.ASCII.GetChars(_bytes));
                _parent.AddString(_string);
                //Console.WriteLine("receive");
                _parent.CharacteristicChanged(this, new CharacteristicChangedEventArgs() { Gatt = gatt, Characteristic = characteristic });
            }

            public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
            {
                Console.WriteLine("OnConnectionStateChange: ");
                base.OnConnectionStateChange(gatt, status, newState);

                switch (newState)
                {
                    // disconnected
                    case ProfileState.Disconnected:
                        Console.WriteLine("disconnected");
                        //TODO/BUG: Need to remove this, but can't remove the key (uncomment and see bug on disconnect)
                        //					if (this._parent._connectedDevices.ContainsKey (gatt.Device))
                        //						this._parent._connectedDevices.Remove (gatt.Device);
                        this._parent.DeviceDisconnected(this, new DeviceConnectionEventArgs() { Device = gatt.Device });
                        break;
                    // connecting
                    case ProfileState.Connecting:
                        Console.WriteLine("Connecting");
                        break;
                    // connected
                    case ProfileState.Connected:
                        Console.WriteLine("Connected");
                        //TODO/BUGBUG: need to remove this when disconnected
                        //this._parent._connectedDevices.Add(gatt.Device, gatt);
                        this._parent.DeviceConnected(this, new DeviceConnectionEventArgs() { Device = gatt.Device });
                        break;
                    // disconnecting
                    case ProfileState.Disconnecting:
                        Console.WriteLine("Disconnecting");
                        break;
                }
            }

            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
            {
                base.OnServicesDiscovered(gatt, status);

                Console.WriteLine("OnServicesDiscovered: " + status.ToString());

                //TODO: somehow, we need to tie this directly to the device, rather than for all
                // google's API deisgners are children.

                //TODO: not sure if this gets called after all services have been enumerated or not
                //if (!this._parent._services.ContainsKey(gatt.Device))
                //    this._parent.Services.Add(gatt.Device, this._parent._connectedDevices[gatt.Device].Services);
                //else
                //    this._parent._services[gatt.Device] = this._parent._connectedDevices[gatt.Device].Services;

                this._parent.ServiceDiscovered(this, new ServiceDiscoveredEventArgs()
                {
                    Gatt = gatt
                });
            }
        }
    }

    public class CharacteristicChangedEventArgs:EventArgs
    {
        public BluetoothGatt Gatt;
        public BluetoothGattCharacteristic Characteristic;
        public CharacteristicChangedEventArgs() : base() { }
    }
}