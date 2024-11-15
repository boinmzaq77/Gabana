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
    public class Login_Dialog_ExpiryEmp : AndroidX.Fragment.App.DialogFragment
    {
        Button btnOK;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Login_Dialog_ExpiryEmp NewInstance()
        {
            var frag = new Login_Dialog_ExpiryEmp { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_dialog_expiryemp, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
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
                StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                if (MainActivity.main_activity != null)
                {
                    MainActivity.main_activity.Finish();
                }
                Dismiss();
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