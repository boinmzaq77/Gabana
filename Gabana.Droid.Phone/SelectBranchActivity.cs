using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SelectBranchActivity : AppCompatActivity
    {
        public static SelectBranchActivity selectbranchactivity;
        private static ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        private static SelectBranch_Adapter_Main selectBranch_Adapter_Main;
        public static List<ORM.MerchantDB.Branch> branch;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        public static string branchSelect;
        private Branch SelectBranch;
        string LoginType, Username;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.selectbranch_activity_main);
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

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            lnBack.PerformClick();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            MainDialog dialog = new MainDialog();
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.logout_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(SupportFragmentManager, myMessage);
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
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
                lstBranch = new List<ORM.MerchantDB.Branch>();

                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    if (await GabanaAPI.CheckSpeedConnection())
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
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at SelectBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetListBranch()
        {
            try
            {
                selectBranch_Adapter_Main = new SelectBranch_Adapter_Main(listBranch);
                gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
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

        protected  override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                GetListBranch();
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }

        bool deviceAsleep = false;
        bool openPage = false;
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
        }
    }
}

