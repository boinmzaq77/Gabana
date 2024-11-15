using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dailog_AddImageMyQR : AndroidX.Fragment.App.DialogFragment
    {
        LinearLayout lnTakePicture, lnChoosPicture;
        TextView txtcancel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dailog_AddImageMyQR NewInstance()
        {
            var frag = new Setting_Dailog_AddImageMyQR { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addimage_dialog, container, false);
            try
            {
                lnTakePicture = view.FindViewById<LinearLayout>(Resource.Id.lnTakePicture);
                lnChoosPicture = view.FindViewById<LinearLayout>(Resource.Id.lnChoosPicture);
                txtcancel = view.FindViewById<TextView>(Resource.Id.txtcancel);

                lnTakePicture.Click += LnTakePicture_Click;
                lnChoosPicture.Click += LnChoosPicture_Click;
                txtcancel.Click += Txtcancel_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private void Txtcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private void LnChoosPicture_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.GalleryOpen();
            Dismiss();
        }

        private void LnTakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.CameraTakePicture();
                Dismiss();
            }
            catch (Exception)
            {

            }
        }
    }
}