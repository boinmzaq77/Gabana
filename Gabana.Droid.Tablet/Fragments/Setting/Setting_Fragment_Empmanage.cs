using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter;
using AndroidX.SwipeRefreshLayout.Widget;
using System.ComponentModel;
using System.Threading;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Empmanage : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Fragment_Empmanage NewInstance()
        {
            Setting_Fragment_Empmanage frag = new Setting_Fragment_Empmanage();
            return frag;
        }
        View view;
        public static Setting_Fragment_Empmanage fragment_main;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_empmanage, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();               
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        private async Task GetData()
        {
            DialogLoading dialogLoading = new DialogLoading();            
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(this.Activity.SupportFragmentManager, nameof(DialogLoading));
                }                
                
                if (DataCashing.CheckNet)
                {
                    if (DataCashingAll.Merchant?.UserAccountInfo == null)
                    {
                        DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    }
                }
                await GetDataEmployee();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GetDataEmployee()
        {
            try
            {
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();
                listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();

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

                SetDataEmployee();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" GetDataEmployee at EmployeeManage");
                return;
            }
        }

        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees;
        List<Gabana.ORM.Master.BranchPolicy> getlstbranchPolicy = new List<ORM.Master.BranchPolicy>();
        ListEmployee lstemployee;
        LinearLayoutManager mLayoutManager;
        RecyclerView recyclerview_listempmanage;
        Setting_Adapter_Empmanage setting_adapter_empmanage;

        private void SetDataEmployee()
        {
            try
            {
                lstemployee = new ListEmployee(listEmployees);
                mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
                recyclerview_listempmanage.HasFixedSize = true;
                recyclerview_listempmanage.SetLayoutManager(mLayoutManager);
                setting_adapter_empmanage = new Setting_Adapter_Empmanage(lstemployee);
                int count = lstemployee == null ? 0 : lstemployee.Count + 1;
                recyclerview_listempmanage.SetItemViewCacheSize(count);
                recyclerview_listempmanage.SetAdapter(setting_adapter_empmanage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
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


        LinearLayout lnBack;
        SwipeRefreshLayout swRefresh;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            recyclerview_listempmanage = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_listempmanage);

            lnBack.Click += LnBack_Click;
            swRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swRefresh);
            swRefresh.Refresh += (sender, e) =>
            {
                OnResume();
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };

        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            swRefresh.Refreshing = false;
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            Setting_Fragment_Main.SetEnableBtnMerchant();
            MainActivity.main_activity.SupportFragmentManager.BeginTransaction().Remove(MainActivity.main_activity.activeR).Commit();
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
                await GetData();
            }
            catch (Exception ex)
            {

                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at Settinr fragment empmanage");
            }
        }
    }
}