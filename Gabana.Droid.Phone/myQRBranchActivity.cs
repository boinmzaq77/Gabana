using Android.App;
using Android.Content;
using Android.OS;
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
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class myQRBranchActivity : AppCompatActivity
    {
        public static myQRBranchActivity myqrbranchactivity;
        private static ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        private static myQRBranch_Adapter_Main myqrBranch_Adapter_Main;
        public static List<ORM.MerchantDB.Branch> branch;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        public static string branchSelect;
        public static List<ORM.MerchantDB.Branch> listChooseBranch = new List<Branch>();
        Button btnClear, btnApply;
        EditText txtSearch;
        string SearchName;
        ImageButton btnSearch;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.myqrbranch_activity_main);
                myqrbranchactivity = this;

                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearchTopping);
                btnSearch.Click += BtnSearch_Click;
                btnApply.Click += BtnApply_Click;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                btnClear = FindViewById<Button>(Resource.Id.btnClear);
                btnClear.Click += BtnClear_Click;

                txtSearch = FindViewById<EditText>(Resource.Id.textSearchTopping);
                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyPress += TxtSearch_KeyPress;
                txtSearch.FocusChange += TxtSearch_FocusChange;

                CheckJwt();
                GetListBranch();
                btnClear.Text = "Clear";
                if (listChooseBranch.Count == 1)
                {
                    branchSelect = listChooseBranch[0].BranchID;
                    btnClear.Text = "All";
                }
                else
                {
                    branchSelect = "All";
                    btnClear.Text = "Clear";
                    if (listChooseBranch.Count == 0)
                    {
                        listChooseBranch = lstBranch;
                    }
                }

                SetListBranch();
                SetBtnApply();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : myQRBranchActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at myQRBranch");
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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            SetListBranch();
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearch();
        }


        private void SetBtnApply()
        {

            if (listChooseBranch.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                GetListBranch();
            }
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

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (btnClear.Text != "All")
            {
                btnClear.Text = "All";
                branchSelect = "";
                listChooseBranch = new List<Branch>();

            }
            else
            {
                btnClear.Text = "Clear";
                branchSelect = "All";
                listChooseBranch = lstBranch;
            }
            SetListBranch();
            SetBtnApply();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseBranch.Count > 0)
            {
                AddmyQRActivity.selectbranch = branchSelect;
                AddmyQRActivity.ListChooseBranch = listChooseBranch;
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
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                BranchManage branchManage = new BranchManage();
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
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at qrBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetListBranch()
        {
            //BranchManage branchManage = new BranchManage();
            //lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            listBranch = new ListBranch(lstBranch);
            myqrBranch_Adapter_Main = new myQRBranch_Adapter_Main(listBranch, listChooseBranch);
            gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
            mRecycleView.SetLayoutManager(gridLayoutManager);
            mRecycleView.HasFixedSize = true;
            mRecycleView.SetItemViewCacheSize(20);
            mRecycleView.SetAdapter(myqrBranch_Adapter_Main);
            myqrBranch_Adapter_Main.ItemClick += SelectBranch_Adapter_Main_ItemClick;
            SetBtnApply();
        }

        private async void SelectBranch_Adapter_Main_ItemClick(object sender, int e)
        {
            try
            {
                if (listChooseBranch.Count == 0 || listChooseBranch.Count == 1 || listChooseBranch.Count == lstBranch.Count)
                {
                    listChooseBranch = new List<Branch>();
                    listChooseBranch.Add(lstBranch[e]);
                    branchSelect = lstBranch[e].BranchID;

                }
                if (listChooseBranch.Count > 0)
                {
                    btnClear.Text = "Clear";
                }
                if (listChooseBranch.Count == lstBranch.Count)
                {
                    btnClear.Text = "Clear";
                    branchSelect = "All";
                }

                SetListBranch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectBranch_Adapter_Main_ItemClick at myQRBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetListBranch();
                SetBtnApply();
                SetBtnSearch();
            }
        }

        private void TxtSearch_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
                txtSearch.ClearFocus();
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
                lstBranch = lstBranch.Where(x => x.BranchName.ToLower().Contains(SearchName.ToLower())).ToList();
                //lstBranch = await branchManage.GetBranchSearch(DataCashingAll.MerchantId, SearchName);
                listBranch = new ListBranch(lstBranch);
                myqrBranch_Adapter_Main = new myQRBranch_Adapter_Main(listBranch, listChooseBranch);
                gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(myqrBranch_Adapter_Main);
                myqrBranch_Adapter_Main.ItemClick += SelectBranch_Adapter_Main_ItemClick;
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at myQRBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'myQRBranchActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'myQRBranchActivity.openPage' is assigned but its value is never used
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

