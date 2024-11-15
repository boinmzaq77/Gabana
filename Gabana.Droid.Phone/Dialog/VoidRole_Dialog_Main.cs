using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class VoidRole_Dialog_Main : Android.Support.V4.App.DialogFragment
    {
        Button btn_save;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static VoidRole_Dialog_Main NewInstance()
        {
            var frag = new VoidRole_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.offline_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                textUsepoint.Text = string.Empty;
                textUsepoint.Text = GetText(Resource.String.notperm);

                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
    }
}