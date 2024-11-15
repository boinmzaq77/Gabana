using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Report;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Employee : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Report_Dialog_Employee NewInstance()
        {
            var frag = new Report_Dialog_Employee { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_employee, container, false);
            try
            {
                CombinUI();
                SetUIEvent();

                if (string.IsNullOrEmpty(searchEmployee))
                {
                    SetEmployeeData();
                    SetBtnSearch();
                }

                SetShowButton();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Report Dialog Employee");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            textSearch.TextChanged += TxtSearchEmp_TextChanged;
            textSearch.FocusChange += TxtSearchEmp_FocusChange;
            textSearch.KeyPress += TxtSearchEmp_KeyPress;
            btnSearch.Click += BtnSearch_Click;
            btnApply.Click += BtnApply_Click;
            btnAll.Click += BtnAll_Click;

        }
        private void BtnAll_Click(object sender, EventArgs e)
        {
            if (employeeSelect != "All Employee" && employeeSelect == "")
            {
                employeeSelect = "All Employee";
                listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                listChooseEmployee = listEmployees;
            }
            else
            {
                listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                employeeSelect = "";
            }
            SetShowButton();
            lstemployee = new ListEmployee(listEmployees);
            Report_Adapter_Employee report_adapter_employee = new Report_Adapter_Employee(lstemployee);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity ,1, 1, false);
            rcvEmployee.SetAdapter(report_adapter_employee);
            rcvEmployee.HasFixedSize = true;
            rcvEmployee.SetLayoutManager(mLayoutManager);
            report_adapter_employee.ItemClick += Report_adapter_employee_ItemClick;
        }
        internal static void SetSelectEmp(List<ORM.MerchantDB.UserAccountInfo> l, List<ORM.MerchantDB.UserAccountInfo> e)
        {
            listChooseEmployee = l;
            listEmployees = e;
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }
        string SearchName;
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearch.Text = string.Empty;
            SetBtnSearch();
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseEmployee.Count > 0)
            {
                Report_Dialog_Custom.listChooseEmployee = listChooseEmployee;
                await Report_Dialog_Custom.dialog_Custom.SetDataEmployee();
                this.Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, "กรุณาเลือกพนักงาน", ToastLength.Short).Show();
            }
        }
        private void TxtSearchEmp_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }
        private void TxtSearchEmp_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterEmployeeData();
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
        private void SetFilterEmployeeData()
        {
            try
            {
                listSearchEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                listSearchEmployee = listEmployees.Where(x => x.UserName.ToLower().Contains(searchEmployee)).ToList();
                if (listSearchEmployee.Count > 0)
                {
                    listSearchEmployee = listSearchEmployee.Where(x => DataCashingAll.UserAccountInfo.Select(x => x.UserName.ToLower()).ToList().Contains(x.UserName.ToLower())).ToList();
                }

                SetShowButton();
                lstemployee = new ListEmployee(listSearchEmployee);
                Report_Adapter_Employee report_adapter_employee = new Report_Adapter_Employee(lstemployee);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_employee);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_employee.ItemClick += Report_adapter_employee_ItemClick; 


                if (report_adapter_employee.ItemCount == 0 && !string.IsNullOrEmpty(searchEmployee))
                {
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                    rcvEmployee.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                    rcvEmployee.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterEmployeeData at ReportEmployee");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        string searchEmployee;

        private void TxtSearchEmp_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchEmployee = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchEmployee))
            {
                SetEmployeeData();
            }
            SetBtnSearch();
        }
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees, listSearchEmployee;
        public static List<ORM.MerchantDB.UserAccountInfo> listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        private static ListEmployee lstemployee;
        async void SetEmployeeData()
        {
            try
            {
                lstemployee = new ListEmployee(listEmployees);
                if (listChooseEmployee.Count == 0)
                {
                    listChooseEmployee = listEmployees;
                }
                //listChooseEmployee = listEmployees;
                Report_Adapter_Employee report_adapter_customer = new Report_Adapter_Employee(lstemployee);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_customer);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_adapter_employee_ItemClick;            
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetEmployeeData at ReportEmp");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private string employeeSelect;

        private void Report_adapter_employee_ItemClick(object sender, int e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {

                var employee = lstemployee[e];
                var search = listChooseEmployee.FindIndex(x => x.UserName == employee.UserName && x.MerchantID == DataCashingAll.MerchantId);
                if (search != -1)
                {
                    listChooseEmployee.RemoveAt(search);
                }
                else
                {
                    listChooseEmployee.Add(employee);
                }

                employeeSelect = "";

                if (listEmployees.Count == listChooseEmployee.Count)
                {
                    employeeSelect = "All Employee";
                }
                else
                {
                    foreach (var item in listChooseEmployee)
                    {
                        if (employeeSelect != "")
                        {
                            employeeSelect += "," + item.UserName;
                        }
                        else
                        {
                            employeeSelect = item.UserName;
                        }
                    }
                }

                SetShowButton();

                Report_Adapter_Employee report_adapter_employee = new Report_Adapter_Employee(lstemployee);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_employee);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_employee.ItemClick += Report_adapter_employee_ItemClick;


            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Report_Adapter_Customer_ItemClick at ReportEmp");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetShowButton()
        {

            if (employeeSelect == "All Employee")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseEmployee.Count > 0)
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(searchEmployee))
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
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        LinearLayout lnBack;
        FrameLayout lnSearch;
        ImageButton btnSearch;
        EditText textSearch;
        Button btnAll;
        RecyclerView rcvEmployee;
        LinearLayout lnNoDataSearch;
        Button btnApply;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearch = view.FindViewById<FrameLayout>(Resource.Id.lnSearch);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            btnAll = view.FindViewById<Button>(Resource.Id.btnAll);
            rcvEmployee = view.FindViewById<RecyclerView>(Resource.Id.rcvEmployee);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);

        }
    }
}