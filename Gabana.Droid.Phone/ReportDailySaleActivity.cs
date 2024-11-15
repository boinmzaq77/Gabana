using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
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

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ReportDailySaleActivity : AppCompatActivity
    {

        internal static string branchID;
        TextView txtTitle, txtBranchName, txtBranch;
        TextView txtToDay, txtMonth, txtYear;
        ImageView imgChechDate, imgChechMonth, imgChechYear;
        TextView txtDateSelect, txtMonthSelect, txtYearSelect;
        string timeType;
        Android.App.DatePickerDialog dialogStartDate, dialogEndDate;
        TextView txtStartDate, txtEndDate;
        private string startDate, endDate, textDate;
        private static string typeRoport;
        Button btnViewReport;
        TextView txtCategory, txtCustomer, txtPaymemt;
        public static TextView txtEmployee;
        LinearLayout lnShowFilterByBranch, lnShowFilterByCategory, lnSelectCategory, lnShowFilterByCustomer, lnSelectCustomer, lnShowFilterByEmployee,
        lnSelectEmployee, lnShowFilterByPayment, lnSelectPaymemt, lnShowFilterByReport;
        internal static List<Category> listChooseCategory = new List<Category>();
        internal static List<Customer> listChooseCustomer = new List<Customer>();
        internal static List<ORM.MerchantDB.UserAccountInfo> listEmployees = new List<ORM.MerchantDB.UserAccountInfo>();
        internal static List<ORM.MerchantDB.UserAccountInfo> listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();
        internal static List<Model.PaymentType> listChoosePayment = new List<Model.PaymentType>();
        LinearLayout lnBySell, lnByUnit;
        ImageView imgCheckBySell, imgCheckByBalance;
        private string BestSellBy;
        private Branch branch;
        DialogLoading dialogLoading = new DialogLoading();
        internal static List<Branch> listChooseBranch = new List<Branch>();
        LinearLayout lnEndDate, lnStartDate;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_dailysale);

                LinearLayout lnChooseBranch = FindViewById<LinearLayout>(Resource.Id.lnChooseBranch);
                lnChooseBranch.Click += LnChooseBranch_Click;

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

                txtBranchName = FindViewById<TextView>(Resource.Id.txtBranchName);
                txtBranch = FindViewById<TextView>(Resource.Id.txtBranch);
                branchID = ReportActivity.branchID;

                LinearLayout lnDate = FindViewById<LinearLayout>(Resource.Id.lnDate);
                lnDate.Click += LnDate_Click;
                LinearLayout lnMonth = FindViewById<LinearLayout>(Resource.Id.lnMonth);
                lnMonth.Click += LnMonth_Click;
                LinearLayout lnYear = FindViewById<LinearLayout>(Resource.Id.lnYear);
                lnYear.Click += LnYear_Click;

                lnShowFilterByBranch = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByBranch);
                lnShowFilterByCategory = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByCategory);
                lnShowFilterByCustomer = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByCustomer);
                lnShowFilterByEmployee = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByEmployee);
                lnShowFilterByPayment = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByPayment);
                lnShowFilterByReport = FindViewById<LinearLayout>(Resource.Id.lnShowFilterByReport);

                txtCategory = FindViewById<TextView>(Resource.Id.txtCategory);
                txtCustomer = FindViewById<TextView>(Resource.Id.txtCustomer);
                txtEmployee = FindViewById<TextView>(Resource.Id.txtEmployee);
                txtPaymemt = FindViewById<TextView>(Resource.Id.txtPaymemt);

                lnSelectCategory = FindViewById<LinearLayout>(Resource.Id.lnSelectCategory);
                lnSelectCustomer = FindViewById<LinearLayout>(Resource.Id.lnSelectCustomer);
                lnSelectEmployee = FindViewById<LinearLayout>(Resource.Id.lnSelectEmployee);
                lnSelectPaymemt = FindViewById<LinearLayout>(Resource.Id.lnSelectPaymemt);

                lnSelectCategory.Click += LnSelectCategory_Click;
                lnSelectCustomer.Click += LnSelectCustomer_Click;
                lnSelectEmployee.Click += LnSelectEmployee_Click;
                lnSelectPaymemt.Click += LnSelectPaymemt_Click;

                txtToDay = FindViewById<TextView>(Resource.Id.txtToDay);
                txtMonth = FindViewById<TextView>(Resource.Id.txtMonth);
                txtYear = FindViewById<TextView>(Resource.Id.txtYear);

                imgChechDate = FindViewById<ImageView>(Resource.Id.imgChechDate);
                imgChechMonth = FindViewById<ImageView>(Resource.Id.imgChechMonth);
                imgChechYear = FindViewById<ImageView>(Resource.Id.imgChechYear);

                txtDateSelect = FindViewById<TextView>(Resource.Id.txtDateSelect);
                txtMonthSelect = FindViewById<TextView>(Resource.Id.txtMonthSelect);
                txtYearSelect = FindViewById<TextView>(Resource.Id.txtYearSelect);

                txtStartDate = FindViewById<TextView>(Resource.Id.txtStartDate);
                txtEndDate = FindViewById<TextView>(Resource.Id.txtEndDate);

                imgCheckBySell = FindViewById<ImageView>(Resource.Id.imgCheckBySell);
                imgCheckByBalance = FindViewById<ImageView>(Resource.Id.imgCheckByUnit);

                lnBySell = FindViewById<LinearLayout>(Resource.Id.lnBySell);
                lnByUnit = FindViewById<LinearLayout>(Resource.Id.lnByUmit);

                btnViewReport = FindViewById<Button>(Resource.Id.btnViewReport);
                btnViewReport.Click += BtnViewReport_Click;

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

                CheckJwt();
                DateTime today = DateTime.Now;
                txtDateSelect.Text = DateTime.Now.ToString("dd MMM", new CultureInfo("en-US"));
                txtMonthSelect.Text = DateTime.Now.ToString("MMM", new CultureInfo("en-US"));
                txtYearSelect.Text = DateTime.Now.ToString("yyyy", new CultureInfo("en-US"));

                txtStartDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                txtEndDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
                startDate = Utils.ChangeDateTimeReport(DateTime.UtcNow);
                var data = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
#pragma warning disable CS0618 // Type or member is obsolete
                dialogStartDate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_StartDate,
                                                         today.Year,
                                                         today.Month - 1,
                                                         today.Day);
#pragma warning restore CS0618 // Type or member is obsolete
                dialogStartDate.DatePicker.MinDate = (long)(data.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogStartDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

#pragma warning disable CS0618 // Type or member is obsolete
                dialogEndDate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_EndDate,
                                                        today.Year,
                                                        today.Month - 1,
                                                        today.Day);
#pragma warning restore CS0618 // Type or member is obsolete
                dialogEndDate.DatePicker.MinDate = (long)(data.Merchant.DateCreated - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialogEndDate.DatePicker.MaxDate = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
                lnEndDate = FindViewById<LinearLayout>(Resource.Id.lnEndDate);
                lnEndDate.Click += LnEndDate_Click;
                lnStartDate = FindViewById<LinearLayout>(Resource.Id.lnStartDate);
                lnStartDate.Click += LnStartDate_Click;
                lnEndDate.Enabled = false;
                timeType = "Date";
                BestSellBy = "Sell";
                setUiFromSeletTime();

                listChooseEmployee = new List<ORM.MerchantDB.UserAccountInfo>();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportDailySaleActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("at ReportDaily");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnSelectPaymemt_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportPaymentActivity)));
            ReportPaymentActivity.SetSelectPayment(listChoosePayment, txtPaymemt.Text);

        }

        private void LnSelectEmployee_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportEmployeeActivity)));
            ReportEmployeeActivity.SetSelectEmp(listChooseEmployee, listEmployees);
        }

        private void LnSelectCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportCustomerActivity)));
            ReportCustomerActivity.SetSelectCustomer(listChooseCustomer, txtCustomer.Text);

        }

        private void LnSelectCategory_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportCategoryActivity)));
            ReportCategoryActivity.SetSelectCategory(listChooseCategory, txtCategory.Text);

        }

        private void BtnViewReport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(endDate))
            {
                endDate = startDate;
            }
            switch (typeRoport)
            {
                case "SalesReport":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("SalesReport",
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
                    break;
                case "SalesReportByBranch":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("SalesReportByBranch",
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
                    break;
                case "ProfitReport":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("ProfitReport",
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
                    break;
                case "CategoryReport":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("CategoryReport",
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
                    break;
                case "CustomerReport":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("CustomerReport",
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
                    break;
                case "EmployeeReport":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("EmployeeReport",
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
                    break;
                case "PaymentReport":
                    //var GetDataPaymentReport = await GabanaAPI.GetDataReportSummaryDailyPayment(DataCashingAll.SysBranchId, startDate, endDate);
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("PaymentReport",
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
                    break;
                case "ReportBestSale":
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ShowReportDailySaleActivity)));
                    ShowReportDailySaleActivity.SetDataReport("ReportBestSale",
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
                    break;

                default:
                    break;
            }
        }

        private void SetUiFromTypeReport()
        {
            lnShowFilterByBranch.Visibility = ViewStates.Visible;
            lnShowFilterByCategory.Visibility = ViewStates.Gone;
            lnShowFilterByCustomer.Visibility = ViewStates.Gone;
            lnShowFilterByEmployee.Visibility = ViewStates.Gone;
            lnShowFilterByPayment.Visibility = ViewStates.Gone;
            lnShowFilterByReport.Visibility = ViewStates.Gone;
            switch (typeRoport)
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
            imgCheckByBalance.Visibility = ViewStates.Gone;
            switch (BestSellBy)
            {
                case "Sell":
                    imgCheckBySell.Visibility = ViewStates.Visible;
                    break;
                case "Unit":
                    imgCheckByBalance.Visibility = ViewStates.Visible;
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
        private void SetDataPayment()
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
        private async void SetDataEmployee()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

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
                _ = TinyInsights.TrackPageViewAsync(" SetDataEmployee at Employee");
                return;
            }
        }

        private async void SetDataCustomer()
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

        private async void SetDataCategory()
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

        internal static void SetTypeReport(string v)
        {
            typeRoport = v;
        }

        private void LnStartDate_Click(object sender, EventArgs e)
        {
            dialogStartDate.Show();
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
                txtToDay.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null));
                txtMonth.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null));
                txtYear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblackcolor, null));
                imgChechDate.Visibility = ViewStates.Gone;
                imgChechMonth.Visibility = ViewStates.Gone;
                imgChechYear.Visibility = ViewStates.Gone;

                txtStartDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                txtEndDate.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
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
        private void LnChooseBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ReportSelectBranchActivity)));
            ReportSelectBranchActivity.listChooseBranch = listChooseBranch;
        }
        public override void OnBackPressed()
        {
            this.Finish();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Finish();
        }
        protected override async void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                DialogLoading dialogLoading = new DialogLoading();
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                await GetBranchSelect();
                SetUiFromTypeReport();
                //GetReport();
                setUiFromSeletTime();
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                OnRestart();

            }

        }

        private async Task GetBranchSelect()
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

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportDailySaleActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportDailySaleActivity.openPage' is assigned but its value is never used
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

