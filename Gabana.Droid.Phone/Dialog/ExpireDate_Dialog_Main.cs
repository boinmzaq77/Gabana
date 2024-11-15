using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Phone;
using System;
using Android.Content;

namespace Gabana.Droid
{
    public class ExpireDate_Dialog_Main : Android.Support.V4.App.Fragment
    {
        Context context;
        Button btn_save;

        public int mycouponno;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public static ExpireDate_Dialog_Main NewInstance()
        {
            var frag = new ExpireDate_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.updateapp_dialog_main, container, false);
            context = container.Context;


            btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
            btn_save.Click += BtnOK_Click;

            return view;
        }


        private void BtnOK_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.seniorsoft.Gabana3");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

    }
}