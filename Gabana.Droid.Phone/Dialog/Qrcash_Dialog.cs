using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Qrcash_Dialog : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView txtShow;
        public static TranWithDetailsLocal tranWithDetails;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Qrcash_Dialog NewInstance()
        {
            var frag = new Qrcash_Dialog { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.qrcash_dialog, container, false);
            try
            {
                txtShow = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_ok);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;
                txtShow.Text = "คุณต้องการที่จะยกเลิกการจ่ายเงินหรือไม่?";
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
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
                StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                PaymentActivity.SetTranDetail(tranWithDetails);
                QRCashActivity.mainacitivity.Finish();
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at addcus");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {           
            MainDialog.CloseDialog();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
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
