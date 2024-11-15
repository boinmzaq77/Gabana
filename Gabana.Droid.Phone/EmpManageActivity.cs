using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Gabana.Droid.Adapter;
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
    public class EmpManageActivity : AppCompatActivity
    {
        public static EmpManageActivity empManage;
        RecyclerView recyclerview_listempmanage;
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees;
        ListEmployee lstemployee;
        LinearLayoutManager mLayoutManager;
        Empmanage_Adapter_Main empmanage_adapter_main;
        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        Switch switchActive;
        SwipeRefreshLayout refreshlayout;
        List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>();
        public static bool CheckNet = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.empmanage_activity);
                empManage = this;

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                recyclerview_listempmanage = FindViewById<RecyclerView>(Resource.Id.recyclerview_listempmanage);
                switchActive = FindViewById<Switch>(Resource.Id.switchActive);
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);

                refreshlayout.Refresh += (sender, e) =>
                {
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                CheckJwt();
                await GetData();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : EmpManageActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at EmployeeManage");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task GetData()
        {  
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            CheckNet = await GabanaAPI.CheckSpeedConnection();
            if (!CheckNet)
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                return;
            }

            if (DataCashingAll.Merchant?.UserAccountInfo == null)
            {
                DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
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

        private async void SetDataEmployee()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();
                listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();

                if (CheckNet)
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

                    var localUserAccount = await userAccountInfoManage.GetAllUserAccount();
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
                            UserName = item.UserName,
                            FUsePincode = DataCashingAll.Merchant?.UserAccountInfo.Where(x => x.UserName.ToLower() == item.UserName.ToLower()).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                            PinCode = DataCashingAll.Merchant?.UserAccountInfo.Where(x => x.UserName.ToLower() == item.UserName.ToLower()).Select(x => x.PinCode).FirstOrDefault(),
                            Comments = data?.userAccountInfo.Comments,
                        };
                        var insertlocal = await userAccountInfoManage.InsertorReplaceUserAccount(localUser);

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

                lstemployee = new ListEmployee(listEmployees);
                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
                recyclerview_listempmanage.HasFixedSize = true;
                recyclerview_listempmanage.SetLayoutManager(mLayoutManager);
                empmanage_adapter_main = new Empmanage_Adapter_Main(lstemployee);
                int count = lstemployee == null ? 0 : lstemployee.Count + 1;
                recyclerview_listempmanage.SetItemViewCacheSize(count);
                recyclerview_listempmanage.SetAdapter(empmanage_adapter_main);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }

        private async Task<List<ORM.MerchantDB.UserAccountInfo>> GetListEmployee()
        {
            try
            {
                listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                listEmployees = await userAccountInfoManage.GetAllUserAccount();
                if (listEmployees == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    return null;
                }
                Log.Debug("Employee", JsonConvert.SerializeObject(listEmployees));
                return listEmployees;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListEmployee at EmployeeManage");
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
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                await GetData();
                SetDataEmployee();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'EmpManageActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'EmpManageActivity.openPage' is assigned but its value is never used
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

