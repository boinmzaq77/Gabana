using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Report;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Report_Dialog_Custom : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public static Report_Dialog_Custom NewInstance()
        {
            var frag = new Report_Dialog_Custom { Arguments = new Bundle() };
            return frag;
        }
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_custom, container, false);
            try
            {
                dialog_Custom = this;
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
        internal static string branchID;
        public override async void OnResume()
        {
            base.OnResume();

            branchID = Report_Fragment_Main.branchID;

            SetUiFromTypeReport();
            await GetBranchSelect();
        }

        private Branch branch;

        public static Report_Dialog_Custom dialog_Custom;
        public async Task GetBranchSelect()
        {
            BranchManage branchManage = new BranchManage();
            List<Branch> lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            branch = lstBranch.Where(x => x.BranchID == branchID).FirstOrDefault();

            if (listChooseBranch.Count == 0)
            {
                listChooseBranch = lstBranch;
            }
            if (lstBranch.Count == listChooseBranch.Count)
            {
                txtBranch.Text = GetString(Resource.String.report_allbranch);
            }
            else
            {
                txtBranch.Text = "";
                foreach (var item in listChooseBranch)
                {
                    if (txtBranch.Text != "")
                    {
                        txtBranch.Text += "," + item.BranchName;

                    }
                    else
                    {
                        txtBranch.Text = item.BranchName;
                    }
                }
            }
        }
        private string startDate, endDate, textDate;
        internal static List<Branch> listChooseBranch = new List<Branch>();
        internal static List<Category> listChooseCategory = new List<Category>();
        internal static List<Customer> listChooseCustomer = new List<Customer>();
        internal static List<ORM.MerchantDB.UserAccountInfo> listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        internal static List<Model.PaymentType> listChoosePayment = new List<Model.PaymentType>();
        string timeType;
        private string BestSellBy;
        Android.App.DatePickerDialog dialogStartDate, dialogEndDate;

        private async void SetUIEvent()
        {
            try
            {
                startDate = string.Empty;
                endDate = string.Empty;
                listChooseBranch = new List<Branch>();
                timeType = string.Empty;
                textDate = string.Empty;
                listChooseCategory = new List<Category>(); ;
                listChooseCustomer = new List<Customer>();
                listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
                listChoosePayment = new List<Model.PaymentType>();
                BestSellBy = string.Empty;

                DateTime today = DateTime.Now;
                txtDateSelect.Text = DateTime.Now.ToString("dd MMM", new CultureInfo("en-US"));
                txtMonthSelect.Text = DateTime.Now.ToString("MMM", new CultureInfo("en-US"));
                txtYearSelect.Text = DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));

                txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                var data = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                dialogStartDate = new DatePickerDialog(this.Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_StartDate,
                                                         today.Year,
                                                         today.Month - 1,
                                                         today.Day);
                dialogStartDate.DatePicker.MinDate = (long)(data.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogStartDate.DatePicker.MaxDate = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;

                dialogEndDate = new DatePickerDialog(this.Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_EndDate,
                                                        today.Year,
                                                        today.Month - 1,
                                                        today.Day);
                dialogEndDate.DatePicker.MinDate = (long)(data.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogEndDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                lnChooseBranch.Click += LnChooseBranch_Click;
                lnEndDate.Click += LnEndDate_Click;
                lnStartDate.Click += LnStartDate_Click;
                //lnEndDate.Enabled = false;
                timeType = "Date";
                BestSellBy = "Sell";
                setUiFromSeletTime();
                lnBack.Click += LnBack_Click;
                lnDate.Click += LnDate_Click;
                lnMonth.Click += LnMonth_Click;
                lnYear.Click += LnYear_Click;

                lnSelectCategory.Click += LnSelectCategory_Click;
                lnSelectCustomer.Click += LnSelectCustomer_Click;
                lnSelectEmployee.Click += LnSelectEmployee_Click;
                lnSelectPaymemt.Click += LnSelectPaymemt_Click;

                btnViewReport.Click += BtnViewReport_Click;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            Report_Dialog_Branch.listChooseBranch = listChooseBranch;

            var fragment = new Report_Dialog_Branch();
            Report_Dialog_Branch dialog = new Report_Dialog_Branch();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Branch));
        }

        private async void BtnViewReport_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            switch (TypeRoport)
            {
                case "SalesReport":
                    Report_Fragment_ShowData.SetDataReport("SalesReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "SalesReportByBranch":
                    Report_Fragment_ShowData.SetDataReport("SalesReportByBranch",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "ProfitReport":
                    Report_Fragment_ShowData.SetDataReport("ProfitReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "CategoryReport":
                    Report_Fragment_ShowData.SetDataReport("CategoryReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                txtCategory.Text,
                                                                listChooseCategory,
                                                                null,
                                                                null,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "CustomerReport":
                    Report_Fragment_ShowData.SetDataReport("CustomerReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                txtCustomer.Text,
                                                                null,
                                                                listChooseCustomer,
                                                                null,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "EmployeeReport":
                    Report_Fragment_ShowData.SetDataReport("EmployeeReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                txtEmployee.Text,
                                                                null,
                                                                null,
                                                                listChooseEmployee,
                                                                null,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "PaymentReport":
                    Report_Fragment_ShowData.SetDataReport("PaymentReport",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                txtPaymemt.Text,
                                                                null,
                                                                null,
                                                                null,
                                                                listChoosePayment,
                                                                null);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;
                case "ReportBestSale":
                    Report_Fragment_ShowData.SetDataReport("ReportBestSale",
                                                                startDate,
                                                                endDate,
                                                                listChooseBranch,
                                                                timeType,
                                                                textDate,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                BestSellBy);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "showreport", "default");
                    break;

                default:
                    break;
            }
            Report_Fragment_ShowData.filterReport = 0; 
            this.Dialog.Dismiss();
        }

        private void LnSelectPaymemt_Click(object sender, EventArgs e)
        {
            Report_Dialog_Payment.SetSelectPayment(listChoosePayment, txtPaymemt.Text);
            var fragment = new Report_Dialog_Payment();
            Report_Dialog_Payment dialog = new Report_Dialog_Payment();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Payment));
        }

        private void LnSelectEmployee_Click(object sender, EventArgs e)
        {
            Report_Dialog_Employee.SetSelectEmp(listChooseEmployee, listEmployees);
            var fragment = new Report_Dialog_Employee();
            Report_Dialog_Employee dialog = new Report_Dialog_Employee();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Employee));
        }

        private void LnSelectCustomer_Click(object sender, EventArgs e)
        {
            Report_Dialog_Customer.SetSelectCustomer(listChooseCustomer, txtCustomer.Text);
            var fragment = new Report_Dialog_Customer();
            Report_Dialog_Customer dialog = new Report_Dialog_Customer();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Customer));
        }

        private void LnSelectCategory_Click(object sender, EventArgs e)
        {
            Report_Dialog_Category.SetSelectCategory(listChooseCategory, txtCategory.Text);
            Report_Dialog_Category.listChooseCategory = listChooseCategory;
            var fragment = new Report_Dialog_Category();
            Report_Dialog_Category dialog = new Report_Dialog_Category();
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Category));
        }
        private void LnYear_Click(object sender, EventArgs e)
        {
            try
            {
                if (timeType != "Year")
                {
                    timeType = "Year";

                    txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    endDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    lnEndDate.Enabled = false;
                }

                setUiFromSeletTime();
            }
            catch (Exception)
            {
                _ = TinyInsights.TrackPageViewAsync("at LnMonth_Click");

            }
        }

        private void LnMonth_Click(object sender, EventArgs e)
        {
            try
            {
                if (timeType != "Month")
                {
                    timeType = "Month";
                    txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    endDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    lnEndDate.Enabled = false;
                }
                setUiFromSeletTime();
            }
            catch (Exception)
            {
                _ = TinyInsights.TrackPageViewAsync("at LnMonth_Click");
            }
        }

        private void LnDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (timeType != "Date")
                {
                    timeType = "Date";
                    txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                    startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    endDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                    lnEndDate.Enabled = false;
                }
                setUiFromSeletTime();
            }
            catch (Exception)
            {
                _ = TinyInsights.TrackPageViewAsync("at LnDate_Click");
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        private static string TypeRoport;
        private void LnStartDate_Click(object sender, EventArgs e)
        {
            dialogStartDate.Show();
        }
        private void SetUiFromTypeReport()
        {
            lnShowFilterByBranch.Visibility = ViewStates.Visible;
            lnShowFilterByCategory.Visibility = ViewStates.Gone;
            lnShowFilterByCustomer.Visibility = ViewStates.Gone;
            lnShowFilterByEmployee.Visibility = ViewStates.Gone;
            lnShowFilterByPayment.Visibility = ViewStates.Gone;
            lnShowFilterByReport.Visibility = ViewStates.Gone;
            switch (TypeRoport)
            {
                case "SalesReport":
                    txtTitle.Text = GetString(Resource.String.report_sale_report);

                    break;
                case "SalesReportByBranch":
                    txtTitle.Text = GetString(Resource.String.report_salebybranch_report);
                    lnShowFilterByBranch.Visibility = ViewStates.Gone;
                    break;
                case "ProfitReport":
                    txtTitle.Text = GetString(Resource.String.report_profitbycostestimate_report);
                    break;
                case "CategoryReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycategory_report);
                    lnShowFilterByCategory.Visibility = ViewStates.Visible;
                    SetDataCategory();
                    break;
                case "CustomerReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycustomer_report);
                    lnShowFilterByCustomer.Visibility = ViewStates.Visible;
                    SetDataCustomer();
                    break;
                case "EmployeeReport":
                    txtTitle.Text = GetString(Resource.String.report_salebyemployee_report);
                    lnShowFilterByEmployee.Visibility = ViewStates.Visible;
                    SetDataEmployee();
                    break;
                case "PaymentReport":
                    txtTitle.Text = GetString(Resource.String.report_salebypaymentmethod_report);
                    lnShowFilterByPayment.Visibility = ViewStates.Visible;
                    SetDataPayment();
                    break;
                case "ReportBestSale":
                    txtTitle.Text = GetString(Resource.String.report_bestitem_report);
                    lnShowFilterByReport.Visibility = ViewStates.Visible;
                    BestSellBy = "Sell";
                    SetImageCheckReport();
                    lnBySell.Click += LnBySell_Click;
                    lnByUnit.Click += LnByUnit_Click;
                    break;
                default:
                    break;
            }

        }
        private void SetImageCheckReport()
        {
            imgCheckBySell.Visibility = ViewStates.Gone;
            imgCheckByUnit.Visibility = ViewStates.Gone;
            switch (BestSellBy)
            {
                case "Sell":
                    imgCheckBySell.Visibility = ViewStates.Visible;
                    break;
                case "Unit":
                    imgCheckByUnit.Visibility = ViewStates.Visible;
                    break;
                default:
                    imgCheckBySell.Visibility = ViewStates.Visible;
                    break;
            }

        }
        private void LnBySell_Click(object sender, EventArgs e)
        {
            BestSellBy = "Sell";
            SetImageCheckReport();
        }
        private void LnByUnit_Click(object sender, EventArgs e)
        {
            BestSellBy = "Unit";
            SetImageCheckReport();
        }
        public async Task SetDataCategory()
        {
            CategoryManage category = new CategoryManage();
            List<Category> lsCategory = await category.GetAllCategory();

            if (listChooseCategory.Count == 0)
            {
                listChooseCategory = lsCategory;
            }
            listChooseCategory = listChooseCategory.OrderBy(x => x.SysCategoryID).ToList();
            if (lsCategory.Count == listChooseCategory.Count)
            {
                txtCategory.Text = GetString(Resource.String.allcategory);
            }
            else
            {
                txtCategory.Text = "";
                foreach (var item in listChooseCategory)
                {
                    if (txtCategory.Text != "")
                    {
                        txtCategory.Text += "," + item.Name;

                    }
                    else
                    {
                        txtCategory.Text = item.Name;
                    }
                }
            }
        }
        public async Task SetDataCustomer()
        {
            CustomerManage customerManage = new CustomerManage();
            List<Customer> lstCustomer = await customerManage.GetAllCustomer();
            if (listChooseCustomer.Count == 0)
            {
                listChooseCustomer = lstCustomer;
            }

            listChooseCustomer = listChooseCustomer.OrderBy(x => x.CustomerID).ToList();
            if (lstCustomer.Count == listChooseCustomer.Count)
            {
                txtCustomer.Text = GetString(Resource.String.allcustomer);
            }
            else
            {
                txtCustomer.Text = "";
                foreach (var item in listChooseCustomer)
                {
                    if (txtCustomer.Text != "")
                    {
                        txtCustomer.Text += "," + item.CustomerName;

                    }
                    else
                    {
                        txtCustomer.Text = item.CustomerName;
                    }
                }
            }
        }
        internal static List<ORM.MerchantDB.UserAccountInfo> listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
        public async Task SetDataEmployee()
        {
            try
            {
                UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
                List<ORM.MerchantDB.UserAccountInfo> emp = new List<ORM.MerchantDB.UserAccountInfo>();
                listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                if (DataCashingAll.UserAccountInfo == null)
                {
                    DataCashingAll.UserAccountInfo = new List<Model.UserAccountInfo>();
                    DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                }
                if (DataCashingAll.BranchPolicy == null)
                {
                    DataCashingAll.BranchPolicy = await GabanaAPI.GetDataBranchPolicy();
                }

                var localUserAccount = await userAccountInfoManage.GetAllUserAccount();
                var sort = DataCashingAll.UserAccountInfo.OrderBy(x =>
                {
                    var xx = 0;
                    switch (x.MainRoles.ToLower())
                    {
                        case "owner":
                            xx = 1;
                            break;
                        case "admin":
                            xx = 2;
                            break;
                        case "manager":
                            xx = 3;
                            break;
                        case "invoice":
                            xx = 4;
                            break;
                        case "cashier":
                            xx = 5;
                            break;
                        case "editor":
                            xx = 6;
                            break;
                        default:
                            xx = 7;
                            break;
                    }
                    return xx;
                })
                .ToList();
                DataCashingAll.UserAccountInfo = sort;

                DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);

                foreach (var item in DataCashingAll.UserAccountInfo)
                {
                    var data = await GabanaAPI.GetDataUserAccount(item.UserName);
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = item.MerchantID,
                        UserName = item.UserName,
                        FUsePincode = DataCashingAll.Merchant?.UserAccountInfo.Where(x => x.UserName == item.UserName).Select(x => (int?)x.FUsePincode).FirstOrDefault() ?? 0,
                        PinCode = DataCashingAll.Merchant?.UserAccountInfo.Where(x => x.UserName == item.UserName).Select(x => x.PinCode).FirstOrDefault(),
                        Comments = data?.userAccountInfo.Comments,
                    };
                    var insertlocal = await userAccountInfoManage.InsertorReplaceUserAccount(localUser);

                    //Insert BranchPolicy
                    if (insertlocal)
                    {
                        if (DataCashingAll.BranchPolicy != null & item.MainRoles.ToLower() != "owner" & item.MainRoles.ToLower() != "admin")
                        {
                            var result = DataCashingAll.BranchPolicy.Where(x => x.UserName == item.UserName).ToList();
                            foreach (var itembranch in result)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName,
                                };
                                BranchPolicyManage policyManage = new BranchPolicyManage();
                                //var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                                //var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, itembranch.UserName);
                                var insertlocalbranchPolicy = await policyManage.InsertorReplacrBranchPolicy(branchPolicy);
                            }
                        }
                        emp.Add(localUser);
                    }
                }

                //Local == GabanaAPI เพื่อลบข้อมูล useraccount 
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                var getUseraccount = await accountInfoManage.GetAllUserAccount();
                var lstGabanaAPI = DataCashingAll.Merchant?.UserAccountInfo;

                HashSet<string> sentIDs = new HashSet<string>(getUseraccount.Select(s => s.UserName.ToLower()));
                var results = lstGabanaAPI.Where(m => !sentIDs.Contains(m.UserName.ToLower())).ToList();
                if (results.Count > 0)
                {
                    foreach (var item in results)
                    {
                        //branchPolicy
                        BranchPolicyManage policyManage = new BranchPolicyManage();
                        var getBranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, item.UserName);
                        foreach (var branchPolicy in getBranchPolicy)
                        {
                            var deleteBranchPolicy = await policyManage.DeleteBranch(DataCashingAll.MerchantId, item.UserName);
                        }

                        //Useraccount
                        var deleteUseraccount = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, item.UserName);
                    }
                }
                listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
                listEmployees.AddRange(emp);


                //List<UserAccountInfo> lstEmployee = await userAccountInfoManage.GetAllUserAccount();


                if (listChooseEmployee.Count == 0)
                {
                    listChooseEmployee = listEmployees;
                }
                listChooseEmployee = listChooseEmployee.OrderBy(x => x.UserName).ToList();
                if (listEmployees.Count == listChooseEmployee.Count)
                {
                    txtEmployee.Text = GetString(Resource.String.allemployee);
                }
                else
                {
                    txtEmployee.Text = "";
                    foreach (var item in listChooseEmployee)
                    {
                        if (txtEmployee.Text != "")
                        {
                            txtEmployee.Text += "," + item.UserName;
                        }
                        else
                        {
                            txtEmployee.Text = item.UserName;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }
        public async Task SetDataPayment()
        {
            List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash", Logo = Resource.Mipmap.RPaymentCash , color = "#0095DA"},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = Resource.Mipmap.RPaymentCredit, color = "#F8971D"},
                new Model.PaymentType(){ Type ="Dr" ,Detail = "Debit Card", Logo = Resource.Mipmap.RPaymentDebit, color = "#E32D49"},
                new Model.PaymentType(){ Type ="GV" ,Detail = "Gift Voucher" , Logo = Resource.Mipmap.RPaymentGiftvoucher , color = "#37AA52" },
                new Model.PaymentType(){ Type ="MYQR",Detail = "myQR", Logo = Resource.Mipmap.RPaymentMyQR , color = "#F75600"},
                new Model.PaymentType(){ Type ="QRCH",Detail = "QR Cash", Logo = Resource.Mipmap.RPaymentQrCash , color = "#3F51B5"},
                new Model.PaymentType(){ Type ="QRCR",Detail = "QR Credit", Logo = Resource.Mipmap.RPaymentQrCredit , color = "#00796B"},
                new Model.PaymentType(){ Type ="WECHAT",Detail = "WECHAT",Logo = Resource.Mipmap.RPaymentWechat , color = "#8BC34A"}
            };
            Model.GabanaModel.gabanaMain.payments = listPayments;
            if (listChoosePayment.Count == 0)
            {
                listChoosePayment = Model.GabanaModel.gabanaMain.payments;
            }
            if (Model.GabanaModel.gabanaMain.payments.Count == listChoosePayment.Count)
            {
                txtPaymemt.Text = GetString(Resource.String.allpayment);
            }
            else
            {
                txtPaymemt.Text = "";
                foreach (var item in listChoosePayment)
                {
                    if (txtPaymemt.Text != "")
                    {
                        txtPaymemt.Text += "," + item.Detail;
                    }
                    else
                    {
                        txtPaymemt.Text = item.Detail;
                    }
                }
            }
        }

        internal static void SetTypeReport(string v)
        {
            TypeRoport = v;
        }
        private void LnEndDate_Click(object sender, EventArgs e)
        {
            dialogEndDate.Show();
        }
        private void DatePickerDialog_StartDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            startDate = Utils.ChangeDateTimeReport(e.Date);
            txtStartDate.Text = e.Date.ToString("dd MMM yyyy", new CultureInfo("en-US"));
            if (dialogEndDate.DatePicker.MinDate < (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds)
            {
                dialogEndDate.DatePicker.MinDate = (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            timeType = "CustomDate";
            textDate = txtStartDate.Text + "-" + txtEndDate.Text;
            imgChechDate.Visibility = ViewStates.Gone;
            imgChechMonth.Visibility = ViewStates.Gone;
            imgChechYear.Visibility = ViewStates.Gone;

            endDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
            lnEndDate.Enabled = true;
            setUiFromSeletTime();

        }
        private void DatePickerDialog_EndDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            txtEndDate.Text = e.Date.ToString("dd MMM yyyy", new CultureInfo("en-US"));
            endDate = Utils.ChangeDateTimeReport(e.Date);
            txtEndDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            timeType = "CustomDate";
            textDate = txtStartDate.Text + "-" + txtEndDate.Text;
            setUiFromSeletTime();
        }
        private void setUiFromSeletTime()
        {
            try
            {
                txtToDay.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                txtMonth.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                txtYear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                imgChechDate.Visibility = ViewStates.Gone;
                imgChechMonth.Visibility = ViewStates.Gone;
                imgChechYear.Visibility = ViewStates.Gone;

                txtStartDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                txtEndDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                var now = DateTime.UtcNow;

                switch (timeType)
                {
                    case "Date":
                        txtToDay.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        imgChechDate.Visibility = ViewStates.Visible;
                        textDate = txtDateSelect.Text;
                        startDate = Utils.ChangeDateTimeReport(now);
                        endDate = Utils.ChangeDateTimeReport(now);
                        break;
                    case "Month":
                        txtMonth.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        imgChechMonth.Visibility = ViewStates.Visible;
                        textDate = txtMonthSelect.Text + " " + txtYearSelect.Text;
                        var startOfMonth = new DateTime(now.Year, now.Month, 1);
                        var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                        var lastDayOfMonth = new DateTime(now.Year, now.Month, DaysInMonth);
                        startDate = Utils.ChangeDateTimeReport(startOfMonth);
                        endDate = Utils.ChangeDateTimeReport(lastDayOfMonth);
                        break;
                    case "Year":
                        txtYear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        imgChechYear.Visibility = ViewStates.Visible;
                        textDate = txtYearSelect.Text;
                        var year = DateTime.UtcNow.Year;
                        var startOfYear = new DateTime(year, 1, 1);
                        //var lastDayOfYear = new DateTime(year, 12, 31);
                        startDate = Utils.ChangeDateTimeReport(startOfYear);
                        endDate = Utils.ChangeDateTimeReport(now);
                        break;
                    case "CustomDate":
                        txtStartDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        txtEndDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                _ = TinyInsights.TrackPageViewAsync("at setUiFromSeletTime");
            }

        }
        LinearLayout lnBack;
        LinearLayout lnShowFilterByBranch;
        TextView txtBranch;
        LinearLayout lnChooseBranch;
        LinearLayout lnShowFilterByCategory;
        TextView txtCategory;
        LinearLayout lnSelectCategory;
        LinearLayout lnShowFilterByCustomer;
        TextView txtCustomer;
        LinearLayout lnSelectCustomer;
        LinearLayout lnShowFilterByEmployee;
        TextView txtEmployee;
        LinearLayout lnSelectEmployee;
        LinearLayout lnShowFilterByPayment;
        TextView txtPaymemt;
        LinearLayout lnSelectPaymemt;
        LinearLayout lnShowFilterByReport;
        ImageView imgCheckBySell;
        LinearLayout lnBySell;
        ImageView imgCheckByUnit;
        LinearLayout lnByUnit;

        TextView txtDateSelect, txtMonthSelect, txtYearSelect;
        ImageView imgChechDate, imgChechMonth, imgChechYear;
        TextView txtStartDate, txtEndDate;
        LinearLayout lnStartDate, lnEndDate;
        TextView txtToDay, txtMonth, txtYear;
        LinearLayout lnDate, lnMonth, lnYear;
        Button btnViewReport;

        TextView txtTitle;
        private void CombinUI()
        {
            txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);

            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnShowFilterByBranch = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByBranch);
            txtBranch = view.FindViewById<TextView>(Resource.Id.txtBranch);
            lnChooseBranch = view.FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
            lnShowFilterByCategory = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByCategory);
            txtCategory = view.FindViewById<TextView>(Resource.Id.txtCategory);
            lnSelectCategory = view.FindViewById<LinearLayout>(Resource.Id.lnSelectCategory);
            lnShowFilterByCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByCustomer);
            txtCustomer = view.FindViewById<TextView>(Resource.Id.txtCustomer);
            lnSelectCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnSelectCustomer);
            lnShowFilterByEmployee = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByEmployee);
            txtEmployee = view.FindViewById<TextView>(Resource.Id.txtEmployee);
            lnSelectEmployee = view.FindViewById<LinearLayout>(Resource.Id.lnSelectEmployee);
            lnShowFilterByPayment = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByPayment);
            txtPaymemt = view.FindViewById<TextView>(Resource.Id.txtPaymemt);
            lnSelectPaymemt = view.FindViewById<LinearLayout>(Resource.Id.lnSelectPaymemt);
            lnShowFilterByReport = view.FindViewById<LinearLayout>(Resource.Id.lnShowFilterByReport);
            lnBySell = view.FindViewById<LinearLayout>(Resource.Id.lnBySell);
            imgCheckBySell = view.FindViewById<ImageView>(Resource.Id.imgCheckBySell);
            imgCheckByUnit = view.FindViewById<ImageView>(Resource.Id.imgCheckByUnit);
            lnByUnit = view.FindViewById<LinearLayout>(Resource.Id.lnByUnit);
            txtToDay = view.FindViewById<TextView>(Resource.Id.txtToDay);
            txtDateSelect = view.FindViewById<TextView>(Resource.Id.txtDateSelect);
            imgChechDate = view.FindViewById<ImageView>(Resource.Id.imgChechDate);
            lnDate = view.FindViewById<LinearLayout>(Resource.Id.lnDate);
            txtMonth = view.FindViewById<TextView>(Resource.Id.txtMonth);
            txtMonthSelect = view.FindViewById<TextView>(Resource.Id.txtMonthSelect);
            imgChechMonth = view.FindViewById<ImageView>(Resource.Id.imgChechMonth);
            lnMonth = view.FindViewById<LinearLayout>(Resource.Id.lnMonth);
            txtYear = view.FindViewById<TextView>(Resource.Id.txtYear);
            txtYearSelect = view.FindViewById<TextView>(Resource.Id.txtYearSelect);
            imgChechYear = view.FindViewById<ImageView>(Resource.Id.imgChechYear);
            lnYear = view.FindViewById<LinearLayout>(Resource.Id.lnYear);
            txtStartDate = view.FindViewById<TextView>(Resource.Id.txtStartDate);
            lnStartDate = view.FindViewById<LinearLayout>(Resource.Id.lnStartDate);
            txtEndDate = view.FindViewById<TextView>(Resource.Id.txtEndDate);
            lnEndDate = view.FindViewById<LinearLayout>(Resource.Id.lnEndDate);
            btnViewReport = view.FindViewById<Button>(Resource.Id.btnViewReport);
        }
    }
}