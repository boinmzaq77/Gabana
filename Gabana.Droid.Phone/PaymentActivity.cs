using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Text;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class PaymentActivity : AppCompatActivity
    {
        public ImageButton imageBack;
        public Button btnSave;
        decimal amount = 0;
        public static TranWithDetailsLocal tranWithDetails;
        ORM.MerchantDB.TranPayment tranPayment = new ORM.MerchantDB.TranPayment();
        TransManage transManage = new TransManage();
        TranPaymentManage paymentManage = new TranPaymentManage();
        TranDetailItemManage tranDetailManage = new TranDetailItemManage();
        static LinearLayout lnNoCustomer, lnHaveCustomer;
        static TextView txtNameCustomer;
        ImageButton btnCustomer, imagCash, imgOrder, imagGiftVoucher, imageCredit, imageMyqr , imageQrCash , imageQrCredit , imageWechat;
        static Customer selectCus;
        TextView textSumAmount, textCurrency, textCash, textOrder, textGiftVoucher , textCredit, textMyqr , textQrCash , textQrCredit , textWechat;
        LinearLayout lnQrCash, lnBack, lnCash, lnSaveOrder, lnGiftVoucher, lnCreditCard, lnmyqrcash,  lnQrCredit, lnWechat;
        bool IsActive = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.payment_activity_main);
                SetUI();
                CheckJwt();
                Log.Debug("swipapp", "Payment OnCreate");

                SetupCurrencySymbol();
                
                _ = TinyInsights.TrackPageViewAsync("OnCreate : PaymentActivity");
                Log.Debug("swipapp", "Payment OnCreate");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Payment");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetupCurrencySymbol()
        {
            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            string currencyTH = "";
            string currencyEn = "";
            switch (CURRENCYSYMBOLS)
            {
                case "$":
                    currencyEn = "US dollar";
                    currencyTH = "ดอลลาร์สหรัฐ";
                    break;
                case "฿":
                    currencyEn = "Thai Baht";
                    currencyTH = "บาท";
                    break;
                case "€":
                    currencyEn = "Euro";
                    currencyTH = "ยูโร";
                    break;
                case "¥":
                    currencyEn = "Yen";
                    currencyTH = "เยน";
                    break;
                default:
                    currencyEn = "";
                    currencyTH = "";
                    break;
            }
            textCurrency.Text = DataCashing.Language == "th" ? currencyTH : currencyEn;
        }

        private void SetUI()
        {
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            imageBack = FindViewById<ImageButton>(Resource.Id.btnBack);

            lnCash = FindViewById<LinearLayout>(Resource.Id.lnCash);
            imagCash = FindViewById<ImageButton>(Resource.Id.imagCash);
            textCash = FindViewById<TextView>(Resource.Id.textCash);

            lnSaveOrder = FindViewById<LinearLayout>(Resource.Id.lnSaveOrder);
            lnGiftVoucher = FindViewById<LinearLayout>(Resource.Id.lnGiftVoucher);
            lnCreditCard = FindViewById<LinearLayout>(Resource.Id.lnCreditCard);
            lnmyqrcash = FindViewById<LinearLayout>(Resource.Id.lnMyqr);
            lnQrCash = FindViewById<LinearLayout>(Resource.Id.lnQrCash);
            lnQrCredit = FindViewById<LinearLayout>(Resource.Id.lnQrCredit);
            lnWechat = FindViewById<LinearLayout>(Resource.Id.lnWechat);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            imgOrder = FindViewById<ImageButton>(Resource.Id.imgOrder);
            imagGiftVoucher = FindViewById<ImageButton>(Resource.Id.imagGiftVoucher);
            imageCredit = FindViewById<ImageButton>(Resource.Id.imageCredit);
            imageMyqr = FindViewById<ImageButton>(Resource.Id.imageMyqr);
            imageQrCash = FindViewById<ImageButton>(Resource.Id.imageQrCash);
            imageQrCredit = FindViewById<ImageButton>(Resource.Id.imageQrCredit);
            imageWechat = FindViewById<ImageButton>(Resource.Id.imageWechat);
            textSumAmount = FindViewById<TextView>(Resource.Id.textAmount);
            textCurrency = FindViewById<TextView>(Resource.Id.textCurrency);
            textOrder = FindViewById<TextView>(Resource.Id.textOrder);
            textGiftVoucher = FindViewById<TextView>(Resource.Id.textGiftVoucher);
            textCredit = FindViewById<TextView>(Resource.Id.textCredit);
            textMyqr = FindViewById<TextView>(Resource.Id.textMyqr);
            textQrCash = FindViewById<TextView>(Resource.Id.textQrCash);
            textQrCredit = FindViewById<TextView>(Resource.Id.textQrCredit);
            textWechat = FindViewById<TextView>(Resource.Id.textWechat);
            btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
            lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);

            btnCustomer.Click += BtnCustomer_Click;
            lnNoCustomer.Click += BtnCustomer_Click;
            lnHaveCustomer.Click += BtnCustomer_Click;
            imageBack.Click += ImageBack_Click;
            lnBack.Click += ImageBack_Click;

            lnCash.Click += LnCash_Click;
            imagCash.Click += LnCash_Click;
            textCash.Click += LnCash_Click;

            lnSaveOrder.Click += LnSaveOrder_Click;
            imgOrder.Click += LnSaveOrder_Click;
            textOrder.Click += LnSaveOrder_Click;

            lnGiftVoucher.Click += LnGiftVoucher_Click;
            imagGiftVoucher.Click += LnGiftVoucher_Click;
            textGiftVoucher.Click += LnGiftVoucher_Click;

            lnCreditCard.Click += LnCreditCard_Click;
            imageCredit.Click += LnCreditCard_Click;
            textCredit.Click += LnCreditCard_Click;

            lnmyqrcash.Click += myQRCash_Click;
            imageMyqr.Click += myQRCash_Click;
            textMyqr.Click += myQRCash_Click;

            lnQrCash.Click += LnQrCash_Click;
            imageQrCash.Click += LnQrCash_Click;
            textQrCash.Click += LnQrCash_Click;

            lnQrCredit.Click += LnQrCredit_Click;
            imageQrCredit.Click += LnQrCredit_Click;
            textQrCredit.Click += LnQrCredit_Click;

            lnWechat.Click += LnWechat_Click;
            imageWechat.Click += LnWechat_Click;
            textWechat.Click += LnWechat_Click;
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }

        public static async void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    if (tranWithDetails.tran.SysCustomerID != 999)
                    {
                        tranWithDetails = await BLTrans.RemovePerson(tranWithDetails);
                    }

                    lnHaveCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Visible;

                }
                else
                {
                    lnHaveCustomer.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;

                    CustomerManage customerManage = new CustomerManage();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    selectCus = listCustomer.Where(x => x.SysCustomerID == DataCashing.SysCustomerID).FirstOrDefault();
                    if (selectCus == null) return;
                    if (tranWithDetails.tran.SysCustomerID != DataCashing.SysCustomerID)
                    {
                        tranWithDetails.tran.SysCustomerID = selectCus.SysCustomerID;
                        tranWithDetails.tran.CustomerName = selectCus.CustomerName;

                        tranWithDetails = await BLTrans.ChoosePerson(tranWithDetails, selectCus);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    }
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void myQRCash_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "MYQR";
            DataCashing.PaymentType = tranPayment.PaymentType;
            StartActivity(new Intent(Application.Context, typeof(myQRCashActivity)));
            myQRCashActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }
        private void LnCreditCard_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "Cr";
            DataCashing.PaymentType = tranPayment.PaymentType;
            StartActivity(new Intent(Application.Context, typeof(CreditReceiptActivity)));
            CreditReceiptActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }
        private void LnGiftVoucher_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "GV";
            DataCashing.PaymentType = tranPayment.PaymentType;
            StartActivity(new Intent(Application.Context, typeof(GiftVoucherPaymentActivity)));
            GiftVoucherPaymentActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }
        private void LnCash_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "CH";
            DataCashing.PaymentType = tranPayment.PaymentType;
            StartActivity(new Intent(Application.Context, typeof(CashActivity)));
            CashActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }
        private async void LnWechat_Click(object sender, EventArgs e)
        {
            //เช็คตเตัส payment 
            //respone_QrKBank qrStatus = new respone_QrKBank();
            //qrStatus = await GabanaAPI.GetDataStatusQRPayment(tranWithDetails.tran.TranNo);
            //Status_QrKBank status_Qr = new Status_QrKBank();
            //status_Qr.statusCode = qrStatus.statusCode;
            //status_Qr.txnStatus = qrStatus.txnStatus;
            //status_Qr.errorDesc = qrStatus.errorDesc;
        }

        private void LnQrCredit_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "QRCR";
            DataCashing.PaymentType = tranPayment.PaymentType;
        }

        private async void LnQrCash_Click(object sender, EventArgs e)
        {
            try
            {
                lnQrCash.Enabled = false;

                DialogLoading dialogLoading = new DialogLoading();
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                if (!await GabanaAPI.CheckNetWork())
                {
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    lnQrCash.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    lnQrCash.Enabled = true;
                    return;
                }

                tranPayment.PaymentType = "QRCH";
                DataCashing.PaymentType = tranPayment.PaymentType;
                double Amount = 0;
                Amount = decimal.ToDouble(amount);
                Amount = Math.Round(Amount, 2, MidpointRounding.AwayFromZero);

                //เพิ่มเช็คว่าเคยกด gen qr ไปบางยัง ถ้ามีให้เพิ่ม -x
                string TrannNo = string.Empty;

                if (DataCashing.countGen == null)
                {
                    DataCashing.countGen.countgenQr = 0;
                    DataCashing.countGen.Tranno = tranWithDetails.tran.TranNo;
                }
                if (DataCashing.countGen.countgenQr == 0)
                {
                    TrannNo = tranWithDetails.tran.TranNo;
                }
                else
                {
                    TrannNo = tranWithDetails.tran.TranNo + "_" + DataCashing.countGen.countgenQr;                    
                }

                DataCashing.countGen.Tranno = TrannNo;

                //ทดสอบยิง KQr
                respone_QrKBank respone_Qr = new respone_QrKBank();
                respone_Qr = await GabanaAPI.GetDataQRPayment(DataCashing.countGen.Tranno, Amount);

                //case too many request
                if (respone_Qr.statusCode == "-1" || respone_Qr.statusCode == "10")
                {
                    //เช็คว่ามีสิทธิ์การใช้งานไหม
                    if (respone_Qr.errorCode.ToLower() == "invalid_merchant") //ไม่มีสิทธิ์การใช้งาน ปุ่มเทา dialog แจ้งเตือน
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.alert_dialog_paymentqrcode.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        dialog.Cancelable = false;
                    }
                    else
                    {
                        Toast.MakeText(this, respone_Qr.errorDesc, ToastLength.Short).Show();
                    }

                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    lnQrCash.Enabled = true;
                    return;
                }

                if (string.IsNullOrEmpty(respone_Qr.qrCode))
                {
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
                    lnQrCash.Enabled = true;
                    return;
                }

                //มีสิทธิ์การใช้งาน ปุ่มม่วง
                var getOpenQrCash = Preferences.Get("OpenQrCash", 0);
                if (getOpenQrCash == 0)
                {
                    Preferences.Set("OpenQrCash", 1); // 1 has , 0 no
                }

                StartActivity(new Intent(Application.Context, typeof(QRCashActivity)));
                QRCashActivity.SetTranDetail(tranWithDetails);
                QRCashActivity.SetResponeQRKBank(respone_Qr);
                int count = DataCashing.countGen.countgenQr;
                count++ ;
                DataCashing.countGen.countgenQr = count;
                IsActive = false;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                this.Finish();
            }
            catch (Exception ex)
            {
                lnQrCash.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnQrCash_Click at Payment");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void LnSaveOrder_Click(object sender, EventArgs e)
        {
            int limitOrder = 0;
            limitOrder = await UtilsAll.GetCountOrder();
            if (limitOrder > 100)
            {
                Toast.MakeText(this, "จำนวน Order สูงสุดต่อสาขา 100 Order", ToastLength.Short).Show();
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(SaveOrderActivity)));
                SaveOrderActivity.SetTranDetail(tranWithDetails);
                IsActive = false;
                this.Finish();
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            try
            {
                tranWithDetails = t;
            }
            catch (Exception ex)
            {
                Log.Debug("swipapp", "Payment SetTranDetail " + ex.Message);
            }
        }

        private void ImageBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(CartActivity)));
            CartActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }

        //private void BtnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(tranPayment.PaymentType))
        //        {
        //            return;
        //        }

        //        switch (tranPayment.PaymentType.ToLower())
        //        {
        //            case "ch":
        //                StartActivity(new Intent(Application.Context, typeof(CashActivity)));
        //                CashActivity.SetDetail(tranWithDetails);
        //                break;
        //            case "gv":

        //                break;
        //            case "cr":

        //                break;
        //            case "ep":

        //                break;
        //            case "qr":
        //                break;
        //            default:
        //                break;
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
        //    }
        //}

        protected  override void OnResume()
        {
            try
            {
                base.OnResume();

                CheckJwt();
                
                int getOpenQrCash = Preferences.Get("OpenQrCash",0);
                if (getOpenQrCash == 1)
                {
                    imageQrCash.SetBackgroundResource(Resource.Mipmap.PaymentQRCash);
                    textQrCash.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null));
                }

                //if (tranWithDetails == null)
                //{
                //    string jsonTran = Preferences.Get("tranWithDetails", "");
                //    tranWithDetails = Utils.ConvertJsontoTranwithDetail(jsonTran);
                //    Log.Debug("swipapp", "Payment OnResume tranWithDetails == null");
                //    Log.Debug("swipapp", jsonTran);
                //}

                IsActive = true;
                SetCustomer();
                decimal paymentAmount = 0;
                paymentAmount = tranWithDetails.tranPayments.Sum(x => x.PaymentAmount);
                //amount คือ ยอดที่ต้องจ่าย  
                var GrandTotal = Regex.Match(tranWithDetails.tran.GrandTotal.ToString(), @"^*[0-9.,]+$");
                amount = Convert.ToDecimal(tranWithDetails.tran.GrandTotal - paymentAmount);
                if (amount <= 0)
                {
                    StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                    BalanceActivity.SetTranDetail(tranWithDetails);
                    //Utils.ConvertTranwithDetailtoJson(tranWithDetails);
                    Toast.MakeText(this, "Utils.ConvertTranwithDetailtoJson", ToastLength.Short).Show();
                    Log.Debug("swipapp", "Payment OnResume Utils.ConvertTranwithDetailtoJson");
                    this.Finish();
                }
                textSumAmount.Text = Utils.DisplayDecimal(amount);
                //Utils.ConvertTranwithDetailtoJson(tranWithDetails);
                Log.Debug("swipapp", "Payment OnResume Utils.ConvertTranwithDetailtoJson");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at payment");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        public override void OnBackPressed()
        {
            imageBack.PerformClick();
        }

        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }

        public override void OnUserInteraction()
        {
            try
            {
                base.OnUserInteraction();
                if (deviceAsleep)
                {
                    deviceAsleep = false;
                    TimeSpan span = DateTime.Now.Subtract(pauseDate);

                    long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                    if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                    {
                        StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                        this.Finish();
                        return;
                    }
                    else
                    {
                        pauseDate = DateTime.Now;
                    }
                }
                else
                {
                    pauseDate = DateTime.Now;

                }
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                    PinCodeActitvity.SetPincode("Pincode");
                    openPage = true;
                }
                Log.Debug("swipapp", "Payment OnUserInteraction");
            }
            catch (Exception ex)
            {
                Log.Debug("swipapp", "Payment OnUserInteraction ex" +  ex.Message);
            }
        }


        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                lnBack?.Dispose();
                imageBack?.Dispose();
                lnCash?.Dispose();
                lnSaveOrder?.Dispose();
                lnGiftVoucher?.Dispose();
                lnCreditCard?.Dispose();
                lnmyqrcash?.Dispose();
                lnQrCash?.Dispose();
                lnQrCredit?.Dispose();
                lnWechat?.Dispose();
                btnSave?.Dispose();
                imagCash?.Dispose();
                imgOrder?.Dispose();
                imagGiftVoucher?.Dispose();
                imageCredit?.Dispose();
                imageMyqr?.Dispose();
                imageQrCash?.Dispose();
                imageQrCredit?.Dispose();
                imageWechat?.Dispose();
                textSumAmount?.Dispose();
                textCurrency?.Dispose();
                textCash?.Dispose();
                textOrder?.Dispose();
                textGiftVoucher?.Dispose();
                textCredit?.Dispose();
                textMyqr?.Dispose();
                textQrCash?.Dispose();
                textQrCredit?.Dispose();
                textWechat?.Dispose();
                btnCustomer?.Dispose();
                lnNoCustomer?.Dispose();
                lnHaveCustomer?.Dispose();
                txtNameCustomer?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnDestroy at Payment");
            }
        }

    }
}