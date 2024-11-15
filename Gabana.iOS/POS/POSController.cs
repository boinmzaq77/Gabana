using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Gabana.ShareSource;
using System.Linq;
using Gabana.POS;
using System.Drawing;
using System.Globalization;
using Xamarin.Essentials;
using Gabana.iOS.ITEMS;
using AVFoundation;
using TinyInsightsLib;
using LinqToDB.SqlQuery;

namespace Gabana.iOS
{
    public partial class POSController : UIViewController
    {
        public DeviceTranRunningNoManage DeviceTranRunningNoManage = new DeviceTranRunningNoManage();
        UILongPressGestureRecognizer lpgr;
        public static string remark = null;
        string usernamelogin;
        public static int countItems = 0;
        public static decimal sumItem = 0;
        public static List<Category> MenuPos;
        bool favoriteMenu;
        int menuSelect = 0;
        public int fillter;
        public static bool scan , newitem;
        public static string itemcode;
        public static long idnewitem;
        private int SelectCategory;
        public static List<Item> Items, ItemsLIST;
        private bool checkroll;
        public static UICollectionView menuPOSCollectionview;
        public static string txtQuantity = "1";
        UICollectionView itemPOSCollectionview, itemPOSCollectionviewList;
        UIView searchView, bottomView , viewcell;
        MenuPosDataSource menuPosData;
        public static ItemPosDataSource itemPosData;
        UIButton btnSearch, btnSearchQr, btnDummy, btnListView, btnQuantity, btnOrder;
        UIButton btnSummatyItems;
        UIImageView iconImg;
        UILabel lblbtnSummatyCost, lblbtnSummatyItem, lblmenu2;
        public static int POScategory = 0;
      //  UIBarButtonItem btnBack;
        public static int flag = 0;
        public static int Quantity;
        string SearchItem="";
        UITextField txtSearch;
        public static int totlaItems;
        public static string totalResult;
        public static TranWithDetailsLocal tranWithDetails;
        UICollectionViewFlowLayout itemflowLayout;
        ItemPOSCollectionDelegate itemposCollectionDelegate;
        UICollectionViewFlowLayout itemflowLayoutList;
        static ItemPosDataSourceList itemPosDataList;
        ItemPOSListCollectionDelegate itemposListCollectionDelegate;
        static POSScanBarcodeController posScanPage = null;
        static POSDummyController posDummyPage;
        static  POSQuantityController posQuantityPage = null;
        static  POSDetailItemController POSDetailPage = null;
        static ItemManage itemmanager = new ItemManage();
        static CategoryManage catagory;
        static TransManage transManage;
        public static int Select;
        static CartController cartPage =null;
        public static Customer SelectedCustomer=null;
        POSCustomerController selectCustomerPage = null;
        public static bool ModifyTranOrder;
        TranDetailItemManage tranDetailItemManage = new TranDetailItemManage();
        TranPaymentManage tranPaymentManage = new TranPaymentManage();
        TranDetailItemToppingManage toppingManage = new TranDetailItemToppingManage();
        TranTradDiscountManage discountManage = new TranTradDiscountManage();
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        public int DetailNo = 0;
        private UINavigationItem naviitem;
        internal static bool GotoCart;
        private string LoginType;
        public static List<Item> AllItem = new List<Item>();
        public static List<Item> AllItemStatusD = new List<Item>();
        //public static bool checknet = true;
        public POSController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                //checknet = await GabanaAPI.CheckNetWork();
                _ = TinyInsights.TrackEventAsync("ViewWillAppear");
                Utils.SetTitle(this.NavigationController,"POS");
                //this.NavigationController.PopToViewController(DataCaching.posPage, false);
                var xxx = this.NavigationController;
                naviitem = this.NavigationItem;
                naviitem.BackBarButtonItem.Title = "POS";

                AllItem = await itemmanager.GetAll(DataCashingAll.MerchantId);
                AllItemStatusD = AllItem.Where(x => x.DataStatus == 'D').ToList();

                if (tranWithDetails is null)
                {
                    await initialDatanew();
                }
                if (SelectedCustomer != null)
                {
                    tranWithDetails.tran.CustomerName = SelectedCustomer.CustomerName;
                    tranWithDetails.tran.SysCustomerID = SelectedCustomer.SysCustomerID;

                }
                
                if (tranWithDetails !=null)
                {
                    if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                    {
                        lblbtnSummatyCost.TextColor = UIColor.FromRGB(0, 149, 218);
                        lblbtnSummatyItem.TextColor = UIColor.FromRGB(0, 149, 218);
                        lblbtnSummatyCost.Text = Utils.DisplayDecimal(0)+" "+ CURRENCYSYMBOLS;
                        lblbtnSummatyItem.Text =  Utils.TextBundle("noitem", "No Item");
                        btnSummatyItems.Enabled = false;
                        btnSummatyItems.BackgroundColor = UIColor.White;
                    }
                    else
                    {
                        lblbtnSummatyCost.TextColor = UIColor.White;
                        lblbtnSummatyItem.TextColor = UIColor.White;
                        lblbtnSummatyCost.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.SubAmount)) + CURRENCYSYMBOLS;
                        lblbtnSummatyItem.Text = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0") + " "+ Utils.TextBundle("item", "Items");
                        btnSummatyItems.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                        btnSummatyItems.Enabled = true;
                    }

                    tranWithDetails.tran.TaxRate = UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE);
                    tranWithDetails.tran.TranTaxType = char.Parse(DataCashingAll.setmerchantConfig.TAXTYPE);
                    tranWithDetails.tran.FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                    //tranWithDetails.tran.ServiceCharge = UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE);
                    tranWithDetails.tran.TranTaxType = char.Parse(DataCashingAll.setmerchantConfig.TAXTYPE);
                    BLTrans.Caltran(tranWithDetails);
                }

                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.btnQuantity.SetTitle("x" + Quantity.ToString("F0"), UIControlState.Normal);

                //Items = await FilterIteMPos(0);
                ////ItemsLIST = await FilterIteMPosLIST(0);
                //((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                //((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                //itemPOSCollectionview.ReloadData();
                //itemPOSCollectionviewList.ReloadData();

                if (SelectedCustomer != null)
                {
                    UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                    uIImageView.Image = UIImage.FromBundle("CustB");
                    UIButton btn = new UIButton();
                    //btn.SetImage(UIImage.FromBundle("Cust"), default);
                    btn.ImageView.BackgroundColor = UIColor.Black;
                    btn.Frame = new CGRect(0, 0, 200, 50);
                    btn.Layer.CornerRadius = 5f;
                    btn.Layer.BorderWidth = 0.5f;
                    btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    //btn.BackgroundColor = UIColor.Red;
                    UILabel lab = new UILabel();
                    lab.TextColor = UIColor.FromRGB(0, 149, 218);
                    lab.Text = SelectedCustomer.CustomerName;
                    lab.TextAlignment = UITextAlignment.Right;
                    lab.TranslatesAutoresizingMaskIntoConstraints = false;
                    uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                    btn.AddSubview(uIImageView);
                    btn.AddSubview(lab);

                    lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                    lab.HeightAnchor.ConstraintEqualTo(50).Active = true;
                    lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                    uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                    uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                    uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;

                    UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                    btn.TouchUpInside += (sender, e) => {
                        // open select customer page
                        //if (selectCustomerPage == null)
                        //{
                            selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
                    };
                    naviitem.RightBarButtonItem = selectCustomer;
                }
                else
                {
                    UIBarButtonItem selectCustomer = new UIBarButtonItem();
                    selectCustomer.Image = UIImage.FromBundle("Cust");
                    selectCustomer.Clicked += (sender, e) => {
                        // open select customer page
                        //if (selectCustomerPage == null)
                        //{
                            selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
                    };
                    //naviitem.RightBarButtonItem = selectCustomer;
                    naviitem.SetRightBarButtonItem(selectCustomer, false);
                }
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
                if (scan)
                {
                    scan = false;
                    txtSearch.Text = itemcode; 
                    SearchBytxt();
                    btnSearch.SetBackgroundImage(UIImage.FromBundle("DelTxt"), UIControlState.Normal);
                }
                if (newitem)
                {
                    newitem = false;
                    SearchBytxt();
                    var index = Items.FindIndex(x => x.SysItemID == idnewitem);
                    if (index >= 0)
                    {
                        itemPOSCollectionview.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                        itemPOSCollectionviewList.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.CenteredVertically, false);

                    }
                }
                if (GotoCart)
                {
                    GotoCart = false;
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("cart", "Cart"));
                    var cartPage = new CartController(false);
                    this.NavigationController.PushViewController(cartPage, false);
                }
                if (flag == 0 )
                {
                    btnListView.SetImage(UIImage.FromBundle("ViewList"), UIControlState.Normal);
                    //itemPOSCollectionview.Delegate = itemposCollectionDelegate;
                    //itemPOSCollectionview.DataSource = new ItemPosDataSource(Items);
                    ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                    itemPOSCollectionview.ReloadData();
                    itemPOSCollectionview.Hidden = false;
                    itemPOSCollectionviewList.Hidden = true;
                }
                else
                {
                    btnListView.SetImage(UIImage.FromBundle("ViewGroup"), UIControlState.Normal);
                    //itemPOSCollectionviewList.Delegate = itemposListCollectionDelegate;
                    //itemPOSCollectionviewList.DataSource = new ItemPosDataSourceList(ItemsLIST);
                    ((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                    itemPOSCollectionviewList.ReloadData();
                    itemPOSCollectionview.Hidden = true;
                    itemPOSCollectionviewList.Hidden = false;
                }


            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }

        public override async void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (this.IsMovingFromParentViewController)
            {
                await Cancel();
            }

        }
        async Task Cancel()
        {
            try
            {
                _ = TinyInsights.TrackEventAsync("Cancel");
                if (tranWithDetails != null && tranWithDetails.tran.TranType == 'O')
                {
                    await Utils.CancelTranOrder(tranWithDetails);
                    POSController.tranWithDetails = null;
                    POSController.SelectedCustomer = null;
                    DataCaching.posPage.initialData();
                    //DataCashing.isCurrentOrder = false;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
            
            
        }
        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            //Items = await FilterIteMPos(0);
            //ItemsLIST = await FilterIteMPosLIST(0);
            //((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
            //((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
             
        }
        
        public async override void ViewDidLoad()
        {
            try
            {

                
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("menupos", "POS"));
                usernamelogin = Preferences.Get("User", "");
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                base.ViewDidLoad();
                LoginType = Preferences.Get("LoginType", "");
                
                transManage = new TransManage();
                var checkOder = await transManage.GetTranOrderBeforeClose(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (checkOder == null)
                {
                    initialData();
                }
                else
                {
                    getTranOrderDetail(checkOder);
                }
                Select = 0;
                DetailNo = tranWithDetails.tranDetailItemWithToppings.Count;
                Quantity = 1;

                View.BackgroundColor = UIColor.White;

                //btn.

                itemmanager = new ItemManage();

                #region POSController
                #region MenuPosCollectionView
                var flowLayout = new POS.MenuBarCollectionViewLayout();
                flowLayout.SizeForItem += (collectionView, layout, indexPath) =>
                {
                    NSString nSString = new NSString((menuPOSCollectionview.DataSource as MenuPosDataSource).GetItem(indexPath.Row));
                    UIFont font = UIFont.SystemFontOfSize(13);
                    CGSize cGSize = nSString.StringSize(font);

                    return new CGSize(cGSize.Width + 40, 38);
                };
                //checknet = await GabanaAPI.CheckNetWork();
                menuPOSCollectionview = new UICollectionView(frame: View.Frame, layout: flowLayout);
                menuPOSCollectionview.BackgroundColor = UIColor.White;
                menuPOSCollectionview.ShowsHorizontalScrollIndicator = false;
                menuPOSCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
                menuPOSCollectionview.RegisterClassForCell(cellType: typeof(POSMenuCollectionViewCell), reuseIdentifier: "menuPosCell");
                MenuPos = await GetMenuPos();
                menuPosData = new MenuPosDataSource(MenuPos);
                menuPOSCollectionview.DataSource = menuPosData;
                POSMenuCollectionDelegate posCollectionDelegate = new POSMenuCollectionDelegate();
                posCollectionDelegate.OnItemSelected += async (indexPath) => {
                    // do somthing
                    try
                    {

                    
                        Select = indexPath.Row;
                    
                        ItemManage itemManage = new ItemManage();
                        var menu = MenuPos[(int)indexPath.Row];
                        SelectCategory = (int)menu.SysCategoryID;
                        Items = await itemManage.SearchItembyCategory(DataCashingAll.MerchantId, (int)menu.SysCategoryID, "");
                        var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
                        if (check)
                        {
                            Items.Add(new Item() { SysItemID = -1 });
                        }
                    
                        txtSearch.Text = "";
                        btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                        // if ((int)indexPath.Row != 0 && (int)indexPath.Row!= -1 && (int)indexPath.Row!=-2)
                        // {
                        //     var category = MenuPos.Where(x => x.Name == MenuPos[(int)indexPath.Row].Name).FirstOrDefault();
                        //     menuSelect = (int)category.SysCategoryID;
                        //     Items = await FilterIteMPos(menuSelect);
                        //     ItemsLIST = await FilterIteMPosLIST(menuSelect);
                        // }
                        //else
                        // {
                        //     menuSelect = (int)MenuPos[indexPath.Row].SysCategoryID;
                        //     Items = await FilterIteMPos(menuSelect);
                        //     ItemsLIST = await FilterIteMPosLIST(menuSelect);
                        // }
                        ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                        ((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                        itemPOSCollectionview.ReloadData();
                        itemPOSCollectionviewList.ReloadData();
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                    }
                };
                menuPOSCollectionview.Delegate = posCollectionDelegate;
                View.AddSubview(menuPOSCollectionview);
                #endregion
                
                #region ItemPOSGridCollectionview

                itemflowLayout = new UICollectionViewFlowLayout();
                itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayout.SectionInset = new UIEdgeInsets(top: 5, left: 5, bottom: 5, right: 5);
                itemflowLayout.ItemSize = new CoreGraphics.CGSize(width: ((View.Frame.Width-10)/3)-10, height: (((View.Frame.Width - 10) / 3) - 10) *1.25);

                itemPOSCollectionview = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
                itemPOSCollectionview.Hidden = false;
                itemPOSCollectionview.BackgroundColor = UIColor.White;
                itemPOSCollectionview.ShowsVerticalScrollIndicator = false;
                itemPOSCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;

                itemPOSCollectionview.RegisterClassForCell(cellType: typeof(ItemPOSCollectionViewCell), reuseIdentifier: "itemPosCell");
                //var item = await FilterIteMPos(0);
                Items = await itemmanager.SearchItembyCategory(DataCashingAll.MerchantId, 0, "");
                //Items = item;
                
                checkroll = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                if (checkroll) 
                {
                    Items.Add(new Item() { SysItemID = -1 });
                }
                itemPosData = new ItemPosDataSource(Items);
                itemPOSCollectionview.DataSource = itemPosData;
                itemPOSCollectionview.BackgroundColor = UIColor.Clear ;
                itemposCollectionDelegate = new ItemPOSCollectionDelegate();
                itemposCollectionDelegate.OnItemSelected += async (indexPath) => {
                    try
                    {

                   
                        DetailNo++;
                        // check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");

                        if ((int)indexPath.Row != ((Items.Count) - 1) || !checkroll)
                        {
                            UICollectionViewCell cell = new UICollectionViewCell();
                            cell = itemPOSCollectionview.CellForItem(indexPath) as ItemPOSCollectionViewCell;
                            //var cell = itemPOSCollectionview.DequeueReusableCell("itemPosCell", indexPath) as ItemPOSCollectionViewCell;
                            //cell.Name += "s" + cell.Name;
                            if (tranWithDetails is null)
                            {
                                initialData();
                            }
                            var itemchoose = Items[indexPath.Row];
                            var trandetail = new TranDetailItemWithTopping()
                            {
                                tranDetailItem = new TranDetailItemNew()
                                {
                                    SysItemID = itemchoose.SysItemID,
                                    MerchantID = tranWithDetails.tran.MerchantID,
                                    SysBranchID = tranWithDetails.tran.SysBranchID,
                                    ItemName = itemchoose.ItemName,
                                    SaleItemType = itemchoose.SaleItemType,
                                    TranNo = tranWithDetails.tran.TranNo,
                                    FProcess = 1,
                                    TaxType = itemchoose.TaxType,
                                    Quantity = Quantity,
                                    Price = itemchoose.Price,
                                    ItemPrice = itemchoose.Price,
                                    Discount = 0,
                                    EstimateCost = itemchoose.EstimateCost,
                                    DetailNo = DetailNo
                                },
                                tranDetailItemToppings = new List<TranDetailItemTopping>()
                            };

                            if (Items[indexPath.Row].FDisplayOption == 1 )
                            {
                                var cartPage = new OptionController(trandetail, false);
                                this.NavigationController.PushViewController(cartPage, false);
                            }
                            else
                            {
                                tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, trandetail);
                                tranWithDetails = BLTrans.Caltran(tranWithDetails);
                                CreateViewCell(cell, Items[indexPath.Row],View);
                                //btnSearch.SetImage(iamg, UIControlState.Normal);
                                CartController.Ismodify = true;

                                lblbtnSummatyCost.TextColor = UIColor.White;
                                lblbtnSummatyItem.TextColor = UIColor.White;
                                lblbtnSummatyCost.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x=>x.tranDetailItem.SubAmount)) + CURRENCYSYMBOLS;
                                lblbtnSummatyItem.Text = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0") + " "+ Utils.TextBundle("item", "Items");
                                btnSummatyItems.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                                btnSummatyItems.Enabled = true;
                                Quantity = 1;
                                this.btnQuantity.SetTitle("x" + Quantity.ToString(), UIControlState.Normal);
                                txtQuantity = "1";
                                itemPOSCollectionview.ReloadData();
                            }
                        }
                        else // add button
                        {
                            if (SelectCategory == 0)
                            {
                                AddItemControllerScroll additem = new AddItemControllerScroll();
                                //additem.SelectCategory = SelectCategory;
                                this.NavigationController.PushViewController(additem, false);
                            }
                            else if (SelectCategory == -2)
                            {
                                AddItemControllerScroll additem = new AddItemControllerScroll(true);
                                //additem.SelectCategory = SelectCategory;
                                this.NavigationController.PushViewController(additem, false);
                            }
                            else if (SelectCategory == -3)
                            {
                                ItemsAddToppingController itemsAddTopping = new ItemsAddToppingController();
                                this.NavigationController.PushViewController(itemsAddTopping, false);
                            }
                            else
                            {
                                AddItemControllerScroll additem = new AddItemControllerScroll();
                                additem.SelectCategory = SelectCategory;
                                this.NavigationController.PushViewController(additem, false);
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                    }
                };
                itemPOSCollectionview.Delegate = itemposCollectionDelegate;

                //lpgr = new UILongPressGestureRecognizer(action: handleLongPress)
                //{
                //    MinimumPressDuration = 0.2,
                //    DelaysTouchesBegan = true
                //};
                //itemPOSCollectionview.AddGestureRecognizer(lpgr);
                View.AddSubview(itemPOSCollectionview);

                #endregion
                #region itemPOSCollectionviewList
                itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.SectionInset = new UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0);
                //itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width - 20), height: (View.Frame.Height - 100) / 8);
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width - 20), height: 80);
                itemflowLayoutList.MinimumLineSpacing = 1f;
                itemflowLayoutList.MinimumInteritemSpacing = 1f;

                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

                itemPOSCollectionviewList = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                //ItemsLIST = await FilterIteMPosLIST(0);
                itemPOSCollectionviewList.BackgroundColor = UIColor.White;
                itemPOSCollectionviewList.ShowsVerticalScrollIndicator = false;
                itemPOSCollectionviewList.TranslatesAutoresizingMaskIntoConstraints = false;
                itemPOSCollectionviewList.Hidden = true;
                itemPOSCollectionviewList.RegisterClassForCell(cellType: typeof(ItemPOSCollectionViewCellList), reuseIdentifier: "itemPosCellList");
                itemPOSCollectionviewList.RegisterClassForCell(cellType: typeof(ItemPOSCollectionViewCellListAdd), reuseIdentifier: "ItemPOSCollectionViewCellListAdd");
                itemPosDataList = new ItemPosDataSourceList(Items);
                itemPOSCollectionviewList.DataSource = itemPosDataList;
                itemposListCollectionDelegate = new ItemPOSListCollectionDelegate();
                itemposListCollectionDelegate.OnItemSelected += async (indexPath) => {

                    try
                    {

                    
                        DetailNo++;
                        if ((int)indexPath.Row != ((Items.Count) - 1))
                        {
                            UICollectionViewCell cell = new UICollectionViewCell();
                            cell = itemPOSCollectionviewList.CellForItem(indexPath) as ItemPOSCollectionViewCellList;
                            var itemchoose = Items[indexPath.Row];
                            var trandetail = new TranDetailItemWithTopping()
                            {
                                tranDetailItem = new TranDetailItemNew()
                                {
                                    SysItemID = itemchoose.SysItemID,
                                    MerchantID = DataCashingAll.MerchantId,
                                    SysBranchID = DataCashingAll.SysBranchId,
                                    ItemName = itemchoose.ItemName,
                                    SaleItemType = itemchoose.SaleItemType,
                                    TranNo = tranWithDetails.tran.TranNo,
                                    FProcess = 1,
                                    TaxType = itemchoose.TaxType,
                                    Quantity = Quantity,
                                    Price = itemchoose.Price,
                                    ItemPrice = itemchoose.Price,
                                    Discount = 0,
                                    EstimateCost = itemchoose.EstimateCost,
                                    DetailNo = DetailNo
                                },
                                tranDetailItemToppings = new List<TranDetailItemTopping>()
                            };

                            if (Items[indexPath.Row].FDisplayOption == 1)
                            {
                                var cartPage = new OptionController(trandetail, false);
                                this.NavigationController.PushViewController(cartPage, false);
                            }
                            else
                            {

                                tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, trandetail);
                                tranWithDetails = BLTrans.Caltran(tranWithDetails);
                                CreateViewCell(cell, Items[indexPath.Row],View);
                                CartController.Ismodify = true;


                                lblbtnSummatyCost.TextColor = UIColor.White;
                                lblbtnSummatyItem.TextColor = UIColor.White;
                                lblbtnSummatyCost.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.SubAmount)) + CURRENCYSYMBOLS;
                                lblbtnSummatyItem.Text = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("N0") + " " +  Utils.TextBundle("item", "Items");
                                btnSummatyItems.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                                btnSummatyItems.Enabled = true;
                                Quantity = 1;
                                this.btnQuantity.SetTitle("x" + Quantity.ToString(), UIControlState.Normal);
                            }
                        }
                        else // add button
                        {
                            AddItemControllerScroll additem = new AddItemControllerScroll();
                            this.NavigationController.PushViewController(additem, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                    }
                };
                itemPOSCollectionviewList.Delegate = itemposListCollectionDelegate;
                View.AddSubview(itemPOSCollectionviewList);
                #endregion
                #endregion

                #region searchView
                searchView = new UIView();
                searchView.BackgroundColor = UIColor.White;
                searchView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(searchView);

                btnSearch = new UIButton();
                btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSearch.TouchUpInside += (sender, e) => {
                    //_ = TinyInsights.TrackEventAsync("search");
                    // search function
                    if (btnSearch.CurrentBackgroundImage == UIImage.FromBundle("DelTxt"))
                    {
                        txtSearch.Text = "";
                        SearchBytxt();
                        btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    }
                    else
                    {
                        txtSearch.BecomeFirstResponder();
                    }
                    
                };
                searchView.AddSubview(btnSearch);

                txtSearch = new UITextField
                {
                    TextAlignment = UITextAlignment.Left,
                    BorderStyle = UITextBorderStyle.None,
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(0,149,218),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                txtSearch.ReturnKeyType = UIReturnKeyType.Done;
                txtSearch.AddTarget((sender, e) =>
                {
                    //_ = TinyInsights.TrackEventAsync("search");
                    if (txtSearch.Text == "")
                    {
                        btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    }
                    else
                    {
                        btnSearch.SetBackgroundImage(UIImage.FromBundle("DelTxt"), UIControlState.Normal);

                    }
                    
                }, UIControlEvent.EditingChanged);
                txtSearch.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);

                    SearchBytxt();
                    return true;
                };
                searchView.AddSubview(txtSearch);

                btnSearchQr = new UIButton();
                btnSearchQr.SetBackgroundImage(UIImage.FromBundle("ScanItem"), UIControlState.Normal);
                btnSearchQr.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSearchQr.TouchUpInside += (sender, e) => {
                    //await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                    //_ = TinyInsights.TrackEventAsync("searchqr");
                    if (Utils.Checkpermisstion())
                    {
                        if (tranWithDetails is null)
                        {
                            initialData();
                        }
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("cart", "Cart") );
                        var cartPage = new CartController(true);
                        this.NavigationController.PushViewController(cartPage, false);
                    }
                };
                searchView.AddSubview(btnSearchQr);

                btnOrder = new UIButton();
                btnOrder.SetBackgroundImage(UIImage.FromBundle("POSOrders"), UIControlState.Normal);
                btnOrder.TranslatesAutoresizingMaskIntoConstraints = false;
                btnOrder.TouchUpInside += (sender, e) => {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("order", "Order"));
                    var orderpage = new OrderController();
                    this.NavigationController.PushViewController(orderpage, false);
                };
                searchView.AddSubview(btnOrder);

                btnDummy = new UIButton();
                btnDummy.SetBackgroundImage(UIImage.FromBundle("Dummy"), UIControlState.Normal);
                btnDummy.TranslatesAutoresizingMaskIntoConstraints = false;
                btnDummy.TouchUpInside += (sender, e) => {
                    // add dummy
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("dummy", "Dummy"));
                    if (posScanPage == null)
                    {
                        posDummyPage = new POSDummyController();
                    }
                    this.NavigationController.PushViewController(posDummyPage, false);
                };
                searchView.AddSubview(btnDummy);

                btnListView = new UIButton();
                btnListView.SetImage(UIImage.FromBundle("ViewList"), UIControlState.Normal);
                btnListView.TranslatesAutoresizingMaskIntoConstraints = false;
                btnListView.TouchUpInside += (sender, e) =>
                {
                    this.NavigationController?.LoadView();
                    btnListView.SetImage(null, UIControlState.Disabled);
                        // flag = 0 is grid view but flag = 1 is list view
                        if (flag == 0)
                    {
                        btnListView.SetImage(UIImage.FromBundle("ViewGroup"), UIControlState.Normal);
                        //itemPOSCollectionviewList.Delegate = itemposListCollectionDelegate;
                        //itemPOSCollectionviewList.DataSource = new ItemPosDataSourceList(ItemsLIST);
                        ((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                        itemPOSCollectionviewList.ReloadData();
                        itemPOSCollectionview.Hidden = true;
                        itemPOSCollectionviewList.Hidden = false;
                        flag = 1;
                    }
                    else
                    {
                        btnListView.SetImage(UIImage.FromBundle("ViewList"), UIControlState.Normal);
                        //itemPOSCollectionview.Delegate = itemposCollectionDelegate;
                        //itemPOSCollectionview.DataSource = new ItemPosDataSource(Items);
                        ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                        itemPOSCollectionview.ReloadData();
                        itemPOSCollectionview.Hidden = false;
                        itemPOSCollectionviewList.Hidden = true;
                        flag = 0;

                    }
                };
                searchView.AddSubview(btnListView);



                btnQuantity = new UIButton();
                btnQuantity.SetBackgroundImage(UIImage.FromBundle("Quantity1"), UIControlState.Normal);
                btnQuantity.TranslatesAutoresizingMaskIntoConstraints = false;
                btnQuantity.TouchUpInside += (sender, e) =>
                {
                    if (posQuantityPage == null)
                    {
                        posQuantityPage = new POSQuantityController();
                    }
                    this.NavigationController.PushViewController(posQuantityPage, false);
                };
                btnQuantity.SetTitle("x" + Quantity.ToString(), UIControlState.Normal);
                btnQuantity.TitleLabel.Font = btnQuantity.TitleLabel.Font.WithSize(9);
                btnQuantity.TitleEdgeInsets = new UIEdgeInsets(top: 0, left: 2, bottom: 0, right: 0);
                btnQuantity.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                searchView.AddSubview(btnQuantity);

                #region BottomView
                bottomView = new UIView();
                bottomView.BackgroundColor = UIColor.White;
                bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(bottomView);


                btnSummatyItems = new UIButton();
                btnSummatyItems.Layer.CornerRadius = 5f;
                btnSummatyItems.Layer.BorderWidth = 0.5f;
                btnSummatyItems.Layer.BorderColor = UIColor.FromRGB(0,149,218).CGColor;
                btnSummatyItems.BackgroundColor = UIColor.White;
                btnSummatyItems.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSummatyItems.TouchUpInside += (sender, e) => {

                    btnSummatyItems_Click();
                };
                View.AddSubview(btnSummatyItems);

                lblbtnSummatyCost = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                lblbtnSummatyCost.TextColor = UIColor.FromRGB(0, 149, 218);
                lblbtnSummatyCost.Text = "0.00 ฿";
                lblbtnSummatyCost.Font = lblbtnSummatyCost.Font.WithSize(20);
                btnSummatyItems.AddSubview(lblbtnSummatyCost);

                lblbtnSummatyItem = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                lblbtnSummatyItem.TextColor = UIColor.FromRGB(0, 149, 218);
                lblbtnSummatyItem.Font = lblbtnSummatyItem.Font.WithSize(10);
                lblbtnSummatyItem.Text = "No Item";
                btnSummatyItems.AddSubview(lblbtnSummatyItem);

                #endregion

                if (countItems != 0)
                {
                    lblbtnSummatyCost.Text = Utils.DisplayDecimal(sumItem) + CURRENCYSYMBOLS;
                    lblbtnSummatyItem.Text = countItems.ToString("N0") + " "+ Utils.TextBundle("item", "Items");
                    btnSummatyItems.Enabled = true;
                }
                else
                { 
                    lblbtnSummatyCost.Text = Utils.DisplayDecimal(0)+ " "  + CURRENCYSYMBOLS;
                    lblbtnSummatyItem.Text = Utils.TextBundle("noitem", "No Item");
                    btnSummatyItems.Enabled = false;
                }
                #endregion

                SetupAutoLayout();
                //var xx = UIView.AnimationsEnabled;
                

            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this,"Error !",ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        public async void SearchBytxt()
        {
            try
            {
               //_ = TinyInsights.TrackEventAsync("SearchBytxt");
                // flag = 0 is grid view but flag = 1 is list view
                if (flag == 1)
                {
                    

                    
                    ItemManage itemManage = new ItemManage();
                    
                    Items = await itemManage.SearchItembyCategory(DataCashingAll.MerchantId, (int)MenuPos[(int)Select].SysCategoryID, txtSearch.Text);
                    
                    var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
                    if (check)
                    {
                        Items.Add(new Item() { SysItemID = -1 });
                    }
                    ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                    ((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                    itemPOSCollectionview.ReloadData();
                    itemPOSCollectionviewList.ReloadData();

                    

                }
                else
                {
                    
                    ItemManage itemManage = new ItemManage();

                    Items = await itemManage.SearchItembyCategory(DataCashingAll.MerchantId, (int)MenuPos[(int)Select].SysCategoryID, txtSearch.Text);
                    
                    var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
                    if (check)
                    {
                        Items.Add(new Item() { SysItemID = -1 });
                    }
                    ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Items);
                    ((ItemPosDataSourceList)itemPOSCollectionviewList.DataSource).ReloadData(Items);
                    itemPOSCollectionview.ReloadData();
                    itemPOSCollectionviewList.ReloadData();

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void CreateViewCell(UICollectionViewCell cell, Item item, UIView view)
        {
            try
            {

           
                //_ = TinyInsights.TrackEventAsync("CreateViewCell");
                var iconImg = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = true };

                UIImage uIImage ;
            
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    //var iamg = (cell as ItemPOSCollectionViewCell).getimage();
                    var r = new UIGraphicsImageRenderer(cell.Bounds.Size);
                    var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                    {
                        cell.Layer.RenderInContext(ctxt.CGContext);
                        //View.Capture(true);
                    });
                    ////var img = View.Capture(true);
                    uIImage = img;
                }
                else
                {
                    UIGraphics.BeginImageContextWithOptions(cell.Bounds.Size, cell.Opaque, 0);
                    cell.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                    //View.Layer.DrawInContext(UIGraphics.GetCurrentContext());
                    var img = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();
                    uIImage = img;
                
                }

                iconImg.Image = uIImage;
                View.AddSubview(iconImg);

            

                var frame = cell.Frame;
                frame.X += itemPOSCollectionview.Frame.X;
                var xx = itemPOSCollectionview.ContentOffset.X;
                var xxx = itemPOSCollectionview.ContentOffset.Y;
                frame.Y += itemPOSCollectionview.Frame.Y - itemPOSCollectionview.ContentOffset.Y;
                iconImg.Frame = frame;

                iconImg.Layer.CornerRadius = 5;
                iconImg.Layer.MasksToBounds = true;
                var frame1 = iconImg.Frame;
                frame1.Width = 10;
                frame1.Height = 10;
                frame1.X = (View.Frame.Width / 2) - (frame1.Width / 2);
                frame1.Y = View.Frame.Height - 30;

                UIView.Animate(0.7, () =>
                {
                    iconImg.Frame = frame1;
               
                }, () =>
                {

                    iconImg.RemoveFromSuperview();
                });
            }
            catch (Exception ex)
            {

                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
       

        //public async void handleLongPress(UILongPressGestureRecognizer gestureReconizer)
        //{
        //    if (gestureReconizer.State != UIGestureRecognizerState.Ended)
        //    {
        //        return;
        //    }
        //    var p = gestureReconizer.LocationInView(itemPOSCollectionview);
        //    var indexPath = this.itemPOSCollectionview.IndexPathForItemAtPoint(p);
        //    if((int)indexPath.Item != Items.Count-1)
        //    {
        //        POSDetailItemController.SelectedPOSItemDetail = Items[(int)indexPath.Item];
        //        if (POSDetailPage == null)
        //        {
        //            POSDetailPage = new POSDetailItemController();
        //        }
        //        DataCaching.DetailItemNavigation = this.NavigationController;
        //        this.NavigationController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        //        this.NavigationController.PresentViewController(POSDetailPage, false, null);
        //    }
        //    else
        //        return;
        //}
        public static void clearData()
        { 
            //remark = null;
            //countItems = 0;
            //sumItem = 0;
            //MenuPos=null;
            //Items = null;
            //ItemsLIST = null;
            //Quantity = 1;
            //itemPosData= null;
            //POScategory = 0;
            //flag = 0;
            //Quantity=0;
            //totlaItems=0;
            //totalResult=null;
            //tranWithDetails=null;
            //posScanPage = null;
            //posQuantityPage = null;
            //POSDetailPage = null;
            //cartPage = null;
    }
        private void ChooseItem(int row)
        {
            try
            {
                _ = TinyInsights.TrackEventAsync("ChooseItem");
                var itemchoose = Items[row];
                var trandetail = new TranDetailItemWithTopping()
                {
                    tranDetailItem = new TranDetailItemNew()
                    {
                        SysItemID = itemchoose.SysItemID,
                        MerchantID = DataCashingAll.MerchantId,
                        SysBranchID = DataCashingAll.SysBranchId,
                        ItemName = itemchoose.ItemName,
                        SaleItemType = itemchoose.SaleItemType,
                        FProcess = 1,
                        TranNo = tranWithDetails.tran.TranNo,
                        TaxType = itemchoose.TaxType,
                        Quantity = Quantity,
                        Price = itemchoose.Price,
                        ItemPrice = itemchoose.Price,
                        Discount = 0,
                        EstimateCost = itemchoose.EstimateCost,
                    }
                };
                if (itemchoose.FDisplayOption == 0)
                {
                    tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, trandetail);
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                }
                else
                {

                }

                
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        private async Task<List<Item>> FilterIteMPos(int row)
        {
            var item = new List<Item>();
            if (row == 0) // all
            {
                item = await itemmanager.GetAllItem();
            }
            else if (row == -1) // discount
            {
                item = await itemmanager.GetAllItem();
            }
            else if (row == -2) // fav item
            {
                item = await itemmanager.GetAllItem();
            }
            else
            {
                item = await itemmanager.GetItembyCategory(Convert.ToInt32(MainController.merchantlocal.MerchantID), row);
            // listItemBodyFilter = await itemManage.GetItembyCategory(DataCashingAll.MerchantId, fillter);
            }
            item.Add(new Item());
            return item; 
        }
        private async Task<List<Item>> FilterIteMPosLIST(int row)
        {
            var item = new List<Item>();
            if (row == 0) // all
            {
                item = await itemmanager.GetAllItem();
            }
            else if (row == -1) // discount
            {
                item = await itemmanager.GetAllItem();
            }
            else if (row == -2) // fav item
            {
                item = await itemmanager.GetAllItem();
            }
            else
            {
                item = await itemmanager.GetItembyCategory(Convert.ToInt32(MainController.merchantlocal.MerchantID), row);
            }
            return item;
        }

        private async Task<List<Category>> GetMenuPos()
        {
            try
            {

            
                catagory = new CategoryManage();
           
                var menu = new List<Category>();
                menu.Add(new Category
                {
                    MerchantID = MainController.merchantlocal.MerchantID,
                    SysCategoryID = 0,
                    Ordinary = null,
                    Name = Utils.TextBundle("all", "ALL"),
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    DataStatus = 'A',
                    FWaitSending = 0,
                    LinkProMaxxID = "xxxxxxxx",
                    WaitSendingTime = DateTime.UtcNow
                }); 

                var listitemall = await itemmanager.GetAllItem();
                AllItem = await itemmanager.GetAll(DataCashingAll.MerchantId);
                AllItemStatusD = AllItem.Where(x => x.DataStatus == 'D').ToList();
                var listFavorite = listitemall.Where(x => x.FavoriteNo > 0).ToList();
                if (listFavorite.Count > 0)
                {
                    menu.Add(new Category
                    {
                        MerchantID = MainController.merchantlocal.MerchantID,
                        SysCategoryID = -2,
                        Ordinary = null,
                        Name = Utils.TextBundle("favorite", "Favorite"),
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        DataStatus = 'A',
                        FWaitSending = 0,
                        LinkProMaxxID = "xxxxxxxx",
                        WaitSendingTime = DateTime.UtcNow
                    });
                }
                var listitemtoppingall = await itemmanager.GetToppingItem();
            
                if (listitemtoppingall.Count > 0)
                {
                    menu.Add(new Category
                    {
                        MerchantID = MainController.merchantlocal.MerchantID,
                        SysCategoryID = -3,
                        Ordinary = null,
                        Name = Utils.TextBundle("extratopping", "Extra Topping"),
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        DataStatus = 'A',
                        FWaitSending = 0,
                        LinkProMaxxID = "xxxxxxxx",
                        WaitSendingTime = DateTime.UtcNow
                    });
                }
                //menu.Add(new Category
                //{
                //    MerchantID = MainController.merchantlocal.MerchantID,
                //    SysCategoryID = -1,
                //    Ordinary = null,
                //    Name = "Discount",
                //    DateCreated = DateTime.UtcNow,
                //    DateModified = DateTime.UtcNow,
                //    DataStatus = 'A',
                //    FWaitSending = 0,
                //    LinkProMaxxID = "xxxxxxxx",
                //    WaitSendingTime = DateTime.UtcNow
                //});

                var tmp = await catagory.GetAllCategoryhaveitem();
                for (int i = 0; i < tmp.Count; i++)
                {
                    menu.Add(tmp[i]);
                }
                return menu;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return  new List<Category>();
            }
        }

        public async void initialData()
        {
            try
            {

           
                var Vat = DataCashingAll.setmerchantConfig.TAXRATE;
                var VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                var FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                DeviceTranRunningNoManage DeviceTranRunningNo = new DeviceTranRunningNoManage();
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                var maxtranno = await DeviceTranRunningNo.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
                maxtranno++;
                if (tranWithDetails is null)
                {
                    tranWithDetails = new TranWithDetailsLocal();
                    tranWithDetails.tran = new ORM.MerchantDB.Tran();
                    tranWithDetails.tran.MerchantID = DataCashingAll.MerchantId;
                    tranWithDetails.tran.SysBranchID = DataCashingAll.SysBranchId;
                    tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
                    tranWithDetails.tran.TranDate = DateTime.UtcNow;
                    tranWithDetails.tran.Status = 10;
                    tranWithDetails.tran.DeviceNo = DataCashingAll.DeviceNo;
                    tranWithDetails.tran.SysCustomerID = 999;
                    tranWithDetails.tran.CustomerName = "ลูกค้าทั่วไป";//
                    tranWithDetails.tran.SellerName = usernamelogin;
                    tranWithDetails.tran.LastDateModified = DateTime.UtcNow;
                    tranWithDetails.tran.LastUserModified = usernamelogin;
                    tranWithDetails.tran.FCancel = 0;
                    tranWithDetails.tran.TranTaxType = 'I';
                    tranWithDetails.tran.CountTradDisc = 0;
                    tranWithDetails.tran.SubTotalNoneVat = 0;
                    tranWithDetails.tran.TotalTradDiscNoneVat = 0;
                    tranWithDetails.tran.TotalNoneVat = 0;
                    tranWithDetails.tran.SubTotalHaveVat = 0;
                    tranWithDetails.tran.TotalTradDiscHaveVat = 0;
                    tranWithDetails.tran.TotalHaveVat = 0;
                    tranWithDetails.tran.Total = 0;
                    tranWithDetails.tran.ServiceCharge = 0;
                    tranWithDetails.tran.FmlServiceCharge = FmlServiceCharge;
                    tranWithDetails.tran.TotalVat = 0;
                    tranWithDetails.tran.GrandTotal = 0;
                    tranWithDetails.tran.PaymentFractional = 0;
                    tranWithDetails.tran.GrandPayment = 0;
                    tranWithDetails.tran.SummaryPayment = 0;
                    tranWithDetails.tran.Change = 0;
                    tranWithDetails.tran.Tips = 0;
                    tranWithDetails.tran.TotalPointEarning = 0;
                    tranWithDetails.tran.PrintCounter = 0;
                    tranWithDetails.tran.TaxRate = UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE);
                    tranWithDetails.tran.TranTaxType = char.Parse(VatType);

                    //merchantDB
                    tranWithDetails.tran.FWaitSending = 1;
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    tranWithDetails.tran.Comments = null;
                    tranWithDetails.tran.LocalDataStatus = 'I';


                    //Order or Bill
                    tranWithDetails.tran.TranType = 'B';
                    tranWithDetails.tran.OrderName = null;
                    tranWithDetails.tran.Status = 10;
                }
                else
                {
                    decimal SubTotalHaveVat = 0;
                    decimal SubTotalNoneVat = 0;
                    totlaItems = 0;
                    foreach (var item in tranWithDetails.tranDetailItemWithToppings)
                    {
                        if (item.tranDetailItem.TaxType == 'N')
                        {
                            SubTotalNoneVat += item.tranDetailItem.Amount;
                        }
                        else
                        {
                            SubTotalHaveVat += item.tranDetailItem.Amount;
                        }
                        totlaItems += (int)item.tranDetailItem.Quantity;
                    }
                    totalResult = SubTotalNoneVat + SubTotalHaveVat + "";
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public async Task initialDatanew()
        {
            try
            {


                var Vat = DataCashingAll.setmerchantConfig.TAXRATE;
                var VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                var FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                DeviceTranRunningNoManage DeviceTranRunningNo = new DeviceTranRunningNoManage();
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                var maxtranno = await DeviceTranRunningNo.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
                maxtranno++;
                if (tranWithDetails is null)
                {
                    tranWithDetails = new TranWithDetailsLocal();
                    tranWithDetails.tran = new ORM.MerchantDB.Tran();
                    tranWithDetails.tran.MerchantID = DataCashingAll.MerchantId;
                    tranWithDetails.tran.SysBranchID = DataCashingAll.SysBranchId;
                    tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
                    tranWithDetails.tran.TranDate = DateTime.UtcNow;
                    tranWithDetails.tran.Status = 10;
                    tranWithDetails.tran.DeviceNo = DataCashingAll.DeviceNo;
                    tranWithDetails.tran.SysCustomerID = 999;
                    tranWithDetails.tran.CustomerName = "ลูกค้าทั่วไป";//
                    tranWithDetails.tran.SellerName = usernamelogin;
                    tranWithDetails.tran.LastDateModified = DateTime.UtcNow;
                    tranWithDetails.tran.LastUserModified = usernamelogin;
                    tranWithDetails.tran.FCancel = 0;
                    tranWithDetails.tran.TranTaxType = 'I';
                    tranWithDetails.tran.CountTradDisc = 0;
                    tranWithDetails.tran.SubTotalNoneVat = 0;
                    tranWithDetails.tran.TotalTradDiscNoneVat = 0;
                    tranWithDetails.tran.TotalNoneVat = 0;
                    tranWithDetails.tran.SubTotalHaveVat = 0;
                    tranWithDetails.tran.TotalTradDiscHaveVat = 0;
                    tranWithDetails.tran.TotalHaveVat = 0;
                    tranWithDetails.tran.Total = 0;
                    tranWithDetails.tran.ServiceCharge = 0;
                    tranWithDetails.tran.FmlServiceCharge = FmlServiceCharge;
                    tranWithDetails.tran.TotalVat = 0;
                    tranWithDetails.tran.GrandTotal = 0;
                    tranWithDetails.tran.PaymentFractional = 0;
                    tranWithDetails.tran.GrandPayment = 0;
                    tranWithDetails.tran.SummaryPayment = 0;
                    tranWithDetails.tran.Change = 0;
                    tranWithDetails.tran.Tips = 0;
                    tranWithDetails.tran.TotalPointEarning = 0;
                    tranWithDetails.tran.PrintCounter = 0;
                    tranWithDetails.tran.TaxRate = UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE);
                    tranWithDetails.tran.TranTaxType = char.Parse(VatType);

                    //merchantDB
                    tranWithDetails.tran.FWaitSending = 1;
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    tranWithDetails.tran.Comments = null;
                    tranWithDetails.tran.LocalDataStatus = 'I';


                    //Order or Bill
                    tranWithDetails.tran.TranType = 'B';
                    tranWithDetails.tran.OrderName = null;
                    tranWithDetails.tran.Status = 10;
                }
                else
                {
                    decimal SubTotalHaveVat = 0;
                    decimal SubTotalNoneVat = 0;
                    totlaItems = 0;
                    foreach (var item in tranWithDetails.tranDetailItemWithToppings)
                    {
                        if (item.tranDetailItem.TaxType == 'N')
                        {
                            SubTotalNoneVat += item.tranDetailItem.Amount;
                        }
                        else
                        {
                            SubTotalHaveVat += item.tranDetailItem.Amount;
                        }
                        totlaItems += (int)item.tranDetailItem.Quantity;
                    }
                    totalResult = SubTotalNoneVat + SubTotalHaveVat + "";
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void btnSummatyItems_Click()
        {

            Utils.SetTitle(this.NavigationController, Utils.TextBundle("cart", "Cart"));
            var cartPage = new CartController(false);
            this.NavigationController.PushViewController(cartPage, false);
        }

        void SetupAutoLayout()
        {
            try
            {

            
                menuPOSCollectionview.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
                menuPOSCollectionview.HeightAnchor.ConstraintEqualTo(40).Active = true;
                menuPOSCollectionview.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                menuPOSCollectionview.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

                searchView.TopAnchor.ConstraintEqualTo(menuPOSCollectionview.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                searchView.HeightAnchor.ConstraintEqualTo(40).Active = true;
                searchView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                searchView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

                btnSearch.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                btnSearch.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnSearch.LeftAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
                btnSearch.HeightAnchor.ConstraintEqualTo(28).Active = true;

                txtSearch.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                txtSearch.RightAnchor.ConstraintEqualTo(btnSearchQr.SafeAreaLayoutGuide.LeftAnchor,-5).Active = true;
                txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
                txtSearch.HeightAnchor.ConstraintEqualTo(28).Active = true;

                btnSearchQr.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                btnSearchQr.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnSearchQr.RightAnchor.ConstraintEqualTo(btnOrder.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
                btnSearchQr.HeightAnchor.ConstraintEqualTo(28).Active = true;


                btnOrder.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                btnOrder.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnOrder.RightAnchor.ConstraintEqualTo(btnDummy.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
                btnOrder.HeightAnchor.ConstraintEqualTo(28).Active = true;

                btnDummy.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                btnDummy.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnDummy.RightAnchor.ConstraintEqualTo(btnListView.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
                btnDummy.HeightAnchor.ConstraintEqualTo(28).Active = true;

                btnListView.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                btnListView.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnListView.RightAnchor.ConstraintEqualTo(btnQuantity.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
                btnListView.HeightAnchor.ConstraintEqualTo(28).Active = true;

                btnQuantity.CenterYAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.CenterYAnchor,0).Active = true;
                btnQuantity.WidthAnchor.ConstraintEqualTo(28).Active = true;
                btnQuantity.RightAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
                btnQuantity.HeightAnchor.ConstraintEqualTo(28).Active = true;

                itemPOSCollectionview.TopAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                itemPOSCollectionview.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -65).Active = true;
                itemPOSCollectionview.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 5).Active = true;
                itemPOSCollectionview.RightAnchor.ConstraintEqualTo(View.RightAnchor, -5).Active = true;

                itemPOSCollectionviewList.TopAnchor.ConstraintEqualTo(searchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                itemPOSCollectionviewList.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -65).Active = true;
                itemPOSCollectionviewList.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
                itemPOSCollectionviewList.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

                bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
                bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

                btnSummatyItems.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
                btnSummatyItems.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor,-10).Active = true;
                btnSummatyItems.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
                btnSummatyItems.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

                lblbtnSummatyCost.TopAnchor.ConstraintEqualTo(btnSummatyItems.SafeAreaLayoutGuide.TopAnchor, 3).Active = true;
                lblbtnSummatyCost.CenterXAnchor.ConstraintEqualTo(btnSummatyItems.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
                lblbtnSummatyCost.HeightAnchor.ConstraintEqualTo(24).Active = true;

                lblbtnSummatyItem.TopAnchor.ConstraintEqualTo(lblbtnSummatyCost.SafeAreaLayoutGuide.BottomAnchor).Active = true;
                lblbtnSummatyItem.CenterXAnchor.ConstraintEqualTo(btnSummatyItems.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
                lblbtnSummatyItem.HeightAnchor.ConstraintEqualTo(12).Active = true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
       
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        async void getTranOrderDetail(Tran tran)
        {
            try
            {
                var lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                TranDetailItemWithTopping detailItemWithTopping = new TranDetailItemWithTopping();

                tranWithDetails = new TranWithDetailsLocal();
                tranWithDetails.tran = tran;
                var checkDateLocal = tranWithDetails.tran.TranDate;
                var dateLocal = UtilsAll.TranDateformat(checkDateLocal);
                tranWithDetails.tran.TranDate = dateLocal;
                tranWithDetails.tran.LastDateModified = dateLocal;
                tranWithDetails.tran.WaitSendingTime = dateLocal;

                //List<Detail Item >             
                var tranDetail = await tranDetailItemManage.GetTranDetailItem(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);

                foreach (var item in tranDetail)
                {
                    //Detail Item
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
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
                        ItemPrice = item.ItemPrice,
                    };
                    List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                    //Detail ItemTopping        
                    var tranTopping = await toppingManage.GetTranDetailItemTopping(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo, (int)item.DetailNo);
                    foreach (var itemtopping in tranTopping)
                    {
                        TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                        {
                            MerchantID = itemtopping.MerchantID,
                            SysBranchID = itemtopping.SysBranchID,
                            TranNo = itemtopping.TranNo,
                            DetailNo = itemtopping.DetailNo,
                            ToppingNo = itemtopping.ToppingNo,
                            ItemName = itemtopping.ItemName,//toppping
                            SysItemID = itemtopping.SysItemID,
                            UnitName = itemtopping.UnitName,
                            RegularSizeName = itemtopping.RegularSizeName,
                            Quantity = itemtopping.Quantity,
                            ToppingPrice = itemtopping.ToppingPrice,
                            EstimateCost = itemtopping.EstimateCost,
                            Comments = itemtopping.Comments
                        };
                        lstitemDetail.Add(itemDetail);
                    }

                    detailItemWithTopping = new TranDetailItemWithTopping();
                    detailItemWithTopping.tranDetailItem = DetailItem;
                    detailItemWithTopping.tranDetailItemToppings = lstitemDetail;
                    lsttranDetailItemWithToppings.Add(detailItemWithTopping);

                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                tranWithDetails.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);

                //Tran Payment
                var tranPayment = await tranPaymentManage.GetTranPayment(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var item in tranPayment)
                {
                    tranWithDetails.tranPayments.Add(item);
                }

                //Tran Discount
                var tranDiscount = await discountManage.GetTranTradDiscount(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                foreach (var itemDiscount in tranDiscount)
                {
                    tranWithDetails.tranTradDiscounts.Add(itemDiscount);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


    }

}
