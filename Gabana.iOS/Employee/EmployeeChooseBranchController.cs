using AutoMapper;
using Foundation;
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
    
    public partial class EmployeeChooseBranchController : UIViewController
    {
        UICollectionView branchCollectionView;

        public Gabana3.JAM.Merchant.Merchants merchant;
        UIView bottomView;
        UIButton btnSelect;
        BranchManage setBranch = new BranchManage();
        List<Branch> branch = new List<Branch>();
        public static int Select;
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch;
        

        public EmployeeChooseBranchController(Gabana3.JAM.Merchant.Merchants merchant)
        {
            this.merchant = merchant;
        }
        public EmployeeChooseBranchController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            try
            {
                base.ViewWillAppear(animated);
                if (branchCollectionView != null)
                {
               // Select = 0;
                    branch = await setBranch.GetAllBranch((int)MainController.merchantlocal.MerchantID);
                    if(UpdateEmployeeController.isModifyBranch)
                    {
                        var SelectBranch = branch.FindIndex(x => x.BranchName == UpdateEmployeeController.BranchSelect);
                        Select = SelectBranch;
                    }
                    branchCollectionView.DataSource = new EmployeeChooseBranchDataSource(branch);
                    ((EmployeeChooseBranchDataSource)branchCollectionView.DataSource).ReloadData(branch);
                    branchCollectionView.ReloadData();
                }
            }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public async override void ViewDidLoad()
        {
            // this.NavigationController.NavigationBar.TopItem.Title = "Choose Branch";
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Choose Branch"));
            View.BackgroundColor = UIColor.White;
            try
            {
                base.ViewDidLoad();

                #region SearchbarView
                SearchbarView = new UIView();
                SearchbarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
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

                #endregion
                #region BranchCollectionview
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

                branchCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                branchCollectionView.BackgroundColor = UIColor.White;
                branchCollectionView.ShowsVerticalScrollIndicator = false;
                branchCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
                branchCollectionView.RegisterClassForCell(cellType: typeof(EmployeeChooseBranchViewCell), reuseIdentifier: "EmployeeChooseBranchViewCell");


                branch =  await setBranch.GetAllBranch((int)MainController.merchantlocal.MerchantID);


                EmployeeChooseBranchDataSource BranchDataList = new EmployeeChooseBranchDataSource(branch);
                branchCollectionView.DataSource = BranchDataList;
                EmployeeChooseBranchCollectionDelegate branchCollectionDelegate = new EmployeeChooseBranchCollectionDelegate();
                branchCollectionDelegate.OnItemSelected += (indexPath) => {
                    if (UpdateEmployeeController.RoleSelected == "Manager")
                    {
                        if (UpdateEmployeeController.listChooseBranch.Any(x=>x.SysBranchID == branch[indexPath.Row].SysBranchID))
                        {
                            var row = UpdateEmployeeController.listChooseBranch.Where(x=>x.SysBranchID== branch[indexPath.Row].SysBranchID).FirstOrDefault();
                            UpdateEmployeeController.listChooseBranch.Remove(row);
                        }
                        else
                        {
                            UpdateEmployeeController.listChooseBranch.Add(new BranchPolicy() { SysBranchID = branch[indexPath.Row].SysBranchID });
                        }
                        
                    }
                    else
                    {
                        UpdateEmployeeController.listChooseBranch = new List<BranchPolicy>();
                        UpdateEmployeeController.listChooseBranch.Add(new BranchPolicy() { SysBranchID = branch[indexPath.Row].SysBranchID });
                    }
                    branchCollectionView.DataSource = new EmployeeChooseBranchDataSource(branch);
                    ((EmployeeChooseBranchDataSource)branchCollectionView.DataSource).ReloadData(branch);
                    branchCollectionView.ReloadData();
                    btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    
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
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSelect.BackgroundColor = UIColor.White;
                btnSelect.Layer.CornerRadius = 5f;
                btnSelect.Layer.BorderWidth = 0.5f;
                btnSelect.Enabled = true;
                btnSelect.SetTitle(Utils.TextBundle("applybranch", "Apply Branch"), UIControlState.Normal);
                btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
                btnSelect.TouchUpInside += (sender, e) => {
                    //UpdateEmployeeController.BranchSelect = branch[Select].BranchName;
                    UpdateEmployeeController.isModifyBranch = true;
                    this.NavigationController.PopViewController(false);
                };
                View.AddSubview(btnSelect);
                #endregion
                SetupAutoLayout();
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        async void SearchBytxt()
        {
            branch = await GetFilterBranchList();
            branchCollectionView.DataSource = new EmployeeChooseBranchDataSource(branch);
            ((EmployeeChooseBranchDataSource)branchCollectionView.DataSource).ReloadData(branch);
        }
        async Task<List<Branch>> GetFilterBranchList()
        {
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await setBranch.GetAllBranch(merchant.Merchant.MerchantID);
                    return itemlst;
                }
                var itm = await setBranch.GetBranchSearch(merchant.Merchant.MerchantID,txtSearch.Text);
                if (itm == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Unable to load data."));
                    return null;
                }
                return itm;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Unable to load data."));
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
            txtSearch.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
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