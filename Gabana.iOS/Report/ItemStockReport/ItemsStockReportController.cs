using CoreGraphics;
using Foundation;
using Gabana.iOS.ITEMS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class ItemsStockReportController : UIViewController
    {
        int MenuSelected=0;
        UIImageView emptyItemView, emptyExtraToppingView, emptyCategoryView;
        UILabel lbl_empty_Item, lbl_empty_ExtaTopping, lbl_empty_Category;
        UIView ItemView, CategoryView, ToppingView;
        ItemStockReportDataSource itemDataList;
        UICollectionView itemListCollectionView, CatagoryListCollectionView,ToppingCollectionView;
        UIButton btnSearch,btnSelectAll,btnApply;
        UITextField txtSearch;
        UIBarButtonItem backButton;
        UIView SearchbarView;
        public static List<Item> items;
        public static List<Item> itemExtra;
        static List<Category> Categories;
        CategoryManage CategoryManager = new CategoryManage();
        ItemManage itemmanager = new ItemManage();

        private static List<Item> listChooseItem = new List<Item>();
        private static List<Item> listChooseItemExtra = new List<Item>();
        public static List<Category> listChooseCategory = new List<Category>();
        int listItemBodyCount;

        List<Item> allitem;
        static ListItem listItem, listExtraItem;
        private static List<Category> lstCategory;
        private static ListCategory listCategory;
        UIView MenuView, ItemMenuView, ExtraMenuView, CategoryMenuView;
        UILabel lblItemMenu, lblExtraMenu, lblCategoryMenu;
        UIView ItemLineView, ExtraLineView, CategoryLineView;

        ItemStockReportDetailController DetailPage = null;

        private bool selcetitemall;
        private string categorySelect;
        private string extraSelect;
        private string itemSelect;

        UIBarButtonItem selectBranch;
        ReportBranchController ReportBranchPage = null;
        private bool dateend;
        private bool datestart;
        NSDateFormatter dateFormatter;
        public ItemsStockReportController()
        {
            
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            this.NavigationController.SetNavigationBarHidden(false, false);
            txtSearch.Text = null;
            
        }
        public async override void ViewDidLoad()
        {
            try
            {
                this.NavigationController.SetNavigationBarHidden(false, false);
                base.ViewDidLoad();



                

                View.BackgroundColor = UIColor.White;

                initAttribute();
                setupAutoLayout();
                SetListItem();
                SetListExtra();
                SetListCategory();
                showList();
                setupButtonMenu();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }

        void setupButtonMenu()
        {

            lblItemMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            ItemLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblExtraMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            ExtraLineView.BackgroundColor = UIColor.White;

            lblCategoryMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            CategoryLineView.BackgroundColor = UIColor.White;

        }
        void initAttribute()
        {
            #region MenuView
            MenuView = new UIView();
            MenuView.BackgroundColor = UIColor.White;
            MenuView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(MenuView);

            #region ItemMenuView
            ItemMenuView = new UIView();
            ItemMenuView.BackgroundColor = UIColor.White;
            ItemMenuView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(ItemMenuView);

            lblItemMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemMenu.Text = "Item";
            lblItemMenu.Font = lblItemMenu.Font.WithSize(15);
            ItemMenuView.AddSubview(lblItemMenu);

            ItemLineView = new UIView();
            ItemLineView.BackgroundColor = UIColor.White;
            ItemLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemMenuView.AddSubview(ItemLineView);

            ItemMenuView.UserInteractionEnabled = true;
            var tapGesturebtnItem = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ItemMenu:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ItemMenuView.AddGestureRecognizer(tapGesturebtnItem);
            #endregion

            #region ExtraMenuView
            ExtraMenuView = new UIView();
            ExtraMenuView.BackgroundColor = UIColor.White;
            ExtraMenuView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(ExtraMenuView);

            lblExtraMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblExtraMenu.Text = "Extra Topping";
            lblExtraMenu.Font = lblExtraMenu.Font.WithSize(15);
            ExtraMenuView.AddSubview(lblExtraMenu);

            ExtraLineView = new UIView();
            ExtraLineView.BackgroundColor = UIColor.White;
            ExtraLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            ExtraMenuView.AddSubview(ExtraLineView);

            ExtraMenuView.UserInteractionEnabled = true;
            var tapGesturebtnExtra = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ExtraMenu:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ExtraMenuView.AddGestureRecognizer(tapGesturebtnExtra);
            #endregion

            #region CategoryMenuView
            CategoryMenuView = new UIView();
            CategoryMenuView.BackgroundColor = UIColor.White;
            CategoryMenuView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(CategoryMenuView);

            lblCategoryMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCategoryMenu.Text = "Category";
            lblCategoryMenu.Font = lblCategoryMenu.Font.WithSize(15);
            CategoryMenuView.AddSubview(lblCategoryMenu);

            CategoryLineView = new UIView();
            CategoryLineView.BackgroundColor = UIColor.White;
            CategoryLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            CategoryMenuView.AddSubview(CategoryLineView);

            CategoryMenuView.UserInteractionEnabled = true;
            var tapGesturebtnCategory = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("CategoryMenu:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            CategoryMenuView.AddGestureRecognizer(tapGesturebtnCategory);
            #endregion
            #endregion

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.White;
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

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
            SearchbarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                txtSearch.BecomeFirstResponder();
            };
            SearchbarView.AddSubview(btnSearch);

            btnSelectAll = new UIButton();
            btnSelectAll.SetTitle("All", UIControlState.Normal);
            btnSelectAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnSelectAll.Layer.BorderWidth = 1;
            btnSelectAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelectAll.Layer.CornerRadius = 5;
            btnSelectAll.ClipsToBounds = false;
            btnSelectAll.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectAll.TouchUpInside += async (sender, e) =>
            {
                if (!selcetitemall)
                {
                    selcetitemall = true;
                    if (MenuSelected == 0)
                    {
                        listChooseItem = new List<Item>();
                        listChooseItem = items;
                        itemSelect = "All Items";

                    }
                    else if (MenuSelected == 1)
                    {
                        listChooseItemExtra = new List<Item>();
                        listChooseItemExtra = itemExtra;
                        extraSelect = "All Extra Topping";
                    }
                    else
                    {
                        listChooseCategory = new List<Category>();
                        listChooseCategory = lstCategory;
                        categorySelect = "All Category";
                    }

                }
                else
                {
                    itemSelect = "";
                    extraSelect = "";
                    categorySelect = "";

                    listChooseItem = new List<Item>();
                    listChooseItemExtra = new List<Item>();
                    listChooseCategory = new List<Category>();
                    selcetitemall = false;
                }
                SetDataItem();
                SetShowButton();
            };
            SearchbarView.AddSubview(btnSelectAll);

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
            lbl_empty_Item.Text = "คุณยังไม่มี Item สามารถเพิ่ม\n ได้ที่หน้า Add Item";
            ItemView.AddSubview(lbl_empty_Item);
            #endregion
            #region itemListCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumLineSpacing = 0;

            itemListCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            itemListCollectionView.BackgroundColor = UIColor.White;
            itemListCollectionView.ShowsVerticalScrollIndicator = false;
            itemListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            itemListCollectionView.RegisterClassForCell(cellType: typeof(ItemStockReportViewCell), reuseIdentifier: "ItemStockReportViewCell");

            ItemStockReportCollectionDelegate itemCollectionDelegate = new ItemStockReportCollectionDelegate();
            itemCollectionDelegate.OnItemSelected += (indexPath) => {
                int x = (int)indexPath.Row;
                ItemClick(x);
            };
            itemListCollectionView.Delegate = itemCollectionDelegate;
            ItemView.AddSubview(itemListCollectionView);
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
            lbl_empty_Category.Text = "คุณยังไม่มี Category สามารถเพิ่ม\n ได้ที่ปุ่ม Add Category ด้านล่าง";
            CategoryView.AddSubview(lbl_empty_Category);
            #endregion
            #region CatagoryListCollectionView

            UICollectionViewFlowLayout CategoryflowLayoutList = new UICollectionViewFlowLayout();
            CategoryflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            CategoryflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            CategoryflowLayoutList.MinimumLineSpacing = 0;

            CatagoryListCollectionView = new UICollectionView(frame: View.Frame, layout: CategoryflowLayoutList);
            CatagoryListCollectionView.BackgroundColor = UIColor.White;
            CatagoryListCollectionView.ShowsVerticalScrollIndicator = false;
            CatagoryListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CatagoryListCollectionView.RegisterClassForCell(cellType: typeof(CatagoryReportViewCell), reuseIdentifier: "CatagoryReportViewCell");

            CatagoryReportCollectionDelegate itemCatCollectionDelegate = new CatagoryReportCollectionDelegate();
            itemCatCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                CategoryClick((int)indexPath.Row);
            };
            CatagoryListCollectionView.Delegate = itemCatCollectionDelegate;
            CategoryView.AddSubview(CatagoryListCollectionView);
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
            lbl_empty_ExtaTopping.Text = "คุณยังไม่มี Extra Topping สามารถเพิ่ม\n ได้ที่หน้า Add Extra Topping";
            ToppingView.AddSubview(lbl_empty_ExtaTopping);
            #endregion
            #region ToppingListCollectionView
            UICollectionViewFlowLayout ToppingflowLayoutList = new UICollectionViewFlowLayout();
            ToppingflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            ToppingflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            ToppingflowLayoutList.MinimumLineSpacing = 0;

            ToppingCollectionView = new UICollectionView(frame: View.Frame, layout: ToppingflowLayoutList);
            ToppingCollectionView.BackgroundColor = UIColor.White;
            ToppingCollectionView.ShowsVerticalScrollIndicator = false;
            ToppingCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingCollectionView.RegisterClassForCell(cellType: typeof(ItemStockReportViewCell), reuseIdentifier: "ItemStockReportViewCell");

            ItemStockReportCollectionDelegate itemExtraToppingCollectionDelegate = new ItemStockReportCollectionDelegate();
            itemExtraToppingCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (int)indexPath.Item;
                ExtraClick(x);
            };
            ToppingCollectionView.Delegate = itemExtraToppingCollectionDelegate;
            ToppingView.AddSubview(ToppingCollectionView);
            #endregion
            #endregion

            #region bottomview
            btnApply = new UIButton();
            btnApply.SetTitle("View Report ", UIControlState.Normal);
            btnApply.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnApply.Layer.BorderWidth = 1;
            btnApply.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnApply.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnApply.Layer.CornerRadius = 5;
            btnApply.ClipsToBounds = false;
            btnApply.TranslatesAutoresizingMaskIntoConstraints = false;
            btnApply.TouchUpInside += (sender, e) =>
            {
                List<int> lstsysitem = new List<int>();
                List<int> lstsysextra = new List<int>();
                List<int> lstsyscategory = new List<int>();
                if((listChooseItem.Count >0  && MenuSelected == 0 )||(MenuSelected == 1 && listChooseItemExtra.Count > 0) || ( listChooseCategory.Count > 0&& MenuSelected == 2))
                {
                    if (MenuSelected == 0)
                    {
                        if (listChooseItem.Count == 0) return;

                        foreach (var item in listChooseItem)
                        {
                            lstsysitem.Add((int)item.SysItemID);
                        }
                        DetailPage = new ItemStockReportDetailController("I", lstsysitem, itemSelect);
                    }
                    else if (MenuSelected == 1)
                    {
                        if (listChooseItemExtra.Count == 0) return;

                        foreach (var item in listChooseItemExtra)
                        {
                            lstsysextra.Add((int)item.SysItemID);
                        }
                        DetailPage = new ItemStockReportDetailController("I", lstsysextra, extraSelect);
                    }
                    else
                    {
                        if (listChooseCategory.Count == 0) return;

                        foreach (var item in listChooseCategory)
                        {
                            lstsyscategory.Add((int)item.SysCategoryID);
                        }
                        DetailPage = new ItemStockReportDetailController("C", lstsyscategory, categorySelect);
                    }
                    this.NavigationController.PushViewController(DetailPage, false);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณาเลือก Item หรือ Category");
                }
            };
            View.AddSubview(btnApply);
            #endregion
        }
        private async void ExtraClick(int e)
        {
                var extraItem = itemExtra[e];

                var search = listChooseItemExtra.FindIndex(x => x.SysItemID == extraItem.SysItemID);
                if (search == -1)
                {
                    listChooseItemExtra.Add(extraItem);
                }
                else
                {
                    listChooseItemExtra.RemoveAt(search);
                }

                SetListExtra();

                extraSelect = "";
                if (itemExtra.Count == listChooseItemExtra.Count)
                {
                    extraSelect = "All Extra Topping";
                }
                else
                {
                    foreach (var item in listChooseItemExtra)
                    {
                        if (extraSelect != "")
                        {
                            extraSelect += "," + item.ItemName;
                        }
                        else
                        {
                            extraSelect = item.ItemName;
                        }
                    }
                }
        }
        private async void ItemClick(int e)
        {
            var item = items[e];

            var search = listChooseItem.FindIndex(x => x.SysItemID == item.SysItemID);
            if (search == -1)
            {
                listChooseItem.Add(item);
            }
            else
            {
                listChooseItem.RemoveAt(search);
            }

            SetListItem();

            itemSelect = "";
            if (items.Count == listChooseItem.Count)
            {
                itemSelect = "All Items";
            }
            else
            {
                foreach (var i in listChooseItem)
                {
                    if (itemSelect != "")
                    {
                        itemSelect += "," + i.ItemName;
                    }
                    else
                    {
                        itemSelect = i.ItemName;
                    }
                }
            }

        }
        private async void CategoryClick(int e)
        {
            try
            {
                var category = listCategory[e];
                var search = listChooseCategory.FindIndex(x => x.SysCategoryID == category.SysCategoryID && x.MerchantID == DataCashingAll.MerchantId);
                if (search == -1)
                {
                    listChooseCategory.Add(category);
                }
                else
                {
                    listChooseCategory.RemoveAt(search);
                }
                SetListCategory();

                categorySelect = "";
                if (listCategory.Count == listChooseCategory.Count)
                {
                    categorySelect = "All Category";
                }
                else
                {
                    foreach (var item in listChooseCategory)
                    {
                        if (categorySelect != "")
                        {
                            categorySelect += "," + item.Name;
                        }
                        else
                        {
                            categorySelect = item.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void SetDataItem()
        {
            switch (MenuSelected)
            {
                case 0:
                    SetListItem();
                    break;
                case 1:
                    SetListExtra();
                    break;
                case 2:
                    SetListCategory();
                    break;
                default:
                    break;
            }
            SetShowButton();
        }
        private async void SetListCategory()
        {
            try
            {
                allitem = new List<Item>();

                List<Item> items = await GetItemList();
                List<Item> itemExtra = await GetExtraList();

                allitem.AddRange(items);
                allitem.AddRange(itemExtra);

                CategoryManage category = new CategoryManage();
                lstCategory = await category.GetAllCategory();
                listCategory = new ListCategory(lstCategory);


                CatagoeyReportDataSource report_adapter_category = new CatagoeyReportDataSource(listCategory, allitem, listChooseCategory);
                CatagoryListCollectionView.DataSource = report_adapter_category;
                CatagoryListCollectionView.ReloadData();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private async void SetListItem()
        {
            try
            {
                items = new List<Item>();
                items = await GetItemList();
                listItem = new ListItem(items);

                ItemStockReportDataSource report_item = new ItemStockReportDataSource(listItem, listChooseItem);
                itemListCollectionView.DataSource = report_item;
                itemListCollectionView.ReloadData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }

        }
        private async void SetListExtra()
        {
            try
            {
                itemExtra = new List<Item>();
                itemExtra = await GetExtraList();
                listExtraItem = new ListItem(itemExtra);

                ItemStockReportDataSource report_Extra = new ItemStockReportDataSource(listExtraItem, listChooseItemExtra);
                ToppingCollectionView.DataSource = report_Extra;
                ToppingCollectionView.ReloadData();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void SetShowButton()
        {
            if (!selcetitemall)
            {
                btnSelectAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelectAll.BackgroundColor = UIColor.White;

            }
            else
            {
                btnSelectAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelectAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);

                btnApply.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnApply.Enabled = true;
                btnApply.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
        }
        async Task<List<Item>> GetItemList()
        {
            try
            {
                items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    return null;
                }

                listItemBodyCount = items.Count();
                return items;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return null;
            }
        }
        async Task<List<Item>> GetExtraList()
        {
            try
            {
                itemExtra = new List<Item>();
                ItemManage itemManage = new ItemManage();
                itemExtra = await itemManage.GetToppingItem();
                if (itemExtra == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    return null;
                }

                listItemBodyCount = itemExtra.Count();
                return itemExtra;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return null;
            }
        }
        #region toggle menu
        [Export("ItemMenu:")]
        public void ItemMenu(UIGestureRecognizer sender)
        {
            lblItemMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            ItemLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblExtraMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            ExtraLineView.BackgroundColor = UIColor.White;

            lblCategoryMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            CategoryLineView.BackgroundColor = UIColor.White;


            MenuSelected = 0;
            showList();
        }
        [Export("ExtraMenu:")]
        public void ExtraMenu(UIGestureRecognizer sender)
        {
            lblItemMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            ItemLineView.BackgroundColor = UIColor.White;

            lblExtraMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            ExtraLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblCategoryMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            CategoryLineView.BackgroundColor = UIColor.White;

            MenuSelected = 1;
            showList();
        }
        [Export("CategoryMenu:")]
        public void CategoryMenu(UIGestureRecognizer sender)
        {
            lblItemMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            ItemLineView.BackgroundColor = UIColor.White;

            lblExtraMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            ExtraLineView.BackgroundColor = UIColor.White;

            lblCategoryMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            CategoryLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            MenuSelected = 2;
            showList();
        }
        #endregion
        async void SearchBytxt()
        {
            if(MenuSelected == 0) // search item
            {
                items = await GetFilterItemList();
                listItem = new ListItem(items);

                ItemStockReportDataSource report_item = new ItemStockReportDataSource(listItem, listChooseItem);
                itemListCollectionView.DataSource = report_item;
                itemListCollectionView.ReloadData();
            }
            else if (MenuSelected == 1) // search topping
            {
                itemExtra = await GetFilterExtraTopping();
                listExtraItem = new ListItem(itemExtra);

                ItemStockReportDataSource report_Extra = new ItemStockReportDataSource(listExtraItem, listChooseItemExtra);
                ToppingCollectionView.DataSource = report_Extra;
                ToppingCollectionView.ReloadData();
            }
            else // search category
            {
                lstCategory = await GetFilterCategory();
                listCategory = new ListCategory(lstCategory);

                allitem = new List<Item>();

                List<Item> items = await GetItemList();
                List<Item> itemExtra = await GetExtraList();

                allitem.AddRange(items);
                allitem.AddRange(itemExtra);


                CatagoeyReportDataSource report_adapter_category = new CatagoeyReportDataSource(listCategory, allitem, listChooseCategory);
                CatagoryListCollectionView.DataSource = report_adapter_category;
                CatagoryListCollectionView.ReloadData();
            }
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
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    return null;
                }
                return itm;
            }
            catch (Exception ex)
            {
               
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
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
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    return null;
                }
                return CategorySerch;
            }
            catch (Exception ex)
            {
                
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
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
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    return null;
                }
                return toppSerch;
            }
            catch (Exception ex)
            {
                
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                return null;
            }
        }
        void showList()
        {
            if (MenuSelected == 0)
            { // item
                ItemView.Hidden = false;
                CategoryView.Hidden = true;
                ToppingView.Hidden = true;
                if (items == null || items.Count == 0)
                {
                    SearchbarView.Hidden = true;
                    itemListCollectionView.Hidden = true;
                    emptyItemView.Hidden = false;
                    lbl_empty_Item.Hidden = false;
                }
                else
                {
                    SearchbarView.Hidden = false;
                    itemListCollectionView.Hidden = false;
                    emptyItemView.Hidden = true;
                    lbl_empty_Item.Hidden = true;
                }
            }
            else if (MenuSelected == 1)
            { //extra 
                ItemView.Hidden = true;
                CategoryView.Hidden = true;
                ToppingView.Hidden = false;
                if (itemExtra == null || itemExtra.Count ==0)
                {
                    SearchbarView.Hidden = true;
                    ToppingCollectionView.Hidden = true;
                    emptyExtraToppingView.Hidden = false;
                    lbl_empty_ExtaTopping.Hidden = false;
                }
                else
                {
                    SearchbarView.Hidden = false;
                    ToppingCollectionView.Hidden = false;
                    emptyExtraToppingView.Hidden = true;
                    lbl_empty_ExtaTopping.Hidden = false;
                }
            }
            else
            { // category
                ItemView.Hidden = true;
                CategoryView.Hidden = false;
                ToppingView.Hidden = true;
                if (lstCategory == null || lstCategory.Count == 0)
                {
                    SearchbarView.Hidden = true;
                    CatagoryListCollectionView.Hidden = true;
                    emptyCategoryView.Hidden = false;
                    lbl_empty_Category.Hidden = false;
                }
                else
                {
                    SearchbarView.Hidden = false;
                    CatagoryListCollectionView.Hidden = false;
                    emptyCategoryView.Hidden = true;
                    lbl_empty_Category.Hidden = true;
                }
            }
        }
        void setupAutoLayout()
        {

            #region MenuView
            MenuView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            MenuView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            MenuView.LeftAnchor.ConstraintEqualTo(MenuView.Superview.LeftAnchor, 0).Active = true;
            MenuView.RightAnchor.ConstraintEqualTo(MenuView.Superview.RightAnchor, 0).Active = true;

            #region ItemMenuView
            ItemMenuView.TopAnchor.ConstraintEqualTo(ItemMenuView.Superview.TopAnchor, 0).Active = true;
            ItemMenuView.BottomAnchor.ConstraintEqualTo(ItemMenuView.Superview.BottomAnchor, 0).Active = true;
            ItemMenuView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
            ItemMenuView.LeftAnchor.ConstraintEqualTo(ItemMenuView.Superview.LeftAnchor, 0).Active = true;

            lblItemMenu.CenterXAnchor.ConstraintEqualTo(lblItemMenu.Superview.CenterXAnchor).Active = true;
            lblItemMenu.CenterYAnchor.ConstraintEqualTo(lblItemMenu.Superview.CenterYAnchor).Active = true;
            lblItemMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            ItemLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            ItemLineView.BottomAnchor.ConstraintEqualTo(ItemMenuView.Superview.BottomAnchor, 0).Active = true;
            ItemLineView.RightAnchor.ConstraintEqualTo(ItemLineView.Superview.RightAnchor).Active = true;
            ItemLineView.LeftAnchor.ConstraintEqualTo(ItemLineView.Superview.LeftAnchor).Active = true;
            #endregion
            #region ExtraMenuView
            ExtraMenuView.TopAnchor.ConstraintEqualTo(ExtraMenuView.Superview.TopAnchor, 0).Active = true;
            ExtraMenuView.BottomAnchor.ConstraintEqualTo(ExtraMenuView.Superview.BottomAnchor, 0).Active = true;
            ExtraMenuView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
            ExtraMenuView.LeftAnchor.ConstraintEqualTo(ItemMenuView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblExtraMenu.CenterXAnchor.ConstraintEqualTo(lblExtraMenu.Superview.CenterXAnchor).Active = true;
            lblExtraMenu.CenterYAnchor.ConstraintEqualTo(lblExtraMenu.Superview.CenterYAnchor).Active = true;
            lblExtraMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            ExtraLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            ExtraLineView.BottomAnchor.ConstraintEqualTo(ExtraLineView.Superview.BottomAnchor, 0).Active = true;
            ExtraLineView.RightAnchor.ConstraintEqualTo(ExtraLineView.Superview.RightAnchor).Active = true;
            ExtraLineView.LeftAnchor.ConstraintEqualTo(ExtraLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #region CategoryMenuView
            CategoryMenuView.TopAnchor.ConstraintEqualTo(CategoryMenuView.Superview.TopAnchor, 0).Active = true;
            CategoryMenuView.BottomAnchor.ConstraintEqualTo(CategoryMenuView.Superview.BottomAnchor, 0).Active = true;
            CategoryMenuView.RightAnchor.ConstraintEqualTo(CategoryMenuView.Superview.RightAnchor).Active = true;
            CategoryMenuView.LeftAnchor.ConstraintEqualTo(ExtraLineView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblCategoryMenu.CenterXAnchor.ConstraintEqualTo(lblCategoryMenu.Superview.CenterXAnchor).Active = true;
            lblCategoryMenu.CenterYAnchor.ConstraintEqualTo(lblCategoryMenu.Superview.CenterYAnchor).Active = true;
            lblCategoryMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            CategoryLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            CategoryLineView.BottomAnchor.ConstraintEqualTo(CategoryLineView.Superview.BottomAnchor, 0).Active = true;
            CategoryLineView.RightAnchor.ConstraintEqualTo(CategoryLineView.Superview.RightAnchor).Active = true;
            CategoryLineView.LeftAnchor.ConstraintEqualTo(CategoryLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #endregion

            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(MenuView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(btnSelectAll.SafeAreaLayoutGuide.LeftAnchor,-15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            btnSelectAll.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSelectAll.WidthAnchor.ConstraintEqualTo(40).Active = true;
            btnSelectAll.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectAll.HeightAnchor.ConstraintEqualTo(30).Active = true;
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
            itemListCollectionView.BottomAnchor.ConstraintEqualTo(btnApply.SafeAreaLayoutGuide.TopAnchor,-10).Active = true;
            itemListCollectionView.RightAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            itemListCollectionView.LeftAnchor.ConstraintEqualTo(ItemView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
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
            CatagoryListCollectionView.BottomAnchor.ConstraintEqualTo(btnApply.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            CatagoryListCollectionView.RightAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CatagoryListCollectionView.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
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
            ToppingCollectionView.BottomAnchor.ConstraintEqualTo(btnApply.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            ToppingCollectionView.RightAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ToppingCollectionView.LeftAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            #endregion

            btnApply.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnApply.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnApply.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnApply.HeightAnchor.ConstraintEqualTo(45).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        
    }
}