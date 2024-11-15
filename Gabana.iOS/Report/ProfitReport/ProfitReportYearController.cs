using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Microcharts;
using Microcharts.iOS;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ProfitReportYearController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        UICollectionView HourTimeCollectionView, MonthlyTimeCollectionView, WeeklyTimeCollectionView;

        UIView Hour_timeView, Hour_TimeHeadView;
        UILabel lblHours, lblSaleHour;

        UIView Monthly_timeView, Monthly_TimeHeadView;
        UILabel lblMonthly, lblSaleMonthly;

        UIView Weekly_timeView, Weekly_TimeHeadView;
        UILabel lblWeekly, lblSaleWeekly;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch;

        UIView MenuView, HourView, MonthlyView, WeeklyView;
        UILabel lblHourMenu, lblWeeklyMenu, lblMonthlyMenu;
        UIView HourLineView, WeeklyLineView, MonthlyLineView;


        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView HourlyGraphView, MonthlyGraphView, WeeklyGraphView;
        UILabel lbl_GraphHead_Hour, lbl_GraphHead_Monthly, lbl_GraphHead_Weekly;
        ChartView SaleHourChartView, SaleMonthlyChartView, SaleWeeklyChartView;
        ReportSale reportSale;
        List<Gabana.ORM.Period.SummaryHourly> GetDataReportYear;
        private UIView totalView;
        private UILabel lbl_totalHead;
        private UILabel lbl_total;
        private UIView SearchbarView;
        private UITextField txtSearch;
        private UIButton btnSearch;
        private UIButton btnfilter;
        private UIView Viewnull;
        private UIImageView imgnull;
        private UILabel lblnull;
        internal static int filterReport;
        internal static bool isModifyFilter;
        private List<ReportMonthly> report4;
        private List<ReportWeekly> report3;
        private List<ReportHourly> report;
        private string datenamefile;

        public ProfitReportYearController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (isModifyFilter)
            {
                setupData();
                isModifyFilter = false;
            }
        }
        public async override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            try
            {
                base.ViewDidLoad();
                filterReport = 1;
                initAttribute();
                SetupAutoLayout();
                setupData();
                setDefaultPAge();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
            }

        }
        void setDefaultPAge()
        {
            lblHourMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            HourLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblMonthlyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            MonthlyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = false;
            MonthlyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = false;
            Monthly_timeView.Hidden = true;
            Weekly_timeView.Hidden = true;
        }
        internal async void setupData()
        {
            try
            {

           
            lblDay.Text = Utils.TextBundle("thisyear", "Items")+", ";
            lblDate.Text = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                datenamefile = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                string namebranch = "";
            if (ReportController.listChooseBranch.Count == ReportController.listAllBranch.Count)
            {
                namebranch = Utils.TextBundle("allbranch", "Items");
            }
            else
            {
                foreach (var item in ReportController.listChooseBranch)
                {
                    if (namebranch == "")
                    {
                        namebranch += item.BranchName;
                    }
                    else
                    {
                        namebranch += " , " + item.BranchName;
                    }

                }
            }
            lblBranch.Text = namebranch;
                var sysbranIdSelect = "";
                foreach (var item2 in ReportController.listChooseBranch)
                {
                    if (sysbranIdSelect != "")
                    {
                        sysbranIdSelect += "," + item2.SysBranchID.ToString();


                    }
                    else
                    {
                        sysbranIdSelect = item2.SysBranchID.ToString();


                    }
                }
                //graph
                var year = DateTime.UtcNow.Year;
            var startOfYear = new DateTime(year, 1, 1);
            var lastDayOfYear = DateTime.UtcNow;
            var startDate = Utils.ChangeDateTimeReport(startOfYear);
            var endDate = Utils.ChangeDateTimeReport(lastDayOfYear);

            GetDataReportYear = await GabanaAPI.GetDataReportSummaryHourly(sysbranIdSelect, startDate, endDate);

                reportSale = new ReportSale();
                if (GetDataReportYear != null && GetDataReportYear.Count != 0)
                {
                    reportSale = await CalReport.CalReportProfit(GetDataReportYear);
                    #region Hour
                    List<ChartEntry> SalesByPeriods = new List<ChartEntry>();
                     report = new List<ReportHourly>();
                    switch (filterReport)
                    {
                        case 1:
                            report = reportSale.reportHourlies.ToList();
                            break;
                        case 2:
                            report = reportSale.reportHourlies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 3:
                            report = reportSale.reportHourlies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            report = reportSale.reportHourlies.ToList();
                            break;
                    }
                    ReportSaleHourlyDataSource report_adapter_showHour = new ReportSaleHourlyDataSource(report);
                    HourTimeCollectionView.DataSource = report_adapter_showHour;
                    HourTimeCollectionView.ReloadData();

                    lbl_total.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + reportSale.reportWeeklies.Sum(x => x.value).ToString("#,##0.00");

                    //foreach (var item in reportSale.reportHourlies)
                    //{
                    //    Entry entry = new Entry((float)item.value)
                    //    {
                    //        Label = item.Hourlyname.ToString(),
                    //        ValueLabel = item.value.ToString("##,###.00"),
                    //        Color = SKColor.Parse("#0095DA")
                    //    };
                    //    if ((item.IdHourly - 1)%3 == 0)
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
                                ValueLabel = item.value.ToString("##,###.00"),
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
                                ValueLabel = item.value.ToString("##,###.00"),
                                Color = SKColor.Parse("#0095DA"),
                                ValueLabelColor = SKColor.Parse("#0095DA")
                            };

                            ChartEntry entry2 = new ChartEntry((float)item.value)
                            {
                                Label = "",
                                TextColor = SKColor.Parse("#fff"),
                                ValueLabel = item.value.ToString("##,###.00"),
                                Color = SKColor.Parse("#0095DA"),
                                ValueLabelColor = SKColor.Parse("#0095DA")
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
                                ValueLabel = item.value.ToString("##,###.00"),
                                Color = SKColor.Parse("#0095DA"),
                                ValueLabelColor = SKColor.Parse("#0095DA")
                            };

                            SalesByPeriods.Add(entry);
                        }
                    }
                    //exportSale = new ReportSale();
                    //exportSale = reportSale;

                    var chart = new LineChart() { Entries = SalesByPeriods, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleHourChartView.Chart = chart;
                    #endregion
                    #region Weekly
                    List<ChartEntry> SalesByPeriods2 = new List<ChartEntry>();
                    report3 = new List<ReportWeekly>();
                    switch (filterReport)
                    {
                        case 1:
                            report3 = reportSale.reportWeeklies.ToList();
                            break;
                        case 2:
                            report3 = reportSale.reportWeeklies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 3:
                            report3 = reportSale.reportWeeklies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            report3 = reportSale.reportWeeklies.ToList();
                            break;
                    }
                    ReportSaleWeeklyDataSource report_adapter_showWeekly = new ReportSaleWeeklyDataSource(report3);
                    WeeklyTimeCollectionView.DataSource = report_adapter_showWeekly;
                    WeeklyTimeCollectionView.ReloadData();

                    foreach (var item in reportSale.reportWeeklies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Weeklyname.ToString(),
                            ValueLabel = item.value.ToString("##,###.00"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods2.Add(entry);

                    }

                    var chart2 = new LineChart() { Entries = SalesByPeriods2, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleWeeklyChartView.Chart = chart2;
                    #endregion
                    #region Monthly
                    List<ChartEntry> SalesByPeriods1 = new List<ChartEntry>();
                    report4 = new List<ReportMonthly>();
                    switch (filterReport)
                    {
                        case 1:
                            report4 = reportSale.reportMonthlies.ToList();
                            break;
                        case 2:
                            report4 = reportSale.reportMonthlies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 3:
                            report4 = reportSale.reportMonthlies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            report4 = reportSale.reportMonthlies.ToList();
                            break;
                    }
                    ReportSaleMonthlyDataSource report_adapter_showMonth = new ReportSaleMonthlyDataSource(report4);
                    MonthlyTimeCollectionView.DataSource = report_adapter_showMonth;
                    MonthlyTimeCollectionView.ReloadData();

                    foreach (var item in reportSale.reportMonthlies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Monthlyname.ToString(),
                            ValueLabel = item.value.ToString("##,###.00"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods1.Add(entry);
                    }

                    var chart1 = new LineChart() { Entries = SalesByPeriods1, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleMonthlyChartView.Chart = chart1;
                    Utils.SetConstant(Viewnull.Constraints, NSLayoutAttribute.Height, 0);
                    #endregion
                    return;
                }
                else
                {
                    //Utils.SetConstant(Viewnull.Constraints, NSLayoutAttribute.Height, 200);
                    Viewnull.Hidden = false;
                    foreach (var item in _contentView.Subviews)
                    {
                        if (item != Viewnull && item != TopView)
                        {
                            item.Hidden = true;
                        }
                    }

                    BottomView.Hidden = true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        private async void Search()
        {
            var result = await SearchBytxt(txtSearch.Text);
            var result4 = await SearchBytxt4(txtSearch.Text);
            var result3 = await SearchBytxt3(txtSearch.Text);

            ReportSaleHourlyDataSource report_adapter_showHour = new ReportSaleHourlyDataSource(result);
            HourTimeCollectionView.DataSource = report_adapter_showHour;
            HourTimeCollectionView.ReloadData();

            //ReportSaleDailyDataSource report_adapter_showDaily = new ReportSaleDailyDataSource(result2);
            //DaillyTimeCollectionView.DataSource = report_adapter_showDaily;
            //DaillyTimeCollectionView.ReloadData();

            ReportSaleMonthlyDataSource report_adapter_showMonth = new ReportSaleMonthlyDataSource(report4);
            MonthlyTimeCollectionView.DataSource = report_adapter_showMonth;
            MonthlyTimeCollectionView.ReloadData();

            ReportSaleWeeklyDataSource report_adapter_showWeekly = new ReportSaleWeeklyDataSource(result3);
            WeeklyTimeCollectionView.DataSource = report_adapter_showWeekly;
            WeeklyTimeCollectionView.ReloadData();
        }

        private async Task<List<ReportHourly>> SearchBytxt(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return report;
            }
            else
            {
                return report.Where(m => m.Hourlyname.Contains(text)).ToList();
            }

        }
        private async Task<List<ReportMonthly>> SearchBytxt4(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return report4;
            }
            else
            {
                return report4.Where(m => m.Monthlyname.Contains(text)).ToList();
            }

        }
        private async Task<List<ReportWeekly>> SearchBytxt3(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return report3;
            }
            else
            {
                return report3.Where(m => m.Weeklyname.Contains(text)).ToList();
            }

        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region TopView
            TopView = new UIView();
            TopView.BackgroundColor = UIColor.White;
            TopView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblDay = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDay.Font = lblDay.Font.WithSize(15);
            TopView.AddSubview(lblDay);

            lblDate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDate.Font = lblDate.Font.WithSize(15);
            TopView.AddSubview(lblDate);

            lblBranch = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBranch.Font = lblBranch.Font.WithSize(15);
            lblBranch.Text = Utils.TextBundle("merchantname", "Items");
            TopView.AddSubview(lblBranch);

            #endregion

            #region MenuView
            MenuView = new UIView();
            MenuView.BackgroundColor = UIColor.White;
            MenuView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region HourView
            HourView = new UIView();
            HourView.BackgroundColor = UIColor.White;
            HourView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(HourView);

            lblHourMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHourMenu.Text = Utils.TextBundle("hourly", "Items");
            lblHourMenu.Font = lblHourMenu.Font.WithSize(15);
            HourView.AddSubview(lblHourMenu);

            HourLineView = new UIView();
            HourLineView.BackgroundColor = UIColor.White;
            HourLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            HourView.AddSubview(HourLineView);

            HourView.UserInteractionEnabled = true;
            var tapGesturebtnHour = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Hour:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            HourView.AddGestureRecognizer(tapGesturebtnHour);
            #endregion

            #region WeeklyView
            WeeklyView = new UIView();
            WeeklyView.BackgroundColor = UIColor.White;
            WeeklyView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(WeeklyView);

            lblWeeklyMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblWeeklyMenu.Text = Utils.TextBundle("weekly", "Items");
            lblWeeklyMenu.Font = lblWeeklyMenu.Font.WithSize(15);
            WeeklyView.AddSubview(lblWeeklyMenu);

            WeeklyLineView = new UIView();
            WeeklyLineView.BackgroundColor = UIColor.White;
            WeeklyLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            WeeklyView.AddSubview(WeeklyLineView);

            WeeklyView.UserInteractionEnabled = true;
            var tapGesturebtnWeekly = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Weekly:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            WeeklyView.AddGestureRecognizer(tapGesturebtnWeekly);
            #endregion


            #region MonthlyView
            MonthlyView = new UIView();
            MonthlyView.BackgroundColor = UIColor.White;
            MonthlyView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(MonthlyView);

            lblMonthlyMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMonthlyMenu.Text = Utils.TextBundle("monthly", "Items");
            lblMonthlyMenu.Font = lblMonthlyMenu.Font.WithSize(15);
            MonthlyView.AddSubview(lblMonthlyMenu);

            MonthlyLineView = new UIView();
            MonthlyLineView.BackgroundColor = UIColor.White;
            MonthlyLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            MonthlyView.AddSubview(MonthlyLineView);

            MonthlyView.UserInteractionEnabled = true;
            var tapGesturebtnDaily = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Monthly:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MonthlyView.AddGestureRecognizer(tapGesturebtnDaily);
            #endregion

            #endregion

            #region HourlyGraphView
            HourlyGraphView = new UIView();
            HourlyGraphView.BackgroundColor = UIColor.White;
            HourlyGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead_Hour = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead_Hour.Text = Utils.TextBundle("profitsbyhour", "Items");
            lbl_GraphHead_Hour.Font = lbl_GraphHead_Hour.Font.WithSize(15);
            HourlyGraphView.AddSubview(lbl_GraphHead_Hour);

            SaleHourChartView = new ChartView();
            SaleHourChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleHourChartView.BackgroundColor = UIColor.White;
            HourlyGraphView.AddSubview(SaleHourChartView);
            #endregion

            #region WeeklyGraphView
            WeeklyGraphView = new UIView();
            WeeklyGraphView.BackgroundColor = UIColor.White;
            WeeklyGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead_Weekly = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead_Weekly.Text = Utils.TextBundle("profitsbyweek", "Items");
            lbl_GraphHead_Weekly.Font = lbl_GraphHead_Weekly.Font.WithSize(15);
            WeeklyGraphView.AddSubview(lbl_GraphHead_Weekly);

            SaleWeeklyChartView = new ChartView();
            SaleWeeklyChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleWeeklyChartView.BackgroundColor = UIColor.White;
            WeeklyGraphView.AddSubview(SaleWeeklyChartView);
            #endregion

            #region MonthlyGraphView
            MonthlyGraphView = new UIView();
            MonthlyGraphView.BackgroundColor = UIColor.White;
            MonthlyGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead_Monthly = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead_Monthly.Text = Utils.TextBundle("profitsbymonth", "Items");
            lbl_GraphHead_Monthly.Font = lbl_GraphHead_Monthly.Font.WithSize(15);
            MonthlyGraphView.AddSubview(lbl_GraphHead_Monthly);

            SaleMonthlyChartView = new ChartView();
            SaleMonthlyChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleMonthlyChartView.BackgroundColor = UIColor.White;
            MonthlyGraphView.AddSubview(SaleMonthlyChartView);
            #endregion

            #region totalView
            totalView = new UIView();
            totalView.BackgroundColor = UIColor.White;
            totalView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_totalHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_totalHead.Text = Utils.TextBundle("allprofits", "Items");
            lbl_totalHead.Font = lbl_totalHead.Font.WithSize(15);
            totalView.AddSubview(lbl_totalHead);

            lbl_total = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_total.TextAlignment = UITextAlignment.Right;
            lbl_total.Text = "";
            lbl_total.Font = lbl_total.Font.WithSize(20);
            totalView.AddSubview(lbl_total);
            _contentView.AddSubview(totalView);
            #endregion

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.White;
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(SearchbarView);

            txtSearch = new UITextField
            {
                Placeholder = "",
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSearch.BackgroundColor = UIColor.Clear;
            txtSearch.Font = txtSearch.Font.WithSize(15);
            txtSearch.ReturnKeyType = UIReturnKeyType.Done;
            txtSearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                Search();
                //SearchBytxt();
                return true;
            };

            SearchbarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                txtSearch.BecomeFirstResponder();
            };
            SearchbarView.AddSubview(btnSearch);

            btnSearch.TouchUpInside += (sender, e) => {
                // search function
                if (btnSearch.CurrentBackgroundImage == UIImage.FromBundle("DelTxt"))
                {
                    txtSearch.Text = "";
                    Search();
                    btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                }
                else
                {
                    txtSearch.BecomeFirstResponder();
                }

            };
            txtSearch.AddTarget((sender, e) =>
            {
                if (txtSearch.Text == "")
                {
                    btnSearch.SetBackgroundImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                }
                else
                {
                    btnSearch.SetBackgroundImage(UIImage.FromBundle("DelTxt"), UIControlState.Normal);

                }

            }, UIControlEvent.EditingChanged);

            btnfilter = new UIButton();
            btnfilter.SetImage(UIImage.FromFile("ReportFilter.png"), UIControlState.Normal);
            btnfilter.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfilter.TouchUpInside += (sender, e) =>
            {
                var filterPage = new ReportFilter2Controller(6);
                this.NavigationController.PushViewController(filterPage, false);
            };
            SearchbarView.AddSubview(btnfilter);
            #endregion

            #region Hour_timeView
            Hour_timeView = new UIView();
            Hour_timeView.BackgroundColor = UIColor.White;
            Hour_timeView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region Hour_TimeHeadView
            Hour_TimeHeadView = new UIView();
            Hour_TimeHeadView.BackgroundColor = UIColor.White;
            Hour_TimeHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            Hour_timeView.AddSubview(Hour_TimeHeadView);

            lblHours = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHours.Font = lblHours.Font.WithSize(15);
            lblHours.Text = Utils.TextBundle("hour", "Items");
            Hour_TimeHeadView.AddSubview(lblHours);

            lblSaleHour = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleHour.Font = lblSaleHour.Font.WithSize(15);
            lblSaleHour.Text = Utils.TextBundle("profit", "Items");
            Hour_TimeHeadView.AddSubview(lblSaleHour);
            #endregion

            #region TimeCollectionView
            UICollectionViewFlowLayout item1flowLayoutList = new UICollectionViewFlowLayout();
            item1flowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 30);
            item1flowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            HourTimeCollectionView = new UICollectionView(frame: View.Frame, layout: item1flowLayoutList);
            HourTimeCollectionView.BackgroundColor = UIColor.White;
            HourTimeCollectionView.ScrollEnabled = false;
            HourTimeCollectionView.ShowsVerticalScrollIndicator = false;
            HourTimeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            HourTimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleViewCell), reuseIdentifier: "HourSaleViewCell");
            Hour_timeView.AddSubview(HourTimeCollectionView);
            #endregion
            #endregion

            #region Weekly_timeView
            Weekly_timeView = new UIView();
            Weekly_timeView.BackgroundColor = UIColor.White;
            Weekly_timeView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region Weekly_TimeHeadView
            Weekly_TimeHeadView = new UIView();
            Weekly_TimeHeadView.BackgroundColor = UIColor.White;
            Weekly_TimeHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            Weekly_timeView.AddSubview(Weekly_TimeHeadView);

            lblWeekly = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblWeekly.Font = lblWeekly.Font.WithSize(15);
            lblWeekly.Text = Utils.TextBundle("Weekly", "Items");
            Weekly_TimeHeadView.AddSubview(lblWeekly);

            lblSaleWeekly = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleWeekly.Font = lblSaleWeekly.Font.WithSize(15);
            lblSaleWeekly.Text = Utils.TextBundle("profit", "Items");
            Weekly_TimeHeadView.AddSubview(lblSaleWeekly);

            lblDay = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDay.Font = lblDay.Font.WithSize(15);
            TopView.AddSubview(lblDay);
            #endregion

            #region TimeCollectionView
            UICollectionViewFlowLayout item3flowLayoutList = new UICollectionViewFlowLayout();
            item3flowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 30);
            item3flowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            WeeklyTimeCollectionView = new UICollectionView(frame: View.Frame, layout: item3flowLayoutList);
            WeeklyTimeCollectionView.BackgroundColor = UIColor.White;
            WeeklyTimeCollectionView.ScrollEnabled = false;
            WeeklyTimeCollectionView.ShowsVerticalScrollIndicator = false;
            WeeklyTimeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            WeeklyTimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleViewCell), reuseIdentifier: "HourSaleViewCell");
            Weekly_timeView.AddSubview(WeeklyTimeCollectionView);
            #endregion
            #endregion

            #region Monthly_timeView
            Monthly_timeView = new UIView();
            Monthly_timeView.BackgroundColor = UIColor.White;
            Monthly_timeView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region Monthly_TimeHeadView
            Monthly_TimeHeadView = new UIView();
            Monthly_TimeHeadView.BackgroundColor = UIColor.White;
            Monthly_TimeHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            Monthly_timeView.AddSubview(Monthly_TimeHeadView);

            lblMonthly = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMonthly.Font = lblMonthly.Font.WithSize(15);
            lblMonthly.Text = Utils.TextBundle("monthly", "Items");
            Monthly_TimeHeadView.AddSubview(lblMonthly);

            lblSaleMonthly = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleMonthly.Font = lblSaleMonthly.Font.WithSize(15);
            lblSaleMonthly.Text = Utils.TextBundle("profit", "Items");
            Monthly_TimeHeadView.AddSubview(lblSaleMonthly);

            lblDay = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDay.Font = lblDay.Font.WithSize(15);
            TopView.AddSubview(lblDay);
            #endregion

            #region MonthlyTimeCollectionView
            UICollectionViewFlowLayout item2flowLayoutList = new UICollectionViewFlowLayout();
            item2flowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 30);
            item2flowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            MonthlyTimeCollectionView = new UICollectionView(frame: View.Frame, layout: item2flowLayoutList);
            MonthlyTimeCollectionView.BackgroundColor = UIColor.White;
            MonthlyTimeCollectionView.ScrollEnabled = false;
            MonthlyTimeCollectionView.ShowsVerticalScrollIndicator = false;
            MonthlyTimeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            MonthlyTimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleViewCell), reuseIdentifier: "HourSaleViewCell");
            Monthly_timeView.AddSubview(MonthlyTimeCollectionView);
            #endregion
            #endregion

            Viewnull = new UIView();
            Viewnull.TranslatesAutoresizingMaskIntoConstraints = false;
            //Viewnull.Hidden = true;
            _contentView.AddSubview(Viewnull);

            imgnull = new UIImageView();
            imgnull.Image = UIImage.FromBundle("DefaultReport");
            imgnull.TranslatesAutoresizingMaskIntoConstraints = false;
            Viewnull.AddSubview(imgnull);

            lblnull = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblnull.Text = Utils.TextBundle("nothavedatasell", "Items");
            lblnull.Font = lblnull.Font.WithSize(16);
            Viewnull.AddSubview(lblnull);

            _contentView.AddSubview(MenuView);
            _contentView.AddSubview(TopView);
            _contentView.AddSubview(MonthlyGraphView);
            _contentView.AddSubview(Monthly_timeView);
            _contentView.AddSubview(WeeklyGraphView);
            _contentView.AddSubview(Weekly_timeView);
            _contentView.AddSubview(HourlyGraphView);
            _contentView.AddSubview(Hour_timeView);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.White;
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;



            #region btnPDF
            btnPDF = new UIView();
            btnPDF.Layer.CornerRadius = 5;
            btnPDF.Layer.BorderWidth = 1;
            btnPDF.Layer.BorderColor = UIColor.FromRGB(200, 200, 200).CGColor;
            btnPDF.ClipsToBounds = true;
            btnPDF.TranslatesAutoresizingMaskIntoConstraints = false;
            BottomView.AddSubview(btnPDF);

            btnPDFImg = new UIImageView();
            btnPDFImg.Image = UIImage.FromFile("BillPdf.png");
            btnPDFImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnPDF.AddSubview(btnPDFImg);

            lbl_btnPDF = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_btnPDF.Text = "PDF";
            lbl_btnPDF.Font = lbl_btnPDF.Font.WithSize(13);
            btnPDF.AddSubview(lbl_btnPDF);

            btnPDF.UserInteractionEnabled = true;
            var tapGesturePDF = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("PDF:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnPDF.AddGestureRecognizer(tapGesturePDF);
            #endregion

            #region btnEmail
            btnEmail = new UIView();
            btnEmail.Layer.CornerRadius = 5;
            btnEmail.Layer.BorderWidth = 1;
            btnEmail.Layer.BorderColor = UIColor.FromRGB(200, 200, 200).CGColor;
            btnEmail.ClipsToBounds = true;
            btnEmail.TranslatesAutoresizingMaskIntoConstraints = false;
            BottomView.AddSubview(btnEmail);

            btnEmailImg = new UIImageView();
            btnEmailImg.Image = UIImage.FromFile("BillMail.png");
            btnEmailImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnEmail.AddSubview(btnEmailImg);

            lbl_btnEmail = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_btnEmail.Text = Utils.TextBundle("email", "Items");
            lbl_btnEmail.Font = lbl_btnEmail.Font.WithSize(13);
            btnEmail.AddSubview(lbl_btnEmail);

            btnEmail.UserInteractionEnabled = true;
            var tapGesturebtnEmail = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("EMail:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnEmail.AddGestureRecognizer(tapGesturebtnEmail);
            #endregion

            #region btnShare
            btnShare = new UIView();
            btnShare.Layer.CornerRadius = 5;
            btnShare.Layer.BorderWidth = 1;
            btnShare.Layer.BorderColor = UIColor.FromRGB(200, 200, 200).CGColor;
            btnShare.ClipsToBounds = true;
            btnShare.TranslatesAutoresizingMaskIntoConstraints = false;
            BottomView.AddSubview(btnShare);

            btnShareImg = new UIImageView();
            btnShareImg.Image = UIImage.FromFile("BillShare.png");
            btnShareImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnShare.AddSubview(btnShareImg);

            lbl_btnShare = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_btnShare.Text = Utils.TextBundle("export", "Items");
            lbl_btnShare.Font = lbl_btnShare.Font.WithSize(13);
            btnShare.AddSubview(lbl_btnShare);

            btnShare.UserInteractionEnabled = true;
            var tapGesturebtnShare = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Share:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnShare.AddGestureRecognizer(tapGesturebtnShare);
            #endregion

            #endregion
            View.AddSubview(BottomView);
        }
        #region bottom view toggle
        [Export("Hour:")]
        public void Hour(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            HourLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblMonthlyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            MonthlyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = false;
            MonthlyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = false;
            Monthly_timeView.Hidden = true;
            Weekly_timeView.Hidden = true;

            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 1430);
            _contentView.LayoutIfNeeded();
        }
        [Export("Monthly:")]
        public void Monthly(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            HourLineView.BackgroundColor = UIColor.White;

            lblMonthlyMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            MonthlyLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = true;
            MonthlyGraphView.Hidden = false;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = true;
            Monthly_timeView.Hidden = false;
            Weekly_timeView.Hidden = true;


            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 1080);
            _contentView.LayoutIfNeeded();
        }
        [Export("Weekly:")]
        public void Weekly(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            HourLineView.BackgroundColor = UIColor.White;

            lblMonthlyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            MonthlyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            WeeklyLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            HourlyGraphView.Hidden = true;
            MonthlyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = false;

            Hour_timeView.Hidden = true;
            Monthly_timeView.Hidden = true;
            Weekly_timeView.Hidden = false;


            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 750);
            _contentView.LayoutIfNeeded();


        }
        [Export("PDF:")]
        public void PDF(UIGestureRecognizer sender)
        {
            UIImage uIImage;
            NSData pdf = null;
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                //var iamg = (cell as ItemPOSCollectionViewCell).getimage();
                var r = new UIGraphicsImageRenderer(_contentView.Bounds.Size);


                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    _contentView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                var r2 = new UIGraphicsPdfRenderer(_contentView.Bounds, UIGraphicsPdfRendererFormat.DefaultFormat);
                //UIGraphicsPDFRenderer
                pdf = r2.CreatePdf((UIGraphicsPdfRendererContext ctxt) =>
                {
                    ctxt.BeginPage();
                    img.Draw(new CoreGraphics.CGRect() { X = 0, Y = 0, Size = img.Size });
                    //View.Capture(true);
                });
                ////var img = View.Capture(true);
                uIImage = img;
            }


            var activityItems = new NSObject[] { pdf };
            UIActivity[] applicationActivities = null;


            var items = new List<NSObject>();

            items.Add(pdf);

            var controller = new UIActivityViewController(items.ToArray(), applicationActivities);

            //var activityController = new UIActivityViewController(activityItems, null);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // Phone
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(controller, true, null);
            }
        }
        [Export("EMail:")]
        public void EMail(UIGestureRecognizer sender)
        {
            UIImage uIImage;

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                //var iamg = (cell as ItemPOSCollectionViewCell).getimage();
                var r = new UIGraphicsImageRenderer(_contentView.Bounds.Size);
                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    _contentView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                ////var img = View.Capture(true);
                uIImage = img;
            }
            else
            {
                UIGraphics.BeginImageContextWithOptions(_contentView.Bounds.Size, _contentView.Opaque, 0);
                _contentView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                //View.Layer.DrawInContext(UIGraphics.GetCurrentContext());
                var img = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
                uIImage = img;

            }

            var activityItems = new NSObject[] { uIImage };
            UIActivity[] applicationActivities = null;

            var items = new List<NSObject>();

            items.Add(uIImage);

            var controller = new UIActivityViewController(items.ToArray(), applicationActivities);

            var activityController = new UIActivityViewController(items.ToArray(), null);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // Phone
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(controller, true, null);
            }
        }
        [Export("Share:")]
        public async void Share(UIGestureRecognizer sender)
        {
            try
            {
                var file = "ProfitReport_" + datenamefile.ToString(); ;
                ReportManager.createDetail("ProfitReport", file, reportSale, "Hourly", "Hourly", null, null, GetDataReportYear, ReportController.listChooseBranch[0].SysBranchID.ToString(), lblBranch.Text);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }


        }
        #endregion
        void SetupAutoLayout()
        {
            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(83).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region btnPDF
            btnPDF.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnPDF.CenterYAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnPDF.HeightAnchor.ConstraintEqualTo(63).Active = true;
            btnPDF.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 40) / 3).Active = true;

            btnPDFImg.TopAnchor.ConstraintEqualTo(btnPDFImg.Superview.TopAnchor, 9).Active = true;
            btnPDFImg.CenterXAnchor.ConstraintEqualTo(btnPDFImg.Superview.CenterXAnchor).Active = true;
            btnPDFImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnPDFImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_btnPDF.TopAnchor.ConstraintEqualTo(btnPDFImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_btnPDF.CenterXAnchor.ConstraintEqualTo(btnPDFImg.Superview.CenterXAnchor).Active = true;
            lbl_btnPDF.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_btnPDF.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region btnEmail
            btnEmail.CenterYAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnEmail.LeftAnchor.ConstraintEqualTo(btnPDF.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnEmail.HeightAnchor.ConstraintEqualTo(63).Active = true;
            btnEmail.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 40) / 3).Active = true;

            btnEmailImg.TopAnchor.ConstraintEqualTo(btnEmailImg.Superview.TopAnchor, 9).Active = true;
            btnEmailImg.CenterXAnchor.ConstraintEqualTo(btnEmailImg.Superview.CenterXAnchor).Active = true;
            btnEmailImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnEmailImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_btnEmail.TopAnchor.ConstraintEqualTo(btnEmailImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_btnEmail.CenterXAnchor.ConstraintEqualTo(btnEmailImg.Superview.CenterXAnchor).Active = true;
            lbl_btnEmail.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_btnEmail.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion





            #region btnShare
            btnShare.LeftAnchor.ConstraintEqualTo(btnEmail.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnShare.CenterYAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnShare.HeightAnchor.ConstraintEqualTo(63).Active = true;
            btnShare.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 40) / 3).Active = true;

            btnShareImg.TopAnchor.ConstraintEqualTo(btnShareImg.Superview.TopAnchor, 9).Active = true;
            btnShareImg.CenterXAnchor.ConstraintEqualTo(btnShareImg.Superview.CenterXAnchor).Active = true;
            btnShareImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnShareImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_btnShare.TopAnchor.ConstraintEqualTo(btnShareImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_btnShare.CenterXAnchor.ConstraintEqualTo(btnShareImg.Superview.CenterXAnchor).Active = true;
            lbl_btnShare.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_btnShare.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion
            #endregion

            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.HeightAnchor.ConstraintEqualTo(1430).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region TopView
            TopView.TopAnchor.ConstraintEqualTo(TopView.Superview.TopAnchor, 0).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(TopView.Superview.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(TopView.Superview.RightAnchor, 0).Active = true;

            lblDay.CenterYAnchor.ConstraintEqualTo(lblDay.Superview.CenterYAnchor, 0).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(lblDay.Superview.LeftAnchor, 15).Active = true;
            // lblDay.WidthAnchor.ConstraintEqualTo(15).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(lblDate.Superview.CenterYAnchor, 0).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblDate.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblBranch.CenterYAnchor.ConstraintEqualTo(lblBranch.Superview.CenterYAnchor, 0).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(lblBranch.Superview.RightAnchor, -20).Active = true;
            lblBranch.WidthAnchor.ConstraintEqualTo(150).Active = true;

            #endregion

            Viewnull.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            Viewnull.LeftAnchor.ConstraintEqualTo(Viewnull.Superview.LeftAnchor, 0).Active = true;
            Viewnull.RightAnchor.ConstraintEqualTo(Viewnull.Superview.RightAnchor, 0).Active = true;
            Viewnull.HeightAnchor.ConstraintEqualTo(220).Active = true;

            imgnull.TopAnchor.ConstraintEqualTo(Viewnull.TopAnchor, 0).Active = true;
            imgnull.LeftAnchor.ConstraintEqualTo(Viewnull.LeftAnchor, 0).Active = true;
            imgnull.RightAnchor.ConstraintEqualTo(Viewnull.RightAnchor, 0).Active = true;
            imgnull.BottomAnchor.ConstraintEqualTo(Viewnull.BottomAnchor, -20).Active = true;

            lblnull.TopAnchor.ConstraintEqualTo(imgnull.BottomAnchor, 0).Active = true;
            lblnull.LeftAnchor.ConstraintEqualTo(Viewnull.LeftAnchor, 0).Active = true;
            lblnull.RightAnchor.ConstraintEqualTo(Viewnull.RightAnchor, 0).Active = true;
            lblnull.BottomAnchor.ConstraintEqualTo(Viewnull.BottomAnchor, 0).Active = true;


            #region MenuView
            MenuView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            MenuView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            MenuView.LeftAnchor.ConstraintEqualTo(MenuView.Superview.LeftAnchor, 0).Active = true;
            MenuView.RightAnchor.ConstraintEqualTo(MenuView.Superview.RightAnchor, 0).Active = true;

            #region HourView
            HourView.TopAnchor.ConstraintEqualTo(HourView.Superview.TopAnchor, 0).Active = true;
            HourView.BottomAnchor.ConstraintEqualTo(HourView.Superview.BottomAnchor, 0).Active = true;
            HourView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
            HourView.LeftAnchor.ConstraintEqualTo(HourView.Superview.LeftAnchor, 0).Active = true;

            lblHourMenu.CenterXAnchor.ConstraintEqualTo(lblHourMenu.Superview.CenterXAnchor).Active = true;
            lblHourMenu.CenterYAnchor.ConstraintEqualTo(lblHourMenu.Superview.CenterYAnchor).Active = true;
            lblHourMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            HourLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            HourLineView.BottomAnchor.ConstraintEqualTo(HourView.Superview.BottomAnchor, 0).Active = true;
            HourLineView.RightAnchor.ConstraintEqualTo(HourLineView.Superview.RightAnchor).Active = true;
            HourLineView.LeftAnchor.ConstraintEqualTo(HourLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #region WeeklyView
            WeeklyView.TopAnchor.ConstraintEqualTo(WeeklyView.Superview.TopAnchor, 0).Active = true;
            WeeklyView.BottomAnchor.ConstraintEqualTo(WeeklyView.Superview.BottomAnchor, 0).Active = true;
            WeeklyView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
            WeeklyView.LeftAnchor.ConstraintEqualTo(HourView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblWeeklyMenu.CenterXAnchor.ConstraintEqualTo(lblWeeklyMenu.Superview.CenterXAnchor).Active = true;
            lblWeeklyMenu.CenterYAnchor.ConstraintEqualTo(lblWeeklyMenu.Superview.CenterYAnchor).Active = true;
            lblWeeklyMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            WeeklyLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            WeeklyLineView.BottomAnchor.ConstraintEqualTo(WeeklyLineView.Superview.BottomAnchor, 0).Active = true;
            WeeklyLineView.RightAnchor.ConstraintEqualTo(WeeklyLineView.Superview.RightAnchor).Active = true;
            WeeklyLineView.LeftAnchor.ConstraintEqualTo(WeeklyLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #region MonthlyView
            MonthlyView.TopAnchor.ConstraintEqualTo(MonthlyView.Superview.TopAnchor, 0).Active = true;
            MonthlyView.BottomAnchor.ConstraintEqualTo(MonthlyView.Superview.BottomAnchor, 0).Active = true;
            MonthlyView.RightAnchor.ConstraintEqualTo(MonthlyView.Superview.RightAnchor).Active = true;
            MonthlyView.LeftAnchor.ConstraintEqualTo(WeeklyView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblMonthlyMenu.CenterXAnchor.ConstraintEqualTo(lblMonthlyMenu.Superview.CenterXAnchor).Active = true;
            lblMonthlyMenu.CenterYAnchor.ConstraintEqualTo(lblMonthlyMenu.Superview.CenterYAnchor).Active = true;
            lblMonthlyMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            MonthlyLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            MonthlyLineView.BottomAnchor.ConstraintEqualTo(MonthlyLineView.Superview.BottomAnchor, 0).Active = true;
            MonthlyLineView.RightAnchor.ConstraintEqualTo(MonthlyLineView.Superview.RightAnchor).Active = true;
            MonthlyLineView.LeftAnchor.ConstraintEqualTo(MonthlyLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #endregion

            #region HourlyGraphView
            HourlyGraphView.TopAnchor.ConstraintEqualTo(MenuView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            HourlyGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;
            HourlyGraphView.LeftAnchor.ConstraintEqualTo(HourlyGraphView.Superview.LeftAnchor, 0).Active = true;
            HourlyGraphView.RightAnchor.ConstraintEqualTo(HourlyGraphView.Superview.RightAnchor, 0).Active = true;

            lbl_GraphHead_Hour.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Hour.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead_Hour.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead_Hour.LeftAnchor.ConstraintEqualTo(lbl_GraphHead_Hour.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead_Hour.WidthAnchor.ConstraintEqualTo(120).Active = true;

            SaleHourChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Hour.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            SaleHourChartView.RightAnchor.ConstraintEqualTo(SaleHourChartView.Superview.RightAnchor).Active = true;
            SaleHourChartView.LeftAnchor.ConstraintEqualTo(SaleHourChartView.Superview.LeftAnchor, 0).Active = true;
            SaleHourChartView.BottomAnchor.ConstraintEqualTo(SaleHourChartView.Superview.BottomAnchor).Active = true;
            #endregion

            #region WeeklyGraphView
            WeeklyGraphView.TopAnchor.ConstraintEqualTo(MenuView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            WeeklyGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;
            WeeklyGraphView.LeftAnchor.ConstraintEqualTo(WeeklyGraphView.Superview.LeftAnchor, 0).Active = true;
            WeeklyGraphView.RightAnchor.ConstraintEqualTo(WeeklyGraphView.Superview.RightAnchor, 0).Active = true;

            lbl_GraphHead_Weekly.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Weekly.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead_Weekly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead_Weekly.LeftAnchor.ConstraintEqualTo(lbl_GraphHead_Weekly.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead_Weekly.WidthAnchor.ConstraintEqualTo(150).Active = true;

            SaleWeeklyChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Weekly.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            SaleWeeklyChartView.RightAnchor.ConstraintEqualTo(SaleWeeklyChartView.Superview.RightAnchor).Active = true;
            SaleWeeklyChartView.LeftAnchor.ConstraintEqualTo(SaleWeeklyChartView.Superview.LeftAnchor, 0).Active = true;
            SaleWeeklyChartView.BottomAnchor.ConstraintEqualTo(SaleWeeklyChartView.Superview.BottomAnchor).Active = true;
            #endregion

            #region MonthlyGraphView
            MonthlyGraphView.TopAnchor.ConstraintEqualTo(MenuView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            MonthlyGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;
            MonthlyGraphView.LeftAnchor.ConstraintEqualTo(MonthlyGraphView.Superview.LeftAnchor, 0).Active = true;
            MonthlyGraphView.RightAnchor.ConstraintEqualTo(MonthlyGraphView.Superview.RightAnchor, 0).Active = true;

            lbl_GraphHead_Monthly.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Monthly.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead_Monthly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead_Monthly.LeftAnchor.ConstraintEqualTo(lbl_GraphHead_Monthly.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead_Monthly.WidthAnchor.ConstraintEqualTo(150).Active = true;

            SaleMonthlyChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Monthly.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            SaleMonthlyChartView.RightAnchor.ConstraintEqualTo(SaleMonthlyChartView.Superview.RightAnchor).Active = true;
            SaleMonthlyChartView.LeftAnchor.ConstraintEqualTo(SaleMonthlyChartView.Superview.LeftAnchor, 0).Active = true;
            SaleMonthlyChartView.BottomAnchor.ConstraintEqualTo(SaleMonthlyChartView.Superview.BottomAnchor).Active = true;
            #endregion

            #region totalView
            totalView.TopAnchor.ConstraintEqualTo(WeeklyGraphView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            totalView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            totalView.LeftAnchor.ConstraintEqualTo(totalView.Superview.LeftAnchor, 0).Active = true;
            totalView.RightAnchor.ConstraintEqualTo(totalView.Superview.RightAnchor, 0).Active = true;

            lbl_totalHead.TopAnchor.ConstraintEqualTo(lbl_totalHead.Superview.TopAnchor, 7).Active = true;
            lbl_totalHead.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_totalHead.LeftAnchor.ConstraintEqualTo(lbl_totalHead.Superview.LeftAnchor, 15).Active = true;
            lbl_totalHead.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lbl_total.TopAnchor.ConstraintEqualTo(lbl_totalHead.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lbl_total.RightAnchor.ConstraintEqualTo(lbl_total.Superview.RightAnchor, -15).Active = true;
            lbl_total.LeftAnchor.ConstraintEqualTo(lbl_total.Superview.LeftAnchor, 0).Active = true;
            lbl_total.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion

            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(totalView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(btnfilter.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            btnfilter.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnfilter.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnfilter.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnfilter.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;
            #endregion

            #region Weekly_timeView
            Weekly_timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            Weekly_timeView.RightAnchor.ConstraintEqualTo(Weekly_timeView.Superview.RightAnchor).Active = true;
            Weekly_timeView.LeftAnchor.ConstraintEqualTo(Weekly_timeView.Superview.LeftAnchor, 0).Active = true;
            Weekly_timeView.BottomAnchor.ConstraintEqualTo(Weekly_timeView.Superview.BottomAnchor).Active = true;
            Weekly_timeView.HeightAnchor.ConstraintEqualTo(500);

            #region Weekly_TimeHeadView
            Weekly_TimeHeadView.TopAnchor.ConstraintEqualTo(Weekly_TimeHeadView.Superview.TopAnchor, 0).Active = true;
            Weekly_TimeHeadView.RightAnchor.ConstraintEqualTo(Weekly_TimeHeadView.Superview.RightAnchor).Active = true;
            Weekly_TimeHeadView.LeftAnchor.ConstraintEqualTo(Weekly_TimeHeadView.Superview.LeftAnchor, 0).Active = true;
            Weekly_TimeHeadView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lblWeekly.CenterYAnchor.ConstraintEqualTo(lblWeekly.Superview.CenterYAnchor, 0).Active = true;
            lblWeekly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblWeekly.LeftAnchor.ConstraintEqualTo(lblWeekly.Superview.LeftAnchor, 20).Active = true;
            lblWeekly.WidthAnchor.ConstraintEqualTo(50).Active = true;

            lblSaleWeekly.CenterYAnchor.ConstraintEqualTo(lblSaleWeekly.Superview.CenterYAnchor, 0).Active = true;
            lblSaleWeekly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSaleWeekly.RightAnchor.ConstraintEqualTo(lblSaleWeekly.Superview.RightAnchor, -20).Active = true;
            lblSaleWeekly.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region WeeklyTimeCollectionView
            WeeklyTimeCollectionView.TopAnchor.ConstraintEqualTo(Weekly_TimeHeadView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            WeeklyTimeCollectionView.RightAnchor.ConstraintEqualTo(WeeklyTimeCollectionView.Superview.RightAnchor).Active = true;
            WeeklyTimeCollectionView.LeftAnchor.ConstraintEqualTo(WeeklyTimeCollectionView.Superview.LeftAnchor, 0).Active = true;
            WeeklyTimeCollectionView.BottomAnchor.ConstraintEqualTo(WeeklyTimeCollectionView.Superview.BottomAnchor).Active = true;
            #endregion
            #endregion

            #region Monthly_timeView
            Monthly_timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            Monthly_timeView.RightAnchor.ConstraintEqualTo(Monthly_timeView.Superview.RightAnchor).Active = true;
            Monthly_timeView.LeftAnchor.ConstraintEqualTo(Monthly_timeView.Superview.LeftAnchor, 0).Active = true;
            Monthly_timeView.BottomAnchor.ConstraintEqualTo(Monthly_timeView.Superview.BottomAnchor).Active = true;
            Monthly_timeView.HeightAnchor.ConstraintEqualTo(500);

            #region Monthly_TimeHeadView
            Monthly_TimeHeadView.TopAnchor.ConstraintEqualTo(Monthly_TimeHeadView.Superview.TopAnchor, 0).Active = true;
            Monthly_TimeHeadView.RightAnchor.ConstraintEqualTo(Monthly_TimeHeadView.Superview.RightAnchor).Active = true;
            Monthly_TimeHeadView.LeftAnchor.ConstraintEqualTo(Monthly_TimeHeadView.Superview.LeftAnchor, 0).Active = true;
            Monthly_TimeHeadView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lblMonthly.CenterYAnchor.ConstraintEqualTo(lblMonthly.Superview.CenterYAnchor, 0).Active = true;
            lblMonthly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMonthly.LeftAnchor.ConstraintEqualTo(lblMonthly.Superview.LeftAnchor, 20).Active = true;
            lblMonthly.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblSaleMonthly.CenterYAnchor.ConstraintEqualTo(lblSaleMonthly.Superview.CenterYAnchor, 0).Active = true;
            lblSaleMonthly.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSaleMonthly.RightAnchor.ConstraintEqualTo(lblSaleMonthly.Superview.RightAnchor, -20).Active = true;
            lblSaleMonthly.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region MonthlyTimeCollectionView
            MonthlyTimeCollectionView.TopAnchor.ConstraintEqualTo(Monthly_TimeHeadView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MonthlyTimeCollectionView.RightAnchor.ConstraintEqualTo(MonthlyTimeCollectionView.Superview.RightAnchor).Active = true;
            MonthlyTimeCollectionView.LeftAnchor.ConstraintEqualTo(MonthlyTimeCollectionView.Superview.LeftAnchor, 0).Active = true;
            MonthlyTimeCollectionView.BottomAnchor.ConstraintEqualTo(MonthlyTimeCollectionView.Superview.BottomAnchor).Active = true;
            #endregion
            #endregion

            #region Hour_timeView
            Hour_timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            Hour_timeView.RightAnchor.ConstraintEqualTo(Hour_timeView.Superview.RightAnchor).Active = true;
            Hour_timeView.LeftAnchor.ConstraintEqualTo(Hour_timeView.Superview.LeftAnchor, 0).Active = true;
            Hour_timeView.BottomAnchor.ConstraintEqualTo(Hour_timeView.Superview.BottomAnchor).Active = true;
            Hour_timeView.HeightAnchor.ConstraintEqualTo(1000);

            #region Hour_TimeHeadView
            Hour_TimeHeadView.TopAnchor.ConstraintEqualTo(Hour_TimeHeadView.Superview.TopAnchor, 0).Active = true;
            Hour_TimeHeadView.RightAnchor.ConstraintEqualTo(Hour_TimeHeadView.Superview.RightAnchor).Active = true;
            Hour_TimeHeadView.LeftAnchor.ConstraintEqualTo(Hour_TimeHeadView.Superview.LeftAnchor, 0).Active = true;
            Hour_TimeHeadView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lblHours.CenterYAnchor.ConstraintEqualTo(lblHours.Superview.CenterYAnchor, 0).Active = true;
            lblHours.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblHours.LeftAnchor.ConstraintEqualTo(lblHours.Superview.LeftAnchor, 20).Active = true;
            lblHours.WidthAnchor.ConstraintEqualTo(50).Active = true;

            lblSaleHour.CenterYAnchor.ConstraintEqualTo(lblSaleHour.Superview.CenterYAnchor, 0).Active = true;
            lblSaleHour.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSaleHour.RightAnchor.ConstraintEqualTo(lblSaleHour.Superview.RightAnchor, -20).Active = true;
            lblSaleHour.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region HourTimeCollectionView
            HourTimeCollectionView.TopAnchor.ConstraintEqualTo(Hour_TimeHeadView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            HourTimeCollectionView.RightAnchor.ConstraintEqualTo(HourTimeCollectionView.Superview.RightAnchor).Active = true;
            HourTimeCollectionView.LeftAnchor.ConstraintEqualTo(HourTimeCollectionView.Superview.LeftAnchor, 0).Active = true;
            HourTimeCollectionView.BottomAnchor.ConstraintEqualTo(HourTimeCollectionView.Superview.BottomAnchor).Active = true;
            #endregion
            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}