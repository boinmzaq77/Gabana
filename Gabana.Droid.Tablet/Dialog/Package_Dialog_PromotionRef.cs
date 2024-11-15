using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Package_Dialog_PromotionRef : AndroidX.Fragment.App.DialogFragment
    {
        Button btnDetailPackage;
        TextView txtDetail;
        static int TotalDayRecieved;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Package_Dialog_PromotionRef NewInstance()
        {
            var frag = new Package_Dialog_PromotionRef { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.package_dialog_promotionref, container, false);
            try
            {
                btnDetailPackage = view.FindViewById<Button>(Resource.Id.btnDetailPackage);
                btnDetailPackage.Click += BtnDetailPackage_Click;
                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = GetString(Resource.String.package_activity_promotionref1) + " " + TotalDayRecieved + " " + GetString(Resource.String.package_activity_promotionref2);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        public static void SetTotalDayReceieve(int _TotalDayRecieved)
        { 
            TotalDayRecieved = _TotalDayRecieved;
        }

        private async void BtnDetailPackage_Click(object sender, EventArgs e)
        {
            //เปิดหน้า Package Detail
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "package");
            this.Dismiss();
        }


    }
}