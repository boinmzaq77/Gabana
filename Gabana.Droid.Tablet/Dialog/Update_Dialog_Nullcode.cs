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
    public class Update_Dialog_Nullcode : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Update_Dialog_Nullcode NewInstance()
        {
            var frag = new Update_Dialog_Nullcode { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addcustomer_dialog_dubicate, container, false);
            try
            {
                TextView txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = GetString(Resource.String.updateprofile_nonref);
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

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateProfileActivity.main.UpdateMerchant();
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                this.Dialog.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at Logout");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();  
        }
    }


}