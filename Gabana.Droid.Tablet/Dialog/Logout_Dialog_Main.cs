using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Logout_Dialog_Main : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;
        Logout_Dialog_Main Dialog_Main;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Logout_Dialog_Main NewInstance()
        {
            var frag = new Logout_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog_Main = this;
            var view = inflater.Inflate(Resource.Layout.logout_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private async void BtnOK_Click(object sender, EventArgs e)
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

                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));

                if (UpdateProfileActivity.main != null)
                {
                    UpdateProfileActivity.main.Finish();
                }

                if (MainActivity.main_activity != null)
                {
                    MainActivity.main_activity.Finish();
                }

                if (RenewPackageActivity.activity != null)
                {
                    RenewPackageActivity.activity.Finish();
                }

                Dialog_Main.Dismiss();
            }
            catch (Exception ex)
            {
                Dialog_Main.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at Logout");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            Dialog_Main.Dismiss();
        }
    }


}