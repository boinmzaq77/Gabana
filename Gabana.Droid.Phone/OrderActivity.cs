using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AutoMapper;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class OrderActivity : AppCompatActivity
    {
        LinearLayout lnBack;
        public static OrderActivity orederactivity;
        RecyclerView recyclerview_listorder;
        ListOrders listOrders;
        ImageButton btnSort, btnSearchOrder;
        EditText textSearchOrder;
        public static bool sort;
        public static TranWithDetailsLocal tranWithDetails;
        TransManage transManage = new TransManage();
        string SearchOrder;
        FrameLayout lnSearchTopping;
        LinearLayout lnNoOrder, lnOrder;
        List<OrderNew> lstClound = new List<OrderNew>();
        List<Tran> lstDevice = new List<Tran>();
        DialogLoading dialogLoading = new DialogLoading();
        public static OrderNew OrderNew;
        SwipeRefreshLayout refreshlayout;
        public static bool OrderCurrentActivity = false, IsActive = false;
        public static Tran checkOder;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.orders_activity);
                orederactivity = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnNoOrder = FindViewById<LinearLayout>(Resource.Id.lnNoOrder);
                lnOrder = FindViewById<LinearLayout>(Resource.Id.lnOrder);

                lnSearchTopping = FindViewById<FrameLayout>(Resource.Id.lnSearchTopping);

                recyclerview_listorder = FindViewById<RecyclerView>(Resource.Id.recyclerview_listorder);
                btnSort = FindViewById<ImageButton>(Resource.Id.btnSort);
                btnSearchOrder = FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
                btnSearchOrder.Click += BtnSearchOrder_Click;
                textSearchOrder = FindViewById<EditText>(Resource.Id.textSearchTopping);
                textSearchOrder.TextChanged += TextSearchOrder_TextChanged;
                textSearchOrder.KeyPress += TextSearchOrder_KeyPress;
                textSearchOrder.FocusChange += TextSearchOrder_FocusChange;
                btnSort.Click += BtnSort_Click;
                lnBack.Click += LnBack_Click;

                CheckJwt();
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += (sender, e) =>
                {
                    //flagData = true;
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                sort = false;
                //SetDataOrder();
                SetBtnSort();
                //RemoveOrder30Day(); คอนนี้ยังไม่ใช้งาน ให่ใช้วิธีไม่แสดงข้อมูลเท่านั้น 04/11/2021
                //flagData = true;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : OrderActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Order");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void TextSearchOrder_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearchOrder.Text.Trim()))
            {
                btnSearchOrder.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchOrder.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearchOrder_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterOrderData();
                textSearchOrder.ClearFocus();
            }
            View view = this.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
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
                textSearchOrder.Text += input;
                textSearchOrder.SetSelection(textSearchOrder.Text.Length);
                return;
            }
        }

        private async void TextSearchOrder_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchOrder = textSearchOrder.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(SearchOrder))
            {
                await SetDataOrder();
            }
            SetBtnSearch();
        }

        private async void BtnSearchOrder_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            await SetDataOrder();
        }

        private void SetClearSearchText()
        {
            SearchOrder = "";
            textSearchOrder.Text = string.Empty;
            SetBtnSearch();
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchOrder))
            {
                btnSearchOrder.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearchOrder.Enabled = false;
            }
            else
            {
                btnSearchOrder.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearchOrder.Enabled = true;
            }
        }

        private async void BtnSort_Click(object sender, EventArgs e)
        {
            if (sort)
            {
                sort = false;
            }
            else
            {
                sort = true;
            }
            SetBtnSort();
            await SetDataOrder();
        }

        private void SetBtnSort()
        {
            if (sort)
            {
                btnSort.SetBackgroundResource(Resource.Mipmap.SortASC);
            }
            else
            {
                btnSort.SetBackgroundResource(Resource.Mipmap.SortDESC);
            }
        }

        //order ทั้งหมด ของ Device and Clound
        private async Task SetDataOrder()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                TransManage transManage = new TransManage();
                List<OrderNew> orders = new List<OrderNew>();
                //Online
                if (await GabanaAPI.CheckNetWork())
                {
                    //Cloud
                    List<Order> ordercloud = new List<Order>();
                    ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                    if (ordercloud == null)
                    {
                        ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                    }

                    //mapping Order to OrderNew
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Gabana3.JAM.Trans.Order, Model.OrderNew>();
                    });

                    var Imapper = config.CreateMapper();
                    lstClound = Imapper.Map<List<Gabana3.JAM.Trans.Order>, List<Model.OrderNew>>(ordercloud);

                    //Device
                    //List<Tran>
                    lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (lstDevice == null)
                    {
                        lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    }

                    if (lstClound != null & lstDevice != null)
                    {
                        //Merge listOrder
                        HashSet<string> sentIDs = new HashSet<string>(lstClound.Select(s => s.tranNo));
                        var results = lstDevice.Where(m => sentIDs.Contains(m.TranNo)).ToList();
                        if (results.Count > 0)
                        {
                            foreach (var item in results)
                            {
                                var removelstDevice = lstDevice.FindIndex(x => x.TranNo == item.TranNo);
                                if (removelstDevice != -1)
                                {
                                    lstDevice.RemoveAt(removelstDevice);
                                }
                            }
                        }
                    }
                    else
                    {
                        dialogLoading.Dismiss();
                        Toast.MakeText(this, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                        return;
                    }

                    List<OrderNew> listThisDevice = new List<OrderNew>();
                    List<OrderNew> listOtherevice = new List<OrderNew>();

                    foreach (var itemmap in lstClound)
                    {
                        if (DataCashingAll.Device.DeviceNo == itemmap.deviceNo)
                        {
                            itemmap.TypeOfflineOrOnline = 'O';
                            itemmap.FWaiting = 0;
                            listThisDevice.Add(itemmap);
                        }
                        else
                        {
                            itemmap.TypeOfflineOrOnline = 'O';
                            itemmap.FWaiting = 0;
                            listOtherevice.Add(itemmap);
                        }
                    }

                    foreach (var itemmap in lstDevice)
                    {
                        OrderNew mappOrder = new OrderNew()
                        {
                            tranNo = itemmap.TranNo,
                            orderName = itemmap.OrderName,
                            tranDate = itemmap.TranDate,
                            deviceNo = (int)itemmap.DeviceNo,
                            grandTotal = itemmap.GrandTotal,
                            comments = itemmap.Comments,
                            TypeOfflineOrOnline = 'O',
                            FWaiting = itemmap.FWaitSending
                        };

                        if (DataCashingAll.Device.DeviceNo == mappOrder.deviceNo)
                        {
                            listThisDevice.Add(mappOrder);
                        }
                        else
                        {
                            listOtherevice.Add(mappOrder);
                        }
                    }

                    List<OrderNew> listShow = new List<OrderNew>();
                    foreach (var item in listThisDevice)
                    {
                        listShow.Add(item);
                    }

                    foreach (var item in listOtherevice)
                    {
                        listShow.Add(item);
                    }

                    if (listShow != null)
                    {
                        if (sort)
                        {
                            listShow = listShow.OrderByDescending(x => x.tranDate).ToList();
                        }
                        listOrders = new ListOrders(listShow);
                    }
                }
                else
                {
                    //Offline
                    //order ทั้งหมด ของ Device
                    var lstOrder = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (lstOrder != null)
                    {
                        foreach (var item in lstOrder)
                        {
                            OrderNew mappOrder = new OrderNew()
                            {
                                tranNo = item.TranNo,
                                orderName = item.OrderName,
                                tranDate = item.TranDate,
                                deviceNo = (int)item.DeviceNo,
                                grandTotal = item.GrandTotal,
                                comments = item.Comments,
                                TypeOfflineOrOnline = 'F',
                                FWaiting = item.FWaitSending
                            };
                            orders.Add(mappOrder);
                        }
                        if (sort)
                        {
                            orders = orders.OrderByDescending(x => x.tranDate).ToList();
                        }

                        listOrders = new ListOrders(orders);
                    }
                }

                Order_Adapter_Main order_adapter_main = new Order_Adapter_Main(listOrders);
                GridLayoutManager gridLayout = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listorder.SetLayoutManager(gridLayout);
                recyclerview_listorder.HasFixedSize = true;
                recyclerview_listorder.SetItemViewCacheSize(100);
                recyclerview_listorder.SetAdapter(order_adapter_main);
                order_adapter_main.ItemClick += Order_adapter_main_ItemClick;

                if (order_adapter_main.ItemCount > 0)
                {
                    lnSearchTopping.Visibility = ViewStates.Visible;
                    lnOrder.Visibility = ViewStates.Visible;
                    lnNoOrder.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnSearchTopping.Visibility = ViewStates.Gone;
                    lnOrder.Visibility = ViewStates.Gone;
                    lnNoOrder.Visibility = ViewStates.Visible;
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
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDataOrder at Order");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void Order_adapter_main_ItemClick(object sender, int e)
        {
            //ดึงรายการ order มาดูข้อมูล แก้ไข เปิดบิล
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                OrderNew = listOrders[e];

                await transManage.UpdateTranNo(tranWithDetails.tran.TranNo);

                //ckeck ว่าเคยเลือก order แล้วหรือยัง
                checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (checkOder != null)
                {
                    //เพิ่ม dialog
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.order_dialog_openorder.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }

                //Click ไป Cart
                SelectOrder(listOrders[e]);
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Order_adapter_main_ItemClick at Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        internal async static void CancelOrder()
        {
            await Utils.CancelTranOrder(tranWithDetails);
            tranWithDetails = null;
            DataCashing.isCurrentOrder = false;
            orederactivity.SelectOrder(OrderNew);
        }

        private async void SelectOrder(OrderNew od)
        {
            try
            {
                var order = od;
                DataCashing.ModifyTranOrder = false;

                if (order.TypeOfflineOrOnline == '\0')
                {
                    Toast.MakeText(this, "TypeOfflineOrOnline is null", ToastLength.Short).Show();
                    return;
                }

                //Online
                if (order.TypeOfflineOrOnline == 'O' && order.FWaiting == 0)
                {
                    //เลือกรายการ Order ที่ List จะทำการเรียก Api เพื่อไป Get ข้อมูล Order ลงมา 
                    var date = Utils.ChangeDateTime(order.tranDate);
                    var getTranDetail = await GabanaAPI.GetDataTranOrderDetail(DataCashingAll.SysBranchId, order.tranNo, date);
                    if (getTranDetail != null)
                    {
                        if (getTranDetail.tran.Comments == "The order has already been applied.")
                        {
                            Toast.MakeText(this, "The order has already been applied.", ToastLength.Short).Show();
                            dialogLoading.Dismiss();
                            return;
                        }

                        var config = new MapperConfiguration(cfg =>
                        {
                            //struct ของ table
                            cfg.CreateMap<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>();
                            cfg.CreateMap<Gabana3.JAM.Trans.TranDetailItemWithTopping, Model.TranDetailItemWithTopping>();
                            cfg.CreateMap<ORM.Period.Tran, Tran>();
                            cfg.CreateMap<ORM.Period.TranDetailItemTopping, TranDetailItemTopping>();
                            cfg.CreateMap<ORM.Period.TranDetailItem, TranDetailItemNew>();
                            cfg.CreateMap<ORM.Period.TranTradDiscount, TranTradDiscount>();
                            cfg.CreateMap<ORM.Period.TranPayment, TranPayment>();
                        });

                        // TranWithDetailsLocal
                        var Imapper = config.CreateMapper();
                        tranWithDetails = Imapper.Map<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>(getTranDetail);

                        tranWithDetails.tran.Status = 100;
                        tranWithDetails.tran.FWaitSending = 0;
                        tranWithDetails.tran.LocalDataStatus = 'U';
                        tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                        tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                        tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                        //ทำการ Insert/Update ข้อมูลเก็บไว้ที่ device 
                        var getDeviceTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, order.tranNo);


                        #region เคสดึงออเดอร์ แล้วสินค้ามีการเปลี่ยนแปลง  
                        TranWithDetailsLocal TranTemp = new TranWithDetailsLocal();
                        TranTemp = tranWithDetails;
                        List<string> lstSysItemIdStatusD;

                        #region 1. เคสสินค้าถูกลบ 
                        //1. เคสสินค้าถูกลบ 
                        //ตรวจสอบข้อมูลก่อนจะบันทึกว่ามีสินค้าที่ไม่มีที่เครื่องไหม ถ้ามีให้ remove สินค้านั้น แล้ว calTran ใหม่ เพื่อบันทึกข้อมูลลงเครื่องก่อน กันกรณีกดเลือกออเดอร์แล้วปิดแอป
                        lstSysItemIdStatusD = new List<string>();
                        lstSysItemIdStatusD = Utils.CheckStatusIteminCart(TranTemp);
                        if (lstSysItemIdStatusD.Count > 0)
                        {
                            foreach (var item in lstSysItemIdStatusD)
                            {
                                int number;
                                bool success = int.TryParse(item, out number);
                                if (success)
                                {
                                    TranTemp.tranDetailItemWithToppings.RemoveAll(x => x.tranDetailItem.SysItemID == Convert.ToInt32(item));
                                }
                            }
                            TranTemp = BLTrans.Caltran(TranTemp);
                        }
                        #endregion                        

                        //check DataStatus != 'D'
                        bool InsertUpdateOrder = false;
                        if (getDeviceTran == null)
                        {
                            // ไม่มีที่ Device Insert                           
                            InsertUpdateOrder = await transManage.InsertTran(TranTemp);
                        }
                        else
                        {
                            // มีที่ DeviceUpdate                            
                            InsertUpdateOrder = await transManage.UpdateTran(TranTemp.tran);
                        }
                        #endregion
                        DataCashing.isCurrentOrder = true;

                        #region Return Order
                        //Return Order
                        //if (!InsertUpdateOrder)
                        //{                            
                        //    var stringDate = UtilsAll.ChangeDateTime(tranWithDetails.tran.TranDate);
                        //    var result = await GabanaAPI.PutDataTran((int)tranWithDetails.tran.SysBranchID, tranWithDetails.tran.TranNo, stringDate);
                        //    if (result.Status)
                        //    {
                        //        tranWithDetails.tran.FWaitSending = 0;
                        //        tranWithDetails.tran.Status = 110;
                        //    }
                        //    else
                        //    {
                        //        tranWithDetails.tran.FWaitSending = 1;
                        //        tranWithDetails.tran.Status = 100;
                        //    }
                        //    await transManage.UpdateTran(tranWithDetails.tran);
                        //} 
                        #endregion
                    }
                }
                else if (order.TypeOfflineOrOnline == 'O' && order.FWaiting != 0)
                {
                    //Fwaiting = 1,2
                    //เมื่อเลือกรายการ Order ที่ต้องการแล้วจะต้อง Set FWaitSending = 0  , เพิ่ม status = 100 , และ  LocalDataStatus = 'I'

                    tranWithDetails = await GetLocalTranOrderDetail(order.tranNo);
                    tranWithDetails.tran.FWaitSending = 0;
                    tranWithDetails.tran.Status = 100;
                    tranWithDetails.tran.LocalDataStatus = 'I';
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                    tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                    tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                    await transManage.UpdateTran(tranWithDetails.tran);
                }
                else
                {
                    //Offline
                    //เมื่อเลือกรายการ Order ที่ต้องการแล้วจะต้อง Set FWaitSending = 0  , เพิ่ม status = 100 , และ  LocalDataStatus = 'I'

                    tranWithDetails = await GetLocalTranOrderDetail(order.tranNo);
                    tranWithDetails.tran.FWaitSending = 0;
                    tranWithDetails.tran.LocalDataStatus = 'I';
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                    tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                    tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                    await transManage.UpdateTran(tranWithDetails.tran);
                }

                DataCashing.SysCustomerID = tranWithDetails.tran.SysCustomerID;
                StartActivity(new Intent(Application.Context, typeof(CartActivity)));
                CartActivity.SetTranDetail(tranWithDetails);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                IsActive = false;
                this.Finish();
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectOrder");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetFilterOrderData()
        {
            try
            {
                // CultureInfo.CurrentCulture = new CultureInfo("en-US");

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                TransManage transManage = new TransManage();
                List<OrderNew> orders = new List<OrderNew>();
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    //Online
                    //order ทั้งหมด ของ Device and Clound
                    //Cloud
                    var ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                    if (ordercloud == null)
                    {
                        ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                    }

                    //mapping Order to OrderNew
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Gabana3.JAM.Trans.Order, Model.OrderNew>();
                    });

                    var Imapper = config.CreateMapper();
                    lstClound = Imapper.Map<List<Gabana3.JAM.Trans.Order>, List<Model.OrderNew>>(ordercloud);

                    //Device
                    //List<Tran>
                    lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (lstDevice == null)
                    {
                        lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    }
                    if (lstClound != null & lstDevice != null)
                    {
                        //Merge listOrder
                        HashSet<string> sentIDs = new HashSet<string>(lstClound.Select(s => s.tranNo));
                        var results = lstDevice.Where(m => sentIDs.Contains(m.TranNo)).ToList();
                        if (results.Count > 0)
                        {
                            foreach (var item in results)
                            {
                                var removelstDevice = lstDevice.FindIndex(x => x.TranNo == item.TranNo);
                                if (removelstDevice != -1)
                                {
                                    lstDevice.RemoveAt(removelstDevice);
                                }
                            }
                        }
                    }
                    else
                    {
                        dialogLoading.Dismiss();

                        Toast.MakeText(this, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                        return;
                    }

                    // var lstmapOrder = new List<Order>();
                    var listThisDevice = new List<OrderNew>();
                    var listOtherevice = new List<OrderNew>();

                    foreach (var itemmap in lstDevice)
                    {
                        OrderNew mappOrder = new OrderNew()
                        {
                            tranNo = itemmap.TranNo,
                            orderName = itemmap.OrderName,
                            tranDate = itemmap.TranDate,
                            deviceNo = (int)itemmap.DeviceNo,
                            grandTotal = itemmap.GrandTotal,
                            comments = itemmap.Comments,
                            TypeOfflineOrOnline = 'O'
                        };

                        if (DataCashingAll.Device.DeviceNo == mappOrder.deviceNo)
                        {
                            listThisDevice.Add(mappOrder);
                        }
                        else
                        {
                            listOtherevice.Add(mappOrder);
                        }
                    }

                    foreach (var item in listThisDevice)
                    {
                        lstClound.Add(item);
                    }
                    foreach (var item in listOtherevice)
                    {
                        lstClound.Add(item);
                    }

                    if (lstClound != null)
                    {
                        if (sort)
                        {
                            lstClound = lstClound.OrderByDescending(x => x.tranDate).ToList();
                        }
                        listOrders = new ListOrders(lstClound);
                    }
                }
                else
                {
                    //Offline
                    //order ทั้งหมด ของ Device
                    var lstOrder = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (lstOrder != null)
                    {
                        if (sort)
                        {
                            orders = orders.OrderByDescending(x => x.tranDate).ToList();
                        }

                        listOrders = new ListOrders(orders);
                    }
                }

                //Search Order
                List<OrderNew> dataOrder = new List<OrderNew>();

                var regexItem = new System.Text.RegularExpressions.Regex("^[device]+$");
                if (regexItem.IsMatch(SearchOrder)) //เฉพาะคำว่า device
                {
                    string searchtxt = string.Empty;
                    if (SearchOrder.Length >= 6)
                    {
                        searchtxt = SearchOrder.Remove(0, 6);
                    }

                    if (searchtxt.Length == 0 | SearchOrder.Length < 6)
                    {
                        dataOrder = listOrders.Trans;
                    }
                    else
                    {
                        dataOrder = listOrders.Trans.Where(x => x.tranNo.Contains(searchtxt) | x.tranDate.Day.Equals(searchtxt) | x.tranDate.Month.Equals(searchtxt) | x.tranDate.Year.Equals(searchtxt) | x.orderName.Contains(searchtxt) | x.tranDate.ToString().Contains(searchtxt) | x.grandTotal.ToString().Contains(searchtxt) | x.deviceNo.ToString().Contains(searchtxt) | x.comments.Contains(searchtxt)).ToList();
                    }
                }
                else
                {
                    //deviceXX , อื่นๆ
                    if (SearchOrder.StartsWith("device"))
                    {
                        string searchtxt = string.Empty;
                        if (SearchOrder.Length >= 6)
                        {
                            searchtxt = SearchOrder.Remove(0, 6);
                        }

                        if (searchtxt.Length == 0 | SearchOrder.Length < 6)
                        {
                            dataOrder = listOrders.Trans;
                        }
                        else
                        {
                            dataOrder = listOrders.Trans.Where(x => x.tranNo.Contains(searchtxt) | x.tranDate.Day.Equals(searchtxt) | x.tranDate.Month.Equals(searchtxt) | x.tranDate.Year.Equals(searchtxt) | x.orderName.Contains(searchtxt) | x.tranDate.ToString().Contains(searchtxt) | x.grandTotal.ToString().Contains(searchtxt) | x.deviceNo.ToString().Contains(searchtxt) | x.comments.Contains(searchtxt)).ToList();
                        }
                    }
                    else
                    {
                        dataOrder = listOrders.Trans.Where(x => x.tranNo.Contains(SearchOrder) | x.tranDate.Day.Equals(SearchOrder) | x.tranDate.Month.Equals(SearchOrder) | x.tranDate.Year.Equals(SearchOrder) | x.orderName.Contains(SearchOrder) | x.tranDate.ToString().Contains(SearchOrder) | x.grandTotal.ToString().Contains(SearchOrder) | x.deviceNo.ToString().Contains(SearchOrder) | x.comments.Contains(SearchOrder)).ToList();
                    }
                }

                listOrders = new ListOrders(dataOrder);

                Order_Adapter_Main order_adapter_main = new Order_Adapter_Main(listOrders);
                GridLayoutManager gridLayout = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listorder.SetLayoutManager(gridLayout);
                recyclerview_listorder.HasFixedSize = true;
                recyclerview_listorder.SetItemViewCacheSize(100);
                recyclerview_listorder.SetAdapter(order_adapter_main);
                order_adapter_main.ItemClick += Order_adapter_main_ItemClick;

                if (order_adapter_main.ItemCount > 0)
                {
                    lnOrder.Visibility = ViewStates.Visible;
                    lnNoOrder.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnOrder.Visibility = ViewStates.Gone;
                    lnNoOrder.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterOrderData at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        //List Order
        //ลบ Order ที่ไม่มีการชำระเงินเกิน 30 วัน
        public async void RemoveOrder30Day()
        {
            try
            {
                TransManage transManage = new TransManage();
                await transManage.DeleteTranOrder30Day((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("RemoveOrder30Day at Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        internal static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        async Task<TranWithDetailsLocal> GetLocalTranOrderDetail(string tranNo)
        {
            try
            {
                // CultureInfo.CurrentCulture = new CultureInfo("en-US");
                List<TranWithDetailsLocal> lst = new List<TranWithDetailsLocal>();
                List<TranDetailItem> tranDetail = new List<TranDetailItem>();
                List<TranDetailItemTopping> tranDetailTopping = new List<TranDetailItemTopping>();
                List<TranPayment> tranPayment = new List<TranPayment>();
                List<TranTradDiscount> tranDiscount = new List<TranTradDiscount>();
                Gabana.Model.TranDetailItemWithTopping detailItemWithTopping = new Gabana.Model.TranDetailItemWithTopping();
                List<Gabana.Model.TranDetailItemWithTopping> lstdetailItemWithTopping = new List<Gabana.Model.TranDetailItemWithTopping>();

                TransManage transManage = new TransManage();
                TranDetailItemManage tranDetailItemManage = new TranDetailItemManage();
                TranPaymentManage tranPaymentManage = new TranPaymentManage();
                TranTradDiscountManage tranTradDiscountManage = new TranTradDiscountManage();
                TranDetailItemToppingManage toppingManage = new TranDetailItemToppingManage();

                tranWithDetails = new TranWithDetailsLocal();
                var Datatran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranNo);
                tranDetail = await tranDetailItemManage.GetTranDetailItem(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, Datatran.TranNo);
                tranPayment = await tranPaymentManage.GetTranPayment(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, Datatran.TranNo);
                tranDiscount = await tranTradDiscountManage.GetTranTradDiscount(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, Datatran.TranNo);

                tranWithDetails.tran = Datatran;

                foreach (var item in tranDetail)
                {
                    tranDetailTopping = await toppingManage.GetTranDetailItemTopping(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, Datatran.TranNo, (int)item.DetailNo); // รอแก้ไข TranDetailItemTopping

                    TranDetailItemNew detailItem = new TranDetailItemNew()
                    {
                        Amount = item.Amount,
                        Comments = item.Comments,
                        CumulativeSum = item.CumulativeSum,
                        DetailNo = item.DetailNo,
                        Discount = item.Discount,
                        DiscountPromotion = item.DiscountPromotion,
                        DiscountRedeem = item.DiscountRedeem,
                        EstimateCost = item.EstimateCost,
                        FmlDiscountRow = item.FmlDiscountRow,
                        FProcess = item.FProcess,
                        ItemName = item.ItemName,
                        ItemPrice = item.ItemPrice,
                        MerchantID = item.MerchantID,
                        Price = item.Price,
                        PricePerWeight = item.PricePerWeight,
                        ProfitAmount = item.ProfitAmount,
                        Quantity = item.Quantity,
                        RedeemCode = item.RedeemCode,
                        SaleItemType = item.SaleItemType,
                        SizeName = item.SizeName,
                        SubAmount = item.SubAmount,
                        SumToppingEstimateCost = item.SumToppingEstimateCost,
                        SumToppingPrice = item.SumToppingPrice,
                        SysBranchID = item.SysBranchID,
                        SysItemID = item.SysItemID,
                        TaxBaseAmount = item.TaxBaseAmount,
                        TaxType = item.TaxType,
                        TotalPrice = item.TotalPrice,
                        TranNo = item.TranNo,
                        UnitName = item.UnitName,
                        VatAmount = item.VatAmount,
                        Weight = item.Weight,
                        WeightTranDisc = item.WeightTranDisc,
                        WeightUnitName = item.WeightUnitName,
                    };
                    detailItemWithTopping = new Model.TranDetailItemWithTopping()
                    {
                        tranDetailItem = detailItem,
                        tranDetailItemToppings = tranDetailTopping
                    };
                    tranWithDetails.tranDetailItemWithToppings.Add(detailItemWithTopping);
                }

                tranWithDetails.tranPayments = tranPayment;
                tranWithDetails.tranTradDiscounts = tranDiscount;

                return tranWithDetails;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOfflineTranOrderDetail at Order");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return new TranWithDetailsLocal();
            }
        }

        protected override async void OnResume()
        {
            base.OnResume();

            CheckJwt();
            IsActive = true;
            //if (flagData)
            //{
            if (string.IsNullOrEmpty(SearchOrder))
            {
                await SetDataOrder();
                SetBtnSearch();
            }
            //flagData = false;
            //}

        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'OrderActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'OrderActivity.openPage' is assigned but its value is never used
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

