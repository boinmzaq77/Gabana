using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Pos_Dialog_SaveOrder : Android.Support.V4.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Pos_Dialog_SaveOrder NewInstance()
        {
            var frag = new Pos_Dialog_SaveOrder { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_saveorder, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                Button btn_ok = view.FindViewById<Button>(Resource.Id.btn_ok);
                btn_ok.Click += BtnOK_Click;
                Button btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click; ;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                PosActivity.DialogCancelSave();
                MainDialog.CloseDialog();
            }
            catch (Exception)
            {
                MainDialog.CloseDialog();
            }
        }


    }
}