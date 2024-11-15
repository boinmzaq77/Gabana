using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class InsertOffline_Dialog_Main : Android.Support.V4.App.DialogFragment
    {
        Button btn_save;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static InsertOffline_Dialog_Main NewInstance()
        {
            var frag = new InsertOffline_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.offline_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                var txt1 = GetString(Resource.String.connectinternet1);
                var txt2 = GetString(Resource.String.connectinternet2);
                textUsepoint.Text = txt1 + "\n" + txt2;
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
    }
}