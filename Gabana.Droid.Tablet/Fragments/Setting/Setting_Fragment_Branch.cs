
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
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
using Gabana.Droid.Helper;
using Gabana.Droid.Tablet.Helper;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Branch : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Fragment_Branch NewInstance()
        {
            Setting_Fragment_Branch frag = new Setting_Fragment_Branch();
            return frag;
        }

        View view;
        public static Setting_Fragment_Branch fragment_main;
        string LoginType;
        bool flagLoadData = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_branch, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                flagLoadData = true;
                LoginType = Preferences.Get("LoginType", "");
                UserLogin = Preferences.Get("User", "");
                //_ = GetListBranch();
                var Width = 130;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this.Activity, rcvBranch, (int)Width);

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Branch");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }
        public async override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            if (flagLoadData)
            {
                if (!DataCashing.CheckNet)
                {
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                }
                else
                {
                    await GetGabanaInfo();
                }
                await GetListBranch();
                SetListBranch();
                flagLoadData = false;
            }

            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "branch");
            if (check && (lstBranch.Count < DataCashingAll.GetGabanaInfo.TotalBranch))
            {
                btnAddBranch.SetBackgroundResource(Resource.Mipmap.Add);
                btnAddBranch.Enabled = true;
            }
            else
            {
                btnAddBranch.SetBackgroundResource(Resource.Mipmap.AddMax);
                btnAddBranch.Enabled = false;
            }
        }        

        RecyclerView rcvBranch;
        SwipeRefreshLayout refreshlayout;
        private async void BtnAddBranch_Click(object sender, EventArgs e)
        {
            //Owner
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "branch");
            if (!check)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }

            if (lstBranch.Count < DataCashingAll.GetGabanaInfo.TotalBranch) //branch มีได้สูงสุด 500 สาขาและสวามารถเพิ่มได้ตามจำนวนแพ็กเกจที่เลือก
            {
                DataCashing.EditBranch = null;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addbranch");
            }
            else
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.maxbranch), ToastLength.Short).Show();
            }
        }

        List<ORM.MerchantDB.Branch> lstBranch;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        BranchManage branchManage = new BranchManage();       
        public static ListBranch listBranch;
        Setting_Adapter_Branch setting_adapter_branch;
        GridLayoutManager gridLayoutManager;
        private async Task GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            dialogLoading.Cancelable = false;
            dialogLoading.Show(Activity.SupportFragmentManager, nameof(DialogLoading));
            try
            {
                List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
                lstBranch = new List<ORM.MerchantDB.Branch>();
                if (DataCashing.CheckNet)
                {
                    cloudbranch = await GabanaAPI.GetDataBranch();
                    if (cloudbranch == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    if (cloudbranch.Count == 0)
                    {
                        lstBranch = new List<ORM.MerchantDB.Branch>();
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

                SetListBranch();
                dialogLoading.Dismiss();
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Branch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetListBranch()
        {
            listBranch = new ListBranch(lstBranch);
            setting_adapter_branch = new Setting_Adapter_Branch(listBranch);
            gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
            rcvBranch.SetLayoutManager(gridLayoutManager);
            rcvBranch.HasFixedSize = true;
            rcvBranch.SetItemViewCacheSize(50);
            rcvBranch.SetAdapter(setting_adapter_branch);
            setting_adapter_branch.ItemClick += Setting_adapter_branch_ItemClick; 
        }

        ORM.MerchantDB.Branch selectBranch;
        BranchPolicyManage policyManage = new BranchPolicyManage();
        string UserLogin;

        private async void Setting_adapter_branch_ItemClick(object sender, int e)
        {
            try
            {
                //Role = Manager จะแก้ไข้ข้อมูลได้เฉพาะสาขาของตัวเองเท่านั้น
                List<BranchPolicy> branchPolicies = new List<BranchPolicy>();
                selectBranch = listBranch[e];
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "update", "branch");               
                if (!check)
                {
                    branchPolicies = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, UserLogin.ToLower());
                    if (branchPolicies != null && branchPolicies.Count > 0)
                    {
                        var index = branchPolicies.FindIndex(x => x.SysBranchID == selectBranch.SysBranchID);
                        if (index != -1)
                        {
                            Setting_Fragment_BranchDetail.branchEdit = selectBranch;
                            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addbranch");
                        }
                        else
                        {
                            Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                        }
                    }
                    return;
                }

                DataCashing.EditBranch = selectBranch;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addbranch");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Branch_Adapter_Main_ItemClick at Branch");
            }
        }

        ImageButton btnAddBranch;
        LinearLayout lnBack;
        private void ComBineUI()
        {
            rcvBranch = view.FindViewById<RecyclerView>(Resource.Id.rcvBranch);
            btnAddBranch = view.FindViewById<ImageButton>(Resource.Id.btnAddBranch);
            btnAddBranch.Click += BtnAddBranch_Click;
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            refreshlayout.Refresh += async (sender, e) =>
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
                else
                {
                    flagLoadData = true;
                    OnResume();
                }
                BackgroundWorker work = new BackgroundWorker();
                work.DoWork += Work_DoWork;
                work.RunWorkerCompleted += Work_RunWorkerCompleted;
                work.RunWorkerAsync();
            };
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
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

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            Setting_Fragment_Main.SetEnableBtnBranch();
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        public void ReloadBranch(Branch NewBranch)
        { 
            try
            {
                int index = 0;
                index = lstBranch.FindIndex(x => x.SysBranchID == NewBranch.SysBranchID);
                if (index > -1)
                {
                    lstBranch[index] = NewBranch;
                    setting_adapter_branch.NotifyItemChanged(index);
                    return;
                }

                lstBranch.Insert(0, NewBranch);
                rcvBranch.SmoothScrollToPosition(0);
                setting_adapter_branch.NotifyItemInserted(0);

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private class MyImplementSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public MyImplementSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
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
                    Resource.Mipmap.DeleteBt2,
                    "#33AAE1",
                    new MyDeleteButtonClick(this)));
            }

            private class MyDeleteButtonClick : MyButtonClickListener
            {
                private MyImplementSwipeHelper myImplementSwipeHelper;

                public MyDeleteButtonClick(MyImplementSwipeHelper myImplementSwipeHelper)
                {
                    this.myImplementSwipeHelper = myImplementSwipeHelper;
                }
                public void OnClick(int position)
                {
                    try
                    {
                        DataCashing.EditBranch = listBranch[position];
                        var fragment = new Branch_Dialog_Delete();
                        fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Branch_Dialog_Delete));
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at giftvoucher");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }


    }
}
