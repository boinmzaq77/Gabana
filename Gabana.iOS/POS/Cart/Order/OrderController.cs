using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class OrderController : UIViewController
    {
        public static bool Ismodify ;
        UICollectionView OrderCollection;
        List<Order> listOrder;
        UpdateCustomerController AddCustomerPage;
        UIImageView addCustomer;
        UIImageView emptyView;
        UILabel lbl_empty_cus;
        CustomerManage CustomerManager = new CustomerManage();
        UIView SearchBarView;
        UIButton btnSearch;
        UITextField txtSearch;
        TransManage transManage = new TransManage();
        OrderDataSource orderDataList;
        List<Order> orders = new List<Order>();
        List<OrderNew> ordersnew = new List<OrderNew>();
        List<OrderNew> lstClound = new List<OrderNew>();
        List<Tran> lstDevice = new List<Tran>();
        public static TranWithDetailsLocal tranWithDetails;
        public OrderController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            txtSearch.Text = null;
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("order", "Items"));
            //if (Ismodify)
            //{
            //    listOrder = await GetListCustomer();
            //    ((CustomerDataSource)OrderCollection.DataSource).ReloadData(listOrder);
            //    OrderCollection.ReloadData();
            //}
            showList();
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async override void ViewDidLoad()
        {
            try
            {
                
                //this.NavigationController.NavigationBar.TopItem.Title = "Order";
                base.ViewDidLoad();
                GabanaLoading.SharedInstance.Show(this);
                View.BackgroundColor = UIColor.White;

                #region SearchBarView
                SearchBarView = new UIView();
                SearchBarView.BackgroundColor = UIColor.White;
                SearchBarView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(SearchBarView);

                txtSearch = new UITextField
                {
                    Placeholder = "",
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtSearch.BackgroundColor = UIColor.Clear;
                txtSearch.Font = txtSearch.Font.WithSize(15);
                txtSearch.ReturnKeyType = UIReturnKeyType.Done;
                txtSearch.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    SearchBytxt();
                    return true;
                };
                SearchBarView.AddSubview(txtSearch);

                btnSearch = new UIButton();
                btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSearch.TouchUpInside += (sender, e) =>
                {
                    txtSearch.BecomeFirstResponder();
                };
                SearchBarView.AddSubview(btnSearch);
                #endregion

                #region emptyView
                emptyView = new UIImageView();
                emptyView.Hidden = true;
                emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                emptyView.Image = UIImage.FromBundle("DefaultBillHistory");
                emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(emptyView);

                lbl_empty_cus = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(160, 160, 160),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                lbl_empty_cus.Hidden = true;
                lbl_empty_cus.Lines = 3;
                lbl_empty_cus.Font = lbl_empty_cus.Font.WithSize(16);
                lbl_empty_cus.Text = "คุณยังไม่มี Order ที่บันทึกไว้ \n" +
                    "Order ที่ไม่มีการชำระเกิน 30 วัน ระบบจะลบอัตโนมัติ";
                View.AddSubview(lbl_empty_cus);
                #endregion


                #region CustomerCollection
                UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
                //itemflowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 0);
                itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
                itemflowLayout.MinimumLineSpacing = 0;

                OrderCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
                OrderCollection.BackgroundColor = UIColor.White;
                OrderCollection.ShowsVerticalScrollIndicator = false;
                OrderCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                OrderCollection.RegisterClassForCell(cellType: typeof(OrderCollectionViewCell), reuseIdentifier: "OrderViewCell");
                OrderCollection.RegisterClassForCell(cellType: typeof(OrderHeadCollectionViewCell), reuseIdentifier: "OrderHeadCollectionViewCell");
                View.AddSubview(OrderCollection);

                setupAutoLayout();



                //Loaddata();
                await LoaddataNew();


                showList();

                // ส่ง list ไป
                OrderCollection.DataSource = orderDataList;
                OrderCollectionDelegate orderCollectionDelegate = new OrderCollectionDelegate();
                orderCollectionDelegate.OnItemSelected +=async  (indexPath) => {
                    var order = ordersnew[indexPath.Row];
                    //DataCashing.ModifyTranOrder = false;
                    CultureInfo.CurrentCulture = new CultureInfo("en-US");
                    if (order.Fhead == false)
                    {

                    //ดึงรายการ order มาดูข้อมูล แก้ไข เปิดบิล
                        try
                        {
                            var checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                            if (POSController.tranWithDetails.tran.TranType == 'O')
                            {
                                await Utils.CancelTranOrder(POSController.tranWithDetails);
                                POSController.tranWithDetails = null;
                                POSController.SelectedCustomer = null;
                                DataCaching.posPage.initialData();
                            }

                            if (await GabanaAPI.CheckNetWork())
                            {
                                //Online

                                //เลือกรายการ Order ที่ List จะทำการเรียก Api เพื่อไป Get ข้อมูล Order ลงมา 
                                var date = Utils.ChangeDateTime(order.tranDate);
                                var getTranDetail = await GabanaAPI.GetDataTranOrderDetail(DataCashingAll.SysBranchId, order.tranNo, date);
                                if (getTranDetail != null)
                                {
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
                                    var dateLocal = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                                    tranWithDetails.tran.TranDate = dateLocal;
                                    tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                                    tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);


                                    //ทำการ Insert/Update ข้อมูลเก็บไว้ที่ device 
                                    var getDeviceTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, order.tranNo);
                                    TranWithDetailsLocal TranTemp = new TranWithDetailsLocal();
                                    TranTemp = tranWithDetails;
                                    //ตรวจสอบข้อมูลก่อนจะบันทึกว่ามีสินค้าที่ไม่มีที่เครื่องไหม ถ้ามีให้ remove สินค้านั้น แล้ว calTran ใหม่
                                    List<string> lstSysItemIdStatusD = new List<string>();
                                    lstSysItemIdStatusD = await CheckStatusIteminCart(TranTemp);

                                    if (lstSysItemIdStatusD.Count > 0)
                                    {
                                        CartController.alert = true; 
                                        foreach (var item in lstSysItemIdStatusD)
                                        {
                                            int number;
                                            bool success = int.TryParse(item, out number);
                                            if (success)
                                            {
                                                TranTemp.tranDetailItemWithToppings.RemoveAll(x => x.tranDetailItem.SysItemID == Convert.ToInt32(item));
                                                tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == Convert.ToInt32(item)).ToList().ForEach(x => x.tranDetailItem.Comments = "delete");
                                            }
                                        }
                                        TranTemp = BLTrans.Caltran(TranTemp);

                                    }

                                    //check DataStatus != 'D'
                                    bool InsertUpdateOrder = false;
                                    if (getDeviceTran == null)
                                    {
                                        // ไม่มีที่ Device Insert                           
                                        var insertTran = await transManage.InsertTran(tranWithDetails);
                                    }
                                    else
                                    {
                                        // มีที่ DeviceUpdate                            
                                        var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                                    }
                                }
                                else if (getTranDetail.tran.Comments == "The order has already been applied.")
                                {
                                    //Toast.MakeText(this, "The order has already been applied.", ToastLength.Short).Show();
                                    return;
                                }
                                else
                                {
                                    //Toast.MakeText(this, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                                    return;
                                }
                            }
                            else
                            {
                                //Offline
                                //เมื่อเลือกรายการ Order ที่ต้องการแล้วจะต้อง Set FWaitSending = 0 
                                tranWithDetails = await GetOfflineTranOrderDetail(order.tranNo);
                                tranWithDetails.tran.FWaitSending = 0;
                                tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                                tranWithDetails.tran.LocalDataStatus = 'I';
                                tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                                tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                                tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                                await transManage.UpdateTran(tranWithDetails.tran);
                            }
                            if (tranWithDetails.tran.SysCustomerID != 999)
                            {
                                POSController.SelectedCustomer = await CustomerManager.GetCustomer((int)tranWithDetails.tran.MerchantID , (int)tranWithDetails.tran.SysCustomerID);
                            }

                            POSController.tranWithDetails = tranWithDetails;
                            Utils.SetTitle(this.NavigationController, Utils.TextBundle("payment", "Items"));
                            //var paymentPage = new PaymentController();
                            ////this.NavigationController.ViewControllers[this.NavigationController.ViewControllers.Length - 2].View.RemoveFromSuperview();
                            ////this.NavigationController.ViewControllers[this.NavigationController.ViewControllers.Length - 2].RemoveFromParentViewController();
                            //var count = this.NavigationController.ViewControllers.Length;
                            //for (int i = 2; i < count; i++)
                            //{
                            //    if (i != count - 1)
                            //    {
                            //        this.NavigationController.ViewControllers[2].View.RemoveFromSuperview();
                            //        this.NavigationController.ViewControllers[2].RemoveFromParentViewController();
                            //    }
                            //}
                            POSController.GotoCart = true; 
                            this.NavigationController.PopViewController(false);
                        
                        }
                        catch (Exception ex)
                        {
                            _ = TinyInsights.TrackErrorAsync(ex);
                            Utils.ShowMessage(ex.Message);
                            //Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                        }
                    }

                };
                OrderCollection.Delegate = orderCollectionDelegate;

                #endregion

                //addCustomer = new UIImageView();
                //addCustomer.Image = UIImage.FromBundle("Add");
                //addCustomer.TranslatesAutoresizingMaskIntoConstraints = false;

                //addCustomer.UserInteractionEnabled = true;
                //var tapGesture = new UITapGestureRecognizer(this,
                //        new ObjCRuntime.Selector("AddCus:"))
                //{
                //    NumberOfTapsRequired = 1 // change number as you want 
                //};
                //addCustomer.AddGestureRecognizer(tapGesture);
                //View.AddSubview(addCustomer);

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    await LoaddataNew();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                OrderCollection.AlwaysBounceVertical = true;
                OrderCollection.AddSubview(refreshControl);


                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex )
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        public async Task<List<string>> CheckStatusIteminCart(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                List<Item> AllItem = new List<Item>();
                List<Item> AllItemStatusD = new List<Item>();
                List<string> lstSysItemId = new List<string>(); //tranDetailItem
                List<string> lstSysItemIdTopping = new List<string>(); //tranDetailToppings
                List<string> lstRemoveDummy = new List<string>();
                tranWithDetailsLocal = tranWithDetails;
                //AllItem = await itemManage.GetAll(DataCashingAll.MerchantId);
                //AllItemStatusD = AllItem.Where(x => x.DataStatus == 'D').ToList();

                AllItem = POSController.AllItem;
                AllItemStatusD = POSController.AllItemStatusD;

                //เคสหาสินค้าที่ไม่มีที่เครื่อง
                lstRemoveDummy = tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0  && !string.IsNullOrEmpty(x.tranDetailItem?.SysItemID?.ToString())).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList();
                
                lstSysItemId.AddRange(lstRemoveDummy); 
                foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                {
                    lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                }
                lstSysItemId.AddRange(lstSysItemIdTopping);

                //เคสหาสินค้าที่มีสถานะเป็น 'D' และ Tran.Payment == 0
                if (tranWithDetailsLocal.tranPayments.Count == 0)
                {
                    lstSysItemId.AddRange(tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0 ).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList());
                    foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                    {
                        lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                    }
                    lstSysItemId.AddRange(lstSysItemIdTopping);
                }
                return lstSysItemId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                return new List<string>();
            }
        }
        private async Task LoaddataNew()
        {
            try
            {

                if (await GabanaAPI.CheckNetWork())
                {
                    //Online
                    //order ทั้งหมด ของ Device and Clound
                    //Clound
                    orders = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                
                    //Device
                    //List<Tran>
                    var lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);

                    //Merge listOrder
                    HashSet<string> sentIDs = new HashSet<string>(orders.Select(s => s.tranNo));
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
                    List<OrderNew> listThisDevice = new List<OrderNew>();
                    List<OrderNew> listOtherevice = new List<OrderNew>();

                    // var lstmapOrder = new List<Order>();
                    foreach (var itemmap in orders)
                    {
                        OrderNew mappOrder = new OrderNew()
                        {
                            tranNo = itemmap.tranNo,
                            orderName = itemmap.orderName,
                            tranDate = itemmap.tranDate,
                            deviceNo = (int)itemmap.deviceNo,
                            grandTotal = itemmap.grandTotal,
                            comments = itemmap.comments
                                , Fhead = false
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
                    List<OrderNew> ordersnew = new List<OrderNew>();
                    OrderNew mappOrderhead = new OrderNew()
                    {
                        orderName = "ออเดอร์ของฉัน",
                        Fhead = true
                    };
                    ordersnew.Add(mappOrderhead);
                    ordersnew.AddRange(listThisDevice);
                    OrderNew mappOrderorther = new OrderNew()
                    {
                        orderName = "ออเดอร์อื่นๆ",
                        Fhead = true
                    };
                    ordersnew.Add(mappOrderorther);
                    ordersnew.AddRange(listOtherevice);


                    if (ordersnew != null)
                    {
                        //
                        if (OrderCollection.DataSource == null)
                        {
                            orderDataList = new OrderDataSource(ordersnew, this);
                            OrderCollection.DataSource = orderDataList;
                        }
                        else
                        {
                            ((OrderDataSource)OrderCollection.DataSource).ReloadData(ordersnew, this);
                        }
                        
                        OrderCollection.ReloadData();
                    }
                    this.ordersnew = ordersnew;
                    //showList();
                }
                else
                {
                    //Offline
                    //order ทั้งหมด ของ Device
                    var lstOrder = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    if (lstOrder != null)
                    {
                        List<OrderNew> ordersnew = new List<OrderNew>();
                        foreach (var itemmap in lstOrder)
                        {
                            OrderNew mappOrder = new OrderNew()
                            {
                                tranNo = itemmap.TranNo,
                                orderName = itemmap.OrderName,
                                tranDate = itemmap.TranDate,
                                deviceNo = (int)itemmap.DeviceNo,
                                grandTotal = itemmap.GrandTotal,
                                comments = itemmap.Comments
                            };
                            ordersnew.Add(mappOrder);
                        }
                        orderDataList = new OrderDataSource(ordersnew,this);
                        OrderCollection.ReloadData();
                        this.ordersnew = ordersnew;
                    }
                
                }
            }
            catch (Exception ex)
            {

            }
        }

        //    private async void Loaddata()
        //{
        //    TransManage transManage = new TransManage();
        //    List<OrderNew> orders = new List<OrderNew>();
        //    //Online
        //    if (await GabanaAPI.CheckNetWork())
        //    {
        //        //Cloud
        //        List<Order> ordercloud = new List<Order>();
        //        ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
        //        if (ordercloud == null)
        //        {
        //            ordercloud = await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
        //        }

        //        //mapping Order to OrderNew
        //        var config = new MapperConfiguration(cfg =>
        //        {
        //            cfg.CreateMap<Gabana3.JAM.Trans.Order, Model.OrderNew>();
        //        });

        //        var Imapper = config.CreateMapper();
        //        lstClound = Imapper.Map<List<Gabana3.JAM.Trans.Order>, List<Model.OrderNew>>(ordercloud);

        //        //Device
        //        //List<Tran>
        //        lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
        //        if (lstDevice == null)
        //        {
        //            lstDevice = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
        //        }

        //        if (lstClound != null & lstDevice != null)
        //        {
        //            //Merge listOrder
        //            HashSet<string> sentIDs = new HashSet<string>(lstClound.Select(s => s.tranNo));
        //            var results = lstDevice.Where(m => sentIDs.Contains(m.TranNo)).ToList();
        //            if (results.Count > 0)
        //            {
        //                foreach (var item in results)
        //                {
        //                    var removelstDevice = lstDevice.FindIndex(x => x.TranNo == item.TranNo);
        //                    if (removelstDevice != -1)
        //                    {
        //                        lstDevice.RemoveAt(removelstDevice);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //dialogLoading.Dismiss();
        //            //Toast.MakeText(this, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
        //            return;
        //        }

        //        List<OrderNew> listThisDevice = new List<OrderNew>();
        //        List<OrderNew> listOtherevice = new List<OrderNew>();


        //        foreach (var itemmap in lstClound)
        //        {
        //            if (DataCashingAll.Device.DeviceNo == itemmap.deviceNo)
        //            {
        //                itemmap.TypeOfflineOrOnline = 'O';
        //                listThisDevice.Add(itemmap);
        //            }
        //            else
        //            {
        //                itemmap.TypeOfflineOrOnline = 'O';
        //                listOtherevice.Add(itemmap);
        //            }
        //        }

        //        foreach (var itemmap in lstDevice)
        //        {
        //            OrderNew mappOrder = new OrderNew()
        //            {
        //                tranNo = itemmap.TranNo,
        //                orderName = itemmap.OrderName,
        //                tranDate = itemmap.TranDate,
        //                deviceNo = (int)itemmap.DeviceNo,
        //                grandTotal = itemmap.GrandTotal,
        //                comments = itemmap.Comments,
        //                TypeOfflineOrOnline = 'O'
        //            };

        //            if (DataCashingAll.Device.DeviceNo == mappOrder.deviceNo)
        //            {
        //                listThisDevice.Add(mappOrder);
        //            }
        //            else
        //            {
        //                listOtherevice.Add(mappOrder);
        //            }
        //        }

        //        List<OrderNew> listShow = new List<OrderNew>();
        //        foreach (var item in listThisDevice)
        //        {
        //            listShow.Add(item);
        //        }

        //        foreach (var item in listOtherevice)
        //        {
        //            listShow.Add(item);
        //        }

        //        if (listShow != null)
        //        {
        //            //if (sort)
        //            //{
        //            //    listShow = listShow.OrderByDescending(x => x.tranDate).ToList();
        //            //}
        //            //listOrders = new ListOrders(listShow);
        //        }
        //    }
        //    else
        //    {
        //        //Offline
        //        //order ทั้งหมด ของ Device
        //        var lstOrder = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
        //        if (lstOrder != null)
        //        {
        //            foreach (var item in lstOrder)
        //            {
        //                OrderNew mappOrder = new OrderNew()
        //                {
        //                    tranNo = item.TranNo,
        //                    orderName = item.OrderName,
        //                    tranDate = item.TranDate,
        //                    deviceNo = (int)item.DeviceNo,
        //                    grandTotal = item.GrandTotal,
        //                    comments = item.Comments,
        //                    TypeOfflineOrOnline = 'F'
        //                };
        //                orders.Add(mappOrder);
        //            }
        //            //if (sort)
        //            //{
        //            //    orders = orders.OrderByDescending(x => x.tranDate).ToList();
        //            //}

        //            //listOrders = new ListOrders(orders);
        //        }
        //    }
        //    OrderCollection.DataSource = orderDataList;
        //}

        async Task<TranWithDetailsLocal> GetOfflineTranOrderDetail(string tranNo)
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
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
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return new TranWithDetailsLocal();
            }
        }
        [Export("AddCus:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            AddCustomerPage = new UpdateCustomerController();
            this.NavigationController.PushViewController(AddCustomerPage, false);
        }
        void showList()
        {
            if (ordersnew == null || ordersnew.Count == 0)
            {
              //  SearchBarView.Hidden = true;
                OrderCollection.Hidden = true;
                emptyView.Hidden = false;
                lbl_empty_cus.Hidden = false;
            }
            else
            {
              //  SearchBarView.Hidden = false;
                OrderCollection.Hidden = false;
                emptyView.Hidden = true;
                lbl_empty_cus.Hidden = true;
            }
        }
        async void SearchBytxt()
        {
            //GetCustomerSearch
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    //listOrder = await GetListCustomer();
                }
                //listOrder = await  CustomerManager.GetCustomerSearch((int)MainController.merchantlocal.MerchantID, txtSearch.Text);
                //if (listOrder == null)
                //{
                //    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                //}
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
            }
        }
        
        void setupAutoLayout()
        {
            #region SearchBar
            SearchBarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchBarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchBarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchBarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            #endregion

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 38).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_cus.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_cus.HeightAnchor.ConstraintEqualTo(63).Active = true;
            lbl_empty_cus.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_cus.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            OrderCollection.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            OrderCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            OrderCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            OrderCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;


            //addCustomer.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            //addCustomer.WidthAnchor.ConstraintEqualTo(45).Active = true;
            //addCustomer.HeightAnchor.ConstraintEqualTo(45).Active = true;
            //addCustomer.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class ChooseCustomer : ORM.Master.Customer
        {
            public bool Choose { get; set; }
        }
        public class ListCustomer
        {
            public List<ORM.MerchantDB.Customer> customers;
            static List<ORM.MerchantDB.Customer> builitem;
            public ListCustomer(List<ORM.MerchantDB.Customer> lstcustomer)
            {
                builitem = lstcustomer;
                this.customers = builitem;

            }
            public int Count
            {
                get
                {
                    return customers == null ? 0 : customers.Count;
                }
            }
            public ORM.MerchantDB.Customer this[int i]
            {
                get { return customers == null ? null : customers[i]; }
            }
        }

    }
}