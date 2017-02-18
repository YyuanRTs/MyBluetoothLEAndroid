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

namespace BLEAndroid
{
    class CardiographAdapter : GenericAdapterBase<mDeviceClass>
    {

        public CardiographAdapter(Activity context, IList<mDeviceClass> items) 
			: base(context, Resource.Layout.CardiographView, items)
		{

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view==null)
            {
                view = context.LayoutInflater.Inflate(resource, null);
            }

            //view.FindViewById<PathView>(Resource.Id.path).mPath = items[position].mPath.mPath;
            //view.FindViewById<PathView>(Resource.Id.path).XPosition = items[position].mPath.XPosition;
            view.FindViewById<PathView>(Resource.Id.path).path = items[position].mPath;
            return view;
        }
    }

    
}