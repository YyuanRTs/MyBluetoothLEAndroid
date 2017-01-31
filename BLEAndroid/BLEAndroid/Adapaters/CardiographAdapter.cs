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
    class CardiographAdapter : GenericAdapterBase<>
    {
        
            private List<ListItem> mList;
            private Context context;
            private ListItemView listItemView;
            public MainListViewAdapter(Context mcontext, List<ListItem> list)
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

    
}