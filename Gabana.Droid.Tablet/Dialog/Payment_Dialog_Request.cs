using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Payment_Dialog_Request : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Payment_Dialog_Request NewInstance()
        {
            var frag = new Payment_Dialog_Request { Arguments = new Bundle() };
            return frag;
        }

        Button btn_save, btn_cancle;
        TextView txtdata;
        public static string txnStatus, txtbtn;
        public static TranWithDetailsLocal tranWithDetails;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.payment_dialog_requested, container, false);
            try
            {
                txtdata = view.FindViewById<TextView>(Resource.Id.txtDetail);
                btn_save = view.FindViewById<Button>(Resource.Id.btnOK);
                btn_cancle = view.FindViewById<Button>(Resource.Id.btnCancel);

                btn_save.Click += BtnOK_Click;
                btn_cancle.Click += Btn_cancle_Click;
                txtdata.Text = "คุณต้องการที่จะยกเลิกการจ่ายเงินหรือไม่?";
                tranWithDetails = MainActivity.tranWithDetails;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void Btn_cancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr = await CancelQrPayment();
                if (status_Qr.statusCode == "-1")
                {
                    Toast.MakeText(Application.Context, status_Qr.errorDesc, ToastLength.Short).Show();
                    return;
                }

                if (status_Qr.statusCode == "10")
                {
                    Toast.MakeText(Application.Context, status_Qr.txnStatus, ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(Application.Context, "ทำรายการยกเลิกสำเร็จ", ToastLength.Short).Show();

                PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "default");
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at addcus");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task<Status_QrKBank> CancelQrPayment()
        {
            try
            {
                respone_QrKBank respone_QrK = new respone_QrKBank();
                respone_QrK = await GabanaAPI.PostDataCancelQRPayment(DataCashing.countGen.Tranno);
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr.statusCode = respone_QrK.statusCode;
                status_Qr.txnStatus = respone_QrK.txnStatus;
                status_Qr.errorDesc = respone_QrK.errorDesc;
                return status_Qr;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CancelQrPayment at QRCashActivity");
                Status_QrKBank status_Qr = new Status_QrKBank();
                return status_Qr;
            }
        }
    }
}