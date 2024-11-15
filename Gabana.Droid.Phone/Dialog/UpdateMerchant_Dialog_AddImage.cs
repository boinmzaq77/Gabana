using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class UpdateMerchant_Dialog_AddImage : Android.Support.V4.App.DialogFragment
    {
        LinearLayout lnTakePicture, lnChoosPicture;
        TextView txtcancel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static UpdateMerchant_Dialog_AddImage NewInstance()
        {
            var frag = new UpdateMerchant_Dialog_AddImage { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addcustomer_dialog_addimage, container, false);
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
            if (MerchantActivity.CurrentActivity)
            {
                MerchantActivity.merchant.CheckPermission();
                MerchantActivity.merchant.GalleryOpen();

                Dismiss();
            }
            else
            {
                UpdateProfileActivity.main.CheckPermission();
                UpdateProfileActivity.main.GalleryOpen();
                Dismiss();
            }
        }

        private void LnTakePicture_Click(object sender, EventArgs e)
        {
            if (MerchantActivity.CurrentActivity)
            {
                MerchantActivity.merchant.CheckPermission();
                MerchantActivity.merchant.CameraTakePicture();
                Dismiss();
            }
            else
            {
                UpdateProfileActivity.main.CheckPermission();
                UpdateProfileActivity.main.CameraTakePicture();
                Dismiss();
            }
        }

    }
}