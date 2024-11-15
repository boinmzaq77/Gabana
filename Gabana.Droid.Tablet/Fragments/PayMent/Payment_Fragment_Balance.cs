using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Payment;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_Balance : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Payment_Fragment_Balance NewInstance()
        {
            Payment_Fragment_Balance frag = new Payment_Fragment_Balance();
            return frag;
        }

        public static Payment_Fragment_Balance fragment_balance;
        View view;
        public static TranWithDetailsLocal tranWithDetails;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_balance, container, false);
            try
            {
                fragment_balance = this;

                tranWithDetails = PaymentActivity.tranWithDetails;
                CheckJwt();
                ComBineUI();
                SetUiEvent();
                //SetCustomer();
                //ShowDetail();

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }

            return view;
        }

        private void SetUiEvent()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

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

                tranWithDetails = PaymentActivity.tranWithDetails;
                CheckJwt();
                SetCustomer();
                ShowDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        string emplogin;
        private async void ShowDetail()
        {
            //DialogLoading dialogLoading = new DialogLoading();
            try
            {
                //if (dialogLoading.Cancelable != false)
                //{
                //    dialogLoading.Cancelable = false;
                //    dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
                //}

                emplogin = Preferences.Get("User", "");

                SetupCurrencySymbol();

                List<TranPayment> listpayment = tranWithDetails.tranPayments.ToList();
                SetupRecyclerView(listpayment);

                await SetupPaymentDetails(listpayment);
                //textreceive.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);

                SetCustomer();
                //dialogLoading.Dismiss();

                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}

                _ = TinyInsights.TrackPageViewAsync("OnCreateView : Payment fragment Balance");
            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at Payment_Fragment_Balance");
                this.Activity.RunOnUiThread(() => { Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show(); });
            }
        }

        private void SetupRecyclerView(List<TranPayment> lsttranpament)
        {
            ListPayment listPayment = new ListPayment(lsttranpament);
            Payment_Adapter_Balance balance_Adapter_Main = new Payment_Adapter_Balance(listPayment);
            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
            rcvlistpayment.HasFixedSize = true;
            rcvlistpayment.SetLayoutManager(mLayoutManager);
            rcvlistpayment.SetAdapter(balance_Adapter_Main);
        }

        private async Task SetupPaymentDetails(List<TranPayment> listpayment)
        {
            try
            {
                if (fragment_balance == null)
                {
                    return;
                }

                decimal payAmount = tranWithDetails.tranPayments.Sum(item => item.PaymentAmount);
                var change = payAmount - tranWithDetails.tran.GrandPayment;
                var index = listpayment.FindLastIndex(x => x.PaymentType == "CH");
                if (index == listpayment.Count - 1)
                {
                    change = payAmount - tranWithDetails.tran.GrandPayment;
                }
                else
                {
                    change = payAmount - tranWithDetails.tran.GrandTotal;
                }

                decimal payByCash = listpayment.Where(x => x.PaymentType == "CH").Sum(x => x.PaymentAmount);
                decimal payByGiftvoucher = listpayment.Where(x => x.PaymentType == "GV").Sum(x => x.PaymentAmount);

                if (payByGiftvoucher > 0 && payAmount < payByGiftvoucher)
                {
                    change = 0;
                }

                if (payByCash > 0)
                {
                    textreceive.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandPayment);
                }

                if (payByCash < change)
                {
                    change = payByCash;
                }

                DataCashing.ModifyTranOrder = true;
                if (change < 0)
                {
                    txtdescrpit_amount.Text = GetString(Resource.String.balance_activity_balance);
                    textChange.Text = Utils.DisplayDecimal(change * -1);
                    tranWithDetails.tran.Change = (decimal)change * -1;
                    btnViewREceipt.Text = GetString(Resource.String.balance_activity_addpayment);
                    DataCashing.ChangePayment = true;
                    btnViewREceipt.Click += AddPayment_Click;
                    btnBack.Click += AddPayment_Click;
                    lnBack.Click += AddPayment_Click;
                    btnBackPos.Visibility = ViewStates.Gone;
                }
                else
                {
                    var CashdrawerConfig = string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.CASHDRAWER) ? "0" : DataCashingAll.setmerchantConfig.CASHDRAWER;
                    if (CashdrawerConfig == "1")
                    {
                       await Utils.Kick();
                    }

                        //Insert to LocalDB
                    tranWithDetails.tran.Change = (decimal)change;

                    //CustomerName
                    if (string.IsNullOrEmpty(tranWithDetails.tran.CustomerName))
                    {
                        tranWithDetails.tran.CustomerName = "ลูกค้าทั่วไป";
                    }

                    if (tranWithDetails.tran.TranType == 'O')
                    {
                        await PaymentTranOrder();
                    }

                    await InsertToTrans();
                    textChange.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
                    txtdescrpit_amount.Text = GetString(Resource.String.balance_activity_change);
                    btnViewREceipt.Text = GetString(Resource.String.balance_activity_viewrecipt);
                    btnViewREceipt.Click += BtnViewREceipt_Click;
                    btnViewREceipt.Visibility = ViewStates.Gone;
                    btnBack.Click += BtnBack_Click;
                    lnBack.Click += BtnBack_Click;
                    btnBackPos.Visibility = ViewStates.Visible;
                    btnBackPos.Click += BtnBack_Click;

                    //เพิ่มมาเพื่อแก้เคส genqr แล้วมีการ genqr มากกว่า 1 ครั้ง
                    DataCashing.countGen = new CountGenQr() { countgenQr = 0, Tranno = string.Empty };
                    DataCashing.saveqrReceipt = false;
                }
            }
            catch (Exception ex)
            {
                this.Activity.RunOnUiThread(() => { Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show(); });
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("at Balance SetupPaymentDetails");
            }
        }

        private void SetupCurrencySymbol()
        {
            try
            {
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
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
                textSignMoney.Text = DataCashing.Language == "th" ? currencyTH : currencyEn;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Customer selectCus;
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
        private async void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.textbillsuccess), ToastLength.Short).Show();
                POS_Fragment_Main.totlaItems = 0;
                DataCashing.setQuantityToCart = 1;
                DataCashing.SysCustomerID = 999;
                //DataCashing.PaymentNo = 0;
               
                //Initial ค่าใหม่หลังจากเปิดการขายรอบใหม่
                tranWithDetails = null;
                tranWithDetails = await Utils.initialData();
                MainActivity.tranWithDetails = tranWithDetails;
                //POS_Fragment_Main.SetTranDetail(tranWithDetails);
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();
                PaymentActivity.tranWithDetails = tranWithDetails;
                PaymentActivity.payment_main.OnBackPressed();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" BtnBack_Click at Balance");
            }
        }

        private async void BtnViewREceipt_Click(object sender, EventArgs e)
        {
            try
            {
                //StartActivity(new Intent(Application.Context, typeof(ReceiptActivity)));
                //PaymentActivity.SetTranDetail(tranWithDetails);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync(" BtnViewREceipt_Click at Balance");
            }
        }

        TransManage transManage = new TransManage();
        async Task InsertToTrans()
        {
            try
            {
                //Insert Trans
                if (tranWithDetails == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(tranWithDetails.tran.TranNo))
                {
                    DeviceTranRunningNoManage DeviceTranRunningNo = new DeviceTranRunningNoManage();
                    var maxtranno = await DeviceTranRunningNo.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
                    maxtranno++;
                    tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
                }

                tranWithDetails.tran.SellerName = emplogin;
                tranWithDetails.tran.LastUserModified = emplogin;


                var result = await transManage.InsertTran(tranWithDetails);
                //if (await GabanaAPI.CheckSpeedConnection())
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendTrans((int)tranWithDetails.tran.MerchantID, (int)tranWithDetails.tran.SysBranchID, tranWithDetails.tran.TranNo);
                }
                else
                {
                    tranWithDetails.tran.Status = 10;
                    tranWithDetails.tran.FWaitSending = 2;
                    var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                }

                //return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertToTrans at Balance TranNo = " + tranWithDetails?.tran?.TranNo.ToString());
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                //return false;
            }
        }

        private void AddPayment_Click(object sender, EventArgs e)
        {
            MainActivity.tranWithDetails = tranWithDetails;
            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment","default");
        }

        async Task PaymentTranOrder()
        {
            //เลือก order มาแล้วต้องการจ่ายเงิน
            //ทำการสร้าง Order ใหม่ขึ้นมาแทน
            try
            {
                var updatetTran = false;
                Model.TranWithDetailsLocal TranWithDetailsnewTran = new TranWithDetailsLocal();

                if (tranWithDetails.tran.FWaitSending == 0 & tranWithDetails.tran.Status == 100)
                {
                    //Old Tran
                    tranWithDetails.tran.Status = 120;
                    tranWithDetails.tran.FWaitSending = 0;
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    updatetTran = await transManage.UpdateTran(tranWithDetails.tran);

                    //New Tran
                    TranWithDetailsnewTran = await Utils.initialData();
                    string newTranNo = TranWithDetailsnewTran.tran.TranNo;
                    if (TranWithDetailsnewTran != null)
                    {
                        TranWithDetailsnewTran.tran = tranWithDetails.tran;
                        TranWithDetailsnewTran.tran.TranNo = newTranNo;
                        TranWithDetailsnewTran.tran.TranType = 'B';
                        TranWithDetailsnewTran.tran.FWaitSending = 2;
                        TranWithDetailsnewTran.tran.Status = 10;
                        TranWithDetailsnewTran.tran.LocalDataStatus = 'I';
                        TranWithDetailsnewTran.tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings;
                        TranWithDetailsnewTran.tranPayments = tranWithDetails.tranPayments;
                        TranWithDetailsnewTran.tranTradDiscounts = tranWithDetails.tranTradDiscounts;

                        //แก้ไข tranNo ให้เป็นตัวใหม่
                        foreach (var item in TranWithDetailsnewTran.tranDetailItemWithToppings)
                        {
                            //TranDetail
                            item.tranDetailItem.TranNo = newTranNo;

                            //TranDetailTopping
                            foreach (var itemTopping in item.tranDetailItemToppings)
                            {
                                itemTopping.TranNo = newTranNo;
                            }
                        }

                        //TranDiscount
                        foreach (var item in TranWithDetailsnewTran.tranTradDiscounts)
                        {
                            //TranDetail
                            item.TranNo = newTranNo;
                        }

                        //TranPayment
                        foreach (var item in TranWithDetailsnewTran.tranPayments)
                        {
                            //TranDetail
                            item.TranNo = newTranNo;
                        }
                    }
                }

                //Set TranWithDetailsnewTran to  tranWithDetails เพื่อไปใช้งานต่อ
                if (updatetTran)
                {
                    tranWithDetails = TranWithDetailsnewTran;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("PaymentTranOrder at Payment");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }

        TextView textreceive, textChange, txtdescrpit_amount, txtNameCustomer, textSignMoney;
        LinearLayout lnNoCustomer, lnHaveCustomer, lnBack;
        RecyclerView rcvlistpayment;
        Button btnViewREceipt, btnBackPos;
        ImageButton btnBack;

        private void ComBineUI()
        {
            textreceive = view.FindViewById<TextView>(Resource.Id.textreceive);
            textChange = view.FindViewById<TextView>(Resource.Id.textChange);
            txtdescrpit_amount = view.FindViewById<TextView>(Resource.Id.txtdescrpit_amount);
            lnNoCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            txtNameCustomer = view.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            textSignMoney = view.FindViewById<TextView>(Resource.Id.textSignMoney);
            rcvlistpayment = view.FindViewById<RecyclerView>(Resource.Id.rcvlistpayment);
            btnViewREceipt = view.FindViewById<Button>(Resource.Id.btnViewREceipt);
            btnBackPos = view.FindViewById<Button>(Resource.Id.btnBackPos);
            btnBack = view.FindViewById<ImageButton>(Resource.Id.btnBack);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
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