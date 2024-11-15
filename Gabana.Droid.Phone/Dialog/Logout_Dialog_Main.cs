using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    public class Logout_Dialog_Main : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;

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
            var view = inflater.Inflate(Resource.Layout.logout_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_save.Enabled = true;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click;
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

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btn_save.Enabled = false;
                DataCashing.flagProgress = false;
                PosActivity.tranWithDetails = null;


                if (this.Activity != null)
                {
                    await BellNotificationHelper.UnRegisterBellNotification(this.Activity, GabanaAPI.gbnJWT);
                }

                var DeviceToken = Preferences.Get("NotificationDeviceToken", "");
                Preferences.Clear();
                Preferences.Set("NotificationDeviceToken", DeviceToken);

                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));

                if (UpdateProfileActivity.main != null)
                {
                    UpdateProfileActivity.main.Finish();
                }

                if (MainActivity.activity != null)
                {
                    MainActivity.activity.Finish();
                }

                if (RenewPackageActivity.activity != null)
                {
                    RenewPackageActivity.activity.Finish();
                }


                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                MainDialog.CloseDialog();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at Logout");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}