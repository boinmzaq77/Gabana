using CoreGraphics;
using Foundation;
using Gabana.iOS.ITEMS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ItemsController : UIViewController
    {
        int MenuSelected=0;
        public static bool Ismodify;
        public static int NoteStatus=0; // 0 add - 1 edit
        UIImageView emptyItemView, emptyExtraToppingView, emptyCategoryView;
        UILabel lbl_empty_Item, lbl_empty_ExtaTopping, lbl_empty_Category, lbl_empty_ItemStock;
        UIView ItemView , ItemStockView , CategoryView, ToppingView;
        UICollectionView menuItemBarCollectionView , itemStockListCollectionView;
        ItemsAddToppingController itemsAddTopping;
        ItemDataSourceList itemPosDataList;
        ItemToppingDataSourceList ToppingDataList;
        UICollectionView itemListCollectionView, CatagoryListCollectionView,ToppingCollectionView;
        MenuItemDataSource menuItemDataSource;
        UIImageView btnAddItem , emptyItemStockView, btnAddCatagory,btnAddTopping;
        UIButton btnSearch, btnScan;
        UITextField txtSearch;
        
        UIView SearchbarView;
        ItemsAddCategoryController itemsAddCategory=null;
        public static List<Item> Items;
        public static List<Item> ItemsStock;
        public static List<Item> Topping;
        public static List<ItemOnBranch> stock = new List<ItemOnBranch>();
        static List<Category> Categories;
        CategoryManage CategoryManager = new CategoryManage();
        ItemManage itemmanager = new ItemManage();
        ItemOnBranchManage ItemOnBranchManager = new ItemOnBranchManage();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        NotificationManager notificationManager = new NotificationManager();
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        public static bool checknet = true; 
        int maxItemRevision;
        private UIView oriView;
        string usernamelogin;
        List<MenuitemHeaderIOS> Menu ;
        public static bool isScanBarcode = false;
        public static string txtBarcodeScan = "";
        private ItemStockDataSourceList itemStockPosDataList;
        private ItemCatagoeyDataSourceList catagoryDataList;
        internal bool FormMain;
        private string LoginType;
        private long LastRevisionNoStock;
        private UILabel txtStockRevision;

        public ItemsController()
        {
            
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                this.NavigationController.SetNavigationBarHidden(false, false);
                checknet = await GabanaAPI.CheckNetWork();
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("item","Item")) ;
            
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                await Utils.CheckJWT();

                //txtSearch.Text = null;
                resetCell();
                if (isScanBarcode == true)
                {
                    isScanBarcode = false;
                    txtSearch.Text = txtBarcodeScan;
                    txtBarcodeScan = "";
                    await showList();
                    //SearchBytxt();
                }
                if (Ismodify)
                {
                    //SearchBytxt
                    //SearchBytxt();
                    await showList();
                    Ismodify = false;

                }
                if (DataCaching.NewItem != null)
                {

                }
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
                if (FormMain)
                {
                    FormMain = false;
                    MenuSelected = 0;
                    Menu[0].select = true;
                    Menu[1].select = false;
                    Menu[2].select = false;
                    Menu[3].select = false;
                    btnScan.Hidden = false;
                    txtSearch.Text = "";
                    ((MenuItemDataSource)menuItemBarCollectionView.DataSource).ReloadData(Menu);
                    menuItemBarCollectionView.ReloadData();
                    await showList();
                }
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public void resetCell()
        {
             
            var data = itemListCollectionView?.DataSource as ItemDataSourceList;
            if (data.choosecell != null)
            {
                var frame2 = data.choosecell.Frame;
                frame2.X = 0;
                UIView.Animate(0.7, () =>
                {
                    data.choosecell.showbtndelete = false;
                    data.choosecell.Frame = frame2;
                });
            };
            //----------------------------------------------------
            var data2 = ToppingCollectionView?.DataSource as ItemToppingDataSourceList;
            if (data2.choosecell != null)
            {
                var frame3 = data2.choosecell.Frame;
                frame3.X = 0;
                UIView.Animate(0.7, () =>
                {
                    data2.choosecell.showbtndelete = false;
                    data2.choosecell.Frame = frame3;
                });
            };
            //----------------------------------------------------
            var data3 = CatagoryListCollectionView?.DataSource as ItemCatagoeyDataSourceList;
            if (data3.choosecell != null)
            {
                var frame4 = data3.choosecell.Frame;
                frame4.X = 0;
                UIView.Animate(0.7, () =>
                {
                    data3.choosecell.showbtndelete = false;
                    data3.choosecell.Frame = frame4;
                });
            };
             
        }
        public async override void ViewDidLoad()
        {
            Ismodify = false;
            try
            {
                checknet = MainController.checknet;
                LoginType = Preferences.Get("LoginType", "");
                oriView = new UIView(this.View.Frame);
                usernamelogin = Preferences.Get("User", "");
                #region getdata
                Items = await itemmanager.GetAllItem();
                Categories = await CategoryManager.GetAllCategory();
                Topping = await itemmanager.GetToppingItem();
                ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                stock = await ItemOnBranchManager.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);

                #endregion
                this.NavigationController.SetNavigationBarHidden(false, false);
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.White;

                var g = new UITapGestureRecognizer(() => View.EndEditing(true));
                g.CancelsTouchesInView = false;
                View.AddGestureRecognizer(g);

                initattribute();
                setupAutoLayout();
                setUpMenu();

                Setkeyboard();
                await showList();

                var refreshControl = new UIRefreshControl() 
                {
                    
                };
                //refreshControl.Frame.Height = View.Frame.Height;
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    Items = await GetFilterItemList();
                    //itemListCollectionView.DataSource = new ItemDataSourceList(Items, stock, Categories);
                    ((ItemDataSourceList)itemListCollectionView.DataSource).ReloadData(Items, stock, Categories);
                    itemListCollectionView.ReloadData();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                itemListCollectionView.AlwaysBounceVertical = true;
                itemListCollectionView.AddSubview(refreshControl);
                //itemListCollectionView.RefreshControl.Frame.Height = View.Frame.Height;
                var refreshControl1 = new UIRefreshControl();
                refreshControl1.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl1.AddTarget(async (obj, sender)  => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.CategoryChange();
                        await notificationManager.ItemChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    Categories = await GetFilterCategory();
                    Items = await GetFilterItemList();
                    // CatagoryListCollectionView.DataSource = new ItemCatagoeyDataSourceList(Categories);
                    ((ItemCatagoeyDataSourceList)CatagoryListCollectionView.DataSource).ReloadData(Categories);
                    CatagoryListCollectionView.ReloadData();
                    refreshControl1.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                CatagoryListCollectionView.AlwaysBounceVertical = true;
                CatagoryListCollectionView.AddSubview(refreshControl1);

                var refreshControl2 = new UIRefreshControl();
                refreshControl2.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl2.AddTarget(async (obj, sender) =>
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    Categories = await CategoryManager.GetAllCategory();
                    Topping = await GetFilterExtraTopping();
                    //ToppingCollectionView.DataSource = new ItemToppingDataSourceList(Topping, stock, Categories);
                    ((ItemToppingDataSourceList)ToppingCollectionView.DataSource).ReloadData(Topping, stock, Categories);
                    ToppingCollectionView.ReloadData();
                    refreshControl2.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                ToppingCollectionView.AlwaysBounceVertical = true;
                ToppingCollectionView.AddSubview(refreshControl2);

                var refreshControl3 = new UIRefreshControl();
                refreshControl3.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl3.AddTarget(async (obj, sender) =>
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemOnBranchChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    Items = await GetFilterItemList();
                    Topping = await GetFilterExtraTopping();
                    var allitem = new List<Item>();
    
                    if (Items != null)
                    {
                        allitem.AddRange(Items);
                    } 
                    if (Topping != null)
                    {
                        allitem.AddRange(Topping);
                    }
                    ItemsStock = new List<Item>(allitem.Where(x => x.FTrackStock == 1).OrderBy(x=>x.ItemName).ToList());

                    //ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                    stock = await ItemOnBranchManager.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                    //itemStockListCollectionView.DataSource = new ItemStockDataSourceList(ItemsStock, stock, Categories);
                    ((ItemStockDataSourceList)itemStockListCollectionView.DataSource).ReloadData(ItemsStock, stock, Categories);
                    itemStockListCollectionView.ReloadData();
                    await GetLastRevisionNoStock();
                    refreshControl3.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                
                itemStockListCollectionView.AlwaysBounceVertical = true;
                itemStockListCollectionView.AddSubview(refreshControl3);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        private void Setkeyboard()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            void OnKeyboardNotification(NSNotification notification)
            {
                if (!IsViewLoaded) return;


                //Check if the keyboard is becoming visible
                var visible = notification.Name == UIKeyboard.WillShowNotification;

                //Start an animation, using values from the keyboard
                //UIView.BeginAnimations("AnimateForKeyboard");
                //UIView.SetAnimationBeginsFromCurrentState(true);
                UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
                UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

                //Pass the notification, calculating keyboard height, etc.
                bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                var keyboardFrame = visible
                                        ? UIKeyboard.FrameEndFromNotification(notification)
                                        : UIKeyboard.FrameBeginFromNotification(notification);

                OnKeyboardChanged(View, visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);

                //Commit the animation
                //UIView.CommitAnimations();
            }
        }
        public void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CGRect(0, 0, oriView.Frame.Width, oriView.Frame.Height);
            else

                view.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height - nfloat);
        }
        private async Task ItemChange()
        {
            try
            {
                
                var listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();

                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 30)
                    {
                        #region Item
                        //------------------------------------------------
                        //Get Item API
                        //offset = index สำหรับเรียกข้อมูล ครั้งละ 100 ตัว เริ่มที่ 0
                        //total >= 100 item = 0 - 99     รอบที่ 1  offset = 0
                        //             item = 100 - 199  รอบที่ 2  offset = 1
                        //total > 100  => totalitem/100 = จำนวนรอบที่เรียก 
                        //------------------------------------------------
                        try
                        {
                            var allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, 0);

                            if (allItem == null)
                            {
                                break;
                            }
                            else if (allItem?.ItemsWithItemExSizes.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                int round = allItem.totalItems / 100;
                                int addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, j);

                                    if (allItem == null)
                                    {
                                        break;
                                    }

                                    if (allItem.totalItems == 0)
                                    {
                                        break;
                                    }

                                    allItem.ItemsWithItemExSizes.ToList().OrderBy(x => x.ItemStatus.item.RevisionNo);
                                    var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            

                                    foreach (var item in allItem.ItemsWithItemExSizes)
                                    {
                                        var data = await itemmanager.GetItem(item.ItemStatus.item.MerchantID, item.ItemStatus.item.SysItemID);
                                        if (item.ItemStatus.DataStatus == 'D')
                                        {
                                            //delete รูป
                                            if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                            {
                                               //
                                            }

                                            //delete itemOnBranchs
                                            if (item.itemOnBranchs != null)
                                            {
                                                foreach (var onBranch in item.itemOnBranchs)
                                                {
                                                    var deleteItemonBranch = await ItemOnBranchManager.DeleteItemOnBranch(item.ItemStatus.item.MerchantID, onBranch.SysBranchID, item.ItemStatus.item.SysItemID);
                                                }
                                            }
                                            var deleteItemSize = await itemExSizeManage.DeleteItemsize(item.ItemStatus.item.MerchantID, item.ItemStatus.item.SysItemID);
                                            var delete = await itemmanager.DeleteItem(item.ItemStatus.item.MerchantID, item.ItemStatus.item.SysItemID);
                                        }
                                        else
                                        {
                                            //insertorreplace
                                            var getItem = new Item()
                                            {
                                                MerchantID = item.ItemStatus.item.MerchantID,
                                                SysItemID = item.ItemStatus.item.SysItemID,
                                                ItemName = item.ItemStatus.item.ItemName,
                                                Ordinary = item.ItemStatus.item.Ordinary,
                                                SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                                ItemCode = item.ItemStatus.item.ItemCode,
                                                ShortName = item.ItemStatus.item.ShortName,
                                                PicturePath = item.ItemStatus.item.PicturePath,
                                                ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                                PictureLocalPath = data?.PictureLocalPath,
                                                ThumbnailLocalPath = data?.ThumbnailLocalPath,
                                                Colors = item.ItemStatus.item.Colors,
                                                FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                                UnitName = item.ItemStatus.item.UnitName,
                                                RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                                EstimateCost = item.ItemStatus.item.EstimateCost,
                                                Price = item.ItemStatus.item.Price,
                                                OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                                TaxType = item.ItemStatus.item.TaxType,
                                                SellBy = item.ItemStatus.item.SellBy,
                                                FTrackStock = item.ItemStatus.item.FTrackStock,
                                                TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                                SaleItemType = item.ItemStatus.item.SaleItemType,
                                                Comments = item.ItemStatus.item.Comments,
                                                LastDateModified = item.ItemStatus.item.LastDateModified,
                                                UserLastModified = item.ItemStatus.item.UserLastModified,
                                                DataStatus = 'I',
                                                FWaitSending = 1,
                                                WaitSendingTime = DateTime.UtcNow,
                                                LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                                LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                                FDisplayOption = item.ItemStatus.item.FDisplayOption
                                            };

                                            var insertOrreplace = await itemmanager.InsertOrReplaceItem(getItem);

                                            if (!string.IsNullOrEmpty(getItem.PicturePath))
                                            {
                                                await Utils.InsertLocalPictureItem(getItem);
                                            }

                                            foreach (var itemSize in item.itemExSizes)
                                            {
                                                var getitemSize = new ItemExSize()
                                                {
                                                    MerchantID = itemSize.MerchantID,
                                                    SysItemID = itemSize.SysItemID,
                                                    EstimateCost = itemSize.EstimateCost,
                                                    ExSizeName = itemSize.ExSizeName,
                                                    ExSizeNo = itemSize.ExSizeNo,
                                                    Price = itemSize.Price,
                                                    Comments = itemSize.Comments
                                                };
                                                var insertItemSize = await itemExSizeManage.InsertOrReplaceItemSize(getitemSize);
                                            }
                                        }
                                        maxItemRevision = item.ItemStatus.item.RevisionNo;
                                    }
                                    await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            Console.WriteLine(ex.Message);
                            await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemChange");
                Utils.ShowMessage(ex.Message);
                
            }
        }
        void setUpMenu()
        {
            Menu = new List<MenuitemHeaderIOS>();
            Menu.Add(new MenuitemHeaderIOS(0, Utils.TextBundle("item", "Item"), true));
            Menu.Add(new MenuitemHeaderIOS(1, Utils.TextBundle("extratopping", "xtra Topping"), false));
            Menu.Add(new MenuitemHeaderIOS(1, Utils.TextBundle("stock", "Stock"), false));
            Menu.Add(new MenuitemHeaderIOS(2, Utils.TextBundle("category", "Category"), false));

            menuItemDataSource = new MenuItemDataSource(Menu);
            menuItemBarCollectionView.DataSource = menuItemDataSource;
            menuItemBarCollectionView.ReloadData();
        }
        async void initattribute()
        {
            #region menuItemBarCollectionView
            UICollectionViewFlowLayout MenuflowLayoutList = new UICollectionViewFlowLayout();
            MenuflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            MenuflowLayoutList.MinimumLineSpacing = 0;
            MenuflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width  / 4) , height: 40);




            menuItemBarCollectionView = new UICollectionView(frame: View.Frame, layout: MenuflowLayoutList);
            menuItemBarCollectionView.BackgroundColor = UIColor.White;
            menuItemBarCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            menuItemBarCollectionView.ShowsVerticalScrollIndicator = false;
            menuItemBarCollectionView.ScrollEnabled = false;
            menuItemBarCollectionView.RegisterClassForCell(cellType: typeof(MenuCollectionViewCell), reuseIdentifier: "MenuCollectionViewCell");;
            MenuItemBarCollectionDelegate CollectionDelegate = new MenuItemBarCollectionDelegate();
            CollectionDelegate.OnItemSelected += async (indexPath) => {

                if ((int)indexPath.Row == 0)
                {
                    Menu[0].select = true;
                    Menu[1].select = false;
                    Menu[2].select = false;
                    Menu[3].select = false;
                    btnScan.Hidden = false;
                    txtSearch.Placeholder = Utils.TextBundle("itemplace", "");
                }
                else if((int)indexPath.Row == 1)
                {
                    Menu[0].select = false;
                    Menu[1].select = true;
                    Menu[2].select = false;
                    Menu[3].select = false;
                    btnScan.Hidden = true;
                    txtSearch.Placeholder = Utils.TextBundle("toppingplace", "");
                }
                else if ((int)indexPath.Row == 2)
                {
                    Menu[0].select = false;
                    Menu[1].select = false;
                    Menu[2].select = true;
                    Menu[3].select = false;
                    btnScan.Hidden = true;
                    txtSearch.Placeholder = Utils.TextBundle("stockplace", "");
                }
                else
                {
                    Menu[0].select = false;
                    Menu[1].select = false;
                    Menu[2].select = false;
                    Menu[3].select = true;
                    btnScan.Hidden = true;
                    txtSearch.Placeholder = Utils.TextBundle("groupplace", "");
                }

                ((MenuItemDataSource)menuItemBarCollectionView.DataSource).ReloadData(Menu);
                menuItemBarCollectionView.ReloadData();
                MenuSelected = (int)indexPath.Row;

                //if (txtSearch.Text != "")
                //{
                //    txtSearch.Text = "";
                //    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                //    Items = await GetFilterItemList();
                //    Categories = await CategoryManager.GetAllCategory();
                //    Topping = await GetFilterExtraTopping();
                //    ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                //}
                //await SearchBytxt();
                await showList();
            };
            menuItemBarCollectionView.Delegate = CollectionDelegate;
            View.AddSubview(menuItemBarCollectionView);
            #endregion

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

            txtSearch = new UITextField
            {
                TextColor = UIColor.FromRGB(0,149,218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSearch.BackgroundColor = UIColor.Clear;
            txtSearch.Placeholder = Utils.TextBundle("itemplace","");
            txtSearch.Font = txtSearch.Font.WithSize(15);
            txtSearch.ReturnKeyType = UIReturnKeyType.Done;
            txtSearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                SearchBytxt();
                return true;
            };
            txtSearch.EditingChanged += TxtSearch_EditingChanged;
            SearchbarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside  += async (sender, e)  =>
            {
                if (btnSearch.ImageView.Image == UIImage.FromBundle("Search"))
                {
                    txtSearch.BecomeFirstResponder();
                }
                else
                {
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    txtSearch.Text = "";
                    Items = await GetFilterItemList();
                    Categories = await CategoryManager.GetAllCategory();
                    Topping = await GetFilterExtraTopping();
                    var allitem = new List<Item>();
                    if (Items != null)
                    {
                        allitem.AddRange(Items);
                    }
                    if (Topping != null)
                    {
                        allitem.AddRange(Topping);
                    }
                    ItemsStock = new List<Item>(allitem.Where(x => x.FTrackStock == 1).OrderBy(x => x.ItemName).ToList());
                    //ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                    //itemListCollectionView.DataSource = new ItemDataSourceList(Items, stock, Categories);
                    ((ItemDataSourceList)itemListCollectionView.DataSource).ReloadData(Items, stock, Categories);
                    itemListCollectionView.ReloadData();
                    ((ItemStockDataSourceList)itemStockListCollectionView.DataSource).ReloadData(ItemsStock, stock, Categories);
                    itemStockListCollectionView.ReloadData();
                    ((ItemToppingDataSourceList)ToppingCollectionView.DataSource).ReloadData(Topping, stock, Categories);
                    ToppingCollectionView.ReloadData();
                    ((ItemCatagoeyDataSourceList)CatagoryListCollectionView.DataSource).ReloadData(Categories);
                    CatagoryListCollectionView.ReloadData();

                }
                
            };
            SearchbarView.AddSubview(btnSearch);

            btnScan = new UIButton();
            btnScan.SetImage(UIImage.FromFile("ScanItem.png"), UIControlState.Normal);
            btnScan.TranslatesAutoresizingMaskIntoConstraints = false;
            btnScan.TouchUpInside += (sender, e) =>
            {
                //scan
                if (Utils.Checkpermisstion())
                {
                    string page = "ITEM";
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("scancode", "Scan code"));
                    POSScanBarcodeController scanPage = new POSScanBarcodeController(page);
                    this.NavigationController.PushViewController(scanPage, false);
                }
                
            };
            SearchbarView.AddSubview(btnScan);
            #endregion

            #region initView
            ItemView = new UIView();
            ItemView.Hidden = false;
            ItemView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ItemView);

            CategoryView = new UIView();
            CategoryView.Hidden = true;
            CategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(CategoryView);

            ToppingView = new UIView();
            ToppingView.Hidden = true;
            ToppingView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ToppingView);

            ItemStockView = new UIView();
            ItemStockView.Hidden = false;
            ItemStockView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ItemStockView);
            #endregion

            #region itemLayout
            #region emptyItemView
            emptyItemView = new UIImageView();
            emptyItemView.Hidden = true;
            emptyItemView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyItemView.Image = UIImage.FromBundle("DefaultItem");
            emptyItemView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemView.AddSubview(emptyItemView);

            lbl_empty_Item = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_Item.Hidden = true;
            lbl_empty_Item.Lines = 2;
            lbl_empty_Item.Font = lbl_empty_Item.Font.WithSize(16);
            lbl_empty_Item.Text = Utils.TextBundle("nullitem", "nullitem");
            ItemView.AddSubview(lbl_empty_Item);
            #endregion
            #region itemListCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 80, height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumLineSpacing = 0;

            itemListCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            itemListCollectionView.BackgroundColor = UIColor.White;
            itemListCollectionView.ShowsVerticalScrollIndicator = false;
            itemListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            itemListCollectionView.RegisterClassForCell(cellType: typeof(CellItemList), reuseIdentifier: "itemPosCellList");

            itemPosDataList = new ItemDataSourceList(Items,stock,Categories);
            itemPosDataList.OnCardCellDelete += ItemPosDataList_OnCardCellDelete;
            itemPosDataList.OnCardCellFav += ItemPosDataList_OnCardCellFav;
            itemPosDataList.OnCardCell += ItemPosDataList_OnCardCell;
            itemPosDataList.OnScroll += ItemPosDataList_OnScroll;


            ItemCollectionDelegate itemCollectionDelegate = new ItemCollectionDelegate();
            itemCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (int)indexPath.Item;
                Utils.SetTitle(this.NavigationController ,Utils.TextBundle("edititem", "edititem"));
                AddItemControllerScroll Edititem = new AddItemControllerScroll(Items[x]);
                this.NavigationController.PushViewController(Edititem, false);
            };
            itemListCollectionView.Delegate = itemCollectionDelegate;
            itemListCollectionView.DataSource = itemPosDataList;
            ItemView.AddSubview(itemListCollectionView);
            #endregion
            #region GoToAddItem
            btnAddItem = new UIImageView();
            btnAddItem.Image = UIImage.FromBundle("Add");
            btnAddItem.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAddItem.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddItem:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddItem.AddGestureRecognizer(tapGesture);

            ItemView.AddSubview(btnAddItem);
            #endregion
            #endregion

            #region CatagoryLayout
            #region emptyCategoryView
            emptyCategoryView = new UIImageView();
            emptyCategoryView.Hidden = true;
            emptyCategoryView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyCategoryView.Image = UIImage.FromBundle("DefaultCategory");
            emptyCategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            CategoryView.AddSubview(emptyCategoryView);

            lbl_empty_Category = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_Category.Hidden = true;
            lbl_empty_Category.Lines = 2;
            lbl_empty_Category.Font = lbl_empty_Item.Font.WithSize(16);
            lbl_empty_Category.Text = Utils.TextBundle("nullcategory", "nullcategory");
            CategoryView.AddSubview(lbl_empty_Category);
            #endregion
            #region CatagoryListCollectionView

            UICollectionViewFlowLayout CategoryflowLayoutList = new UICollectionViewFlowLayout();
            CategoryflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 80, height: 80);
            CategoryflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            CategoryflowLayoutList.MinimumLineSpacing = 0;

            CatagoryListCollectionView = new UICollectionView(frame: View.Frame, layout: CategoryflowLayoutList);
            CatagoryListCollectionView.BackgroundColor = UIColor.White;
            CatagoryListCollectionView.ShowsVerticalScrollIndicator = false;
            CatagoryListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CatagoryListCollectionView.RegisterClassForCell(cellType: typeof(itemCatagoryCollectionViewCellList), reuseIdentifier: "itemCatagoryCellList");

            catagoryDataList = new ItemCatagoeyDataSourceList(Categories);
            catagoryDataList.OnCardCellDelete += ItemCategoryDataList_OnCardCellDelete;
            catagoryDataList.OnScroll += ItemPosDataList_OnScroll;

            ItemCatagoryCollectionDelegate itemCatCollectionDelegate = new ItemCatagoryCollectionDelegate();
            itemCatCollectionDelegate.OnItemSelected += (indexPath) => {
                var x = indexPath.Row;
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "view", "category");
                if (check)
                {
                    ItemsAddCategoryController addCategory = new ItemsAddCategoryController(Categories[x].SysCategoryID);
                    this.NavigationController.PushViewController(addCategory, false);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                }
            };
            CatagoryListCollectionView.Delegate = itemCatCollectionDelegate;
            CatagoryListCollectionView.DataSource = catagoryDataList;
            CategoryView.AddSubview(CatagoryListCollectionView);
            #endregion
            #region GoToAddCatagory
            btnAddCatagory = new UIImageView();
            btnAddCatagory.Image = UIImage.FromBundle("Add");
            btnAddCatagory.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAddCatagory.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddCategory:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddCatagory.AddGestureRecognizer(tapGesture1);

            CategoryView.AddSubview(btnAddCatagory);
            #endregion
            #endregion

            #region ToppingLayout
            #region emptyExtraToppingView
            emptyExtraToppingView = new UIImageView();
            emptyExtraToppingView.Hidden = true;
            
            emptyExtraToppingView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyExtraToppingView.Image = UIImage.FromBundle("DefaultTopping");
            emptyExtraToppingView.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingView.AddSubview(emptyExtraToppingView);
            

            lbl_empty_ExtaTopping = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_ExtaTopping.Hidden = true;
            lbl_empty_ExtaTopping.Lines = 2;
            lbl_empty_ExtaTopping.Font = lbl_empty_ExtaTopping.Font.WithSize(16);
            lbl_empty_ExtaTopping.Text = Utils.TextBundle("nullextratopping", "nullextratopping");
            ToppingView.AddSubview(lbl_empty_ExtaTopping);
            #endregion
            #region ToppingListCollectionView
            UICollectionViewFlowLayout ToppingflowLayoutList = new UICollectionViewFlowLayout();
            ToppingflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 80, height: 80);
            ToppingflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            ToppingflowLayoutList.MinimumLineSpacing = 0;

            ToppingCollectionView = new UICollectionView(frame: View.Frame, layout: ToppingflowLayoutList);
            ToppingCollectionView.BackgroundColor = UIColor.White;
            ToppingCollectionView.ShowsVerticalScrollIndicator = false;
            ToppingCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingCollectionView.RegisterClassForCell(cellType: typeof(CellItemToppingList), reuseIdentifier: "cellItemToppingList");

            ToppingDataList = new ItemToppingDataSourceList(Topping, stock, Categories);
            ToppingDataList.OnCardCellDelete += ItemToppingDataList_OnCardCellDelete;
            ToppingDataList.OnCardCellFav += ItemToppingDataList_OnCardCellFav;
            ToppingDataList.OnScroll2 += ToppingDataList_OnScroll2;

            ItemExtraToppingCollectionDelegate itemExtraToppingCollectionDelegate = new ItemExtraToppingCollectionDelegate();
            itemExtraToppingCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (int)indexPath.Item;
                
                //     Ismodify = true;
                ItemsAddToppingController addTopping = new ItemsAddToppingController(Topping[x]);
                this.NavigationController.PushViewController(addTopping, false);
            };
            ToppingCollectionView.Delegate = itemExtraToppingCollectionDelegate;
            ToppingCollectionView.DataSource = ToppingDataList;
            ToppingView.AddSubview(ToppingCollectionView);
            #endregion
            #region GotoAddTopping
            btnAddTopping = new UIImageView();
            btnAddTopping.Image = UIImage.FromBundle("Add");
            btnAddTopping.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAddTopping.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddTopping:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddTopping.AddGestureRecognizer(tapGesture2);

            ToppingView.AddSubview(btnAddTopping);
            #endregion
            #endregion

            #region itemStockLayout
            #region emptyItemView
            emptyItemStockView = new UIImageView();
            emptyItemStockView.Hidden = true;
            emptyItemStockView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyItemStockView.Image = UIImage.FromBundle("DefaultItem");
            emptyItemStockView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemView.AddSubview(emptyItemStockView);

            lbl_empty_ItemStock = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_ItemStock.Hidden = true;
            lbl_empty_ItemStock.Lines = 2;
            lbl_empty_ItemStock.Font = lbl_empty_ItemStock.Font.WithSize(16);
            lbl_empty_ItemStock.Text = Utils.TextBundle("nullitem", "nullitem");
            ItemView.AddSubview(lbl_empty_ItemStock);
            #endregion
            #region itemListCollectionView

            txtStockRevision = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txtStockRevision.Font = txtStockRevision.Font.WithSize(10);
            txtStockRevision.Text = "";
            ItemStockView.AddSubview(txtStockRevision);

            UICollectionViewFlowLayout itemflowLayoutList2 = new UICollectionViewFlowLayout();
            itemflowLayoutList2.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 80, height: 80);
            itemflowLayoutList2.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList2.MinimumLineSpacing = 0;

            itemStockListCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList2);
            itemStockListCollectionView.BackgroundColor = UIColor.White;
            itemStockListCollectionView.ShowsVerticalScrollIndicator = false;
            itemStockListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            itemStockListCollectionView.RegisterClassForCell(cellType: typeof(CellItemStockList), reuseIdentifier: "itemStockPosCellList");

            itemStockPosDataList = new ItemStockDataSourceList(ItemsStock, stock, Categories);
            itemStockPosDataList.OnScroll2 += ItemStockPosDataList_OnScroll2;
            //itemStockPosDataList.OnCardCellDelete += ItemPosDataList_OnCardCellDelete;
            //itemStockPosDataList.OnCardCellFav += ItemPosDataList_OnCardCellFav;

            ItemStockCollectionDelegate itemStockCollectionDelegate = new ItemStockCollectionDelegate();
            itemStockCollectionDelegate.OnItemSelected += (indexPath) => {

                var x = (int)indexPath.Item;
                if (ItemsStock[x].SaleItemType != 'T')
                {
                    AddItemControllerScroll Edititem = new AddItemControllerScroll(ItemsStock[x]);
                    Edititem.openstock = true;
                    this.NavigationController.PushViewController(Edititem, false);
                }
                else
                {
                    ItemsAddToppingController Edititemtopping = new ItemsAddToppingController(ItemsStock[x]);
                    Edititemtopping.openstock = true;
                    this.NavigationController.PushViewController(Edititemtopping, false);
                }
                


            };
            itemStockListCollectionView.Delegate = itemStockCollectionDelegate;
            itemStockListCollectionView.DataSource = itemStockPosDataList;
            ItemStockView.AddSubview(itemStockListCollectionView);
            await GetLastRevisionNoStock();
            #endregion
            //#region GoToAddItem
            //btnAddItem = new UIImageView();
            //btnAddItem.Image = UIImage.FromBundle("Add");
            //btnAddItem.TranslatesAutoresizingMaskIntoConstraints = false;

            //btnAddItem.UserInteractionEnabled = true;
            //var tapGesture2 = new UITapGestureRecognizer(this,
            //        new ObjCRuntime.Selector("AddItem:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //btnAddItem.AddGestureRecognizer(tapGesture2);

            //ItemStockView.AddSubview(btnAddItem);
            //#endregion

            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "topping");
            if (!check)
            {
                btnAddItem.Alpha = 0.5f;
                btnAddCatagory.Alpha = 0.5f;
                btnAddTopping.Alpha = 0.5f;
            }


                #endregion
            }

        private void ItemStockPosDataList_OnScroll2()
        {
            View.EndEditing(true);
        }
        async Task GetLastRevisionNoStock()
        {
            try
            {
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
                SystemRevisionNo RevisionNoStock = new SystemRevisionNo();

                RevisionNoStock = await systemRevisionNoManage.GetSystemRevisionNo(DataCashingAll.MerchantId, 31);
                if (RevisionNoStock != null)
                {
                    LastRevisionNoStock = RevisionNoStock.LastRevisionNo;
                }

                txtStockRevision.Text = Utils.TextBundle("revisionno", "revisionno") + LastRevisionNoStock;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetLastRevisionNoStock at Item");
            }
        }
        private void ToppingDataList_OnScroll2()
        {
            View.EndEditing(true);
        }

        private void ItemPosDataList_OnScroll()
        {
            View.EndEditing(true);
        }

        private void ItemPosDataList_OnCardCell(NSIndexPath indexPath)
        {
            if (checknet)
            {
                var item = Items?.Where(x => x.SysItemID == Items[(int)indexPath.Row].SysItemID).FirstOrDefault();
                if (!string.IsNullOrEmpty(item?.PicturePath))
                {
                    GabanaShowImage.SharedInstance.Show(this, item.PicturePath, "UserInactive.png");
                }
            }
            

            //GabanaShowImage.SharedInstance.Show(this, item.)
        }

        private void TxtSearch_EditingChanged(object sender, EventArgs e)
        {
            btnSearch.SetImage(UIImage.FromBundle("DelTxt"), UIControlState.Normal);
        }

        #region Add btn
        [Export("AddItem:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
            if (check)
            {
                AddItemControllerScroll additem = new AddItemControllerScroll();
                this.NavigationController.PushViewController(additem, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("AddTopping:")]
        public void AddTopping(UIGestureRecognizer sender)
        {
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                if (check)
                {
                    itemsAddTopping = new ItemsAddToppingController();
                    this.NavigationController.PushViewController(itemsAddTopping, false);
                }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("AddCategory:")]
        public void AddCategory(UIGestureRecognizer sender)
        {
                    var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "item");
                    if (check)
                    {
                        itemsAddCategory = new ItemsAddCategoryController();
                        this.NavigationController.PushViewController(itemsAddCategory, false);
                    }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        #endregion
        private async void ItemToppingDataList_OnCardCellFav(NSIndexPath indexPath)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                var item = Topping?.Where(x => x.SysItemID == Topping[(int)indexPath.Row].SysItemID).FirstOrDefault();
                if (item.FavoriteNo == 0)
                {
                    item.FavoriteNo = 1;
                }
                else
                {
                    item.FavoriteNo = 0;
                }

                item.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                item.UserLastModified = usernamelogin;
                item.DataStatus = 'M';
                item.FWaitSending = 1;
                item.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                item.TrackStockDateTime = Utils.GetTranDate(item.TrackStockDateTime);
                ItemManage ItemManage = new ItemManage();
                var result = await ItemManage.UpdateItem(item);
                if (result)
                {
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "editdatasuccessfully"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                    else
                    {
                        item.FWaitSending = 2;
                        await ItemManage.UpdateItem(item);
                    }
                    SearchBytxt();

                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                    GabanaLoading.SharedInstance.Hide();
                    return;
                }
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "cannotdelete"));
                GabanaLoading.SharedInstance.Hide();
                await TinyInsights.TrackErrorAsync(ex);
                return;
               
            }
        }
        private async void ItemPosDataList_OnCardCellFav(NSIndexPath indexPath)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                var item = Items?.Where(x => x.SysItemID == Items[(int)indexPath.Row].SysItemID).FirstOrDefault();
                if (item.FavoriteNo == 0)
                {
                    item.FavoriteNo = 1;
                }
                else
                {
                    item.FavoriteNo = 0;
                }

                item.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                item.UserLastModified = usernamelogin;
                item.DataStatus = 'M';
                item.FWaitSending = 1;
                item.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                item.TrackStockDateTime = Utils.GetTranDate(item.TrackStockDateTime);
                ItemManage ItemManage = new ItemManage();
                var result = await ItemManage.UpdateItem(item);
                if (result)
                {
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "editdatasuccessfully"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                    else
                    {
                        item.FWaitSending = 2;
                        await ItemManage.UpdateItem(item);
                    }
                    SearchBytxt();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                    GabanaLoading.SharedInstance.Hide();
                    return;
                }
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "cannotdelete"));
                GabanaLoading.SharedInstance.Hide();
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void ItemPosDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "item");
            if (check)
            {
                var x = (int)indexPath.Row;
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "wanttodelete"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => deleteItem(x)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }

        }
        private void ItemCategoryDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "category");
            if (check)
            {
                var x = (int)indexPath.Row;

                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "wanttodelete"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => deleteCategories(x)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        private async void ItemToppingDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "topping");
            if (check)
            {
                var x = (int)indexPath.Row;
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "wanttodelete"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => deleteTopping(x)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        private async void deleteItem(int position)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                var itemDelete = Items[position];
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 1;
                itemDelete.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                var result = await itemmanager.UpdateItem(itemDelete);
                if (result)
                {
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "deletesuccessfully"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, (int)itemDelete.SysItemID);
                    }
                    

                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                }
                SearchBytxt();
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                await TinyInsights.TrackErrorAsync(ex);
                GabanaLoading.SharedInstance.Hide();
                return;
            }
        }
        private async void deleteCategories(int position)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                var cusdelete = Categories[position];
                var cateDelte = await CategoryManager.GetCategory((int)DataCashingAll.MerchantId, (int)cusdelete.SysCategoryID);
                var UpdateItem = await itemmanager.GetItembyCategory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                if (UpdateItem != null)
                {
                    foreach (var update in UpdateItem)
                    {
                        update.SysCategoryID = null;
                        var resultUpdate = await itemmanager.UpdateItem(update);
                    }
                }
                cateDelte.DataStatus = 'D';
                cateDelte.FWaitSending = 1;
                cateDelte.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                var updateCate = await CategoryManager.UpdateCategory(cateDelte);
                if (updateCate)
                {

                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "deletesuccessfully"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCatagory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                    }
                    else
                    {
                        cateDelte.FWaitSending = 2;
                        await CategoryManager.UpdateCategory(cateDelte);
                    }
                    SearchBytxt();
                    GabanaLoading.SharedInstance.Hide();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                    GabanaLoading.SharedInstance.Hide();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                GabanaLoading.SharedInstance.Hide();
            }
        }
        private async void deleteTopping(int position)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                ItemManage itemManage = new ItemManage();
                var itemDelete = Topping[position];
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 1;
                itemDelete.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                var result = await itemManage.UpdateItem(itemDelete);
                if (result)
                {
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "deletesuccessfully"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, (int)itemDelete.SysItemID);
                    }
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                    
                }
                SearchBytxt();
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
                await TinyInsights.TrackErrorAsync(ex);
                GabanaLoading.SharedInstance.Hide();
                return;
            }
            
        }

        public async Task SearchBytxt()
        {
            Items = await GetFilterItemList();
            stock = await ItemOnBranchManager.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
            Categories = await CategoryManager.GetAllCategory();
            Topping = await GetFilterExtraTopping();
            var allitem = new List<Item>();
            if (Items != null)
            {
                allitem.AddRange(Items);
            }
            if (Topping != null)
            {
                allitem.AddRange(Topping);
            }
            ItemsStock = new List<Item>(allitem.Where(x => x.FTrackStock == 1).OrderBy(x => x.ItemName).ToList()); 
            
            if (MenuSelected == 0) // search item
            {
                //Items = await GetFilterItemList();
                //itemListCollectionView.DataSource = new ItemDataSourceList(Items, stock,Categories);
                ((ItemDataSourceList)itemListCollectionView.DataSource).ReloadData(Items, stock, Categories);
                itemListCollectionView.ReloadData();
                
                if (DataCaching.NewItem != 0)
                {
                    var index = Items.FindIndex(x => x.SysItemID == DataCaching.NewItem);
                    if (index >= 0) itemListCollectionView.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.CenteredVertically ,  false);

                    DataCaching.NewItem = 0; 
                }
                if (Items == null || Items.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    itemListCollectionView.Hidden = true;
                    emptyItemView.Hidden = false;
                    lbl_empty_Item.Hidden = false;

                }
                else
                {
                    //SearchbarView.Hidden = false;
                    itemListCollectionView.Hidden = false;
                    emptyItemView.Hidden = true;
                    lbl_empty_Item.Hidden = true;

                }

            }
            else if (MenuSelected == 1) // search topping
            {
                //Categories = await CategoryManager.GetAllCategory();
                //Topping = await GetFilterExtraTopping();
                //ToppingCollectionView.DataSource = new ItemToppingDataSourceList(Topping, stock, Categories);
                ((ItemToppingDataSourceList)ToppingCollectionView.DataSource).ReloadData(Topping, stock, Categories);
                ToppingCollectionView.ReloadData();

                if (Topping == null || Topping.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    ToppingCollectionView.Hidden = true;
                    emptyExtraToppingView.Hidden = false;
                    lbl_empty_ExtaTopping.Hidden = false;

                }
                else
                {
                    //SearchbarView.Hidden = false;
                    ToppingCollectionView.Hidden = false;
                    emptyExtraToppingView.Hidden = true;
                    lbl_empty_ExtaTopping.Hidden = false;

                }
            }
            else if (MenuSelected == 2) // search topping
            {
                //Items = await GetFilterItemList();
                //ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                //itemStockListCollectionView.DataSource = new ItemStockDataSourceList(ItemsStock, stock, Categories);
                ((ItemStockDataSourceList)itemStockListCollectionView.DataSource).ReloadData(ItemsStock, stock, Categories);
                itemStockListCollectionView.ReloadData();

                if (ItemsStock == null || ItemsStock.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    txtStockRevision.Hidden = true;
                    itemStockListCollectionView.Hidden = true;
                    emptyItemStockView.Hidden = false;
                    lbl_empty_ItemStock.Hidden = false;

                }
                else
                {
                    //SearchbarView.Hidden = false;
                    txtStockRevision.Hidden = false;
                    itemStockListCollectionView.Hidden = false;
                    emptyItemStockView.Hidden = true;
                    lbl_empty_ItemStock.Hidden = true;

                }
            }
            else // search category
            {
                Categories = await GetFilterCategory();
                //CatagoryListCollectionView.DataSource = new ItemCatagoeyDataSourceList(Categories);
                ((ItemCatagoeyDataSourceList)CatagoryListCollectionView.DataSource).ReloadData(Categories);
                CatagoryListCollectionView.ReloadData();

                if (Categories == null || Categories.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    CatagoryListCollectionView.Hidden = true;
                    emptyCategoryView.Hidden = false;
                    lbl_empty_Category.Hidden = false;

                }
                else
                {
                    //SearchbarView.Hidden = false;
                    CatagoryListCollectionView.Hidden = false;
                    emptyCategoryView.Hidden = true;
                    lbl_empty_Category.Hidden = true;
                }
            }
            resetCell();
            
        }
        
        async Task<List<Item>> GetFilterItemList()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await itemmanager.GetAllItem();
                    return itemlst;
                }
                var itm = await itemmanager.GetItemSearch(txtSearch.Text);
                if (itm == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                    return null;
                }
                return itm;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                return null;
            }
        }
        async Task<List<Category>> GetFilterCategory()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await CategoryManager.GetAllCategory();
                    return itemlst;
                }
                var CategorySerch = await CategoryManager.GetCategorySearch(txtSearch.Text);
                if(CategorySerch == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                    return null;
                }
                return CategorySerch;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                return null;
            }
        }
        async Task<List<Item>> GetFilterExtraTopping()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await itemmanager.GetToppingItem();
                    return itemlst;
                }
                var toppSerch = await itemmanager.getExtraToppingSearch(txtSearch.Text);
                if (toppSerch == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                    return null;
                }
                return toppSerch;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                return null;
            }
        }
        async Task showList()
        {
            if (MenuSelected == 0)
            { // item
                ItemView.Hidden = false;
                CategoryView.Hidden = true;
                ToppingView.Hidden = true;
                ItemStockView.Hidden = true;

                itemListCollectionView.Hidden = true;
                emptyItemView.Hidden = true;
                lbl_empty_Item.Hidden = true;

                ToppingCollectionView.Hidden = true;
                emptyExtraToppingView.Hidden = true;
                lbl_empty_ExtaTopping.Hidden = true;

                txtStockRevision.Hidden = true;
                itemStockListCollectionView.Hidden = true;
                emptyItemStockView.Hidden = true;
                lbl_empty_ItemStock.Hidden = true;

                CatagoryListCollectionView.Hidden = true;
                emptyCategoryView.Hidden = true;
                lbl_empty_Category.Hidden = true;

                Items = await GetFilterItemList();
                if ( Items == null || Items.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    itemListCollectionView.Hidden = true;
                    emptyItemView.Hidden = false;
                    lbl_empty_Item.Hidden = false;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    //Items = await GetFilterItemList();
                    //itemListCollectionView.DataSource = new ItemDataSourceList(Items, stock, Categories);
                    ((ItemDataSourceList)itemListCollectionView.DataSource).ReloadData(Items, stock, Categories);
                    itemListCollectionView.ReloadData();
                }
                else
                {
                    //SearchbarView.Hidden = false;
                    itemListCollectionView.Hidden = false;
                    emptyItemView.Hidden = true;
                    lbl_empty_Item.Hidden = true;
                    //itemListCollectionView.DataSource = new ItemDataSourceList(Items, stock, Categories);
                    ((ItemDataSourceList)itemListCollectionView.DataSource).ReloadData(Items, stock, Categories);
                    itemListCollectionView.ReloadData();
                }
                

                ItemView.Hidden = false;
                CategoryView.Hidden = true;
                ToppingView.Hidden = true;
                ItemStockView.Hidden = true;


            }
            else if (MenuSelected == 1)
            { //extra 
                
                ItemView.Hidden = true;
                CategoryView.Hidden = true;
                ToppingView.Hidden = false;
                ItemStockView.Hidden = true;

                itemListCollectionView.Hidden = true;
                emptyItemView.Hidden = true;
                lbl_empty_Item.Hidden = true;

                ToppingCollectionView.Hidden = true;
                emptyExtraToppingView.Hidden = true;
                lbl_empty_ExtaTopping.Hidden = true;

                txtStockRevision.Hidden = true;
                itemStockListCollectionView.Hidden = true;
                emptyItemStockView.Hidden = true;
                lbl_empty_ItemStock.Hidden = true;

                CatagoryListCollectionView.Hidden = true;
                emptyCategoryView.Hidden = true;
                lbl_empty_Category.Hidden = true;
                Categories = await CategoryManager.GetAllCategory();
                Topping = await GetFilterExtraTopping();
                if ( Topping==null || Topping.Count ==0)
                {
                    //SearchbarView.Hidden = true;
                    ToppingCollectionView.Hidden = true;
                    emptyExtraToppingView.Hidden = false;
                    lbl_empty_ExtaTopping.Hidden = false;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    
                    //ToppingCollectionView.DataSource = new ItemToppingDataSourceList(Topping, stock, Categories);
                    ((ItemToppingDataSourceList)ToppingCollectionView.DataSource).ReloadData(Topping, stock, Categories);
                    ToppingCollectionView.ReloadData();
                }
                else
                {
                    //SearchbarView.Hidden = false;
                    ToppingCollectionView.Hidden = false;
                    emptyExtraToppingView.Hidden = true;
                    lbl_empty_ExtaTopping.Hidden = false;
                    //ToppingCollectionView.DataSource = new ItemToppingDataSourceList(Topping, stock, Categories);
                    ((ItemToppingDataSourceList)ToppingCollectionView.DataSource).ReloadData(Topping, stock, Categories);
                    ToppingCollectionView.ReloadData();
                }
            }
            else if (MenuSelected == 2)
            { //extra 
               
                ItemView.Hidden = true;
                CategoryView.Hidden = true;
                ToppingView.Hidden = true;
                ItemStockView.Hidden = false;
                itemListCollectionView.Hidden = true;
                emptyItemView.Hidden = true;
                lbl_empty_Item.Hidden = true;

                ToppingCollectionView.Hidden = true;
                emptyExtraToppingView.Hidden = true;
                lbl_empty_ExtaTopping.Hidden = true;

                txtStockRevision.Hidden = true;
                itemStockListCollectionView.Hidden = true;
                emptyItemStockView.Hidden = true;
                lbl_empty_ItemStock.Hidden = true;

                CatagoryListCollectionView.Hidden = true;
                emptyCategoryView.Hidden = true;
                lbl_empty_Category.Hidden = true;

                Items = await GetFilterItemList();
                //ItemsStock = new List<Item>(Items.Where(x => x.FTrackStock == 1).ToList());
                Topping = await GetFilterExtraTopping();
                var allitem = new List<Item>();

                if (Items != null)
                {
                    allitem.AddRange(Items);
                }
                if (Topping != null)
                {
                    allitem.AddRange(Topping);
                }
                ItemsStock = new List<Item>(allitem.Where(x => x.FTrackStock == 1).OrderBy(x => x.ItemName).ToList()) ;
                stock = await ItemOnBranchManager.GetAllItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);
                if (Items == null || Items.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    txtStockRevision.Hidden = true;
                    itemStockListCollectionView.Hidden = true;
                    emptyItemStockView.Hidden = false;
                    lbl_empty_ItemStock.Hidden = false;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.ItemOnBranchChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    
                    //itemStockListCollectionView.DataSource = new ItemStockDataSourceList(ItemsStock, stock, Categories);
                    ((ItemStockDataSourceList)itemStockListCollectionView.DataSource).ReloadData(ItemsStock, stock, Categories);
                    itemStockListCollectionView.ReloadData();
                }
                else
                {
                    //SearchbarView.Hidden = false;
                    txtStockRevision.Hidden = false;
                    itemStockListCollectionView.Hidden = false;
                    emptyItemStockView.Hidden = true;
                    lbl_empty_ItemStock.Hidden = true;
                    //itemStockListCollectionView.DataSource = new ItemStockDataSourceList(ItemsStock, stock, Categories);
                    ((ItemStockDataSourceList)itemStockListCollectionView.DataSource).ReloadData(ItemsStock, stock, Categories);
                    itemStockListCollectionView.ReloadData();
                }
                await GetLastRevisionNoStock();
            }
            else
            { // category
                
                ItemView.Hidden = true;
                CategoryView.Hidden = false;
                ToppingView.Hidden = true;
                ItemStockView.Hidden = true;

                itemListCollectionView.Hidden = true;
                emptyItemView.Hidden = true;
                lbl_empty_Item.Hidden = true;

                ToppingCollectionView.Hidden = true;
                emptyExtraToppingView.Hidden = true;
                lbl_empty_ExtaTopping.Hidden = true;

                txtStockRevision.Hidden = true;
                itemStockListCollectionView.Hidden = true;
                emptyItemStockView.Hidden = true;
                lbl_empty_ItemStock.Hidden = true;

                CatagoryListCollectionView.Hidden = true;
                emptyCategoryView.Hidden = true;
                lbl_empty_Category.Hidden = true;

                Categories = await GetFilterCategory();
                if ( Categories ==null || Categories.Count == 0)
                {
                    //SearchbarView.Hidden = true;
                    CatagoryListCollectionView.Hidden = true;
                    emptyCategoryView.Hidden = false;
                    lbl_empty_Category.Hidden = false;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.CategoryChange();
                    }
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    
                    //CatagoryListCollectionView.DataSource = new ItemCatagoeyDataSourceList(Categories);
                    ((ItemCatagoeyDataSourceList)CatagoryListCollectionView.DataSource).ReloadData(Categories);
                    CatagoryListCollectionView.ReloadData();
                }
                else
                {
                    //searchbarView.Hidden = false;
                    CatagoryListCollectionView.Hidden = false;
                    emptyCategoryView.Hidden = true;
                    lbl_empty_Category.Hidden = true;
                    //CatagoryListCollectionView.DataSource = new ItemCatagoeyDataSourceList(Categories);
                    ((ItemCatagoeyDataSourceList)CatagoryListCollectionView.DataSource).ReloadData(Categories);
                    CatagoryListCollectionView.ReloadData();
                }
            }
        }
        void setupAutoLayout()
        {
            menuItemBarCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            menuItemBarCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            menuItemBarCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            menuItemBarCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(menuItemBarCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor,-15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            btnScan.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnScan.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnScan.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnScan.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region itemLayout
            ItemView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ItemView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ItemView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ItemView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            #region emptyItemView
            emptyItemView.TopAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyItemView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyItemView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyItemView.CenterXAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_Item.TopAnchor.ConstraintEqualTo(emptyItemView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_Item.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_Item.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_Item.CenterXAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            itemListCollectionView.TopAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            itemListCollectionView.BottomAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            itemListCollectionView.RightAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            itemListCollectionView.LeftAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddItem.BottomAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            btnAddItem.RightAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            btnAddItem.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAddItem.HeightAnchor.ConstraintEqualTo(45).Active = true;
            #endregion

            #region CategoryLayout
            CategoryView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CategoryView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CategoryView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CategoryView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            #region emptyCategoryView
            emptyCategoryView.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyCategoryView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyCategoryView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyCategoryView.CenterXAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_Category.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_Category.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_Category.TopAnchor.ConstraintEqualTo(emptyCategoryView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_Category.CenterXAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            CatagoryListCollectionView.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            CatagoryListCollectionView.BottomAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CatagoryListCollectionView.RightAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CatagoryListCollectionView.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddCatagory.BottomAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            btnAddCatagory.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAddCatagory.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAddCatagory.RightAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            #endregion

            #region ToppingLayout
            ToppingView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ToppingView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ToppingView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ToppingView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            #region emptyToppingView
            emptyExtraToppingView.TopAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyExtraToppingView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyExtraToppingView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyExtraToppingView.CenterXAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_ExtaTopping.TopAnchor.ConstraintEqualTo(emptyExtraToppingView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_ExtaTopping.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_ExtaTopping.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_ExtaTopping.CenterXAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            ToppingCollectionView.TopAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            ToppingCollectionView.BottomAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ToppingCollectionView.RightAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ToppingCollectionView.LeftAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddTopping.BottomAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            btnAddTopping.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAddTopping.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAddTopping.RightAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            #endregion

            #region itemstockLayout
            ItemStockView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ItemStockView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ItemStockView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ItemStockView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            #region emptyItemView
            emptyItemStockView.TopAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyItemStockView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyItemStockView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyItemStockView.CenterXAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_ItemStock.TopAnchor.ConstraintEqualTo(emptyItemStockView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_ItemStock.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_ItemStock.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_ItemStock.CenterXAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion
            txtStockRevision.TopAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            txtStockRevision.HeightAnchor.ConstraintEqualTo(12).Active = true;
            txtStockRevision.RightAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            txtStockRevision.LeftAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            itemStockListCollectionView.TopAnchor.ConstraintEqualTo(txtStockRevision.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            itemStockListCollectionView.BottomAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            itemStockListCollectionView.RightAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            itemStockListCollectionView.LeftAnchor.ConstraintEqualTo(ItemStockView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

           
            #endregion
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        [Export("showimage:")]
        public void Close2(UIGestureRecognizer sender)
        {
           GabanaShowImage.SharedInstance.Hide();
        }

    }
}