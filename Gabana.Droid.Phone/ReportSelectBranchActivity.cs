using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportSelectBranchActivity : AppCompatActivity
    {
        public static ReportSelectBranchActivity selectbranchactivity;
        private ListBranch listBranch;
        List<Branch> lstBranch;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack, lnNoDataSearch;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        public static List<Branch> listChooseBranch = new List<Branch>();
        public static string branchSelect;
        EditText txtSearch;
        string SearchName, emplogin, LoginType;
        ImageButton btnSearch;
        Button btnApply, btnAll;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        SwipeRefreshLayout refreshlayout;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_choosebranch);
                selectbranchactivity = this;

                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;
                btnApply.Click += BtnApply_Click;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                btnAll.Click += BtnAll_Click;

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                CheckJwt();
                txtSearch = FindViewById<EditText>(Resource.Id.textSearch);
                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyPress += TxtSearch_KeyPress;
                txtSearch.FocusChange += TxtSearch_FocusChange;
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout); 
                refreshlayout.Refresh += (sender, e) =>
                {
                    OnResume();
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };
                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportSelectBranchActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at chooseBranch");
                Log.Debug("Error", ex.Message);
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

        private void BtnAll_Click(object sender, EventArgs e)
        {

            if (listChooseBranch.Count != lstBranch.Count)
            {
                branchSelect = "All Branch";
                listChooseBranch = lstBranch;
                GetListBranch();
            }
            else
            {
                listChooseBranch = new List<Branch>();
                branchSelect = "";
                foreach (var item in listChooseBranch)
                {
                    if (branchSelect != "" && branchSelect != "All Branch")
                    {
                        branchSelect += "," + item.BranchName;

                    }
                    else
                    {
                        branchSelect = item.BranchName;
                    }
                }
                SetListBranch();
            }

        }

        private void SetShowButton()
        {

            if (lstBranch.Count == listChooseBranch.Count)
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseBranch.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            GetListBranch();
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearchItem();
        }

        private void SetBtnSearchItem()
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

        private void TxtSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearchItem();
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

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                GetListBranch();
            }
            SetBtnSearchItem();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseBranch.Count > 0)
            {
                DataCashing.isModifyBranch = true;
                ReportDailySaleActivity.listChooseBranch = listChooseBranch;
                this.Finish();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private async void GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                BranchManage branchManage = new BranchManage();
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

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
                    var lstuserBranch = await branchPolicyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, emplogin.ToLower());
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
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at Report Select Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetListBranch()
        {
            try
            {
                Report_Adapter_ChooseBranch report_adapter_choosebranch = new Report_Adapter_ChooseBranch(listBranch, listChooseBranch);
                gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(report_adapter_choosebranch);
                report_adapter_choosebranch.ItemClick += Report_adapter_choosebranch_ItemClick;

                if (report_adapter_choosebranch.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    mRecycleView.Visibility = ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    mRecycleView.Visibility = ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                }

                SetShowButton();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetGetListBranch at SelectBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void Report_adapter_choosebranch_ItemClick(object sender, int e)
        {
            Branch branch = new Branch();
            branch = lstBranch[e];
            List<Branch> branches = new List<Branch>();
            branches = lstBranch;
            var index = listChooseBranch.FindIndex(x => x.SysBranchID == branch.SysBranchID);
            if (index == -1)
            {
                listChooseBranch.Add(lstBranch[e]);
            }
            else
            {
                listChooseBranch.RemoveAt(index);
            }
            SetListBranch();
        }
        protected  override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                if (string.IsNullOrEmpty(SearchName))
                {
                    GetListBranch();
                    SetBtnSearchItem();
                    SetShowButton();
                }
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }
        private async void SetFilterBranchData()
        {
            try
            {
                lstBranch = lstBranch.Where(x => x.MerchantID == DataCashingAll.MerchantId & x.BranchName.Contains(SearchName) & x.Status == 'A').OrderBy(x => x.BranchName).ToList();
                listBranch = new ListBranch(lstBranch);
                SetListBranch();
                SetBtnSearchItem();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportSelectBranchActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportSelectBranchActivity.openPage' is assigned but its value is never used
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

