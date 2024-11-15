using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
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

    public partial class ReportSelectCategoryController : UIViewController
    {
        UICollectionView CategoryCollectionView;

        public Gabana3.JAM.Merchant.Merchants merchant;
        UIView bottomView;
        UIButton btnSelect;
        CategoryManage setCate = new CategoryManage();
        List<Category> lstCategory = new List<Category>();
        public static List<Category> listChooseCategory = new List<Category>();
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch, btnAll;
        private string categorySelect ="";


        int catcount = 0;
        ListCategory listCategory;

        List<Item> allitem;
        private static List<Item> items, itemExtra;

        public ReportSelectCategoryController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Items"));
        }
        public async override void ViewDidLoad()
        {
            // this.NavigationController.NavigationBar.TopItem.Title = "Choose Branch";
            View.BackgroundColor = UIColor.White;

            base.ViewDidLoad();

            initAttribute();
            SetupAutoLayout();
            SetCategoryData();
        }
        async void SetCategoryData()
        {
            allitem = new List<Item>();

            List<Item> items = await GetItemList();
            List<Item> itemExtra = await GetExtraList();

            allitem.AddRange(items);
            allitem.AddRange(itemExtra);

            CategoryManage category = new CategoryManage();
            lstCategory = await category.GetAllCategory();
            listCategory = new ListCategory(lstCategory);

            catcount = lstCategory.Count;
            listChooseCategory = CategoryReportController.listChooseCategory;
            if (CategoryReportController.listChooseCategory.Count == catcount)
            {
                categorySelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                categorySelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.White;
            }
           

            CategoryDataSource CategoryDataList = new CategoryDataSource(listCategory, allitem, listChooseCategory);
            CategoryCollectionView.DataSource = CategoryDataList;
        }

        

        //
        async Task<List<Item>> GetItemList()
        {
            try
            {
                items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    return null;
                }
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
                    return null;
                }

                return itemExtra;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return null;
            }
        }
        void initAttribute()
        {
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

            btnAll = new UIButton();
            btnAll.SetTitle(Utils.TextBundle("all", "Items"), UIControlState.Normal);
            btnAll.TitleLabel.Font = UIFont.SystemFontOfSize(15);
            btnAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAll.Layer.BorderWidth = 1;
            btnAll.ClipsToBounds = true;
            btnAll.Layer.CornerRadius = 5;
            btnAll.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAll.TouchUpInside += (sender, e) =>
            {
                //selectAllCategory
                if (categorySelect != Utils.TextBundle("all", "Items") && categorySelect == "" )
                {
                    categorySelect = Utils.TextBundle("all", "Items");
                    listChooseCategory = new List<Category>();
                    listChooseCategory = lstCategory;

                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    listChooseCategory = new List<Category>();
                    categorySelect = "";

                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                }

                listCategory = new ListCategory(lstCategory);

                ((CategoryDataSource)CategoryCollectionView.DataSource).ReloadData(listCategory, allitem, listChooseCategory);
                CategoryCollectionView.ReloadData();

            };
            SearchbarView.AddSubview(btnAll);

            #endregion

            #region CategoryCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            CategoryCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CategoryCollectionView.BackgroundColor = UIColor.White;
            CategoryCollectionView.ShowsVerticalScrollIndicator = false;
            CategoryCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CategoryCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseCategoryViewCell), reuseIdentifier: "ReportChooseCategoryViewCell");
            View.AddSubview(CategoryCollectionView);

            ReportCategoryCollectionDelegate branchCollectionDelegate = new ReportCategoryCollectionDelegate();
            branchCollectionDelegate.OnItemSelected += async (indexPath) => {
                var category = listCategory[(int)indexPath.Row];
                var search = listChooseCategory.FindIndex(x => x.SysCategoryID == category.SysCategoryID && x.MerchantID == (int)MainController.merchantlocal.MerchantID);
                if (search == -1)
                {
                    listChooseCategory.Add(category);
 
                }
                else
                {
                    listChooseCategory.RemoveAt(search);
                    //if(categorySelect == "ALL")
                    //{

                    //}
                }

                categorySelect = "";

                if (catcount == listChooseCategory.Count)
                {
                    categorySelect = Utils.TextBundle("all", "Items");
                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    categorySelect = "";
                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
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
                CategoryManage categoryManage = new CategoryManage();
                lstCategory = await categoryManage.GetAllCategory();
                listCategory = new ListCategory(lstCategory);

                 ((CategoryDataSource)CategoryCollectionView.DataSource).ReloadData(listCategory, allitem, listChooseCategory);
                CategoryCollectionView.ReloadData();
            };
            CategoryCollectionView.Delegate = branchCollectionDelegate;
            
            #endregion

            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.White;
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = true;
            btnSelect.SetTitle(Utils.TextBundle("applycategory", "Apply Category"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                CategoryReportController.listChooseCategory = listChooseCategory;
                CategoryReportController.isModifyCategory = true;
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        async void SearchBytxt()
        {
            var list = await GetFilterCategoryList();

            if (list != null)
            {
                listCategory= new ListCategory(list);
            }
            ((CategoryDataSource)CategoryCollectionView.DataSource).ReloadData(listCategory, allitem, listChooseCategory);
            CategoryCollectionView.ReloadData();
        }
        async Task<List<Category>> GetFilterCategoryList()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await setCate.GetCategorySearch(txtSearch.Text);
                    return itemlst;
                }
                var itm = await setCate.GetAllCategory();
                if (itm == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
                    return null;
                }
                return itm;
            }
            catch (Exception ex)
            {

                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
                return null;
            }
        }
        void SetupAutoLayout()
        {
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(btnAll.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.HeightAnchor.ConstraintEqualTo(36).Active = true;

            btnAll.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnAll.WidthAnchor.ConstraintEqualTo(38).Active = true;
            btnAll.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnAll.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion

            CategoryCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CategoryCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            CategoryCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CategoryCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSelect.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSelect.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelect.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelect.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}