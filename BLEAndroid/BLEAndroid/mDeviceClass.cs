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

namespace BLEAndroid
{
    public class mDeviceClass
    {
        BluetoothAdapter localAdapter = BluetoothAdapter.DefaultAdapter;
        BluetoothDevice mBluetoothDevice;
        BluetoothGatt mBluetoothGatt;
        mGattCallback _GattCallback;
        IList<BluetoothGattService> mServices;
        BluetoothGattService mService;
        IList<BluetoothGattCharacteristic> mCharacteristics;
        BluetoothGattCharacteristic mCharacteristic;
        public event EventHandler<DeviceConnectionEventArgs> DeviceConnected = delegate { };
        public event EventHandler<DeviceConnectionEventArgs> DeviceDisconnected = delegate { };
        public Path mPath;
        public int count;
        public float distance=(float)20;
        public string mString;
        public string tempString;
        public string saveString;
        public mDeviceClass(BluetoothDevice BD)
        {
            mBluetoothDevice = BD;
            _GattCallback = new mGattCallback(this);
            mBluetoothGatt = mBluetoothDevice.ConnectGatt(Application.Context, true, _GattCallback);
            mServices = mBluetoothGatt.Services;
            foreach (var item in mServices)
            {
                if(item.Uuid==UUID.FromString("0000FFE0-0000-1000-8000-00805F9B34FB"))
                { mService = item; }
            }
            //mService = new BluetoothGattService(UUID.FromString("0000FFE0-0000-1000-8000-00805F9B34FB"), GattServiceType.Primary);
            mCharacteristics = mService.Characteristics;
            foreach (var item in mCharacteristics)
            {
                if(item.Uuid==UUID.FromString("0000FFE1-0000-1000-8000-00805F9B34FB"))
                { mCharacteristic = item;}
            }
            mBluetoothGatt.SetCharacteristicNotification(mCharacteristic, true);

            //initialize
            mString = "";
            tempString = "";
            saveString = "";
            count = 0;
            

        }

        private void AddString(string s)
        {
            tempString += s;
            mString += s;
            string[] temp = tempString.Split('\n');
            for (int i = 0; i < temp.Length-1; i++)
            {
                mPath.LineTo(count * distance, Convert.ToInt32(temp[i]));
                count += 1;
                saveString += AppendStringForSave(temp[i]);
            }
            tempString = temp.Last();
        }

        private string AppendStringForSave(string s)
        {
            var date = new Date();
            return string.Format($"{mBluetoothDevice.Name},{date.Time},{s}\r\n");
        }

        public void SendMessage(string s)
        {
            mCharacteristic.SetValue(Encoding.ASCII.GetBytes(s));
        }

        protected class mGattCallback:BluetoothGattCallback
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
                _string=characteristic.GetStringValue(0);
                _parent.AddString(_string);
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
                        //this._parent.DeviceConnected(this, new DeviceConnectionEventArgs() { Device = gatt.Device });
                        break;
                    // disconnecting
                    case ProfileState.Disconnecting:
                        Console.WriteLine("Disconnecting");
                        break;
                }
            }
        }
    }
}