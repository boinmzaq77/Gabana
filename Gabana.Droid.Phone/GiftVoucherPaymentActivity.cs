using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class GiftVoucherPaymentActivity : AppCompatActivity
    {
        ImageButton imagebtnBack;
        Button btnApply;
        public static LinearLayout lnNoCustomer, lnHaveCustomer;
        static TextView txtNameCustomer;
        ImageButton btnCustomer;
        TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();
        int PaymentNo;
        decimal Cash;//เงินทอน
        string CURRENCYSYMBOLS;
        static Customer selectCus;
        RecyclerView recyclerview_listGiftvoucher;
        public static List<GiftVoucher> lstvouchers;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        ListGiftVoucher listgiftvoucher;
        public static GiftVoucher voucher;
        LinearLayout lnNoGiftvoucher, lnBtnApply;
        List<GiftVoucher> gifts = new List<GiftVoucher>();
        List<ORM.Master.GiftVoucher> giftVouchers = new List<ORM.Master.GiftVoucher>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.giftvoucher_activity_payment);

                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);

                btnApply = FindViewById<Button>(Resource.Id.btnApply);

                lnNoGiftvoucher = FindViewById<LinearLayout>(Resource.Id.lnNoGiftvoucher);
                lnBtnApply = FindViewById<LinearLayout>(Resource.Id.lnBtnApply);

                btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
                txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
                CheckJwt();
                SetCustomer();

                recyclerview_listGiftvoucher = FindViewById<RecyclerView>(Resource.Id.recyclerview_listGiftvoucher);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listGiftvoucher.HasFixedSize = true;
                recyclerview_listGiftvoucher.SetLayoutManager(mLayoutManager);

                GetGiftvoucherData();

                imagebtnBack.Click += ImagebtnBack_Click;
                btnCustomer.Click += BtnCustomer_Click;

                btnApply.Click += BtnApply_Click;

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                //paymentAmount = 0;
                //foreach (var item in tranWithDetails.tranPayments)
                //{
                //    paymentAmount += item.PaymentAmount;
                //}
                ////amount คือ ยอดที่ต้องจ่าย 
                //amount = Convert.ToDecimal(tranWithDetails.tran.GrandTotal - paymentAmount);

                //txtAmount คือ จำนวนเงินที่จะจ่าย
                SetBtnCharge();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : GiftVoucherPaymentActivity");


            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("at Giftvoucher Payment");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private async void GetGiftvoucherData()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                lstvouchers = new List<GiftVoucher>();
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (giftVouchers == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (giftVouchers.Count == 0)
                    {
                        lstvouchers = new List<GiftVoucher>();
                    }
                    if (giftVouchers.Count > 0)
                    {
                        var lst = giftVouchers.OrderBy(x => x.FmlAmount).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                FmlAmount = item.FmlAmount,
                                GiftVoucherCode = item.GiftVoucherCode,
                                GiftVoucherName = item.GiftVoucherName,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified
                            };
                            await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                            gifts.Add(giftVoucher);
                        }
                        lstvouchers = new List<GiftVoucher>();
                        lstvouchers.AddRange(gifts);
                    }
                }
                else
                {
                    lstvouchers = await giftVoucherManage.GetAllGiftVoucher();
                    if (lstvouchers == null)
                    {
                        Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    }
                }

                listgiftvoucher = new ListGiftVoucher(lstvouchers);
                GiftVoucher_Adapter_Payment giftVoucher_Adapter = new GiftVoucher_Adapter_Payment(listgiftvoucher);
                recyclerview_listGiftvoucher.SetItemViewCacheSize(50);
                recyclerview_listGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

                if (giftVoucher_Adapter.ItemCount == 0)
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Visible;
                    recyclerview_listGiftvoucher.Visibility = ViewStates.Gone;
                    lnBtnApply.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Gone;
                    recyclerview_listGiftvoucher.Visibility = ViewStates.Visible;
                    lnBtnApply.Visibility = ViewStates.Visible;
                }
                dialogLoading.Dismiss();
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackPageViewAsync("GetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void GiftVoucher_Adapter_ItemClick(object sender, int e)
        {
            try
            {
                if (voucher == lstvouchers[e])
                {
                    voucher = null;
                }
                else
                {
                    voucher = lstvouchers[e];
                }

                GiftVoucher_Adapter_Payment giftVoucher_Adapter = new GiftVoucher_Adapter_Payment(listgiftvoucher);
                recyclerview_listGiftvoucher.SetItemViewCacheSize(50);
                recyclerview_listGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

                SetBtnCharge();

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucher_Adapter_ItemClick at giftvoucher");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetBtnCharge()
        {
            if (voucher != null)
            {
                btnApply.Enabled = true;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);

            }
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


        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                btnApply.Enabled = false;

                var check = voucher.FmlAmount.IndexOf('%');
                if (check == -1)
                {
                    Cash = Convert.ToDecimal(voucher.FmlAmount);
                }
                else
                {
                    Cash = ((Convert.ToDecimal(voucher.FmlAmount.Remove(check)) / 100) * tranWithDetails.tran.GrandTotal);
                }

                //คำนวณยอดเงินที่ชำระ บันทึกลง tranpayment  cash (strValue) >= amount (ยอดจ่าย)
                initialData();

                int PaymentNo = tranWithDetails.tranPayments.Count();
                PaymentNo++;

                tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = Cash; //เงินที่จ่าย
                tranPayment.ReferenceNo1 = voucher.GiftVoucherCode;
                tranPayment.Comments = voucher.GiftVoucherName;
                tranWithDetails.tranPayments.Add(tranPayment);

                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                btnApply.Enabled = true;
                return;
            }
        }

        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
            PaymentActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        private void initialData()
        {
            if (tranWithDetails == null)
            {
                return;
            }

            tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentNo = PaymentNo,
                PaymentType = DataCashing.PaymentType,
                PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                CreditCardType = null,
                CardNo = null,
                ExprieDateYYYYMM = null,
                ApproveCode = null,
                TotalRedeemPoint = null,
                //EPaymentType = null,
                RequestNum = null,
                RequestDateTime = null,
                FEPaymentCancel = 0,
                ReferenceNo1 = null,
                ReferenceNo2 = null,
                ReferenceNo3 = null,
                ReferenceNo4 = null,
                Comments = null,
            };
        }
        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            SetCustomer();
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();   
            imagebtnBack.PerformClick();
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'GiftVoucherPaymentActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'GiftVoucherPaymentActivity.openPage' is assigned but its value is never used
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

        }
    }
}