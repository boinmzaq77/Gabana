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
    public class Package_Dialog_Error : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Package_Dialog_Error NewInstance()
        {
            var frag = new Package_Dialog_Error { Arguments = new Bundle() };
            return frag;
        }
        TextView txtDetail;
        Button btnBack;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.package_dialog_error, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
                btnBack.Click += BtnBack_Click;
                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = PackageActivity.TextError;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

    }
}