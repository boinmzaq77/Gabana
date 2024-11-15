using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using Newtonsoft.Json;
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
    public partial class EmployeeController : UIViewController
    {
        public static bool Ismodify ;
      //  UIBarButtonItem backButton;
        UICollectionView EmployeeCollection;
        List<UserAccountInfo> listEmployee;
        UIImageView addEmp;
        UserAccountInfoManage UserManager = new UserAccountInfoManage();

        string emplogin;
        string LoginType;
        private bool checkinsert;
        private bool checkdelete;
        private bool check;
        UIView SearchBar;
        UIImageView SearchBtn;
        UIImageView emptyView;
        UILabel lblempty;
        UITextField txtsearch;
        UIScrollView scroll;
        EmployeeRoleController employeeRolePage = null;
        public EmployeeController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                Utils.SetTitle(this.NavigationController,Utils.TextBundle("employee", "Employee"));
                if (await GabanaAPI.CheckNetWork())
                {
                    await InsertUserAccount();
                }
                txtsearch.Text = "";
                if (EmployeeCollection != null&& EmployeeCollection.DataSource!=null)
                {
                    var data = EmployeeCollection?.DataSource as EmployeeDataSource;
                    if (data?.choosecell != null)
                    {
                        var frame2 = data.choosecell.Frame;
                        frame2.X = 0;
                        UIView.Animate(0.7, () =>
                        {
                            data.choosecell.showbtndelete = false;
                            data.choosecell.Frame = frame2;
                        });
                    };

                    //DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    listEmployee = await GetListEmployee();
                    ((EmployeeDataSource)EmployeeCollection.DataSource).ReloadData(listEmployee);
                    EmployeeCollection.ReloadData();

                    
                }
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }
        private async Task GetGabanaInfo()
        {
            try
            {
                Gabana.Model.GabanaInfo gabanaInfo = new Gabana.Model.GabanaInfo();
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetCloudProductLicence at Package");
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this.NavigationController);
                this.NavigationController.SetNavigationBarHidden(false, false);
                View.BackgroundColor = UIColor.White;
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("employee", "Employee"));
                //  this.NavigationController.NavigationBar.TopItem.Title = "Employee";
                base.ViewDidLoad();
                await GetGabanaInfo();
                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                checkinsert = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "employee");
                checkdelete = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "employee");
                
                #region SearchBar
                SearchBar  = new UIView();
                SearchBar.TranslatesAutoresizingMaskIntoConstraints = false;
                SearchBar.BackgroundColor = UIColor.FromRGB(226,226,226);
                View.AddSubview(SearchBar);

                SearchBtn = new UIImageView();
                SearchBtn.Image = UIImage.FromBundle("Search");
                SearchBtn.TranslatesAutoresizingMaskIntoConstraints = false;
                SearchBar.AddSubview(SearchBtn);
                SearchBtn.UserInteractionEnabled = true;
                var tapGestureSearch = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Search:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                SearchBtn.AddGestureRecognizer(tapGestureSearch);

                txtsearch = new UITextField
                {
                    Placeholder = "",
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtsearch.BackgroundColor = UIColor.Clear;
                txtsearch.Font = txtsearch.Font.WithSize(15);
                txtsearch.ReturnKeyType = UIReturnKeyType.Done;
                txtsearch.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    SearchBytxt();
                    return true;
                };
                txtsearch.Placeholder = Utils.TextBundle("employeeplace", "");
                SearchBar.AddSubview(txtsearch);

                #endregion

                scroll = new UIScrollView();
                scroll.UserInteractionEnabled = true;
                scroll.ShowsVerticalScrollIndicator = true;
                scroll.ScrollEnabled = true;
                scroll.BackgroundColor = UIColor.White;
                scroll.ContentSize = new CGSize(View.Frame.Width, View.Frame.Height + 100);
                scroll.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(scroll);

                #region emptyView
                emptyView = new UIImageView();
                emptyView.Hidden = true;
                emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                emptyView.Image = UIImage.FromBundle("DefaultEmployee");
                emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
                scroll.AddSubview(emptyView);

                lblempty = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(160,160,160),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblempty.Hidden = true;
                lblempty.Lines = 2;
                lblempty.Font = lblempty.Font.WithSize(16);
                lblempty.Text = Utils.TextBundle("emptyemployee", "คุณยังไม่มี Employee สามารถเพิ่ม\n ได้ที่ปุ่ม Add Employee ด้านล่าง");
                scroll.AddSubview(lblempty);

                #endregion

                

                #region EmployeeCollection
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 80, height: 80);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

                EmployeeCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                EmployeeCollection.BackgroundColor = UIColor.White;
                EmployeeCollection.Hidden = true;
                EmployeeCollection.AlwaysBounceVertical = true;
                EmployeeCollection.ShowsVerticalScrollIndicator = false;
                EmployeeCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                
                View.AddSubview(EmployeeCollection);
                #endregion

                #region btnadd
                addEmp = new UIImageView();
                addEmp.Image = UIImage.FromBundle("Add");
                addEmp.Hidden = true;
                addEmp.TranslatesAutoresizingMaskIntoConstraints = false;
                

                addEmp.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("AddEmp:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                addEmp.AddGestureRecognizer(tapGesture);
                View.AddSubview(addEmp);
                #endregion
                setupAutoLayout();

                //Owner
                //if (LoginType.ToLower() == "owner" || LoginType.ToLower() == "admin")
                //{

                //}
                //else
                //{

                //    EmployeeCollection.Hidden = true;
                //    emptyView.Hidden = false;
                //    lblempty.Hidden = false;
                //    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเข้าถึงได้");
                //}


                EmployeeCollection.RegisterClassForCell(cellType: typeof(EmployeeViewCell), reuseIdentifier: "employeeViewCell");
                listEmployee = await GetListEmployee();
                EmployeeDataSource BranchDataList = new EmployeeDataSource(listEmployee); // ส่ง list ไป
                BranchDataList.OnCardCellDelete += Employee_OnCardCellDelete;

                EmployeeCollectionDelegate EmployeeCollectionDelegate = new EmployeeCollectionDelegate();
                EmployeeCollectionDelegate.OnItemSelected += async (indexPath) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("editemployee", "Edit Employee"));
                        var employee = listEmployee[indexPath.Row];
                        var emplogin = Preferences.Get("User", "");
                        var LoginType = Preferences.Get("LoginType", "");
                        if (checkinsert || emplogin == employee.UserName)
                        {
                            UpdateEmployeeController UpdateCustomerPage = new UpdateEmployeeController(listEmployee[(int)indexPath.Row]);
                            this.NavigationController.PushViewController(UpdateCustomerPage, false);
                            //this.Finish();
                        }
                        else
                        {
                            Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                            //Toast.MakeText(this, "ไม่สามารถเข้าถึงได้", ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Utils.ShowAlert(this,Utils.TextBundle("plsconnect", "หากต้องการดำเนินการ กรุณาเชื่อมต่ออินเทอร์เน็ต"), "");
                    }
                };
                EmployeeCollection.Delegate = EmployeeCollectionDelegate;
                EmployeeCollection.DataSource = BranchDataList;
                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await InsertUserAccount();
                    }
                    listEmployee = await GetListEmployee();
                    ((EmployeeDataSource)EmployeeCollection.DataSource).ReloadData(listEmployee);



                    EmployeeCollection.ReloadData();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                
                EmployeeCollection.AddSubview(refreshControl);

                if (!checkinsert || listEmployee.Count > DataCashingAll.GetGabanaInfo.TotalUser)
                {
                    addEmp.Alpha = 0.5f;
                }

                var res = await GabanaAPI.CheckNetWork();
                addEmp.Hidden = false;


                if (!res)
                {
                    //EmployeeCollection.Hidden = true;
                    //scroll.Hidden = false;
                    //emptyView.Hidden = false;
                    //lblempty.Hidden = false;
                    //lblempty.Text = "Gabana must be online or connected to complete this action.";
                    //addEmp.UserInteractionEnabled = false;
                    //addEmp.Alpha = 0.5f;
                    if (listEmployee == null || listEmployee.Count == 0)
                    {
                        EmployeeCollection.Hidden = true;
                        emptyView.Hidden = false;
                        scroll.Hidden = false;
                        lblempty.Hidden = false;
                        lblempty.Text = Utils.TextBundle("emptyemployee", "Empty Employee");
                        //DefaultEmployee
                    }
                    else
                    {
                        EmployeeCollection.Hidden = false;
                        emptyView.Hidden = true;
                        scroll.Hidden = true;
                        lblempty.Hidden = true;
                    }
                }
                else
                {
                    addEmp.Hidden = false;
                    if (listEmployee == null || listEmployee.Count == 0)
                    {
                        EmployeeCollection.Hidden = true;
                        emptyView.Hidden = false;
                        scroll.Hidden = false;
                        lblempty.Hidden = false;
                        lblempty.Text = Utils.TextBundle("emptyemployee", "Empty Employee");
                        //DefaultEmployee
                    }
                    else
                    {
                        EmployeeCollection.Hidden = false;
                        emptyView.Hidden = true;
                        scroll.Hidden = true;
                        lblempty.Hidden = true;
                    }
                }
                GabanaLoading.SharedInstance.Hide();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        async Task InsertUserAccount() 
        {
            try
            {
                string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                var merchants = await GabanaAPI.GetMerchantDetail("APNS", Id);
                //InsertUserAccount
                List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy;
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();


                //insert Seauth to local
                foreach (var UserAccountInfo in DataCashingAll.UserAccountInfo)
                {
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = UserAccountInfo.MerchantID,
                        UserName = UserAccountInfo.UserName,
                        FUsePincode = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                        PinCode = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => x.PinCode).FirstOrDefault(),
                        Comments = merchants.UserAccountInfo.Where(x => x.UserName == UserAccountInfo.UserName).Select(x => x.Comments).FirstOrDefault(),
                    };
                    var insertlocal = await accountInfoManage.InsertorReplaceUserAccount(localUser);

                    //Insert BranchPolicy
                    if (insertlocal)
                    {
                        if (getlstbranchPolicy != null & UserAccountInfo.MainRoles.ToLower() != "owner" & UserAccountInfo.MainRoles.ToLower() != "admin")
                        {
                            List<ORM.Master.BranchPolicy> result = new List<ORM.Master.BranchPolicy>();
                            result = getlstbranchPolicy.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).ToList();
                            if (result == null)
                            {
                                return;
                            }
                            foreach (var itembranch in result)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName?.ToLower(),
                                };
                                BranchPolicyManage policyManage = new BranchPolicyManage();
                                var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                            }
                        }

                    }

                    #region Insert Useraccount Seauth to GabanaAPI เพื่อให้ข้อมูลตรงกันก่อน
                    //Insert To GabanaAPI
                    //List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                    //List<ORM.Master.Branch> lstMerchantBranch = new List<ORM.Master.Branch>();
                    //BranchManage branchManage = new BranchManage();

                    ////MainRole Admin Owner
                    //var getBranch = await branchManage.GetAllBranch(UserAccountInfo.MerchantID);
                    //if (getBranch.Count > 0)
                    //{
                    //    foreach (var item in getBranch)
                    //    {
                    //        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                    //        {
                    //            MerchantID = UserAccountInfo.MerchantID,
                    //            UserName = UserAccountInfo.UserName,
                    //            SysBranchID = (int)item.SysBranchID
                    //        };
                    //        lstbranchPolicies.Add(branchPolicy);
                    //    }

                    //    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    //    {
                    //        MerchantID = UserAccountInfo.MerchantID,
                    //        UserName = UserAccountInfo.UserName,
                    //        FUsePincode = 0,
                    //        PinCode = null,
                    //        Comments = null,
                    //    };

                    //    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    //    {
                    //        branchPolicy = lstbranchPolicies,
                    //        userAccountInfo = gbnAPIUser
                    //    };

                    //    var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    //}
                    #endregion
                }
                var listEmployees = await accountInfoManage.GetAllUserAccount();

                var Employeeoffline = JsonConvert.SerializeObject(DataCashingAll.UserAccountInfo);
                Preferences.Set("Employeeoffline", Employeeoffline);
                var Employee = Preferences.Get("Employeeoffline", "");
                if (Employee != "")
                {
                    var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                    DataCashingAll.UserAccountInfo = lstEmployee;
                }

                //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                var getUseraccount = await accountInfoManage.GetAllUserAccount();
                var lstGabanaAPI = merchants.UserAccountInfo;

                HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName));
                var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName)).ToList();
                if (results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        if (item.UserName.ToLower() == "owner")
                        {
                            break;
                        }
                        //branchPolicy
                        BranchPolicyManage policyManage = new BranchPolicyManage();
                        var getBranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, item.UserName);
                        foreach (var branchPolicy in getBranchPolicy)
                        {
                            var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, item.UserName);
                        }

                        //Useraccount
                        var deleteUseraccount = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, item.UserName);
                    }
                }

                ////Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                //var getUseraccount = await accountInfoManage.GetAllUserAccount();
                //var lstGabanaAPI = merchants.UserAccountInfo;

                //HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName));
                //var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName)).ToList();
                //if (results.Count > 0)
                //{
                //    foreach (var item in results)
                //    {
                //        //branchPolicy
                //        BranchPolicyManage policyManage = new BranchPolicyManage();
                //        var getBranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, item.UserName);
                //        foreach (var branchPolicy in getBranchPolicy)
                //        {
                //            var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, item.UserName);
                //        }

                //        //Useraccount
                //        var deleteUseraccount = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, item.UserName);
                //    }
                //}

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void Employee_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                if (checkdelete)
                {
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "Are you sure you want to delete?"), UIAlertControllerStyle.Alert);

                    //Add Actions
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteEmp(indexPath)));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                    //Present Alert
                    PresentViewController(okCancelAlertController, true, null);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล Customer ได้");
                return;
            }            
        }

        private async void DeleteEmp(NSIndexPath indexPath)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this.NavigationController);
                var mainrole = DataCashingAll.UserAccountInfo.Where(x => x.UserName == listEmployee[(int)indexPath.Row].UserName & x.MainRoles.ToLower() == "owner").FirstOrDefault();
                if (mainrole != null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("notperm", "Not accessible"));
                    return;
                }

                var result = await GabanaAPI.DeleteSeAuthDataUserAccount(listEmployee[(int)indexPath.Row].UserName);
                if (!result.Status)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), result.Message);
                    return;
                }

                var resultGabana = await GabanaAPI.DeleteDataUserAccount(listEmployee[(int)indexPath.Row].UserName);
                if (!resultGabana.Status)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), resultGabana.Message);
                    return;
                }

                //Delete BranchPolicy ของ employee 
                BranchPolicyManage policyManage = new BranchPolicyManage();
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, listEmployee[(int)indexPath.Row].UserName);
                foreach (var item in lstbranchPolicy)
                {
                    var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, listEmployee[(int)indexPath.Row].UserName);
                }

                //Delete useraccount
                var resultLocal = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, listEmployee[(int)indexPath.Row].UserName);
                if (!resultLocal)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "OK"));
                    return;
                }

                if (resultLocal)
                {
                    var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == listEmployee[(int)indexPath.Row].UserName);
                    DataCashingAll.UserAccountInfo.Remove(data);
                    listEmployee = await GetListEmployee();
                    ((EmployeeDataSource)EmployeeCollection.DataSource).ReloadData(listEmployee);
                    EmployeeCollection.ReloadData();
                }
                Utils.ShowMessage("ลบพนักงานสำเร็จ");
                GabanaLoading.SharedInstance.Hide();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "OK"));
                return;
            }
        }

        [Export("Search:")]
        public void Search(UIGestureRecognizer sender)
        {
            txtsearch.BecomeFirstResponder();
        }
        [Export("AddEmp:")]
        public void AddEmp(UIGestureRecognizer sender)
        {
            if (checkinsert && listEmployee.Count < DataCashingAll.GetGabanaInfo.TotalUser )
            {
                AddEmployeeController  empUpdatePage = new AddEmployeeController();
                this.NavigationController.PushViewController(empUpdatePage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        async void SearchBytxt()
        {
            try
            {
                if (string.IsNullOrEmpty(txtsearch.Text))
                {
                    listEmployee = await GetListEmployee();
                }
                else
                {
                    listEmployee = await UserManager.GetEmployeeSearch((int)MainController.merchantlocal.MerchantID, txtsearch.Text);

                }
                if (listEmployee == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "OK"));
                }
                else
                {
                    ((EmployeeDataSource)EmployeeCollection.DataSource).ReloadData(listEmployee);
                    EmployeeCollection.ReloadData();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "OK"));
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
        async Task<List<UserAccountInfo>> GetListEmployee()
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
        void setupAutoLayout()
        {
            #region SearchBar
            SearchBar.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchBar.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchBar.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchBar.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            SearchBtn.TopAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            SearchBtn.WidthAnchor.ConstraintEqualTo(26).Active = true;
            SearchBtn.LeftAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            SearchBtn.BottomAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtsearch.TopAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            txtsearch.RightAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.RightAnchor,-15).Active = true;
            txtsearch.LeftAnchor.ConstraintEqualTo(SearchBtn.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtsearch.BottomAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
            #endregion

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblempty.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lblempty.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lblempty.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lblempty.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            scroll.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scroll.TopAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            scroll.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            EmployeeCollection.TopAnchor.ConstraintEqualTo(SearchBar.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            EmployeeCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            EmployeeCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            EmployeeCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            addEmp.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            addEmp.WidthAnchor.ConstraintEqualTo(45).Active = true;
            addEmp.HeightAnchor.ConstraintEqualTo(45).Active = true;
            addEmp.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class ChooseCustomer : ORM.Master.Customer
        {
            public bool Choose { get; set; }
        }
       
    }
}