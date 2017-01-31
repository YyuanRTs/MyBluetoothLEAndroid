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
namespace BLEAndroid
{
    class mDeviceClass
    {
        BluetoothAdapter localAdapter = BluetoothAdapter.DefaultAdapter;
        BluetoothDevice mBluetoothDevice;
        BluetoothGatt mBluetoothGatt;
        mGattCallback _GattCallback;
        IList<BluetoothGattService> mServices;
        BluetoothGattService mService;
        IList<BluetoothGattCharacteristic> mCharacteristics;
        BluetoothGattCharacteristic mCharacteristic;
        public Path mPath;
        public string mString;
        public mDeviceClass(BluetoothDevice BD)
        {
            mBluetoothDevice = BD;
            _GattCallback = new mGattCallback(this);
            mBluetoothGatt = mBluetoothDevice.ConnectGatt(Application.Context, false, _GattCallback);
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
        }

        private void AddString(string s)
        {

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
        }
    }
}