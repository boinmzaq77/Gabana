using AutoMapper;
using Foundation;
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

    public partial class ReportBranchController : UIViewController
    {
        UICollectionView branchCollectionView;

        public Gabana3.JAM.Merchant.Merchants merchant;
        UIView bottomView;
        UIButton btnSelect , btnAll;
        string LoginType, Username;
        BranchManage setBranch = new BranchManage();
        List<Gabana.ORM.MerchantDB.Branch> lstBranch;
        int Select;
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch;

        List<Branch> choosebranch = new List<Branch>();
        public static int branchSelect;


        public ReportBranchController(Gabana3.JAM.Merchant.Merchants merchant)
        {
            this.merchant = merchant;
        }
        public ReportBranchController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Choose Branch"));
        }
        public async override void ViewDidLoad()
        {
            // this.NavigationController.NavigationBar.TopItem.Title = "Choose Branch";
            View.BackgroundColor = UIColor.White;
            LoginType = Preferences.Get("LoginType", "");
            try
            {
                base.ViewDidLoad();
                #region BottomView
                bottomView = new UIView();
                bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                bottomView.BackgroundColor = UIColor.White;
                View.AddSubview(bottomView);

                btnSelect = new UIButton();
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSelect.BackgroundColor = UIColor.White;
                btnSelect.Layer.CornerRadius = 5f;
                btnSelect.Layer.BorderWidth = 0.5f;
                btnSelect.Enabled = true;
                btnSelect.SetTitle(Utils.TextBundle("applybranch", "Items"), UIControlState.Normal);
                btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSelect.TouchUpInside += (sender, e) => {
                   
                    this.NavigationController.PopViewController(false);
                };
                View.AddSubview(btnSelect);
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

                btnAll = new UIButton();
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAll.BackgroundColor = UIColor.White;
                btnAll.Layer.CornerRadius = 5f;
                btnAll.Layer.BorderWidth = 0.5f;
                btnAll.SetTitle(Utils.TextBundle("all", "All"), UIControlState.Normal);
                //btnAll.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                btnAll.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAll.TouchUpInside += (sender, e) =>
                {
                    if (ReportController.listChooseBranch.Count != choosebranch.Count)
                    {
                        ReportController.listChooseBranch = new List<Branch>();
                        foreach (var item in choosebranch)
                        {
                            ReportController.listChooseBranch.Add(item);
                        }
                    }
                    else
                    {
                        ReportController.listChooseBranch = new List<Branch>();
                    }
                    branchCollectionView.DataSource = new ReportChooseBranchDataSource(choosebranch);
                    ((ReportChooseBranchDataSource)branchCollectionView.DataSource).ReloadData(choosebranch);
                    branchCollectionView.ReloadData();
                    Checkbtnall();
                };
                SearchbarView.AddSubview(btnAll);

                #endregion

                #region BranchCollectionview
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80) ;
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

                branchCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                branchCollectionView.BackgroundColor = UIColor.White;
                branchCollectionView.ShowsVerticalScrollIndicator = false;
                branchCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
                branchCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseBranchViewCell), reuseIdentifier: "ReportChooseBranchViewCell");

                //var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<ORM.Master.Branch, Branch>());
                //var mapper = mapperConfiguration.CreateMapper();
                //choosebranch = mapper.Map<List<ORM.Master.Branch>, List<Branch>>(MainController.merchant.Branch);
                //branchSelect = Convert.ToInt32(Preferences.Get("Branch", ""));
                ////choosebranch.ConvertAll(x => x.Choose = x.SysBranchID == branchSelect ? true : false);

                await GetListBranch();

                ReportChooseBranchDataSource BranchDataList = new ReportChooseBranchDataSource(choosebranch);
                branchCollectionView.DataSource = BranchDataList;
                ReportChooseBranchCollectionDelegate branchCollectionDelegate = new ReportChooseBranchCollectionDelegate();
                branchCollectionDelegate.OnItemSelected += (indexPath) => {

                    if (ReportController.listChooseBranch.Any(x => x.SysBranchID == choosebranch[indexPath.Row].SysBranchID))
                    {
                        var row = ReportController.listChooseBranch.Where(x => x.SysBranchID == choosebranch[indexPath.Row].SysBranchID).FirstOrDefault();
                        ReportController.listChooseBranch.Remove(row);
                    }
                    else
                    {
                        ReportController.listChooseBranch.Add(choosebranch[indexPath.Row]);
                    }
                    
                    ((ReportChooseBranchDataSource)branchCollectionView.DataSource).ReloadData(choosebranch);
                    branchCollectionView.ReloadData();
                    btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    Checkbtnall();
                };
                branchCollectionView.Delegate = branchCollectionDelegate;
                View.AddSubview(branchCollectionView);
                #endregion
               
                SetupAutoLayout();
                Checkbtnall();
                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                //throw;
            }

          
        }

        private void Checkbtnall()
        {
            if (ReportController.listChooseBranch.Count != choosebranch.Count)
            {
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAll.BackgroundColor = UIColor.White;
            }
            else
            {
                btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            if (ReportController.listChooseBranch.Count == 0 )
            {
                btnSelect.Enabled = false;
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSelect.BackgroundColor = UIColor.White;
            }
            else
            {
                btnSelect.Enabled = true;
                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                
            }
        }

        private async Task GetListBranch()
        {
            try
            {
                List<Gabana.ORM.MerchantDB.Branch> getbranch = new List<Gabana.ORM.MerchantDB.Branch>();
                BranchManage branchManage = new BranchManage();
                BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
                Username = Preferences.Get("User", "");

                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    choosebranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                }
                else
                {
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
                    choosebranch = lstBranch;
                }

                ReportChooseBranchDataSource data = new ReportChooseBranchDataSource(choosebranch);
                branchCollectionView.DataSource = data;
                branchCollectionView.ReloadData();

                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
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

                    ReportChooseBranchDataSource data = new ReportChooseBranchDataSource(lstBranch);
                    branchCollectionView.DataSource = data;
                    branchCollectionView.ReloadData();
                    return;
                }
                lstBranch = await setBranch.GetBranchSearch(DataCashingAll.MerchantId, txtSearch.Text);

                ReportChooseBranchDataSource data2 = new ReportChooseBranchDataSource(lstBranch);
                branchCollectionView.DataSource = data2;
                branchCollectionView.ReloadData();

                if (lstBranch == null || lstBranch.Count == 0)
                {
                    Utils.ShowMessage(Utils.TextBundle("notdata", "ไม่พบข้อมูล"));
                    return;
                }


            }
            catch (Exception ex)
            {

                Utils.ShowMessage(Utils.TextBundle("notdata", "ไม่พบข้อมูล"));
            }

        }
        void SetupAutoLayout()
        {
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            


            btnAll.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnAll.WidthAnchor.ConstraintEqualTo(38).Active = true;
            btnAll.HeightAnchor.ConstraintEqualTo(30).Active = true;
            btnAll.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(btnAll.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
            //btnAll.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
            #endregion

            branchCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            branchCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
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
}