using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ChangeBranchController : UIViewController
    {
        UICollectionView branchCollectionView;

        public Gabana3.JAM.Merchant.Merchants merchant;
        List<ORM.MerchantDB.Branch> lstBranch;
        UIView bottomView, SearchbarView;
        UITextField txtSearch;
        UIButton btnSelect, btnSearch;
        BranchManage setBranch = new BranchManage();
        string LoginType, Username;
        List<ChooseBranch> choosebranch = new List<ChooseBranch>();
        public static int branchSelect;
        ChooseBranch SelectBranch = new ChooseBranch();

        public ChangeBranchController(Gabana3.JAM.Merchant.Merchants merchant)
        {
            this.merchant = merchant;
        }
        public ChangeBranchController() {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            View.BackgroundColor = UIColor.White;
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            {

                var navBarAppearance = new UINavigationBarAppearance();
                navBarAppearance.ConfigureWithOpaqueBackground();
                navBarAppearance.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                navBarAppearance.LargeTitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                navBarAppearance.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.StandardAppearance = navBarAppearance;
                this.NavigationController.NavigationBar.ScrollEdgeAppearance = navBarAppearance;
                this.NavigationController.NavigationBar.TintColor = UIColor.White;
            }
            else
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                this.NavigationController.NavigationBar.TintColor = UIColor.White;
            }
        }

        public async override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                branchSelect = Convert.ToInt32(Preferences.Get("Branch", ""));
                LoginType = Preferences.Get("LoginType", "");
                Username = Preferences.Get("User", "");

                initAttribute();
                SetupAutoLayout();

                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    GetListBranch();
                }
                else
                {
                    GetListBranchbyMainRole();
                }
            }
            catch (Exception ex)
            {
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);

            }
           
        }
        private async void GetListBranchbyMainRole()
        {
            List<Gabana.ORM.MerchantDB.Branch> getbranch = new List<Gabana.ORM.MerchantDB.Branch>();
            BranchManage branchManage = new BranchManage();
            BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
            var lstuserBranch = await branchPolicyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Username);
            if (lstuserBranch != null)
            {
                foreach (var item in lstuserBranch)
                {
                    var Branch = await branchManage.GetBranch(DataCashingAll.MerchantId, (int)item.SysBranchID);
                    if (Branch != null)
                    {
                        getbranch.Add(Branch);
                    }
                }
                lstBranch = new List<Gabana.ORM.MerchantDB.Branch>();
                lstBranch.AddRange(getbranch);
            }
            else
            {
                lstBranch = new List<Gabana.ORM.MerchantDB.Branch>();
            }

            var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<ORM.MerchantDB.Branch, ChooseBranch>());
            var mapper = mapperConfiguration.CreateMapper();

            choosebranch = mapper.Map<List<ORM.MerchantDB.Branch>, List<ChooseBranch>>(lstBranch);

            ChangeBranchDatasource BranchDataList = new ChangeBranchDatasource(choosebranch);
            branchCollectionView.DataSource = BranchDataList;
            branchCollectionView.ReloadData();
        }
        private async void GetListBranch()
        {
            var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<ORM.MerchantDB.Branch, ChooseBranch>());
            var mapper = mapperConfiguration.CreateMapper();

            BranchManage branchManage = new BranchManage();
            lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);

            choosebranch = mapper.Map<List<ORM.MerchantDB.Branch>, List<ChooseBranch>>(lstBranch);
            branchSelect = Convert.ToInt32(Preferences.Get("Branch", ""));
            choosebranch.ConvertAll(x => x.Choose = x.SysBranchID == branchSelect ? true : false);

            ChangeBranchDatasource data = new ChangeBranchDatasource(choosebranch);
            branchCollectionView.DataSource = data;
            branchCollectionView.ReloadData();

            btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSelect.BackgroundColor = UIColor.FromRGB(51,170,225);
        }
        async void initAttribute()
        {
            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

            txtSearch = new UITextField
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
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
            #endregion

            #region BranchCollectionview
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumLineSpacing = 0;

            branchCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            branchCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            branchCollectionView.ShowsVerticalScrollIndicator = false;
            branchCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            branchCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseBranchViewCell), reuseIdentifier: "ReportChooseBranchViewCell");
            BranchCollectionDelegate branchCollectionDelegate = new BranchCollectionDelegate();
            branchCollectionDelegate.OnItemSelected += (indexPath) => {
                SelectBranch = choosebranch[(int)indexPath.Row];
                branchSelect = (int)choosebranch[(int)indexPath.Row].SysBranchID;
                branchCollectionView.DataSource = new ChangeBranchDatasource(choosebranch);
                ((ChangeBranchDatasource)branchCollectionView.DataSource).ReloadData(choosebranch);
                branchCollectionView.ReloadData();
            };
            branchCollectionView.Delegate = branchCollectionDelegate;
            View.AddSubview(branchCollectionView);
           
            #endregion

            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor(UIColor.FromRGB(51,170,225), UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(51, 170, 225).CGColor;
            btnSelect.BackgroundColor = UIColor.White;
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = true;
            btnSelect.SetTitle("Save", UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                Selectbranch();
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        async void SearchBytxt()
        {
            try
            {

                var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<ORM.MerchantDB.Branch, ChooseBranch>());
                var mapper = mapperConfiguration.CreateMapper();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    lstBranch = await setBranch.GetAllBranch(DataCashingAll.MerchantId);

                    choosebranch = mapper.Map<List<ORM.MerchantDB.Branch>, List<ChooseBranch>>(lstBranch);
                    choosebranch.ConvertAll(x => x.Choose = x.SysBranchID == branchSelect ? true : false);

                    ChangeBranchDatasource data = new ChangeBranchDatasource(choosebranch);
                    branchCollectionView.DataSource = data;
                    branchCollectionView.ReloadData();
                    return;
                }
                lstBranch = await setBranch.GetBranchSearch(DataCashingAll.MerchantId, txtSearch.Text);

                choosebranch = mapper.Map<List<ORM.MerchantDB.Branch>, List<ChooseBranch>>(lstBranch);
                choosebranch.ConvertAll(x => x.Choose = x.SysBranchID == branchSelect ? true : false);

                ChangeBranchDatasource data2 = new ChangeBranchDatasource(choosebranch);
                branchCollectionView.DataSource = data2;
                branchCollectionView.ReloadData();

                if (lstBranch == null || lstBranch.Count == 0)
                {
                    Utils.ShowMessage("ไม่พบข้อมูล");
                    return;
                }

                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage("ไม่พบข้อมูล");
            }
            
        }
        private void Selectbranch()
        {
            try
            {
                if (branchSelect != null)
                {
                    POSController.tranWithDetails = null;
                    POSController.SelectedCustomer = null;
                    
                    DataCashingAll.SysBranchId = branchSelect;

                    Preferences.Set("Branch", branchSelect);
                    DataCaching.branchDeatail = SelectBranch;
                    choosebranch.ConvertAll(x => x.Choose = x.SysBranchID == branchSelect ? true : false);

                    var name = choosebranch.Where(x => x.SysBranchID == branchSelect).FirstOrDefault();

                    SplashLoadingController SplashLoading = new SplashLoadingController();
                    UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                    uIWindowRoot.RootViewController = SplashLoading;
                    Utils.ShowMessage("เปลี่ยนสาขาเป็น " + name.BranchName + " เรียบร้อย!");
                }
                else
                {
                    Utils.ShowMessage("กรุณาเลือกสาขา");
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }

        }
        void SetupAutoLayout()
        {
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            #endregion

            branchCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            branchCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            branchCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            branchCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

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
        }
    }
}