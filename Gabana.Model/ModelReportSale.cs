using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;

namespace Gabana.Model
{
    public class ReportHourly
    {
        public int IdHourly { get; set; }
        public string Hourlyname { get; set; }
        public decimal value { get; set; }
    }
    public class ReportDaily
    {
        public int IdDaily { get; set; }
        public string Dailyname { get; set; }
        public decimal value { get; set; }
    }
    public class ReportWeekly
    {
        public int IdWeekly { get; set; }
        public string Weeklyname { get; set; }
        public decimal value { get; set; }
    }
    public class ReportMonthly
    {
        public int IdMonthly { get; set; }
        public string Monthlyname { get; set; }
        public decimal value { get; set; }
    }
    public class ReportSale
    {
        public List<ReportHourly> reportHourlies { get; set; }
        public List<ReportDaily> reportDailies { get; set; }
        public List<ReportWeekly> reportWeeklies { get; set; }
        public List<ReportMonthly> reportMonthlies { get; set; }
    }


}
