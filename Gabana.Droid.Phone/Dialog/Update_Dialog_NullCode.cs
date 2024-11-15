using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    public class Update_Dialog_NullCode : Android.Support.V4.App.DialogFragment
    {
        Button btnCancel;
        Button btnOK;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Update_Dialog_NullCode NewInstance()
        {
            var frag = new Update_Dialog_NullCode { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.update_dialog_nullcode, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnCancel.Click += BtnCancle_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateProfileActivity.main.UpdateMerchant();
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                MainDialog.CloseDialog();
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}