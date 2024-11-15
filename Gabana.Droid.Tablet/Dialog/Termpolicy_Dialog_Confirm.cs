using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Termpolicy_Dialog_Confirm : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Termpolicy_Dialog_Confirm NewInstance()
        {
            var frag = new Termpolicy_Dialog_Confirm { Arguments = new Bundle() };
            return frag;
        }

        Button btn_cancel;
        Button btn_save;
        private static string typeSet;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.term_dialog_confirm, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click; ;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            if (typeSet == "term")
            {
                TermActivity.term = true;
            }
            if (typeSet == "policy")
            {
                TermActivity.policy = true;
            }
            TermActivity.activity.SetImageTermPolicy();
            MainDialog.CloseDialog();
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            var updatemerchant = await GabanaAPI.PutMerchant(TermActivity.MerchantDetail, null);
            if (updatemerchant.Status)
            {
                Toast.MakeText(this.Context, Application.Context.GetString(Resource.String.savesucess), ToastLength.Short).Show();
                DataCashingAll.Merchant.Merchant = TermActivity.MerchantDetail.Merchant;
                //TermActivity.activity.Finish();

                ////logout
                //Preferences.Set("AppState", "logout");
                //Intent intent = new Intent(Context, typeof(SplashActivity));
                //StartActivity(intent);

                MainDialog.CloseDialog();
            }
            else
            {
                Toast.MakeText(this.Context, Application.Context.GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                MainDialog.CloseDialog();

            }

        }

        internal static void SetAction(string v)
        {
            typeSet = v;
        }
    }
}