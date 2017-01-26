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

namespace BLEAndroid
{
    [Activity(Label = "BLEAndroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        // about _Y_listview
        private ListView mListView;
        private List<ListItem> mList;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // about _Z_listview 
            mListView = (ListView)FindViewById(Resource.Id.deviceList);
            mList = new List<ListItem>();
            //MainLi
        }

        public void button1_onClick()
        {
            EditText et =(EditText) FindViewById(Resource.Id.text1);
            //////
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

