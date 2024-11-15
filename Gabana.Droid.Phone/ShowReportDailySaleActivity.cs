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
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Report;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class ShowReportDailySaleActivity : AppCompatActivity
    {
        TextView txtTitle, txtBranchName, txtCustomDate, txtDate;
        TextView txtTitleChart, txtTypeTime, txtGroupBy;
        static List<Gabana.ORM.MerchantDB.Branch> listChooseBranch = new List<ORM.MerchantDB.Branch>();
        private static string StartDate, EndDate, TimeType;
        public static string TypeReport;
        private static string TextDate, TextGroup;
        private static string TypeItem;
        private static List<Category> ListCategory;
        private static List<Customer> ListCustomer;
        private static List<ORM.MerchantDB.UserAccountInfo> ListEmployee;
        private static List<PaymentType> lsPayment;
        private static List<PaymentType> paymentTypes = new List<PaymentType> {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash", Logo = Resource.Mipmap.RPaymentCash , color = "#0095DA"},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = Resource.Mipmap.RPaymentCredit, color = "#F8971D"},
                new Model.PaymentType() { Type = "Dr", Detail = "Debit Card", Logo = Resource.Mipmap.RPaymentDebit, color = "#E32D49" },
                new Model.PaymentType() { Type = "GV", Detail = "Gift Voucher", Logo = Resource.Mipmap.RPaymentGiftvoucher, color = "#37AA52" },
                new Model.PaymentType() { Type = "MYQR", Detail = "myQR", Logo = Resource.Mipmap.RPaymentMyQR, color = "#F75600" },
                new Model.PaymentType() { Type = "QRCH", Detail = "QR Cash", Logo = Resource.Mipmap.RPaymentQrCash, color = "#3F51B5" },
                new Model.PaymentType() { Type = "QRCR", Detail = "QR Credit", Logo = Resource.Mipmap.RPaymentQrCredit, color = "#00796B" },
                new Model.PaymentType() { Type = "WECHAT", Detail = "WECHAT", Logo = Resource.Mipmap.RPaymentWechat, color = "#8BC34A" }
        };

        private static string BestSellBy;
        ChartView chartView, chartView2;
        RecyclerView rcvListTime, recyclerHeaderReport, rcvListItem;
        public static string tabSelected = "";
        LinearLayout lnHeader, lnChart;
        public List<MenuTab> MenuTab { get; set; }
        LinearLayout lnSaleReport, lnCategoryReport;
        public static int filterReport;

        FrameLayout lnPDF, lnEmail, lnShare;
        LinearLayout lnFullReport;
        FrameLayout lnSearch1, lnSearch2;
        EditText textSearch1, textSearch2;
        ImageButton btnSearch1, btnSearch2;
        ImageButton imgFilter1, imgFilter2;
        string search = "";
        TextView txtAmount;
        private static string queryType;
        private static List<int> lstsysitem;
        LinearLayout lnHaveData, lnNoData, lnShowButton, lnChartSale;
        string sysbranIdSelect = "", StrBranchName;
        List<int> lstsysBranchID = new List<int>();
        private List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();

        //Share Report
        ReportSale exportSale = new ReportSale();
        DateTime ConvertStartDate, ConvertEndDate;
        string FileName, FilterDate = string.Empty, FilterPeriodDate = string.Empty;
        List<ORM.Period.SummaryHourly> GetDataSalesReport = new List<ORM.Period.SummaryHourly>();
        List<SaleReportBranch> exportSalesByBranch = new List<SaleReportBranch>();
        bool CreateReport = false;
        List<Gabana3.JAM.Report.SummaryItemModel> summaryItems = new List<Gabana3.JAM.Report.SummaryItemModel>();
        List<Gabana3.JAM.Report.SalesByPaymentResponse> paymentResponse = new List<Gabana3.JAM.Report.SalesByPaymentResponse>();
        List<EmployeeReport> employeeReports = new List<EmployeeReport>();
        List<Gabana3.JAM.Report.SalesByCustomerResponse> customerResponses = new List<Gabana3.JAM.Report.SalesByCustomerResponse>();
        List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = new List<Gabana3.JAM.Report.SalesByCategoryResponse>();
        List<ReportProfit> ReportProfits = new List<ReportProfit>();
        private List<ReportProfit> reportProfitsCustom = new List<ReportProfit>();
        Android.Graphics.Bitmap bitmap;
        LinearLayout lnTotal;
        TextView textTotalName, textGrandTotal;
        decimal totalSale;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_showdailysale);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;   
                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

                txtCustomDate = FindViewById<TextView>(Resource.Id.txtCustomDate);
                txtDate = FindViewById<TextView>(Resource.Id.txtDate);

                txtBranchName = FindViewById<TextView>(Resource.Id.txtBranchName);
                //txtBranchName.Text = BranchSelect.BranchName;

                recyclerHeaderReport = FindViewById<RecyclerView>(Resource.Id.recyclerHeaderReport);
                txtTypeTime = FindViewById<TextView>(Resource.Id.txtTypeTime);
                txtAmount = FindViewById<TextView>(Resource.Id.txtAmount);
                chartView = FindViewById<ChartView>(Resource.Id.chartView);
                chartView2 = FindViewById<ChartView>(Resource.Id.chartView2);

                lnChart = FindViewById<LinearLayout>(Resource.Id.lnChart);
                lnChart.Visibility = ViewStates.Gone;

                lnTotal = FindViewById<LinearLayout>(Resource.Id.lnTotal);
                textTotalName = FindViewById<TextView>(Resource.Id.textTotalName);
                textGrandTotal = FindViewById<TextView>(Resource.Id.textGrandTotal);
                lnTotal.Visibility = ViewStates.Gone;

                lnChartSale = FindViewById<LinearLayout>(Resource.Id.lnChartSale);
                lnFullReport = FindViewById<LinearLayout>(Resource.Id.lnFullReport);
                lnFullReport.SetBackgroundColor(Android.Graphics.Color.White);

                rcvListTime = FindViewById<RecyclerView>(Resource.Id.recyclerview_list);
                rcvListItem = FindViewById<RecyclerView>(Resource.Id.recyclerview_listGroup);
                lnHeader = FindViewById<LinearLayout>(Resource.Id.lnHeader);
                txtTitleChart = FindViewById<TextView>(Resource.Id.txtTitleChart);
                txtGroupBy = FindViewById<TextView>(Resource.Id.txtGroupBy);
                lnSaleReport = FindViewById<LinearLayout>(Resource.Id.lnSaleReport);
                lnCategoryReport = FindViewById<LinearLayout>(Resource.Id.lnCategoryReport);
                lnHaveData = FindViewById<LinearLayout>(Resource.Id.lnHaveData);
                lnNoData = FindViewById<LinearLayout>(Resource.Id.lnNoData);
                lnShowButton = FindViewById<LinearLayout>(Resource.Id.lnShowButton);
                //branchID = Convert.ToInt32(ReportDailySaleActivity.branchID);
                lnPDF = FindViewById<FrameLayout>(Resource.Id.lnPDF);
                lnEmail = FindViewById<FrameLayout>(Resource.Id.lnEmail);
                lnShare = FindViewById<FrameLayout>(Resource.Id.lnShare);
                lnPDF.Click += LnPDF_Click;
                lnEmail.Click += LnEmail_Click;
                lnShare.Click += LnShare_Click;

                lnSearch1 = FindViewById<FrameLayout>(Resource.Id.lnSearch1);
                lnSearch2 = FindViewById<FrameLayout>(Resource.Id.lnSearch2);
                btnSearch1 = FindViewById<ImageButton>(Resource.Id.btnSearch1);
                btnSearch2 = FindViewById<ImageButton>(Resource.Id.btnSearch2);
                textSearch1 = FindViewById<EditText>(Resource.Id.textSearch1);
                textSearch2 = FindViewById<EditText>(Resource.Id.textSearch2);
                imgFilter1 = FindViewById<ImageButton>(Resource.Id.imgFilter1);
                imgFilter2 = FindViewById<ImageButton>(Resource.Id.imgFilter2);

                btnSearch2.Click += BtnSearch2_Click;
                textSearch2.TextChanged += TextSearch2_TextChanged; ;
                textSearch2.KeyPress += TextSearch2_KeyPress; ;
                textSearch2.FocusChange += TextSearch2_FocusChange; ;
                imgFilter2.Click += ImgFilter2_Click;

                btnSearch1.Click += BtnSearch1_Click;
                textSearch1.TextChanged += TextSearch1_TextChanged;
                textSearch1.KeyPress += TextSearch1_KeyPress;
                textSearch1.FocusChange += TextSearch1_FocusChange;
                imgFilter1.Click += ImgFilter2_Click;
                CheckJwt();
                GetBranchSelect();
                if (TypeReport.Contains("SalesReport") || TypeReport.Contains("ProfitReport"))
                {
                    filterReport = 0;
                }
                else
                {
                    filterReport = 1;
                }
                tabSelected = "Hourly";
                SetNoData(false);
                SetTabMenu();
                SetTabShowMenu();
                SetUiFromTypeReport();

                ConvertStartDate = Utils.ChangeDateTimeStringReport(StartDate);
                ConvertEndDate = Utils.ChangeDateTimeStringReport(EndDate);


                if (lstBranch.Count != listChooseBranch.Count)
                {
                    StrBranchName = "";
                    foreach (var item in listChooseBranch)
                    {
                        if (StrBranchName != "")
                        {
                            StrBranchName += "," + item.BranchName;
                        }
                        else
                        {
                            StrBranchName = "Branch : " + item.BranchName;
                        }
                    }
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ShowReportDailySaleActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at showdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TextSearch2_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch2.Text.Trim()))
            {
                btnSearch2.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch2.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearch2_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearchItem(btnSearch2);
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                ViewReport();
                textSearch2.ClearFocus();
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
                textSearch2.Text += input;
                textSearch2.SetSelection(textSearch2.Text.Length);
                return;
            }
        }

        private void TextSearch2_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            search = textSearch2.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                ViewReport();
            }
            SetBtnSearchItem(btnSearch2);
        }

        private void TextSearch1_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch1.Text.Trim()))
            {
                btnSearch1.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch1.SetBackgroundResource(Resource.Mipmap.Search);
            }
        }

        private void TextSearch1_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearchItem(btnSearch1);
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                ViewReport();
                textSearch1.ClearFocus();
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
                textSearch1.Text += input;
                textSearch1.SetSelection(textSearch1.Text.Length);
                return;
            }
        }

        private void TextSearch1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            search = textSearch1.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                ViewReport();
            }
            SetBtnSearchItem(btnSearch1);
        }

        private void BtnSearch1_Click(object sender, EventArgs e)
        {
            SetClearSearchText(textSearch1);
            SetBtnSearchItem(btnSearch1);
            ViewReport();
        }

        private void BtnSearch2_Click(object sender, EventArgs e)
        {
            SetClearSearchText(textSearch2);
            SetBtnSearchItem(btnSearch2);
            ViewReport();

        }
        private void SetClearSearchText(EditText editText)
        {
            search = "";
            editText.Text = string.Empty;
        }
        private void SetBtnSearchItem(ImageButton button)
        {
            if (string.IsNullOrEmpty(search))
            {
                button.SetBackgroundResource(Resource.Mipmap.Search);
                button.Enabled = false;
            }
            else
            {
                button.SetBackgroundResource(Resource.Mipmap.DelTxt);
                button.Enabled = true;
            }
        }
        private async void GetBranchSelect()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                //listChooseBranch = new List<ORM.MerchantDB.Branch>();
                if (listChooseBranch.Count == 0)
                {
                    listChooseBranch = lstBranch;
                }

                if (lstBranch.Count == listChooseBranch.Count)
                {
                    txtBranchName.Text = GetString(Resource.String.addemployee_allbranch);
                }
                else
                {
                    txtBranchName.Text = "";
                    foreach (var item in listChooseBranch)
                    {
                        if (txtBranchName.Text != "")
                        {
                            txtBranchName.Text += "," + item.BranchName;
                        }
                        else
                        {
                            txtBranchName.Text = item.BranchName;
                        }
                    }
                }

                sysbranIdSelect = "";
                foreach (var item in listChooseBranch)
                {
                    if (sysbranIdSelect != "")
                    {
                        sysbranIdSelect += "," + item.SysBranchID.ToString();
                        lstsysBranchID.Add((int)item.SysBranchID);

                    }
                    else
                    {
                        sysbranIdSelect = item.SysBranchID.ToString();
                        lstsysBranchID = new List<int>();
                        lstsysBranchID.Add((int)item.SysBranchID);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetBranchSelect at showReportdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ImgFilter2_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(FilterActivity)));
            FilterActivity.SetFilter(filterReport);
        }

        private void SetNoData(bool noData)
        {
            if (noData)
            {
                lnHaveData.Visibility = ViewStates.Gone;
                lnShowButton.Visibility = ViewStates.Gone;
                lnNoData.Visibility = ViewStates.Visible;
            }
            else
            {
                lnHaveData.Visibility = ViewStates.Visible;
                lnShowButton.Visibility = ViewStates.Visible;
                lnNoData.Visibility = ViewStates.Gone;
            }
        }

        private void SetTabMenu()
        {
            if (TimeType == "Year")
            {
                MenuTab = new List<MenuTab>
                    {
                        new MenuTab() { NameMenuEn = "Hourly" , NameMenuTh = "รายชั่วโมง" },
                        new MenuTab() { NameMenuEn = "Weekly" , NameMenuTh = "รายสัปดาห์" },
                        new MenuTab() { NameMenuEn = "Monthly" , NameMenuTh = "รายเดือน" }
                    };
            }
            else
            {
                MenuTab = new List<MenuTab>
                {
                    new MenuTab() { NameMenuEn = "Hourly" , NameMenuTh = "รายชั่วโมง" },
                    new MenuTab() { NameMenuEn = "Daily" , NameMenuTh = "รายวัน" },
                    new MenuTab() { NameMenuEn = "Weekly" , NameMenuTh = "รายสัปดาห์" }
                };
            }
        }
        private void SetTabShowMenu()
        {
            try
            {
                if (tabSelected == "")
                {
                    tabSelected = "Hourly";
                }

                GridLayoutManager menuLayoutManager = new GridLayoutManager(this, 3, 1, false);
                recyclerHeaderReport.HasFixedSize = true;
                recyclerHeaderReport.SetLayoutManager(menuLayoutManager);
                Report_Adapter_Header report_adapter_header = new Report_Adapter_Header(MenuTab, tabSelected);
                recyclerHeaderReport.SetAdapter(report_adapter_header);
                report_adapter_header.ItemClick += AddItem_Adapter_Header_ItemClick;

                ViewReport();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at ReportDailySale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void AddItem_Adapter_Header_ItemClick(object sender, int e)
        {
            tabSelected = MenuTab[e].NameMenuEn;
            SetTabShowMenu();
            SetUiFromTypeReport();
        }

        private void SetUiFromTypeReport()
        {
            lnSaleReport.Visibility = ViewStates.Gone;
            txtGroupBy.Visibility = ViewStates.Gone;
            lnCategoryReport.Visibility = ViewStates.Gone;
            lnSaleReport.Visibility = ViewStates.Gone;

            switch (TimeType)
            {
                case "Date":
                    txtCustomDate.Text = GetString(Resource.String.showreport_activity_today);
                    lnHeader.Visibility = ViewStates.Gone;
                    break;
                case "Month":
                    txtCustomDate.Text = GetString(Resource.String.showreport_activity_thismonth);
                    lnHeader.Visibility = ViewStates.Visible;
                    break;
                case "Year":
                    txtCustomDate.Text = GetString(Resource.String.showreport_activity_thisyear);
                    lnHeader.Visibility = ViewStates.Visible;
                    break;
                case "CustomDate":
                    txtTypeTime.Text = GetString(Resource.String.report_daily);
                    txtCustomDate.Text = GetString(Resource.String.showreport_activity_custom);
                    lnHeader.Visibility = ViewStates.Gone;
                    break;
                default:
                    break;
            }
            txtAmount.Text = GetString(Resource.String.showreport_activity_sales);
            switch (TypeReport)
            {
                case "SalesReport":
                    txtTitle.Text = GetString(Resource.String.report_sale_report);
                    txtTitleChart.Text = GetString(Resource.String.report_sales);
                    lnSaleReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    break;
                case "SalesReportByBranch":
                    txtTitle.Text = GetString(Resource.String.report_salebybranch_report);
                    lnSaleReport.Visibility = ViewStates.Visible;
                    lnHeader.Visibility = ViewStates.Gone;
                    lnTotal.Visibility = ViewStates.Visible;
                    break;
                case "ProfitReport": 
                    txtTitle.Text = GetString(Resource.String.report_profitbycostestimate_report);
                    txtTitleChart.Text = GetString(Resource.String.dashboard_activity_profit);
                    txtAmount.Text = GetString(Resource.String.dashboard_activity_profit);
                    lnSaleReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    break;
                case "CategoryReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycategory_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    break;
                case "CustomerReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycustomer_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    break;
                case "EmployeeReport":
                    txtTitle.Text = GetString(Resource.String.report_salebyemployee_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    break;
                case "PaymentReport":
                    txtTitle.Text = GetString(Resource.String.report_salebypaymentmethod_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnChart.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    break;
                case "ReportBestSale":
                    lnHeader.Visibility = ViewStates.Gone;
                    lnSearch2.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    break;
                case "ReportBalance":
                    lnHeader.Visibility = ViewStates.Gone;
                    lnSearch2.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }

            txtDate.Text = TextDate;

            switch (tabSelected)
            {
                case "Hourly":
                    txtTitleChart.Text += GetString(Resource.String.report_byhourly);
                    txtTypeTime.Text = GetString(Resource.String.report_hour);
                    break;
                case "Daily":
                    txtTitleChart.Text += GetString(Resource.String.report_bydaily);
                    txtTypeTime.Text = GetString(Resource.String.report_daily);
                    break;
                case "Weekly":
                    txtTitleChart.Text += GetString(Resource.String.report_byweekly);
                    txtTypeTime.Text = GetString(Resource.String.report_weekly);
                    break;
                case "Monthly":
                    txtTitleChart.Text += GetString(Resource.String.report_bymonth);
                    txtTypeTime.Text = GetString(Resource.String.report_monthly);
                    break;
                default:
                    break;
            }
        }

        internal static void SetDataReport(string typeReport, string startDate, string endDate, List<Gabana.ORM.MerchantDB.Branch> b, string timeType, string textDate, string textGroup, List<Category> categories, List<Customer> customers, List<ORM.MerchantDB.UserAccountInfo> userAccountInfos, List<PaymentType> payments, string bestSellBy)
        {
            TypeReport = typeReport;
            StartDate = startDate;
            EndDate = endDate;
            listChooseBranch = b;
            TimeType = timeType;
            TextDate = textDate;
            TextGroup = textGroup;
            ListCategory = categories;
            ListCustomer = customers;
            ListEmployee = userAccountInfos;
            lsPayment = payments;
            BestSellBy = bestSellBy;
        }

        internal static void SetBalanceReport(string typeReport, string v, List<ORM.MerchantDB.Branch> b, List<int> l, string itemSelect, string v1)
        {
            TypeReport = typeReport;
            queryType = v;
            listChooseBranch = b;
            lstsysitem = l;
            TextGroup = itemSelect;
            TypeItem = v1;
        }

        private async void ViewReport()
        {
            DialogLoading dialog = new DialogLoading();
            try
            {
                if (dialog.Cancelable != false)
                {
                    dialog.Cancelable = false;
                    dialog?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                if (string.IsNullOrEmpty(EndDate))
                {
                    EndDate = StartDate;
                }
                switch (TypeReport)
                {
                    case "SalesReport":
                        await ShowSaleReport();
                        break;
                    case "SalesReportByBranch":
                        await ShowSalesReportByBranch();
                        break;
                    case "ProfitReport":
                        await ShowSaleReport();
                        break;
                    case "CategoryReport":
                        await ShowCategoryReport();
                        break;
                    case "CustomerReport":
                        await ShowCustomerReport();
                        break;
                    case "EmployeeReport":
                        await ShowEmployeeReport();
                        break;
                    case "PaymentReport":
                        await ShowPaymentReport();
                        break;
                    case "ReportBestSale":
                        await ShowBestSale();
                        break;
                    case "ReportBalance":
                        //ShowBalanceReport();
                        break;
                    default:
                        break;
                }
                if (dialog != null)
                {
                    dialog.DismissAllowingStateLoss();
                    dialog.Dismiss();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" Show report");
                if (dialog != null)
                {
                    dialog.DismissAllowingStateLoss();
                    dialog.Dismiss();
                }
                return;
            }
        }

        private async Task ShowSalesReportByBranch()
        {
            try
            {
                txtTypeTime.Text = GetString(Resource.String.report_branch);
                txtTitleChart.Text = GetString(Resource.String.report_top10salesbybranch);
                lnChartSale.Visibility = ViewStates.Gone;

                lnHeader.Visibility = ViewStates.Gone;
                //var GetDataSalesReport = await GabanaAPI.GetDataReportSummaryHourly(sysbranIdSelect, StartDate, EndDate);
                List<SalesByBranchModel> salesByBranches = new List<SalesByBranchModel>();
                salesByBranches = await GabanaAPI.GetDataReportSalesByBranchModel(StartDate, EndDate);
                totalSale = salesByBranches.Sum(x => x.sumGrandTotal);
                textGrandTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + totalSale.ToString("#,##0.00");
                textTotalName.Text = GetString(Resource.String.report_allbranch);

                //exportSalesByBranch = await CalReport.initialBranch(salesByBranches, lstBranch);
                if (salesByBranches == null || salesByBranches.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                List<SaleReportBranch> listsaleReportBranch = new List<SaleReportBranch>();
                foreach (var item in salesByBranches)
                {
                    var branch = lstBranch.Where(x => x.SysBranchID == item.sysBranchID).FirstOrDefault();
                    string branchName = "";
                    if (branch != null)
                    {
                        branchName = branch.BranchName?.ToString();
                        SaleReportBranch saleReportBranch = new SaleReportBranch()
                        {
                            BranchID = item.sysBranchID,
                            BranchName = branchName,
                            sumGrandTotal = item.sumGrandTotal,
                        };
                        listsaleReportBranch.Add(saleReportBranch);
                    }
                }
                exportSalesByBranch = new List<SaleReportBranch>();
                exportSalesByBranch = listsaleReportBranch;
                if (!string.IsNullOrEmpty(search))
                {
                    listsaleReportBranch = listsaleReportBranch.Where(x => x.BranchName.ToLower().Contains(search.ToLower())).ToList();
                }
                Report_Adapter_ShowByBranch Adapter_ShowByBranch = new Report_Adapter_ShowByBranch(listsaleReportBranch);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                gridLayoutManager.CanScrollVertically();
                rcvListTime.SetAdapter(Adapter_ShowByBranch);
                rcvListTime.SetLayoutManager(gridLayoutManager);
                rcvListTime.HasFixedSize = true;
                rcvListTime.SetItemViewCacheSize(50);
                List<ChartEntry> SaleByBranch = new List<ChartEntry>();
                //List<string> Colors = new List<string> { "#0077AF", "#0086C4", "#0086C4", "#1AA0DE", "#33AAE2", "#4DB5E5", "#66C0E9", "#80CAEC", "#99D4F0", "#B3DFF4" };


                List<string> Colors = new List<string>();
                Colors.Add("#0095DA");
                Colors.Add("#F8971D");
                Colors.Add("#37AA52");
                Colors.Add("#F75600");
                Colors.Add("#3F51B5");
                Colors.Add("#00796B");
                Colors.Add("#8BC34A");
                Colors.Add("#DD527E");

                int i = 0;

                foreach (var item in listsaleReportBranch)
                {

                    ChartEntry entry = new ChartEntry((float)item.sumGrandTotal)
                    {
                        Label = item.BranchName?.ToString(),
                        ValueLabel = item.sumGrandTotal.ToString("#,###"),
                        Color = SKColor.Parse(Colors[i])
                    };

                    SaleByBranch.Add(entry);
                    if (i < salesByBranches.Count)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                };
                for (int a = 0; a < 10; a++)
                {
                    ChartEntry entry = new ChartEntry(0)
                    {
                        Label = " ",
                        ValueLabel = "",
                        Color = SKColor.Parse("#ffffff")
                    };
                    if (salesByBranches.Count <= a)
                    {
                        SaleByBranch.Add(entry);
                    }
                }
                BarChart chart = new BarChart
                {
                    Entries = SaleByBranch,
                    LabelTextSize = 15f,
                    BackgroundColor = SKColor.Parse("#FFF"),
                    YAxisPosition = Position.Left,
                    ValueLabelOption = ValueLabelOption.None,
                    ShowYAxisLines = true,
                    ShowYAxisText = false,
                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Vertical,
                    IsAnimated = false,
                    //Margin = 10f,
                };
                chartView.Chart = chart;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowSaleReport at ShowReportDailySaleActivity");
                return;
            }
        }

        //private async void ShowBalanceReport()
        //{
        //    try
        //    {
        //        if (dialogLoading.Cancelable != false)
        //        {
        //            dialogLoading.Cancelable = false;
        //            dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
        //        }


        //        txtTitle.Text = "รายงานสินค้าคงเหลือ";
        //        txtGroupBy.Text = TextGroup;
        //        txtDate.Text = DateTime.Now.ToString("dd MMM yyyy", new CultureInfo("en-US"));
        //        Gabana3.JAM.Report.ItemsBalanceStockRequest itemsBalanceStock = new Gabana3.JAM.Report.ItemsBalanceStockRequest()
        //        {
        //            queryType = queryType,
        //            sysBranchID = (int)listChooseBranch[0].SysBranchID,
        //            sysIds = lstsysitem
        //        };
        //        List<Gabana.ORM.Master.ItemOnBranch> result = new List<ORM.Master.ItemOnBranch>();
        //        result = await GabanaAPI.GetDataReportBalanceItemsStock(itemsBalanceStock);
        //        List<Gabana.ORM.Master.ItemOnBranch> itemOnBranches = new List<Gabana.ORM.Master.ItemOnBranch>();
        //        result = result.Where(x => x.MerchantID == DataCashingAll.MerchantId).ToList();
        //        if (result == null || result.Count == 0)
        //        {
        //            SetNoData(true);
        //            dialogLoading.Dismiss();
        //            return;
        //        }
        //        List<Item> items = new List<Item>();
        //        List<Category> categories = new List<Category>();
        //        if (TypeItem == "i")
        //        {
        //            ItemManage itemManage = new ItemManage();
        //            items = await itemManage.GetAllItem();
        //        }
        //        if (TypeItem == "e")
        //        {
        //            ItemManage itemManage = new ItemManage();
        //            items = await itemManage.GetToppingItem();
        //        }
        //        if (TypeItem == "c")
        //        {
        //            ItemManage itemManage = new ItemManage();
        //            var item = await itemManage.GetAllItem();
        //            var topping = await itemManage.GetToppingItem();
        //            items.AddRange(item);
        //            items.AddRange(topping);
        //        }
        //        List<Item> selectSys = new List<Item>();

        //        if (TypeItem == "c")
        //        {
        //            foreach (var sysCategoryID in lstsysitem)
        //            {
        //                var listitem = items.Where(x => x.SysCategoryID == sysCategoryID).ToList();
        //                foreach (var item in listitem)
        //                {
        //                    selectSys.Add(item);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var i in lstsysitem)
        //            {
        //                var sys = items.Where(x => x.SysItemID == i).FirstOrDefault();
        //                if (sys != null)
        //                {
        //                    selectSys.Add(sys);
        //                }
        //            }
        //        }

        //        if (filterReport == 3)
        //        {
        //            selectSys = selectSys.OrderBy(x => x.ItemName).ToList();
        //        }
        //        if (filterReport == 4)
        //        {
        //            selectSys = selectSys.OrderByDescending(x => x.ItemName).ToList();
        //        }
        //        List<ItemOnBranch> itemOns = new List<ItemOnBranch>();
        //        //foreach (var i in selectSys)
        //        //{
        //        //    var a = result.Where(x => x.SysItemID == i.SysItemID).ToList();
        //        //    ItemOnBranch onBranch = new ItemOnBranch()
        //        //    {
        //        //        MerchantID = a[0].MerchantID,
        //        //        SysBranchID = a[0].SysBranchID,
        //        //        SysItemID = a[0].SysItemID,
        //        //        BalanceStock = a.Sum(x => x.BalanceStock),
        //        //        MinimumStock = a[0].MinimumStock
        //        //    };
        //        //    itemOns.Add(onBranch);
        //        //}
        //        foreach (var i in selectSys)
        //        {
        //            var a = result.LastOrDefault(x => x.SysItemID == i.SysItemID);
        //            ItemOnBranch onBranch = new ItemOnBranch()
        //            {
        //                MerchantID = a.MerchantID,
        //                SysBranchID = a.SysBranchID,
        //                SysItemID = a.SysItemID,
        //                BalanceStock = a.BalanceStock,
        //                MinimumStock = a.MinimumStock
        //            };
        //            itemOns.Add(onBranch);
        //        }
        //        if (filterReport == 1)
        //        {
        //            itemOns = itemOns.OrderByDescending(x => x.BalanceStock).ToList();
        //        }
        //        if (filterReport == 2)
        //        {
        //            itemOns = itemOns.OrderBy(x => x.BalanceStock).ToList();
        //        }
        //        if (itemOns.Count != selectSys.Count)
        //        {
        //            return;
        //        }

        //        Report_Adapter_ItemBalance report_adapter_itembalance = new Report_Adapter_ItemBalance(itemOns, selectSys);
        //        GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
        //        recyclerview_listGroup.SetAdapter(report_adapter_itembalance);
        //        recyclerview_listGroup.SetLayoutManager(gridLayoutItem);
        //        recyclerview_listGroup.HasFixedSize = true;

        //        if (dialogLoading != null)
        //        {
        //            dialogLoading.DismissAllowingStateLoss();
        //            dialogLoading.Dismiss();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        dialogLoading.Dismiss();
        //        await TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("ShowBalanceReport at showdailysale");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //        return;
        //    }
        //}
        private async Task ShowBestSale()
        {
            try
            {
                txtGroupBy.Visibility = ViewStates.Gone;
                var result = await GabanaAPI.GetDataReportSummaryDailyItem(sysbranIdSelect, StartDate, EndDate);
                if (result == null || result.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                summaryItems = new List<Gabana3.JAM.Report.SummaryItemModel>();
                switch (BestSellBy)
                {
                    case "Sell":
                        txtTitle.Text = "รายงานสินค้าขายดีตามยอดขาย";
                        summaryItems = result.OrderByDescending(x => x.SumTotalAmount).ToList();
                        break;
                    case "Unit":
                        txtTitle.Text = "รายงานสินค้าขายดีตามจำนวน";
                        summaryItems = result.OrderByDescending(x => x.SumQuantity).ToList();
                        break;
                    default:
                        break;
                }
                switch (filterReport)
                {
                    case 1:
                        if (BestSellBy == "Sell")
                        {
                            summaryItems = result.OrderByDescending(x => x.SumTotalAmount).ToList();
                        }
                        if (BestSellBy == "Unit")
                        {
                            summaryItems = result.OrderByDescending(x => x.SumQuantity).ToList();
                        }
                        break;
                    case 2:
                        if (BestSellBy == "Sell")
                        {
                            summaryItems = result.OrderBy(x => x.SumTotalAmount).ToList();
                        }
                        if (BestSellBy == "Unit")
                        {
                            summaryItems = result.OrderBy(x => x.SumQuantity).ToList();
                        }
                        break;
                    case 3:
                        summaryItems = result.OrderBy(x => x.ItemName).ToList();
                        break;
                    case 4:
                        summaryItems = result.OrderByDescending(x => x.ItemName).ToList();
                        break;
                    default:
                        break;
                }
                List<Item> items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItemType();
                if (!string.IsNullOrEmpty(search))
                {
                    summaryItems = summaryItems.Where(x => x.ItemName.ToLower().Contains(search.ToLower())).ToList();
                }
                Report_Adapter_Item report_adapter_item = new Report_Adapter_Item(summaryItems, items, BestSellBy);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                rcvListItem.SetAdapter(report_adapter_item);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBestSale at showdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async Task ShowPaymentReport()
        {
            try
            {
                txtGroupBy.Text = TextGroup;
                txtGroupBy.Visibility = ViewStates.Visible;
                List<string> payments = new List<string>();
                //if (lsPayment.Count == paymentTypes.Count)
                //{
                //    payments = null;

                //}
                //else
                //{
                //    foreach (var item in lsPayment)
                //    {
                //        payments.Add(item.Type);
                //    }

                //}
                foreach (var item in lsPayment)
                {
                    payments.Add(item.Type);
                }

                Gabana3.JAM.Report.SalesByPaymentRequest paymentRequest = new Gabana3.JAM.Report.SalesByPaymentRequest();
                paymentRequest.sysBranchIDs = lstsysBranchID;
                paymentRequest.startDate = StartDate;
                paymentRequest.endDate = EndDate;
                paymentRequest.paymentTypes = payments;
                paymentResponse = new List<Gabana3.JAM.Report.SalesByPaymentResponse>();

                var result = await GabanaAPI.GetDataReportSummaryDailyPayment(paymentRequest);

                if (result == null || result.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                switch (filterReport)
                {
                    case 1:
                        paymentResponse = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                        break;
                    case 2:
                        paymentResponse = result.OrderBy(x => x.sumTotalAmount).ToList();
                        break;
                    case 3:
                        paymentResponse = result.OrderBy(x => x.paymentType).ToList();
                        break;
                    case 4:
                        paymentResponse = result.OrderByDescending(x => x.paymentType).ToList();
                        break;
                    default:
                        break;
                }

                List<ChartEntry> SalesByPayments = new List<ChartEntry>();
                foreach (var item in paymentResponse)
                {
                    var index = paymentTypes.FindIndex(x => x.Type.ToUpper().Contains(item.paymentType.ToUpper()));
                    if (index != -1)
                    {
                        var colorType = paymentTypes.Where(x => x.Type.ToUpper().Contains(item.paymentType.ToUpper())).Select(x => x.color).FirstOrDefault();
                        var defaulf = SKColor.Parse("#0095DA");
                        SKColor.TryParse(colorType, out defaulf);
                        ChartEntry entry = new ChartEntry((float)item.percentTotal)
                        {
                            Label = Utils.SetPaymentNameChart(item.paymentType.ToString()),
                            ValueLabel = item.percentTotal.ToString("##0.00") + "%",
                            Color = defaulf
                        };
                        SalesByPayments.Add(entry);
                    }
                }
                var chart2 = new DonutChart() { Entries = SalesByPayments, HoleRadius = 0.7f, LabelMode = LabelMode.None };
                chartView2.Chart = chart2;

                if (!string.IsNullOrEmpty(search))
                {
                    paymentResponse = paymentResponse.Where(x => Utils.SetPaymentName(x.paymentType).ToLower().Contains(search.ToLower())).ToList();
                }
                Report_Adapter_ShowPayment report_adapter_payment = new Report_Adapter_ShowPayment(paymentResponse, SalesByPayments);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                rcvListItem.SetAdapter(report_adapter_payment);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowPaymentReport at showdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async Task ShowEmployeeReport()
        {
            try
            {
                txtGroupBy.Text = TextGroup;
                txtGroupBy.Visibility = ViewStates.Visible;
                UserAccountInfoManage userAccountInfoManage = new UserAccountInfoManage();
                var lstEmployee = await userAccountInfoManage.GetAllUserAccount();
                List<string> employeeName = new List<string>();
                foreach (var item in ListEmployee)
                {
                    employeeName.Add(item.UserName);
                }
                if (lstEmployee.Count == ListEmployee.Count)
                {
                    employeeName = null;
                }
                Gabana3.JAM.Report.SalesBySellerRequest employeeRequest = new Gabana3.JAM.Report.SalesBySellerRequest();
                employeeRequest.sysBranchIDs = lstsysBranchID;
                employeeRequest.startDate = StartDate;
                employeeRequest.endDate = EndDate;
                employeeRequest.sellerName = employeeName;
                List<Gabana3.JAM.Report.SalesBySellerResponse> sellerResponses = new List<Gabana3.JAM.Report.SalesBySellerResponse>();
                var result = await GabanaAPI.GetDataReportSummaryDailySeller(employeeRequest);
                if (result == null || result.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                switch (filterReport)
                {
                    case 1:
                        sellerResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                        break;
                    case 2:
                        sellerResponses = result.OrderBy(x => x.sumTotalAmount).ToList();
                        break;
                    case 3:
                        sellerResponses = result.OrderBy(x => x.sellerName).ToList();
                        break;
                    case 4:
                        sellerResponses = result.OrderByDescending(x => x.sellerName).ToList();

                        break;
                    default:
                        break;
                }
                employeeReports = new List<EmployeeReport>();
                foreach (var item in sellerResponses)
                {
                    var dataEmpPosition = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == item.sellerName.ToLower()).FirstOrDefault();
                    string position = "";
                    if (dataEmpPosition != null)
                    {
                        position = dataEmpPosition.MainRoles?.ToString();
                    }
                    EmployeeReport employeeReport = new EmployeeReport()
                    {
                        sellerName = item.sellerName,
                        MainRoles = position,
                        sumTotalAmount = item.sumTotalAmount
                    };
                    employeeReports.Add(employeeReport);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    sellerResponses = sellerResponses.Where(x => x.sellerName.ToLower().Contains(search.ToLower())).ToList();
                }
                Report_Adapter_ShowEmployee report_adapter_showCustomer = new Report_Adapter_ShowEmployee(sellerResponses, lstEmployee);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                rcvListItem.SetAdapter(report_adapter_showCustomer);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowEmployeeReport at showdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async Task ShowCustomerReport()
        {
            try
            {
                txtGroupBy.Visibility = ViewStates.Visible;
                txtGroupBy.Text = TextGroup;
                CustomerManage customerManage = new CustomerManage();
                var listCustomer = await customerManage.GetAllCustomer();
                List<string> customerName = new List<string>();
                foreach (var item in ListCustomer)
                {
                    customerName.Add(item.CustomerName);
                }
                if (listCustomer.Count == ListCustomer.Count)
                {
                    customerName = null;
                }
                Gabana3.JAM.Report.SalesByCustomerRequest customerRequest = new Gabana3.JAM.Report.SalesByCustomerRequest();
                customerRequest.sysBranchIDs = lstsysBranchID;
                customerRequest.startDate = StartDate;
                customerRequest.endDate = EndDate;
                customerRequest.customerName = customerName;
                customerResponses = new List<Gabana3.JAM.Report.SalesByCustomerResponse>();
                var result = await GabanaAPI.GetDataReportSummaryDailyCustomer(customerRequest);
                if (result == null || result.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                switch (filterReport)
                {
                    case 1:
                        customerResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                        break;
                    case 2:
                        customerResponses = result.OrderBy(x => x.sumTotalAmount).ToList();
                        break;
                    case 3:
                        customerResponses = result.OrderBy(x => x.customerName).ToList();
                        break;
                    case 4:
                        customerResponses = result.OrderByDescending(x => x.customerName).ToList();
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(search))
                {
                    customerResponses = customerResponses.Where(x => x.customerName.ToLower().Contains(search.ToLower())).ToList();
                }
                //List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                Report_Adapter_ShowCustomer report_adapter_showCustomer = new Report_Adapter_ShowCustomer(customerResponses, listCustomer);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                rcvListItem.SetAdapter(report_adapter_showCustomer);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCustomerReport at showdailysale");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async Task ShowCategoryReport()
        {
            try
            {
                string Language = Preferences.Get("Language", "");

                CategoryManage category = new CategoryManage();
                var lstCategory = await category.GetAllCategory();


                List<string> categotyName = new List<string>();

                if (lstCategory.Count == ListCategory.Count)
                {
                    categotyName = null;
                }
                else
                {
                    foreach (var item in ListCategory)
                    {
                        categotyName.Add(item.Name);
                    }
                }
                Gabana3.JAM.Report.SalesByCategoryRequest categoryRequest = new Gabana3.JAM.Report.SalesByCategoryRequest();
                categoryRequest.sysBranchIDs = lstsysBranchID;
                categoryRequest.startDate = StartDate;
                categoryRequest.endDate = EndDate;
                categoryRequest.categoriesName = categotyName;

                var result = await GabanaAPI.GetDataReportSummaryDailyCategory(categoryRequest);

                if (result == null || result.Count == 0)
                {
                    SetNoData(true);
                    return;
                }

                string namenocate;
                decimal amountnocate = 0;
                if (Language == "th")
                {
                    namenocate = "ไม่มีหมวดหมู่สินค้า";
                }
                else
                {
                    namenocate = "None Category";
                }

                categoryResponses = new List<Gabana3.JAM.Report.SalesByCategoryResponse>();

                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.categoryName))
                    {
                        amountnocate += item.sumTotalAmount;
                    }
                    else
                    {
                        categoryResponses.Add(item);
                    }
                }
                Gabana3.JAM.Report.SalesByCategoryResponse nocate = new Gabana3.JAM.Report.SalesByCategoryResponse()
                {
                    categoryName = namenocate,
                    sumTotalAmount = amountnocate
                };
                if (nocate.sumTotalAmount > 0)
                {
                    categoryResponses.Add(nocate);
                }

                switch (filterReport)
                {
                    case 1:
                        categoryResponses = categoryResponses.OrderByDescending(x => x.sumTotalAmount).ToList();
                        break;
                    case 2:
                        categoryResponses = categoryResponses.OrderBy(x => x.sumTotalAmount).ToList();
                        break;
                    case 3:
                        categoryResponses = categoryResponses.OrderBy(x => x.categoryName).ToList();
                        break;
                    case 4:
                        categoryResponses = categoryResponses.OrderByDescending(x => x.categoryName).ToList();
                        break;
                    default:
                        break;
                }
                //List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();

                txtGroupBy.Text = TextGroup;

                if (!string.IsNullOrEmpty(search))
                {
                    categoryResponses = categoryResponses.Where(x => x.categoryName.ToLower().Contains(search.ToLower())).ToList();
                }

                Report_Adapter_ShowCategory report_adapter_showCategory = new Report_Adapter_ShowCategory(categoryResponses);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                rcvListItem.SetAdapter(report_adapter_showCategory);
                rcvListItem.SetLayoutManager(gridLayoutItem);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync(" ShowCategoryReport at ShowReportDailySaleActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async Task ShowSaleReport()
        {
            try
            {
                ReportSale reportSale = new ReportSale();
                GetDataSalesReport = new List<ORM.Period.SummaryHourly>();
                GetDataSalesReport = await GabanaAPI.GetDataReportSummaryHourly(sysbranIdSelect, StartDate, EndDate);
                if (GetDataSalesReport == null || GetDataSalesReport.Count == 0)
                {
                    SetNoData(true);
                    return;
                }
                if (TypeReport == "SalesReport")
                {
                    reportSale = await CalReport.CalReportSale(GetDataSalesReport);
                    totalSale = GetDataSalesReport.Sum(x => x.SumGrandTotal);
                    textTotalName.Text = GetString(Resource.String.showreport_activity_totalsales);
                }
                if (TypeReport == "ProfitReport")
                {
                    reportSale = await CalReport.CalReportProfit(GetDataSalesReport);
                    totalSale = GetDataSalesReport.Sum(x => x.SumProfitAmount);
                    textTotalName.Text = GetString(Resource.String.showreport_activity_totalprofits);
                }
                textGrandTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + totalSale.ToString("#,##0.00");

                List<ChartEntry> SalesByPeriods = new List<ChartEntry>();
                if (tabSelected == "Hourly")
                {
                    List<ReportHourly> reportHourly = new List<ReportHourly>();
                    reportHourly = reportSale.reportHourlies;
                    if (!string.IsNullOrEmpty(search))
                    {
                        reportHourly = reportHourly.Where(x => x.Hourlyname.ToLower().Contains(search.ToLower())).ToList();
                    }
                    switch (filterReport)
                    {
                        case 0:
                            reportHourly = reportHourly.OrderBy(x => x.Hourlyname).ToList();
                            break;
                        case 1:
                            reportHourly = reportHourly.OrderByDescending(x => x.value).ToList();
                            break;
                        case 2:
                            reportHourly = reportHourly.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            break;
                    }
                    Report_Adapter_ShowHour report_adapter_showHour = new Report_Adapter_ShowHour(reportHourly);
                    rcvListTime.SetAdapter(report_adapter_showHour);
                    //foreach (var item in reportSale.reportHourlies)
                    //{
                    //    ChartEntry entry = new Entry((float)item.value)
                    //    {
                    //        Label = item.Hourlyname.ToString(),
                    //        ValueLabel = item.value.ToString("#,###.##"),
                    //        Color = SKColor.Parse("#0095DA")
                    //    };
                    //    if (item.IdHourly % 3 == 0)
                    //    {
                    //        SalesByPeriods.Add(entry);
                    //    }
                    //}

                    var firstIndex = reportSale.reportHourlies.FindIndex(x => x.value > 0) - 1;
                    var lastIndex = reportSale.reportHourlies.FindLastIndex(x => x.value > 0) + 2;
                    if (firstIndex < 0) firstIndex = 0;
                    if (lastIndex > reportSale.reportHourlies.Count) lastIndex = reportSale.reportHourlies.Count;

                    var dif = Math.Abs(lastIndex - firstIndex);
                    if (dif > 16)
                    {
                        foreach (var item in reportSale.reportHourlies)
                        {
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Hourlyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            if (item.IdHourly == firstIndex || item.IdHourly == lastIndex)
                            {
                                SalesByPeriods.Add(entry);
                            }
                            else if (item.IdHourly % 3 == 0 && item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriods.Add(entry);
                            }
                        }
                    }
                    else if (dif > 8)
                    {
                        firstIndex = reportSale.reportHourlies.FindIndex(x => x.value > 0);
                        lastIndex = reportSale.reportHourlies.FindLastIndex(x => x.value > 0);
                        if (firstIndex % 3 != 0) firstIndex = firstIndex - (firstIndex % 3);
                        if (lastIndex % 3 != 0) lastIndex = lastIndex + (3 - (lastIndex % 3));

                        foreach (var item in reportSale.reportHourlies)
                        {
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Hourlyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                Color = SKColor.Parse("#0095DA"),
                            };

                            ChartEntry entry2 = new ChartEntry((float)item.value)
                            {
                                Label = "",
                                TextColor = SKColor.Parse("#fff"),
                                ValueLabel = item.value.ToString("#,###"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            if (item.IdHourly == firstIndex || item.IdHourly == lastIndex)
                            {
                                SalesByPeriods.Add(entry);
                            }
                            else if (item.IdHourly % 3 == 0 && item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriods.Add(entry);
                            }
                            else if (item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriods.Add(entry2);
                            }
                        }
                    }
                    else
                    {
                        for (int i = firstIndex; i < lastIndex; i++)
                        {
                            var item = reportSale.reportHourlies[i];
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Hourlyname.ToString(),
                                ValueLabel = item.value.ToString("#,###.##"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            SalesByPeriods.Add(entry);
                        }
                    }
                    exportSale = new ReportSale();
                    exportSale = reportSale;
                }
                if (tabSelected == "Daily")
                {
                    lnChartSale.Visibility = ViewStates.Visible;

                    List<ReportDaily> reportDaily = new List<ReportDaily>();
                    reportDaily = reportSale.reportDailies;
                    if (!string.IsNullOrEmpty(search))
                    {
                        reportDaily = reportDaily.Where(x => x.Dailyname.ToLower().Contains(search.ToLower())).ToList();
                    }
                    switch (filterReport)
                    {
                        case 0:
                            reportDaily = reportDaily.OrderBy(x => int.Parse(x.Dailyname)).ToList();
                            break;
                        case 1:
                            reportDaily = reportDaily.OrderByDescending(x => x.value).ToList();
                            break;
                        case 2:
                            reportDaily = reportDaily.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            break;
                    }
                    Report_Adapter_ShowDaily report_adapter_showHour = new Report_Adapter_ShowDaily(reportDaily);
                    rcvListTime.SetAdapter(report_adapter_showHour);
                    foreach (var item in reportSale.reportDailies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Dailyname.ToString(),
                            ValueLabel = item.value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA")
                        };
                        // เพิ่มเงื่อนไขเพื่อให้แสดงข้อมูลทุก 4 วันในแนวแกน x
                        if (item.IdDaily == 1 || item.IdDaily == reportSale.reportDailies.Count || item.IdDaily % 4 == 0)
                        {
                            entry.Label = item.Dailyname.ToString();
                        }
                        else
                        {
                            entry.Label = "";
                        }
                        SalesByPeriods.Add(entry);
                    }
                }
                if (tabSelected == "Weekly")
                {
                    lnChartSale.Visibility = ViewStates.Visible;

                    List<ReportWeekly> reportWeeklies = new List<ReportWeekly>();
                    reportWeeklies = reportSale.reportWeeklies;
                    if (!string.IsNullOrEmpty(search))
                    {
                        reportWeeklies = reportWeeklies.Where(x => x.Weeklyname.ToLower().Contains(search.ToLower())).ToList();
                    }
                    switch (filterReport)
                    {
                        case 0:
                            reportWeeklies = reportWeeklies.OrderBy(x => x.Weeklyname).ToList();
                            break;
                        case 1:
                            reportWeeklies = reportWeeklies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 2:
                            reportWeeklies = reportWeeklies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            break;
                    }
                    Report_Adapter_ShowWeekly report_adapter_showHour = new Report_Adapter_ShowWeekly(reportSale.reportWeeklies);
                    rcvListTime.SetAdapter(report_adapter_showHour);
                    foreach (var item in reportSale.reportWeeklies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Weeklyname.ToString(),
                            ValueLabel = item.value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods.Add(entry);
                    }
                }
                if (tabSelected == "Monthly")
                {
                    lnChartSale.Visibility = ViewStates.Visible;

                    List<ReportMonthly> reportMonthlies = new List<ReportMonthly>();
                    reportMonthlies = reportSale.reportMonthlies;
                    if (!string.IsNullOrEmpty(search))
                    {
                        reportMonthlies = reportMonthlies.Where(x => x.Monthlyname.ToLower().Contains(search.ToLower())).ToList();
                    }
                    switch (filterReport)
                    {
                        case 0:
                            reportMonthlies = reportMonthlies.OrderBy(x => x.Monthlyname).ToList();
                            break;
                        case 1:
                            reportMonthlies = reportMonthlies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 2:
                            reportMonthlies = reportMonthlies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            break;
                    }
                    Report_Adapter_ShowMonthly report_adapter_showMonthly = new Report_Adapter_ShowMonthly(reportSale.reportMonthlies);
                    rcvListTime.SetAdapter(report_adapter_showMonthly);
                    foreach (var item in reportSale.reportMonthlies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Monthlyname.ToString(),
                            ValueLabel = item.value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods.Add(entry);
                    }
                }
                if (TimeType == "CustomDate")
                {
                    lnChartSale.Visibility = ViewStates.Gone;

                    DateTime begin = ConvertStartDate; //some start date
                    DateTime end = ConvertEndDate;//some end date
                    List<DateTime> dates = new List<DateTime>();
                    for (DateTime date = begin; date <= end; date = date.AddDays(1))
                    {
                        dates.Add(date);
                    }

                    //GetDataSalesReport = GetDataSalesReport.GroupBy(x=> x.DateModified.Date).Select(x=>x.Sum<>)
                    List<ORM.Period.SummaryHourly> listCustom = new List<ORM.Period.SummaryHourly>();
                    listCustom = GetDataSalesReport.GroupBy(x => x.DateModified.Date).Select(x => new ORM.Period.SummaryHourly
                    {
                        DateModified = x.Key,
                        SumGrandTotal = x.Sum(x => x.SumGrandTotal),
                        SumProfitAmount = x.Sum(x => x.SumProfitAmount)
                    }).ToList();

                    reportProfitsCustom = new List<ReportProfit>();
                    foreach (var date in dates)
                    {
                        ReportProfit reportProfit = new ReportProfit();

                        CultureInfo cultureInfo = new CultureInfo("en-US");
                        var timezoneslocal = TimeZoneInfo.Local;
                        reportProfit.dateTime = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                        reportProfit.sumGrandTotal = GetDataSalesReport.Where(x => x.DateModified.Date == date.Date).Sum(x => x.SumGrandTotal);
                        reportProfit.sumProfitTotal = GetDataSalesReport.Where(x => x.DateModified.Date == date.Date).Sum(x => x.SumProfitAmount);
                        reportProfitsCustom.Add(reportProfit);
                    }
                    if (!string.IsNullOrEmpty(search))
                    {
                        reportProfitsCustom = reportProfitsCustom.Where(x => x.dateTime.ToLower().Contains(search.ToLower())).ToList();
                    }

                    switch (filterReport)
                    {
                        case 0:
                            reportProfitsCustom = reportProfitsCustom.OrderBy(x => Convert.ToDateTime(x.dateTime)).ToList();
                            break;
                        case 1:
                            if (TypeReport == "SalesReport")
                            {
                                reportProfitsCustom = reportProfitsCustom.OrderByDescending(x => x.sumGrandTotal).ToList();

                            }
                            if (TypeReport == "ProfitReport")
                            {
                                reportProfitsCustom = reportProfitsCustom.OrderByDescending(x => x.sumProfitTotal).ToList();

                            }
                            break;
                        case 2:
                            if (TypeReport == "SalesReport")
                            {
                                reportProfitsCustom = reportProfitsCustom.OrderBy(x => x.sumGrandTotal).ToList();

                            }
                            if (TypeReport == "ProfitReport")
                            {
                                reportProfitsCustom = reportProfitsCustom.OrderBy(x => x.sumProfitTotal).ToList();

                            }
                            break;
                        default:
                            break;
                    }
                    Report_Adapter_ShowCustom report_adapter_showcustom = new Report_Adapter_ShowCustom(reportProfitsCustom, TypeReport);
                    rcvListTime.SetAdapter(report_adapter_showcustom);
                    SalesByPeriods = new List<ChartEntry>();
                    foreach (var item in listCustom)
                    {
                        decimal value = 0;
                        if (TypeReport == "SalesReport")
                        {
                            value = item.SumGrandTotal;

                        }
                        if (TypeReport == "ProfitReport")
                        {
                            value = item.SumProfitAmount;
                        }
                        ChartEntry entry = new ChartEntry((float)item.SumGrandTotal)
                        {
                            Label = item.DateModified.Date.ToString("dd/MM/yyyy"),
                            ValueLabel = value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods.Add(entry);
                    }
                }

                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                gridLayoutManager.CanScrollVertically();
                rcvListTime.SetLayoutManager(gridLayoutManager);
                rcvListTime.HasFixedSize = true;
                rcvListTime.SetItemViewCacheSize(50);
                //var chart = new LineChart()
                //{
                //    Entries = SalesByPeriods,
                //    LabelTextSize = 16f,
                //    BackgroundColor = SKColor.Parse("#FFF"),
                //    LineMode = LineMode.Straight,
                //    PointMode = PointMode.None,
                //    ShowYAxisLines = true,
                //    ShowYAxisText = true,
                //    ValueLabelOption = ValueLabelOption.None,
                //    YAxisPosition = Position.Left,
                //    LabelOrientation = Microcharts.Orientation.Horizontal,
                //    ValueLabelOrientation = Microcharts.Orientation.Vertical,
                //    IsAnimated = false,
                //    //Series
                //    //LineAreaAlpha = byte.MaxValue
                //};

                LineChart chart = new LineChart();
                if (tabSelected == "Daily") //วันที่แบบ Vertical
                {
                    chart = new LineChart()
                    {
                        Entries = SalesByPeriods,
                        LabelTextSize = 16f,
                        BackgroundColor = SKColor.Parse("#FFF"),
                        LineMode = LineMode.Straight,
                        PointMode = PointMode.None,
                        ShowYAxisLines = true,
                        ShowYAxisText = true,
                        ValueLabelOption = ValueLabelOption.None,
                        YAxisPosition = Position.Left,
                        LabelOrientation = Microcharts.Orientation.Vertical,
                        ValueLabelOrientation = Microcharts.Orientation.Vertical,
                        IsAnimated = false,

                    };
                }
                else //วันที่แบบ Horizontal
                {
                    chart = new LineChart()
                    {
                        Entries = SalesByPeriods,
                        LabelTextSize = 16f,
                        BackgroundColor = SKColor.Parse("#FFF"),
                        LineMode = LineMode.Straight,
                        PointMode = PointMode.None,
                        ShowYAxisLines = true,
                        ShowYAxisText = true,
                        ValueLabelOption = ValueLabelOption.None,
                        YAxisPosition = Position.Left,
                        LabelOrientation = Microcharts.Orientation.Horizontal,
                        ValueLabelOrientation = Microcharts.Orientation.Vertical,
                        IsAnimated = false,
                    };
                }

                chartView.Chart = chart;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(" ShowSaleReport at ShowReportDailySaleActivity");
                return;
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            DeletePictureinFolder();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override void OnResume()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                base.OnResume();
                CheckJwt();
                ViewReport();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at ShowReportDailySaleActivity");
                base.OnRestart();

            }
        }

        //ลบรูปที่สร้างตอน share,Email ,PDF
        void DeletePictureinFolder()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(DataCashingAll.PathImageBill);

            foreach (FileInfo file in di.GetFiles())
            {
                string name = file.FullName;
                if (name.Contains(".csv"))
                {
                    file.Delete();
                }
            }
        }

        private async void LnShare_Click(object sender, EventArgs e)
        {
            try
            {
                string Fullname = CreateFileCSV();
                createDetail(Fullname);
                //CreateReport สร้างไฟล์สำเร็จหรือไม่
                if (CreateReport)
                {
                    await SaveandShare(Fullname, FileName, FileName);
                }
                else
                {
                    Toast.MakeText(this, "ไม่สำเร็จ", ToastLength.Short);
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public Task SaveandShare(string fullName, string message, string title)
        {
            try
            {
                Java.IO.File file = new Java.IO.File(fullName);

                Android.Net.Uri imageUri;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    imageUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    imageUri = Android.Net.Uri.FromFile(file);
                }

                var intent = new Intent();
                intent.SetAction(Intent.ActionSend);
                intent.SetType("text/*");
                intent.PutExtra(Intent.ExtraText, message);
                intent.PutExtra(Intent.ExtraSubject, title ?? string.Empty);
                intent.PutExtra(Intent.ExtraStream, imageUri);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset);

                var chooserIntent = Intent.CreateChooser(intent, title);
                this.StartActivity(chooserIntent);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Show at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        private async void LnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                string Fullname = CreateFileCSV();
                createDetail(Fullname);
                if (CreateReport)
                {
                    await SaveandShowEmail(Fullname, FileName, FileName);
                }
                else
                {
                    Toast.MakeText(this, "ไม่สำเร็จ", ToastLength.Short);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public Task SaveandShowEmail(string fullName, string message, string title)
        {
            try
            {
                Java.IO.File file = new Java.IO.File(fullName);

                Android.Net.Uri Attachments;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    Attachments = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    Attachments = Android.Net.Uri.FromFile(file);
                }

                var intentemail = new Intent();
                intentemail.SetAction(Intent.ActionSendto);
                intentemail.SetData(Android.Net.Uri.Parse("mailto:"));

                var intent = new Intent();
                intent.SetAction(Intent.ActionSend);
                intent.SetType("text/*");
                intent.SetData(Android.Net.Uri.Parse("mailto:"));
                intent.PutExtra(Intent.ExtraText, message);
                intent.PutExtra(Intent.ExtraEmail, new String[] { });
                intent.PutExtra(Intent.ExtraSubject, title ?? string.Empty);
                intent.PutExtra(Intent.ExtraStream, Attachments);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.GrantWriteUriPermission);
                intent.SetFlags(ActivityFlags.ClearWhenTaskReset);
                intent.Selector = intentemail;

                var chooserIntent = Intent.CreateChooser(intent, "Share Email");
                this.StartActivity(chooserIntent);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowEmail at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        private async void LnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                string Fullname = CreateFilePDF();
                CreateBitmap();
                await CreatePDF(Fullname);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public Task CreatePDF(string FullName)
        {
            try
            {   //Create image 
                var fullpathpng = FilterDate + TextDate + ".png";
                string filePathpng = DataCashingAll.PathImageBill;
                string fullNamepng = filePathpng + fullpathpng;
                if (File.Exists(fullNamepng))
                {
                    File.Delete(fullNamepng);
                }

                using (var os = new System.IO.FileStream(fullNamepng, System.IO.FileMode.CreateNew))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
                }

                var ImageInfo = bitmap.GetBitmapInfo();

                //Create path PDF
                var fullpath = FullName;

                int newWidth, newHeight;
                //newWidth = (int)ImageInfo.Width / 2;
                //newHeight = (int)ImageInfo.Height / 2;

                newWidth = (int)ImageInfo.Width;
                newHeight = (int)ImageInfo.Height;

                iTextSharp.text.Document document = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(newWidth, newHeight));
                using (var stream = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var imageStream = new FileStream(fullNamepng, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var image = iTextSharp.text.Image.GetInstance(imageStream);
                        image.ScaleAbsolute(newWidth, newHeight);
                        image.SetAbsolutePosition(0, 0);
                        document.Add(image);
                    }
                    document.Close();
                }

                Java.IO.File file = new Java.IO.File(fullpath);

                Android.Net.Uri pdfUri;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    pdfUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", file);
                }
                else
                {
                    pdfUri = Android.Net.Uri.FromFile(file);
                }

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(pdfUri, "application/pdf");
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetFlags(ActivityFlags.NoHistory);
                intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

                var chooserIntent = Intent.CreateChooser(intent, "Open File");
                try
                {
                    this.StartActivity(chooserIntent);
                }
                catch (ActivityNotFoundException)
                {
                    Toast.MakeText(this, GetString(Resource.String.nonapplication), ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        async void CreateBitmap()
        {
            try
            {
                int width = lnFullReport.Width;
                int height = lnFullReport.Height;

                bitmap = Android.Graphics.Bitmap.CreateBitmap(width, height, Android.Graphics.Bitmap.Config.Argb8888);
                Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(bitmap);
                canvas.DrawColor(Android.Graphics.Color.White);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    canvas.DrawBitmap(bitmap, width, height, null);
                }
                else
                {
                    canvas.SetViewport(width, height);
                }
                lnFullReport.Draw(canvas);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CreateBitmap at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        //Create Name and Path File         
        //TimeType = Today,Month,Year
        //tabSelected = Hourly,Weekly,Monthly
        //Startdate = 20220201
        string CreateFileCSV()
        {
            try
            {
                switch (TypeReport)
                {
                    case "SalesReport":
                        FilterDate = "SaleReport_";
                        break;
                    case "SalesReportByBranch":
                        FilterDate = "SaleReport_Branch_";
                        break;
                    case "ProfitReport":
                        FilterDate = "ProfitReport_";
                        break;
                    case "CategoryReport":
                        FilterDate = "SaleReport_Category_";
                        break;
                    case "CustomerReport":
                        FilterDate = "SaleReport_Customer_";
                        break;
                    case "EmployeeReport":
                        FilterDate = "SaleReport_Employee_";
                        break;
                    case "PaymentReport":
                        FilterDate = "SaleReport_Payment_";
                        break;
                    case "ReportBestSale":
                        FilterDate = "BestItemReport_";
                        break;
                    default:
                        break;
                }

                #region old code
                //if (TypeReport == "SalesReport" || TypeReport == "ReportBestSale")
                //{
                //    switch (TimeType)
                //    {
                //        case "Date":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate);
                //            break;
                //        case "Month":
                //            switch (tabSelected)
                //            {
                //                case "Hourly":
                //                    FilterPeriodDate = "Hourly";
                //                    break;
                //                case "Daily":
                //                    FilterPeriodDate = "Daily";
                //                    break;
                //                case "Weekly":
                //                    FilterPeriodDate = "Weekly";
                //                    break;
                //                case "Monthly":
                //                    FilterPeriodDate = "Monthly";
                //                    break;
                //                default:
                //                    break;
                //            }
                //            FilterDate += FilterPeriodDate + "_" + Utils.ShowDateReport(ConvertStartDate).Substring(3, 7);
                //            break;
                //        case "Year":
                //            switch (tabSelected)
                //            {
                //                case "Hourly":
                //                    FilterPeriodDate = "Hourly";
                //                    break;
                //                case "Weekly":
                //                    FilterPeriodDate = "Weekly";
                //                    break;
                //                case "Monthly":
                //                    FilterPeriodDate = "Monthly";
                //                    break;
                //                default:
                //                    break;
                //            }
                //            FilterDate += FilterPeriodDate + "_" + Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
                //            break;
                //        case "CustomDate":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate) + "-" + Utils.ShowDateReport(ConvertEndDate);
                //            break;
                //        default:
                //            break;
                //    }
                //}
                //else
                //{
                //    switch (TimeType)
                //    {
                //        case "Date":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate);
                //            break;
                //        case "Month":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate).Substring(3, 7);
                //            break;
                //        case "Year":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
                //            break;
                //        case "customDate":
                //            FilterDate += Utils.ShowDateReport(ConvertStartDate) + "-" + Utils.ShowDateReport(ConvertEndDate);
                //            break;
                //        default:
                //            break;
                //    }
                //} 
                #endregion

                FileName = FilterDate + TextDate + ".csv";
                string DirectoryPath = DataCashingAll.PathImageBill;
                string fullName = Path.Combine(DirectoryPath, FileName);
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return fullName;
            }
            catch (Exception ex)
            {
                return string.Empty;
#pragma warning disable CS0162 // Unreachable code detected
                _ = TinyInsights.TrackErrorAsync(ex);
#pragma warning restore CS0162 // Unreachable code detected
                _ = TinyInsights.TrackPageViewAsync("CreateFileCSV at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        string CreateFilePDF()
        {
            try
            {
                switch (TypeReport)
                {
                    case "SalesReport":
                        FilterDate = "SaleReport_";
                        break;
                    case "SalesReportByBranch":
                        FilterDate = "SaleReport_Branch_";
                        break;
                    case "ProfitReport":
                        FilterDate = "ProfitReport_";
                        break;
                    case "CategoryReport":
                        FilterDate = "SaleReport_Category_";
                        break;
                    case "CustomerReport":
                        FilterDate = "SaleReport_Customer_";
                        break;
                    case "EmployeeReport":
                        FilterDate = "SaleReport_Employee_";
                        break;
                    case "PaymentReport":
                        FilterDate = "SaleReport_Payment_";
                        break;
                    case "ReportBestSale":
                        FilterDate = "BestItemReport_";
                        break;
                    default:
                        break;
                }
                
                FileName = FilterDate + TextDate + ".PDF";
                string DirectoryPath = DataCashingAll.PathImageBill;
                string fullName = Path.Combine(DirectoryPath, FileName);
                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }
                return fullName;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CreateFileCSV at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return string.Empty;
            }
        }

        void createDetail(string filename)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
            {
                switch (TypeReport)
                {
                    case "SalesReport":
                        SaleReport(file);
                        break;
                    case "SalesReportByBranch":
                        SaleReportBranch(file);
                        break;
                    case "ProfitReport":
                        ProfitReport(file);
                        break;
                    case "CategoryReport":
                        DetailCategoryReport(file);
                        break;
                    case "CustomerReport":
                        DetailCustomerReport(file);
                        break;
                    case "EmployeeReport":
                        DetailEmployeeReport(file);
                        break;
                    case "PaymentReport":
                        DetailPaymentReport(file);
                        break;
                    case "ReportBestSale":
                        DetailBestSaleReport(file);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void DetailCategoryReport(StreamWriter file)
        {
            try
            {
                CreateReport = await CreateDetailCategoryReport(file);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailCategoryReport");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private Task<bool> CreateDetailCategoryReport(StreamWriter file)
        {
            if (categoryResponses != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("CategoryName" + "," + "TotalAmount");

                foreach (var item in categoryResponses)
                {
                    file.WriteLine(item.categoryName + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async void DetailCustomerReport(StreamWriter file)
        {
            try
            {
                CreateReport = await CreateDetailReportCustomer(file);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailCustomerReport");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private Task<bool> CreateDetailReportCustomer(StreamWriter file)
        {
            if (customerResponses != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("CustomerName" + "," + "TotalAmount");

                foreach (var item in customerResponses)
                {
                    file.WriteLine(item.customerName + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async void DetailEmployeeReport(StreamWriter file)
        {
            try
            {
                CreateReport = await CreateDetailReportEmployeeSeller(file);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailEmployeeReport");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private Task<bool> CreateDetailReportEmployeeSeller(StreamWriter file)
        {
            if (employeeReports != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("SellerName" + "," + "MainRoles" + "," + "TotalAmount");

                foreach (var item in employeeReports)
                {
                    file.WriteLine(item.sellerName + "," + item.MainRoles + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async void DetailPaymentReport(StreamWriter file)
        {
            try
            {
                CreateReport = await CreateDetailReportByPayment(file);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailPaymentReport");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private Task<bool> CreateDetailReportByPayment(StreamWriter file)
        {
            if (paymentResponse != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("PaymentType" + "," + "TotalAmount");

                foreach (var item in paymentResponse)
                {
                    file.WriteLine(Utils.SetPaymentName(item.paymentType) + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private async void DetailBestSaleReport(System.IO.StreamWriter file)
        {
            try
            {
                CreateReport = await CreateDetailReportBestSaleItem(file);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailBestSaleReport");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        Task<bool> CreateDetailReportBestSaleItem(System.IO.StreamWriter file)
        {
            if (summaryItems != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("ItemName" + "," + "SumTotalAmount" + "," + "SumQuantity");

                foreach (var item in summaryItems)
                {
                    file.WriteLine(item.ItemName + "," + item.SumTotalAmount + "," + item.SumQuantity);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        async void SaleReport(System.IO.StreamWriter file)
        {
            try
            {
                switch (TimeType)
                {
                    case "Date":
                        CreateReport = await CreateDetailReport_Date_Time_Sale(file);
                        break;
                    case "Month":
                        switch (tabSelected)
                        {
                            case "Hourly":
                                CreateReport = await CreateDetailReport_Time_Sale(file);
                                break;
                            case "Daily":
                                CreateReport = await CreateDetailReport_Date_Sale(file);
                                break;
                            case "Weekly":
                                CreateReport = await CreateDetailReport_MonthYear_Day_Sale(file);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Year":
                        switch (tabSelected)
                        {
                            case "Hourly":
                                CreateReport = await CreateDetailReport_Year_Time_Sale(file);
                                break;
                            case "Weekly":
                                CreateReport = await CreateDetailReport_Year_Day_Sale(file);
                                break;
                            case "Monthly":
                                CreateReport = await CreateDetailReport_Year_Month_Sale(file);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CustomDate":
                        CreateReport = await CreateDetailReport_Custom_Sale(file);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        #region SaleReport
        //วันเดือนปี, เวลา, ยอดขาย => "SaleReport_DD-MM-YYYY , 
        Task<bool> CreateDetailReport_Date_Time_Sale(System.IO.StreamWriter file)
        {
            if (exportSale != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("Date" + "," + "Time" + "," + "Sales");

                foreach (var item in exportSale.reportHourlies)
                {
                    file.WriteLine(Utils.ShowDateReport(ConvertStartDate) + "," + item.Hourlyname + "," + item.value);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //เวลา, ยอดขาย =>  SaleReport_Hourly_MM-YYYY
        Task<bool> CreateDetailReport_Time_Sale(System.IO.StreamWriter file)
        {
            if (exportSale != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("Time" + "," + "Sales");

                foreach (var item in exportSale.reportHourlies)
                {
                    file.WriteLine(item.Hourlyname + "," + item.value);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //วันเดือนปี, ยอดขาย => SaleReport_Daily_MM-YYYY
        Task<bool> CreateDetailReport_Date_Sale(System.IO.StreamWriter file)
        {
            string Month = Utils.ShowDateReport(ConvertStartDate).Substring(3, 2);
            string Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            var daysInLeap = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month)); //28,29,30,31

            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Date" + "," + "Sales");

            int i = 0;
            //Check จำนวนวัน ตามเดือนที่จะเขียนลงไฟล์             
            foreach (var item in exportSale.reportDailies)
            {
                i++;
                if ((i <= daysInLeap))
                {
                    string date = item.Dailyname + "/" + Month + "/" + Year;
                    file.WriteLine(date + "," + item.value);
                }
            }
            return Task.FromResult(true);
        }

        //เดือน-ปี, วัน(จ.-อา.), ยอดขาย => SaleReport_Weekly_MM-YYYY
        Task<bool> CreateDetailReport_MonthYear_Day_Sale(System.IO.StreamWriter file)
        {
            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Day" + "," + "Sales");

            foreach (var item in exportSale.reportWeeklies)
            {
                file.WriteLine(item.Weeklyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        //ปี, เวลา, ยอดขาย => SaleReport_Hourly_YYYY
        Task<bool> CreateDetailReport_Year_Time_Sale(System.IO.StreamWriter file)
        {
            var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Year" + "," + "Time" + "," + "Sales");

            foreach (var item in exportSale.reportHourlies)
            {
                file.WriteLine(Year + "," + item.Hourlyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        //ปี, วัน(จ-อา), ยอดขาย => SaleReport_Weekly_YYYY
        Task<bool> CreateDetailReport_Year_Day_Sale(System.IO.StreamWriter file)
        {
            var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Year" + "," + "Day" + "," + "Sales");

            foreach (var item in exportSale.reportWeeklies)
            {
                file.WriteLine(Year + "," + item.Weeklyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        //ปี, เดือน(1-12), ยอดขาย => SaleReport_Monthly_YYYY
        Task<bool> CreateDetailReport_Year_Month_Sale(System.IO.StreamWriter file)
        {
            var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Year" + "," + "Month" + "," + "Sales");

            foreach (var item in exportSale.reportMonthlies)
            {
                file.WriteLine(Year + "," + item.Monthlyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        //วันเดือนปี, ยอดขาย => SaleReport_DD-MM-YYYY-DD-MM-YYYY
        Task<bool> CreateDetailReport_Custom_Sale(System.IO.StreamWriter file)
        {
            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Date" + "," + "Sales");

            var groupdate = GetDataSalesReport.GroupBy(x => x.DateModified.Date).Select(x => new ORM.Period.SummaryHourly
            {
                DateModified = x.First().DateModified,
                SumGrandTotal = x.Sum(x => x.SumGrandTotal),
            }).ToList();

            foreach (var item in groupdate)
            {
                file.WriteLine(Utils.ShowDateReport(item.DateModified) + "," + item.SumGrandTotal);
            }


            string Month = Utils.ShowDateReport(ConvertStartDate).Substring(3, 2);
            string Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            var daysInLeap = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month)); //28,29,30,31

            file.WriteLine('"' + StrBranchName + '"');
            file.WriteLine("Date" + "," + "Sales");

            int i = 0;
            //Check จำนวนวัน ตามเดือนที่จะเขียนลงไฟล์             
            foreach (var item in exportSale.reportDailies)
            {
                i++;
                if ((i <= daysInLeap))
                {
                    string date = item.Dailyname + "/" + Month + "/" + Year;
                    file.WriteLine(date + "," + item.value);
                }
            }

            return Task.FromResult(true);
        }
        #endregion
        async void SaleReportBranch(System.IO.StreamWriter file)
        {
            try
            {
                switch (TimeType)
                {
                    case "Date":
                        CreateReport = await CreateDetailReportBranch_Today(file);
                        break;
                    case "Month":
                        CreateReport = await CreateDetailReportBranch_Month(file);
                        break;
                    case "Year":
                        CreateReport = await CreateDetailReportBranch_Year(file);
                        break;
                    case "CustomDate":
                        CreateReport = await CreateDetailReportBranch_Custom(file);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        #region SaleReportBranch
        //วันเดือนปี, ชื่อสาขา, ยอดขาย => SaleReport_Branch_DD-MM-YYYY, 
        Task<bool> CreateDetailReportBranch_Today(System.IO.StreamWriter file)
        {
            if (exportSalesByBranch != null)
            {
                file.WriteLine("Branch" + "," + "Sales");

                foreach (var item in exportSalesByBranch)
                {
                    file.WriteLine(item.BranchName + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //เดือน-ปี, ชื่อสาขา, ยอดขาย => SaleReport_Branch_MM-YYYY, 
        Task<bool> CreateDetailReportBranch_Month(System.IO.StreamWriter file)
        {
            if (exportSalesByBranch != null)
            {
                file.WriteLine("Branch" + "," + "Sales");

                foreach (var item in exportSalesByBranch)
                {
                    file.WriteLine(item.BranchName + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //ปี,ชื่อสาขา, ยอดขาย => SaleReport_Branch_YYYY, 
        Task<bool> CreateDetailReportBranch_Year(System.IO.StreamWriter file)
        {
            if (exportSalesByBranch != null)
            {
                file.WriteLine("Branch" + "," + "Sales");

                foreach (var item in exportSalesByBranch)
                {
                    file.WriteLine(item.BranchName + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //วันเดือนปี, ชื่อสาขา, ยอดขาย => SaleReport_Branch_, 
        Task<bool> CreateDetailReportBranch_Custom(System.IO.StreamWriter file)
        {
            if (exportSalesByBranch != null)
            {
                file.WriteLine("Branch" + "," + "Sales");
                foreach (var item in exportSalesByBranch)
                {
                    file.WriteLine(item.BranchName + "," + item.sumGrandTotal);
                }
            }
            return Task.FromResult(true);
        }
        #endregion
        async void ProfitReport(System.IO.StreamWriter file)
        {
            try
            {
                if (TimeType == "Custom")
                {
                    ReportProfits = new List<ReportProfit>();
                    ReportProfits = reportProfitsCustom;
                    CreateReport = await CreateDetailProfitReportCustom(file);
                }
                else
                {
                    switch (tabSelected)
                    {
                        case "Hourly":
                            ReportProfits = new List<ReportProfit>();
                            foreach (var item in exportSale.reportHourlies)
                            {
                                ReportProfit reportProfit = new ReportProfit()
                                {
                                    dateTime = item.Hourlyname,
                                    sumGrandTotal = item.value,
                                    sumProfitTotal = GetDataSalesReport.Where(x => x.Hourly == item.IdHourly).Sum(x => x.SumProfitAmount),
                                };
                                ReportProfits.Add(reportProfit);
                            }
                            break;
                        case "Daily":
                            ReportProfits = new List<ReportProfit>();
                            foreach (var item in exportSale.reportDailies)
                            {
                                ReportProfit reportProfit = new ReportProfit()
                                {
                                    dateTime = item.Dailyname,
                                    sumGrandTotal = item.value,
                                    sumProfitTotal = GetDataSalesReport.Where(x => x.Daily == item.IdDaily).Sum(x => x.SumProfitAmount),
                                };
                                ReportProfits.Add(reportProfit);
                            }
                            break;
                        case "Weekly":
                            ReportProfits = new List<ReportProfit>();
                            foreach (var item in exportSale.reportWeeklies)
                            {
                                ReportProfit reportProfit = new ReportProfit()
                                {
                                    dateTime = item.Weeklyname,
                                    sumGrandTotal = item.value,
                                    sumProfitTotal = GetDataSalesReport.Where(x => x.DayOfWeek == item.IdWeekly).Sum(x => x.SumProfitAmount),
                                };
                                ReportProfits.Add(reportProfit);
                            }
                            break;
                        case "Monthly":
                            ReportProfits = new List<ReportProfit>();
                            foreach (var item in exportSale.reportMonthlies)
                            {
                                ReportProfit reportProfit = new ReportProfit()
                                {
                                    dateTime = item.Monthlyname,
                                    sumGrandTotal = item.value,
                                    sumProfitTotal = GetDataSalesReport.Where(x => x.Monthly == item.IdMonthly).Sum(x => x.SumProfitAmount),
                                };
                                ReportProfits.Add(reportProfit);
                            }
                            break;
                        default:
                            ReportProfits = new List<ReportProfit>();
                            break;
                    }
                    CreateReport = await CreateDetailProfitReport(file);

                }




            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private Task<bool> CreateDetailProfitReportCustom(StreamWriter file)
        {
            if (exportSale != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                file.WriteLine("Date" + "," + "Total Profit" + "," + "Total Sales");

                foreach (var item in ReportProfits)
                {
                    file.WriteLine(item.dateTime + "," + item.sumProfitTotal + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        #region ProfitReport
        //วันเดือนปี, ยอดขาย,ทุนประเมิน,กำไร => ProfitReport_DD-MM-YYYY
        Task<bool> CreateDetailProfitReport(System.IO.StreamWriter file)
        {
            if (exportSale != null)
            {
                file.WriteLine('"' + StrBranchName + '"');
                switch (tabSelected)
                {
                    case "Hourly":
                        file.WriteLine("Hourly" + "," + "Total Profit" + "," + "Total Sales");
                        break;
                    case "Daily":
                        file.WriteLine("Daily" + "," + "Total Profit" + "," + "Total Sales");
                        break;
                    case "Weekly":
                        file.WriteLine("Weekly" + "," + "Total Profit" + "," + "Total Sales");
                        break;
                    case "Monthly":
                        file.WriteLine("Monthly" + "," + "Total Profit" + "," + "Total Sales");
                        break;
                    default:
                        file.WriteLine("Time" + "," + "Total Profit" + "," + "Total Sales");
                        break;
                }

                foreach (var item in ReportProfits)
                {
                    file.WriteLine(item.dateTime + "," + item.sumProfitTotal + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        #endregion
        bool deviceAsleep = false;
        bool openPage = false;
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

