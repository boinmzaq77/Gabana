using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Update_Dialog_Error : Android.Support.V4.App.DialogFragment
    {
        TextView txtDetail;
        Button btnBack;

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
            var view = inflater.Inflate(Resource.Layout.update_dialog_error, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                btnBack = view.FindViewById<Button>(Resource.Id.btnBack);
                btnBack.Click += BtnBack_Click;
                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = UpdateProfileActivity.TextError;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }


    }
}