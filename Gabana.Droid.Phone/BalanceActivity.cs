using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
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
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class BalanceActivity : AppCompatActivity
    {
        public static TranWithDetailsLocal tranWithDetails;
        public static BalanceActivity balanceActivity;
        ImageButton btnBack;
        LinearLayout lnNoCustomer, lnHaveCustomer, lnBack;
        TextView txtNameCustomer, textSignMoney, textreceive, textChange, txtdescrpit_amount;
        Button btnViewREceipt , btnBackPos; 
        RecyclerView recyclerView_listpayment;
        TransManage transManage = new TransManage();
        public static Customer selectCus;
        string emplogin;
        

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                balanceActivity = this;
                SetContentView(Resource.Layout.balance_activity_main);

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                InitializeUIElements();

                CheckJwt();
                //Utils.ConvertTranwithDetailtoJson(tranWithDetails);
                Log.Debug("swipapp", "balance OnCreate");

                SetupCurrencySymbol();

                List<TranPayment> listpayment = tranWithDetails.tranPayments.ToList();
                SetupRecyclerView(listpayment);
                await SetupPaymentDetails(listpayment);
                SetCustomer();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : BalanceActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync(" Balance");
            }
        }

        private async Task SetupPaymentDetails(List<TranPayment> listpayment)
        {
            try
            {
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
                    btnViewREceipt.Click += AddPayment;
                    btnBack.Click += AddPayment;
                    lnBack.Click += AddPayment;
                    btnBackPos.Visibility = ViewStates.Gone;
                }
                else
                {
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
                    btnBack.Click += BtnBack_Click;
                    lnBack.Click += BtnBack_Click;
                    btnBackPos.Visibility = ViewStates.Visible;
                    btnBackPos.Click += BtnBack_Click;

                    //เพิ่มมาเพื่อแก้เคส genqr แล้วมีการ genqr มากกว่า 1 ครั้ง
                    DataCashing.countGen = new CountGenQr() { countgenQr = 0, Tranno = string.Empty };
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("at Balance SetupPaymentDetails");
            }
        }

        private void SetupRecyclerView(List<TranPayment> lsttranpament)
        {
            ListPayment listPayment = new ListPayment(lsttranpament);
            Balance_Adapter_Main balance_Adapter_Main = new Balance_Adapter_Main(listPayment);
            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerView_listpayment.HasFixedSize = true;
            recyclerView_listpayment.SetLayoutManager(mLayoutManager);
            recyclerView_listpayment.SetAdapter(balance_Adapter_Main);
        }

        private void InitializeUIElements()
        {
            textreceive = FindViewById<TextView>(Resource.Id.textreceive);
            textChange = FindViewById<TextView>(Resource.Id.textChange);
            txtdescrpit_amount = FindViewById<TextView>(Resource.Id.txtdescrpit_amount);
            lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
            textSignMoney = FindViewById<TextView>(Resource.Id.textSignMoney);

            emplogin = Preferences.Get("User", "");

            textreceive.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
            btnViewREceipt = FindViewById<Button>(Resource.Id.btnViewREceipt);
            btnBackPos = FindViewById<Button>(Resource.Id.btnBackPos);
            btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            recyclerView_listpayment = FindViewById<RecyclerView>(Resource.Id.recyclerView_listpayment);
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
            textSignMoney.Text = DataCashing.Language == "th" ? currencyTH : currencyEn;
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
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private void AddPayment(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
            PaymentActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }

        public async void SetCustomer()
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
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        private async void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Toast.MakeText(this, GetString(Resource.String.textbillsuccess), ToastLength.Short).Show();
                PosActivity.totlaItems = 0;
                DataCashing.setQuantityToCart = 1;
                DataCashing.SysCustomerID = 999;
                tranWithDetails = null;
                tranWithDetails = await Utils.initialData();
                PosActivity.SetTranDetail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" BtnBack_Click at Balance");
            }
        }

        protected  override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                SetCustomer();

                //if (tranWithDetails == null)
                //{
                //    string jsonTran = Preferences.Get("tranWithDetails", "");
                //    tranWithDetails = Utils.ConvertJsontoTranwithDetail(jsonTran);                    
                //    Log.Debug("swipapp", "balance Onresume tranWithDetails == null");
                //    Log.Debug("swipapp", jsonTran);
                //}
                //Utils.ConvertTranwithDetailtoJson(tranWithDetails);
                Log.Debug("swipapp", "balance Onresume");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                base.OnRestart();
            }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            btnBack.PerformClick();
        }

        private async void BtnViewREceipt_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(ReceiptActivity)));
                ReceiptActivity.SetTranDeail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync(" BtnViewREceipt_Click at Balance");
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

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
                if (!result)
                {
                    Toast.MakeText(this,"บันทึกไม่สำเร็จ",ToastLength.Short).Show();
                    return;
                }

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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertToTrans at Balance TranNo = " + tranWithDetails?.tran?.TranNo.ToString());
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

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                textreceive?.Dispose();
                textChange?.Dispose();
                txtdescrpit_amount?.Dispose();
                lnNoCustomer?.Dispose();
                lnHaveCustomer?.Dispose();
                txtNameCustomer?.Dispose();
                textSignMoney?.Dispose();
                btnViewREceipt?.Dispose();
                btnBackPos?.Dispose();
                btnBack?.Dispose();
                lnBack?.Dispose();
                recyclerView_listpayment?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Balance at Cart");
            }

        }
    }
}