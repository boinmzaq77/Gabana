using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AutoMapper;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using TinyInsightsLib;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Adapter.Pos;
using Java.Lang.Annotation;
using Gabana.Droid.Tablet.Fragments.POS;
using Android.Views.InputMethods;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Pos_Dialog_Order : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Pos_Dialog_Order NewInstance()
        {
            var frag = new Pos_Dialog_Order { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public static Pos_Dialog_Order dialog_order;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_order, container, false);
            try
            {
                dialog_order = this;
                CombinUI();
                SetUIEvent();
                tranWithDetails = MainActivity.tranWithDetails;
                return view;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Pos_Dialog_Order");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        public static bool sort;
        private void SetUIEvent()
        {
            textSearchOrder.TextChanged += TextSearchOrder_TextChanged; 
            textSearchOrder.KeyPress += TextSearchOrder_KeyPress; 
            textSearchOrder.FocusChange += TextSearchOrder_FocusChange; 
            btnSort.Click += BtnSort_Click; 
            lnBack.Click += LnBack_Click; 
            btnSearchOrder.Click += BtnSearchOrder_Click;
            swipRefresh.Refresh += (sender, e) =>
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

        }
        public async override void OnResume()
        { 
            base.OnResume();
            IsActive = true;
            if (string.IsNullOrEmpty(SearchOrder))
            {
                await SetDataOrder();
                SetBtnSearch();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dismiss();
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

        private async void SetFilterOrderData()
        {
            try
            {
                // CultureInfo.CurrentCulture = new CultureInfo("en-US");

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                TransManage transManage = new TransManage();
                List<OrderNew> orders = new List<OrderNew>();
                if (await GabanaAPI.CheckNetWork())
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

                        Toast.MakeText(this.Activity, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
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

                Pos_Adapter_Order pos_adapter_order = new Pos_Adapter_Order(listOrders);
                GridLayoutManager gridLayout = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListOrder.SetLayoutManager(gridLayout);
                rcvListOrder.HasFixedSize = true;
                rcvListOrder.SetItemViewCacheSize(100);
                rcvListOrder.SetAdapter(pos_adapter_order);
                pos_adapter_order.ItemClick += Pos_adapter_order_ItemClick ;

                if (pos_adapter_order.ItemCount > 0)
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
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

        internal async static void CancelOrder()
        {
            await Utils.CancelTranOrder(tranWithDetails);
            tranWithDetails = null;
            DataCashing.isCurrentOrder = false;
            dialog_order.SelectOrder(OrderNew);
        }

        public static bool OrderCurrentActivity = false, IsActive = false ;
        private async void SelectOrder(OrderNew od)
        {            
            try
            {
                var order = od;
                DataCashing.ModifyTranOrder = false;

                if (order.TypeOfflineOrOnline == '\0')
                {
                    Toast.MakeText(this.Activity, "TypeOfflineOrOnline is null", ToastLength.Short).Show();
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
                            Toast.MakeText(this.Activity, "The order has already been applied.", ToastLength.Short).Show();
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

                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                IsActive = false;
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectOrder");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return new TranWithDetailsLocal();
            }
        }

        //order ทั้งหมด ของ Device and Clound
        List<OrderNew> lstClound = new List<OrderNew>();
        List<Tran> lstDevice = new List<Tran>();
        ListOrders listOrders;
        private async Task SetDataOrder()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
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
                        Toast.MakeText(this.Activity, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
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

                Pos_Adapter_Order pos_adapter_order = new Pos_Adapter_Order(listOrders);
                GridLayoutManager gridLayout = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListOrder.SetLayoutManager(gridLayout);
                rcvListOrder.HasFixedSize = true;
                rcvListOrder.SetItemViewCacheSize(100);
                rcvListOrder.SetAdapter(pos_adapter_order);
                pos_adapter_order.ItemClick += Pos_adapter_order_ItemClick; 

                if (pos_adapter_order.ItemCount > 0)
                {
                    lnSearchOrder.Visibility = ViewStates.Visible;
                    lnOrder.Visibility = ViewStates.Visible;
                    lnNoOrder.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnSearchOrder.Visibility = ViewStates.Gone;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public static OrderNew OrderNew;
        TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static Tran checkOder;
        DialogLoading dialogLoading = new DialogLoading();

        private async void Pos_adapter_order_ItemClick(object sender, int e)
        {            
            //ดึงรายการ order มาดูข้อมูล แก้ไข เปิดบิล
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }
                OrderNew = listOrders[e];

                await transManage.UpdateTranNo(tranWithDetails.tran.TranNo);

                //ckeck ว่าเคยเลือก order แล้วหรือยัง
                checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (checkOder != null)
                {
                    var fragment = new Order_Dialog_OpenOrder();
                    Order_Dialog_OpenOrder dialog = new Order_Dialog_OpenOrder();
                    fragment.Show(Activity.SupportFragmentManager, nameof(Order_Dialog_OpenOrder));
                    if (dialogLoading != null)
                    {
                        dialogLoading.DismissAllowingStateLoss();
                        dialogLoading.Dismiss();
                    }
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
        string SearchOrder;

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

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swipRefresh.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }
        LinearLayout lnBack;
        FrameLayout lnSearchOrder;
        ImageButton btnSearchOrder;
        EditText textSearchOrder;
        ImageButton btnSort;
        SwipeRefreshLayout swipRefresh;
        LinearLayout lnOrder;
        RecyclerView rcvListOrder;
        LinearLayout lnNoOrder;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearchOrder = view.FindViewById<FrameLayout>(Resource.Id.lnSearchOrder);
            btnSearchOrder = view.FindViewById<ImageButton>(Resource.Id.btnSearchOrder);
            textSearchOrder = view.FindViewById<EditText>(Resource.Id.textSearchOrder);
            btnSort = view.FindViewById<ImageButton>(Resource.Id.btnSort);
            swipRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipRefresh);
            lnOrder = view.FindViewById<LinearLayout>(Resource.Id.lnOrder);
            rcvListOrder = view.FindViewById<RecyclerView>(Resource.Id.rcvListOrder);
            lnNoOrder = view.FindViewById<LinearLayout>(Resource.Id.lnNoOrder);

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
    }
    public class ListOrders
    {
        public List<OrderNew> Trans;
        static List<OrderNew> builitem;
        public ListOrders(List<OrderNew> tranOrder)
        {
            if (tranOrder != null)
            {
                builitem = tranOrder;
                this.Trans = builitem;
            }
        }

        public int Count
        {
            get
            {
                return Trans == null ? 0 : Trans.Count;
            }
        }

        public OrderNew this[int i]
        {
            get { return Trans == null ? null : Trans[i]; }
        }

    }

}