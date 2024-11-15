using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Gabana.iOS
{
    public partial class itemDiscountController : UIViewController
    {
        static List<DiscountTemplate> Discounts;
        UIView SearchbarView;
        UIButton btnSearch, btnAddDiscount;
        UICollectionView DiscountListCollectionView;
        public itemDiscountController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Discounts = await getDiscountItem();
            DiscountListCollectionView.DataSource = new ItemDiscountDataSourceList(Discounts);
            ((ItemDiscountDataSourceList)DiscountListCollectionView.DataSource).ReloadData(Discounts);
            DiscountListCollectionView.ReloadData();
        }
        public async override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.Title = Utils.TextBundle("discount", "Discount");
            View.BackgroundColor = UIColor.White;
            base.ViewDidLoad();

            initAttribute();
            SetupAutoLayout();
        }
        private async Task<List<DiscountTemplate>> getDiscountItem()
        {
            DiscountTemplateManage DiscountManager = new DiscountTemplateManage();
            var Dis = new List<DiscountTemplate>();
            Dis = await DiscountManager.GetAllDiscountTemplate();
            return Dis;
        }
        async void initAttribute()
        {

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                //search
            };
            SearchbarView.AddSubview(btnSearch);

            #endregion

            #region DiscountListCollectionView
            UICollectionViewFlowLayout DiscountLayoutList = new UICollectionViewFlowLayout();
            DiscountLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: (int)View.Frame.Height * 9 / 100);
            DiscountLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            Discounts = await getDiscountItem();
            DiscountListCollectionView = new UICollectionView(frame: View.Frame, layout: DiscountLayoutList);
            DiscountListCollectionView.BackgroundColor = UIColor.White;
            DiscountListCollectionView.ShowsVerticalScrollIndicator = false;
            DiscountListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            DiscountListCollectionView.RegisterClassForCell(cellType: typeof(itemDiscountCollectionViewCellList), reuseIdentifier: "itemDiscountCellList");
            ItemDiscountDataSourceList DiscountDataList = new ItemDiscountDataSourceList(Discounts);
            ItemDiscountCollectionDelegate itemDisCollectionDelegate = new ItemDiscountCollectionDelegate();
            itemDisCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var x = (int)indexPath.Item;
                ItemsAddDiscountController addDiscount = new ItemsAddDiscountController(Discounts[x]);
                this.NavigationController.PushViewController(addDiscount, false);
            };
            DiscountListCollectionView.Delegate = itemDisCollectionDelegate;
            DiscountListCollectionView.DataSource = DiscountDataList;
            View.AddSubview(DiscountListCollectionView);
            #endregion
            #region GoToAddDiscount
            btnAddDiscount = new UIButton();
            btnAddDiscount.Layer.CornerRadius = 25;
            btnAddDiscount.Font = btnAddDiscount.Font.WithSize(14);
            btnAddDiscount.SetTitle(Utils.TextBundle("adddiscount", "Add Discount"), UIControlState.Normal);
            btnAddDiscount.BackgroundColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1);
            btnAddDiscount.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddDiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddDiscount.TouchUpInside += (sender, e) => {
                // go to add Discount page
                ItemsAddDiscountController itemsAddDiscount = new ItemsAddDiscountController();
                this.NavigationController.PushViewController(itemsAddDiscount, false);
            };
            View.AddSubview(btnAddDiscount);
            #endregion
        }
        void SetupAutoLayout()
        {
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 59) / 1000).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
            #endregion

            #region DiscountLayout
            DiscountListCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DiscountListCollectionView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DiscountListCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DiscountListCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddDiscount.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -((int)View.Frame.Height * 29) / 1000).Active = true;
            btnAddDiscount.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 29) / 100).Active = true;
            btnAddDiscount.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;
            btnAddDiscount.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion
        }
    }
}