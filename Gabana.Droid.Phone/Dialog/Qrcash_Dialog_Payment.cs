using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View.Accessibility;
using Gabana.Model;
using Gabana3.JAM.Trans;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace Gabana.Droid
{
    public class Qrcash_Dialog_Payment : Android.Support.V4.App.DialogFragment
    {
        Button btn_save;
        TextView txtdata;
        public static string txnStatus, txtbtn;
        public static TranWithDetailsLocal tranWithDetails;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Qrcash_Dialog_Payment NewInstance()
        {
            var frag = new Qrcash_Dialog_Payment { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.qrcash_dialog_payment, container, false);
            try
            {
                txtdata =  view.FindViewById<TextView>(Resource.Id.textData);                
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
            btn_save.Enabled = false;
            switch (txnStatus)
            {
                case "PAID":
                    txtdata.Text = "ทำรายการสำเร็จ";
                    StartActivity(new Intent(Application.Context, typeof(myQRReceiptActivity)));
                    myQRReceiptActivity.SetTranDetail(tranWithDetails);
                    QRCashActivity.mainacitivity.Finish();
                    break;
                case "EXPIRED":
                    txtdata.Text = "Qrcode ผิดพลาด กรุณาเลือกวิธีการชำระใหม่อีกครั้ง";
                    StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                    PaymentActivity.SetTranDetail(tranWithDetails);
                    QRCashActivity.mainacitivity.Finish();
                    break;
                case "CANCELLED":
                    txtdata.Text = "Qrcode ถูกยกเลิก กรุณาเลือกวิธีการชำระใหม่อีกครั้ง";
                    StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                    PaymentActivity.SetTranDetail(tranWithDetails);
                    QRCashActivity.mainacitivity.Finish();
                    break;               
                default:
                    Toast.MakeText(Application.Context, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                    break;
            }
            btn_save.Enabled = true;
            MainDialog.CloseDialog();
        }

        public static void SetDetail(TranWithDetailsLocal t, string _txnstatus)
        {
            tranWithDetails = t;
            txnStatus = _txnstatus;
        }
    }
}