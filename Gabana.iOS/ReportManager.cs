using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana
{
    internal class ReportManager
    {
        static DateTime ConvertStartDate, ConvertEndDate;
        private static string StartDate, EndDate;
        static string Branchname; 
        public static async void createDetail(string TypeReport,
            string filename, object value, string TimeType,
            string tabSelected, string StartDate, string EndDate,
            List<ORM.Period.SummaryHourly> getDataReportMonth,string sysbranchid,string Branchname )
        {
            try
            {
                Branchname = Branchname;
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "CSV");
                var filepart = Path.Combine(libFolder, filename+".csv");
                if (!Directory.Exists(libFolder))
                {
                    Directory.CreateDirectory(libFolder);
                }
                string[] files = Directory.GetFiles(libFolder);
                foreach (string file in files)
                {
                    File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }
                //Directory.CreateDirectory(filepart);


                System.Text.StringBuilder s = new System.Text.StringBuilder();
                //ConvertStartDate = Utils.ChangeDateTimeStringReport(StartDate);
                //ConvertEndDate = Utils.ChangeDateTimeStringReport(EndDate);
                var branchshow = "";
                if (ReportController.listChooseBranch.Count != ReportController.listAllBranch.Count)
                {
                    foreach (var item in ReportController.listChooseBranch)
                    {
                        if (branchshow == "")
                        {
                            branchshow += item.BranchName;
                        }
                        else
                        {
                            branchshow += "," + item.BranchName;
                        }

                    }
                }
                
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepart, true, Encoding.UTF8))

                {
                    if (!string.IsNullOrEmpty(branchshow))
                    {
                        file.WriteLine('"' + branchshow + '"');
                    }
                    
                    switch (TypeReport)
                    {
                        case "SalesReport":
                            await SaleReport(file, value, TimeType, tabSelected, StartDate, getDataReportMonth);
                            break;
                        case "SalesReportByBranch":
                            await SaleReportBranch(file, value, TimeType);
                            break;
                        case "ProfitReport":
                            await ProfitReport(file, value, TimeType, StartDate,EndDate, sysbranchid, tabSelected);
                            break;
                        case "CategoryReport":
                            await DetailCategoryReport(file, value);
                            break;
                        case "CustomerReport":
                            await CreateDetailReportCustomer(file, value);
                            break;
                        case "EmployeeReport":
                            await DetailEmployeeReport(file, value);
                            break;
                        case "PaymentReport":
                            await CreateDetailReportByPayment(file, value);
                            break;
                        case "ReportBestSale":
                            await CreateDetailReportBestSaleItem(file, value);
                            break;
                        default:
                            break;
                    }
                    var items = new NSObject();

                    //var url = new NSUrl(filename);
                    //NSObject[] activityItems = { url };
                    UIActivity[] applicationActivities = null;

                    var url = NSUrl.FromFilename(filepart);
                    var item = url.Copy();
                    var activityItems = new[] { item };

                    //items.Add(url);
                    //    var doc = UIDocumentInteractionController.FromUrl(url);

                    var controller = new UIActivityViewController(activityItems, applicationActivities)
                    {
                        ExcludedActivityTypes = new NSString[]
                        {
                            UIActivityType.Mail,
                            UIActivityType.AddToReadingList
                        }
                    };
                    if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                    {
                        // Phone
                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(controller, true, null);
                    }

                }
             }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        public static Task<bool> DetailCategoryReport(StreamWriter file, object value)
        {
            try
            {
                var categoryResponses = value as List<Gabana3.JAM.Report.SalesByCategoryResponse>;
                if (categoryResponses != null)
                {
                    file.WriteLine("CategoryName" + "," + "TotalAmount");

                    foreach (var item in categoryResponses)
                    {
                        file.WriteLine(item.categoryName + "," + item.sumTotalAmount);
                    }
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailCategoryReport");

                return Task.FromResult(false);
            }
        }

        private Task<bool> CreateDetailCategoryReport(StreamWriter file, List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses)
        {
            if (categoryResponses != null)
            {
                file.WriteLine("CategoryName" + "," + "TotalAmount");

                foreach (var item in categoryResponses)
                {
                    file.WriteLine(item.categoryName + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //private async void DetailCustomerReport(StreamWriter file)
        //{
        //    try
        //    {
        //        CreateReport = await CreateDetailReportCustomer(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("DetailCustomerReport");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}

        public static Task<bool> CreateDetailReportCustomer(StreamWriter file, object value)
        {
            var customerResponses = value as List<Gabana3.JAM.Report.SalesByCustomerResponse>;
            if (customerResponses != null)
            {
                file.WriteLine("CustomerName" + "," + "TotalAmount");

                foreach (var item in customerResponses)
                {
                    file.WriteLine(item.customerName + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //private async void DetailEmployeeReport(StreamWriter file)
        //{
        //    try
        //    {
        //        CreateReport = await CreateDetailReportEmployeeSeller(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("DetailEmployeeReport");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}

        public static Task<bool> DetailEmployeeReport(StreamWriter file, object value)
        {
            var employeeReports = value as List<Gabana3.JAM.Report.SalesBySellerResponse >;
            if (employeeReports != null)
            {
                file.WriteLine("SellerName" + "," + "TotalAmount");

                foreach (var item in employeeReports)
                {
                    file.WriteLine(item.sellerName + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //private async void DetailPaymentReport(StreamWriter file)
        //{
        //    try
        //    {
        //        CreateReport = await CreateDetailReportByPayment(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("DetailPaymentReport");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}

        public static Task<bool> CreateDetailReportByPayment(StreamWriter file, object value)
        {
            var paymentResponse = value as List<Gabana3.JAM.Report.SalesByPaymentResponse >;
            if (paymentResponse != null)
            {
                file.WriteLine("PaymentType" + "," + "TotalAmount");

                foreach (var item in paymentResponse)
                {
                    file.WriteLine(Utils.SetPaymentNameChart(item.paymentType) + "," + item.sumTotalAmount);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //private async void DetailBestSaleReport(System.IO.StreamWriter file)
        //{
        //    try
        //    {
        //        CreateReport = await CreateDetailReportBestSaleItem(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("DetailBestSaleReport");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}
        public static Task<bool> CreateDetailReportBestSaleItem(System.IO.StreamWriter file, object value)
        {
            var summaryItems = value as List<Gabana3.JAM.Report.SummaryItemModel>;
            if (summaryItems != null)
            {
                file.WriteLine("ItemName" + "," + "SumTotalAmount" + "," + "SumQuantity");

                foreach (var item in summaryItems)
                {
                    file.WriteLine(item.ItemName + "," + item.SumTotalAmount + "," + item.SumQuantity);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public static async Task SaleReport(System.IO.StreamWriter file, object value, string timeType, string tabSelected ,string date, List<ORM.Period.SummaryHourly> getDataReportMonth)
        {
            try
            {
                switch (timeType)
                {
                    case "Date":
                        var CreateReport = await CreateDetailReport_Date_Time_Sale(file, value, date);

                        break;
                    case "Month":
                        switch (tabSelected)
                        {
                            case "Hourly":
                                CreateReport = await CreateDetailReport_Time_Sale(file, value);
                                break;
                            case "Daily":
                                CreateReport = await CreateDetailReport_Date_Sale(file, value, date);
                                break;
                            case "Weekly":
                                CreateReport = await CreateDetailReport_MonthYear_Day_Sale(file, value);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Year":
                        switch (tabSelected)
                        {
                            case "Hourly":
                                CreateReport = await CreateDetailReport_Year_Time_Sale(file, value, date);
                                break;
                            case "Weekly":
                                CreateReport = await CreateDetailReport_Year_Day_Sale(file, value, date);
                                break;
                            case "Monthly":
                                CreateReport = await CreateDetailReport_Year_Month_Sale(file, value, date);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CustomDate":
                        CreateReport = await CreateDetailReport_Custom_Sale(file, value, date, getDataReportMonth);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        #region SaleReport
        //วันเดือนปี, เวลา, ยอดขาย => "SaleReport_DD-MM-YYYY , 
        public static Task<bool> CreateDetailReport_Date_Time_Sale(System.IO.StreamWriter file, object value,string date)
        {
            var exportSale = value as ReportSale;
            if (exportSale != null)
            {
                file.WriteLine("Date" + "," + "Time" + "," + "Sales");

                foreach (var item in exportSale.reportHourlies)
                {
                    file.WriteLine(date + "," + item.Hourlyname + "," + item.value);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        //เวลา, ยอดขาย =>  SaleReport_Hourly_MM-YYYY
        public static Task<bool> CreateDetailReport_Time_Sale(System.IO.StreamWriter file, object value)
        {
            var exportSale = value as ReportSale;
            if (exportSale != null)
            {
                file.WriteLine("Time" + "," + "Sales");

                foreach (var item in exportSale.reportHourlies)
                {
                    file.WriteLine(item.Hourlyname + "," + item.value);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        ////วันเดือนปี, ยอดขาย => SaleReport_Daily_MM-YYYY
        public static Task<bool> CreateDetailReport_Date_Sale(System.IO.StreamWriter file, object value,string dateshow)
        {
            var exportSale = value as ReportSale;
            string Month = Utils.ShowDateReport(ConvertStartDate).Substring(3, 2);
            string Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            var daysInLeap = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month)); //28,29,30,31

            file.WriteLine("Date" + "," + "Sales");
            var x = dateshow.Split(' ');
            int i = 0;
            //Check จำนวนวัน ตามเดือนที่จะเขียนลงไฟล์             
            foreach (var item in exportSale.reportDailies)
            {
                i++;
                if ((i <= daysInLeap))
                {
                    string date = item.Dailyname + "/" + x[0] + "/" + x[1];
                    file.WriteLine(date + "," + item.value);
                }
            }
            return Task.FromResult(true);
        }

        ////เดือน-ปี, วัน(จ.-อา.), ยอดขาย => SaleReport_Weekly_MM-YYYY
        public static Task<bool> CreateDetailReport_MonthYear_Day_Sale(System.IO.StreamWriter file, object value)
        {
            var exportSale = value as ReportSale;
            file.WriteLine("Day" + "," + "Sales");

            foreach (var item in exportSale.reportWeeklies)
            {
                file.WriteLine(item.Weeklyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        ////ปี, เวลา, ยอดขาย => SaleReport_Hourly_YYYY
        public static Task<bool> CreateDetailReport_Year_Time_Sale(System.IO.StreamWriter file, object value, string date)
        {
            var exportSale = value as ReportSale;
            //var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine("Year" + "," + "Time" + "," + "Sales");

            foreach (var item in exportSale.reportHourlies)
            {
                file.WriteLine(date + "," + item.Hourlyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        ////ปี, วัน(จ-อา), ยอดขาย => SaleReport_Weekly_YYYY
        public static Task<bool> CreateDetailReport_Year_Day_Sale(System.IO.StreamWriter file, object value,string date)
        {
            var exportSale = value as ReportSale;
            //var x = date.Split(' ');
            //var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine("Year" + "," + "Day" + "," + "Sales");

            foreach (var item in exportSale.reportWeeklies)
            {
                file.WriteLine(date + "," + item.Weeklyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        ////ปี, เดือน(1-12), ยอดขาย => SaleReport_Monthly_YYYY
        public static Task<bool> CreateDetailReport_Year_Month_Sale(System.IO.StreamWriter file, object value, string date)
        {
            var exportSale = value as ReportSale;
            //var Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            file.WriteLine("Year" + "," + "Month" + "," + "Sales");

            foreach (var item in exportSale.reportMonthlies)
            {
                file.WriteLine(date + "," + item.Monthlyname + "," + item.value);
            }
            return Task.FromResult(true);
        }

        ////วันเดือนปี, ยอดขาย => SaleReport_DD-MM-YYYY-DD-MM-YYYY
        public static Task<bool> CreateDetailReport_Custom_Sale(System.IO.StreamWriter file, object value, string date, List<ORM.Period.SummaryHourly> getDataReportMonth)
        {
            var exportSale = value as ReportSale;
            file.WriteLine("Date" + "," + "Sales");
            var GetDataSalesReport = getDataReportMonth;
           

            var groupdate = GetDataSalesReport.GroupBy(x => x.DateModified.Date).Select(x => new ORM.Period.SummaryHourly
            {
                DateModified = x.First().DateModified,
                SumGrandTotal = x.Sum(x1 => x1.SumGrandTotal),
            }).ToList();

            foreach (var item in groupdate)
            {
                file.WriteLine(Utils.ShowDateReport(item.DateModified) + "," + item.SumGrandTotal);
            }


            string Month = Utils.ShowDateReport(ConvertStartDate).Substring(3, 2);
            string Year = Utils.ShowDateReport(ConvertStartDate).Substring(6, 4);
            var daysInLeap = DateTime.DaysInMonth(Convert.ToInt32(Year), Convert.ToInt32(Month)); //28,29,30,31
            var x2 = date.Split(' ');
            file.WriteLine("Date" + "," + "Sales");

            int i = 0;
            //Check จำนวนวัน ตามเดือนที่จะเขียนลงไฟล์
            foreach (var item in exportSale.reportDailies)
            {
                i++;
                if ((i <= daysInLeap))
                {
                    string dateshow = item.Dailyname + "/" + x2[0] + "/" + x2[1];
                    file.WriteLine(dateshow + "," + item.value);
                }
            }

            return Task.FromResult(true);
        }
        #endregion

        public static async Task SaleReportBranch(System.IO.StreamWriter file, object value, string timeType)
        {
            try
            {

                List<SaleReportBranch> exportSalesByBranch = value as List<SaleReportBranch>;
                switch (timeType)
                {
                    case "Date":
                        var CreateReport = await CreateDetailReportBranch_Today(file, exportSalesByBranch);
                        break;
                    case "Month":
                        CreateReport = await CreateDetailReportBranch_Month(file, exportSalesByBranch);
                        break;
                    case "Year":
                        CreateReport = await CreateDetailReportBranch_Year(file, exportSalesByBranch);
                        break;
                    case "CustomDate":
                        CreateReport = await CreateDetailReportBranch_Custom(file, exportSalesByBranch);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        #region SaleReportBranch
        //วันเดือนปี, ชื่อสาขา, ยอดขาย => SaleReport_Branch_DD-MM-YYYY, 
        public static Task<bool> CreateDetailReportBranch_Today(System.IO.StreamWriter file, List<SaleReportBranch> exportSalesByBranch)
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
        public static Task<bool> CreateDetailReportBranch_Month(System.IO.StreamWriter file, List<SaleReportBranch> exportSalesByBranch)
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
        public static Task<bool> CreateDetailReportBranch_Year(System.IO.StreamWriter file, List<SaleReportBranch> exportSalesByBranch)
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
        public static Task<bool> CreateDetailReportBranch_Custom(System.IO.StreamWriter file, List<SaleReportBranch> exportSalesByBranch)
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

        public static async Task ProfitReport(System.IO.StreamWriter file, object value, string timeType, string StartDate, string endDate, string sysbranchid, string tabSelected)
        {
            try
            {
                List<ORM.Period.SummaryHourly> GetDataSalesReport = new List<ORM.Period.SummaryHourly>();
                GetDataSalesReport = await GabanaAPI.GetDataReportSummaryHourly(sysbranchid, StartDate, EndDate);
                var exportSale = value as ReportSale;
                var ReportProfits = new List<ReportProfit>();
                if (timeType == "Custom")
                {
                    ReportProfits = new List<ReportProfit>();
                    ConvertStartDate = Utils.ChangeDateTimeStringReport(StartDate);
                    ConvertEndDate = Utils.ChangeDateTimeStringReport(endDate);
                    DateTime begin = ConvertStartDate; //some start date
                    DateTime end = ConvertEndDate;
                    List<DateTime> dates = new List<DateTime>();
                    for (DateTime date = begin; date <= end; date = date.AddDays(1))
                    {
                        dates.Add(date);
                    }
                    var reportProfitsCustom = new List<ReportProfit>();
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
                    //if (!string.IsNullOrEmpty(search))
                    //{
                    //    reportProfitsCustom = reportProfitsCustom.Where(x => x.dateTime.ToLower().Contains(search.ToLower())).ToList();
                    //}
                    ReportProfits = reportProfitsCustom;
                    var CreateReport = await CreateDetailProfitReportCustom(file, exportSale, ReportProfits);
                }
                else
                {
                    ReportProfits = new List<ReportProfit>();
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
                    var CreateReport = await CreateDetailProfitReport(file, exportSale, ReportProfits, tabSelected);

                }




            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SalesReport at Bill Detaial");

            }
        }

        public static Task<bool> CreateDetailProfitReportCustom(StreamWriter file, ReportSale exportSale, List<ReportProfit> reportProfits)
        {
            if (exportSale != null)
            {

                file.WriteLine("Date" + "," + "Total Profit" + "," + "Total Sales");

                foreach (var item in reportProfits)
                {
                    file.WriteLine(item.dateTime + "," + item.sumProfitTotal + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public static Task<bool> CreateDetailProfitReport(System.IO.StreamWriter file, ReportSale exportSale, List<ReportProfit> reportProfits, string tabSelected)
        {
            if (exportSale != null)
            {
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

                foreach (var item in reportProfits)
                {
                    file.WriteLine(item.dateTime + "," + item.sumProfitTotal + "," + item.sumGrandTotal);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}