using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Payment_Dialog_QrCash : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Payment_Dialog_QrCash NewInstance()
        {
            var frag = new Payment_Dialog_QrCash { Arguments = new Bundle() };
            return frag;
        }

        Button btn_save;
        TextView txtdata;
        public static string txnStatus, txtbtn;
        public static TranWithDetailsLocal tranWithDetails;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.payment_dialog_qrcash, container, false);
            try
            {
                txtdata = view.FindViewById<TextView>(Resource.Id.txtDetail);
                btn_save = view.FindViewById<Button>(Resource.Id.btnOK);
                btn_save.Click += BtnOK_Click;
                tranWithDetails = MainActivity.tranWithDetails;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            btn_save.Enabled = false;
            switch (txnStatus)
            {
                case "PAID":
                    txtdata.Text = "ทำรายการสำเร็จ";
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "myqrreceipt");
                    break;
                case "EXPIRED":
                    txtdata.Text = "Qrcode ผิดพลาด กรุณาเลือกวิธีการชำระใหม่อีกครั้ง";
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "default");
                    break;
                case "CANCELLED":
                    txtdata.Text = "Qrcode ถูกยกเลิก กรุณาเลือกวิธีการชำระใหม่อีกครั้ง";
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "default");
                    break;
                default:
                    Toast.MakeText(Application.Context, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                    break;
            }
            btn_save.Enabled = true;
            this.Dialog.Dismiss(); 
        }

        public static void SetDetail(string _txnstatus)
        {
            txnStatus = _txnstatus;
        }
    }
}