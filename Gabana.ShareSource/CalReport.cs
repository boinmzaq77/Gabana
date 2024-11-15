using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource
{
    public class CalReport
    {
        public async static Task<ReportSale> CalReportSale(List<Gabana.ORM.Period.SummaryHourly> summaryHourlies)
        {
            ReportSale reportSale = initialData();
            foreach (var item in reportSale.reportHourlies)
            {
                item.value = summaryHourlies.Where(x => x.Hourly == item.IdHourly).Sum(x => x.SumGrandTotal);
            }
            foreach (var item in reportSale.reportDailies)
            {
                item.value = summaryHourlies.Where(x => x.Daily == item.IdDaily).Sum(x => x.SumGrandTotal);
            }
            foreach (var item in reportSale.reportWeeklies)
            {
                item.value = summaryHourlies.Where(x => x.DayOfWeek == item.IdWeekly).Sum(x => x.SumGrandTotal);
            }
            foreach (var item in reportSale.reportMonthlies)
            {
                item.value = summaryHourlies.Where(x => x.Monthly == item.IdMonthly).Sum(x => x.SumGrandTotal);
            }
            return reportSale;
        }
        public async static Task<ReportSale> CalReportProfit(List<Gabana.ORM.Period.SummaryHourly> summaryHourlies)
        {
            ReportSale reportSale = initialData();
            foreach (var item in reportSale.reportHourlies)
            {
                item.value = summaryHourlies.Where(x => x.Hourly == item.IdHourly).Sum(x => x.SumProfitAmount);
            }
            foreach (var item in reportSale.reportDailies)
            {
                item.value = summaryHourlies.Where(x => x.Daily == item.IdDaily).Sum(x => x.SumProfitAmount);
            }
            foreach (var item in reportSale.reportWeeklies)
            {
                item.value = summaryHourlies.Where(x => x.DayOfWeek == item.IdWeekly).Sum(x => x.SumProfitAmount);
            }
            foreach (var item in reportSale.reportMonthlies)
            {
                item.value = summaryHourlies.Where(x => x.Monthly == item.IdMonthly).Sum(x => x.SumProfitAmount);
            }
            return reportSale;
        }

        private static ReportSale initialData()
        {
            List<ReportDaily> reportDailies = new List<ReportDaily>();
            for (int i = 0; i < 31; i++)
            {
                ReportDaily reportDaily = new ReportDaily()
                {
                    IdDaily = i + 1,
                    Dailyname = (i + 1).ToString(),
                    value = 0
                };
                reportDailies.Add(reportDaily);
            }
            List<ReportHourly> reportHourlies = new List<ReportHourly>();
            for (int i = 0; i < 24; i++)
            {
                ReportHourly reportHourly = new ReportHourly()
                {
                    IdHourly = i,
                    Hourlyname = i.ToString("D2") + ":00",
                    value = 0
                };
                reportHourlies.Add(reportHourly);
            }
            string[] month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            List<ReportMonthly> reportMonthlies = new List<ReportMonthly>();
            for (int i = 0; i < 12; i++)
            {
                ReportMonthly reportMonthly = new ReportMonthly()
                {
                    IdMonthly = i + 1,
                    Monthlyname = month[i],
                    value = 0
                };
                reportMonthlies.Add(reportMonthly);
            }
            string[] week = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            List<ReportWeekly> reportWeeklies = new List<ReportWeekly>();

            for (int i = 0; i < 7; i++)
            {
                ReportWeekly reportWeekly = new ReportWeekly()
                {
                    IdWeekly = i + 1,
                    Weeklyname = week[i],
                    value = 0
                };
                reportWeeklies.Add(reportWeekly);
            }

            ReportSale reportSale = new ReportSale()
            {
                reportDailies = reportDailies,
                reportHourlies = reportHourlies,
                reportWeeklies = reportWeeklies,
                reportMonthlies = reportMonthlies
            };


            return reportSale;
        }
        public static async Task<List<SaleReportBranch>> initialBranch(List<Gabana3.JAM.Report.SalesByBranchModel> salesByBranches, List<ORM.MerchantDB.Branch> lstBranch)
        {
            List<Gabana3.JAM.Report.SalesByBranchModel> branchModels = new List<Gabana3.JAM.Report.SalesByBranchModel>();
            List<ORM.MerchantDB.Branch> branchList = new List<ORM.MerchantDB.Branch>();
            List<SaleReportBranch> lstsaleReportBranch = new List<SaleReportBranch>();
            branchModels = salesByBranches;
            branchList = lstBranch;

            if (branchModels == null || branchList == null)
            {
                return new List<SaleReportBranch>();
            }
            foreach (var item in branchModels)
            {
                ORM.MerchantDB.Branch branch = branchList.Where(x => x.BranchID == item.sysBranchID.ToString()).FirstOrDefault();
                if (branch != null)
                {
                    SaleReportBranch saleReportBranch = new SaleReportBranch()
                    {
                        BranchName = branch.BranchName,
                        sumGrandTotal = item.sumGrandTotal,
                        BranchID = item.sysBranchID,
                    };
                    lstsaleReportBranch.Add(saleReportBranch);
                }
            }
            return lstsaleReportBranch;
        }
    }

}
