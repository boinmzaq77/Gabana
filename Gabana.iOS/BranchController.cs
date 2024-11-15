using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class BranchController : UIViewController
    {
        UICollectionView branchCollectionView;

        public Gabana3.JAM.Merchant.Merchants merchant;
        UIView bottomView;
        UIButton btnSelect;
        string LoginType, Username;
        List<ChooseBranch> choosebranch = new List<ChooseBranch>();
        string branchSelect;
        public BranchController(Gabana3.JAM.Merchant.Merchants merchant)
        {
            this.merchant = merchant;
        }
        public BranchController() {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public async override void ViewDidLoad()
        {
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

            }
            else
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
            }
            //this.NavigationController.NavigationBar.Translucent = true;
            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
            //this.NavigationController.NavigationBar.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            this.NavigationController.NavigationBar.TopItem.Title = Utils.TextBundle("choosebranch", "Choose Branch");
            //this.NavigationController.NavigationBar.TintColor = UIColor.White;
            View.BackgroundColor = UIColor.White;
            //this.NavigationController.SetNavigationBarHidden(false, false);
            //this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            //{
            //    ForegroundColor = UIColor.White
            //    //BackgroundColor = UIColor.FromRGB(51, 170, 225)
            //};

            base.ViewDidLoad();

            #region BranchCollectionview
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            branchCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            branchCollectionView.BackgroundColor = UIColor.White;
            branchCollectionView.ShowsVerticalScrollIndicator = false;
            branchCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            branchCollectionView.RegisterClassForCell(cellType: typeof(BranchCollectionViewCell), reuseIdentifier: "BranchViewCell");

            //BranchManage setBranch = new BranchManage();
            //var branch =  await setBranch.GetBranch(merchant.Merchant.MerchantID);

            BranchManage branchManage = new BranchManage();
            LoginType = Preferences.Get("LoginType", "");
            Username = Preferences.Get("User", "");
            var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
            {
                //var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            }
            else
            {
                List<Branch> getbranch = new List<Branch>();
               
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
                    lstBranch = new List<Branch>();
                    lstBranch.AddRange(getbranch);
                }
                else
                {
                    lstBranch = new List<Branch>();
                }            
            }
            
            //listBranch = new ListBranch(lstBranch);

            var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<Branch, ChooseBranch>());
            var mapper = mapperConfiguration.CreateMapper();
            choosebranch = mapper.Map<List<Branch>, List<ChooseBranch>>(lstBranch);
            branchSelect = Preferences.Get("Branch", "");
            choosebranch.ConvertAll(x => x.Choose = x.BranchID == branchSelect ? true : false  );


            BranchDataSource BranchDataList = new BranchDataSource(choosebranch);
            branchCollectionView.DataSource = BranchDataList;
            BranchmainCollectionDelegate branchCollectionDelegate = new BranchmainCollectionDelegate();
            branchCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                choosebranch.ConvertAll(x => x.Choose =  false);
                choosebranch[indexPath.Row].Choose = true; 
                ((BranchDataSource)branchCollectionView.DataSource).ReloadData(choosebranch);
                branchCollectionView.ReloadData();
                branchSelect = choosebranch[indexPath.Row].BranchID;

                DataCashingAll.SysBranchId = Convert.ToInt32(branchSelect);
                Preferences.Set("Branch", branchSelect);
                DataCaching.branchDeatail = choosebranch[indexPath.Row] as ORM.MerchantDB.Branch;
            };
            branchCollectionView.Delegate = branchCollectionDelegate;
            View.AddSubview(branchCollectionView);
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
            btnSelect.SetTitle(Utils.TextBundle("gotomain", "Go To Main"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                Selectbranch();
            };
            View.AddSubview(btnSelect);
            #endregion
            SetupAutoLayout();
        }

        private void Selectbranch()
        {
            if (branchSelect != string.Empty)
            {
                var choose = choosebranch.Where(x => x.Choose == true).FirstOrDefault();
                Preferences.Set("branch", choose.BranchID.ToString());
                this.NavigationController.DismissViewController(false, null);
            }
        }
        void SetupAutoLayout()
        {
            branchCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
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
            // Release any cached data, images, etc that aren't in use.
        }
    }
    public class ChooseBranch : ORM.MerchantDB.Branch
    {
        public bool Choose { get; set; }
    }
}