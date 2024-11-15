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
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class BillFilterBranchActivity : AppCompatActivity
    {
        public static BillFilterBranchActivity selectbranchactivity;
        ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        ChooseBranch_Adapter_Main branch_Adapter_Main;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        List<Item> itemsChoose = new List<Item>();
        private static string userName;
        long branchSelect;
        EditText txtSearch;
        string SearchName;
        ImageButton btnSearch;
        Button btnApply;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.billfilter_activity_item);
                selectbranchactivity = this;

                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;
                btnApply.Click += BtnApply_Click;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtSearch = FindViewById<EditText>(Resource.Id.textSearch);
                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyPress += TxtSearch_KeyPress;
                txtSearch.FocusChange += TxtSearch_FocusChange;

                SetButtonApply();
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

        private void SetButtonApply()
        {
            if (itemsChoose.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
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

        private void TxtSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                //SetFilterBranchData();
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

        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                //GetListBranch();
            }
            SetBtnSearch();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (itemsChoose.Count > 0)
            {
                DataCashing.isModifyBranch = true;
                BillHistoryActivity.itemsChoose = itemsChoose;
                this.Finish();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

       
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                Gabana.Model.ResultAPI resultAPI = Utils.CheckNullValue();
                if (resultAPI.Status)
                {
                    if (resultAPI.Message == "login")
                    {
                        StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    }
                    else
                    {
                        StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    }
                    this.Finish(); return;
                }
                if (string.IsNullOrEmpty(SearchName))
                {
                    SetBtnSearch();
                }
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

       
    }
}

