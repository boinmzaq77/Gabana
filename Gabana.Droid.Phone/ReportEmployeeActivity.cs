using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportEmployeeActivity : AppCompatActivity
    {
        public static ReportEmployeeActivity reportEmployeeActivity;
        private static ListEmployee lstemployee;
        ImageButton btnSearch;
        EditText textSearch;
        LinearLayout lnBack;
        RecyclerView rcvEmployee;
        public static List<ORM.MerchantDB.UserAccountInfo> listEmployees, listSearchEmployee;
        public static List<ORM.MerchantDB.UserAccountInfo> listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        Button btnAll, btnApply;
        private string employeeSelect;
        UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
        DialogLoading dialogLoading = new DialogLoading();
        List<ORM.MerchantDB.UserAccountInfo> listBeforeSetlect = new List<ORM.MerchantDB.UserAccountInfo>();
        string searchEmployee;
        LinearLayout lnNoDataSearch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_employee);
                reportEmployeeActivity = this;

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                textSearch = FindViewById<EditText>(Resource.Id.textSearch);
                textSearch.TextChanged += TxtSearchEmp_TextChanged;
                textSearch.FocusChange += TxtSearchEmp_FocusChange;
                textSearch.KeyPress += TxtSearchEmp_KeyPress;
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;

                rcvEmployee = FindViewById<RecyclerView>(Resource.Id.rcvEmployee);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;

                CheckJwt();

                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnAll.Click += BtnAll_Click;
                //employeeSelect = "All Employee";
                listBeforeSetlect = ReportDailySaleActivity.listChooseEmployee;
                SetShowButton();
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);

                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));

                dialogLoading.Dismiss();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportEmployeeActivity");

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackPageViewAsync("at ReportEmp");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_employee);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_employee.ItemClick += Report_Adapter_Customer_ItemClick;


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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
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

        private void TxtSearchEmp_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchEmployee = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchEmployee))
            {
                SetEmployeeData();
            }
            SetBtnSearch();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }
#pragma warning disable CS0414 // The field 'ReportEmployeeActivity.SearchName' is assigned but its value is never used
        string SearchName;
#pragma warning restore CS0414 // The field 'ReportEmployeeActivity.SearchName' is assigned but its value is never used
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearch.Text = string.Empty;
            SetBtnSearch();
        }
        private void SetShowButton()
        {

            if (employeeSelect == "All Employee")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

            if (listChooseEmployee.Count > 0)
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
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
            GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
            rcvEmployee.SetAdapter(report_adapter_employee);
            rcvEmployee.HasFixedSize = true;
            rcvEmployee.SetLayoutManager(mLayoutManager);
            report_adapter_employee.ItemClick += Report_Adapter_Customer_ItemClick;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (listChooseEmployee.Count > 0)
            {
                ReportDailySaleActivity.listChooseEmployee = listChooseEmployee;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, "กรุณาเลือกพนักงาน", ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_customer);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetEmployeeData at ReportEmp");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        internal static void SetSelectEmp(List<ORM.MerchantDB.UserAccountInfo> l, List<ORM.MerchantDB.UserAccountInfo> e)
        {
            listChooseEmployee = l;
            listEmployees = e;
        }


        private async void Report_Adapter_Customer_ItemClick(object sender, int e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

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
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                rcvEmployee.SetAdapter(report_adapter_employee);
                rcvEmployee.HasFixedSize = true;
                rcvEmployee.SetLayoutManager(mLayoutManager);
                report_adapter_employee.ItemClick += Report_Adapter_Customer_ItemClick;

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
                _ = TinyInsights.TrackPageViewAsync("Report_Adapter_Customer_ItemClick at ReportEmp");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
            if (string.IsNullOrEmpty(searchEmployee))
            {
                SetEmployeeData();
                SetBtnSearch();
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

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportEmployeeActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportEmployeeActivity.openPage' is assigned but its value is never used
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

