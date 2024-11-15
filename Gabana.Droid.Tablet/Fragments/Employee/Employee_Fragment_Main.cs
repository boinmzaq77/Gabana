using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter.Employee;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Dashboard;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Employee
{
    public class Employee_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Employee_Fragment_Main NewInstance()
        {
            Employee_Fragment_Main frag = new Employee_Fragment_Main();
            return frag;
        }
        View view;
        string SearchName, LoginType;
        public static Employee_Fragment_Main fragment_main;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.employee_fragment_main, container, false);
            try
            {
                fragment_main = this;
                CombineUI();
                SetUIEvent();
                CheckJwt();               

                refreshlayout.Refresh += async (sender, e) =>
                {
                    //refresh Online Data    
                    if (!DataCashing.CheckNet)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }                    
                    else
                    {
                        await GetOnlineDataEmployee();                        
                    }
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };               
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetUIEvent()
        {
            btnSearchEmployee.Click += BtnSearchEmployee_Click;
            textSearch.TextChanged += TextSearch_TextChanged;
            textSearch.KeyPress += TextSearch_KeyPress;
            textSearch.FocusChange += TextSearch_FocusChange;
            btnAddEmployee.Click += BtnAddEmployee_Click;
        }

        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
        public static List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>();
        public static List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();


        private async Task GetListBranch()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                if (DataCashing.CheckNet)
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        lstBranch = new List<Branch>();
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Branch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private async Task GetData()
        {
            DialogLoading dialogloading = new DialogLoading();
            try
            {
                if (dialogloading.Cancelable != false)
                {
                    dialogloading.Cancelable = false;
                    dialogloading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                if (DataCashing.CheckNet)
                {
                    if (DataCashingAll.Merchant?.UserAccountInfo == null)
                    {
                        DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    }
                    getlstbranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                    await GetGabanaInfo();
                }
                else
                {
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                    Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                }

                if (string.IsNullOrEmpty(SearchName))
                {
                    await GetDataEmployee();
                    SetBtnSearch();
                }

                SetButtonAddReload();
                await GetListBranch();

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;
                //MySwipeHelper EmpSwipe = new EmpswipeHelper(this, recyclerview_listemployee, (int)Width);

                if (dialogloading != null)
                {
                    dialogloading.DismissAllowingStateLoss();
                    dialogloading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogloading.Dismiss();
                Toast.MakeText(this.Activity,ex.Message , ToastLength.Short).Show();
            }
        }

        public async override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (check)
                {
                    btnAddEmployee.SetBackgroundResource(Resource.Mipmap.Add);
                }
                else
                {
                    btnAddEmployee.SetBackgroundResource(Resource.Mipmap.AddMax);
                }
                await GetData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" OnResume at Employee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        
        private async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        {
            try
            {
                listEmployees = await userAccountInfoManage.GetAllUserAccount();
                if (listEmployees == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    return listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                }
                return listEmployees;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" GetListEmployee at Employee");
                return listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
            }
        }

        private async Task GetDataEmployee()
        {
            try
            {
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();

                if (DataCashing.CheckNet)
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
                foreach (var item in listEmployees)
                {
                    var dataEmpPosition = DataCashingAll.UserAccountInfo
                                            .Where(x => x.UserName.ToLower() == item.UserName.ToLower()).FirstOrDefault();
                    bool CheckDelete = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", dataEmpPosition.MainRoles.ToLower());
                    if (CheckDelete)
                    {
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" GetDataEmployee at Employee");
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
            textSearch.Text = string.Empty;
            SetBtnSearch();
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearchEmployee.Enabled = false;
            }
            else
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearchEmployee.Enabled = true;
            }
        }
        ListEmployee lstemployee;
        Employee_Adapter_Main employee_adapter_main;

        private void SetDataEmployee()
        {
            try
            {
                lstemployee = new ListEmployee(listEmployees);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistemployee.HasFixedSize = true;
                rcvlistemployee.SetLayoutManager(mLayoutManager);
                employee_adapter_main = new Employee_Adapter_Main(lstemployee);
                rcvlistemployee.SetAdapter(employee_adapter_main);
                employee_adapter_main.ItemClick += Employee_adapter_main_ItemClick; 

                if (employee_adapter_main.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    rcvlistemployee.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    rcvlistemployee.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }

        private void Employee_adapter_main_ItemClick(object sender, int e)
        {
            try
            {
                if (!DataCashing.CheckNet)
                {
                    var fragment = new Alert_Dialog_Offline();
                    fragment.Show(Activity.SupportFragmentManager, nameof(Alert_Dialog_Offline));
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

                if (LoginType == "owner" || LoginType == "admin" || emplogin.ToLower() == employee.UserName.ToLower())
                {
                    DataCashing.EditEmployee = employee;
                    Employee_Fragment_AddEmployee.flagdatachange = false;
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "addemployee");
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync(" Employee_Adapter_Main_ItemClick at Employee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                btnAddEmployee.Enabled = false;
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAddEmployee.Enabled = true;
                    return;
                }

                DataCashing.EditEmployee = null;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "checkuser");
                btnAddEmployee.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAddEmployee.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" BtnAddEmployee_Click at Employee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        static string FocusName = string.Empty;
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees, listSearchemployee;
        private void EmployeeFocus()
        {
            try
            {
                if (!string.IsNullOrEmpty(FocusName))
                {
                    if (string.IsNullOrEmpty(SearchName))
                    {
                        var index = listEmployees.FindIndex(x => x.UserName == FocusName);
                        if (index == -1)
                        {
                            return;
                        }
                        else
                        {
                            rcvlistemployee.ScrollToPosition(index);
                            FocusName = string.Empty;
                        }
                    }
                    else
                    {
                        var index = listSearchemployee.FindIndex(x => x.UserName == FocusName);
                        if (index == -1)
                        {
                            return;
                        }
                        else
                        {
                            rcvlistemployee.ScrollToPosition(index);
                            FocusName = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EmployeeFocus at Employee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        ImageButton btnSearchEmployee, btnAddEmployee;
        EditText textSearch;
        SwipeRefreshLayout refreshlayout;
        RecyclerView rcvlistemployee;
        LinearLayout lnNoDataSearch;
        string emplogin;
        private void CombineUI()
        {
            btnSearchEmployee = view.FindViewById<ImageButton>(Resource.Id.btnSearchEmployee);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            rcvlistemployee = view.FindViewById<RecyclerView>(Resource.Id.rcvlistemployee);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnAddEmployee = view.FindViewById<ImageButton>(Resource.Id.btnAddEmployee);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);

            emplogin = Preferences.Get("User", "");
            string str = Preferences.Get("LoginType", "");
            LoginType = str.ToLower();
        }

        private void TextSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearchEmployee.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        private async void SetFilterEmployeeData()
        {
            try
            {
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

                var duplicates = listSearchemployee.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
                if (duplicates.Count > 0)
                {
                    foreach (var item in duplicates)
                    {
                        var index = listSearchemployee.FindIndex(x=>x.UserName.ToLower() == item.UserName.ToLower());
                        if (index != -1) 
                        {
                            listSearchemployee.RemoveAt(index);
                            break;
                        }
                    }
                }
                            

                lstemployee = new ListEmployee(listSearchemployee);
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                rcvlistemployee.HasFixedSize = true;
                rcvlistemployee.SetItemViewCacheSize(50);
                rcvlistemployee.SetLayoutManager(mLayoutManager);
                employee_adapter_main = new Employee_Adapter_Main(lstemployee);
                rcvlistemployee.SetAdapter(employee_adapter_main);
                employee_adapter_main.ItemClick += Employee_adapter_main_ItemClick;

                if (employee_adapter_main.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(SearchName))
                    {
                        lnNoDataSearch.Visibility = ViewStates.Visible;
                        rcvlistemployee.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNoDataSearch.Visibility = ViewStates.Gone;
                        rcvlistemployee.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    rcvlistemployee.Visibility = ViewStates.Visible;
                }

                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetFilterEmployeeData at Employee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TextSearch_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterEmployeeData();
            }
            MainActivity.main_activity.CloseKeyboard(textSearch);
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
                textSearch.Text += input;
                textSearch.SetSelection(textSearch.Text.Length);
                return;
            }
        }

        private void TextSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetDataEmployee();
            }
            SetBtnSearch();
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
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
        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
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

        public void ReloadEmployee(ORM.MerchantDB.UserAccountInfo NewEmployee)
        {
            try
            {
                int index = 0;
                index = listEmployees.FindIndex(x => x.UserName.ToLower() == NewEmployee.UserName.ToLower());
                if (index > -1)
                {
                    listEmployees[index] = NewEmployee;
                    employee_adapter_main.NotifyItemChanged(index);
                    return;
                }

                listEmployees.Insert(0, NewEmployee);
                rcvlistemployee.SmoothScrollToPosition(0);
                employee_adapter_main.NotifyItemInserted(0);

                SetButtonAddReload();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void DeleteEmployee(ORM.MerchantDB.UserAccountInfo userAccountInfo)
        {
            try
            {
                int index = 0;
                index = listEmployees.FindIndex(x => x.UserName.ToLower() == userAccountInfo.UserName.ToLower());
                if (index == -1)
                {
                    return;
                }

                listEmployees.RemoveAt(index);
                employee_adapter_main.NotifyItemRemoved(index);

                //26/05/2566
                //ลบแล้วเช็คปุ่มเพิ่มพนักงาน                
                SetButtonAddReload();               
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetButtonAddReload()
        {
            //Owner ,Admin
            bool CheckDelete = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "employee");
            var checkInsert = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "employee");
            int userCurrent = DataCashingAll.UserAccountInfo.Count;
            int maxuser = DataCashingAll.GetGabanaInfo.TotalUser;
            if ((checkInsert && (userCurrent < maxuser)) && DataCashing.CheckNet)
            {
                btnAddEmployee.SetBackgroundResource(Resource.Mipmap.Add);
                btnAddEmployee.Enabled = true;
            }
            else
            {
                btnAddEmployee.SetBackgroundResource(Resource.Mipmap.AddMax);
                btnAddEmployee.Enabled = false;
            }
        }
    }
    public class ListEmpRole
    {
        public List<EmployeeRole> menuitems;
        static List<EmployeeRole> builitem;
        public ListEmpRole()
        {
            if (GabanaModel.gabanaMain.empRole != null)
            {
                builitem = GabanaModel.gabanaMain.empRole;
                this.menuitems = builitem;
            }
        }
        public int Count
        {
            get
            {
                return menuitems == null ? 0 : menuitems.Count;
            }
        }
        public EmployeeRole this[int i]
        {
            get { return menuitems == null ? null : menuitems[i]; }
        }
    }
}