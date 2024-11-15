using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class SelectBranchActivity : AppCompatActivity
    {
        SelectBranchActivity selectbranchactivity;
        RecyclerView mRecycleView;
        public static string branchSelect;
        string LoginType, Username;
        private Branch SelectBranch;
        LinearLayout lnBack;
        DialogLoading dialogLoading = new DialogLoading();
        List<ORM.MerchantDB.Branch> lstBranch;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        private static ListBranch listBranch;
        private static Login_Adapter_SelectBranch selectBranch_Adapter_Main;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.login_activity_selectbranch);
                selectbranchactivity = this;

                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                Button btnMain = FindViewById<Button>(Resource.Id.btnMain);
                branchSelect = Preferences.Get("Branch", "");
                LoginType = Preferences.Get("LoginType", "");
                Username = Preferences.Get("User", "");
                btnMain.Click += BtnMain_Click;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at SelectBranch");
                Log.Debug("Error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            Bundle bundle = new Bundle();
            var fragement = new Logout_Dialog_Main();
            fragement.Arguments = bundle;
            fragement.Show(this.SupportFragmentManager, nameof(Logout_Dialog_Main));
        }
        private void BtnMain_Click(object sender, EventArgs e)
        {
            if (branchSelect != string.Empty)
            {
                DataCashingAll.SysBranchId = Convert.ToInt32(branchSelect);
                Preferences.Set("Branch", branchSelect);
                DataCashing.branchDeatail = SelectBranch;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, "กรุณาเลือกสาขา", ToastLength.Short).Show();
            }
        }
        private async void GetListBranch()
        {
            try
            {
                List<Branch> getbranch = new List<Branch>();
                BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
                BranchManage branchManage = new BranchManage();
                dialogLoading.Cancelable = false;
                dialogLoading.Show(SupportFragmentManager, nameof(DialogLoading));
                List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
                lstBranch = new List<ORM.MerchantDB.Branch>();

                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    if (await GabanaAPI.CheckNetWork())
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
                            Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                        }
                    }
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
                        lstBranch = new List<Branch>();
                        lstBranch.AddRange(getbranch);
                    }
                    else
                    {
                        lstBranch = new List<Branch>();
                    }
                }

                listBranch = new ListBranch(lstBranch);
                SetListBranch();
                dialogLoading.Dismiss();
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at SelectBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        GridLayoutManager gridLayoutManager;
        private void SetListBranch()
        {
            try
            {
                selectBranch_Adapter_Main = new Login_Adapter_SelectBranch(listBranch);
                gridLayoutManager = new GridLayoutManager(this, 2, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(selectBranch_Adapter_Main);
                selectBranch_Adapter_Main.ItemClick += SelectBranch_Adapter_Main_ItemClick;
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at SelectBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void SelectBranch_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                branchSelect = lstBranch[e].BranchID;
                SelectBranch = lstBranch[e];
                SetListBranch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectBranch_Adapter_Main_ItemClick at SelectBranch");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                GetListBranch();
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }
    }

}