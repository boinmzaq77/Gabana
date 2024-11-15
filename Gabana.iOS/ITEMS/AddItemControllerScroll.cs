using CoreGraphics;
using Foundation;
using Gabana.CustomClass;
using Gabana.iOS.ITEMS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using LinqToDB.Common;
using Newtonsoft.Json;
using Photos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Gabana.iOS
{
    public partial class AddItemControllerScroll : UIViewController
    {
        public int idCategory;
        List<ItemExSize> newlsItemExSize = new List<ItemExSize>();
        UICollectionView menuAddItemBarCollectionView;
        Gabana.ShareSource.Manage.ItemManage itemManager = new Gabana.ShareSource.Manage.ItemManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        int flagBtn = 0;
        #region ItemViewComponent
        UIView  _contentView;
        UIScrollView _scrollView;
        ItemOnBranch itemOnBranch;
        int SizeHeight = 0;
        public static int flagDetail = 0;
        private static byte[] picture;
        MenuItemDataSource menuAddItemDataSource;
        UIView bottomView;
        UIButton btnAdd;
        Item sysItemIDEdit;
        long SysItemId;
        bool checkManageStock;
        bool favFlag = false;
        UICollectionView ExtraSizeCollectionView;
        public static long addColor;
        private bool changecolor;
        public static long CatID;
        UIView btnDeleteView;
        UIImageView btnDelete, imgSelectStockmove;
        public List<Category> CatList;
        UIImageView favImg;
        UILabel lblFav;
        UIButton btnToggleDetail, btnSelectVatType,btnAddSize;
        UIButton btnSelectCategory;
        UIView ItemCodeView, CategoryView, VatView, CostView, DetailClickView, viewstockmove;
        UILabel lblDetail, lblCategory, lblVat;
        public static UITextField lblVatMode;
        public static UITextField lblSelectedCategory, txtNote;
        public static UITextField txtItemCode, txtItemCost;
        UILabel lblItemCode, lblItemCost,lblSize,lblMaxSize,lblDiaplayPOS,lblDisplaytext;
        UISwitch DisplaySwitch;
        UIImagePickerController imagePicker;
        UIAlertController selectPhotoMenuSheet;
        UIView imageView, setColorView, setItemNameView, SetItemPriceView;
        UIView itemCardFooter, SizeView,DisplayPOSView, line4;
        UIImageView itemCardView, itemCardViewTop;
        UILabel lblItemName, lblItemPrice;
        UIImage editedImage;
        public List<Gabana.ORM.MerchantDB.ItemExSize> extraList = new List<Gabana.ORM.MerchantDB.ItemExSize>();
        public static int extraListCount = 0;
        public static UILabel lblItemCardName, LblItemCardPrice;
        public static UITextField txtItemName, txtItemPrice;
        ItemExSizeManage extra = new ItemExSizeManage();
        public List<Item> NoteList;
        UIButton btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
        bool edit = false; 
        #endregion
        #region StockViewComponent
        UIView StockbarView, line3, disableView,stockView, lineEmpty;
        UILabel lblStock, lblStockMinimum, lblStockmove, lbltxtOnHand;
        UITextField lblStockNumber, lblItemCardShortName;
        UITextField txtStockMinimum;

        MerchantManage merchantManage = new MerchantManage();
        UISwitch switchStock;
        int MenuSelected = 0;

        List<MenuitemHeaderIOS> Menu = new List<MenuitemHeaderIOS>();

        public static bool isModifyOnhand = false;
        public static long onHand = 0;
        #endregion
        Gabana.ORM.MerchantDB.Item addItem = new Gabana.ORM.MerchantDB.Item();
        private ItemOnBranch itemOnBranchedit;
        private bool feditstock = false;
        ItemManage ItemManage;
        private decimal fstockbefore ;
        internal int SelectCategory;
        private UIButton btnScan;
        internal static bool isScanBarcode;
        internal static string txtBarcodeScan;
        private UIView Viewblock;
        private bool Editchange = false;
        public  bool openstock;
        private List<ItemExSize> itemExSizes;
        public AddItemControllerScroll()
        {
            
        }
        public AddItemControllerScroll(bool fav)
        {
            favFlag = fav;
        }
        public AddItemControllerScroll(Item sys)
        {
            addItem = sys; 
            sysItemIDEdit = sys;
            SysItemId = sys.SysItemID;
            edit = true; 
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                if (edit)
                {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("edititem", "edititem"));
                }
                else
                {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("additem", "additem"));
                }
           
                this.NavigationController.SetNavigationBarHidden(false, false);
                if (isModifyOnhand)
                {
                    lblStockNumber.Text = onHand.ToString();
                    feditstock = true;
                    isModifyOnhand = false;
                }
                if (isScanBarcode == true)
                {
                    isScanBarcode = false;
                    txtItemCode.Text = txtBarcodeScan;
                    //SearchBytxt();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("havechage", "havechage"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    async alert => await BtnSave_Click3()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => this.NavigationController.PopViewController(true)));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                this.NavigationController.PopViewController(true);
            }

        }
        private async Task BtnSave_Click3()
        {
            try
            {
                btnAdd.Enabled = false;
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");
                ItemManage a = new ItemManage();
                var it = await a.GetAllItem();

                if (string.IsNullOrWhiteSpace(txtItemName.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("enteritemname", "enteritemname"));
                    btnAdd.Enabled = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtItemPrice.Text.Remove(0, 2)))
                {
                    Utils.ShowMessage(Utils.TextBundle("enteritemprice", "enteritemprice"));
                    btnAdd.Enabled = true;
                    return;
                }


                addItem.MerchantID = MainController.merchantlocal.MerchantID;
                if (!edit)
                {
                    addItem.SysItemID = long.Parse(sys);
                }
                //addItem.ItemName = txtItemName.Text.ToString();
                addItem.FavoriteNo = 0;
                if (txtItemCost.Text != null && txtItemCost.Text != "")
                {
                    if (string.IsNullOrEmpty(txtItemCost.Text.Remove(0, 2)))
                    {
                        addItem.EstimateCost = 0;
                    }
                    else
                    {
                        addItem.EstimateCost = Convert.ToDecimal(txtItemCost.Text.Remove(0, 2));
                    }
                }
                else
                {
                    addItem.EstimateCost = 0;
                }

                //addItem.ItemCode = txtItemCode.Text ?? "";
                addItem.Price = Convert.ToDecimal(txtItemPrice.Text.Remove(0, 2));
                long? category = null;
                if (lblSelectedCategory.Text == Utils.TextBundle("none", "none"))
                {
                    category = null;
                }
                else
                {
                    category = idCategory;
                }
                addItem.SysCategoryID = category;
                addItem.OptSalePrice = 'F';
                if (lblVatMode.Text == Utils.TextBundle("havevat", "havevat"))
                {
                    addItem.TaxType = 'V'; // addItem.Vat
                }
                if (lblVatMode.Text == Utils.TextBundle("nonevat", "None Vat"))
                {
                    addItem.TaxType = 'N'; // addItem.Vat
                }
                addItem.UnitName = null;
                addItem.RegularSizeName = null;

                addItem.Ordinary = 2;
                addItem.SellBy = 'U';
                addItem.FTrackStock = 0;
                if (DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 1;
                }
                else if (!DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 0;
                }
                addItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.SaleItemType = 'U';
                addItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                addItem.UserLastModified = DataCashingAll.MerchantLocal.UserNameModified;
                addItem.DataStatus = 'I';
                addItem.FWaitSending = 1;
                addItem.Comments = null;
                addItem.LinkProMaxxItemID = null;
                addItem.LinkProMaxxItemUnit = null;
                addItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.ShortName = lblItemCardShortName.Text;
                //if (txtItemName.Text.Length > 7)
                //{
                //    addItem.ShortName = lblItemCardShortName.Text;
                //}
                //else
                //{
                //    addItem.ShortName = txtItemName.Text;
                //}
                addItem.Colors = addColor;
                if (favFlag == true)
                {

                    addItem.FavoriteNo = 1;
                }
                else if (favFlag == false)
                {
                    addItem.FavoriteNo = 0;
                }

                if (switchStock.On)
                {
                    if (addItem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStockMinimum.Text))
                        {
                            txtStockMinimum.Text = "0";
                        }

                        if (string.IsNullOrEmpty(lblStockNumber.Text))
                        {
                            Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                            btnAdd.Enabled = true;
                            return;
                        }

                        addItem.FTrackStock = 1;
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = addItem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = addItem.SysItemID,
                            BalanceStock = ConvertToDecimal(lblStockNumber.Text),
                            MinimumStock = ConvertToDecimal(txtStockMinimum.Text),
                        };
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                        btnAdd.Enabled = true;
                        return;
                    }
                }


                //Check ชื่อสินค้า 
                var checkName = false;
                var checkItewmCode = false;
                if (sysItemIDEdit == null)
                {
                    checkName = await itemManager.CheckNameItem(txtItemName.Text);
                    checkItewmCode = await itemManager.CheckItemCode(txtItemCode.Text);
                }
                else
                {
                    if (txtItemName.Text != sysItemIDEdit.ItemName) checkName = await itemManager.CheckNameItem(txtItemName.Text);
                    if (txtItemCode.Text != sysItemIDEdit.ItemCode) checkItewmCode = await itemManager.CheckItemCode(txtItemCode.Text);
                }


                string message = "";
                if ((checkName && checkItewmCode && sysItemIDEdit == null) || (checkName && checkItewmCode && sysItemIDEdit != null && txtItemCode.Text != sysItemIDEdit.ItemCode && txtItemName.Text != sysItemIDEdit.ItemName))
                {
                    message = txtItemName.Text +  Utils.TextBundle("alertitem1", "none") + txtItemCode.Text + Utils.TextBundle("alertitem2", "none");
                }
                else if ((checkName && !checkItewmCode && sysItemIDEdit == null) || (checkName && !checkItewmCode && sysItemIDEdit != null && txtItemName.Text != sysItemIDEdit.ItemName))
                {
                    message = txtItemName.Text + Utils.TextBundle("alertitem3", "none");
                }
                else if ((!checkName && checkItewmCode && sysItemIDEdit == null) || (!checkName && checkItewmCode && sysItemIDEdit != null && txtItemCode.Text != sysItemIDEdit.ItemCode))
                {
                    message = txtItemCode.Text + Utils.TextBundle("alertitem4", "none");
                }

                addItem.ItemName = txtItemName.Text.ToString();
                addItem.ItemCode = txtItemCode.Text ?? "";
                if (message != "")
                {
                    InsertRepeatItem insertRepeat = new InsertRepeatItem();
                    insertRepeat.checkManageStock = checkManageStock;
                    insertRepeat.DetailITem = addItem;
                    insertRepeat.Stock = lblStockNumber.Text;
                    insertRepeat.minimumstock = txtStockMinimum.Text;
                    var json = JsonConvert.SerializeObject(insertRepeat);

                    //ชื่อซ้ำ
                    var okCancelAlertController = UIAlertController.Create("", message, UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => CheckEdit()));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => btnAdd.Enabled = true));
                    PresentViewController(okCancelAlertController, true, null);
                    return;
                }



                CheckEdit();

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                btnAdd.Enabled = true;
                await Utils.ReloadInitialData();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdditem_Click at add Item");
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {
                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                if (edit)
                {
                    button.SetTitle("  "+Utils.TextBundle("edititem", ""), UIControlState.Normal);
                }
                else
                {
                    button.SetTitle("  "+Utils.TextBundle("additem", ""), UIControlState.Normal);
                }
                //button.SetTitle("  Back", UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);
                //GabanaLoading.SharedInstance.Show(this);
                this.NavigationController.SetNavigationBarHidden(false, false);
                //GabanaLoading.SharedInstance.Show(this);
                base.ViewDidLoad();

                Setkeyboard();

                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                if (sysItemIDEdit != null)
                {
                    extraList = await extra.GetItemSize((int)MainController.merchantlocal.MerchantID, (int)sysItemIDEdit.SysItemID);
                    for (int i = 0; i < extraList.Count; i++)
                    {
                        extraList[i].Comments = (i + 1).ToString();
                    }
                }
                initAttribute();
                setColorButton();
                setupAutoLayout();
                Textboxfocus(View);
                setCategory();
                setUpMenu();
                if (favFlag)
                {
                    favImg.Image = UIImage.FromBundle("Fav");
                }
                if (sysItemIDEdit != null && sysItemIDEdit.ItemName!=null)
                {
                    getDataOfItem();
                    //show edit btn
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                    setDetailShow(true);
                    flagDetail = 1;
                    btnDeleteView.Hidden = false;
                    btnAdd.LeftAnchor.ConstraintEqualTo(btnDeleteView.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
                }
                else
                {
                    btnDeleteView.Hidden = true;
                    itemCardView.BackgroundColor = UIColor.FromRGB(0,149,218);
                    itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    itemCardFooter.BackgroundColor = UIColor.Black;
                    itemCardFooter.Layer.Opacity = 0.2f;
                    btnAdd.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
                }


                View.BringSubviewToFront(bottomView);
                bottomView.BringSubviewToFront(btnAdd);
                View.BackgroundColor = UIColor.White;
                if (edit)
                {
                    GabanaLoading.SharedInstance.Show(this);
                    if (sysItemIDEdit.FTrackStock==1)
                    { 
                        await GetStockData();
                    }
                    
                }
                else
                {
                    addItem.Colors = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
                }


                Checkper();
                if (openstock)
                {
                    Menu[0].select = false;
                    Menu[1].select = true;
                    stockView.Hidden = false;
                    flagBtn = 1;
                    stockView.BringSubviewToFront(disableView);
                    if (switchStock.On == false)
                    {
                        disableView.Hidden = false;
                    }
                    openstock = false;
                    ((MenuItemDataSource)menuAddItemBarCollectionView.DataSource).ReloadData(Menu);
                    menuAddItemBarCollectionView.ReloadData();
                    MenuSelected = 1;
                }
                GabanaLoading.SharedInstance.Hide();

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
                GabanaLoading.SharedInstance.Hide();
            }
        }
        private void Checkper()
        {
            try
            {

            
                var LoginType = Preferences.Get("LoginType", "");
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
                if (check)
                {

                }
                else
                {

                    View.Alpha = 0.9f;
                    Viewblock = new UIView();
                    Viewblock.TranslatesAutoresizingMaskIntoConstraints = false;
                    Viewblock.BackgroundColor = UIColor.Clear;
                    View.AddSubview(Viewblock);
                    View.BringSubviewToFront(Viewblock);

                    Viewblock.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
                    Viewblock.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
                    Viewblock.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
                    Viewblock.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
                    btnDeleteView.UserInteractionEnabled = false;
                    btnAdd.Enabled = false;
                
                }
            }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        private void Setkeyboard()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            void OnKeyboardNotification(NSNotification notification)
            {
                if (!IsViewLoaded) return;
                if (flagDetail == 1)
                {

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

                    //OnKeyboardChanged(View, visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
                    OnKeyboardChanged(visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
                }
                //Commit the animation
                //UIView.CommitAnimations();
            }
        }
        private void OnKeyboardChanged(bool visible, nfloat nfloat)
        {
            if (!visible)
                RestoreScrollPosition(_scrollView);
            else
                CenterViewInScroll(View, _scrollView, nfloat);
        }
        protected virtual void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, nfloat keyboardHeight)
        {
            var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
            scrollView.ContentInset = contentInsets;
            scrollView.ScrollIndicatorInsets = contentInsets;
        }
        protected virtual void RestoreScrollPosition(UIScrollView scrollView)
        {
            scrollView.ContentInset = UIEdgeInsets.Zero;
            scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }
        async void getDataOfItem()
        {
            try
            {

            
                txtItemName.Text = sysItemIDEdit.ItemName;
                lblItemCardName.Text = sysItemIDEdit.ItemName;
                lblItemCardShortName.Text  = sysItemIDEdit.ShortName;
                LblItemCardPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS +" " + Utils.DisplayDecimal(sysItemIDEdit.Price);
                if (sysItemIDEdit.Price!=0 && sysItemIDEdit.Price !=null)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(sysItemIDEdit.Price);
                }
                if (sysItemIDEdit.EstimateCost != 0 && sysItemIDEdit.EstimateCost != null)
                {
                    txtItemCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(sysItemIDEdit.EstimateCost);
                }
          
                txtItemCode.Text = sysItemIDEdit.ItemCode;
                itemCardViewTop.Image = null;
                        
                if (!string.IsNullOrEmpty(sysItemIDEdit.ThumbnailLocalPath))
                {
                    itemCardViewTop.Hidden = false;
                    //Utils.SetImageURL(itemCardViewTop, sysItemIDEdit.PictureLocalPath);
                    lblItemCardShortName.Hidden = true;
                    itemCardView.BackgroundColor = UIColor.FromRGB(162, 162, 162);
                    itemCardViewTop.BackgroundColor = UIColor.White;
                    itemCardFooter.BackgroundColor = UIColor.FromRGB(162, 162, 162);
                    itemCardFooter.Layer.Opacity = 1f;
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Utils.SetImageURL(itemCardViewTop, Path.Combine(docFolder, sysItemIDEdit.ThumbnailLocalPath));
                }
                else
                {
                    itemCardViewTop.Hidden = true;
                    itemCardView.BackgroundColor = UIColor.FromRGB(0,149,218);
                    itemCardFooter.BackgroundColor = UIColor.Black;
                    itemCardFooter.Layer.Opacity = 0.2f;
                    if (sysItemIDEdit.Colors != null)
                    {
                        Utils.SetColor(itemCardView, Convert.ToInt64(sysItemIDEdit.Colors));
                        addColor = (long)sysItemIDEdit.Colors ;
                    }
                    else
                    {
                        Utils.SetColor(itemCardView,0);
                        addColor = 0;

                    }
               
                }
                if (sysItemIDEdit.TaxType=='V')
                {
                    lblVatMode.Text = Utils.TextBundle("havevat", "havevat");
                }
                else
                {
                    lblVatMode.Text = Utils.TextBundle("nonevat", "nonevat");
                }
                if(sysItemIDEdit.FavoriteNo == 1)
                {
                    favFlag = true;
                    favImg.Image = UIImage.FromBundle("Fav"); // Fav
               
                }
                else
                {
                    favFlag = false;
                    favImg.Image = UIImage.FromBundle("Unfav"); // Fav
                }
                if (sysItemIDEdit.FDisplayOption == 1)
                {
                    DisplaySwitch.On = true; ;
                }
                else
                {
                    DisplaySwitch.On = false;
                }
                if (extraList.Count>0)
                {
                    SizeHeight = 220 * extraList.Count;
                    Utils.SetConstant(ExtraSizeCollectionView.Constraints, NSLayoutAttribute.Height, SizeHeight);
                    ExtraSizeCollectionView.LayoutIfNeeded();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        void setUpMenu()
        {
            Menu = new List<MenuitemHeaderIOS>();
            Menu.Add(new MenuitemHeaderIOS(0, Utils.TextBundle("item", "item"), true));
            Menu.Add(new MenuitemHeaderIOS(1, Utils.TextBundle("stock", "stock"), false));

            menuAddItemDataSource = new MenuItemDataSource(Menu);
            menuAddItemBarCollectionView.DataSource = menuAddItemDataSource;
            menuAddItemBarCollectionView.ReloadData();
        }
        void initAttribute()
        {
            try
            {

            
            #region MenuItem

            UICollectionViewFlowLayout MenuflowLayoutList = new UICollectionViewFlowLayout();
            MenuflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            MenuflowLayoutList.MinimumLineSpacing = 0;
            MenuflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width / 2), height: 40);


            menuAddItemBarCollectionView = new UICollectionView(frame: View.Frame, layout: MenuflowLayoutList);
            menuAddItemBarCollectionView.BackgroundColor = UIColor.White;
            menuAddItemBarCollectionView.ScrollEnabled = false;
            menuAddItemBarCollectionView.ShowsVerticalScrollIndicator = true;
            menuAddItemBarCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            menuAddItemBarCollectionView.RegisterClassForCell(cellType: typeof(MenuCollectionViewCell), reuseIdentifier: "MenuCollectionViewCell");
           
            MenuItemBarCollectionDelegate CollectionDelegate = new MenuItemBarCollectionDelegate();
            CollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (int)(indexPath).Item;
                if (x == 0)
                {
                    Menu[0].select = true;
                    Menu[1].select = false;

                    stockView.Hidden = true;
                    flagBtn = 0;
                    disableView.Hidden = true;
                    _scrollView.Hidden = false;
                }
                else if (x == 1)
                {
                    Menu[0].select = false;
                    Menu[1].select = true;
                    stockView.Hidden = false;
                    flagBtn = 1;
                    stockView.BringSubviewToFront(disableView);
                    if (switchStock.On == false)
                    {
                        disableView.Hidden = false;
                    }
                }
                ((MenuItemDataSource)menuAddItemBarCollectionView.DataSource).ReloadData(Menu);
                menuAddItemBarCollectionView.ReloadData();
                MenuSelected = (int)indexPath.Row;

            };
            menuAddItemBarCollectionView.Delegate = CollectionDelegate;
            View.AddSubview(menuAddItemBarCollectionView);
            #endregion

            #region bottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);
           

            btnAdd = new UIButton();
            btnAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAdd.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAdd.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnAdd.Layer.CornerRadius = 5f;
            btnAdd.Layer.BorderWidth = 0.5f;
            if (sysItemIDEdit != null && sysItemIDEdit.MerchantID != 0)
            {
                btnAdd.SetTitle(Utils.TextBundle("save", "save"), UIControlState.Normal);
            }
            else
            {
                btnAdd.SetTitle(Utils.TextBundle("additem", "additem"), UIControlState.Normal);
            }
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAdd.TouchUpInside += BtnAdditem_Click;
            bottomView.AddSubview(btnAdd);

            btnDeleteView = new UIView();
            btnDeleteView.Hidden = true;
            btnDeleteView.Layer.CornerRadius = 5;
            btnDeleteView.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            bottomView.AddSubview(btnDeleteView);

            btnDelete = new UIImageView();
            btnDelete.Image = UIImage.FromFile("Trash.png");
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteView.AddSubview(btnDelete);

            btnDeleteView.UserInteractionEnabled = true;
            var tapGestureDelete = new UITapGestureRecognizer(this,
             new ObjCRuntime.Selector("Delete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnDeleteView.AddGestureRecognizer(tapGestureDelete);

            #endregion

            #region ItemViewAttribute
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region SetItemImage
            imageView = new UIView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.BackgroundColor = UIColor.White;
            

            itemCardView = new UIImageView();
            itemCardView.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardView.Layer.CornerRadius = 10;
            imageView.AddSubview(itemCardView);


            itemCardView.UserInteractionEnabled = true;
            var tapGestureimg = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("imageview:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            itemCardView.AddGestureRecognizer(tapGestureimg);



            itemCardViewTop = new UIImageView();
            itemCardViewTop.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardViewTop.Layer.CornerRadius = 10;
            itemCardViewTop.ClipsToBounds = true;
            itemCardView.AddSubview(itemCardViewTop);

            itemCardFooter = new UIView();
            itemCardFooter.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardFooter.Layer.CornerRadius = 10;
            itemCardView.AddSubview(itemCardFooter);

            lblItemCardName = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCardName.Font = lblItemCardName.Font.WithSize(13);
            lblItemCardName.Text = Utils.TextBundle("itemname", "Item Name");
            itemCardView.AddSubview(lblItemCardName);

            

            lblItemCardShortName = new UITextField
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false,
                KeyboardType = UIKeyboardType.ASCIICapable
            };
            lblItemCardShortName.ReturnKeyType = UIReturnKeyType.Done;
            lblItemCardShortName.Text = "Item Na";
            lblItemCardShortName.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 7;
            };
            lblItemCardShortName.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            lblItemCardShortName.AttributedPlaceholder = new NSAttributedString("", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 221, 245) });
            lblItemCardShortName.Font = lblItemCardShortName.Font.WithSize(15);
            itemCardView.AddSubview(lblItemCardShortName);



            LblItemCardPrice = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            LblItemCardPrice.Font = LblItemCardPrice.Font.WithSize(13);
            LblItemCardPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0.00";
            itemCardView.AddSubview(LblItemCardPrice);

            //itemCardView.BringSubviewToFront(lblItemCardName);
            ////itemCardView.BringSubviewToFront(lblItemCardShortName);
            //itemCardView.BringSubviewToFront(LblItemCardPrice);

            #endregion
            #region SetColor
            setColorView = new UIView();
            setColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            setColorView.BackgroundColor = UIColor.White;
            #endregion
            #region SetItemName
            setItemNameView = new UIView();
            setItemNameView.TranslatesAutoresizingMaskIntoConstraints = false;
            setItemNameView.BackgroundColor = UIColor.White;
            
            lblItemName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemName.Font = lblItemName.Font.WithSize(15);
            lblItemName.Text = Utils.TextBundle("itemname", "Item Name");
            setItemNameView.AddSubview(lblItemName);

            txtItemName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtItemName.ReturnKeyType = UIReturnKeyType.Next;
            txtItemName.ShouldReturn = (tf) =>
            {
                txtItemPrice.BecomeFirstResponder();
                return true;
            };
            txtItemName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("itemname", "itemname"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 221, 245) });
            txtItemName.Font = txtItemName.Font.WithSize(15);
            txtItemName.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true;
                lblItemCardName.Text = txtItemName.Text;
                if (txtItemName.Text.Length > 7)
                {
                    lblItemCardShortName.Text = txtItemName.Text.Substring(0, 7);
                }
                else
                {
                    lblItemCardShortName.Text = txtItemName.Text;
                }
                if (txtItemName.Text.Length == 0 )
                {
                    lblItemCardShortName.Text = "Item Na";
                }
            };
            setItemNameView.AddSubview(txtItemName);
            #endregion
            #region SetItemPrice
            SetItemPriceView = new UIView();
            SetItemPriceView.TranslatesAutoresizingMaskIntoConstraints = false;
            SetItemPriceView.BackgroundColor = UIColor.White;
            

            lblItemPrice = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemPrice.Font = lblItemPrice.Font.WithSize(15);
            lblItemPrice.Text = Utils.TextBundle("price", "Price");
            SetItemPriceView.AddSubview(lblItemPrice);

            UIToolbar NumpadToolbarPrice = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)View.Frame.Width, 44.0f));
            NumpadToolbarPrice.Translucent = true;
            NumpadToolbarPrice.BarStyle = UIBarStyle.Default;
            NumpadToolbarPrice.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
              if(!txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                     txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text;
                }
                View.EndEditing(true);
            })
            };
            NumpadToolbarPrice.SizeToFit();

            txtItemPrice = new EditTextNopaste
            {
                
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
                
            };
            
            txtItemPrice.InputAccessoryView = NumpadToolbarPrice;
            txtItemPrice.AttributedPlaceholder = new NSAttributedString(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS+" 0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtItemPrice.KeyboardType = UIKeyboardType.DecimalPad;
            
            txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + 0;
            txtItemPrice.Font = txtItemPrice.Font.WithSize(15);
            //txtItemPrice.EditingChanged += TxtItemPrice_EditingChanged;
            txtItemPrice.EditingDidBegin += TxtItemPrice_EditingDidBegin;
            txtItemPrice.EditingDidEnd += TxtItemPrice_EditingDidEnd;
            txtItemPrice.ShouldChangeCharacters =  (textField, range, replacementString) => {
                if (textField.Text.Contains(".") && replacementString == ".")
                {
                    return false;
                }
                return true;
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 15;

            };

            SetItemPriceView.AddSubview(txtItemPrice);
            #endregion
            #region DetailItem
            DetailClickView = new UIView();
            DetailClickView.TranslatesAutoresizingMaskIntoConstraints = false;
            DetailClickView.BackgroundColor = UIColor.White;
            
            lblDetail = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDetail.Font = lblDetail.Font.WithSize(15);
            lblDetail.Text = Utils.TextBundle("details", "Details");

            DetailClickView.AddSubview(lblDetail);

            DetailClickView.UserInteractionEnabled = true;
            var tapdetail= new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Detail:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DetailClickView.AddGestureRecognizer(tapdetail);

            btnToggleDetail = new UIButton();
            btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
            btnToggleDetail.TranslatesAutoresizingMaskIntoConstraints = false;
            btnToggleDetail.TouchUpInside += (sender, e) =>
            {
                if (flagDetail == 0)
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                    setDetailShow(true);
                    flagDetail = 1;
                }
                else
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                    setDetailShow(false);
                    flagDetail = 0;
                }

            };
            DetailClickView.AddSubview(btnToggleDetail);

            #region itemcodeField
            ItemCodeView = new UIView();
            ItemCodeView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemCodeView.BackgroundColor = UIColor.White;
            ItemCodeView.Hidden = true;
           

            lblItemCode = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCode.Font = lblItemCode.Font.WithSize(15);
            lblItemCode.Text = Utils.TextBundle("itemcode", "Item Code");
            ItemCodeView.AddSubview(lblItemCode);

            txtItemCode = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
                KeyboardType = UIKeyboardType.ASCIICapable
            };
            txtItemCode.ReturnKeyType = UIReturnKeyType.Done;
            txtItemCode.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtItemCode.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("code", "Code"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 221, 245) });
            txtItemCode.Font = txtItemCode.Font.WithSize(15);
            ItemCodeView.AddSubview(txtItemCode);

            btnScan = new UIButton();
            btnScan.SetImage(UIImage.FromBundle("ScanItem"), UIControlState.Normal);
            btnScan.Layer.CornerRadius = 3;
            //btnScan.BackgroundColor = UIColor.Red;
            btnScan.TranslatesAutoresizingMaskIntoConstraints = false;
            btnScan.TouchUpInside += (sender, e) =>
            {
                if (Utils.Checkpermisstion())
                {
                    string page = "ADDITEM";
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("scancode", "Scan code"));
                    POSScanBarcodeController scanPage = new POSScanBarcodeController(page);
                    this.NavigationController.PushViewController(scanPage, false);
                }
            };
            ItemCodeView.AddSubview(btnScan);

            #endregion

            #region CategoryField
            CategoryView = new UIView();
            CategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            CategoryView.BackgroundColor = UIColor.White;
            CategoryView.Hidden = true;
           

            lblCategory = new UILabel
            {
                TextColor =  UIColor.FromRGB(64 , 64 ,  64 ),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCategory.Font = lblCategory.Font.WithSize(15);
            lblCategory.Text = Utils.TextBundle("category", "Category");
            CategoryView.AddSubview(lblCategory);

            lblSelectedCategory = new UITextField
            {
                BorderStyle = UITextBorderStyle.None,
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = UIColor.FromRGB(162, 162, 162),
            };
            lblSelectedCategory.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("category", "Category"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            lblSelectedCategory.Font.WithSize(14);
            CategoryView.AddSubview(lblSelectedCategory);

            btnSelectCategory = new UIButton();
            btnSelectCategory.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectCategory.TouchUpInside += (sender, e) => {
                lblSelectedCategory.BecomeFirstResponder();
            };
            CategoryView.AddSubview(btnSelectCategory);
            #endregion

            #region vatField
            VatView = new UIView();
            VatView.TranslatesAutoresizingMaskIntoConstraints = false;
            VatView.BackgroundColor = UIColor.White;
            VatView.Hidden = true;

            lblVat = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64 ,  64 ),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblVat.Font = lblVat.Font.WithSize(15);
            lblVat.Text = Utils.TextBundle("vat", "Vat");
            VatView.AddSubview(lblVat);

            lblVatMode = new UITextField
            {
                BorderStyle = UITextBorderStyle.None,
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = UIColor.FromRGB(162, 162, 162),
            };
            lblVatMode.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("havevat", "Have Vat"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            lblVatMode.Font.WithSize(15);
            VatView.AddSubview(lblVatMode);

            btnSelectVatType = new UIButton();
            btnSelectVatType.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectVatType.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectVatType.TouchUpInside += (sender, e) => {
                lblVatMode.BecomeFirstResponder();
            };
            VatView.AddSubview(btnSelectVatType);
            #endregion

            #region costField
            CostView = new UIView();
            CostView.TranslatesAutoresizingMaskIntoConstraints = false;
            CostView.BackgroundColor = UIColor.White;
            CostView.Hidden = true;
            

            lblItemCost = new UILabel
            {
                TextColor =  UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCost.Font = lblItemCost.Font.WithSize(15);
            lblItemCost.Text = Utils.TextBundle("cost", "Cost");
            CostView.AddSubview(lblItemCost);

            UIToolbar NumpadToolbarCost = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)View.Frame.Width, 44.0f));
            NumpadToolbarCost.Translucent = true;
            NumpadToolbarCost.BarStyle = UIBarStyle.Default;
            NumpadToolbarCost.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                if(!txtItemCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                     txtItemCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemCost.Text;
                }
                View.EndEditing(true);
            })
            };
            NumpadToolbarCost.SizeToFit();

            txtItemCost = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
            };
            txtItemCost.ReturnKeyType = UIReturnKeyType.Done;
            txtItemCost.InputAccessoryView = NumpadToolbarCost;
            txtItemCost.AttributedPlaceholder = new NSAttributedString(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 221, 245) });
            txtItemCost.TranslatesAutoresizingMaskIntoConstraints = false;
            txtItemCost.Font = txtItemCost.Font.WithSize(15);
            txtItemCost.KeyboardType = UIKeyboardType.DecimalPad;
            txtItemCost.ShouldChangeCharacters = (textField, range, replacementString) => {
                if (textField.Text.Contains(".") && replacementString == ".")
                {
                    return false;
                }
                return true;
            };
            txtItemCost.EditingDidEnd += TxtItemCost_EditingDidEnd1;
            CostView.AddSubview(txtItemCost);

            #endregion
            // size
            #region SizeView
            #region Size Collection
            UICollectionViewFlowLayout itemflowLayoutListExtra = new UICollectionViewFlowLayout();
            itemflowLayoutListExtra.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 210);
            itemflowLayoutListExtra.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            ExtraSizeCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutListExtra);
            ExtraSizeCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            ExtraSizeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ExtraSizeCollectionView.ScrollEnabled = false;
            ExtraSizeCollectionView.RegisterClassForCell(cellType: typeof(ExtraSizeCollectionViewCell), reuseIdentifier: "extraSizeCollectionViewCell");
            ItemExtraSizeDetailDataSource itemExtraDetailDataSource = new ItemExtraSizeDetailDataSource(extraList);
            itemExtraDetailDataSource.OnExtraSizeDeleteIndex += (indexPath) =>
            {
                try
                {




                    //int i = ExtraSizeCollectionView.VisibleCells.Count() - 1;
                    //foreach (ExtraSizeCollectionViewCell item in ExtraSizeCollectionView.VisibleCells)
                    //{
                    //    extraList[item.Nub - 1].ExSizeName = item.SizeName;
                    //    if (string.IsNullOrEmpty(item.Price)) item.Price = "0";
                    //    extraList[item.Nub - 1].Price = Convert.ToDecimal(item.Price);
                    //    if (string.IsNullOrEmpty(item.EstimateCost)) item.EstimateCost = "0";
                    //    extraList[item.Nub - 1].EstimateCost = Convert.ToDecimal(item.EstimateCost);
                    //    extraList[item.Nub - 1].Comments = item.Nub.ToString();
                    //    i--;
                    //}
                    var x = (int)(indexPath).Item;
                    extraList.RemoveAt(x);
                    ((ItemExtraSizeDetailDataSource)ExtraSizeCollectionView.DataSource).ReloadData(this.extraList);
                    ExtraSizeCollectionView.ReloadData();
                    SizeHeight = 220 * extraList.Count;
                    Utils.SetConstant(ExtraSizeCollectionView.Constraints, NSLayoutAttribute.Height, SizeHeight);
                    ExtraSizeCollectionView.LayoutIfNeeded();
                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                }
            };
            ExtraSizeCollectionView.DataSource = itemExtraDetailDataSource;

            #endregion

            SizeView = new UIView();
            SizeView.Hidden = true;
            SizeView.TranslatesAutoresizingMaskIntoConstraints = false;
            SizeView.BackgroundColor = UIColor.White;

            lblSize = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSize.Font = lblSize.Font.WithSize(15);
            lblSize.Text = Utils.TextBundle("size", "Size");
            SizeView.AddSubview(lblSize);

            lblMaxSize = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMaxSize.Font = lblMaxSize.Font.WithSize(15);
            lblMaxSize.Text = Utils.TextBundle("maxsize", "Size Maximum 5");
            SizeView.AddSubview(lblMaxSize);

            btnAddSize = new UIButton();
            btnAddSize.BackgroundColor = UIColor.FromRGB(0,149,218);
            btnAddSize.SetTitle(Utils.TextBundle("addsize", "Add Size"), UIControlState.Normal);
            btnAddSize.SetTitleColor(UIColor.White,UIControlState.Normal);
            btnAddSize.Layer.CornerRadius = 17;
            btnAddSize.ClipsToBounds = true;
            btnAddSize.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddSize.TouchUpInside += (sender, e) =>
            {
                try
                {
                    if (switchStock.On)
                    {
                        Utils.ShowMessage(Utils.TextBundle("havestocknoaddsize", "สินค้ามีการกำหนด Stock ไม่สามารถเพิ่ม Size ได้")) ;

                        return;
                    }
                    int i = ExtraSizeCollectionView.VisibleCells.Count() - 1;
                    foreach (ExtraSizeCollectionViewCell item in ExtraSizeCollectionView.VisibleCells)
                    {
                        extraList[i].ExSizeName = item.SizeName;
                        if (string.IsNullOrEmpty(item.Price)) item.Price = "0";
                        extraList[i].Price = Convert.ToDecimal(item.Price);
                        if (string.IsNullOrEmpty(item.EstimateCost)) item.EstimateCost = "0";
                        extraList[i].EstimateCost = Convert.ToDecimal(item.EstimateCost);
                        extraList[i].Comments = item.Nub.ToString();
                        i--;
                    }
                    
                    if (extraList.Count > 0 && extraList.Count < 5)
                    {
                        extraList.OrderBy(x => x.Price);
                        extraList.Add(new ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = SysItemId, ExSizeName = "", Price = 0, EstimateCost = 0 , Comments = (extraList.Count+1).ToString() });
                        SizeHeight = 220 * extraList.Count;
                        Utils.SetConstant(ExtraSizeCollectionView.Constraints, NSLayoutAttribute.Height, SizeHeight);
                        //ExtraSizeCollectionView.LayoutIfNeeded();
                        extraList = extraList.OrderBy(x => x.Comments).ToList();
                        ((ItemExtraSizeDetailDataSource)ExtraSizeCollectionView.DataSource).ReloadData(this.extraList);
                        ExtraSizeCollectionView.ReloadData();
                    }
                    else if (extraList.Count == 0)
                    {
                        extraList.Add(new ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = SysItemId, ExSizeName = "", Price = 0, EstimateCost = 0 , Comments = "1"});
                        Utils.SetConstant(ExtraSizeCollectionView.Constraints, NSLayoutAttribute.Height, 220);
                        //ExtraSizeCollectionView.LayoutIfNeeded();
                        ((ItemExtraSizeDetailDataSource)ExtraSizeCollectionView.DataSource).ReloadData(this.extraList);
                        ExtraSizeCollectionView.ReloadData();
                    }
                    //if (extraList.Count>1)
                    //{
                    //    ExtraSizeCollectionView.ScrollToItem(NSIndexPath.FromRowSection(extraList.Count, 0), UICollectionViewScrollPosition.CenteredHorizontally, true);
                    //}
                    
                    
                }
                catch (Exception ex )
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    Utils.ShowMessage(ex.Message);
                }
                
            };
            SizeView.AddSubview(btnAddSize);
            #endregion
            // display on pos
            #region DisplayPOSView
            DisplayPOSView = new UIView();
            DisplayPOSView.Hidden = true;
            DisplayPOSView.TranslatesAutoresizingMaskIntoConstraints = false;
            DisplayPOSView.BackgroundColor = UIColor.White;

            lblDiaplayPOS = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDiaplayPOS.Font = lblDiaplayPOS.Font.WithSize(15);
            lblDiaplayPOS.Text = Utils.TextBundle("display", "display");
            DisplayPOSView.AddSubview(lblDiaplayPOS);

            lblDisplaytext = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDisplaytext.Font = lblDisplaytext.Font.WithSize(15);
            lblDisplaytext.Text = Utils.TextBundle("extratop", "extratop");
            DisplayPOSView.AddSubview(lblDisplaytext);

            DisplaySwitch = new UISwitch();
            DisplaySwitch.OnTintColor = UIColor.FromRGB(0, 149, 218);
            DisplaySwitch.TranslatesAutoresizingMaskIntoConstraints = false;
            DisplaySwitch.SetState(DisplaySwitch.On, true);
            DisplaySwitch.ValueChanged += (sender, e) =>
            {
                if (DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 1;
                }
                else if (!DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 0;
                }
            };
            DisplayPOSView.AddSubview(DisplaySwitch);
            #endregion
            #endregion
           

            _contentView.AddSubview(imageView);
            _contentView.AddSubview(setColorView);
            _contentView.AddSubview(setItemNameView);
            _contentView.AddSubview(SetItemPriceView);
            _contentView.AddSubview(DetailClickView);
            _contentView.AddSubview(ItemCodeView);
            _contentView.AddSubview(CategoryView);
            _contentView.AddSubview(VatView);
            _contentView.AddSubview(CostView);

            _contentView.AddSubview(SizeView);
            _contentView.AddSubview(ExtraSizeCollectionView);
            _contentView.AddSubview(DisplayPOSView);
            _scrollView.AddSubview(_contentView);

            View.AddSubview(_scrollView);

            #endregion

            #region StockViewAttribute
            stockView = new UIView();
            stockView.TranslatesAutoresizingMaskIntoConstraints = false;
            stockView.BackgroundColor = UIColor.White;
            stockView.Hidden = true;
            View.AddSubview(stockView);

            StockbarView = new UIView();
            StockbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            StockbarView.BackgroundColor = UIColor.White;
            stockView.AddSubview(StockbarView);

            disableView = new UIView();
            disableView.TranslatesAutoresizingMaskIntoConstraints = false;
            disableView.BackgroundColor = UIColor.White;
            disableView.Alpha = 0.5f;
            stockView.AddSubview(disableView);

            lblStock = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStock.Font = lblStock.Font.WithSize(15);
            lblStock.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStock.Text = Utils.TextBundle("stock", "stock");
            StockbarView.AddSubview(lblStock);

            switchStock = new UISwitch();
            switchStock.TranslatesAutoresizingMaskIntoConstraints = false;
            switchStock.OnTintColor = UIColor.FromRGB(0,149,218);
            switchStock.SetState(switchStock.On, true);
                
            switchStock.ValueChanged += async (sender, e) =>
            {
                feditstock = true ;
                if (await GabanaAPI.CheckNetWork())
                {
                    if (switchStock.On)
                    {
                        if (ExtraSizeCollectionView.VisibleCells.Count()>0)
                        {
                            switchStock.On = false;
                            Utils.ShowMessage(Utils.TextBundle("havesizenoaddstock", "สินค้ามีการกำหนด Size ไม่สามารถเปิดระบบการกำหนด Stock ได้"));
                            return;
                        }
                        txtStockMinimum.Enabled = true;
                        disableView.Hidden = true;
                    }
                    else
                    {
                        txtStockMinimum.Enabled = false;
                        disableView.Hidden = false;
                    }
                }
            };
            StockbarView.AddSubview(switchStock);

            lbltxtOnHand = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtOnHand.Font = lbltxtOnHand.Font.WithSize(15);
            lbltxtOnHand.TranslatesAutoresizingMaskIntoConstraints = false;
            lbltxtOnHand.Text = Utils.TextBundle("onhand", "On Hand");
            StockbarView.AddSubview(lbltxtOnHand);

            lblStockNumber = new UITextField
            {
                Placeholder = "0" ,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockNumber.Font = lblStock.Font.WithSize(45);
            lblStockNumber.TranslatesAutoresizingMaskIntoConstraints = false;

            lblStockNumber.UserInteractionEnabled = true;
            var tapGestureOnHand = new UITapGestureRecognizer(this,new ObjCRuntime.Selector("OnHand:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            lblStockNumber.AddGestureRecognizer(tapGestureOnHand);

            stockView.AddSubview(lblStockNumber);

            lblStockMinimum = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockMinimum.Font = lblStock.Font.WithSize(15);
            lblStockMinimum.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockMinimum.Text = Utils.TextBundle("minstock", "Minimum Stock");
            stockView.AddSubview(lblStockMinimum);



                

                line3 = new UIView();
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            line3.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(line3);


            UIToolbar NumpadToolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)View.Frame.Width, 44.0f));
            NumpadToolbar.Translucent = true;
            NumpadToolbar.BarStyle = UIBarStyle.Default;
            NumpadToolbar.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            })
            };
            NumpadToolbar.SizeToFit();

            txtStockMinimum = new UITextField
            {
                Placeholder = "0",
                
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            txtStockMinimum.EditingDidEnd += TxtStockMinimum_EditingDidEnd;
            txtStockMinimum.Font = txtStockMinimum.Font.WithSize(15);
            txtStockMinimum.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 6;
            };
            txtStockMinimum.InputAccessoryView = NumpadToolbar;
            txtStockMinimum.KeyboardType = UIKeyboardType.NumberPad;
            txtStockMinimum.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            stockView.AddSubview(txtStockMinimum);

                lblStockMinimum.UserInteractionEnabled = true;
                var tapGestureOnHand2 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("min:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                lblStockMinimum.AddGestureRecognizer(tapGestureOnHand2);
                lineEmpty = new UIView();
            lineEmpty.TranslatesAutoresizingMaskIntoConstraints = false;
            lineEmpty.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(lineEmpty);

            line4 = new UIView();
            line4.TranslatesAutoresizingMaskIntoConstraints = false;
            line4.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(line4);

            viewstockmove = new UIView();
            viewstockmove.TranslatesAutoresizingMaskIntoConstraints = false;
            viewstockmove.BackgroundColor = UIColor.White;
            viewstockmove.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Stockmove:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            viewstockmove.AddGestureRecognizer(tapGesture);
            stockView.AddSubview(viewstockmove);

            lblStockmove = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockmove.Font = lblStock.Font.WithSize(15);
            lblStockmove.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockmove.Text = Utils.TextBundle("stockmove", "Stock Movement");
            viewstockmove.AddSubview(lblStockmove);

            imgSelectStockmove = new UIImageView();
            imgSelectStockmove.Image = UIImage.FromFile("Next.png");
            imgSelectStockmove.TranslatesAutoresizingMaskIntoConstraints = false;
            viewstockmove.AddSubview(imgSelectStockmove);

                #endregion
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void TxtItemPrice_EditingChanged(object sender, EventArgs e)
        {
            var text = "";
            if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                text = txtItemPrice.Text.Remove(0, 1).Trim();
            }
            if (!string.IsNullOrEmpty(text))
            {
                if ((decimal)(Convert.ToDouble(text)) < 9999999999999)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal((decimal)(Convert.ToDouble(text)));
                }
                else
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal((decimal)(Convert.ToDouble(9999999999999)));
                }
            }
        }

        [Export("showimage:")]
        public void Close2(UIGestureRecognizer sender)
        {
            GabanaShowImage.SharedInstance.Hide();
        }


        private void TxtStockMinimum_EditingDidEnd(object sender, EventArgs e)
        {
            feditstock = true;
            Editchange = true;
        }

        private void TxtItemPrice_EditingDidBegin(object sender, EventArgs e)
        {
            if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                if (!string.IsNullOrEmpty(txtItemPrice.Text.Remove(0, 1).Trim()) && Convert.ToDecimal(txtItemPrice.Text.Remove(0, 1).Trim()) == 0)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " ";
                }
                
            }
            txtItemPrice.BecomeFirstResponder();
            
        }

        private void TxtItemPrice_EditingDidEnd(object sender, EventArgs e)
        {
            var text = "";
            Editchange = true;
            if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                text = txtItemPrice.Text.Remove(0, 1).Trim();
            }
            if (!string.IsNullOrEmpty(text))
            {
                if ((decimal)(Convert.ToDouble(text)) < 9999999999999)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal((decimal)(Convert.ToDouble(text)));
                }
                else
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal((decimal)(Convert.ToDouble(9999999999999)));
                }
            }
            LblItemCardPrice.Text = txtItemPrice.Text;
            //if ((decimal)(Convert.ToDouble(amount) * 0.01) < 10000000000)
            //{
            //    lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble(amount) * 0.01));
            //    strValue = lblDummy.Text;
            //}
            //else
            //{
            //    lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble("999999999999") * 0.01));
            //    strValue = lblDummy.Text;
            //}
            //if (!txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            //{
            //    //txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text;
            //    if (!string.IsNullOrEmpty(txtItemPrice.Text.Trim()))
            //    {
            //        txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text.Trim();
            //    }
            //    else
            //    {
            //        txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
            //    }
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty( txtItemPrice.Text.Remove(0, 1).Trim()))
            //    {
            //        txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(Convert.ToDecimal(txtItemPrice.Text.Remove(0, 1).Trim()));
            //    }
            //    else
            //    {
            //        txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
            //    }
                 
            //}
            //LblItemCardPrice.Text = txtItemPrice.Text;
        }
        private void TxtItemCost_EditingDidEnd1(object sender, EventArgs e)
        {
            Editchange = true;
            if (!txtItemCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                txtItemCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemCost.Text;
            }
            else
            {
                if (!string.IsNullOrEmpty(txtItemCost.Text.Remove(0, 1).Trim()))
                {
                    txtItemCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(Convert.ToDecimal(txtItemCost.Text.Remove(0, 1).Trim()));
                }
                else
                {
                    txtItemCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
                }

            }
            

        }

        private async void BtnAdditem_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");
                ItemManage a = new ItemManage();
                var it = await a.GetAllItem();

                if (string.IsNullOrWhiteSpace(txtItemName.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("enteritemname", "enteritemname"));
                    btnAdd.Enabled = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtItemPrice.Text.Remove(0, 2)))
                {
                    Utils.ShowMessage(Utils.TextBundle("enteritemprice", "enteritemprice"));
                    btnAdd.Enabled = true;
                    return;
                }


                addItem.MerchantID = MainController.merchantlocal.MerchantID;
                if (!edit)
                {
                    addItem.SysItemID = long.Parse(sys);
                }
                //addItem.ItemName = txtItemName.Text.ToString();
                addItem.FavoriteNo = 0;
                if(txtItemCost.Text!=null && txtItemCost.Text!="")
                {
                    if (string.IsNullOrEmpty(txtItemCost.Text.Remove(0, 2)))
                    {
                        addItem.EstimateCost = 0;
                    }
                    else
                    {
                        addItem.EstimateCost = Convert.ToDecimal(txtItemCost.Text.Remove(0, 2));
                    }
                }
                else
                {
                    addItem.EstimateCost = 0;
                }
                
                //addItem.ItemCode = txtItemCode.Text ?? "";
                addItem.Price = Convert.ToDecimal(txtItemPrice.Text.Remove(0,2));
                long? category = null;
                if (lblSelectedCategory.Text == Utils.TextBundle("none", "none") )
                {
                    category = null;
                }
                else
                {
                    category = idCategory;
                }
                addItem.SysCategoryID = category;
                addItem.OptSalePrice = 'F';
                if (lblVatMode.Text == Utils.TextBundle("havevat", "Have Vat") )
                {
                    addItem.TaxType = 'V'; // addItem.Vat
                }
                if (lblVatMode.Text == Utils.TextBundle("nonevat", "None Vat") )
                {
                    addItem.TaxType = 'N'; // addItem.Vat
                }
                addItem.UnitName = null;
                addItem.RegularSizeName = null;
                
                addItem.Ordinary = 2;
                addItem.SellBy = 'U';
                addItem.FTrackStock = 0;
                if (DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 1;
                }
                else if (!DisplaySwitch.On)
                {
                    addItem.FDisplayOption = 0;
                }
                addItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.SaleItemType = 'U';
                addItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                addItem.UserLastModified = DataCashingAll.MerchantLocal.UserNameModified;
                addItem.DataStatus = 'I';
                addItem.FWaitSending = 1;
                addItem.Comments = null;
                addItem.LinkProMaxxItemID = null;
                addItem.LinkProMaxxItemUnit = null;
                addItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.ShortName = lblItemCardShortName.Text;
                //if (txtItemName.Text.Length > 7)
                //{
                //    addItem.ShortName = lblItemCardShortName.Text;
                //}
                //else
                //{
                //    addItem.ShortName = txtItemName.Text;
                //}
                addItem.Colors = addColor;
                if (favFlag == true)
                {

                    addItem.FavoriteNo = 1;
                }
                else if (favFlag == false)
                {
                    addItem.FavoriteNo = 0;
                }

                if (switchStock.On)
                {
                    if (addItem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStockMinimum.Text))
                        {
                            txtStockMinimum.Text = "0";
                        }
                        
                        if (string.IsNullOrEmpty(lblStockNumber.Text))
                        {
                            Utils.ShowMessage(Utils.TextBundle("enterall", "กรุณากรอกข้อมูลให้ครบถ้วน"));
                            btnAdd.Enabled = true;
                            return;
                        }

                        addItem.FTrackStock = 1;
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = addItem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = addItem.SysItemID,
                            BalanceStock = ConvertToDecimal(lblStockNumber.Text),
                            MinimumStock = ConvertToDecimal(txtStockMinimum.Text),
                        };
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("enterall", "กรุณากรอกข้อมูลให้ครบถ้วน"));
                        btnAdd.Enabled = true;
                        return;
                    }
                }


                //Check ชื่อสินค้า 
                var checkName = false;
                var checkItewmCode = false;
                if (sysItemIDEdit == null)
                {
                    checkName = await itemManager.CheckNameItem(txtItemName.Text);
                    checkItewmCode = await itemManager.CheckItemCode(txtItemCode.Text);
                }
                else
                {
                    if (txtItemName.Text != sysItemIDEdit.ItemName) checkName = await itemManager.CheckNameItem(txtItemName.Text);
                    if (txtItemCode.Text != sysItemIDEdit.ItemCode) checkItewmCode = await itemManager.CheckItemCode(txtItemCode.Text);
                }
                

                string message = "";
                if ((checkName && checkItewmCode && sysItemIDEdit == null) || (checkName && checkItewmCode && sysItemIDEdit != null && txtItemCode.Text != sysItemIDEdit.ItemCode && txtItemName.Text != sysItemIDEdit.ItemName))
                {
                    message = txtItemName.Text + Utils.TextBundle("alertitem1", "none") + txtItemCode.Text + Utils.TextBundle("alertitem2", "none");
                }
                else if ((checkName && !checkItewmCode && sysItemIDEdit == null) || (checkName && !checkItewmCode && sysItemIDEdit != null && txtItemName.Text != sysItemIDEdit.ItemName))
                {
                    message = txtItemName.Text + Utils.TextBundle("alertitem3", "none");
                }
                else if ((!checkName && checkItewmCode && sysItemIDEdit == null) || (!checkName && checkItewmCode && sysItemIDEdit != null && txtItemCode.Text != sysItemIDEdit.ItemCode))
                {
                    message = txtItemCode.Text + Utils.TextBundle("alertitem4", "none");
                }

                addItem.ItemName = txtItemName.Text.ToString();
                addItem.ItemCode = txtItemCode.Text ?? "";
                if (message!="")
                {
                    InsertRepeatItem insertRepeat = new InsertRepeatItem();
                    insertRepeat.checkManageStock = checkManageStock;
                    insertRepeat.DetailITem = addItem;
                    insertRepeat.Stock = lblStockNumber.Text;
                    insertRepeat.minimumstock = txtStockMinimum.Text;
                    var json = JsonConvert.SerializeObject(insertRepeat);
                    
                    //ชื่อซ้ำ
                    var okCancelAlertController = UIAlertController.Create("", message, UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => CheckEdit()));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => btnAdd.Enabled = true));
                    PresentViewController(okCancelAlertController, true, null);
                    return;
                }



                CheckEdit();
                
            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                btnAdd.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdditem_Click at add Item");
            }
        }
        void CheckEdit()
        {
            if (sysItemIDEdit != null) // update
            {
                addItem.DataStatus = 'M';
                UpdateItemToDB();
            }
            else // insert
            {
                addItemToDB();
            }
        }
        decimal ConvertToDecimal(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(txt);
            }
        }
        async  Task GetStockData()
        {
            try
            {
                feditstock = false;
                
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                //BalanceStock กับ MinimumStock
                
                if (SysItemId != 0 && await GabanaAPI.CheckNetWork())
                {
                    if (addItem.FTrackStock == 0)
                    {
                        fstockbefore = 0;
                        switchStock.On = false;

                        disableView.Hidden = false;
                    }
                    else
                    {
                        fstockbefore = 1;
                        switchStock.On = true;
                        disableView.Hidden = true;

                    }
                    var DataStock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (DataStock != null)
                    {
                        itemOnBranchedit = new ItemOnBranch()
                        {
                            MerchantID = DataStock.MerchantID,
                            SysBranchID = DataStock.SysBranchID,
                            SysItemID = DataStock.SysItemID,
                            BalanceStock = DataStock.BalanceStock,
                            MinimumStock = DataStock.MinimumStock,
                            LastDateBalanceStock = DataStock.LastDateBalanceStock
                        };
                        
                        var insert = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranchedit);
                        var getBalance = await onBranchManage.GetItemOnBranch((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId, (int)SysItemId);
                        lblStockNumber.Text = getBalance.BalanceStock.ToString("#,###");
                        txtStockMinimum.Text = getBalance.MinimumStock.ToString("#,###");

                        
                    }
                    else
                    {
                        disableView.Hidden = true;
                    }
                }
                else if (SysItemId != 0 & !await GabanaAPI.CheckNetWork())
                {
                    var getBalance = await onBranchManage.GetItemOnBranch((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (addItem.FTrackStock == 0)
                    {
                        switchStock.On = false;
                        switchStock.Enabled = false;
                        disableView.Hidden = true;
                        fstockbefore = 0;

                    }
                    else
                    {
                        fstockbefore = 1;
                        switchStock.On = true;
                        switchStock.Enabled = false; 
                        disableView.Hidden = false;

                    }
                    if (getBalance != null)
                    {
                        
                        lblStockNumber.Text = getBalance.BalanceStock.ToString();
                        lblStockMinimum.Text = getBalance.MinimumStock.ToString();
                    }
                    else
                    {
                        lblStockNumber.Text =  "0";
                        lblStockMinimum.Text = "0";
                        //disableView.Hidden = true;
                    }
                    
                   
                }
                else //SysItemId == 0 
                {

                    disableView.Hidden = true;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }
        [Export("OnHand:")]
        public void OnHand(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("editstock", ""));
            ItemStockOnHandController onHandPage = new ItemStockOnHandController("Item", lblStockNumber.Text);
            this.NavigationController.PushViewController(onHandPage,false);
        }
        [Export("min:")] 
        public void Min(UIGestureRecognizer sender)  
        { 
            txtStockMinimum.BecomeFirstResponder();
        }
        [Export("Detail:")]
        public void Detail(UIGestureRecognizer sender)
        {
            if (flagDetail == 0)
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                setDetailShow(true);
                flagDetail = 1;
            }
            else
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                setDetailShow(false);
                flagDetail = 0;
            }
        }
        [Export("Delete:")]
        public async void DeleteItem(UIGestureRecognizer sender)
        {
            var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "none"), UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                alert => Delete()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
            PresentViewController(okCancelAlertController, true, null);
        }
        public async void Delete()
        {
            try
            {
                Item itemDelete = new Item();
                itemDelete = await itemManager.GetItem(DataCashingAll.MerchantId, (int)SysItemId);
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 1;
                itemDelete.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                var update = await itemManager.UpdateItem(itemDelete);
                if (update)
                {
                    Editchange = false;
                    this.NavigationController.PopViewController(false);
                    ItemsController.Ismodify = true;
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "none"));

                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, (int)SysItemId);
                    }

                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "none"));
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        void setDetailShow(bool x)
        {
            if (x) // show
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                ItemCodeView.Hidden = false;
                CategoryView.Hidden = false;
                VatView.Hidden = false;
                CostView.Hidden = false;
                SizeView.Hidden = false;
                DisplayPOSView.Hidden = false;

            }
            else // close
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                ItemCodeView.Hidden = true;
                CategoryView.Hidden = true;
                VatView.Hidden = true;
                CostView.Hidden = true;
                SizeView.Hidden = true;
                DisplayPOSView.Hidden = true;
            }
        }
        public async void EditItemExSize()
        {
            try
            {
                int x = ExtraSizeCollectionView.VisibleCells.Count() - 1;
                foreach (ExtraSizeCollectionViewCell item in ExtraSizeCollectionView.VisibleCells)
                {
                    extraList[x].ExSizeName = item.SizeName;
                    if (string.IsNullOrEmpty(item.Price)) item.Price = "0";
                    extraList[x].Price = Convert.ToDecimal(item.Price);
                    if (string.IsNullOrEmpty(item.EstimateCost)) item.EstimateCost = "0";
                    extraList[x].EstimateCost = Convert.ToDecimal(item.EstimateCost);
                    x--;
                }
                List<ItemExSize> itemExSizes = new List<ItemExSize>();
                for (int i = 0; i < extraList.Count; i++)
                {
                    var newitemsize = new ORM.MerchantDB.ItemExSize()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        SysItemID = sysItemIDEdit.SysItemID,
                        ExSizeNo = i + 1,
                        ExSizeName = extraList[i].ExSizeName,
                        Price = Convert.ToDecimal(extraList[i].Price),
                        EstimateCost = Convert.ToDecimal(extraList[i].EstimateCost),
                        Comments = ""
                    };
                    itemExSizes.Add(newitemsize);
                }
                newlsItemExSize = itemExSizes;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                
                return;
            }
        }
        private async void UpdateItemExsize()
        {
            //กรณีที่แก้ไขข้อมูล ExSize ตอนกดแก้ไขข้อมูล Item 
            //ลบ ItemSize เดิมออกแล้ว Insert ใหม่
            try
            {

            
                var getItemSize = await extra.GetItemSize((int)sysItemIDEdit.MerchantID, (int)sysItemIDEdit.SysItemID);
                if (getItemSize.Count > 0)
                {
                    var checkResult = await extra.DeleteItemsize((int)sysItemIDEdit.MerchantID, (int)sysItemIDEdit.SysItemID);
                    if (!checkResult)
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotedit", "none"));
                        return;
                    }
                }

                var checkInsert = await extra.InsertListItemsize(newlsItemExSize);
                if (!checkInsert)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "none"));
                  //  Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "แก้ไขข้อมูลไม่สำเร็จ");
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "none"));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }
        private async void UpdateItemToDB()
        {
            try
            {

            
            if (editedImage != null)
            {
                var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
                var namepic = DateTime.UtcNow.Ticks.ToString();
                var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", namepic + ".png");
                var FullfilePath = Path.Combine(docFolder, libFolder, namepic + ".png");
                if (!Directory.Exists(libFolder))
                {
                    Directory.CreateDirectory(libFolder);
                }
                NSData data = thumbnail.AsPNG();
                var _picture = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                File.WriteAllBytes(FullfilePath, _picture);

                //var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                var libFolderthumbnail = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");
                var filePaththumbnail = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic + ".png");
                var FullfilePaththumbnail = Path.Combine(docFolder, libFolderthumbnail, namepic + ".png");
                if (!Directory.Exists(libFolderthumbnail))
                {
                    Directory.CreateDirectory(libFolderthumbnail);
                }
                NSData datathumbnail = thumbnail.AsPNG();
                var _picturethumbnail = new byte[datathumbnail.Length];
                System.Runtime.InteropServices.Marshal.Copy(datathumbnail.Bytes, _picturethumbnail, 0, Convert.ToInt32(_picturethumbnail.Length));
                File.WriteAllBytes(FullfilePaththumbnail, _picturethumbnail);

                addItem.ThumbnailLocalPath = filePaththumbnail;
                addItem.PictureLocalPath = filePath;
                addItem.ThumbnailPath = filePaththumbnail;
            }
            else
            {
                if (changecolor)
                {
                    //ลบรุปด้วย
                    addItem.ThumbnailLocalPath = null;
                    addItem.PictureLocalPath = null;
                    addItem.ThumbnailPath = null;
                    addItem.PicturePath = null;
                }
                else
                {
                    addItem.ThumbnailLocalPath = sysItemIDEdit.ThumbnailLocalPath;
                    addItem.PictureLocalPath = sysItemIDEdit.PictureLocalPath;
                    addItem.ThumbnailPath = sysItemIDEdit.ThumbnailPath;
                    addItem.PicturePath = sysItemIDEdit.PicturePath;
                    addItem.Colors = sysItemIDEdit.Colors;
                }
            }

            bool resultstock = false;
            if (feditstock)
            {
                if (switchStock.On && fstockbefore == 0)
                {
                    if (string.IsNullOrEmpty(lblStockNumber.Text) | string.IsNullOrEmpty(txtStockMinimum.Text))
                    {
                        Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                        return;
                    }

                    //int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock
                    resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)addItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)), ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)));
                    if (!resultstock)
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                        return;
                    }

                    itemOnBranch = new ItemOnBranch()
                    {
                        MerchantID = addItem.MerchantID,
                        SysBranchID = DataCashingAll.SysBranchId,
                        SysItemID = addItem.SysItemID,
                        BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)),
                        MinimumStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)),
                        LastDateBalanceStock = DateTime.UtcNow
                    };
                    addItem.FTrackStock = 1;
                }
                else if (!switchStock.On && fstockbefore == 1)
                {
                    resultstock = await UpdateClosetock((int)addItem.SysItemID);
                    if (!resultstock)
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                        btnAdd.Enabled = true;
                        return;
                    }
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = addItem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = addItem.SysItemID,
                            BalanceStock = 0,
                            MinimumStock = 0,
                            LastDateBalanceStock = DateTime.UtcNow
                        };
                        addItem.FTrackStock = 0;
                }
                else
                {
                    var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(DataCashingAll.SysBranchId, (int)addItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)), ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)));
                    if (!PostDataTrackStockAdjust.Status)
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                        btnAdd.Enabled = true;
                        return ;
                    }
                    else
                    {
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = addItem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = addItem.SysItemID,
                            BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)),
                            MinimumStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)),
                            LastDateBalanceStock = DateTime.UtcNow
                        };
                        //ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                        //var updateStock = await onBranchManage.UpdateItemOnBranch(itemOnBranch);

                    }
                }
                addItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
            }
            

            addItem.DataStatus = 'M';
            var resultAddSize = await  AddItemExSize();
            if (!resultAddSize)
            {
                Utils.ShowMessage(Utils.TextBundle("samenamesize", "ชื่อขนาดซ้ำกัน กรุณากรอกชื่อใหม่"));
                btnAdd.Enabled = true;
                return;
            }

            var result = await itemManager.UpdateItem(addItem);
            if (result)
            {
                EditItemExSize();
                UpdateItemExsize();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)sysItemIDEdit.MerchantID, (int)sysItemIDEdit.SysItemID);
                }
                else
                {
                    addItem.FWaitSending = 2;
                    await itemManager.UpdateItem(addItem);
                }
                if (feditstock)
                {
                    ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                    var updateStock = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);
                }

                ItemsController.Ismodify = true;
                Editchange = false;
                this.NavigationController.PopViewController(false);
                Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "none"));
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("cannotedit", "none"));
                btnAdd.Enabled = true;
                return;
            }
            //Test get Item
            var getItem = await itemManager.GetItem(DataCashingAll.MerchantId, (int)SysItemId);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        async Task<bool> UpdateOpenStock(int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock)
        {
            try
            {

            
            ItemManage = new ItemManage();
            //Post/Open การเปิดระบบ Track Stock
            var PostDataTrackStockOpen = await GabanaAPI.PostDataTrackStockOpen(sysItemID, deviceNo);
            if (PostDataTrackStockOpen.Status)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if ("Item has stock tracking already." == PostDataTrackStockOpen.Message)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Utils.ShowMessage(PostDataTrackStockOpen.Message);

                //Toast.MakeText(this, PostDataTrackStockOpen.Message, ToastLength.Long).Show();
                return false;
            }
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return false; 
            }
        }

        async Task<bool> UpdateClosetock(int sysitem)
        {
            //Post/Close เป็นการปิดระบบ Track Stock
            try
            {

            
            var PostDataTrackStockClose = await GabanaAPI.PostDataTrackStockClose(sysitem, (int)DataCashingAll.DeviceNo);
            if (PostDataTrackStockClose.Status)
            {
                //lnSwithStcok.Visibility = ViewStates.Gone;
                return true;
            }
            else
            {
                Utils.ShowMessage(PostDataTrackStockClose.Message);
               
                return false;
            }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return false;
            }
        }
        private async void addItemToDB()
        {
            try
            {

            
            if (editedImage != null)
            {
                var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
                var namepic = DateTime.UtcNow.Ticks.ToString();
                var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", namepic + ".png");
                var FullfilePath = Path.Combine(docFolder, libFolder, namepic + ".png");
                if (!Directory.Exists(libFolder))
                {
                    Directory.CreateDirectory(libFolder);
                }
                NSData data = editedImage.AsPNG();
                var _picture = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                File.WriteAllBytes(FullfilePath, _picture);

                //var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                var libFolderthumbnail = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");
                var filePaththumbnail = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic + ".png");
                var FullfilePaththumbnail = Path.Combine(docFolder, libFolderthumbnail, namepic + ".png");
                if (!Directory.Exists(libFolderthumbnail))
                {
                    Directory.CreateDirectory(libFolderthumbnail);
                }
                NSData datathumbnail = editedImage.AsPNG();
                var _picturethumbnail = new byte[datathumbnail.Length];
                System.Runtime.InteropServices.Marshal.Copy(datathumbnail.Bytes, _picturethumbnail, 0, Convert.ToInt32(_picturethumbnail.Length));
                File.WriteAllBytes(FullfilePaththumbnail, _picturethumbnail);

                addItem.ThumbnailLocalPath = filePaththumbnail;
                addItem.PictureLocalPath = filePath;
                //addItem.PicturePath = filePath;
                //addItem.ThumbnailPath = filePaththumbnail;
            }
            else
            {

                addItem.ThumbnailLocalPath = null;
                addItem.PictureLocalPath = null;
                addItem.PicturePath = null;
                addItem.ThumbnailPath = null;
            }

                SysItemId = addItem.SysItemID;
                var resultAddSize = await AddItemExSize();
            if (!resultAddSize)
            {
                Utils.ShowMessage(Utils.TextBundle("samenamesize", "samenamesize"));
                btnAdd.Enabled = true;
                return;
            }

            var result = await itemManager.InsertItem(addItem, itemOnBranch, itemExSizes);
            if (!result)
            {
                Utils.ShowMessage(Utils.TextBundle("cannotsave", "cannotsave"));
                btnAdd.Enabled = true;
                return;
            }

            //SysItemId = addItem.SysItemID;
            
            ItemsController.Ismodify = true;

            if (await GabanaAPI.CheckNetWork())
            {
                JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
            }
            else
            {
                addItem.FWaitSending = 2;
                await itemManager.UpdateItem(addItem);
            }
            DataCaching.NewItem = addItem.SysItemID;
            POSController.newitem = true;
            POSController.idnewitem = addItem.SysItemID;
            //if (POSController != null)
            //{
            //    POSController.
            //}
            Editchange = false; 
            this.NavigationController.PopViewController(false);
            Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "none"));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public async Task<bool> AddItemExSize()
        {
            try
            {
                
                int x = ExtraSizeCollectionView.VisibleCells.Count() -1;
                foreach (ExtraSizeCollectionViewCell item in ExtraSizeCollectionView.VisibleCells)
                {
                    if (!string.IsNullOrEmpty(item.SizeName.Trim()))
                    {
                        extraList[x].ExSizeName = item.SizeName;
                        if (string.IsNullOrEmpty(item.Price)) item.Price = "0";
                        extraList[x].Price = Convert.ToDecimal(item.Price);
                        if (string.IsNullOrEmpty(item.EstimateCost)) item.EstimateCost = "0";
                        extraList[x].EstimateCost = Convert.ToDecimal(item.EstimateCost);
                    }
                    x--;
                        
                }
                
                itemExSizes = new List<ItemExSize>();
                for (int i = 0; i <extraList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(extraList[i].ExSizeName.Trim()))
                    {
                        var newitemsize = new ORM.MerchantDB.ItemExSize()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SysItemID = SysItemId,
                            ExSizeNo = i + 1,
                            ExSizeName = extraList[i].ExSizeName,
                            Price = Convert.ToDecimal(extraList[i].Price),
                            EstimateCost = Convert.ToDecimal(extraList[i].EstimateCost),
                            Comments = ""
                        };
                        itemExSizes.Add(newitemsize);
                    }
                }

                //Check SizeName ห้ามซ้ำกันภายในสินค้าตัวเดียวกัน               
                for (int i = 0; i < itemExSizes.Count; i++)
                {
                    for (int j = 0; j < itemExSizes.Count; j++)
                    {
                        if (i != j)
                        {
                            var checkk = string.Compare(itemExSizes[i].ExSizeName, itemExSizes[j].ExSizeName);
                            if (checkk == 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                //var result = await extra.InsertListItemsize(itemExSizes);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotsave", "none"));
                return false;
            }
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void setupAutoLayout()
        {
            try
            {

            
            menuAddItemBarCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            menuAddItemBarCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            menuAddItemBarCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            menuAddItemBarCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region bottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnAdd.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAdd.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnAdd.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;

            btnDeleteView.CenterYAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDeleteView.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnDeleteView.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDeleteView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            btnDelete.CenterYAnchor.ConstraintEqualTo(btnDeleteView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnDelete.CenterXAnchor.ConstraintEqualTo(btnDeleteView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnDelete.HeightAnchor.ConstraintEqualTo(24).Active = true;

            #endregion

            #region StockViewLayout
            stockView.TopAnchor.ConstraintEqualTo(menuAddItemBarCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            stockView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            stockView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            stockView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor).Active = true;

            StockbarView.TopAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            StockbarView.RightAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            StockbarView.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            StockbarView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            disableView.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            disableView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            disableView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            disableView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            lblStock.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.TopAnchor, 13).Active = true;
            lblStock.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblStock.LeftAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.LeftAnchor, 30).Active = true;
            lblStock.HeightAnchor.ConstraintEqualTo(18).Active = true;

            switchStock.CenterYAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            switchStock.WidthAnchor.ConstraintEqualTo(52).Active = true;
            switchStock.RightAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.RightAnchor, -22).Active = true;
            switchStock.HeightAnchor.ConstraintEqualTo(32).Active = true;

            lbltxtOnHand.BottomAnchor.ConstraintEqualTo(lblStockNumber.SafeAreaLayoutGuide.TopAnchor, -5).Active = true;
            lbltxtOnHand.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lbltxtOnHand.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblStockNumber.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor, 35).Active = true;
            lblStockNumber.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblStockNumber.HeightAnchor.ConstraintEqualTo(54).Active = true;

            line3.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor, 120).Active = true;
            line3.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(5).Active = true;

            lblStockMinimum.TopAnchor.ConstraintEqualTo(line3.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblStockMinimum.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblStockMinimum.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtStockMinimum.TopAnchor.ConstraintEqualTo(lblStockMinimum.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtStockMinimum.RightAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtStockMinimum.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;

            line4.TopAnchor.ConstraintEqualTo(txtStockMinimum.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            line4.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line4.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line4.HeightAnchor.ConstraintEqualTo(5).Active = true;

            viewstockmove.TopAnchor.ConstraintEqualTo(line4.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            viewstockmove.RightAnchor.ConstraintEqualTo(stockView .SafeAreaLayoutGuide.RightAnchor,15).Active = true;
            viewstockmove.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            viewstockmove.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblStockmove.CenterYAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblStockmove.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblStockmove.LeftAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            lblStockmove.HeightAnchor.ConstraintEqualTo(20).Active = true;

            imgSelectStockmove.CenterYAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imgSelectStockmove.WidthAnchor.ConstraintEqualTo(28).Active = true;
            imgSelectStockmove.RightAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.RightAnchor,-25).Active = true;
            imgSelectStockmove.HeightAnchor.ConstraintEqualTo(28).Active = true;


            lineEmpty.TopAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            lineEmpty.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lineEmpty.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lineEmpty.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            #endregion

            #region ItemViewLayout
            _scrollView.TopAnchor.ConstraintEqualTo(menuAddItemBarCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region ImageCardView
            imageView.TopAnchor.ConstraintEqualTo(imageView.Superview.TopAnchor, 0).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(150).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(imageView.Superview.LeftAnchor, (View.Frame.Width - 370) / 2).Active = true;
            imageView.RightAnchor.ConstraintEqualTo(setColorView.LeftAnchor, 0).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(154).Active = true;
            //imageView.BackgroundColor = UIColor.Black;

            itemCardViewTop.TopAnchor.ConstraintEqualTo(itemCardViewTop.Superview.TopAnchor, 0).Active = true;
            itemCardViewTop.RightAnchor.ConstraintEqualTo(itemCardViewTop.Superview.RightAnchor,0).Active = true;
            itemCardViewTop.LeftAnchor.ConstraintEqualTo(itemCardViewTop.Superview.LeftAnchor, 0).Active = true;
            itemCardViewTop.BottomAnchor.ConstraintEqualTo(itemCardFooter.TopAnchor).Active = true;

            itemCardView.TopAnchor.ConstraintEqualTo(itemCardView.Superview.TopAnchor, 15).Active = true;
            itemCardView.BottomAnchor.ConstraintEqualTo(itemCardView.Superview.BottomAnchor, -15).Active = true;
            itemCardView.LeftAnchor.ConstraintEqualTo(itemCardView.Superview.LeftAnchor, 15).Active = true;
            itemCardView.RightAnchor.ConstraintEqualTo(itemCardView.Superview.RightAnchor, -15).Active = true;

            lblItemCardName.TopAnchor.ConstraintEqualTo(itemCardFooter.SafeAreaLayoutGuide.TopAnchor, 3).Active = true;
            //lblItemCardName.WidthAnchor.ConstraintEqualTo(112).Active = true;
            lblItemCardName.LeftAnchor.ConstraintEqualTo(lblItemCardName.Superview.LeftAnchor, 5).Active = true;
            lblItemCardName.RightAnchor.ConstraintEqualTo(lblItemCardName.Superview.RightAnchor, -5).Active = true;

            lblItemCardName.HeightAnchor.ConstraintEqualTo(15).Active = true;

            LblItemCardPrice.TopAnchor.ConstraintEqualTo(lblItemCardName.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            //LblItemCardPrice.WidthAnchor.ConstraintEqualTo(112).Active = true;
            LblItemCardPrice.LeftAnchor.ConstraintEqualTo(LblItemCardPrice.Superview.LeftAnchor, 5).Active = true;
            LblItemCardPrice.LeftAnchor.ConstraintEqualTo(LblItemCardPrice.Superview.RightAnchor, -5).Active = true;
            LblItemCardPrice.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblItemCardShortName.CenterXAnchor.ConstraintEqualTo(lblItemCardShortName.Superview.CenterXAnchor).Active = true;
            lblItemCardShortName.CenterYAnchor.ConstraintEqualTo(lblItemCardShortName.Superview.CenterYAnchor, -20).Active = true;
            lblItemCardShortName.HeightAnchor.ConstraintEqualTo(29).Active = true;

            itemCardFooter.HeightAnchor.ConstraintEqualTo(40).Active = true;
            itemCardFooter.TopAnchor.ConstraintEqualTo(lblItemCardShortName.SafeAreaLayoutGuide.BottomAnchor, 26).Active = true;
            itemCardFooter.BottomAnchor.ConstraintEqualTo(itemCardFooter.Superview.BottomAnchor, 0).Active = true;
            itemCardFooter.LeftAnchor.ConstraintEqualTo(itemCardFooter.Superview.LeftAnchor, 0).Active = true;
            itemCardFooter.RightAnchor.ConstraintEqualTo(itemCardFooter.Superview.RightAnchor, 0).Active = true;

            #endregion

            #region setItemNameView
            setItemNameView.TopAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            setItemNameView.RightAnchor.ConstraintEqualTo(setItemNameView.Superview.RightAnchor, 0).Active = true;
            setItemNameView.LeftAnchor.ConstraintEqualTo(setItemNameView.Superview.LeftAnchor, 0).Active = true;
            setItemNameView.WidthAnchor.ConstraintEqualTo(200).Active = true; 
            setItemNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemName.TopAnchor.ConstraintEqualTo(lblItemName.Superview.TopAnchor, 11).Active = true;
            lblItemName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblItemName.LeftAnchor.ConstraintEqualTo(lblItemName.Superview.LeftAnchor, 15).Active = true;
            lblItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemName.TopAnchor.ConstraintEqualTo(lblItemName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtItemName.LeftAnchor.ConstraintEqualTo(txtItemName.Superview.LeftAnchor, 15).Active = true;
            txtItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region SetItemPriceView
            SetItemPriceView.TopAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SetItemPriceView.RightAnchor.ConstraintEqualTo(SetItemPriceView.Superview.RightAnchor, 0).Active = true;
            SetItemPriceView.LeftAnchor.ConstraintEqualTo(SetItemPriceView.Superview.LeftAnchor, 0).Active = true;
            SetItemPriceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemPrice.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemPrice.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblItemPrice.LeftAnchor.ConstraintEqualTo(lblItemPrice.Superview.LeftAnchor, 15).Active = true;
            lblItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemPrice.TopAnchor.ConstraintEqualTo(lblItemPrice.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemPrice.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtItemPrice.LeftAnchor.ConstraintEqualTo(txtItemPrice.Superview.LeftAnchor, 15).Active = true;
            txtItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region detail
            DetailClickView.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            DetailClickView.RightAnchor.ConstraintEqualTo(DetailClickView.Superview.RightAnchor, 0).Active = true;
            DetailClickView.LeftAnchor.ConstraintEqualTo(DetailClickView.Superview.LeftAnchor, 0).Active = true;
            DetailClickView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblDetail.TopAnchor.ConstraintEqualTo(lblDetail.Superview.TopAnchor, 11).Active = true;
            lblDetail.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblDetail.LeftAnchor.ConstraintEqualTo(lblDetail.Superview.LeftAnchor, 15).Active = true;
            lblDetail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnToggleDetail.TopAnchor.ConstraintEqualTo(btnToggleDetail.Superview.TopAnchor, 11).Active = true;
            btnToggleDetail.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnToggleDetail.RightAnchor.ConstraintEqualTo(btnToggleDetail.Superview.RightAnchor, -20).Active = true;
            btnToggleDetail.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #region ItemCodeView
            ItemCodeView.TopAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            ItemCodeView.RightAnchor.ConstraintEqualTo(ItemCodeView.Superview.RightAnchor, 0).Active = true;
            ItemCodeView.LeftAnchor.ConstraintEqualTo(ItemCodeView.Superview.LeftAnchor, 0).Active = true;
            ItemCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemCode.TopAnchor.ConstraintEqualTo(lblItemCode.Superview.TopAnchor, 11).Active = true;
            lblItemCode.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblItemCode.LeftAnchor.ConstraintEqualTo(lblItemCode.Superview.LeftAnchor, 15).Active = true;

            txtItemCode.TopAnchor.ConstraintEqualTo(lblItemCode.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemCode.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtItemCode.LeftAnchor.ConstraintEqualTo(txtItemCode.Superview.LeftAnchor, 15).Active = true;

            btnScan.CenterYAnchor.ConstraintEqualTo(ItemCodeView.CenterYAnchor).Active = true;
            btnScan.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnScan.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnScan.RightAnchor.ConstraintEqualTo(ItemCodeView.RightAnchor , -20).Active = true;
            #endregion
            #region CostView
            CostView.TopAnchor.ConstraintEqualTo(ItemCodeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CostView.RightAnchor.ConstraintEqualTo(CostView.Superview.RightAnchor, 0).Active = true;
            CostView.LeftAnchor.ConstraintEqualTo(CostView.Superview.LeftAnchor, 0).Active = true;
            CostView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemCost.TopAnchor.ConstraintEqualTo(lblItemCost.Superview.TopAnchor, 11).Active = true;
            lblItemCost.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblItemCost.LeftAnchor.ConstraintEqualTo(lblItemCost.Superview.LeftAnchor, 15).Active = true;

            txtItemCost.TopAnchor.ConstraintEqualTo(lblItemCost.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemCost.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtItemCost.LeftAnchor.ConstraintEqualTo(txtItemCost.Superview.LeftAnchor, 15).Active = true;
            #endregion
            #region CategoryView
            CategoryView.TopAnchor.ConstraintEqualTo(CostView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CategoryView.RightAnchor.ConstraintEqualTo(CategoryView.Superview.RightAnchor, 0).Active = true;
            CategoryView.LeftAnchor.ConstraintEqualTo(CategoryView.Superview.LeftAnchor, 0).Active = true;
            CategoryView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblCategory.TopAnchor.ConstraintEqualTo(lblCategory.Superview.TopAnchor, 11).Active = true;
            lblCategory.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblCategory.LeftAnchor.ConstraintEqualTo(lblCategory.Superview.LeftAnchor, 15).Active = true;

            lblSelectedCategory.TopAnchor.ConstraintEqualTo(lblCategory.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblSelectedCategory.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblSelectedCategory.LeftAnchor.ConstraintEqualTo(lblSelectedCategory.Superview.LeftAnchor, 15).Active = true;

            btnSelectCategory.CenterYAnchor.ConstraintEqualTo(btnSelectCategory.Superview.CenterYAnchor).Active = true;
            btnSelectCategory.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectCategory.RightAnchor.ConstraintEqualTo(btnSelectCategory.Superview.RightAnchor, -25).Active = true;
            btnSelectCategory.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region VatView
            VatView.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            VatView.RightAnchor.ConstraintEqualTo(VatView.Superview.RightAnchor, 0).Active = true;
            VatView.LeftAnchor.ConstraintEqualTo(VatView.Superview.LeftAnchor, 0).Active = true;
            VatView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblVat.TopAnchor.ConstraintEqualTo(lblVat.Superview.TopAnchor, 11).Active = true;
            lblVat.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblVat.LeftAnchor.ConstraintEqualTo(lblVat.Superview.LeftAnchor, 15).Active = true;

            lblVatMode.TopAnchor.ConstraintEqualTo(lblVat.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblVatMode.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblVatMode.LeftAnchor.ConstraintEqualTo(lblVatMode.Superview.LeftAnchor, 15).Active = true;

            btnSelectVatType.CenterYAnchor.ConstraintEqualTo(btnSelectVatType.Superview.CenterYAnchor).Active = true;
            btnSelectVatType.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectVatType.RightAnchor.ConstraintEqualTo(btnSelectVatType.Superview.RightAnchor, -25).Active = true;
            btnSelectVatType.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region SizeView
            SizeView.TopAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            SizeView.RightAnchor.ConstraintEqualTo(SizeView.Superview.RightAnchor, 0).Active = true;
            SizeView.LeftAnchor.ConstraintEqualTo(SizeView.Superview.LeftAnchor, 0).Active = true;
            SizeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSize.CenterYAnchor.ConstraintEqualTo(lblSize.Superview.CenterYAnchor).Active = true;
            lblSize.LeftAnchor.ConstraintEqualTo(lblSize.Superview.LeftAnchor, 15).Active = true;
            lblSize.WidthAnchor.ConstraintEqualTo(50).Active = true;

            lblMaxSize.CenterYAnchor.ConstraintEqualTo(lblSize.Superview.CenterYAnchor).Active = true;
            lblMaxSize.LeftAnchor.ConstraintEqualTo(lblSize.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lblMaxSize.WidthAnchor.ConstraintEqualTo(150).Active = true;

            btnAddSize.CenterYAnchor.ConstraintEqualTo(lblSize.Superview.CenterYAnchor).Active = true;
            btnAddSize.RightAnchor.ConstraintEqualTo(btnAddSize.Superview.RightAnchor, -21).Active = true;
            btnAddSize.WidthAnchor.ConstraintEqualTo(100).Active = true;
            btnAddSize.HeightAnchor.ConstraintEqualTo(35).Active = true;

            #endregion
            #region SizeCollection
            ExtraSizeCollectionView.TopAnchor.ConstraintEqualTo(SizeView.BottomAnchor, 1).Active = true;
            ExtraSizeCollectionView.RightAnchor.ConstraintEqualTo(ExtraSizeCollectionView.Superview.RightAnchor, 0).Active = true;
            ExtraSizeCollectionView.LeftAnchor.ConstraintEqualTo(ExtraSizeCollectionView.Superview.LeftAnchor, 0).Active = true;
            ExtraSizeCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
           
            #endregion
            #region DisplayPOSView
            DisplayPOSView.TopAnchor.ConstraintEqualTo(ExtraSizeCollectionView.BottomAnchor, 0.4f).Active = true;
            DisplayPOSView.RightAnchor.ConstraintEqualTo(DisplayPOSView.Superview.RightAnchor, 0).Active = true;
            DisplayPOSView.LeftAnchor.ConstraintEqualTo(DisplayPOSView.Superview.LeftAnchor, 0).Active = true;
            DisplayPOSView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            DisplayPOSView.BottomAnchor.ConstraintEqualTo(DisplayPOSView.Superview.BottomAnchor, 0).Active = true;

            lblDiaplayPOS.TopAnchor.ConstraintEqualTo(lblDiaplayPOS.Superview.TopAnchor, 11).Active = true;
            lblDiaplayPOS.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblDiaplayPOS.LeftAnchor.ConstraintEqualTo(lblDiaplayPOS.Superview.LeftAnchor, 15).Active = true;

            lblDisplaytext.TopAnchor.ConstraintEqualTo(lblDiaplayPOS.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblDisplaytext.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblDisplaytext.LeftAnchor.ConstraintEqualTo(lblDisplaytext.Superview.LeftAnchor, 15).Active = true;

            DisplaySwitch.CenterYAnchor.ConstraintEqualTo(DisplaySwitch.Superview.CenterYAnchor).Active = true;
            DisplaySwitch.WidthAnchor.ConstraintEqualTo(51).Active = true;
            DisplaySwitch.RightAnchor.ConstraintEqualTo(DisplaySwitch.Superview.RightAnchor, -15).Active = true;
            DisplaySwitch.HeightAnchor.ConstraintEqualTo(31).Active = true;
            #endregion
            #endregion

            #endregion
            View.BringSubviewToFront(bottomView);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        void setColorButton()
        {
            setColorView.TopAnchor.ConstraintEqualTo(setColorView.Superview.TopAnchor, 0).Active = true;
            setColorView.RightAnchor.ConstraintEqualTo(setColorView.Superview.RightAnchor, 0).Active = true;
            setColorView.WidthAnchor.ConstraintEqualTo(220).Active = true;
            setColorView.HeightAnchor.ConstraintEqualTo(154).Active = true;

            //btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
            #region Buttom Blue
            btnColor1 = new UIButton();
            btnColor1.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnColor1.Layer.CornerRadius = 3;
            btnColor1.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor1.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Blue
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                itemCardViewTop.Image = null;
                addColor = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor1);

            btnColor1.TopAnchor.ConstraintEqualTo(btnColor1.Superview.TopAnchor, 15).Active = true;
            btnColor1.LeftAnchor.ConstraintEqualTo(btnColor1.Superview.LeftAnchor, 10).Active = true;
            btnColor1.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor1.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Yellow
            btnColor2 = new UIButton();
            btnColor2.BackgroundColor = UIColor.FromRGB(248, 151, 29);
            btnColor2.Layer.CornerRadius = 3;
            btnColor2.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor2.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Yellow
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(248, 151, 29);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(248, 151, 29);
                addColor = int.Parse("F8971D", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor2);

            btnColor2.TopAnchor.ConstraintEqualTo(btnColor2.Superview.TopAnchor, 15).Active = true;
            btnColor2.LeftAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor2.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor2.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Red
            btnColor3 = new UIButton();
            btnColor3.BackgroundColor = UIColor.FromRGB(227, 45, 73);
            btnColor3.Layer.CornerRadius = 3;
            btnColor3.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor3.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Red
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(227, 45, 73);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(227, 45, 73);
                addColor = int.Parse("E32D49", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor3);

            btnColor3.TopAnchor.ConstraintEqualTo(btnColor3.Superview.TopAnchor, 15).Active = true;
            btnColor3.LeftAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor3.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor3.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Green
            btnColor4 = new UIButton();
            btnColor4.BackgroundColor = UIColor.FromRGB(55, 172, 82);
            btnColor4.Layer.CornerRadius = 3;
            btnColor4.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor4.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Green
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(55, 172, 82);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(55, 172, 82);
                addColor = int.Parse("37AA52", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor4);

            btnColor4.TopAnchor.ConstraintEqualTo(btnColor4.Superview.TopAnchor, 15).Active = true;
            btnColor4.LeftAnchor.ConstraintEqualTo(btnColor3.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor4.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor4.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Orange
            btnColor5 = new UIButton();
            btnColor5.BackgroundColor = UIColor.FromRGB(247, 86, 0);
            btnColor5.Layer.CornerRadius = 3;
            btnColor5.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor5.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Orange
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(247, 86, 0);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(247, 86, 0);
                addColor = int.Parse("F75600", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor5);

            btnColor5.TopAnchor.ConstraintEqualTo(btnColor5.Superview.TopAnchor, 15).Active = true;
            btnColor5.LeftAnchor.ConstraintEqualTo(btnColor4.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor5.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor5.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region Buttom Purple
            btnColor6 = new UIButton();
            btnColor6.BackgroundColor = UIColor.FromRGB(63, 81, 181);
            btnColor6.Layer.CornerRadius = 3;
            btnColor6.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor6.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Purple
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(63, 81, 181);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(63, 81, 181);
                addColor = int.Parse("3F51B5", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor6);

            btnColor6.TopAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor6.LeftAnchor.ConstraintEqualTo(btnColor6.Superview.LeftAnchor, 10).Active = true;
            btnColor6.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor6.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom DarkGreen
            btnColor7 = new UIButton();
            btnColor7.BackgroundColor = UIColor.FromRGB(0, 121, 107);
            btnColor7.Layer.CornerRadius = 3;
            btnColor7.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor7.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //Dark Green
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 121, 107);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 121, 107);
                addColor = int.Parse("00796B", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor7);

            btnColor7.TopAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor7.LeftAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor7.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor7.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom LightGreen
            btnColor8 = new UIButton();
            btnColor8.BackgroundColor = UIColor.FromRGB(139, 195, 74);
            btnColor8.Layer.CornerRadius = 3;
            btnColor8.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor8.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //LightGreen
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(139, 195, 74);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(139, 195, 74);
                addColor = int.Parse("8BC34A", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor8);

            btnColor8.TopAnchor.ConstraintEqualTo(btnColor3.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor8.LeftAnchor.ConstraintEqualTo(btnColor7.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor8.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor8.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Pink
            btnColor9 = new UIButton();
            btnColor9.BackgroundColor = UIColor.FromRGB(221, 82, 126);
            btnColor9.Layer.CornerRadius = 3;
            btnColor9.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor9.TouchUpInside += (sender, e) =>
            {
                Editchange = true;
                //PINK
                lblItemCardShortName.Hidden = false;
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardView.BackgroundColor = UIColor.FromRGB(221, 82, 126);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(221, 82, 126);
                addColor = int.Parse("DD527E", System.Globalization.NumberStyles.HexNumber);
                changecolor = true;
                editedImage = null;
            };
            setColorView.AddSubview(btnColor9);

            btnColor9.TopAnchor.ConstraintEqualTo(btnColor4.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor9.LeftAnchor.ConstraintEqualTo(btnColor8.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor9.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor9.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region Buttom ChangePIC
            btnChangeItemPhoto = new UIButton();
            btnChangeItemPhoto.SetImage(UIImage.FromBundle("Album"), UIControlState.Normal);
            btnChangeItemPhoto.Layer.CornerRadius = 3;
            btnChangeItemPhoto.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangeItemPhoto.TouchUpInside += (sender, e) =>
            {

                //btnChangeItemPhoto
                addColor = 0;
                #region PhotoEditActionSheet

                selectPhotoMenuSheet = UIAlertController.Create("Add Logo" , null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Take a picture" , UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Choose your picture" , UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.PresentModalViewController(selectPhotoMenuSheet, true);
                #endregion
            };
            setColorView.AddSubview(btnChangeItemPhoto);

            btnChangeItemPhoto.TopAnchor.ConstraintEqualTo(btnColor5.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnChangeItemPhoto.LeftAnchor.ConstraintEqualTo(btnColor9.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnChangeItemPhoto.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnChangeItemPhoto.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region fav
            favImg = new UIImageView();
            favImg.TranslatesAutoresizingMaskIntoConstraints = false;
            favImg.Image = UIImage.FromBundle("Unfav"); // Fav
            setColorView.AddSubview(favImg);

            favImg.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("FavClick:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            favImg.AddGestureRecognizer(tapGesture0);


            lblFav = new UILabel();
            lblFav.Font = lblFav.Font.WithSize(16);
            lblFav.TextColor = UIColor.FromRGB(64,64,64);
            lblFav.Lines = 1;
            lblFav.Text =Utils.TextBundle("favorite", "Favorite");
            lblFav.TextAlignment = UITextAlignment.Left;
            lblFav.TranslatesAutoresizingMaskIntoConstraints = false;
            setColorView.AddSubview(lblFav);

            favImg.TopAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            favImg.LeftAnchor.ConstraintEqualTo(favImg.Superview.LeftAnchor, 10).Active = true;
            favImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            favImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblFav.TopAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            lblFav.LeftAnchor.ConstraintEqualTo(favImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblFav.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblFav.RightAnchor.ConstraintEqualTo(lblFav.Superview.RightAnchor,-15).Active = true;
            #endregion
        }
        [Export("FavClick:")]
        public void FavClick(UIGestureRecognizer sender)
        {
            //check if it's fav or not.
            if (favFlag == true)
            {
                favFlag = false;
                favImg.Image = UIImage.FromBundle("Unfav"); // Fav
            }
            else
            {
                favFlag = true;
                favImg.Image = UIImage.FromBundle("Fav"); // Fav
            }
        }
        private void Pic(string v)
        {

            
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                if (Utils.Checkpermisstion())
                {
                    //imagePicker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
                    imagePicker.AllowsImageEditing = true;
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    //imagePicker.AllowsEditing = true;
                    
                    imagePicker.AutomaticallyAdjustsScrollViewInsets = true;
                    imagePicker.ModalPresentationCapturesStatusBarAppearance = true;
                    imagePicker.PreferredContentSize = new CGSize(320, 320);
                    imagePicker.ContentSizeForViewInPopover = new CGSize(320,320);
                    //var x = imagePicker.CropRect;
                    imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
                    imagePicker.Canceled += Handle_Canceled;
                    imagePicker.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                    this.NavigationController.PresentModalViewController(imagePicker, true);
                }


            }
            else
            {
                
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                imagePicker.AllowsEditing = true;
                imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
                imagePicker.Canceled += Handle_Canceled;
                imagePicker.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                this.NavigationController.PresentModalViewController(imagePicker, true);

            }


        }
        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
            lblItemCardShortName.Hidden = false;
        }
        protected void Handle_Finishedtake(object sender, UIImagePickerMediaPickedEventArgs e)
        {
        }
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            Editchange = true;
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {
                var x = e.Info[UIImagePickerController.OriginalImage];

                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {

                    originalImage.Scale(new CoreGraphics.CGSize(200, 200));
                    nfloat quality = (nfloat)0.7;
                    // do something with the image
                    itemCardView.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = originalImage.AsJPEG(xx).AsStream();
                }

                editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the edited image");
                    nfloat quality = (nfloat)0.7;
                    itemCardView.Image = editedImage;
                    
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = imageprofile.Image.AsJPEG(quality).AsStream();

                }

                changecolor = false;

            }
            itemCardViewTop.Hidden = true;
            imagePicker.DismissModalViewController(true);
            lblItemCardShortName.Hidden = true;
            itemCardFooter.BackgroundColor = UIColor.FromRGB(162, 162, 162);
            itemCardFooter.Layer.Opacity = 1f;
            itemCardFooter.BackgroundColor = UIColor.Black;
            itemCardFooter.Layer.Opacity = 0.2f;
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        //public static Stream ToStream(uiImage image, ImageFormat format)
        //{
        //    var stream = new System.IO.MemoryStream();
        //    image.Save(stream, format);
        //    stream.Position = 0;
        //    return stream;
        //}
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class PickerCategoryModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public long ID { get; set; }
                public string CategoryName { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;


            private readonly List<Category> values;
            public PickerCategoryModel(List<Category> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)].Name;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].SysCategoryID.ToString(),
                        ID = values[Convert.ToInt32(row)].SysCategoryID,
                        CategoryName = values[Convert.ToInt32(row)].Name
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }
        }
        public class PickerModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public string Vat { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private readonly List<string> values;
            public PickerModel(List<string> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)];
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].ToString(),
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }
        }

        private readonly List<string> VatType = new List<string>
        {
            Utils.TextBundle("havevat", "Have Vat"),
            Utils.TextBundle("nonevat", "None Vat")
        };

        private async void setCategory()
        {
            try
            {
                CategoryManage categoryManage = new CategoryManage();
                Category addcategory = new Category();
                var category = new List<Category>();
                var getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = Utils.TextBundle("none", "None"),
                    SysCategoryID = 0
                };
                category.Add(addcategory);
                getallCategory = await categoryManage.GetAllCategory();
                if(getallCategory!=null)
                {
                    category.AddRange(getallCategory);
                }

                PickerCategoryModel model2 = new PickerCategoryModel(category);
                if(sysItemIDEdit!=null && sysItemIDEdit.SysCategoryID!=null)
                {
                    lblSelectedCategory.Text = category.Where(x=>x.SysCategoryID == sysItemIDEdit.SysCategoryID).FirstOrDefault().Name;
                    idCategory = (int)sysItemIDEdit.SysCategoryID; 
                }
                else
                {
                    lblSelectedCategory.Text = category[0].Name;
                    idCategory = (int)category[0].SysCategoryID;
                }

                if (SelectCategory > 0)
                {
                    lblSelectedCategory.Text = category.Where(x => x.SysCategoryID == SelectCategory).FirstOrDefault().Name;
                    idCategory = SelectCategory;
                    SelectCategory = 0;
                }

                model2.PickerChanged += async (sender, e) =>
                {
                    lblSelectedCategory.Text = e.CategoryName;
                    idCategory = (int)e.ID;
                    Editchange = true;
                };

                UIToolbar toolbar = new UIToolbar();
                toolbar.BarStyle = UIBarStyle.Default;
                toolbar.Translucent = true;
                toolbar.SizeToFit();
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton = new UIBarButtonItem(Utils.TextBundle("done", "none"), UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("CategoryAction"));
                toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
                var xy = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
                lblSelectedCategory.InputView = xy;
                if (category != null)
                {
                    if (sysItemIDEdit==null || sysItemIDEdit.SysCategoryID == null)
                    {
                    //    lblSelectedCategory.Text = category[0].Name;
                        xy.Select(0, 0, false);
                    }
                    else
                    {
                     //   lblSelectedCategory.Text = category.Where(x => x.SysCategoryID == sysItemIDEdit.SysCategoryID).FirstOrDefault().Name;
                        CatID = (int)category.FindIndex(x => x.SysCategoryID == sysItemIDEdit.SysCategoryID);
                        xy.Select((int)CatID, 0, false);
                    }
                }
                lblSelectedCategory.InputAccessoryView = toolbar;

                PickerModel modelVat = new PickerModel(VatType);
                lblVatMode.Text = VatType[0];
                modelVat.PickerChanged += async (sender, e) =>
                {
                    lblVatMode.Text = e.SelectedValue;
                    Editchange = true;
                };

                UIToolbar toolbar2 = new UIToolbar();
                toolbar2.BarStyle = UIBarStyle.Default;
                toolbar2.Translucent = true;
                toolbar2.SizeToFit();
                //VatAction
                var doneButton2 = new UIBarButtonItem(Utils.TextBundle("done", "DONE"), UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("VatAction"));
                toolbar2.SetItems(new UIBarButtonItem[] { flexible, doneButton2 }, true);
                lblVatMode.InputView = new UIPickerView() { Model = modelVat, ShowSelectionIndicator = true };
                lblVatMode.InputAccessoryView = toolbar2;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("check category");
                return;
            }
            
        }
        [Export("VatAction")]
        private void DoneAction5()
        {
            lblVatMode.ResignFirstResponder();
        }
        [Export("CategoryAction")]
        private void DoneAction4()
        {
            lblSelectedCategory.ResignFirstResponder();
        }
        [Export("Stockmove:")]
        public void Stockmove(UIGestureRecognizer sender)
        {
            if(addItem!= null && addItem.FTrackStock==1)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("stockmove", ""));
                StockMovementController stockMovementController = new StockMovementController(addItem);
                this.NavigationController.PushViewController(stockMovementController, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notfoundstockmove", "ไม่พบรายการ Stock Movement"));
            }
        }

        [Export("imageview:")]
        public void Stockmoveimg(UIGestureRecognizer sender)
        {
            if (!string.IsNullOrEmpty(sysItemIDEdit?.PicturePath) && editedImage == null )
            {
                GabanaShowImage.SharedInstance.Show(this, sysItemIDEdit.PicturePath, "UserInactive.png");
            }
        }
    }
}