using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AutoMapper;
using Gabana.Droid.Tablet.Adapter.Bill;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource.Sync;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Bill
{
    public class BillHistory_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static BillHistory_Fragment_Main NewInstance()
        {
            BillHistory_Fragment_Main frag = new BillHistory_Fragment_Main();
            return frag;
        }
        View view;
        public static BillHistory_Fragment_Main fragment_main;
        string LoginType, Username;

        TranWithDetailsLocal tranWithDetails = new TranWithDetailsLocal();
        List<TransHistory> lsttransHistory = new List<TransHistory>();
        List<TransHistoryNew> lsttransHistoryNew = new List<TransHistoryNew>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.billhistory_fragment_main, container, false);
            try
            {
                fragment_main = this;
                LoginType = Preferences.Get("LoginType", "");
                Username = Preferences.Get("User", "");
                CheckJwt();
                CombineUI();
                SetUIEvent();
                DataCashing.isModifyBillhistory = true;
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        private async Task GetDetail()
        {
            try
            {
                if (!DataCashing.isModifyBillhistory)
                {
                    return;
                }

                if (string.IsNullOrEmpty(txtSearchBill.Text))
                {
                    GetNameBranch();
                    await SetBillHistory();
                    SetBtnSearchItem();
                }
                else
                {
                    await SearchFilterBillNewDetail();
                    SetBtnSearchItem();
                }
                DataCashing.isModifyBillhistory = false;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetail Bill History");
            }
        }

        DateTime latestTranDate = new DateTime();
        internal static int offset;
        int position;
        internal static string branchID = "1";
        Bill_Adapter_Main bill_Adapter_Main;

        private void SetUIEvent()
        {
            lnChooseBranch.Click += LnChooseBranch_Click;
            btnSearchBill.Click += BtnSearchBill_Click;
            lnSearchBill.Click += BtnSearchBill_Click;

            txtSearchBill.TextChanged += TxtSearchBill_TextChanged;
            txtSearchBill.KeyPress += TxtSearchBill_KeyPress;
            txtSearchBill.FocusChange += TxtSearchBill_FocusChange;
            lnSearchFilter.Click += LnSearchFilter_Click;

            latestTranDate = DateTime.UtcNow;
            offset = 0;
            position = 0;

            branchID = Preferences.Get("Branch", "");
            GetNameBranch();

            refreshlayout.Refresh += async (sender, e) =>
            {
                this.Activity.RunOnUiThread(async () =>
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await Task.Run(() => StartPostTrantoAPI());

                        DataCashing.isModifyBillhistory = true;
                        OnResume();

                        BackgroundWorker work = new BackgroundWorker();
                        work.DoWork += Work_DoWork;
                        work.RunWorkerCompleted += Work_RunWorkerCompleted;
                        work.RunWorkerAsync();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                        BackgroundWorker work = new BackgroundWorker();
                        work.DoWork += Work_DoWork;
                        work.RunWorkerCompleted += Work_RunWorkerCompleted;
                        work.RunWorkerAsync();
                    }
                });
                DataCashing.isModifyBillhistory = true;
                OnResume();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        private async void GetNameBranch()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                var branchName = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();
                txtBranchName.Text = branchName.BranchName?.ToString();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetNameBranch at Bill History");
                return;
            }
        }
        private void LnSearchFilter_Click(object sender, EventArgs e)
        {
            //StartActivity(new Android.Content.Intent(this.Activity, typeof(BillHistoryFilterActivity)));
        }

        private void TxtSearchBill_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearchBill.Text.Trim()))
            {
                btnSearchBill.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchBill.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }
        private async void TxtSearchBill_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            //SetBtnSearchItem();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                //await SearchFilterBillDetail();
                await SearchFilterBillNewDetail();
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    MainActivity.main_activity.CloseKeyboard(view);
                }
            }
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
            {
                e.Handled = false;
            }

            e.Handled = false;
            if (e.Handled)
            {
                string input = string.Empty;
                switch (e.KeyCode)
                {
                    case Keycode.Num0:
                        input += "0";
                        break;
                    case Keycode.Num1:
                        input += "1";
                        break;
                    case Keycode.Num2:
                        input += "2";
                        break;
                    case Keycode.Num3:
                        input += "3";
                        break;
                    case Keycode.Num4:
                        input += "4";
                        break;
                    case Keycode.Num5:
                        input += "5";
                        break;
                    case Keycode.Num6:
                        input += "6";
                        break;
                    case Keycode.Num7:
                        input += "7";
                        break;
                    case Keycode.Num8:
                        input += "8";
                        break;
                    case Keycode.Num9:
                        input += "9";
                        break;
                    default:
                        break;
                }
                //e.Handled = false;
                txtSearchBill.Text += input;
                txtSearchBill.SetSelection(txtSearchBill.Text.Length);
                return;
            }
        }
        ListBillHistoryNew ListbillNew;
        async Task SearchFilterBillNewDetail()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                List<TransHistoryNew> lstBillSearch = new List<TransHistoryNew>();
                if (await GabanaAPI.CheckNetWork())
                {
                    TransHistory transHistory = new TransHistory();
                    transHistory = await GetOnlineSearch(textSearchBill);
                    lstBillSearch = new List<TransHistoryNew>();

                    if (transHistory != null)
                    {
                        TransHistoryNew transHistoryNew = new TransHistoryNew()
                        {
                            tranNo = transHistory.tranNo,
                            tranDate = transHistory.tranDate,
                            customerName = transHistory.customerName,
                            fCancel = transHistory.fCancel,
                            grandTotal = transHistory.grandTotal,
                            paymentType = transHistory.paymentType,
                            TypeOfflineOrOnline = 'O',
                            FWaiting = 0,
                        };
                        lstBillSearch = new List<TransHistoryNew>
                        {
                            transHistoryNew
                        };
                    }

                    ListbillNew = new ListBillHistoryNew(lstBillSearch);
                }
                else
                {
                    TransHistoryNew transNew = new TransHistoryNew();
                    transNew = await GetOfflineSearch(textSearchBill);
                    if (transNew == null)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.notdata), ToastLength.Short).Show();
                    }
                    else
                    {
                        lstBillSearch = new List<TransHistoryNew>
                        {
                            transNew
                        };
                        ListbillNew = new ListBillHistoryNew(lstBillSearch);
                    }
                }

                bill_Adapter_Main = new Bill_Adapter_Main(ListbillNew);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListBill.SetLayoutManager(gridLayoutItem);
                rcvListBill.HasFixedSize = true;
                rcvListBill.SetItemViewCacheSize(100);
                rcvListBill.SetAdapter(bill_Adapter_Main);
                bill_Adapter_Main.ItemClick += Bill_Adapter_Main_ItemClick;
                if (bill_Adapter_Main.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(textSearchBill))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        lnNoBill.Visibility = ViewStates.Gone;
                        rcvListBill.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        lnNoBill.Visibility = ViewStates.Visible;
                        rcvListBill.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    lnNoBill.Visibility = ViewStates.Gone;
                    rcvListBill.Visibility = ViewStates.Visible;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SearchFilterBillDetail at Bill History");
            }
        }
        private async void Bill_Adapter_Main_ItemClick(object sender, int e)
        {
            var bill = ListbillNew.Trans[e];
            if (!DataCashing.CheckNet)
            {
                var dataTran = await transManage.GetTran(DataCashingAll.MerchantId, Convert.ToInt32(branchID), bill.tranNo);
                if (dataTran == null)
                {
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnBill, "billhistory", "default");
                    return;
                }
            }
            DataCashing.billHistory = bill;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnBill, "billhistory", "billdetail");
        }
        async Task<TransHistoryNew> GetOfflineSearch(string SearchTranNo)
        {
            try
            {
                TransHistoryNew transHistoryNew = new TransHistoryNew();
                string tranNo = SearchTranNo;
                if (lsttransHistoryNew.Count > 0)
                {
                    transHistoryNew = lsttransHistoryNew.Where(x => x.tranNo.ToString() == tranNo).FirstOrDefault();
                    return transHistoryNew;
                }
                else
                {
                    return transHistoryNew;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("GetOfflineSearch at Bill History");
                return new TransHistoryNew();
            }
        }

        TransHistory transHistory = new TransHistory();
        async Task<TransHistory> GetOnlineSearch(string SearchTranNo)
        {
            try
            {
                string tranNo = SearchTranNo;
                Gabana3.JAM.Trans.TranWithDetails tranConvert = new Gabana3.JAM.Trans.TranWithDetails();
                tranConvert = await GabanaAPI.GetDataTranSearch(Convert.ToInt32(branchID), tranNo);

                if (tranConvert == null)
                {
                    return null;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    //struct ของ table
                    cfg.CreateMap<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>();
                    cfg.CreateMap<ORM.Period.Tran, Tran>();
                    cfg.CreateMap<ORM.Period.TranPayment, TranPayment>();
                });

                var Imapper = config.CreateMapper();
                //TranWithDetails
                tranWithDetails.tran = Imapper.Map<ORM.Period.Tran, Tran>(tranConvert.tran);
                tranWithDetails.tranPayments = Imapper.Map<List<ORM.Period.TranPayment>, List<TranPayment>>(tranConvert.tranPayments);

                transHistory = new TransHistoryNew()
                {
                    tranNo = tranWithDetails.tran.TranNo,
                    tranDate = tranWithDetails.tran.TranDate,
                    customerName = tranWithDetails.tran.CustomerName,
                    grandTotal = tranWithDetails.tran.GrandTotal,
                    fCancel = tranWithDetails.tran.FCancel,
                    paymentType = tranWithDetails.tranPayments[0].PaymentType,
                };

                return transHistory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("GetOnlineSearch at Bill History");
                return new TransHistory();
            }
        }

        private async void TxtSearchBill_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            textSearchBill = txtSearchBill.Text.Trim();
            if (string.IsNullOrEmpty(textSearchBill))
            {
                //await setDataBill();
                await SetBillHistory();
            }
            SetBtnSearchItem();
        }
        int Last;
        async Task SetBillHistory()
        {
            try
            {

                List<TransHistoryNew> lsttranHistoryCloud = new List<TransHistoryNew>();
                List<TransHistoryNew> lsttranHistoryDevice = new List<TransHistoryNew>();

                if (DataCashing.CheckNet)
                {
                    //Cloud
                    lsttranHistoryCloud = await GetOnlineHistoryCloud();
                    //Device
                    lsttranHistoryDevice = await GetOfflineHistoryDevice();

                    if (lsttranHistoryCloud != null && lsttranHistoryDevice != null)
                    {

                        //Merge listOrder
                        HashSet<string> sentIDs = new HashSet<string>(lsttranHistoryCloud.Select(s => s.tranNo));
                        var results = lsttranHistoryDevice.Where(m => sentIDs.Contains(m.tranNo)).ToList();
                        if (results.Count > 0)
                        {
                            foreach (var item in results)
                            {
                                var removelstDevice = lsttranHistoryDevice.FindIndex(x => x.tranNo == item.tranNo);
                                if (removelstDevice != -1)
                                {
                                    lsttranHistoryDevice.RemoveAt(removelstDevice);
                                }
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                        return;
                    }

                    lsttranHistoryCloud.AddRange(lsttranHistoryDevice);
                    lsttranHistoryCloud = lsttranHistoryCloud.OrderByDescending(x => x.tranDate).ThenByDescending(x => x.tranNo).ToList();
                    lsttransHistoryNew = lsttranHistoryCloud;
                }
                else
                {
                    lsttranHistoryDevice = await GetHistoryDevice();
                    lsttransHistoryNew = lsttranHistoryDevice;
                }

                Last = lsttransHistoryNew.Count;

                //Sort ตาม Desc
                ListbillNew = new ListBillHistoryNew(lsttransHistoryNew);

                bill_Adapter_Main = new Bill_Adapter_Main(ListbillNew);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListBill.SetLayoutManager(gridLayoutItem);
                rcvListBill.HasFixedSize = true;
                rcvListBill.SetItemViewCacheSize(100);
                if (Last >= 100)
                {
                    bill_Adapter_Main.OnCardCellbtnIndex5 += Bill_Adapter_Main_OnCardCellbtnIndex5;
                }
                rcvListBill.SetAdapter(bill_Adapter_Main);
                position = bill_Adapter_Main.ItemCount;
                bill_Adapter_Main.ItemClick += Bill_Adapter_Main_ItemClick;

                if (bill_Adapter_Main.ItemCount == 0)
                {
                    rcvListBill.Visibility = ViewStates.Gone;
                    lnNoBill.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnNoBill.Visibility = ViewStates.Gone;
                    rcvListBill.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetBillHistory at Order");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        bool islast;
        private async void Bill_Adapter_Main_OnCardCellbtnIndex5()
        {
            try
            {
                offset++;
                if (islast)
                {
                    position = ListbillNew.Count - 1;

                    List<TransHistoryNew> lsttranHistoryCloud = new List<TransHistoryNew>();
                    List<TransHistoryNew> lsttranHistoryDevice = new List<TransHistoryNew>();

                    if (await GabanaAPI.CheckNetWork())
                    {
                        //Cloud
                        lsttranHistoryCloud = await GetOnlineHistoryCloud();
                        //Device
                        lsttranHistoryDevice = await GetOfflineHistoryDevice();

                        if (lsttranHistoryCloud != null && lsttranHistoryDevice != null)
                        {
                            //Merge listOrder
                            HashSet<string> sentIDs = new HashSet<string>(lsttranHistoryCloud.Select(s => s.tranNo));
                            var results = lsttranHistoryDevice.Where(m => sentIDs.Contains(m.tranNo)).ToList();
                            if (results.Count > 0)
                            {
                                foreach (var item in results)
                                {
                                    var removelstDevice = lsttranHistoryDevice.FindIndex(x => x.tranNo == item.tranNo);
                                    if (removelstDevice != -1)
                                    {
                                        lsttranHistoryDevice.RemoveAt(removelstDevice);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Toast.MakeText(this.Activity, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                            return;
                        }

                        lsttranHistoryCloud.AddRange(lsttranHistoryDevice);
                        lsttranHistoryCloud = lsttranHistoryCloud.OrderByDescending(x => x.tranDate).ThenByDescending(x => x.tranNo).ToList();
                        lsttransHistoryNew = lsttranHistoryCloud;
                    }
                    else
                    {
                        lsttranHistoryDevice = await GetHistoryDevice();
                        lsttransHistoryNew = lsttranHistoryDevice;
                    }

                    if (Last == lsttransHistoryNew.Count)
                    {
                        islast = false;
                    }

                    Last = lsttransHistoryNew.Count;
                    ListbillNew = new ListBillHistoryNew(lsttransHistoryNew);
                    var ad = rcvListBill.GetAdapter() as Bill_Adapter_Main;
                    ad.refresh(ListbillNew);
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Bill_Adapter_Main_OnCardCellbtnIndex5 at Bill History");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                DataCashing.isModifyBillhistory = true;
                await GetDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
            }
        }

        async Task<List<TransHistoryNew>> GetHistoryDevice()
        {
            try
            {
                List<TransHistoryNew> lsthistories = new List<TransHistoryNew>();
                transManage = new TransManage();
                lsthistories = await transManage.GetTranHistory(DataCashingAll.MerchantId, Convert.ToInt32(branchID));
                return lsthistories;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("GetOfflineHistoryDevice at Bill History");
                return new List<TransHistoryNew>();
            }
        }

        TransManage transManage = new TransManage();
        async Task<List<TransHistoryNew>> GetOfflineHistoryDevice()
        {
            try
            {
                List<TransHistoryNew> lsthistories = new List<TransHistoryNew>();
                transManage = new TransManage();
                lsthistories = await transManage.GetTranHistoryNew(DataCashingAll.MerchantId, Convert.ToInt32(branchID));
                return lsthistories;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("GetOfflineHistoryDevice at Bill History");
                return new List<TransHistoryNew>();
            }
        }

        async Task<List<TransHistoryNew>> GetOnlineHistoryCloud()
        {
            try
            {
                List<TransHistoryNew> lst = new List<TransHistoryNew>();
                List<TransHistoryNew> lstTransHistoryCloud = new List<TransHistoryNew>();
                List<TransHistory> tranConvert = new List<TransHistory>();
                string date = Utils.ChangeDateTime(latestTranDate);
                int row = offset;
                tranConvert = await GabanaAPI.GetDataTranHistory(Convert.ToInt32(branchID), offset, date);


                //mapping Order to OrderNew
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TransHistory, TransHistoryNew>();
                });

                var Imapper = config.CreateMapper();
                lstTransHistoryCloud = Imapper.Map<List<TransHistory>, List<TransHistoryNew>>(tranConvert);

                foreach (var item in lstTransHistoryCloud)
                {
                    item.TypeOfflineOrOnline = 'O';
                    item.FWaiting = 0;
                    lst.Add(item);
                }

                if (offset == 0)
                {
                    lsttransHistoryNew = new List<TransHistoryNew>();
                    lsttransHistoryNew.AddRange(lst);
                }
                else
                {
                    lsttransHistoryNew.AddRange(lst);
                }
                return lsttransHistoryNew;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineHistory at Bill History");
                return new List<TransHistoryNew>();
            }
        }

        private void BtnSearchBill_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }

        string textSearchBill;
        private void SetClearSearchText()
        {
            textSearchBill = "";
            txtSearchBill.Text = string.Empty;
            SetBtnSearchItem();
        }
        private void SetBtnSearchItem()
        {
            if (string.IsNullOrEmpty(textSearchBill))
            {
                btnSearchBill.SetBackgroundResource(Resource.Mipmap.Search);
            }
            else
            {
                btnSearchBill.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }
        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            var fragment = new Employee_Dialog_SelectBranch();
            Employee_Dialog_SelectBranch dialog = new Employee_Dialog_SelectBranch();
            fragment.Show(Activity.SupportFragmentManager, nameof(Employee_Dialog_SelectBranch));
        }

        TextView txtBranchName;
        LinearLayout lnChooseBranch, lnSearchBill;
        ImageButton btnSearchBill;
        EditText txtSearchBill;
        FrameLayout lnSearchFilter;
        SwipeRefreshLayout refreshlayout;
        RecyclerView rcvListBill;
        LinearLayout lnNoBill, lnNoDataSearch;
        private void CombineUI()
        {
            txtBranchName = view.FindViewById<TextView>(Resource.Id.txtBranchName);
            lnChooseBranch = view.FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
            lnSearchBill = view.FindViewById<LinearLayout>(Resource.Id.lnSearchBill);
            btnSearchBill = view.FindViewById<ImageButton>(Resource.Id.btnSearchBill);
            txtSearchBill = view.FindViewById<EditText>(Resource.Id.txtSearchBill);
            lnSearchFilter = view.FindViewById<FrameLayout>(Resource.Id.lnSearchFilter);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            rcvListBill = view.FindViewById<RecyclerView>(Resource.Id.rcvListBill);
            lnNoBill = view.FindViewById<LinearLayout>(Resource.Id.lnNoBill);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
        }

        public void BillHistoryFocus(TransHistoryNew TranHis)
        {
            try
            {
                var index = lsttransHistoryNew.FindIndex(x => x.tranNo == TranHis.tranNo.ToString());
                if (index > -1)
                {
                    lsttransHistoryNew[index] = TranHis;
                    bill_Adapter_Main.NotifyItemChanged(index);
                    return;
                }

                lsttransHistoryNew.Insert(0, TranHis);
                rcvListBill.SmoothScrollToPosition(0);
                bill_Adapter_Main.NotifyItemInserted(0);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BillHistoryFocus at BillHistory");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void StartPostTrantoAPI()
        {
            try
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(1000); // รอเวลา 500 มิลลิวินาที
                    await PostTrantoAPI();
                });
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task PostTrantoAPI()
        {
            byte[] imageByteArray = null;
            DateTime CheckdateTime = DateTime.UtcNow;
            try
            {
                TransManage transManage = new TransManage();
                TranPaymentManage tranPaymentManage = new TranPaymentManage();
                List<Tran> lsttrans = await transManage.GetTranFwaitingOneTwo();
                if (lsttrans != null)
                {
                    //เช็คกับ JobQueue ก่อน ถ้ามีใน JobQueue ให้เอาออกจาก list ที่จะยิงตรง

                    IList<JobInfo> allPendingJobs = JobQueue.jobScheduler2?.AllPendingJobs;
                    if (allPendingJobs != null)
                    {
                        foreach (var item in lsttrans)
                        {
                            lsttrans = lsttrans.Where(item =>
                             !allPendingJobs.Any(job =>
                                 string.Equals(job.Extras?.GetString("systransid_value")?.ToLower(), item.TranNo, StringComparison.OrdinalIgnoreCase))
                            ).ToList();
                        }
                    }
                    /////////////////////////////////////////////////////////

                    foreach (var item in lsttrans)
                    {
                        await Task.Delay(1000); // รอเวลา 500 มิลลิวินาที

                        Tran tran = new Tran();
                        tran = await transManage.GetTranAndroid((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                        if (tran != null)
                        {
                            if (CheckdateTime > tran.WaitSendingTime)
                            {
                                var Payment = await tranPaymentManage.GetTranPayment((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                                if (Payment != null)
                                {
                                    var Paymentpicture = Payment.Where(x => !string.IsNullOrEmpty(x.PicturePath)).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(Paymentpicture?.PicturePath))
                                    {
                                        imageByteArray = await Utils.streamImageOffine(Paymentpicture?.PicturePath); //แนบรูป ได้ 1 ใบ
                                    }
                                }

                                await TransSync.SentTransAndroid((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString(), imageByteArray);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
    }
    public class ListBillHistoryNew
    {
        public List<Model.TransHistoryNew> Trans;
        static List<Model.TransHistoryNew> builitem;
        public ListBillHistoryNew(List<Model.TransHistoryNew> transHistories)
        {
            builitem = transHistories;
            this.Trans = builitem;
        }

        public int Count
        {
            get
            {
                return Trans == null ? 0 : Trans.Count;
            }
        }

        public Model.TransHistory this[int i]
        {
            get { return Trans == null ? null : Trans[i]; }
        }
    }

}