using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
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

namespace Gabana.iOS.ITEMS
{
    public partial class ItemsAddToppingController : UIViewController
    {
        private bool feditstock = false;
        ItemManage ItemManage;
        private decimal fstockbefore;
        int flagBtn = 0;
        int MenuSelected = 0;
        UIImage editedImage;
        bool checkManageStock;
        UIView BottomView, btnDeleteView;
        bool favFlag = false;
        UIButton btnAddCategory;
        ItemOnBranch itemOnBranch;
        UICollectionView menuAddItemBarCollectionView;
        Gabana.ORM.MerchantDB.Item addItem = new Gabana.ORM.MerchantDB.Item();
        MenuItemDataSource menuAddItemDataSource;
        List<MenuitemHeaderIOS>  Menu = new List<MenuitemHeaderIOS>();
        #region ItemViewComponent
        public static int flagDetail = 0;
        UIAlertController selectPhotoMenuSheet;
        UIImagePickerController imagePicker;
        UIView _contentView, DetailCategoryView, DetailCostView, lines3;
        public static long addColor;
        UIScrollView _scrollView;
        public static long CatID;
        UIView imageView, setColorView, setItemNameView, SetItemPriceView, itemCardFooter, DetailClickView;
        UIImageView itemCardView, itemCardViewTop;
        private static byte[] picture;
        public List<Category> CatList;
        UILabel lblItemCardName, lblItemCardShortName, LblItemCardPrice, lblItemName, lblItemPrice, lblDetail;
        UITextField txtItemName, txtItemPrice, txtExtraCategoryName, txtCost;
        String OldName = null;
        long? OldCategory = null;
        UIView BottomViewEdit;
        UIButton btnDelete, btnSaveCategory;
        decimal OldPrice = 0, OldCost = 0;
        UIButton btnToggleDetail;
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        UIImageView SetCategory;
        Gabana.ShareSource.Manage.ItemManage itemManager = new Gabana.ShareSource.Manage.ItemManage();
        UIView line1, line2, line3, line4, line5, line7, line45;
        UIButton btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
        UIImageView favImg;
        UILabel lblFav;
        List<Category> category = new List<Category>();

        #endregion
        #region StockViewComponent
        UIView stockView, lineEmpty;
        UIView StockbarView, disableView;
        UILabel lblStock, lblStockMinimum, lblStockNumber, lblExteaCategoryName, lblextraCost;
        UITextField txtStockMinimum;
        UISwitch switchStock;
        Item itemsData = null;
        UIView viewstockmove;
        UILabel lblStockmove, lbltxtOnHand;
        UIImageView imgSelectStockmove;
        public static bool isModifyOnhand = false;
        public static long onHand = 0;
        string usernamelogin = "";
        bool edit = false;
        long SysItemId;
        private ItemOnBranch itemOnBranchedit;
        private UIView Viewblock;
        public bool openstock;  
        #endregion
        public ItemsAddToppingController() {
        }
        public ItemsAddToppingController(Item item)
        {
            this.itemsData = item;
            addItem = item;
            OldName = item.ItemName;
            OldPrice = item.Price;
            OldCost = item.EstimateCost;
            OldCategory = item.SysCategoryID;
            SysItemId = item.SysItemID;
            edit = true;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (isModifyOnhand)
            {
                feditstock = true;
                lblStockNumber.Text = onHand.ToString("#,##0");
                isModifyOnhand = false;
            }
        }

        public async override void ViewDidLoad()
        {
            try
            {
                //addItem.Colors = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
                usernamelogin = Preferences.Get("User", "");
                this.NavigationController.SetNavigationBarHidden(false, false);
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                Setkeyboard();
                initAttribute();
                SetupAutoLayout();
                SetupPicker();
                setUpMenu();
                Textboxfocus(View);
                Checkper();
                if (this.itemsData != null && itemsData.ItemName != null)
                {
                    BottomView.Hidden = true;
                    BottomViewEdit.Hidden = false;

                    setData();
                }
                else
                {
                    itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    itemCardFooter.BackgroundColor = UIColor.Black;
                    itemCardFooter.Layer.Opacity = 0.2f;
                    BottomView.Hidden = false;
                    BottomViewEdit.Hidden = true;
                }

                if (edit)
                {
                    GabanaLoading.SharedInstance.Show(this);
                    if (addItem.FTrackStock == 1)
                    {
                        await GetStockData();
                    }

                }
                else
                {
                    addItem.Colors = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
                }
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
                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                
            }
            finally 
            {
                GabanaLoading.SharedInstance.Hide();
            }

        }
        async Task GetStockData()
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
                            MinimumStock = DataStock.MinimumStock
                        };

                        var insert = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranchedit);

                        lblStockNumber.Text = DataStock.BalanceStock.ToString("#,###");
                        txtStockMinimum.Text = DataStock.MinimumStock.ToString("#,###");

                        
                    }
                    else
                    {
                        disableView.Hidden = true;
                    }
                }
                else if (SysItemId != 0 & !await GabanaAPI.CheckNetWork())
                {
                    var getBalance = await onBranchManage.GetItemOnBranch((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (getBalance != null)
                    {

                        lblStockNumber.Text = getBalance.BalanceStock.ToString();
                        lblStockMinimum.Text = getBalance.MinimumStock.ToString();

                        if (addItem.FTrackStock == 0)
                        {
                            switchStock.On = false;
                            disableView.Hidden = true;
                            fstockbefore = 0;

                        }
                        else
                        {
                            fstockbefore = 1;
                            switchStock.On = true;
                            disableView.Hidden = false;

                        }
                    }
                    else
                    {
                        disableView.Hidden = true;
                    }
                    Utils.ShowMessage(Utils.TextBundle("nointernet", "onhand"));

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
        private void Checkper()
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
                //btnDeleteView.UserInteractionEnabled = false;
                //btnAdd.Enabled = false;

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
        void initAttribute()
        {
            #region MenuItem
            UICollectionViewFlowLayout MenuflowLayoutList = new UICollectionViewFlowLayout();
            MenuflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            MenuflowLayoutList.MinimumLineSpacing = 0;
            MenuflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width / 2), height: 40);

            menuAddItemBarCollectionView = new UICollectionView(frame: View.Frame, layout: MenuflowLayoutList);
            menuAddItemBarCollectionView.BackgroundColor = UIColor.White;
            menuAddItemBarCollectionView.ShowsHorizontalScrollIndicator = true;
            menuAddItemBarCollectionView.ScrollEnabled = false;
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

                    flagBtn = 0;
                    stockView.Hidden = true;
                    disableView.Hidden = true;
                    _scrollView.Hidden = false;
                }
                else if (x == 1)
                {
                    Menu[0].select = false;
                    Menu[1].select = true;

                    flagBtn = 1;
                    stockView.Hidden = false;
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

            #region ItemViewLayout
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
            itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            imageView.AddSubview(itemCardView);

            itemCardViewTop = new UIImageView();
            itemCardViewTop.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardViewTop.Layer.CornerRadius = 10;
            
            itemCardViewTop.ClipsToBounds = true;
            itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            itemCardView.AddSubview(itemCardViewTop);

            itemCardFooter = new UIView();
            itemCardFooter.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardFooter.BackgroundColor = UIColor.FromRGB(162, 162, 162);
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

            lblItemCardShortName = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCardShortName.Font = lblItemCardShortName.Font.WithSize(24);
            lblItemCardShortName.Text = Utils.TextBundle("item", "Item");
            itemCardView.AddSubview(lblItemCardShortName);

            LblItemCardPrice = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            LblItemCardPrice.Font = LblItemCardPrice.Font.WithSize(13);
            LblItemCardPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0.00";
            itemCardView.AddSubview(LblItemCardPrice);

            imageView.AddSubview(itemCardView);

            itemCardView.BringSubviewToFront(lblItemCardName);
            itemCardView.BringSubviewToFront(lblItemCardShortName);
            itemCardView.BringSubviewToFront(LblItemCardPrice);
            #endregion
            #region SetColor
            setColorView = new UIView();
            setColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            setColorView.BackgroundColor = UIColor.White;

            setColorButton();
            #endregion

            line1 = new UIView();
            line1.TranslatesAutoresizingMaskIntoConstraints = false;
            line1.BackgroundColor = UIColor.FromRGB(248, 248, 248);

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
            lblItemName.Text = Utils.TextBundle("extraname", "Extra Topping Name");
            setItemNameView.AddSubview(lblItemName);

            txtItemName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtItemName.ReturnKeyType = UIReturnKeyType.Next;
            txtItemName.ShouldReturn = (tf) =>
            {
                txtItemPrice.BecomeFirstResponder();
                return true;
            };
            txtItemName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("extraname", "extraname"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtItemName.Font = txtItemName.Font.WithSize(15);
            txtItemName.EditingChanged += (object sender, EventArgs e) =>
            {

                lblItemCardName.Text = txtItemName.Text;
                if (txtItemName.Text.Length > 7)
                {
                    lblItemCardShortName.Text = txtItemName.Text.Substring(0, 7);
                }
                else
                {
                    lblItemCardShortName.Text = txtItemName.Text;
                }
                if (txtItemName.Text.Length == 0)
                {
                    lblItemCardShortName.Text = "Item Na";
                }
            };
            setItemNameView.AddSubview(txtItemName);
            #endregion

            line2 = new UIView();
            line2.TranslatesAutoresizingMaskIntoConstraints = false;
            line2.BackgroundColor = UIColor.FromRGB(248, 248, 248);

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

            txtItemPrice = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            txtItemPrice.InputAccessoryView = NumpadToolbarPrice;
            txtItemPrice.AttributedPlaceholder = new NSAttributedString(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtItemPrice.KeyboardType = UIKeyboardType.DecimalPad;
            txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + 0;
            txtItemPrice.Font = txtItemPrice.Font.WithSize(15);
            txtItemPrice.ShouldChangeCharacters = (textField, range, replacementString) => {
                if (textField.Text.Contains(".") && replacementString == ".")
                {
                    return false;
                }
                return true;
            };
            txtItemPrice.EditingChanged += (object sender, EventArgs e) =>
            {
                try
                {

                    if (!txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                    {
                        LblItemCardPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text;
                    }
                    else
                    {
                        LblItemCardPrice.Text = txtItemPrice.Text;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            };
            txtItemPrice.EditingDidBegin += TxtItemPrice_EditingDidBegin;
            txtItemPrice.EditingDidEnd += TxtItemPrice_EditingDidEnd;
            SetItemPriceView.AddSubview(txtItemPrice);
            #endregion

            line3 = new UIView();
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            line3.BackgroundColor = UIColor.FromRGB(248, 248, 248);

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

            btnToggleDetail = new UIButton();
            btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
            btnToggleDetail.TranslatesAutoresizingMaskIntoConstraints = false;
            btnToggleDetail.TouchUpInside += (sender, e) =>
            {
                //Detail Not Show
                if (flagDetail == 0)
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                    setDetailShow(false);
                    flagDetail = 1;
                }
                else
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                    setDetailShow(true);
                    flagDetail = 0;
                }
            };
            DetailClickView.AddSubview(btnToggleDetail);
            #endregion

            line4 = new UIView();
            line4.TranslatesAutoresizingMaskIntoConstraints = false;
            line4.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region DetailCostView
            DetailCostView = new UIView();
            DetailCostView.Hidden = true;
            DetailCostView.TranslatesAutoresizingMaskIntoConstraints = false;
            DetailCostView.BackgroundColor = UIColor.White;


            lblextraCost = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblextraCost.Font = lblextraCost.Font.WithSize(15);
            lblextraCost.Text = Utils.TextBundle("cost", "Cost");
            DetailCostView.AddSubview(lblextraCost);

            UIToolbar NumpadToolbar2 = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)View.Frame.Width, 44.0f));
            NumpadToolbar2.Translucent = true;
            NumpadToolbar2.Items = new UIBarButtonItem[]{
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    if(!txtCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                    {
                         txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtCost.Text;
                    }
                    View.EndEditing(true);
                })
                };
            NumpadToolbar2.SizeToFit();

            txtCost = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtCost.ReturnKeyType = UIReturnKeyType.Next;
            txtCost.InputAccessoryView = NumpadToolbar2;
            txtCost.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtCost.ShouldChangeCharacters = (textField, range, replacementString) => {
                if (textField.Text.Contains(".") && replacementString == ".")
                {
                    return false;
                }
                return true;
            };
            txtCost.AttributedPlaceholder = new NSAttributedString(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 221, 245) });
            txtCost.Font = txtCost.Font.WithSize(15);
            txtCost.KeyboardType = UIKeyboardType.DecimalPad;
            txtCost.EditingDidEnd += TxtItemCost_EditingDidEnd1;
            DetailCostView.AddSubview(txtCost);
            #endregion

            line5 = new UIView();
            line5.Hidden = true;
            line5.TranslatesAutoresizingMaskIntoConstraints = false;
            line5.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region DetailCategory
            DetailCategoryView = new UIView();
            DetailCategoryView.Hidden = true;
            DetailCategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            DetailCategoryView.BackgroundColor = UIColor.White;


            lblExteaCategoryName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblExteaCategoryName.Font = lblExteaCategoryName.Font.WithSize(15);
            lblExteaCategoryName.Text = Utils.TextBundle("category", "Category");
            DetailCategoryView.AddSubview(lblExteaCategoryName);

            txtExtraCategoryName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtExtraCategoryName.ReturnKeyType = UIReturnKeyType.Done;
            txtExtraCategoryName.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtExtraCategoryName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("none", "None"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtExtraCategoryName.Font = txtExtraCategoryName.Font.WithSize(15);
            DetailCategoryView.AddSubview(txtExtraCategoryName);

            SetCategory = new UIImageView();
            SetCategory.Image = UIImage.FromBundle("Next");
            SetCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            DetailCategoryView.AddSubview(SetCategory);

            SetCategory.UserInteractionEnabled = true;
            var ItemSet_Cat = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ItemSet_Cat:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            SetCategory.AddGestureRecognizer(ItemSet_Cat);
            #endregion

            line7 = new UIView();
            line7.Hidden = true;
            line7.TranslatesAutoresizingMaskIntoConstraints = false;
            line7.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView.AddSubview(imageView);
            _contentView.AddSubview(setColorView);
            _contentView.AddSubview(line1);
            _contentView.AddSubview(setItemNameView);
            _contentView.AddSubview(line2);
            _contentView.AddSubview(SetItemPriceView);
            _contentView.AddSubview(line3);
            _contentView.AddSubview(DetailClickView);
            _contentView.AddSubview(line4);
            _contentView.AddSubview(DetailCostView);
            _contentView.AddSubview(line5);
            _contentView.AddSubview(DetailCategoryView);
            _contentView.AddSubview(line7);

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
            stockView.AddSubview(disableView);

            lblStock = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStock.Font = lblStock.Font.WithSize(15);
            lblStock.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStock.Text = Utils.TextBundle("stock", "Stock");
            StockbarView.AddSubview(lblStock);

            switchStock = new UISwitch();
            switchStock.TranslatesAutoresizingMaskIntoConstraints = false;
            switchStock.OnTintColor = UIColor.FromRGB(0, 149, 218);
            switchStock.SetState(switchStock.On, true);
            switchStock.ValueChanged += (sender, e) =>
            {
                if (switchStock.On)
                {
                    txtStockMinimum.Enabled = true;
                    disableView.Hidden = true;
                }
                else if (!switchStock.On)
                {
                    txtStockMinimum.Enabled = false;
                    disableView.Hidden = false;
                }
            };
            StockbarView.AddSubview(switchStock);

            lblStockNumber = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockNumber.Font = lblStock.Font.WithSize(45);
            lblStockNumber.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockNumber.Text = "0";
            stockView.AddSubview(lblStockNumber);

            lblStockNumber.UserInteractionEnabled = true;
            var tapGestureOnHand = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("OnHand:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            lblStockNumber.AddGestureRecognizer(tapGestureOnHand);


            lblStockMinimum = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockMinimum.Font = lblStock.Font.WithSize(15);

            lblStockMinimum.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockMinimum.Text = Utils.TextBundle("minstock", "Minimum Stock");
            stockView.AddSubview(lblStockMinimum);

            lines3 = new UIView();
            lines3.TranslatesAutoresizingMaskIntoConstraints = false;
            lines3.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(lines3);

            lbltxtOnHand = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtOnHand.Font = lbltxtOnHand.Font.WithSize(15);
            lbltxtOnHand.TranslatesAutoresizingMaskIntoConstraints = false;
            lbltxtOnHand.Text = Utils.TextBundle("onhand", "onhand");
            StockbarView.AddSubview(lbltxtOnHand);

            txtStockMinimum = new UITextField
            {
                Placeholder = "0",
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtStockMinimum.Enabled = false;
            txtStockMinimum.Font = txtStockMinimum.Font.WithSize(15);
            txtStockMinimum.InputAccessoryView = NumpadToolbar;
            txtStockMinimum.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtStockMinimum.KeyboardType = UIKeyboardType.NumberPad;
            stockView.AddSubview(txtStockMinimum);

            lineEmpty = new UIView();
            lineEmpty.TranslatesAutoresizingMaskIntoConstraints = false;
            lineEmpty.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(lineEmpty);

            line45 = new UIView();
            line45.TranslatesAutoresizingMaskIntoConstraints = false;
            line45.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            stockView.AddSubview(line45);

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

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomView);

            btnAddCategory = new UIButton();
            btnAddCategory.SetTitle(Utils.TextBundle("additem", "Add Item"), UIControlState.Normal);
            btnAddCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddCategory.Layer.CornerRadius = 5f;
            btnAddCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddCategory.TouchUpInside += async (sender, e) => {
                btnAddCategory.Enabled = false;
                if (!string.IsNullOrEmpty(txtItemName.Text) && !string.IsNullOrEmpty(txtItemPrice.Text))
                {
                    createTopping();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("enterall", "กรุณากรอกข้อมูลให้ครบถ้วน"));
                    btnAddCategory.Enabled = true;
                }
            };
            BottomView.AddSubview(btnAddCategory);

            #endregion

            #region BottomViewEdit
            BottomViewEdit = new UIView();
            BottomViewEdit.Hidden = true;
            BottomViewEdit.BackgroundColor = UIColor.Clear;
            BottomViewEdit.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomViewEdit);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "wanttodelete"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => Delete()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                PresentViewController(okCancelAlertController, true, null);
            };
            BottomViewEdit.AddSubview(btnDelete);

            btnSaveCategory = new UIButton();
            btnSaveCategory.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSaveCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSaveCategory.Layer.CornerRadius = 5f;
            btnSaveCategory.BackgroundColor = UIColor.FromRGB(51, 172, 225);
            btnSaveCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSaveCategory.TouchUpInside += (sender, e) => {
                btnSaveCategory.Enabled = false;
                UpdateTopping();
            };
            BottomViewEdit.AddSubview(btnSaveCategory);
            #endregion
        }
        private void TxtItemCost_EditingDidEnd1(object sender, EventArgs e)
        {
            if (!txtCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtCost.Text;
            }
            else
            {
                if (!string.IsNullOrEmpty(txtCost.Text.Remove(0, 1).Trim()))
                {
                    txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(Convert.ToDecimal(txtCost.Text.Remove(0, 1).Trim()));
                }
                else
                {
                    txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
                }

            }


        }
        private void TxtItemPrice_EditingDidBegin(object sender, EventArgs e)
        {
            if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                if (Convert.ToDecimal(txtItemPrice.Text.Remove(0, 1).Trim()) == 0)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " ";
                }
            }

            txtItemPrice.BecomeFirstResponder();

        }

        private void TxtItemPrice_EditingDidEnd(object sender, EventArgs e)
        {

            if (!txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
            {
                //txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text;
                if (!string.IsNullOrEmpty(txtItemPrice.Text.Trim()))
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + txtItemPrice.Text.Trim();
                }
                else
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtItemPrice.Text.Remove(0, 1).Trim()))
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(Convert.ToDecimal(txtItemPrice.Text.Remove(0, 1).Trim()));
                }
                else
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(0);
                }

            }
            LblItemCardPrice.Text = txtItemPrice.Text;
        }
        void setUpMenu()
        {
            Menu = new List<MenuitemHeaderIOS>();
            Menu.Add(new MenuitemHeaderIOS(0, Utils.TextBundle("extratopping", "Extra Topping"), true));
            Menu.Add(new MenuitemHeaderIOS(1, Utils.TextBundle("stock", "stock"), false));

            menuAddItemDataSource = new MenuItemDataSource(Menu);
            menuAddItemBarCollectionView.DataSource = menuAddItemDataSource;
            menuAddItemBarCollectionView.ReloadData();
        }
        async void setData()
        {
            try
            {
                txtItemName.Text = itemsData.ItemName;
                LblItemCardPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(itemsData.Price);
                lblItemCardName.Text = itemsData.ItemName;
                lblItemCardShortName.Text = itemsData.ShortName;
                if (itemsData.Price != 0 && itemsData.Price != null)
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(itemsData.Price);
                }
                else
                {
                    txtItemPrice.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0";
                }
                if (itemsData.EstimateCost != 0 && itemsData.EstimateCost != null)
                {
                    txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(itemsData.EstimateCost);
                }
                else
                {
                    txtCost.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 0";
                }


                itemCardViewTop.Image = null;
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                if ((itemsData.EstimateCost != null && itemsData.EstimateCost != 0) || itemsData.SysCategoryID != null)
                {
                    flagDetail = 1;
                    setDetailShow(false);
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                }
                if (!string.IsNullOrEmpty(itemsData.ThumbnailLocalPath))
                {
                    itemCardViewTop.Hidden = false;
                    //Utils.SetImageURL(itemCardViewTop, sysItemIDEdit.PictureLocalPath);
                    lblItemCardShortName.Hidden = true;
                    itemCardView.BackgroundColor = UIColor.FromRGB(162, 162, 162);
                    itemCardViewTop.BackgroundColor = UIColor.White;
                    itemCardFooter.BackgroundColor = UIColor.FromRGB(162, 162, 162);
                    itemCardFooter.Layer.Opacity = 1f;
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Utils.SetImageURL(itemCardViewTop, Path.Combine(docFolder, itemsData.ThumbnailLocalPath));
                }
                else
                {
                    itemCardViewTop.Hidden = true;
                    itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    itemCardFooter.BackgroundColor = UIColor.Black;
                    itemCardFooter.Layer.Opacity = 0.2f;
                    if (itemsData.Colors != null)
                    {
                        Utils.SetColor(itemCardView, Convert.ToInt64(itemsData.Colors));

                    }
                    else
                    {
                        Utils.SetColor(itemCardView, 0);
                    }
                }

                if (itemsData.FavoriteNo == 1)
                {
                    favFlag = true;
                    favImg.Image = UIImage.FromBundle("Fav"); // Fav
                }
                else
                {
                    favFlag = false;
                    favImg.Image = UIImage.FromBundle("Unfav"); // Fav
                }

            }
            catch (Exception ex )
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("checkItewmCode at add Item");
                return;
            }
            

        }
        async void UpdateTopping()
        {
            try
            {
                if (addColor == addItem.Colors)
                {
                    addItem.Colors = itemsData.Colors;
                }
                else
                {
                    addItem.Colors = addColor;
                }
                addItem.MerchantID = itemsData.MerchantID;
                addItem.SysItemID = itemsData.SysItemID;
                if(txtItemName.Text != itemsData.ItemName)
                {
                    addItem.ItemName = txtItemName.Text;
                }
                else
                {
                    addItem.ItemName = itemsData.ItemName;
                }
                addItem.Ordinary = itemsData.Ordinary;

                long? category = null;
                if (txtExtraCategoryName.Text == Utils.TextBundle("none", "None"))
                {
                    category = null;
                }
                else
                {
                    category = CatID;
                }
                addItem.SysCategoryID = category;
                //if (CatID == itemsData.SysCategoryID)
                //{
                //    addItem.SysCategoryID = itemsData.SysCategoryID;
                //}
                //else if (CatID == 0)
                //{
                //    addItem.SysCategoryID = null;
                //}
                //else
                //{
                //    addItem.SysCategoryID = CatID;
                //}
                addItem.ShortName = lblItemCardShortName.Text;
                addItem.ItemCode = itemsData.ItemCode;
                

                addItem.UnitName = itemsData.UnitName;
                addItem.RegularSizeName = itemsData.RegularSizeName;

                if (txtCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                    txtCost.Text = txtCost.Text.Remove(0, 2);
                }
                if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                    txtItemPrice.Text = txtItemPrice.Text.Remove(0, 2);
                }
                if (string.IsNullOrEmpty(txtItemPrice.Text))
                {
                    addItem.Price = 0;
                }
                else
                {
                    addItem.Price = Convert.ToDecimal(txtItemPrice.Text);
                }
                if (string.IsNullOrEmpty(txtCost.Text))
                {
                    addItem.EstimateCost = Convert.ToDecimal(txtItemPrice.Text.Trim());
                }
                else
                {
                    addItem.EstimateCost = Convert.ToDecimal(txtCost.Text.Trim());
                }

                if (favFlag)
                {
                    addItem.FavoriteNo = 1;
                }
                else
                {
                    addItem.FavoriteNo = 0;
                }
                
                addItem.OptSalePrice = itemsData.OptSalePrice;
                addItem.TaxType = itemsData.TaxType;
                addItem.SellBy = itemsData.SellBy;
                addItem.FDisplayOption = itemsData.FDisplayOption;
                
                addItem.TrackStockDateTime = Utils.GetTranDate(itemsData.TrackStockDateTime); 
                addItem.SaleItemType = itemsData.SaleItemType;
                addItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                addItem.UserLastModified = usernamelogin;
                addItem.DataStatus = 'M';
                addItem.Comments = itemsData.Comments;
                addItem.FWaitSending = 1;
                addItem.LinkProMaxxItemID = null;
                addItem.LinkProMaxxItemUnit = null;
                addItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                addItem.FTrackStock = itemsData.FTrackStock;
                if (txtItemName.Text.Trim() != itemsData.ItemName && txtItemName.Text.Trim() != string.Empty)
                {
                    var checkName = await itemManager.CheckNameItem(addItem.ItemName);
                    if (checkName)
                    {
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                            InsertRepeatItem insertRepeat = new InsertRepeatItem();
                            insertRepeat.checkManageStock = checkManageStock;
                            insertRepeat.DetailITem = addItem;
                            insertRepeat.Stock = lblStockNumber.Text;
                            insertRepeat.minimumstock = txtStockMinimum.Text;
                            var json = JsonConvert.SerializeObject(insertRepeat);

                            //ชื่อซ้ำ
                            var okCancelAlertController = UIAlertController.Create("", txtItemName.Text + Utils.TextBundle("alertitem3", "alertitem3"), UIAlertControllerStyle.Alert);
                            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                                alert => CheckEdit()));
                            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                            PresentViewController(okCancelAlertController, true, null);
                            btnSaveCategory.Enabled = true;
                            return;
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("InsertItem at add Item");
                            return;
                        }
                    }
                }
                CheckEdit();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotedit", "cannotedit"));
            }
        }
        async void createTopping()
        {
            try
            {
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                addItem.MerchantID = DataCashingAll.MerchantId;
                addItem.SysItemID = Convert.ToInt64(sys);
                addItem.Ordinary = 0;
                addItem.ItemName = txtItemName.Text;
                if (favFlag)
                {
                    addItem.FavoriteNo = 1;
                }
                else
                {
                    addItem.FavoriteNo = 0;
                }
                if (txtCost.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                    txtCost.Text = txtCost.Text.Remove(0, 2);
                }
                if (txtItemPrice.Text.Contains(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS))
                {
                    txtItemPrice.Text = txtItemPrice.Text.Remove(0, 2);
                }

                if (string.IsNullOrEmpty(txtCost.Text))
                {
                    addItem.EstimateCost = Convert.ToDecimal(txtItemPrice.Text);
                }
                else
                {
                    addItem.EstimateCost = Convert.ToDecimal(txtCost.Text);
                }

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
                    addItem.PicturePath = filePath;
                    addItem.ThumbnailPath = filePaththumbnail;
                }
                else
                {

                    addItem.ThumbnailLocalPath = null;
                    addItem.PictureLocalPath = null;
                    addItem.PicturePath = null;
                    addItem.ThumbnailPath = null;
                }


                addItem.ItemCode = null;
                addItem.Price = Convert.ToDecimal(txtItemPrice.Text);
                addItem.OptSalePrice = 'F';
                addItem.TaxType = 'N';
                addItem.UnitName = null;
                addItem.ThumbnailPath = null;
                addItem.RegularSizeName = null;
                
                
                addItem.SellBy = 'T';
                addItem.FDisplayOption = 0;
                addItem.FTrackStock = 0;
                addItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.SaleItemType = 'T';
                addItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                addItem.UserLastModified = usernamelogin;
                addItem.DataStatus = 'I';
                addItem.Comments = null;
                addItem.FWaitSending = 1;
                addItem.LinkProMaxxItemID = null;
                addItem.LinkProMaxxItemUnit = null;
                addItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addItem.ShortName = lblItemCardShortName.Text;
                long? category = null;
                if (txtExtraCategoryName.Text == Utils.TextBundle("none", "None"))
                {
                    category = null;
                }
                else
                {
                    category = CatID;
                }
                addItem.SysCategoryID = category;
                //if (CatID == 0)
                //{
                //    addItem.SysCategoryID = null;
                //}
                //else
                //{
                //    addItem.SysCategoryID = CatID;
                //}
                addItem.Colors = addColor;

                if (switchStock.On)
                {
                    if (addItem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(lblStockNumber.Text) | string.IsNullOrEmpty(txtStockMinimum.Text))
                        {
                            Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                            btnAddCategory.Enabled = true;
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
                        btnAddCategory.Enabled = true;
                        return;
                    }
                }

                

                var checkName = await itemManager.CheckNameItem(addItem.ItemName);
                if (checkName)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                        InsertRepeatItem insertRepeat = new InsertRepeatItem();
                        insertRepeat.checkManageStock = checkManageStock;
                        insertRepeat.DetailITem = addItem;
                        insertRepeat.Stock = lblStockNumber.Text;
                        insertRepeat.minimumstock = txtStockMinimum.Text;
                        var json = JsonConvert.SerializeObject(insertRepeat);

                        //ชื่อซ้ำ
                        var okCancelAlertController = UIAlertController.Create("", txtItemName.Text + Utils.TextBundle("alertitem3", "alertitem3"), UIAlertControllerStyle.Alert);
                        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                            alert => CheckEdit()));
                        okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel,   alert => Console.WriteLine("Cancel was clicked")));
                        PresentViewController(okCancelAlertController, true, null);
                        btnAddCategory.Enabled = true;
                        return;
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add Item");
                        btnAddCategory.Enabled = true;
                        return;
                    }
                }

                CheckEdit();
                

                //var result = await itemManager.InsertItem(addItem, itemOnBranch);
                //if (!result)
                //{
                //    //Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                //    return;
                //}

                ////Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                //if (await GabanaAPI.CheckNetWork())
                //{
                //    JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
                //}
                //else
                //{
                //    addItem.FWaitSending = 2;
                //    await itemManager.UpdateItem(addItem);
                //}
                //checkManageStock = false;
                //DataCaching.NewItem = addItem.SysItemID;
                //this.NavigationController.PopViewController(false);

            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"),Utils.TextBundle("cannotsave", "cannotsave"));
            }
        }
        void CheckEdit()
        {
            if (itemsData != null) // update
            {
                addItem.DataStatus = 'M';
                UpdateItemToDB();
            }
            else // insert
            {
                addItemToDB();
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
                    addItem.PicturePath = filePath;
                    addItem.ThumbnailPath = filePaththumbnail;
                }
                else
                {

                    addItem.ThumbnailLocalPath = null;
                    addItem.PictureLocalPath = null;
                    addItem.PicturePath = null;
                    addItem.ThumbnailPath = null;
                }
                var check = await itemManager.InsertItem(addItem, itemOnBranch,null);

                if (check)
                {
                    //done insert
                   

                    // senttocloud 
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
                    ItemsController.Ismodify = true;
                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "savedatasuccessfully"));
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotsave", "cannotsave"));
                    btnAddCategory.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                btnAddCategory.Enabled = true;
                _ = TinyInsights.TrackPageViewAsync("InsertToppping at add Extra");
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
                    addItem.PicturePath = filePath;
                    addItem.ThumbnailPath = filePaththumbnail;
                }
                bool resultstock = false;
                if (feditstock)
                {
                    if (switchStock.On && fstockbefore == 0)
                    {
                        if (string.IsNullOrEmpty(lblStockNumber.Text) | string.IsNullOrEmpty(txtStockMinimum.Text))
                        {
                            Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                            btnSaveCategory.Enabled = true;
                            return;
                        }

                        //int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock
                        resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)addItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)), ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)));
                        if (!resultstock)
                        {
                            Utils.ShowMessage(Utils.TextBundle("cannotedit", "onhand"));
                            btnSaveCategory.Enabled = true;
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
                            btnSaveCategory.Enabled = true;
                            return;
                        }
                        addItem.FTrackStock = 0;
                    }
                    else
                    {
                        var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(DataCashingAll.SysBranchId, (int)addItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(lblStockNumber.Text)), ConvertToDecimal(Utils.CheckLenghtValue(txtStockMinimum.Text)));
                        if (!PostDataTrackStockAdjust.Status)
                        {
                            Utils.ShowMessage(Utils.TextBundle("cannotedit", "cannotedit"));
                            btnSaveCategory.Enabled = true;
                            return;
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
                            ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                            var updateStock = await onBranchManage.UpdateItemOnBranch(itemOnBranch);

                        }
                    }
                    addItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                }

                addItem.DataStatus = 'M';
                var check = await itemManager.UpdateItem(addItem);
                if (check)
                {
                    //done insert
                    ItemsController.Ismodify = true;
                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "editdatasuccessfully"));
                    // senttocloud 
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
                    }
                    else
                    {
                        addItem.FWaitSending = 2;
                        await itemManager.UpdateItem(addItem);
                    }
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "onhand"));
                    btnSaveCategory.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                btnSaveCategory.Enabled = true;
                _ = TinyInsights.TrackPageViewAsync("InsertToppping at add Extra");
            }
           
        }
        async Task<bool> UpdateOpenStock(int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock)
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

        async Task<bool> UpdateClosetock(int sysitem)
        {
            //Post/Close เป็นการปิดระบบ Track Stock
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
        private async void SetupPicker()
        {
            // Setup the picker and model
            try
            {
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton1 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { View.EndEditing(true); });

                CategoryManage categoryManage = new CategoryManage();
                Category addcategory = new Category();
                
                var getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = Utils.TextBundle("none", "None"),
                    SysCategoryID = 0
                };
                category.Add(addcategory);
                getallCategory = await categoryManage.GetAllCategory();
                if (getallCategory != null)
                {
                    category.AddRange(getallCategory);
                }

                #region Category Picker
                PickerModelCategory model1 = new PickerModelCategory(category);
                model1.PickerChanged += (sender, e) => {
                    txtExtraCategoryName.Text = e.CategoryName;
                    CatID = e.ID;
                };
                UIToolbar toolbar1 = new UIToolbar();
                toolbar1.Translucent = true;
                toolbar1.SizeToFit();


                toolbar1.SetItems(new UIBarButtonItem[] { flexible, doneButton1 }, true);
                var xy = new UIPickerView() { Model = model1, ShowSelectionIndicator = true };
                txtExtraCategoryName.InputView = xy;
                if (category != null)
                {
                    if (itemsData ==null || itemsData.SysCategoryID == null)
                    {
                        txtExtraCategoryName.Text = category[0].Name;
                        xy.Select(0, 0, false);
                    }
                    else
                    {
                        txtExtraCategoryName.Text = category.Where(x => x.SysCategoryID == itemsData.SysCategoryID).FirstOrDefault().Name;
                        CatID = (int)category.Where(x => x.SysCategoryID == itemsData.SysCategoryID).FirstOrDefault().SysCategoryID;
                        var C = (int)category.FindIndex(x => x.SysCategoryID == itemsData.SysCategoryID);
                        xy.Select((int)C, 0, false);
                    }
                }
                txtExtraCategoryName.InputAccessoryView = toolbar1;
                #endregion
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("check extra topping category");
                return;
            }
           
            
        }
        [Export("ItemSet_Cat:")]
        public void ItemSet_Cat(UIGestureRecognizer sender)
        {
            txtExtraCategoryName.BecomeFirstResponder();
        }
        public async void Delete()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                var itemDelete = itemsData;
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 1;
                itemDelete.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                var result = await itemManage.UpdateItem(itemDelete);
                if (result)
                {
                    this.NavigationController.PopViewController(false);
                    ItemsController.Ismodify = true;
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "onhand"));
                    if (await GabanaAPI.CheckNetWork())
                        {
                            JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, (int)itemDelete.SysItemID);
                        }
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "onhand"));
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
            line5.Hidden = x;
            line7.Hidden = x;
            DetailCostView.Hidden = x;
            DetailCategoryView.Hidden = x;
        }
        void setColorButton()
        {
            //btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
            #region Buttom Blue
            btnColor1 = new UIButton();
            btnColor1.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnColor1.Layer.CornerRadius = 3;
            btnColor1.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor1.TouchUpInside += (sender, e) =>
            {
                //Blue
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                itemCardViewTop.Image = null;
                addColor = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor1);

            #endregion
            #region Buttom Yellow
            btnColor2 = new UIButton();
            btnColor2.BackgroundColor = UIColor.FromRGB(248, 151, 29);
            btnColor2.Layer.CornerRadius = 3;
            btnColor2.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor2.TouchUpInside += (sender, e) =>
            {
                //Yellow
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(248, 151, 29);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(248, 151, 29);
                addColor = int.Parse("F8971D", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor2);

            #endregion
            #region Buttom Red
            btnColor3 = new UIButton();
            btnColor3.BackgroundColor = UIColor.FromRGB(227, 45, 73);
            btnColor3.Layer.CornerRadius = 3;
            btnColor3.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor3.TouchUpInside += (sender, e) =>
            {
                //Red
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(227, 45, 73);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(227, 45, 73);
                addColor = int.Parse("E32D49", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor3);
            #endregion
            #region Buttom Green
            btnColor4 = new UIButton();
            btnColor4.BackgroundColor = UIColor.FromRGB(55, 172, 82);
            btnColor4.Layer.CornerRadius = 3;
            btnColor4.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor4.TouchUpInside += (sender, e) =>
            {
                //Green
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(55, 172, 82);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(55, 172, 82);
                addColor = int.Parse("37AA52", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor4);
            #endregion
            #region Buttom Orange
            btnColor5 = new UIButton();
            btnColor5.BackgroundColor = UIColor.FromRGB(247, 86, 0);
            btnColor5.Layer.CornerRadius = 3;
            btnColor5.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor5.TouchUpInside += (sender, e) =>
            {
                //Orange
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(247, 86, 0);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(247, 86, 0);
                addColor = int.Parse("F75600", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor5);
            #endregion

            #region Buttom Purple
            btnColor6 = new UIButton();
            btnColor6.BackgroundColor = UIColor.FromRGB(63, 81, 181);
            btnColor6.Layer.CornerRadius = 3;
            btnColor6.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor6.TouchUpInside += (sender, e) =>
            {
                //Purple
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(63, 81, 181);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(63, 81, 181);
                addColor = int.Parse("3F51B5", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor6);
            #endregion
            #region Buttom DarkGreen
            btnColor7 = new UIButton();
            btnColor7.BackgroundColor = UIColor.FromRGB(0, 121, 107);
            btnColor7.Layer.CornerRadius = 3;
            btnColor7.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor7.TouchUpInside += (sender, e) =>
            {
                //Dark Green
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                lblItemCardShortName.Hidden = false;
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 121, 107);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(0, 121, 107);
                addColor = int.Parse("00796B", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor7);
            #endregion
            #region Buttom LightGreen
            btnColor8 = new UIButton();
            btnColor8.BackgroundColor = UIColor.FromRGB(139, 195, 74);
            btnColor8.Layer.CornerRadius = 3;
            btnColor8.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor8.TouchUpInside += (sender, e) =>
            {
                //LightGreen
                lblItemCardShortName.Hidden = false;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardViewTop.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(139, 195, 74);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(139, 195, 74);
                addColor = int.Parse("8BC34A", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor8);
            #endregion
            #region Buttom Pink
            btnColor9 = new UIButton();
            btnColor9.BackgroundColor = UIColor.FromRGB(221, 82, 126);
            btnColor9.Layer.CornerRadius = 3;
            btnColor9.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor9.TouchUpInside += (sender, e) =>
            {
                //PINK
                lblItemCardShortName.Hidden = false;
                itemCardViewTop.Image = null;
                itemCardFooter.BackgroundColor = UIColor.Black;
                itemCardFooter.Layer.Opacity = 0.2f;
                itemCardView.BackgroundColor = UIColor.FromRGB(221, 82, 126);
                itemCardViewTop.BackgroundColor = UIColor.FromRGB(221, 82, 126);
                addColor = int.Parse("DD527E", System.Globalization.NumberStyles.HexNumber);
            };
            setColorView.AddSubview(btnColor9);
           
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

                selectPhotoMenuSheet = UIAlertController.Create(Utils.TextBundle("addlogo", "addlogo"), null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("takepic", "takepic"), UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("choosepic", "choosepic"), UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.NavigationController.PresentModalViewController(selectPhotoMenuSheet, true);
                #endregion
            };
            setColorView.AddSubview(btnChangeItemPhoto);

            #endregion

            favImg = new UIImageView();
            favImg.Image = UIImage.FromBundle("Unfav");
            favImg.TranslatesAutoresizingMaskIntoConstraints = false;
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
            lblFav.TextColor = UIColor.FromRGB(64, 64, 64);
            lblFav.Lines = 1;
            lblFav.Text = Utils.TextBundle("favorite", "favorite");
            lblFav.TextAlignment = UITextAlignment.Left;
            lblFav.TranslatesAutoresizingMaskIntoConstraints = false;
            setColorView.AddSubview(lblFav);
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
        public class PickerModelCategory : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public string CategoryName { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;


            private readonly List<Category> values;
            public PickerModelCategory(List<Category> listCategory)
            {
                this.values = listCategory;
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
                        CategoryName = values[Convert.ToInt32(row)].Name,
                        ID = (int)values[Convert.ToInt32(row)].SysCategoryID
                    });
                }
            }
            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        private void Pic(string v)
        {
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                if (Utils.Checkpermisstion())
                {
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.AllowsEditing = true;
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
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
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
        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
        void setToggleMenu(bool x)
        {
            _scrollView.Hidden = x;
            //------------------------------- press stock -------------------------
            StockbarView.Hidden = !x;
            lblStock.Hidden = !x;
            switchStock.Hidden = !x;
            lblStockNumber.Hidden = !x;
            line3.Hidden = !x;
            lblStockMinimum.Hidden = !x;
            txtStockMinimum.Hidden = !x;
        }
        void SetupAutoLayout()
        {
            menuAddItemBarCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            menuAddItemBarCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            menuAddItemBarCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            menuAddItemBarCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region BottomViewLayout
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddCategory.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddCategory.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddCategory.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddCategory.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnSaveCategory.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSaveCategory.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSaveCategory.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnSaveCategory.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            #endregion

            #region ItemViewAttribute
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(menuAddItemBarCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor).Active = true;

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

            itemCardViewTop.TopAnchor.ConstraintEqualTo(itemCardViewTop.Superview.TopAnchor, 0).Active = true;
            itemCardViewTop.RightAnchor.ConstraintEqualTo(itemCardViewTop.Superview.RightAnchor, 0).Active = true;
            itemCardViewTop.LeftAnchor.ConstraintEqualTo(itemCardViewTop.Superview.LeftAnchor, 0).Active = true;
            itemCardViewTop.BottomAnchor.ConstraintEqualTo(itemCardFooter.TopAnchor).Active = true;

            itemCardView.TopAnchor.ConstraintEqualTo(itemCardView.Superview.TopAnchor, 15).Active = true;
            itemCardView.BottomAnchor.ConstraintEqualTo(itemCardView.Superview.BottomAnchor, -15).Active = true;
            itemCardView.LeftAnchor.ConstraintEqualTo(itemCardView.Superview.LeftAnchor, 15).Active = true;
            itemCardView.RightAnchor.ConstraintEqualTo(itemCardView.Superview.RightAnchor, -15).Active = true;

            lblItemCardName.TopAnchor.ConstraintEqualTo(itemCardFooter.SafeAreaLayoutGuide.TopAnchor, 3).Active = true;
            lblItemCardName.WidthAnchor.ConstraintEqualTo(112).Active = true;
            lblItemCardName.LeftAnchor.ConstraintEqualTo(lblItemCardName.Superview.LeftAnchor, 5).Active = true;
            lblItemCardName.HeightAnchor.ConstraintEqualTo(15).Active = true;

            LblItemCardPrice.TopAnchor.ConstraintEqualTo(lblItemCardName.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            LblItemCardPrice.WidthAnchor.ConstraintEqualTo(112).Active = true;
            LblItemCardPrice.LeftAnchor.ConstraintEqualTo(LblItemCardPrice.Superview.LeftAnchor, 5).Active = true;
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

            #region ColorLayout
            setColorView.TopAnchor.ConstraintEqualTo(setColorView.Superview.TopAnchor, 0).Active = true;
            setColorView.RightAnchor.ConstraintEqualTo(setColorView.Superview.RightAnchor, 0).Active = true;
            setColorView.WidthAnchor.ConstraintEqualTo(220).Active = true;
            setColorView.HeightAnchor.ConstraintEqualTo(154).Active = true;
            // setColorView.HeightAnchor.ConstraintEqualTo(((int)(View.Frame.Height) / 6)).Active = true;

            btnColor1.TopAnchor.ConstraintEqualTo(btnColor1.Superview.TopAnchor, 15).Active = true;
            btnColor1.LeftAnchor.ConstraintEqualTo(btnColor1.Superview.LeftAnchor, 10).Active = true;
            btnColor1.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor1.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor2.TopAnchor.ConstraintEqualTo(btnColor2.Superview.TopAnchor, 15).Active = true;
            btnColor2.LeftAnchor.ConstraintEqualTo(btnColor1.RightAnchor, 10).Active = true;
            btnColor2.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor2.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor3.TopAnchor.ConstraintEqualTo(btnColor3.Superview.TopAnchor, 15).Active = true;
            btnColor3.LeftAnchor.ConstraintEqualTo(btnColor2.RightAnchor, 10).Active = true;
            btnColor3.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor3.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor4.TopAnchor.ConstraintEqualTo(btnColor4.Superview.TopAnchor, 15).Active = true;
            btnColor4.LeftAnchor.ConstraintEqualTo(btnColor3.RightAnchor, 10).Active = true;
            btnColor4.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor4.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor5.TopAnchor.ConstraintEqualTo(btnColor5.Superview.TopAnchor, 15).Active = true;
            btnColor5.LeftAnchor.ConstraintEqualTo(btnColor4.RightAnchor, 10).Active = true;
            btnColor5.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor5.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor6.TopAnchor.ConstraintEqualTo(btnColor1.BottomAnchor, 10).Active = true;
            btnColor6.LeftAnchor.ConstraintEqualTo(btnColor6.Superview.LeftAnchor, 10).Active = true;
            btnColor6.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor6.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor7.TopAnchor.ConstraintEqualTo(btnColor2.BottomAnchor, 10).Active = true;
            btnColor7.LeftAnchor.ConstraintEqualTo(btnColor6.RightAnchor, 10).Active = true;
            btnColor7.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor7.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor8.TopAnchor.ConstraintEqualTo(btnColor3.BottomAnchor, 10).Active = true;
            btnColor8.LeftAnchor.ConstraintEqualTo(btnColor7.RightAnchor, 10).Active = true;
            btnColor8.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor8.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnColor9.TopAnchor.ConstraintEqualTo(btnColor4.BottomAnchor, 10).Active = true;
            btnColor9.LeftAnchor.ConstraintEqualTo(btnColor8.RightAnchor, 10).Active = true;
            btnColor9.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor9.WidthAnchor.ConstraintEqualTo(32).Active = true;

            btnChangeItemPhoto.TopAnchor.ConstraintEqualTo(btnColor5.BottomAnchor, 10).Active = true;
            btnChangeItemPhoto.LeftAnchor.ConstraintEqualTo(btnColor9.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnChangeItemPhoto.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnChangeItemPhoto.WidthAnchor.ConstraintEqualTo(32).Active = true;

            #region fav
            favImg.TopAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            favImg.LeftAnchor.ConstraintEqualTo(favImg.Superview.LeftAnchor, 10).Active = true;
            favImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            favImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblFav.TopAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            lblFav.LeftAnchor.ConstraintEqualTo(favImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblFav.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblFav.RightAnchor.ConstraintEqualTo(lblFav.Superview.RightAnchor, -15).Active = true;
            #endregion
            #endregion

            line1.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line1.WidthAnchor.ConstraintEqualTo(line1.Superview.WidthAnchor).Active = true;
            line1.LeftAnchor.ConstraintEqualTo(line1.Superview.LeftAnchor, 0).Active = true;
            line1.RightAnchor.ConstraintEqualTo(line1.Superview.RightAnchor, 0).Active = true;
            line1.HeightAnchor.ConstraintEqualTo(5).Active = true;

            #region setItemNameView
            setItemNameView.TopAnchor.ConstraintEqualTo(line1.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            setItemNameView.RightAnchor.ConstraintEqualTo(setItemNameView.Superview.RightAnchor, 0).Active = true;
            setItemNameView.LeftAnchor.ConstraintEqualTo(setItemNameView.Superview.LeftAnchor, 0).Active = true;
            setItemNameView.WidthAnchor.ConstraintEqualTo(setItemNameView.Superview.WidthAnchor).Active = true;
            setItemNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemName.TopAnchor.ConstraintEqualTo(lblItemName.Superview.TopAnchor, 11).Active = true;
            lblItemName.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblItemName.LeftAnchor.ConstraintEqualTo(lblItemName.Superview.LeftAnchor, 15).Active = true;
            lblItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemName.TopAnchor.ConstraintEqualTo(lblItemName.BottomAnchor, 2).Active = true;
            txtItemName.WidthAnchor.ConstraintEqualTo(300).Active = true;
            txtItemName.LeftAnchor.ConstraintEqualTo(txtItemName.Superview.LeftAnchor, 15).Active = true;
            txtItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line2.TopAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line2.WidthAnchor.ConstraintEqualTo(line2.Superview.WidthAnchor).Active = true;
            line2.LeftAnchor.ConstraintEqualTo(line2.Superview.LeftAnchor, 0).Active = true;
            line2.RightAnchor.ConstraintEqualTo(line2.Superview.RightAnchor, 0).Active = true;
            line2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region SetItemPriceView
            SetItemPriceView.TopAnchor.ConstraintEqualTo(line2.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SetItemPriceView.RightAnchor.ConstraintEqualTo(SetItemPriceView.Superview.RightAnchor, 0).Active = true;
            SetItemPriceView.WidthAnchor.ConstraintEqualTo(SetItemPriceView.Superview.WidthAnchor).Active = true;
            SetItemPriceView.LeftAnchor.ConstraintEqualTo(SetItemPriceView.Superview.LeftAnchor, 0).Active = true;
            SetItemPriceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemPrice.TopAnchor.ConstraintEqualTo(lblItemPrice.Superview.TopAnchor, 11).Active = true;
            lblItemPrice.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblItemPrice.LeftAnchor.ConstraintEqualTo(lblItemPrice.Superview.LeftAnchor, 15).Active = true;
            lblItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemPrice.TopAnchor.ConstraintEqualTo(lblItemPrice.BottomAnchor, 2).Active = true;
            txtItemPrice.WidthAnchor.ConstraintEqualTo(300).Active = true;
            txtItemPrice.LeftAnchor.ConstraintEqualTo(txtItemPrice.Superview.LeftAnchor, 15).Active = true;
            txtItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line3.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line3.WidthAnchor.ConstraintEqualTo(line3.Superview.WidthAnchor).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(line3.Superview.LeftAnchor, 0).Active = true;
            line3.RightAnchor.ConstraintEqualTo(line3.Superview.RightAnchor, 0).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(5).Active = true;

            #region DetailClickView
            DetailClickView.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            DetailClickView.RightAnchor.ConstraintEqualTo(DetailClickView.Superview.RightAnchor, 0).Active = true;
            DetailClickView.WidthAnchor.ConstraintEqualTo(DetailClickView.Superview.WidthAnchor).Active = true;
            DetailClickView.LeftAnchor.ConstraintEqualTo(DetailClickView.Superview.LeftAnchor, 0).Active = true;
            DetailClickView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblDetail.CenterYAnchor.ConstraintEqualTo(lblDetail.Superview.CenterYAnchor).Active = true;
            lblDetail.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblDetail.LeftAnchor.ConstraintEqualTo(lblDetail.Superview.LeftAnchor, 15).Active = true;
            lblDetail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnToggleDetail.CenterYAnchor.ConstraintEqualTo(btnToggleDetail.Superview.CenterYAnchor).Active = true;
            btnToggleDetail.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnToggleDetail.RightAnchor.ConstraintEqualTo(btnToggleDetail.Superview.RightAnchor, -22).Active = true;
            btnToggleDetail.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            line4.TopAnchor.ConstraintEqualTo(DetailClickView.BottomAnchor, 0).Active = true;
            line4.WidthAnchor.ConstraintEqualTo(line4.Superview.WidthAnchor).Active = true;
            line4.LeftAnchor.ConstraintEqualTo(line4.Superview.LeftAnchor, 0).Active = true;
            line4.RightAnchor.ConstraintEqualTo(line4.Superview.RightAnchor, 0).Active = true;
            line4.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region DetailCostView
            DetailCostView.TopAnchor.ConstraintEqualTo(line4.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            DetailCostView.RightAnchor.ConstraintEqualTo(DetailCostView.Superview.RightAnchor, 0).Active = true;
            DetailCostView.LeftAnchor.ConstraintEqualTo(DetailCostView.Superview.LeftAnchor, 0).Active = true;
            DetailCostView.WidthAnchor.ConstraintEqualTo(DetailCostView.Superview.WidthAnchor).Active = true;
            DetailCostView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblextraCost.TopAnchor.ConstraintEqualTo(lblextraCost.Superview.TopAnchor, 11).Active = true;
            lblextraCost.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblextraCost.LeftAnchor.ConstraintEqualTo(lblextraCost.Superview.LeftAnchor, 15).Active = true;
            lblextraCost.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtCost.TopAnchor.ConstraintEqualTo(lblextraCost.BottomAnchor, 2).Active = true;
            txtCost.WidthAnchor.ConstraintEqualTo(300).Active = true;
            txtCost.LeftAnchor.ConstraintEqualTo(txtCost.Superview.LeftAnchor, 15).Active = true;
            txtCost.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line5.TopAnchor.ConstraintEqualTo(DetailCostView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line5.WidthAnchor.ConstraintEqualTo(line5.Superview.WidthAnchor).Active = true;
            line5.LeftAnchor.ConstraintEqualTo(line5.Superview.LeftAnchor, 0).Active = true;
            line5.RightAnchor.ConstraintEqualTo(line5.Superview.RightAnchor, 0).Active = true;
            line5.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region DetailCategoryView
            DetailCategoryView.TopAnchor.ConstraintEqualTo(line5.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DetailCategoryView.RightAnchor.ConstraintEqualTo(DetailCategoryView.Superview.RightAnchor, 0).Active = true;
            DetailCategoryView.LeftAnchor.ConstraintEqualTo(DetailCategoryView.Superview.LeftAnchor, 0).Active = true;
            DetailCategoryView.WidthAnchor.ConstraintEqualTo(DetailCategoryView.Superview.WidthAnchor).Active = true;
            DetailCategoryView.BottomAnchor.ConstraintEqualTo(DetailCategoryView.Superview.BottomAnchor, 0).Active = true;
            DetailCategoryView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblExteaCategoryName.TopAnchor.ConstraintEqualTo(lblExteaCategoryName.Superview.TopAnchor, 11).Active = true;
            lblExteaCategoryName.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblExteaCategoryName.LeftAnchor.ConstraintEqualTo(lblExteaCategoryName.Superview.LeftAnchor, 15).Active = true;
            lblExteaCategoryName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtExtraCategoryName.TopAnchor.ConstraintEqualTo(lblExteaCategoryName.BottomAnchor, 2).Active = true;
            txtExtraCategoryName.WidthAnchor.ConstraintEqualTo(300).Active = true;
            txtExtraCategoryName.LeftAnchor.ConstraintEqualTo(txtExtraCategoryName.Superview.LeftAnchor, 15).Active = true;
            txtExtraCategoryName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            SetCategory.CenterYAnchor.ConstraintEqualTo(SetCategory.Superview.CenterYAnchor).Active = true;
            SetCategory.WidthAnchor.ConstraintEqualTo(28).Active = true;
            SetCategory.RightAnchor.ConstraintEqualTo(SetCategory.Superview.RightAnchor, -25).Active = true;
            SetCategory.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            line7.TopAnchor.ConstraintEqualTo(DetailCategoryView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line7.WidthAnchor.ConstraintEqualTo(line7.Superview.WidthAnchor).Active = true;
            line7.LeftAnchor.ConstraintEqualTo(line7.Superview.LeftAnchor, 0).Active = true;
            line7.RightAnchor.ConstraintEqualTo(line7.Superview.RightAnchor, 0).Active = true;
            line7.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            #endregion

            #region StockViewLayout
            stockView.TopAnchor.ConstraintEqualTo(menuAddItemBarCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            stockView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            stockView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            stockView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor).Active = true;

            StockbarView.TopAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            StockbarView.RightAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            StockbarView.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            StockbarView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            disableView.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            disableView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            disableView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            disableView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

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

            lines3.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor, 120).Active = true;
            lines3.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lines3.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lines3.HeightAnchor.ConstraintEqualTo(5).Active = true;

            lblStockMinimum.TopAnchor.ConstraintEqualTo(lines3.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblStockMinimum.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblStockMinimum.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtStockMinimum.TopAnchor.ConstraintEqualTo(lblStockMinimum.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtStockMinimum.WidthAnchor.ConstraintEqualTo(200).Active = true;
            txtStockMinimum.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;

            line45.TopAnchor.ConstraintEqualTo(txtStockMinimum.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            line45.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line45.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line45.HeightAnchor.ConstraintEqualTo(5).Active = true;

            viewstockmove.TopAnchor.ConstraintEqualTo(line45.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            viewstockmove.RightAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            viewstockmove.LeftAnchor.ConstraintEqualTo(stockView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            viewstockmove.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblStockmove.CenterYAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblStockmove.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblStockmove.LeftAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            lblStockmove.HeightAnchor.ConstraintEqualTo(20).Active = true;

            imgSelectStockmove.CenterYAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imgSelectStockmove.WidthAnchor.ConstraintEqualTo(28).Active = true;
            imgSelectStockmove.RightAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            imgSelectStockmove.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lineEmpty.TopAnchor.ConstraintEqualTo(viewstockmove.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            lineEmpty.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lineEmpty.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lineEmpty.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            #endregion
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        [Export("Stockmove:")]
        public void Stockmove(UIGestureRecognizer sender)
        {
            if (addItem != null && addItem.FTrackStock == 1)
            {
                StockMovementController stockMovementController = new StockMovementController(addItem);
                this.NavigationController.PushViewController(stockMovementController, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notfoundstockmove", "notfoundstockmove"));
            }
        }

        [Export("OnHand:")]
        public void OnHand(UIGestureRecognizer sender)
        {
            ItemStockOnHandController onHandPage = new ItemStockOnHandController("Extra", lblStockNumber.Text);
            this.NavigationController.PushViewController(onHandPage, false);
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
    }
   
}