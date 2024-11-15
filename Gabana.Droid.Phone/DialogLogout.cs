using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Phone;
using System;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    public class DialogLogout : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static DialogLogout NewInstance()
        {
            var frag = new DialogLogout { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.logout_fragment_dialog, container, false);
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
            Dismiss();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {

            Preferences.Set("AppState", "logout");

            Intent intent = new Intent(Context, typeof(SplashActivity));
            StartActivity(intent);
        }
    }
}