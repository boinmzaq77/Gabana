using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.More;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Android.Accounts;
using System.Text.RegularExpressions;
using Gabana.ORM.Period;
using Gabana.Droid.Tablet.Dialog;
using Java.Lang.Annotation;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Payment_Fragment_Main NewInstance()
        {
            Payment_Fragment_Main frag = new Payment_Fragment_Main();
            return frag;
        }

        public static Payment_Fragment_Main fragment_main;
        View view;
        public static TranWithDetailsLocal tranWithDetails;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_main, container, false);
            try
            {
                fragment_main = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                CheckJwt();
                ComBineUI();
                SetEventUI();
                SetCustomer();
                ShowDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }

            return view;
        }

        private void SetEventUI()
        {
            try
            {
                btnCustomer.Click += BtnCustomer_Click;
                btnCustomerB.Click += BtnCustomer_Click;
                lnNoCustomer.Click += BtnCustomer_Click;
                lnHaveCustomer.Click += BtnCustomer_Click;
                lnBack.Click += LnBack_Click;

                lnCash.Click += LnCash_Click;
                lnGiftVoucher.Click += LnGiftVoucher_Click;
                lnCreditCard.Click += LnCreditCard_Click;
                lnMyqr.Click += LnMyqr_Click;
                lnSaveOrder.Click += LnSaveOrder_Click;

                //เพิ่ม debit
                lnDebit.Click += LnDebit_Click;
                lnQrCash.Click += LnQrCash_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            //MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnPos, "pos", "default");
            PaymentActivity.payment_main.Finish();
        }

        private void LnMyqr_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "MYQR";
            DataCashing.PaymentType = tranPayment.PaymentType;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "myqr");
        }

        private void LnGiftVoucher_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "GV";
            DataCashing.PaymentType = tranPayment.PaymentType;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "giftvoucher");
        }

        private void LnCreditCard_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "Cr";
            DataCashing.PaymentType = tranPayment.PaymentType;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "credit");
        }

        private void LnDebit_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "Dr";
            DataCashing.PaymentType = tranPayment.PaymentType;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "debit");
        }

        private async void LnQrCash_Click(object sender, EventArgs e)
        {
            //DialogLoading dialogLoading = new DialogLoading();
            try
            {
                lnQrCash.Enabled = false;
                //if (dialogLoading.Cancelable != false)
                //{
                //    dialogLoading.Cancelable = false;
                //    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                //}

                if (!await GabanaAPI.CheckNetWork())
                {
                    //if (dialogLoading != null)
                    //{
                    //    dialogLoading.DismissAllowingStateLoss();
                    //    dialogLoading.Dismiss();
                    //}
                    Toast.MakeText(Application.Context, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    lnQrCash.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    //if (dialogLoading != null)
                    //{
                    //    dialogLoading.DismissAllowingStateLoss();
                    //    dialogLoading.Dismiss();
                    //}
                    Toast.MakeText(Application.Context, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    lnQrCash.Enabled = true;
                    return;
                }

                tranPayment.PaymentType = "QRCH";
                DataCashing.PaymentType = tranPayment.PaymentType;
                double Amount = 0;
                Amount = decimal.ToDouble(amount);
                Amount = Math.Round(Amount, 2, MidpointRounding.AwayFromZero);

                if (Amount < 1)
                {
                    Toast.MakeText(this.Activity, "ไม่สามารถชำระเงินได้ เนื่องจาก จำนวนเงินน้อยเกินไป", ToastLength.Short).Show();
                    lnQrCash.Enabled = true;
                    return;
                }

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
                        var fragment = new Alert_Dialog_PaymentQrCash();
                        fragment.Show(Activity.SupportFragmentManager, nameof(Alert_Dialog_PaymentQrCash));
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, respone_Qr.errorDesc, ToastLength.Short).Show();
                    }
                    //if (dialogLoading != null)
                    //{
                    //    dialogLoading.DismissAllowingStateLoss();
                    //    dialogLoading.Dismiss();
                    //}
                    lnQrCash.Enabled = true;
                    return;
                }

                if (string.IsNullOrEmpty(respone_Qr.qrCode))
                {
                    //if (dialogLoading != null)
                    //{
                    //    dialogLoading.DismissAllowingStateLoss();
                    //    dialogLoading.Dismiss();
                    //}
                    lnQrCash.Enabled = true;
                    return;
                }

                //มีสิทธิ์การใช้งาน ปุ่มม่วง
                var getOpenQrCash = Preferences.Get("OpenQrCash", 0);
                if (getOpenQrCash == 0)
                {
                    Preferences.Set("OpenQrCash", 1); // 1 has , 0 no
                }

                Payment_Fragment_QRCash.SetResponeQRKBank(respone_Qr);
                int count = DataCashing.countGen.countgenQr;
                count++;
                DataCashing.countGen.countgenQr = count;

                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}

                PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "qrcash");
                lnQrCash.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}
            }
        }

        private async void LnSaveOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int limitOrder = 0;
                limitOrder = await UtilsAll.GetCountOrder();
                if (limitOrder > 100)
                {
                    Toast.MakeText(this.Activity, "จำนวน Order สูงสุดต่อสาขา 100 Order", ToastLength.Short).Show();
                }
                else
                {
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "saveorder");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                Cart_Dailog_Customer dialog = new Cart_Dailog_Customer();
                Cart_Dailog_Customer.SetTranWithDetail(tranWithDetails);
                var fragment = new Cart_Dailog_Customer();
                fragment.Cancelable = false;
                fragment.Show(Activity.SupportFragmentManager, nameof(Cart_Dailog_Customer));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        ORM.MerchantDB.TranPayment tranPayment = new ORM.MerchantDB.TranPayment();
        private void LnCash_Click(object sender, EventArgs e)
        {
            tranPayment.PaymentType = "CH";
            DataCashing.PaymentType = tranPayment.PaymentType;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment","cash");
        }
       

        decimal amount = 0;
        public override void OnResume()
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                tranWithDetails = PaymentActivity.tranWithDetails;
                SetCustomer();
                ShowDetail();

                int getOpenQrCash = Preferences.Get("OpenQrCash", 0);
                if (getOpenQrCash == 1)
                {
                    imageQrCash.SetBackgroundResource(Resource.Mipmap.PaymentQRCash);
                    textQrCash.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static Customer selectCus;
        private void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    lnHaveCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Visible;

                }
                else
                {
                    lnHaveCustomer.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }

        private void ShowDetail()
        {
            try
            {
                tranWithDetails = PaymentActivity.tranWithDetails;
                decimal paymentAmount = 0;
                paymentAmount = tranWithDetails.tranPayments.Sum(x => x.PaymentAmount);
                //amount คือ ยอดที่ต้องจ่าย  
                var GrandTotal = Regex.Match(tranWithDetails.tran.GrandTotal.ToString(), @"^*[0-9.,]+$");
                //amount = Convert.ToDecimal(Convert.ToDecimal(GrandTotal.Value) - paymentAmount);
                amount = Convert.ToDecimal(tranWithDetails.tran.GrandTotal - paymentAmount);
                if (amount <= 0)
                {
                    //StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));

                    //BalanceActivity.SetTranDetail(tranWithDetails);
                }
                textAmount.Text = Utils.DisplayDecimal(amount);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }


        LinearLayout lnBack, lnNoCustomer, lnHaveCustomer;
        ImageView btnCustomer, btnCustomerB;
        TextView txtNameCustomer, textAmount, textQrCash;
        FrameLayout lnCash, lnSaveOrder, lnGiftVoucher, lnCreditCard, lnDebit, lnMyqr, lnQrCash, lnQrCredit, lnWechat;
        ImageButton imageQrCash;

        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnNoCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);

            btnCustomer = view.FindViewById<ImageView>(Resource.Id.btnCustomer);
            btnCustomerB = view.FindViewById<ImageView>(Resource.Id.btnCustomer);

            txtNameCustomer = view.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            textAmount = view.FindViewById<TextView>(Resource.Id.textAmount);

            lnCash = view.FindViewById<FrameLayout>(Resource.Id.lnCash);
            lnSaveOrder = view.FindViewById<FrameLayout>(Resource.Id.lnSaveOrder);
            lnGiftVoucher = view.FindViewById<FrameLayout>(Resource.Id.lnGiftVoucher);
            lnCreditCard = view.FindViewById<FrameLayout>(Resource.Id.lnCreditCard);
            lnDebit = view.FindViewById<FrameLayout>(Resource.Id.lnDebit);
            lnMyqr = view.FindViewById<FrameLayout>(Resource.Id.lnMyqr);
            lnQrCash = view.FindViewById<FrameLayout>(Resource.Id.lnQrCash);
            lnQrCredit = view.FindViewById<FrameLayout>(Resource.Id.lnQrCredit);
            lnWechat = view.FindViewById<FrameLayout>(Resource.Id.lnWechat);
            imageQrCash = view.FindViewById<ImageButton>(Resource.Id.imageQrCash);
            textQrCash = view.FindViewById<TextView>(Resource.Id.textQrCash);
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    return;
                }
                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}