using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid.Phone
{
    public class Package_Dialog_Refresh : Android.Support.V4.App.DialogFragment
    {
        Button btnRefresh;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Package_Dialog_Refresh NewInstance()
        {
            var frag = new Package_Dialog_Refresh { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.package_dialog_refresh, container, false);
            try
            {
                btnRefresh = view.FindViewById<Button>(Resource.Id.btnRefresh);
                btnRefresh.Click += BtnRefresh_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //if (RenewPackageActivity.activity != null)
                //{
                //    RenewPackageActivity.activity.CallPutRenew();
                //}

                //if (PackageActivity.activity != null)
                //{
                //    PackageActivity.activity.CallPutData();
                //}

                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

    }
}
