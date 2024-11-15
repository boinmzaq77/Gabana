using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
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
    public class DashboardBranchActivity : AppCompatActivity
    {
        public static DashboardBranchActivity dashboardbranchactivity;
        private static ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        private static DashBoardBranch_Adapter_Main dachboardBranch_Adapter_Main;
        public static List<ORM.MerchantDB.Branch> branch;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack, lnNoDataSearch;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        public static string branchSelect;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        EditText txtSearch;
        string SearchName;
        ImageButton btnSearch;
        string LoginType, Username;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.choosebranch_activity_main);
                dashboardbranchactivity = this;

                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                Button btnApply = FindViewById<Button>(Resource.Id.btnApply);
                branchSelect = DashboardActivity.branchID;

                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;
                btnApply.Click += BtnApply_Click;

                LoginType = Preferences.Get("LoginType", "");
                Username = Preferences.Get("User", "");
                CheckJwt();
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtSearch = FindViewById<EditText>(Resource.Id.textSearch);
                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyPress += TxtSearch_KeyPress;
                txtSearch.FocusChange += TxtSearch_FocusChange;

                
                _ = TinyInsights.TrackPageViewAsync("OnCreate : DashboardBranchActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at DSashboardBranch");
                Log.Debug("Error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        private void TxtSearch_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearch();

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
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

        private async void SetFilterBranchData()
        {
            try
            {
                lstBranch = lstBranch.Where(x => x.MerchantID == DataCashingAll.MerchantId & x.BranchName.Contains(SearchName) & x.Status == 'A').OrderBy(x => x.BranchName).ToList();
                listBranch = new ListBranch(lstBranch);
                SetListBranch();
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchName = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                GetListBranch();
            }
            SetBtnSearch();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearch();
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearch.Enabled = false;
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Enabled = true;
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (branchSelect != string.Empty)
            {
                DashboardActivity.branchID = branchSelect;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.selectbrach), ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private async void GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                BranchManage branchManage = new BranchManage();
               
                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
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
                }
                else
                {
                    List<Branch> getbranch = new List<Branch>();
                    BranchPolicyManage branchPolicyManage = new BranchPolicyManage();
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
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Dashboard");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetListBranch()
        {
            try
            {
                //listBranch = new ListBranch(lstBranch);
                dachboardBranch_Adapter_Main = new DashBoardBranch_Adapter_Main(listBranch);
                gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(dachboardBranch_Adapter_Main);
                dachboardBranch_Adapter_Main.ItemClick += SelectBranch_Adapter_Main_ItemClick;

                if (dachboardBranch_Adapter_Main.ItemCount == 0)
                {
                    mRecycleView.Visibility = ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    mRecycleView.Visibility = ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetListBranch at SelectBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void SelectBranch_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                branchSelect = lstBranch[e].BranchID;
                SetListBranch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SelectBranch_Adapter_Main_ItemClick at DSashboardBranch");
                return;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override void OnResume()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
#pragma warning disable CS0414 // The field 'DashboardBranchActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'DashboardBranchActivity.openPage' is assigned but its value is never used
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

