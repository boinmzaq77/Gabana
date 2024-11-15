using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Order_Dialog_Openorder : Android.Support.V4.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Order_Dialog_Openorder NewInstance()
        {
            var frag = new Order_Dialog_Openorder { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.order_dialog_openorder, container, false);
            try
            {
                TextView textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm2.Text += " " + OrderActivity.OrderNew.orderName + " ?";
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
                OrderActivity.CancelOrder();
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                MainDialog.CloseDialog();
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }


    }
}