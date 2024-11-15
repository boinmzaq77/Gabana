using AutoMapper;
using CoreGraphics;
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

namespace Gabana.iOS
{
    public partial class EmployeeManagementController : UIViewController
    {
        public static bool Ismodify ;
        UICollectionView EmployeeCollection;
        List<ORM.MerchantDB.UserAccountInfo> listEmployee;
        UserAccountInfoManage UserManager = new UserAccountInfoManage();

        UIImageView emptyView;
        UILabel lblempty;

        public EmployeeManagementController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async override void ViewDidLoad()
     {
            try
            {
                View.BackgroundColor = UIColor.White;
                base.ViewDidLoad();

                initAttribute();
                setupAutoLayout();
                setEmployeeData();
                

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget((obj, sender) => {
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                EmployeeCollection.AddSubview(refreshControl);


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        void ShowList()
        {
            if (listEmployee != null && listEmployee.Count != 0)
            {
                //EmployeeDataSource DataList = new EmployeeDataSource(listEmployee); // ส่ง list ไป
                //EmployeeCollection.DataSource = DataList;

                EmployeeCollection.Hidden = false;
                emptyView.Hidden = true;
                lblempty.Hidden = true;
            }
            else
            {
                EmployeeCollection.Hidden = true;
                emptyView.Hidden = false;
                lblempty.Hidden = false;
            }
        }
        private async void setEmployeeData()
        {
            listEmployee = await GetListEmployee();
            ShowList();
            var lstemployee = new ListEmployee(listEmployee);
            if (listEmployee==null || listEmployee.Count ==0)
            {
                return;
            }
            //datasource

            EmployeeManageDataSource EmployeeDatasourse = new EmployeeManageDataSource(lstemployee);
            EmployeeCollection.DataSource = EmployeeDatasourse;
            EmployeeCollection.ReloadData();
        }
        async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        {
            try
            {
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();
                var listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy;
                if (await GabanaAPI.CheckNetWork())
                {
                    DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    if (DataCashingAll.UserAccountInfo == null)
                    {
                        DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    }

                    getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                    if (getlstbranchPolicy == null)
                    {
                        getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                    }
                    var sort = DataCashingAll.UserAccountInfo.OrderBy(x => {
                        var xx = 0;
                        switch (x.MainRoles.ToLower())
                        {
                            case "owner":
                                xx = 1;
                                break;
                            case "admin":
                                xx = 2;
                                break;
                            case "manager":
                                xx = 3;
                                break;
                            case "invoice":
                                xx = 4;
                                break;
                            case "cashier":
                                xx = 5;
                                break;
                            case "editor":
                                xx = 6;
                                break;
                            default:
                                xx = 7;
                                break;
                        }
                        return xx;
                    })
                    .ToList();
                    DataCashingAll.UserAccountInfo = sort;
                    foreach (var item in DataCashingAll.UserAccountInfo)
                    {
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = item.MerchantID,
                            UserName = item.UserName,
                            FUsePincode = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName == item.UserName).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                            PinCode = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName == item.UserName).Select(x => x.PinCode).FirstOrDefault(),
                            Comments = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName == item.UserName).Select(x => x.Comments).FirstOrDefault(),
                        };
                        var insertlocal = await UserManager.InsertorReplaceUserAccount(localUser);

                        //Insert BranchPolicy
                        if (insertlocal)
                        {
                            if (getlstbranchPolicy != null & item.MainRoles.ToLower() != "owner" & item.MainRoles.ToLower() != "admin")
                            {
                                var result = getlstbranchPolicy.Where(x => x.UserName == item.UserName).ToList();
                                foreach (var itembranch in result)
                                {
                                    ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                    {
                                        MerchantID = itembranch.MerchantID,
                                        SysBranchID = (int)itembranch.SysBranchID,
                                        UserName = itembranch.UserName,
                                    };
                                    BranchPolicyManage policyManage = new BranchPolicyManage();
                                    var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                                }
                            }
                            emp.Add(localUser);
                        }
                    }
                    listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                    listEmployees.AddRange(emp);
                }
                else
                {

                    var sort = DataCashingAll.UserAccountInfo.OrderBy(x => {
                        var xx = 0;
                        switch (x.MainRoles.ToLower())
                        {
                            case "owner":
                                xx = 1;
                                break;
                            case "admin":
                                xx = 2;
                                break;
                            case "manager":
                                xx = 3;
                                break;
                            case "invoice":
                                xx = 4;
                                break;
                            case "cashier":
                                xx = 5;
                                break;
                            case "editor":
                                xx = 6;
                                break;
                            default:
                                xx = 7;
                                break;
                        }
                        return xx;
                    })
                    .ToList();
                    DataCashingAll.UserAccountInfo = sort;
                    var getlistEmployees = await GetListEmployeenew();
                    List<ORM.MerchantDB.UserAccountInfo> data = new List<ORM.MerchantDB.UserAccountInfo>();
                    foreach (var item in DataCashingAll.UserAccountInfo)
                    {
                        var useraccount = getlistEmployees.Where(x => x.UserName.ToLower() == item.UserName.ToLower()).FirstOrDefault();
                        data.Add(useraccount);
                    }
                    listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                    listEmployees.AddRange(data);

                    //listEmployees = new List<UserAccountInfo>();
                    //listEmployees = await UserManager.GetAllUserAccount();
                    //if (listEmployees == null)
                    //{
                    //    return null;
                    //}

                }

                return listEmployees;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployeenew()
        {
            try
            {
                var listEmployees = await UserManager.GetAllUserAccount();
                if (listEmployees == null)
                {
                    //Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    return null;
                }
                return listEmployees;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" GetListEmployee at Employee");
                return null;
            }
        }
        void initAttribute()
        {

            #region emptyView
            emptyView = new UIImageView();
            emptyView.Hidden = true;
            emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyView.Image = UIImage.FromBundle("DefaultEmployee");
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(emptyView);

            lblempty = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblempty.Hidden = true;
            lblempty.Lines = 2;
            lblempty.Font = lblempty.Font.WithSize(16);
            lblempty.Text = Utils.TextBundle("defaultemployee", "You don't have Employee yet. You can add\n to the Employee menu.") ;
            View.AddSubview(lblempty);

            #endregion

            #region EmployeeCollection
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            EmployeeCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            EmployeeCollection.BackgroundColor = UIColor.White;
            EmployeeCollection.Hidden = true;
            EmployeeCollection.ShowsVerticalScrollIndicator = false;
            EmployeeCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            EmployeeCollection.RegisterClassForCell(cellType: typeof(EmployeeManageViewCell), reuseIdentifier: "employeeManageViewCell");
            View.AddSubview(EmployeeCollection);
            #endregion

        }
        //public async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        //{
        //    try
        //    {
        //        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        //       var  listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
        //        listEmployees = await userAccountInfoManage.GetAllUserAccount();
        //        if (listEmployees == null)
        //        {
        //            return null;
        //        }
        //        return listEmployees;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
        //}
        void setupAutoLayout()
        {

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblempty.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lblempty.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lblempty.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lblempty.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            EmployeeCollection.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            EmployeeCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            EmployeeCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            EmployeeCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

    }
}