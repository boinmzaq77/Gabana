using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid.Phone
{
    public class Package_Dialog_PromotionRef : Android.Support.V4.App.DialogFragment
    {
        Button btnDetailPackage;
        TextView txtDetail;
        static int TotalDayRecieved;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Package_Dialog_PromotionRef NewInstance(int _TotalDayRecieved)
        {
            TotalDayRecieved = _TotalDayRecieved;
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

        private void BtnDetailPackage_Click(object sender, EventArgs e)
        {
            //เปิดหน้า Package Detail
            StartActivity(new Android.Content.Intent(Application.Context, typeof(SettingPackageActivity)));

            //if (PackageActivity.activity != null)
            //{
            //    PackageActivity.activity.CallPutData();
            //}

            MainDialog.CloseDialog();
        }

    }
}
