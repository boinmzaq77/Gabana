using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Adapter.Report;
using Android.Views.InputMethods;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Branch : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public static Report_Dialog_Branch NewInstance()
        {
            var frag = new Report_Dialog_Branch { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_branch, container, false);
            try
            {
                CombinUI();
                SetUIEvent();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        public override void OnResume()
        {
            base.OnResume();
            if (string.IsNullOrEmpty(SearchName))
            {
                GetListBranch();
                SetBtnSearchItem();
                SetShowButton();
            }

        }
        LinearLayout lnBack;
        FrameLayout lnSearch;
        ImageButton btnSearch;
        EditText textSearch;
        Button btnAll, btnApply;
        SwipeRefreshLayout swipRefresh;
        RecyclerView rcvBranch;
        LinearLayout lnNoDataSearch;
        private void SetUIEvent()
        {
            btnSearch.Click += BtnSearch_Click;
            btnApply.Click += BtnApply_Click;

            lnBack.Click += LnBack_Click;
            btnBack.Click += LnBack_Click;
            btnAll.Click += BtnAll_Click;

            emplogin = Preferences.Get("User", "");
            LoginType = Preferences.Get("LoginType", "");

            textSearch.TextChanged += TxtSearch_TextChanged;
            textSearch.KeyPress += TxtSearch_KeyPress;
            textSearch.FocusChange += TxtSearch_FocusChange;
        }
        private void TxtSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearchItem();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    MainActivity.main_activity.CloseKeyboard(view);
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
                textSearch.Text += input;
                textSearch.SetSelection(textSearch.Text.Length);
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
                SetBtnSearchItem();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void TxtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                GetListBranch();
            }
            SetBtnSearchItem();
        }

        ImageButton btnBack;
        string SearchName, emplogin, LoginType;
        private void CombinUI()
        {
            btnBack = view.FindViewById<ImageButton>(Resource.Id.btnBack);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearch = view.FindViewById<FrameLayout>(Resource.Id.lnSearch);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            btnAll = view.FindViewById<Button>(Resource.Id.btnAll);
            swipRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipRefresh);
            rcvBranch = view.FindViewById<RecyclerView>(Resource.Id.rcvBranch);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
        }
        public static string branchSelect;
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
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        public static List<Branch> listChooseBranch = new List<Branch>();
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseBranch.Count > 0)
            {
                DataCashing.isModifyBranch = true;
                Report_Dialog_Custom.listChooseBranch = listChooseBranch;
                await Report_Dialog_Custom.dialog_Custom.GetBranchSelect();
                this.Dialog.Dismiss();
            }
        }
        private void TxtSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
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
            textSearch.Text = string.Empty;
            SetBtnSearchItem();
        }
        List<Branch> lstBranch;
        private ListBranch listBranch;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        private async void GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                BranchManage branchManage = new BranchManage();
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    List<ORM.MerchantDB.Branch> localbranch = new List<ORM.MerchantDB.Branch>();
                    lstBranch = new List<ORM.MerchantDB.Branch>();
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
                            Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }

        }
        GridLayoutManager gridLayoutManager;
        private void SetListBranch()
        {
            try
            {
                Report_Adapter_ChooseBranch report_adapter_choosebranch = new Report_Adapter_ChooseBranch(listBranch, listChooseBranch);
                gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvBranch.SetLayoutManager(gridLayoutManager);
                rcvBranch.HasFixedSize = true;
                rcvBranch.SetItemViewCacheSize(20);
                rcvBranch.SetAdapter(report_adapter_choosebranch);
                report_adapter_choosebranch.ItemClick += Report_adapter_choosebranch_ItemClick; ;

                if (report_adapter_choosebranch.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    rcvBranch.Visibility = ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    rcvBranch.Visibility = ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                }

                SetShowButton();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetGetListBranch at SelectBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

        private void SetShowButton()
        {

            if (lstBranch.Count == listChooseBranch.Count)
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseBranch.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
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
    }

}