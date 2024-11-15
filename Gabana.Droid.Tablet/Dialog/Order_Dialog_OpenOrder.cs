using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Order_Dialog_OpenOrder : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Order_Dialog_OpenOrder NewInstance()
        {
            var frag = new Order_Dialog_OpenOrder { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.order_dialog_openorder, container, false);
            try
            {
                TextView textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm2.Text += " " + Pos_Dialog_Order.OrderNew.orderName + " ?";
                Button btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click; 
                Button btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnCancel.Click += BtnCancle_Click; 
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Pos_Dialog_Order.CancelOrder();
                Dismiss();
            }
            catch (Exception ex)
            {
                Dismiss();
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}