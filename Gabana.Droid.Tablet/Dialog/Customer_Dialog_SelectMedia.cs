using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    internal class Customer_Dialog_SelectMedia : AndroidX.Fragment.App.DialogFragment
    {

        LinearLayout lnTakePicture, lnChoosPicture;
        TextView txtcancel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Customer_Dialog_SelectMedia NewInstance()
        {
            var frag = new Customer_Dialog_SelectMedia { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_choosemedia, container, false);
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
            DataCashing.flagChooseMedia = false;
            Dismiss();
        }

        private void LnChoosPicture_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = MainActivity.main_activity.CheckPermission();
                if (check)
                {
                    MainActivity.main_activity.GalleryOpen();
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }

        private void LnTakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = MainActivity.main_activity.CheckPermission();
                if (check)
                {
                    MainActivity.main_activity.CameraTakePicture();
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

    }

}