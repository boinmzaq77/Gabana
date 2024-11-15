using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Gabana.Droid
{
    public class Pos_Dialog_Item : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_item, container, false);           
            return view;
        }

        public static Pos_Dialog_Item NewInstance()
        {
            var frag = new Pos_Dialog_Item { Arguments = new Bundle() };
            return frag;
        }


    }
}