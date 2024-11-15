using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Gabana.Droid.Tablet.Adapter.Employee;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Employee_Dialog_SelectBranch : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Employee_Dialog_SelectBranch NewInstance()
        {
            var frag = new Employee_Dialog_SelectBranch { Arguments = new Bundle() };
            return frag;
        }
        View view;
        ListBranch listBranch;
        List<ORM.MerchantDB.Branch> lstBranch;
        public Gabana3.JAM.Merchant.Merchants MerchantDetail;
        LinearLayout lnBack;
        GridLayoutManager gridLayoutManager;
        RecyclerView mRecycleView;
        public static List<BranchPolicy> listChooseBranch = new List<BranchPolicy>();
        private static string userName;
        long branchSelect;
        EditText txtSearch;
        string SearchName;
        ImageButton btnSearch;
        Button btnApply;
        List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.employee_dialog_selectbranch, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                GetListBranch();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Employee_Dialog_SelectBranch");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        private void CombinUI()
        {
            mRecycleView = view.FindViewById<RecyclerView>(Resource.Id.recyclerview_listbranch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            btnSearch.Click += BtnSearch_Click;
            btnApply.Click += BtnApply_Click;

            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click; ;

            txtSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyPress += TxtSearch_KeyPress;
            txtSearch.FocusChange += TxtSearch_FocusChange;

            SetButtonApply();
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

        private void LnBack_Click(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }

        internal static void SetBranch(List<BranchPolicy> l, string u)
        {
            listChooseBranch = l;
            userName = u;
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
                DataCashing.isModifyBranch = true;
                Employee_Fragment_AddEmployee.listChooseBranch = listChooseBranch;
                Employee_Fragment_AddEmployee.SetTextBranch();
                Employee_Fragment_AddEmployee.fragment_main.CheckDataChange();
                Dialog.Dismiss();
            }
        }

        private void SetClearSearchText()
        {
            SearchName = "";
            txtSearch.Text = string.Empty;
            SetBtnSearch();
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
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetListBranch at choosebranch");
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

        private async void SetFilterBranchData()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                lstBranch = await branchManage.GetBranchSearch(DataCashingAll.MerchantId, SearchName);
                listBranch = new ListBranch(lstBranch);
                Employee_Adapter_Branch employee_adapter_branch = new Employee_Adapter_Branch(listBranch);
                gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(employee_adapter_branch);
                employee_adapter_branch.ItemClick += Employee_adapter_branch_ItemClick; ;
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
        private void SetListBranch()
        {
            try
            {
                Employee_Adapter_Branch employee_adapter_branch = new Employee_Adapter_Branch(listBranch);
                gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                mRecycleView.SetLayoutManager(gridLayoutManager);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetItemViewCacheSize(20);
                mRecycleView.SetAdapter(employee_adapter_branch);
                employee_adapter_branch.ItemClick += Employee_adapter_branch_ItemClick;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetListBranch at SelectBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void Employee_adapter_branch_ItemClick(object sender, int e)
        {
            try
            {
                branchSelect = lstBranch[e].SysBranchID;
                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    SysBranchID = lstBranch[e].SysBranchID,
                    UserName = userName
                };

                if (Employee_Fragment_AddEmployee.positionEmp != "Admin" && Employee_Fragment_AddEmployee.positionEmp != "Owner" && Employee_Fragment_AddEmployee.positionEmp != "Manager")
                {
                    listChooseBranch = new List<BranchPolicy>();
                    listChooseBranch.Add(branchPolicy);
                }

                if (Employee_Fragment_AddEmployee.positionEmp == "Manager")
                {

                    var search = listChooseBranch.FindIndex(x => x.SysBranchID == lstBranch[e].SysBranchID && x.UserName.Contains(userName) && x.MerchantID == DataCashingAll.MerchantId);
                    if (search != -1)
                    {
                        listChooseBranch.RemoveAt(search);
                    }
                    else
                    {
                        listChooseBranch.Add(branchPolicy);
                    }
                }

                listBranch = new ListBranch(lstBranch);
                SetListBranch();
                SetButtonApply();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectBranch_Adapter_Main_ItemClick at chooseBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }

        }
    }
}