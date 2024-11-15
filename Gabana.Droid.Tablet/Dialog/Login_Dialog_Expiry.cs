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
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Login_Dialog_Expiry : AndroidX.Fragment.App.DialogFragment
    {
        Button btnCancel;
        Button btnRenew;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Login_Dialog_Expiry NewInstance()
        {
            var frag = new Login_Dialog_Expiry { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_dialog_expiry, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btnRenew = view.FindViewById<Button>(Resource.Id.btnRenew);
                btnRenew.Click += BtnOK_Click;
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnCancel.Click += BtnCancle_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private async void BtnCancle_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagProgress = false;
                MainActivity.tranWithDetails = null;
                Preferences.Set("AppState", "logout");

                await BellNotificationHelper.UnRegisterBellNotification(this.Activity, GabanaAPI.gbnJWT);

                var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
                Preferences.Clear();
                Preferences.Set("NotificationDeviceToken", DeviceToken);

                StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                Dismiss();
            }
            catch (Exception ex)
            {
                Dismiss();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(RenewPackageActivity)));
                if (MainActivity.main_activity != null)
                {
                    MainActivity.main_activity.Finish();
                }

                if (LoginActivity.login_activity != null)
                {
                    LoginActivity.login_activity.Finish();
                }

                if (OtpActivity.OTP_Activity != null)
                {
                    OtpActivity.OTP_Activity.Finish();
                }
            }
            catch (Exception ex)
            {
                Dismiss();
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}