using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using System;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    public class Login_Dialog_Expiry : Android.Support.V4.App.DialogFragment
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
                PosActivity.tranWithDetails = null;
                Preferences.Set("AppState", "logout");
                await BellNotificationHelper.UnRegisterBellNotification(this.Activity, GabanaAPI.gbnJWT);
                StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                MainDialog.CloseDialog();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(RenewPackageActivity)));
                if (MainActivity.activity != null)
                {
                    MainActivity.activity.Finish();
                }

                if (LoginActivity.login_Activity != null)
                {
                    LoginActivity.login_Activity.Finish();
                }

                if (OtpActivity.OTP_Activity != null)
                {
                    OtpActivity.OTP_Activity.Finish();
                }
            }
            catch (Exception ex)
            {
                MainDialog.CloseDialog();
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}