using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Microcharts.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TinyInsightsLib;
using static Android.Icu.Text.TimeZoneFormat;
using Gabana.Droid.Tablet.Adapter.Report;
using AndroidX.CardView.Widget;
using Microcharts;
using Org.BouncyCastle.Bcpg.Sig;
using SkiaSharp;
using System.Globalization;
using System.Threading.Tasks;
using Gabana3.JAM.Report;
using Xamarin.Essentials;
using Gabana.Droid.Tablet.Dialog;
using System.IO;

namespace Gabana.Droid.Tablet.Fragments.Report
{
    public class Report_Fragment_ShowData : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Report_Fragment_ShowData NewInstance()
        {
            Report_Fragment_ShowData frag = new Report_Fragment_ShowData();
            return frag;
        }
        public static Report_Fragment_ShowData fragment_showdata;
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_fragment_showdata, container, false);
            try
            {
                fragment_showdata = this;
                CombinUI();
                SetUIEvent();
                //SetUiFromTypeReport();
                if (TypeReport.Contains("SalesReport") || TypeReport.Contains("ProfitReport"))
                {
                    filterReport = 0;
                }
                else
                {
                    filterReport = 1;
                }
                tabSelected = "Hourly";
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

        public static int filterReport;
        public static string tabSelected = "";
        string StrBranchName;
        DateTime ConvertStartDate, ConvertEndDate;
        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                SetDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetDetail()
        {
            try
            {
                if (string.IsNullOrEmpty(TypeReport))
                {
                    return;
                }
                GetBranchSelect();
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
                //ViewReport();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

                GridLayoutManager menuLayoutManager = new GridLayoutManager(this.Activity, 3, 1, false);
                rcvHeaderReport.HasFixedSize = true;
                rcvHeaderReport.SetLayoutManager(menuLayoutManager);
                Report_Adapter_MenuTime report_adapter_header = new Report_Adapter_MenuTime(MenuTab, tabSelected);
                rcvHeaderReport.SetAdapter(report_adapter_header);
                report_adapter_header.ItemClick += Report_adapter_header_ItemClick;
                ViewReport();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at ReportDailySale");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void ViewReport()
        {
            DialogLoading dialog = new DialogLoading();
            try
            {
                if (dialog.Cancelable != false)
                {
                    dialog.Cancelable = false;
                    dialog?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
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
        List<Gabana3.JAM.Report.SummaryItemModel> summaryItems = new List<Gabana3.JAM.Report.SummaryItemModel>();
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
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategoryLeft.SetAdapter(report_adapter_item);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutItem);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBestSale at showdailysale");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<Gabana3.JAM.Report.SalesByPaymentResponse> paymentResponse = new List<Gabana3.JAM.Report.SalesByPaymentResponse>();
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
                        paymentResponse = result;
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
                chartViewLeft.Chart = chart2;

                if (!string.IsNullOrEmpty(search))
                {
                    paymentResponse = paymentResponse.Where(x => Utils.SetPaymentName(x.paymentType).ToLower().Contains(search.ToLower())).ToList();
                }
                Report_Adapter_ShowPayment report_adapter_payment = new Report_Adapter_ShowPayment(paymentResponse, SalesByPayments);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategoryLeft.SetAdapter(report_adapter_payment);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutItem);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowPaymentReport at showdailysale");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<EmployeeReport> employeeReports = new List<EmployeeReport>();
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
                        sellerResponses = result;
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
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategoryLeft.SetAdapter(report_adapter_showCustomer);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutItem);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowEmployeeReport at showdailysale");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<Gabana3.JAM.Report.SalesByCustomerResponse> customerResponses = new List<Gabana3.JAM.Report.SalesByCustomerResponse>();
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
                        customerResponses = result;
                        break;
                }
                if (!string.IsNullOrEmpty(search))
                {
                    customerResponses = customerResponses.Where(x => x.customerName.ToLower().Contains(search.ToLower())).ToList();
                }
                //List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                Report_Adapter_ShowCustomer report_adapter_showCustomer = new Report_Adapter_ShowCustomer(customerResponses, listCustomer);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategoryLeft.SetAdapter(report_adapter_showCustomer);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutItem);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCustomerReport at showdailysale");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = new List<Gabana3.JAM.Report.SalesByCategoryResponse>();
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
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListCategoryLeft.SetAdapter(report_adapter_showCategory);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutItem);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync(" ShowCategoryReport at ShowReportDailySaleActivity");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        ReportSale exportSale = new ReportSale();
        List<ORM.Period.SummaryHourly> GetDataSalesReport = new List<ORM.Period.SummaryHourly>();
        decimal totalSale;
        string search = "";

        private List<ReportProfit> reportProfitsCustom = new List<ReportProfit>();
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
                        reportProfit.DateTime = date;
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
                            reportProfitsCustom = reportProfitsCustom.OrderBy(x => Convert.ToDateTime(x.DateTime)).ToList();
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

                    Report_Adapter_CustomTime adapter_CustomTime = new Report_Adapter_CustomTime(reportProfitsCustom, TypeReport);
                    rcvListCategoryLeft.SetAdapter(adapter_CustomTime);


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

                if (TimeType == "CustomDate")
                {
                    lnChartSale.Visibility = ViewStates.Gone;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    lnRight.Visibility = ViewStates.Gone;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                    gridLayoutManager.CanScrollVertically();
                    rcvListCategoryLeft.SetLayoutManager(gridLayoutManager);
                    rcvListCategoryLeft.HasFixedSize = true;
                    rcvListCategoryLeft.SetItemViewCacheSize(50);
                }
                else
                {
                    lnChartSale.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Gone;
                    lnRight.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Gone;
                    GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                    gridLayoutManager.CanScrollVertically();
                    rcvListTime.SetLayoutManager(gridLayoutManager);
                    rcvListTime.HasFixedSize = true;
                    rcvListTime.SetItemViewCacheSize(50);
                }
                
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
        List<SaleReportBranch> exportSalesByBranch = new List<SaleReportBranch>();
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
                Report_Adapter_Branch Adapter_ShowByBranch = new Report_Adapter_Branch(listsaleReportBranch);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                gridLayoutManager.CanScrollVertically();
                rcvListCategoryLeft.SetAdapter(Adapter_ShowByBranch);
                rcvListCategoryLeft.SetLayoutManager(gridLayoutManager);
                rcvListCategoryLeft.HasFixedSize = true;
                rcvListCategoryLeft.SetItemViewCacheSize(50);
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
        private void Report_adapter_header_ItemClick(object sender, int e)
        {
            tabSelected = MenuTab[e].NameMenuEn;
            filterReport = 0;
            SetTabShowMenu();
            SetUiFromTypeReport();
        }

        private void SetUiFromTypeReport()
        {
            lnSaleReport.Visibility = ViewStates.Gone;
            txtGroupBy.Visibility = ViewStates.Gone;
            lnCategoryReport.Visibility = ViewStates.Gone;
            lnSaleReport.Visibility = ViewStates.Gone;

            lnRight.Visibility = ViewStates.Gone;
            lnReportLeft.Visibility = ViewStates.Gone;
            lnHeaderLeft.Visibility = ViewStates.Gone;

            rcvListCategoryLeft.Visibility = ViewStates.Gone;
            chartViewLeft.Visibility = ViewStates.Gone;
            lnChartLeft.Visibility = ViewStates.Gone;

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
                    lnRight.Visibility = ViewStates.Visible;
                    lnHeaderLeft.Visibility = ViewStates.Visible;
                    break;
                case "SalesReportByBranch":
                    txtTitle.Text = GetString(Resource.String.report_salebybranch_report);
                    lnSaleReport.Visibility = ViewStates.Visible;
                    lnHeader.Visibility = ViewStates.Gone;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    lnHeaderLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    break;
                case "ProfitReport":
                    txtTitle.Text = GetString(Resource.String.report_profitbycostestimate_report);
                    txtTitleChart.Text = GetString(Resource.String.dashboard_activity_profit);
                    txtAmount.Text = GetString(Resource.String.dashboard_activity_profit);
                    lnSaleReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnRight.Visibility = ViewStates.Visible;
                    lnHeaderLeft.Visibility = ViewStates.Visible;
                    break;
                case "CategoryReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycategory_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    break;
                case "CustomerReport":
                    txtTitle.Text = GetString(Resource.String.report_salebycustomer_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    break;
                case "EmployeeReport":
                    txtTitle.Text = GetString(Resource.String.report_salebyemployee_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    break;
                case "PaymentReport":
                    txtTitle.Text = GetString(Resource.String.report_salebypaymentmethod_report);
                    lnHeader.Visibility = ViewStates.Gone;
                    txtGroupBy.Visibility = ViewStates.Visible;
                    lnChartCategory.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    lnTotal.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
                    chartViewLeft.Visibility = ViewStates.Visible;
                    lnChartLeft.Visibility = ViewStates.Visible;
                    break;
                case "ReportBestSale":
                    lnHeader.Visibility = ViewStates.Gone;
                    lnSearch2.Visibility = ViewStates.Visible;
                    lnCategoryReport.Visibility = ViewStates.Visible;
                    lnReportLeft.Visibility = ViewStates.Visible;
                    rcvListCategoryLeft.Visibility = ViewStates.Visible;
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
                    txtTypeTimeRight.Text = GetString(Resource.String.report_hour);
                    break;
                case "Daily":
                    txtTitleChart.Text += GetString(Resource.String.report_bydaily);
                    txtTypeTimeRight.Text = GetString(Resource.String.report_daily);
                    break;
                case "Weekly":
                    txtTitleChart.Text += GetString(Resource.String.report_byweekly);
                    txtTypeTimeRight.Text = GetString(Resource.String.report_weekly);
                    break;
                case "Monthly":
                    txtTitleChart.Text += GetString(Resource.String.report_bymonth);
                    txtTypeTimeRight.Text = GetString(Resource.String.report_monthly);
                    break;
                default:
                    break;
            }
        }
        public List<MenuTab> MenuTab { get; set; }
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
        private void SetNoData(bool noData)
        {
            if (noData)
            {
                lnLeft.Visibility = ViewStates.Visible;
                lnRight.Visibility = ViewStates.Gone;
                lnHaveData.Visibility = ViewStates.Gone;
                lnNoData.Visibility = ViewStates.Visible;
            }
            else
            {
                lnLeft.Visibility = ViewStates.Visible;
                lnRight.Visibility = ViewStates.Visible;
                lnHaveData.Visibility = ViewStates.Visible;
                lnNoData.Visibility = ViewStates.Gone;
            }
        }
        private List<ORM.MerchantDB.Branch> lstBranch = new List<ORM.MerchantDB.Branch>();
        string sysbranIdSelect = "";
        List<int> lstsysBranchID = new List<int>();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;

            imgShare1.Click += ImgShare_Click;
            imgShare2.Click += ImgShare_Click;
            imgShare3.Click += ImgShare_Click;

            imgFilter1.Click += ImgFilter_Click;
            imgFilter2.Click += ImgFilter_Click;
            imgFilter3.Click += ImgFilter_Click;
        }

        private void ImgFilter_Click(object sender, EventArgs e)
        {
            var fragment = new Report_Dialog_Filter();
            Report_Dialog_Filter dialog = new Report_Dialog_Filter() { Cancelable = false };
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Filter));
        }

        private void ImgShare_Click(object sender, EventArgs e)
        {
            var fragment = new Report_Dialog_Share();
            Report_Dialog_Share dialog = new Report_Dialog_Share() { Cancelable = false };
            fragment.Show(Activity.SupportFragmentManager, nameof(Report_Dialog_Share));
        }
        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnReport, "report", "default");
        }

        TextView txtTitle;
        LinearLayout lnBack, lnLeft;
        TextView txtCustomDate, txtDate, txtBranchName, txtGroupBy;
        LinearLayout lnSaleReport, lnHeader;
        RecyclerView rcvHeaderReport;
        TextView txtTitleChart;
        CardView lnChartSale;
        ChartView chartView;
        CardView lnTotal;
        TextView textTotalName, textGrandTotal;
        LinearLayout lnRight;
        ImageButton btnSearch1;
        EditText textSearch1;
        ImageButton imgFilter1;
        TextView txtTypeTime, txtAmount, txtTypeTimeRight, txtAmountRight;
        RecyclerView rcvListTime;
        LinearLayout lnCategoryReport;
        FrameLayout lnSearch2;
        ImageButton btnSearch2;
        EditText textSearch2;
        ImageButton imgFilter2;
        LinearLayout lnChartCategory;
        ChartView chartView2;
        RecyclerView rcvListCategory;
        LinearLayout lnHaveData, lnNoData;

        LinearLayout lnReportLeft;
        RecyclerView rcvListCategoryLeft;
        FrameLayout lnSearchLeft;
        EditText textSearch3;
        ImageButton btnSearch3;
        ImageButton imgFilter3;
        ImageButton imgShare1, imgShare2, imgShare3;
        LinearLayout lnHeaderLeft;
        ChartView chartViewLeft;
        FrameLayout lnChartLeft;

        ScrollView lnFullReport;

        private void CombinUI()
        {
            txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnLeft = view.FindViewById<LinearLayout>(Resource.Id.lnLeft);
            txtCustomDate = view.FindViewById<TextView>(Resource.Id.txtCustomDate);
            txtDate = view.FindViewById<TextView>(Resource.Id.txtDate);
            txtBranchName = view.FindViewById<TextView>(Resource.Id.txtBranchName);
            txtGroupBy = view.FindViewById<TextView>(Resource.Id.txtGroupBy);
            lnSaleReport = view.FindViewById<LinearLayout>(Resource.Id.lnSaleReport);
            lnHeader = view.FindViewById<LinearLayout>(Resource.Id.lnHeader);
            rcvHeaderReport= view.FindViewById<RecyclerView>(Resource.Id.rcvHeaderReport);
            txtTitleChart = view.FindViewById<TextView>(Resource.Id.txtTitleChart);
            lnChartSale = view.FindViewById<CardView>(Resource.Id.lnChartSale);
            chartView = view.FindViewById<ChartView>(Resource.Id.chartView);
            lnTotal = view.FindViewById<CardView>(Resource.Id.lnTotal);
            textTotalName = view.FindViewById<TextView>(Resource.Id.textTotalName);
            textGrandTotal = view.FindViewById<TextView>(Resource.Id.textGrandTotal);
            lnRight = view.FindViewById<LinearLayout>(Resource.Id.lnRight);
            btnSearch1 = view.FindViewById<ImageButton>(Resource.Id.btnSearch1);
            textSearch1 = view.FindViewById<EditText>(Resource.Id.textSearch1);
            imgFilter1 = view.FindViewById<ImageButton>(Resource.Id.imgFilter1);
            txtTypeTime = view.FindViewById<TextView>(Resource.Id.txtTypeTime);
            txtTypeTimeRight = view.FindViewById<TextView>(Resource.Id.txtTypeTimeRight);
            txtAmount = view.FindViewById<TextView>(Resource.Id.txtAmount);
            txtAmountRight = view.FindViewById<TextView>(Resource.Id.txtAmountRight);
            rcvListTime = view.FindViewById<RecyclerView>(Resource.Id.rcvListTime);
            lnCategoryReport = view.FindViewById<LinearLayout>(Resource.Id.lnCategoryReport);
            lnSearch2 = view.FindViewById<FrameLayout>(Resource.Id.lnSearch2);
            btnSearch2 = view.FindViewById<ImageButton>(Resource.Id.btnSearch2);
            textSearch2 = view.FindViewById<EditText>(Resource.Id.textSearch2);
            imgFilter2 = view.FindViewById<ImageButton>(Resource.Id.imgFilter2);
            lnChartCategory = view.FindViewById<LinearLayout>(Resource.Id.lnChartCategory);
            chartView2 = view.FindViewById<ChartView>(Resource.Id.chartView2);
            rcvListCategory = view.FindViewById<RecyclerView>(Resource.Id.rcvListCategory);
            lnHaveData = view.FindViewById<LinearLayout>(Resource.Id.lnHaveData);
            lnNoData = view.FindViewById<LinearLayout>(Resource.Id.lnNoData);
            lnReportLeft = view.FindViewById<LinearLayout>(Resource.Id.lnReportLeft);
            lnHeaderLeft = view.FindViewById<LinearLayout>(Resource.Id.lnHeaderLeft);
            rcvListCategoryLeft = view.FindViewById<RecyclerView>(Resource.Id.rcvListCategoryLeft);
            lnSearchLeft = view.FindViewById<FrameLayout>(Resource.Id.lnSearchLeft);
            btnSearch3 = view.FindViewById<ImageButton>(Resource.Id.btnSearch3);
            textSearch3 = view.FindViewById<EditText>(Resource.Id.textSearch3);

            imgFilter3 = view.FindViewById<ImageButton>(Resource.Id.imgFilter3);

            imgShare1 = view.FindViewById<ImageButton>(Resource.Id.imgShare1);
            imgShare2 = view.FindViewById<ImageButton>(Resource.Id.imgShare2);
            imgShare3 = view.FindViewById<ImageButton>(Resource.Id.imgShare3);
            chartViewLeft = view.FindViewById<ChartView>(Resource.Id.chartViewLeft);
            lnChartLeft = view.FindViewById<FrameLayout>(Resource.Id.lnChartLeft);

            lnFullReport = view.FindViewById<ScrollView>(Resource.Id.lnFullReport);

        }

        public static string TypeReport = "";
        private static string StartDate, EndDate, TimeType;
        static List<Gabana.ORM.MerchantDB.Branch> listChooseBranch = new List<ORM.MerchantDB.Branch>();
        private static string TextDate, TextGroup;
        private static List<Category> ListCategory;
        private static List<Customer> ListCustomer;
        private static List<ORM.MerchantDB.UserAccountInfo> ListEmployee;
        private static List<PaymentType> lsPayment;
        private static string BestSellBy;
        List<ReportProfit> ReportProfits = new List<ReportProfit>();

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


        public async Task LnPDF_Click()
        {
            try
            {
                string Fullname = CreateFilePDF();
                CreateBitmap();
                await CreatePDF(Fullname);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        Android.Graphics.Bitmap bitmap;
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.nonapplication), ToastLength.Short).Show();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OpenFile at Bill Detail");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

        string FileName, FilterDate = string.Empty, FilterPeriodDate = string.Empty;
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
                _ = TinyInsights.TrackPageViewAsync("CreateFilePDF at Report");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return string.Empty;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task LnEmail_Click()
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
                    Toast.MakeText(this.Activity, "ไม่สำเร็จ", ToastLength.Short);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
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
        bool CreateReport = false;
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CreateFileCSV at Bill Detaial");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return string.Empty;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity    , ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

        public async Task LnExport_Click()
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
                    Toast.MakeText(this.Activity, "ไม่สำเร็จ", ToastLength.Short);
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return Task.FromResult(false);
            }
        }

    }
}