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

    public partial class ReportSelectEmployeeController : UIViewController
    {
        UICollectionView CustomerCollectionView;

        UIView bottomView;
        UIButton btnSelect;
        UserAccountInfoManage setEmp = new UserAccountInfoManage();
        List<ORM.MerchantDB.UserAccountInfo> lstEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        public static List<ORM.MerchantDB.UserAccountInfo> listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch, btnAll;
        private string EmployeeSelect ="";

        ORM.MerchantDB.UserAccountInfo Employee = new ORM.MerchantDB.UserAccountInfo();


        int catcount = 0;
        private static ListEmployee listEmployee;



        public ReportSelectEmployeeController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Items"));
            base.ViewWillAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            // this.NavigationController.NavigationBar.TopItem.Title = "Choose Branch";
            try
            {
                View.BackgroundColor = UIColor.White;

                base.ViewDidLoad();

                initAttribute();
                SetupAutoLayout();
                SetEmployeeData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        async void SetEmployeeData()
        {
            lstEmployee = await GetListEmployee();
            listEmployee = new ListEmployee(lstEmployee);
            listChooseEmployee = EmployeeReportController.listChooseEmployee;

            catcount = lstEmployee.Count;
            if (EmployeeReportController.listChooseEmployee.Count == catcount)
            {
                EmployeeSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                EmployeeSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.White;
            }
            

            EmployeeReportSelectDataSource empDataList = new EmployeeReportSelectDataSource(listEmployee);
            CustomerCollectionView.DataSource = empDataList;
        }
        async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        {
            try
            {
                lstEmployee = await setEmp.GetAllUserAccount();
                if (lstEmployee == null)
                {
                    return null;
                }
                return lstEmployee;
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
                if (EmployeeSelect != Utils.TextBundle("all", "Items") && EmployeeSelect == "" )
                {
                    EmployeeSelect = Utils.TextBundle("all", "Items");
                    listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                    listChooseEmployee = lstEmployee;

                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                    EmployeeSelect = "";

                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                }
                listEmployee = new ListEmployee(lstEmployee);

                EmployeeReportSelectDataSource report_adapter_customer = new EmployeeReportSelectDataSource(listEmployee);
                CustomerCollectionView.DataSource = report_adapter_customer;
                CustomerCollectionView.ReloadData();
            };
            SearchbarView.AddSubview(btnAll);

            #endregion

            #region CustomerCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            CustomerCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CustomerCollectionView.BackgroundColor = UIColor.White;
            CustomerCollectionView.ShowsVerticalScrollIndicator = false;
            CustomerCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CustomerCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseEmployeeViewCell), reuseIdentifier: "ReportChooseEmployeeViewCell");
            View.AddSubview(CustomerCollectionView);

            ReportEmployeeCollectionDelegate CollectionDelegate = new ReportEmployeeCollectionDelegate();
            CollectionDelegate.OnItemSelected += async (indexPath) => {
                var cusotmer = listEmployee[(int)indexPath.Row];
                var search = listChooseEmployee.FindIndex(x => x.UserName == cusotmer.UserName && x.MerchantID == (int)MainController.merchantlocal.MerchantID);
                if (search == -1)
                {
                    listChooseEmployee.Add(cusotmer);
 
                }
                else
                {
                    listChooseEmployee.RemoveAt(search);
                }

                EmployeeSelect = "";

                if (catcount == listChooseEmployee.Count)
                {
                    EmployeeSelect = Utils.TextBundle("all", "Items");
                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    EmployeeSelect = "";
                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                    foreach (var item in listChooseEmployee)
                    {
                        if (EmployeeSelect != "")
                        {
                            EmployeeSelect += "," + item.UserName;
                        }
                        else
                        {
                            EmployeeSelect = item.UserName;
                        }
                    }
                }
                lstEmployee = await GetListEmployee();
                listEmployee = new ListEmployee(lstEmployee);

                EmployeeReportSelectDataSource report_adapter_customer = new EmployeeReportSelectDataSource(listEmployee);
                CustomerCollectionView.DataSource = report_adapter_customer;
                CustomerCollectionView.ReloadData();
            };
            CustomerCollectionView.Delegate = CollectionDelegate;
            
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
            btnSelect.SetTitle(Utils.TextBundle("applyemployee", "Items"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                EmployeeReportController.listChooseEmployee = listChooseEmployee;
                EmployeeReportController.isModifyEmployee = true;
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        async void SearchBytxt()
        {
            var list = await GetFilterEmployeeList();

            if (list!= null)
            {
                listEmployee = new ListEmployee(list);
                
            }
            EmployeeReportSelectDataSource report_adapter_Emp = new EmployeeReportSelectDataSource(listEmployee);
            CustomerCollectionView.DataSource = report_adapter_Emp;
            CustomerCollectionView.ReloadData();
        }
        async Task<List<ORM.MerchantDB.UserAccountInfo>> GetFilterEmployeeList()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await setEmp.GetEmployeeSearch((int)MainController.merchantlocal.MerchantID,txtSearch.Text);
                    return itemlst;
                }
                var itm = await setEmp.GetAllUserAccount();
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

            CustomerCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            CustomerCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CustomerCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

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