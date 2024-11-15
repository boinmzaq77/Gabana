using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Droid.Tablet.Adapter.Payment;
using TinyInsightsLib;
using AndroidX.RecyclerView.Widget;
using Gabana3.JAM.Trans;
using System.Security.Policy;
using Gabana.Model;
using System.Threading.Tasks;
using AndroidX.SwipeRefreshLayout.Widget;
using System.ComponentModel;
using System.Threading;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_GiftVoucher : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Payment_Fragment_GiftVoucher NewInstance()
        {
            Payment_Fragment_GiftVoucher frag = new Payment_Fragment_GiftVoucher();
            return frag;
        }
        Payment_Fragment_GiftVoucher fragment_giftvoucher;
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_giftvoucher, container, false);
            try
            {
                fragment_giftvoucher = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                ComBineUI();
                SetUIEvent();
                //_ = GetGiftvoucherData();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }

        string CURRENCYSYMBOLS;
        private void SetUIEvent()
        {

            //lnBack.Click += LnBack_Click;
            btnApply.Click += BtnApply_Click;
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            SetBtnCharge();
        }

        //private void LnBack_Click(object sender, EventArgs e)
        //{
        //    PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment","default");
        //}

        FrameLayout lnBack;
        RecyclerView rcvGiftvoucher;
        LinearLayout lnNoGiftvoucher, lnApply;
        Button btnApply;
        SwipeRefreshLayout refreshlayout;

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
                _ = GetGiftvoucherData();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }


        decimal Cash;
        public static TranWithDetailsLocal tranWithDetails;
        int PaymentNo;
        public static TranPayment tranPayment = new TranPayment();
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
                CheckBalance();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                btnApply.Enabled = true;
                return;
            }
        }
        private void CheckBalance()
        {
            try
            {
                var list = tranWithDetails.tranPayments.ToList();
                decimal payAmount = 0;
                foreach (var item in list)
                {
                    payAmount += item.PaymentAmount;

                }
                var change = payAmount - tranWithDetails.tran.GrandPayment;
                var index = list.FindLastIndex(x => x.PaymentType == "CH");
                if (index == list.Count - 1)
                {
                    change = payAmount - tranWithDetails.tran.GrandPayment;
                }
                else
                {
                    change = payAmount - tranWithDetails.tran.GrandTotal;
                }

                decimal payByCash = list.Where(x => x.PaymentType == "CH").Sum(x => x.PaymentAmount);
                decimal payByGiftvoucher = list.Where(x => x.PaymentType == "GV").Sum(x => x.PaymentAmount);

                if (payByGiftvoucher > 0 && payAmount < payByGiftvoucher)
                {
                    change = 0;
                }

                if (payByCash < change)
                {
                    change = payByCash;
                }
                if (change < 0)
                {
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "default");
                }
                else
                {
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "receipt");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void initialData()
        {
            try
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
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void ComBineUI()
        {
            //lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            rcvGiftvoucher = view.FindViewById<RecyclerView>(Resource.Id.rcvGiftvoucher);
            lnNoGiftvoucher = view.FindViewById<LinearLayout>(Resource.Id.lnNoGiftvoucher);
            lnApply = view.FindViewById<LinearLayout>(Resource.Id.lnApply);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipRefresh);
            refreshlayout.Refresh += async (sender, e) =>
            {
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                }
                else
                {
                    DataCashingAll.flagGiftVoucherChange = true;
                    OnResume();
                }
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };
        }
        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        public static List<GiftVoucher> lstvouchers = new List<GiftVoucher>();
        List<ORM.Master.GiftVoucher> giftVouchers = new List<ORM.Master.GiftVoucher>();
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        List<GiftVoucher> gifts = new List<GiftVoucher>();
        ListGiftVoucher listgiftvoucher;
        public static GiftVoucher voucher;
 
        private async Task GetGiftvoucherData()
        {
            try
            {
                gifts = new List<GiftVoucher>();
                lstvouchers = new List<GiftVoucher>();
                if (await GabanaAPI.CheckNetWork())
                {
                    giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (giftVouchers == null)
                    {
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstvouchers = new List<GiftVoucher>();
                    }
                }

                listgiftvoucher = new ListGiftVoucher(lstvouchers);
                Payment_Adapter_GiftVoucher giftVoucher_Adapter = new Payment_Adapter_GiftVoucher(listgiftvoucher);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvGiftvoucher.SetLayoutManager(mLayoutManager);
                rcvGiftvoucher.SetItemViewCacheSize(50);
                rcvGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

                if (giftVoucher_Adapter.ItemCount == 0)
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Visible;
                    rcvGiftvoucher.Visibility = ViewStates.Gone;
                    lnApply.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoGiftvoucher.Visibility = ViewStates.Gone;
                    rcvGiftvoucher.Visibility = ViewStates.Visible;
                    lnApply.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("GetGiftvoucherData at Giftvoucher");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

                Payment_Adapter_GiftVoucher giftVoucher_Adapter = new Payment_Adapter_GiftVoucher(listgiftvoucher);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvGiftvoucher.SetLayoutManager(mLayoutManager);
                rcvGiftvoucher.SetItemViewCacheSize(50);
                rcvGiftvoucher.SetAdapter(giftVoucher_Adapter);
                giftVoucher_Adapter.ItemClick += GiftVoucher_Adapter_ItemClick;

               

                SetBtnCharge();

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucher_Adapter_ItemClick at giftvoucher");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetBtnCharge()
        {
            if (voucher != null)
            {
                btnApply.Enabled = true;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);

            }
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }
    }
}