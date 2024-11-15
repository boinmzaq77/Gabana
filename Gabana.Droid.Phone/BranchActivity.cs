using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.Helper;
using Gabana.Droid.ListData;
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
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class BranchActivity : AppCompatActivity
    {
        public static BranchActivity branchactivity;
        public static ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        Branch_Adapter_Main branch_Adapter_Main;
        ImageButton btnAddBranch;
        LinearLayout lnBack;
        RecyclerView mRecycleView;
        GridLayoutManager gridLayoutManager;
        ORM.MerchantDB.Branch selectBranch;
        string emplogin;
        string LoginType;
        SwipeRefreshLayout refreshlayout;
        BranchManage branchManage = new BranchManage();
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        public static bool flagLoadData = false;
        BranchPolicyManage policyManage = new BranchPolicyManage();
        string branchID;
        bool checkNet = false;
        public static Branch FocusBranch;
        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'BranchActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'BranchActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.branch_activity);
                branchactivity = this;

                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                CheckJwt();

                //GetListBranch();
                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                btnAddBranch = FindViewById<ImageButton>(Resource.Id.btnAddBranch);
                btnAddBranch.Click += BtnAddBranch_Click;
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.SetSlingshotDistance(5);

                refreshlayout.Refresh += async (sender, e) =>
                {
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
                        flagLoadData = true;
                        OnResume();
                    }                    
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;

                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "branch");
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, mRecycleView, (int)Width);
                flagLoadData = true;
                branchID = Preferences.Get("Branch", "");
                btnAddBranch.Enabled = false;
                _ = TinyInsights.TrackPageViewAsync("OnCreate : BranchActivity");

            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);                
                _= TinyInsights.TrackPageViewAsync("Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
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

        //แก้ไข branch
        private async void Branch_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                //Role = Manager จะแก้ไข้ข้อมูลได้เฉพาะสาขาของตัวเองเท่านั้น
                List<BranchPolicy> branchPolicies = new List<BranchPolicy>();
                selectBranch = listBranch[e];
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "update", "branch");
                if (!check)
                {
                    branchPolicies = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, emplogin.ToLower());
                    if (branchPolicies != null && branchPolicies.Count > 0)
                    {
                        var index = branchPolicies.FindIndex(x => x.SysBranchID == selectBranch.SysBranchID);
                        if (index == -1)
                        {
                            Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                            return;
                        }
                    }                    
                }

                StartActivity(new Intent(Application.Context, typeof(AddBranchActivity)));
                AddBranchActivity.branchEdit = selectBranch;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Branch_Adapter_Main_ItemClick at Branch");
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FocusBranch = null;
        }

        private void BtnAddBranch_Click(object sender, EventArgs e)
        {
            btnAddBranch.Enabled = false;
            //Owner
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "branch");            
            if (!check)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                btnAddBranch.Enabled = true;
                return;
            }

            if (lstBranch.Count < DataCashingAll.GetGabanaInfo.TotalBranch) //branch มีได้สูงสุด 500 สาขาและสวามารถเพิ่มได้ตามจำนวนแพ็กเกจที่เลือก
            {
                AddBranchActivity.branchEdit = null;
                StartActivity(new Intent(Application.Context, typeof(AddBranchActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.maxbranch), ToastLength.Short).Show();
            }
            btnAddBranch.Enabled = true;
        }

        private async Task GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
                lstBranch = new List<ORM.MerchantDB.Branch>();
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
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetListBranch()
        {
            try
            {
                listBranch = new ListBranch(lstBranch);
                branch_Adapter_Main = new Branch_Adapter_Main(listBranch);
                gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(50);
                mRecycleView.SetAdapter(branch_Adapter_Main);
                branch_Adapter_Main.ItemClick += Branch_Adapter_Main_ItemClick;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                checkNet = await GabanaAPI.CheckSpeedConnection();                                
                
                if (flagLoadData)
                {
                    if (!checkNet)
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

                BranchFocus();

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
            catch (Exception)
            {
                btnAddBranch.Enabled = true;
            }
        }

        private void BranchFocus()
        {
            try
            {
                if (FocusBranch != null)
                {
                    int index = -1;
                    if (lstBranch != null)
                    {
                        index = lstBranch.FindIndex(x=>x.SysBranchID == FocusBranch.SysBranchID);
                        if (index != -1)
                        {
                            lstBranch.RemoveAt(index);
                        }
                        lstBranch.Insert(0,FocusBranch);
                    }
                    branch_Adapter_Main.NotifyDataSetChanged();
                    FocusBranch = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BranchFocus at Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        internal static void SetFocusBranch(Branch branch)
        {
            try
            {
                FocusBranch = branch;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusItem at Item");
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
                    Resource.Mipmap.DeleteBt,
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
                    #region oldcode
                    //try
                    //{
                    //    string branch = Preferences.Get("Branch", "");
                    //    int branchID = Convert.ToInt32(branch);
                    //    BranchManage BranchManage = new BranchManage();
                    //    var branchdelete = listBranch[position];
                    //    if (branchdelete.SysBranchID == 1)
                    //    {
                    //        Toast.MakeText(myImplementSwipeHelper.context, $"Can't delete Head Office", ToastLength.Short).Show();
                    //    }
                    //    else if (branchdelete.SysBranchID == branchID)
                    //    {
                    //        Toast.MakeText(myImplementSwipeHelper.context, $"ไม่สามารถลบข้อมูลได้ เนื่องจาก ใช้งานสาขาที่เลือกอยู่", ToastLength.Short).Show();
                    //    }
                    //    else
                    //    {
                    //        var DeleteonCloud = await GabanaAPI.DeleteDataBranch((int)branchdelete.SysBranchID);
                    //        if (DeleteonCloud.Status)
                    //        {
                    //            var DeleteonLocal = await BranchManage.DeleteBranch(DataCashingAll.MerchantId, (int)branchdelete.SysBranchID);
                    //            if (DeleteonLocal)
                    //            {
                    //                Toast.MakeText(myImplementSwipeHelper.context, $"ลบสำเร็จ", ToastLength.Short).Show();
                    //                MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                    //                var deleteqr = await myQrCodeManage.DeleteMyQrCodefromBranch(DataCashingAll.MerchantId, (int)AddBranchActivity.SetDetailBranch.SysBranchID);
                    //                BranchActivity.branchactivity.OnResume();
                    //            }
                    //            else
                    //            {
                    //                Toast.MakeText(myImplementSwipeHelper.context, $"ลบไม่สำเร็จ", ToastLength.Short).Show();
                    //            }
                    //            BranchActivity.branchactivity.OnResume();

                    //        }
                    //        else
                    //        {
                    //            Toast.MakeText(myImplementSwipeHelper.context, "ลบไม่สำเร็จ", ToastLength.Short).Show();
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Toast.MakeText(myImplementSwipeHelper.context, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                    //    return;
                    //} 
                    #endregion

                    try
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("deleteType", "branch");
                        bundle.PutInt("systemID", (int)listBranch[position].SysBranchID);
                        bundle.PutString("fromPage", "main");
                        dialog.Arguments = bundle;
                        dialog.Show(branchactivity.SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at Branch");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
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

