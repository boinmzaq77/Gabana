using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Gabana.iOS.ITEMS
{
    public partial class AddItemController : UIViewController
    {
        public int idCategory;
        public static UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
        ItemDetailDataSource itemDetailDataSource;
        UICollectionView menuAddItemBarCollectionView;
        Gabana.ShareSource.Manage.ItemManage itemManager = new Gabana.ShareSource.Manage.ItemManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        int flagBtn = 0;
        #region ItemViewComponent
        public static int flagDetail = 0;
        private static byte[] picture;
        MenuAddItemDataSource menuAddItemDataSource;
        UIView bottomView;
        public static UICollectionView ScrollView;
        UIButton btnAdd;
        public static int wide=0;
        public static int tall = 0;
        public static long sysItemIDEdit;
        #endregion
        #region StockViewComponent
        UIView StockbarView, line3, disableView;
        UILabel lblStock, lblStockMinimum, lblStockNumber;
        UITextField txtStockMinimum;
        UISwitch switchStock;
        #endregion
        Gabana.ORM.MerchantDB.Item addItem = new Gabana.ORM.MerchantDB.Item();
        public AddItemController()
        {
        }
        public AddItemController(long sysItemID)
        {
            sysItemIDEdit = sysItemID;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }

        public async override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            //AddItemControllerScroll

            addItem.Colors = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
            View.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            #region MenuItem

            var flowLayout = new POS.MenuBarCollectionViewLayout();
            flowLayout.SizeForItem += (collectionView, layout, indexPath) =>
            {
                NSString nSString = new NSString((menuAddItemBarCollectionView.DataSource as MenuAddItemDataSource).GetItem(indexPath.Row));
                UIFont font = UIFont.SystemFontOfSize(18);
                CGSize cGSize = nSString.StringSize(font);

                return new CGSize((View.Frame.Width / 2), 38); //CGSize(cGSize.Width + 30
            };

            menuAddItemBarCollectionView = new UICollectionView(frame: View.Frame, layout: flowLayout);
            menuAddItemBarCollectionView.BackgroundColor = UIColor.White;
            menuAddItemBarCollectionView.ShowsHorizontalScrollIndicator = true;
            menuAddItemBarCollectionView.ScrollEnabled = false;
            menuAddItemBarCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            menuAddItemBarCollectionView.RegisterClassForCell(cellType: typeof(MenuCollectionViewCell), reuseIdentifier: "menuItemCell");
            menuAddItemDataSource = new MenuAddItemDataSource();
            menuAddItemBarCollectionView.DataSource = menuAddItemDataSource;
            ItemMenuCollectionDelegate CollectionDelegate = new ItemMenuCollectionDelegate();
            CollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (indexPath).Item;
                if (x == 0)
                {
                    //Add item
                    flagBtn = 0;
                    btnAdd.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                    btnAdd.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
                    btnAdd.BackgroundColor = UIColor.White;
                    disableView.Hidden = true;
                    if (flagDetail == 0)
                    {
                        ScrollView.Hidden = false;
                    }
                    else
                    {
                        ScrollView.Hidden = true;
                    }
                    setToggleMenu(false);
                }
                else if (x == 1)
                {
                    //Stock
                    flagBtn = 1;
                    btnAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAdd.Layer.BorderColor = UIColor.FromRGB(51, 170, 225).CGColor;
                    btnAdd.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    if (switchStock.On == false)
                    {
                        disableView.Hidden = false;
                    }
                    ScrollView.Hidden = false;
                    setToggleMenu(true);
                }
            };
            menuAddItemBarCollectionView.Delegate = CollectionDelegate;
            View.AddSubview(menuAddItemBarCollectionView);
            #endregion
            #region bottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.White;
            View.AddSubview(bottomView);

            btnAdd = new UIButton();
            btnAdd.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnAdd.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnAdd.BackgroundColor = UIColor.White;
            btnAdd.Layer.CornerRadius = 5f;
            btnAdd.Layer.BorderWidth = 0.5f;
            btnAdd.Enabled = true;
            if (sysItemIDEdit != null && sysItemIDEdit != 0)
            {
                btnAdd.SetTitle("Edit Item", UIControlState.Normal);
            }
            else
            {
                btnAdd.SetTitle("Add Item", UIControlState.Normal);
            }
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAdd.TouchUpInside += async (sender, e) => {
                try
                {
                    if (ItemAddDatailViewCell.txtItemCost.Text == "")
                    {
                        ItemAddDatailViewCell.txtItemCost.Text = "0";
                    }
                    if (flagBtn == 0)
                    {
                        int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);

                        var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");
                        ItemManage a = new ItemManage();
                        var it = await a.GetAllItem();
                        if ((ItemAddDatailViewCell.txtItemName.Text != "" && ItemAddDatailViewCell.txtItemName.Text != null) && (ItemAddDatailViewCell.txtItemPrice.Text != "" && ItemAddDatailViewCell.txtItemPrice.Text != null)
                            && (ItemAddDatailViewCell.txtItemPrice.Text != "" || ItemAddDatailViewCell.txtItemPrice.Text != null))
                        {
                            addItem.MerchantID = MainController.merchantlocal.MerchantID;
                            addItem.SysItemID = long.Parse(sys); //last max sysitemid
                            addItem.ItemName = ItemAddDatailViewCell.txtItemName.Text.ToString();
                            addItem.FavoriteNo = 0;
                            if (ItemAddDatailViewCell.txtItemCost.Text == null || ItemAddDatailViewCell.txtItemCost.Text == "")
                            {
                                addItem.EstimateCost = 0;
                            }
                            else
                            {
                                addItem.EstimateCost = Convert.ToDecimal(ItemAddDatailViewCell.txtItemCost.Text);
                            }
                            addItem.ItemCode = ItemAddDatailViewCell.txtItemCode.Text ?? "";
                            addItem.Price = Convert.ToDecimal(ItemAddDatailViewCell.txtItemPrice.Text);
                            long? category = null;
                            if (ItemAddDatailViewCell.lblSelectedCategory.Text == "None")
                            {
                                category = null;
                            }
                            else
                            {
                                category = ItemAddDatailViewCell.CatID;
                            }
                            addItem.SysCategoryID = category;
                            addItem.OptSalePrice = 'F';
                            if (ItemAddDatailViewCell.lblVatMode.Text == "Include Vat")
                            {
                                addItem.TaxType = 'V'; // addItem.Vat
                            }
                            if (ItemAddDatailViewCell.lblVatMode.Text == "None Vat")
                            {
                                addItem.TaxType = 'N'; // addItem.Vat
                            }
                            addItem.UnitName = null;
                            addItem.RegularSizeName = null;
                            addItem.PicturePath = "";
                            addItem.Ordinary = 2;
                            addItem.SellBy = 'U';
                            addItem.FTrackStock = 0;
                            addItem.TrackStockDateTime = DateTime.UtcNow;
                            addItem.SaleItemType = 'U';
                            addItem.LastDateModified = DateTime.UtcNow;
                            addItem.UserLastModified = DataCashingAll.MerchantId.ToString();
                            addItem.DataStatus = 'I';
                            addItem.FWaitSending = 1;
                            addItem.LinkProMaxxItemID = null;
                            addItem.LinkProMaxxItemUnit = null;
                            addItem.WaitSendingTime = DateTime.UtcNow;
                            if (ItemAddDatailViewCell.txtItemName.Text.Length > 5)
                            {
                                addItem.ShortName = ItemAddDatailViewCell.txtItemName.Text.Substring(0, 5);
                            }
                            else
                            {
                                addItem.ShortName = ItemAddDatailViewCell.txtItemName.Text;
                            }
                            addItem.Colors = ItemAddDatailViewCell.addColor;
                            if (sysItemIDEdit != null && sysItemIDEdit != 0) // update
                            {
                                UpdateItemToDB();
                            }
                            else // insert
                            {
                                addItemToDB();
                            }
                        }
                        else
                        {
                            Utils.ShowAlert(this, "ไม่สำเร็จ !", "กรุณากรอกข้อมูลให้ครบถ้วน");
                        }
                    }
                    if (flagBtn == 1)
                    {
                        // add stock
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowMessage(ex.Message);
                }
            };
            View.AddSubview(btnAdd);
            #endregion
            #region ItemViewAttribute
            wide = (int)View.Frame.Width;
            //  tall = ((int)View.Frame.Height*125)/100;
            //  tall = ((int)View.Frame.Height);
            tall = 800;
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: wide, height:tall);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            ScrollView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            ScrollView.BackgroundColor = UIColor.White;
            ScrollView.ShowsVerticalScrollIndicator = true;
            ScrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            ScrollView.Hidden = false;

            ScrollView.RegisterClassForCell(cellType: typeof(ItemAddDatailViewCell), reuseIdentifier: "ItemDetailDataSource");
            if (sysItemIDEdit == null || sysItemIDEdit == 0)
            {
                itemDetailDataSource = new ItemDetailDataSource();
                btnAdd.SetTitle("Add Item", UIControlState.Normal);
            }
            else
            {
                itemDetailDataSource = new ItemDetailDataSource(sysItemIDEdit);
                btnAdd.SetTitle("Edit Item", UIControlState.Normal);
            }
            ScrollView.DataSource = itemDetailDataSource;
            View.AddSubview(ScrollView);


            #endregion
            #region StockViewAttribute
            StockbarView = new UIView();
            StockbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            StockbarView.BackgroundColor = UIColor.White;
            StockbarView.Hidden = true;
            View.AddSubview(StockbarView);

            disableView = new UIView();
            disableView.TranslatesAutoresizingMaskIntoConstraints = false;
            disableView.BackgroundColor = UIColor.White;
            disableView.Layer.Opacity = 0.8f;
            disableView.Hidden = true;
            View.AddSubview(disableView);

            lblStock = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStock.Font = lblStock.Font.WithSize(15);
            lblStock.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStock.Text = "Stock";
            lblStock.Hidden = true;
            View.AddSubview(lblStock);

            switchStock = new UISwitch();
            switchStock.TranslatesAutoresizingMaskIntoConstraints = false;
            switchStock.Hidden = true;
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
            View.AddSubview(switchStock);

            lblStockNumber = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockNumber.Font = lblStock.Font.WithSize(45);
            lblStockNumber.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockNumber.Text = "0";
            lblStockNumber.Hidden = true;
            View.AddSubview(lblStockNumber);

            lblStockMinimum = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblStockMinimum.Font = lblStock.Font.WithSize(15);
            lblStockMinimum.TranslatesAutoresizingMaskIntoConstraints = false;
            lblStockMinimum.Text = "Minimum Stock";
            lblStockMinimum.Hidden = true;
            View.AddSubview(lblStockMinimum);

            line3 = new UIView();
            line3.Hidden = true;
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            line3.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            View.AddSubview(line3);

            txtStockMinimum = new UITextField
            {
                Placeholder = "0",
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtStockMinimum.Hidden = true;
            txtStockMinimum.Enabled = false;
            txtStockMinimum.Font = txtStockMinimum.Font.WithSize(15);
            txtStockMinimum.KeyboardType = UIKeyboardType.NumberPad;
            View.AddSubview(txtStockMinimum);
            #endregion
            // setColorButton();
            setupAutoLayout();
            try
            {
                Textboxfocus(View);
                setCategory();
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, "ไม่สำเร็จ !! ", ex.Message);
            }
        }
        public static void ChangeDetailTall(int count){
            itemflowLayoutList.ItemSize = new CGSize(wide, tall + (250*count)-45);
        }
        
        private async void UpdateItemToDB()
        {
            byte[] stream = null;
            if (picture != null)
            {
                stream = picture;
                addItem.PictureLocalPath = stream.ToString();
                addItem.Colors = null;
            }
            var check = await itemManager.UpdateItem(addItem);
            if (check)
            {
                //done insert
                this.NavigationController.PopViewController(false);
                Utils.ShowAlert(this, "สำเร็จ !", "แก้ไขข้อมูลสำเร็จ");
                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
                }
            }
            else
            {
                //error insert
                Utils.ShowAlert(this, "ไม่สำเร็จ !", "ไม่สามารถแก้ไขข้อมูลได้");
            }
        }
        private async void addItemToDB()
        {
            byte[] stream = null;
            if (picture != null)
            {
                stream = picture;
                addItem.PictureLocalPath = stream.ToString();
                addItem.Colors = null;
            }
            var itemOnBranch = new ItemOnBranch()
            {
                MerchantID = addItem.MerchantID,
                SysBranchID = DataCashingAll.SysBranchId,
                SysItemID = addItem.SysItemID,
                BalanceStock = Convert.ToDecimal("100"),
                MinimumStock = Convert.ToDecimal("100"),
            };
            var check = await itemManager.InsertItem(addItem, itemOnBranch);
            if(check)
            {

                //done insert
                this.NavigationController.PopViewController(false);
                Utils.ShowAlert(this, "สำเร็จ !", "เพิ่มข้อมูลสำเร็จ");
                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
                }
            }
            else
            {
                //error insert
                Utils.ShowAlert(this, "ไม่สำเร็จ !", "ไม่สามารถเพิ่มข้อมูลได้");
            }
        }
        void setToggleMenu(bool x)
        {
            ScrollView.Hidden = x;
            //------------------------------- press stock -------------------------
            StockbarView.Hidden = !x;
            lblStock.Hidden = !x;
            switchStock.Hidden = !x;
            lblStockNumber.Hidden = !x;
            line3.Hidden = !x;
            lblStockMinimum.Hidden =! x;
            txtStockMinimum.Hidden = !x;
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void setupAutoLayout()
        {
            #region StockViewLayout
            StockbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 40).Active = true;
            StockbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            StockbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            StockbarView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            View.BringSubviewToFront(disableView);
            disableView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 85).Active = true;
            disableView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            disableView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            disableView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            lblStock.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.TopAnchor, 13).Active = true;
            lblStock.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblStock.LeftAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.LeftAnchor, 30).Active = true;
            lblStock.HeightAnchor.ConstraintEqualTo(18).Active = true;

            switchStock.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.TopAnchor, 13).Active = true;
            switchStock.WidthAnchor.ConstraintEqualTo(52).Active = true;
            switchStock.RightAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.RightAnchor, -22).Active = true;
            switchStock.HeightAnchor.ConstraintEqualTo(32).Active = true;

            lblStockNumber.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor, 35).Active = true;
            lblStockNumber.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblStockNumber.HeightAnchor.ConstraintEqualTo(54).Active = true;

            line3.TopAnchor.ConstraintEqualTo(StockbarView.SafeAreaLayoutGuide.BottomAnchor, 120).Active = true;
            line3.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(5).Active = true;

            lblStockMinimum.TopAnchor.ConstraintEqualTo(line3.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblStockMinimum.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblStockMinimum.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtStockMinimum.TopAnchor.ConstraintEqualTo(lblStockMinimum.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtStockMinimum.WidthAnchor.ConstraintEqualTo(200).Active = true;
            txtStockMinimum.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtStockMinimum.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region ItemViewLayout
                    ScrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 40).Active = true;
                    ScrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
                    ScrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                    ScrollView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            
                    #region bottomView
                             bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
                            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

                            btnAdd.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
                            btnAdd.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
                            btnAdd.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
                            btnAdd.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,10).Active = true;
                    #endregion
            #endregion
        }
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

            private UILabel personLabel;


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
                    Name = "None",
                    SysCategoryID = 0
                };
                category.Add(addcategory);
                getallCategory = await categoryManage.GetAllCategory();
                category.AddRange(getallCategory);

                PickerCategoryModel model2 = new PickerCategoryModel(category);
                model2.PickerChanged += async (sender, e) =>
                {
                    ItemAddDatailViewCell.lblSelectedCategory.Text = e.SelectedValue;
                    idCategory = (int)e.ID;
                };

                UIToolbar toolbar = new UIToolbar();
                toolbar.BarStyle = UIBarStyle.Default;
                toolbar.Translucent = true;
                toolbar.SizeToFit();
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton = new UIBarButtonItem("DONE", UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("CategoryAction"));
                toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
                ItemAddDatailViewCell.lblSelectedCategory.InputView = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
                ItemAddDatailViewCell.lblSelectedCategory.InputAccessoryView = toolbar;
            }
            catch (Exception ex)
            {
               // Log.Debug("error", ex.Message);
            }
            

           
            
        }
        [Export("CategoryAction")]
        private void DoneAction4()
        {
            ItemAddDatailViewCell.lblSelectedCategory.ResignFirstResponder();

        }
    }
}