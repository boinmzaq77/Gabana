﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.Helper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class EmployeeActivity : AppCompatActivity
    {
        public static EmployeeActivity employeeActivity;
        static RecyclerView recyclerview_listemployee;
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees, listSearchemployee;
        ListEmployee lstemployee;
        LinearLayoutManager mLayoutManager;
        Employee_Adapter_Main Employee_Adapter_Main;
        ImageButton btnAddEmployee;
        EditText txtSearch;
        string SearchName;
        string emplogin;
        SwipeRefreshLayout refreshlayout;
        string LoginType;
        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        ImageButton btnSearchEmployee;
        List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>();
        List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        public static bool flagEmployeeChange { get; set; }
        static ORM.MerchantDB.UserAccountInfo FocusEmployee;
        public static bool CheckNet = false;
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        LinearLayout lnNoDataSearch;
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.employeer_activity);
                employeeActivity = this;
                txtSearch = FindViewById<EditText>(Resource.Id.textSearch);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                LinearLayout lnEmpRole = FindViewById<LinearLayout>(Resource.Id.lnEmpRole);
                lnEmpRole.Click += LnEmpRole_Click;
                recyclerview_listemployee = FindViewById<RecyclerView>(Resource.Id.recyclerview_listemployee);
                btnAddEmployee = FindViewById<ImageButton>(Resource.Id.btnAddEmployee);
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                btnSearchEmployee = FindViewById<ImageButton>(Resource.Id.btnSearchEmployee);
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                flagEmployeeChange = true;
                btnSearchEmployee.Click += BtnSearchEmployee_Click;

                refreshlayout.Refresh += async (sender, e) =>
                {
                    flagEmployeeChange = true;
                    //refresh Online Data    
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await GetOnlineDataEmployee();
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyPress += TxtSearch_KeyPress;
                txtSearch.FocusChange += TxtSearch_FocusChange;

                CheckJwt();
                await GetData();
                await GetListBranch();

                //27/26/2565 ถ้ากรณนีออฟไลน์มาจะกดได้แต่แสดงป็อบอัปแทน

                _ = TinyInsights.TrackPageViewAsync("OnCreate : EmployeeActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Employee");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task GetData()
        {
            if (!await GabanaAPI.CheckNetWork() || !await GabanaAPI.CheckSpeedConnection())
            {
                string gabanaInfo = Preferences.Get("GabanaInfo", "");
                GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                DataCashingAll.GetGabanaInfo = GabanaInfo;
                CheckNet = false;
                return;
            }

            if (DataCashingAll.Merchant?.UserAccountInfo == null)
            {
                DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
            }

            getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
            await GetGabanaInfo();
            CheckNet = true;
        }

        private async Task GetListBranch()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();
                if (await GabanaAPI.CheckSpeedConnection() || await GabanaAPI.CheckNetWork())
                {
                    cloudbranch = await GabanaAPI.GetDataBranch();
                    if (cloudbranch == null)
                    {
                        return;
                    }
                    if (cloudbranch.Count > 0)
                    {
                        foreach (var item in cloudbranch)
                        {
                            ORM.MerchantDB.Branch branch = new ORM.MerchantDB.Branch()
                            {
                                Address = item.Address,
                                AmphuresId = item.AmphuresId,
                                BranchID = item.BranchID,
                                BranchName = item.BranchName,
                                Comments = item.Comments,
                                DisplayLanguage = item.DisplayLanguage,
                                DistrictsId = item.DistrictsId,
                                Email = item.Email,
                                Facebook = item.Facebook,
                                Instagram = item.Instagram,
                                Lat = item.Lat,
                                Line = item.Line,
                                LinkProMaxxID = item.LinkProMaxxID,
                                Lng = item.Lng,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                ProvincesId = item.ProvincesId,
                                Status = item.Status,
                                SysBranchID = item.SysBranchID,
                                TaxBranchID = item.TaxBranchID,
                                TaxBranchName = item.TaxBranchName,
                                Tel = item.Tel,
                            };
                            await branchManage.InsertorReplacrBranch(branch);
                            if (branch.Status == 'A')
                            {
                                localbranch.Add(branch);
                            }
                        }
                        lstBranch = new List<ORM.MerchantDB.Branch>();
                        lstBranch.AddRange(localbranch);
                        lstBranch = lstBranch.Where(x => x.Status == 'A').OrderBy(x => x.SysBranchID).ToList();
                    }
                }
                else
                {
                    lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                    if (lstBranch == null)
                    {
                        Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstBranch = new List<Branch>();
                    }
                }

                //DataCashingAll.MaxEmployee = lstBranch.Count * 10;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnSearchEmployee_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            SetDataEmployee();
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearch();
        }

        private void TxtSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void TxtSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterEmployeeData();
                SetBtnSearch();
            }
            View view = this.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
                }
            }
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
            {
                e.Handled = false;
            }

            e.Handled = false;
            if (e.Handled)
            {
                string input = string.Empty;
                switch (e.KeyCode)
                {
                    case Keycode.Num0:
                        input += "0";
                        break;
                    case Keycode.Num1:
                        input += "1";
                        break;
                    case Keycode.Num2:
                        input += "2";
                        break;
                    case Keycode.Num3:
                        input += "3";
                        break;
                    case Keycode.Num4:
                        input += "4";
                        break;
                    case Keycode.Num5:
                        input += "5";
                        break;
                    case Keycode.Num6:
                        input += "6";
                        break;
                    case Keycode.Num7:
                        input += "7";
                        break;
                    case Keycode.Num8:
                        input += "8";
                        break;
                    case Keycode.Num9:
                        input += "9";
                        break;
                    default:
                        break;
                }
                //e.Handled = false;
                txtSearch.Text += input;
                txtSearch.SetSelection(txtSearch.Text.Length);
                return;
            }
        }

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetDataEmployee();
            }
            SetBtnSearch();
        }

        private async void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            btnAddEmployee.Enabled = false;
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }

            btnAddEmployee.Enabled = true;
            StartActivity(new Android.Content.Intent(Application.Context, typeof(CheckNewUserActivity)));
        }

        private void SetDataEmployee()
        {
            try
            {
                lstemployee = new ListEmployee(listEmployees);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listemployee.HasFixedSize = true;
                recyclerview_listemployee.SetLayoutManager(mLayoutManager);
                Employee_Adapter_Main = new Employee_Adapter_Main(lstemployee);
                int count = lstemployee == null ? 0 : lstemployee.Count + 1;
                recyclerview_listemployee.SetItemViewCacheSize(count);
                recyclerview_listemployee.SetAdapter(Employee_Adapter_Main);
                Employee_Adapter_Main.ItemClick += Employee_Adapter_Main_ItemClick;

                if (Employee_Adapter_Main.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    recyclerview_listemployee.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    recyclerview_listemployee.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }

        private async Task GetDataEmployee()
        {
            try
            {
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    if (DataCashingAll.UserAccountInfo == null)
                    {
                        DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                        DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    }

                    var Employeeoffline = JsonConvert.SerializeObject(DataCashingAll.UserAccountInfo);
                    Preferences.Set("Employeeoffline", Employeeoffline);
                    var Employee = Preferences.Get("Employeeoffline", "");
                    if (Employee != "")
                    {
                        var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                        DataCashingAll.UserAccountInfo = lstEmployee;
                    }

                    if (getlstbranchPolicy == null)
                    {
                        getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                    }

                    var sort = DataCashingAll.UserAccountInfo.OrderBy(x =>
                    {
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
                        var data = await GabanaAPI.GetDataUserAccount(item.UserName);
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = item.MerchantID,
                            UserName = item.UserName.ToLower(),
                            FUsePincode = data?.userAccountInfo.FUsePincode ?? 0,
                            PinCode = data?.userAccountInfo.PinCode ?? null,
                            Comments = data?.userAccountInfo.Comments,
                        };
                        var insertlocal = await userAccountInfoManage.InsertorReplaceUserAccount(localUser);

                        //Insert BranchPolicy
                        if (insertlocal)
                        {
                            if (getlstbranchPolicy != null & item.MainRoles.ToLower() != "owner" & item.MainRoles.ToLower() != "admin")
                            {
                                var result = getlstbranchPolicy.Where(x => x.UserName.ToLower() == item.UserName.ToLower()).ToList();
                                foreach (var itembranch in result)
                                {
                                    ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                    {
                                        MerchantID = itembranch.MerchantID,
                                        SysBranchID = (int)itembranch.SysBranchID,
                                        UserName = itembranch.UserName.ToLower(),
                                    };
                                    BranchPolicyManage policyManage = new BranchPolicyManage();
                                    //var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);

                                    //var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, itembranch.UserName);
                                    var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                                }
                            }
                            emp.Add(localUser);
                        }
                    }

                    //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                    UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                    var getUseraccount = await accountInfoManage.GetAllUserAccount();
                    var lstGabanaAPI = DataCashingAll.Merchant?.UserAccountInfo;

                    HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName.ToLower()));
                    var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName.ToLower())).ToList();
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
                    listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                    listEmployees.AddRange(emp);
                }
                else
                {
                    var sort = DataCashingAll.UserAccountInfo.OrderBy(x =>
                    {
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
                    var getlistEmployees = await GetListEmployee();
                    List<ORM.MerchantDB.UserAccountInfo> data = new List<ORM.MerchantDB.UserAccountInfo>();
                    foreach (var item in DataCashingAll.UserAccountInfo)
                    {
                        var useraccount = getlistEmployees.Where(x => x.UserName.ToLower() == item.UserName.ToLower()).FirstOrDefault();
                        data.Add(useraccount);
                    }
                    listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                    listEmployees.AddRange(data);
                }

                //สไลด์ลบข้อมูล 
                DataCashing.StatusSwipe = new List<int>();

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                bool CheckDelete = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "employee");
                if (CheckDelete)
                {
                    foreach (var item in listEmployees)
                    {
                        var dataEmpPosition = DataCashingAll.UserAccountInfo
                                            .Where(x => x.UserName.ToLower() == item.UserName.ToLower()).FirstOrDefault();

                        if (LoginType.ToLower() == "owner" && dataEmpPosition.UserName != emplogin)
                        {
                            DataCashing.StatusSwipe.Add(listEmployees.FindIndex(x => x.UserName == item.UserName));
                        }
                        else if (LoginType.ToLower() == "admin" && dataEmpPosition.UserName != emplogin && dataEmpPosition.MainRoles.ToLower() != "owner")
                        {
                            DataCashing.StatusSwipe.Add(listEmployees.FindIndex(x => x.UserName == item.UserName));
                        }
                        else if (LoginType.ToLower() == "manager" && dataEmpPosition.UserName != emplogin && dataEmpPosition.MainRoles.ToLower() != "owner" && dataEmpPosition.MainRoles.ToLower() != "admin")
                        {
                            DataCashing.StatusSwipe.Add(listEmployees.FindIndex(x => x.UserName == item.UserName));
                        }
                    }                    
                }
                SetDataEmployee();
            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }

        private async void Employee_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                if (!await GabanaAPI.CheckNetWork() || !await GabanaAPI.CheckSpeedConnection())
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.offline_dialog_main.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("empoffline", "empoffline");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }

                ORM.MerchantDB.UserAccountInfo employee = new ORM.MerchantDB.UserAccountInfo();

                if (string.IsNullOrEmpty(SearchName))
                {
                    employee = listEmployees[e];
                }
                else
                {
                    employee = listSearchemployee[e];
                }

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                var logintype = LoginType.ToLower();
                if (logintype == "owner" || logintype == "admin" || emplogin.ToLower() == employee.UserName.ToLower())
                {
                    StartActivity(new Intent(Application.Context, typeof(AddEmployeeActivity)));
                    AddEmployeeActivity.LocalEmployee = employee;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" Employee_Adapter_Main_ItemClick at Employee");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        {
            try
            {
                listEmployees = await userAccountInfoManage.GetAllUserAccount();
                if (listEmployees == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
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

        private void LnEmpRole_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeRoleActivity)));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            AddEmployeeActivity.LocalEmployee = null;
            DataCashing.StatusSwipe = null;
            FocusEmployee = null;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                await GetData();

                if (flagEmployeeChange)
                {
                    if (string.IsNullOrEmpty(SearchName))
                    {
                        await GetDataEmployee();
                        SetBtnSearch();
                    }
                    flagEmployeeChange = false;
                }

                EmployeeFocus();

                //Owner ,Admin
                string Login = LoginType.ToLower();
                var checkInsert = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "employee");
                int userCurrent = DataCashingAll.UserAccountInfo.Count;
                int maxuser = DataCashingAll.GetGabanaInfo.TotalUser;

                if ((checkInsert && (userCurrent < maxuser)) && await GabanaAPI.CheckSpeedConnection())
                {
                    btnAddEmployee.SetBackgroundResource(Resource.Mipmap.Add);
                    btnAddEmployee.Enabled = true;
                    btnAddEmployee.Click += BtnAddEmployee_Click;
                }
                else
                {
                    btnAddEmployee.SetBackgroundResource(Resource.Mipmap.AddMax);
                    btnAddEmployee.Enabled = false;
                }

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;
                MySwipeHelper EmpSwipe = new EmpswipeHelper(this, recyclerview_listemployee, (int)Width);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                btnAddEmployee.Enabled = true;
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" OnResume at Employee");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                base.OnRestart();
            }
        }

        private async Task GetGabanaInfo()
        {
            try
            {
                GabanaInfo gabanaInfo = new GabanaInfo();
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

        public void Resume()
        {
            OnResume();
        }

        private async void SetFilterEmployeeData()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchName))
                {
                    return;
                }


                listSearchemployee = new List<ORM.MerchantDB.UserAccountInfo>();
                List<ORM.MerchantDB.UserAccountInfo> listMainRole = new List<ORM.MerchantDB.UserAccountInfo>();
                List<ORM.MerchantDB.UserAccountInfo> listSearch = new List<ORM.MerchantDB.UserAccountInfo>();

                //การค้นหาตำแหน่งด้วยภาษาไทย
                List<string> Role = new List<string> { "แอดมิน", "พนักงานเก็บเงิน", "ผู้แก้ไข", "พนักงานออกใบเสร็จ", "ผู้จัดการ", "พนักงานทั่วไป", "เจ้าของ" };
                var SearchMainRoleTH = Role.Where(x => x.ToLower().Contains(SearchName)).ToList();
                if (SearchMainRoleTH != null && SearchMainRoleTH.Count > 0)
                {
                    foreach (var item in SearchMainRoleTH)
                    {
                        switch (item)
                        {
                            case "แอดมิน":
                                SearchName = "admin";
                                break;
                            case "พนักงานเก็บเงิน":
                                SearchName = "cashier";
                                break;
                            case "ผู้แก้ไข":
                                SearchName = "editor";
                                break;
                            case "พนักงานออกใบเสร็จ":
                                SearchName = "invoice";
                                break;
                            case "ผู้จัดการ":
                                SearchName = "manager";
                                break;
                            case "พนักงานทั่วไป":
                                SearchName = "officer";
                                break;
                            default:
                                SearchName = "owner";
                                break;
                        }
                        var MainRole = DataCashingAll.UserAccountInfo.Where(x => x.MainRoles.ToLower().Contains(SearchName)).ToList();
                        if (MainRole != null && MainRole.Count > 0)
                        {
                            listMainRole.AddRange(listEmployees.Where(x => MainRole.Select(x => x.UserName.ToLower()).ToList().Contains(x.UserName.ToLower())).ToList());
                        }
                    }
                }
                else
                {
                    var SearchMainRole = DataCashingAll.UserAccountInfo.Where(x => x.MainRoles.ToLower().Contains(SearchName)).ToList();
                    if (SearchMainRole != null && SearchMainRole.Count > 0)
                    {
                        listMainRole = listEmployees.Where(x => SearchMainRole.Select(x => x.UserName.ToLower()).ToList().Contains(x.UserName.ToLower())).ToList();
                    }
                }
               
                listSearch = listEmployees.Where(x => x.UserName.ToLower().Contains(SearchName)).ToList();

                if (listSearch.Count > 0)
                {
                    listSearch = listSearch.Where(x => DataCashingAll.UserAccountInfo.Select(x => x.UserName.ToLower()).ToList().Contains(x.UserName.ToLower())).ToList();
                }

                listSearchemployee.AddRange(listMainRole);
                listSearchemployee.AddRange(listSearch);

                lstemployee = new ListEmployee(listSearchemployee);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listemployee.HasFixedSize = true;
                recyclerview_listemployee.SetItemViewCacheSize(50);
                recyclerview_listemployee.SetLayoutManager(mLayoutManager);
                Employee_Adapter_Main = new Employee_Adapter_Main(lstemployee);
                recyclerview_listemployee.SetAdapter(Employee_Adapter_Main);
                Employee_Adapter_Main.ItemClick += Employee_Adapter_Main_ItemClick;
                var logintype = LoginType.ToLower();

                if (Employee_Adapter_Main.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    recyclerview_listemployee.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    recyclerview_listemployee.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetFilterEmployeeData at Employee");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.Search);
                //btnSearchEmployee.Enabled = false;
            }
            else
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
                //btnSearchEmployee.Enabled = true;
            }
        }

        private class EmpswipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public EmpswipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
            {
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
            }

            public override void InstantiateMybutton(RecyclerView.ViewHolder viewHolder, List<MyButton> buffer)
            {


                buffer.Add(new MyButton(context,
                   "Delete",
                   0,
                   Resource.Mipmap.DeleteBt,
                   "#33AAE1",
                   new MyDeleteEmpButtonClick(this)));
            }

            private class MyDeleteEmpButtonClick : MyButtonClickListener
            {
                private EmpswipeHelper empswipeHelper;

                public MyDeleteEmpButtonClick(EmpswipeHelper empswipeHelper)
                {
                    this.empswipeHelper = empswipeHelper;
                }

                public async void OnClick(int position)
                {
                    #region oldcode
                    try
                    {
                        if (!await GabanaAPI.CheckNetWork())
                        {
                            Toast.MakeText(empswipeHelper.context, empswipeHelper.context.GetString(Resource.String.nointernet), ToastLength.Short).Show();
                            return;
                        }
                        else if (!await GabanaAPI.CheckSpeedConnection())
                        {
                            Toast.MakeText(empswipeHelper.context, empswipeHelper.context.GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                            return;
                        }

                        var emplogin = Preferences.Get("User", "");
                        var LoginType = Preferences.Get("LoginType", "");

                        var employee = listEmployees[position];
                        var getdataEmp = DataCashingAll.UserAccountInfo.Where(x => x.UserName == employee.UserName).FirstOrDefault();

                        //Owner
                        if (emplogin.ToLower() != employee.UserName.ToLower())
                        {
                            if (LoginType.ToLower() == "admin" | LoginType.ToLower() == "owner")
                            {
                                try
                                {
                                    MainDialog dialog = new MainDialog();
                                    Bundle bundle = new Bundle();
                                    String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                                    bundle.PutString("message", myMessage);
                                    bundle.PutString("deleteType", "employee");
                                    bundle.PutString("fromPage", "main");
                                    bundle.PutString("username", employee.UserName);
                                    dialog.Arguments = bundle;
                                    dialog.Show(employeeActivity.SupportFragmentManager, myMessage);
                                }
                                catch (Exception ex)
                                {
                                    _ = TinyInsights.TrackErrorAsync(ex);
                                    _ = TinyInsights.TrackPageViewAsync("EmpswipeHelper");
                                    Toast.MakeText(empswipeHelper.context, ex.Message, ToastLength.Short).Show();
                                    return;
                                }
                            }
                            else
                            {
                                Toast.MakeText(empswipeHelper.context, $"ลบไม่สำเร็จ", ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(empswipeHelper.context, $"ลบไม่สำเร็จ", ToastLength.Short).Show();
                        }

                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(empswipeHelper.context, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                        return;

                    }
                    #endregion
                }
            }
        }

        internal static void SetFocusEmployee(ORM.MerchantDB.UserAccountInfo userAccount)
        {
            try
            {
                FocusEmployee = userAccount;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusEmployee at Employee");
            }
        }

        private void EmployeeFocus()
        {
            try
            {
                if (FocusEmployee != null)
                {
                    int index = -1;
                    if (listEmployees != null)
                    {
                        index = listEmployees.FindIndex(x => x.UserName == FocusEmployee.UserName);
                        if (index != -1)
                        {
                            listEmployees.RemoveAt(index);
                        }
                        listEmployees.Insert(0, FocusEmployee);
                    }
                    if (listSearchemployee?.Count > 0)
                    {
                        index = listSearchemployee.FindIndex(x => x.UserName == FocusEmployee.UserName);
                        if (index != -1)
                        {
                            listSearchemployee.RemoveAt(index);
                        }
                        listSearchemployee.Insert(0, FocusEmployee);
                    }
                    Employee_Adapter_Main.NotifyDataSetChanged();
                    FocusEmployee = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EmployeeFocus at Employee");
                Toast.MakeText(employeeActivity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GetOnlineDataEmployee()
        {
            try
            {
                //InsertUserAccount
                List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>(); ;
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                if (DataCashingAll.Merchant?.UserAccountInfo == null)
                {
                    DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                }

                if (DataCashingAll.UserAccountInfo.Count > 0 & getlstbranchPolicy.Count > 0)
                {
                    //insert Seauth to local
                    foreach (var UserAccountInfo in DataCashingAll.UserAccountInfo)
                    {
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = UserAccountInfo.MerchantID,
                            UserName = UserAccountInfo.UserName?.ToLower(),
                            FUsePincode = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                            PinCode = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => x.PinCode).FirstOrDefault(),
                            Comments = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName.ToLower() == UserAccountInfo.UserName?.ToLower()).Select(x => x.Comments).FirstOrDefault(),
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
                                    //var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                                    //var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, itembranch.UserName);
                                    var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                                }
                            }
                        }

                    }

                    var Employeeoffline = JsonConvert.SerializeObject(DataCashingAll.UserAccountInfo);
                    Preferences.Set("Employeeoffline", Employeeoffline);
                    var Employee = Preferences.Get("Employeeoffline", "");
                    if (Employee != "")
                    {
                        var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                        DataCashingAll.UserAccountInfo = lstEmployee;
                    }

                    //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                    List<ORM.MerchantDB.UserAccountInfo> getUseraccount = new List<ORM.MerchantDB.UserAccountInfo>();
                    List<ORM.Master.UserAccountInfo> lstGabanaAPI = new List<ORM.Master.UserAccountInfo>();
                    getUseraccount = await accountInfoManage.GetAllUserAccount();
                    lstGabanaAPI = DataCashingAll.Merchant?.UserAccountInfo;

                    if (getUseraccount[0].MerchantID == 0)
                    {
                        _ = TinyInsights.TrackPageViewAsync("InsertUserAccount :" + getUseraccount[0].Comments);
                        return;
                    }

                    HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName.ToLower()));
                    var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName.ToLower())).ToList();
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
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataEmployee at Employee ");
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'EmployeeActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'EmployeeActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

            CheckJwt();
        }
    }
}

