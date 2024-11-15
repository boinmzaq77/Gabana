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
    public class Update_Dialog_Error : AndroidX.Fragment.App.DialogFragment
    {
        Button btnOK;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Update_Dialog_Error NewInstance()
        {
            var frag = new Update_Dialog_Error { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_dialog_expiryemp, container, false);
            try
            {
                TextView txtError = view.FindViewById<TextView>(Resource.Id.txtDetail);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
                txtError.Text = UpdateProfileActivity.TextError;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {              
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                this.Dialog.Dismiss();
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}