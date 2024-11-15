using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Adapter.Employee;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Fragments.Setting;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Myqr_Dialog_SelectBranch : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Myqr_Dialog_SelectBranch NewInstance()
        {
            var frag = new Myqr_Dialog_SelectBranch { Arguments = new Bundle() };
            return frag;
        }
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.myqr_dialog_selectbranch, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                GetListBranch();
                return view;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        RecyclerView mRecycleView;
        ImageButton btnSearch;
        LinearLayout lnBack;
        EditText txtSearch;
        public static string branchSelect;
        Button btnClear, btnApply;

        private void CombinUI()
        {

            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            mRecycleView = view.FindViewById<RecyclerView>(Resource.Id.rcvBranch);
            btnSearch.Click += BtnSearch_Click;
            btnApply.Click += BtnApply_Click;

            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;

            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnClear.Click += BtnClear_Click;

            txtSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyPress += TxtSearch_KeyPress;
            txtSearch.FocusChange += TxtSearch_FocusChange;

            SetButtonApply();
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
        private void SetBtnApply()
        {

            if (listChooseBranch.Count > 0)
            {
                btnApply.Enabled = true;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
        }
        private void SetButtonApply()
        {
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
        string SearchName;
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
        ListBranch listBranch;
        GridLayoutManager gridLayoutManager;
        public static List<ORM.MerchantDB.Branch> listChooseBranch = new List<Branch>();

        private async void SetFilterBranchData()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                lstBranch = await branchManage.GetBranchSearch(DataCashingAll.MerchantId, SearchName);
                listBranch = new ListBranch(lstBranch);
                MyQR_Adapter_Branch myqr_adapter_branch = new MyQR_Adapter_Branch(listBranch, listChooseBranch);
                gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(myqr_adapter_branch);
                myqr_adapter_branch.ItemClick += Myqr_adapter_branch_ItemClick; 
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        List<ORM.MerchantDB.Branch> lstBranch;
        private void Myqr_adapter_branch_ItemClick(object sender, int e)
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectBranch_Adapter_Main_ItemClick at myQRBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetListBranch()
        {
            try
            {

                listBranch = new ListBranch(lstBranch);
                MyQR_Adapter_Branch myqr_adapter_branch = new MyQR_Adapter_Branch(listBranch, listChooseBranch);
                gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(myqr_adapter_branch);
                myqr_adapter_branch.ItemClick += Myqr_adapter_branch_ItemClick;

                SetBtnApply();

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetListBranch at SelectBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void TxtSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
                txtSearch.ClearFocus();
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                MainActivity.main_activity.CloseKeyboard(view);
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

        private void LnBack_Click(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseBranch.Count > 0)
            {
                Setting_Fragment_AddMyQR.selectbranch = branchSelect;
                Setting_Fragment_AddMyQR.ListChooseBranch = listChooseBranch;
                this.Dialog.Dismiss();
            }
        }
        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearch();
        }
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        private async void GetListBranch()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                BranchManage branchManage = new BranchManage();
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at choosebranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}