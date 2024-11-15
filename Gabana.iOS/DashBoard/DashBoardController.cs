using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using Microcharts;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using Microcharts.iOS;
using SkiaSharp;
using System.Globalization;
using static Gabana.iOS.BestSellingItemDataSource;

namespace Gabana.iOS
{
    public partial class DashBoardController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;

        UIView TopView;
        UILabel lblTextDay, lblTextDate;

        UIView SaleTotalView;
        UIImageView SaleIcon;
        UILabel lblTextSaleTotal, lblSaleTotal;

        UIView SaleCountView;
        UIImageView SaleCountIcon;
        UILabel lblTextSaleCountTotal, lblSaleCountTotal;

        UIView SaleProfitView;
        UIImageView SaleProfitIcon;
        UILabel lblTextSaleProfitTotal, lblSaleProfitTotal;

        UIView SaleAverageView;
        UIImageView SaleAvgIcon;
        UILabel lblTextSaleAvgTotal, lblSaleAvgTotal;

        UIView SaleComparisonView;
        UIImageView SaleComparisonIcon;
        UILabel lblTextSaleComparisonTotal, lblSaleComparisonTotal;

        UIView SalePeriodGraphView;
        UILabel lblPeriodGraph;

        UIView SaleByTypeView;
        UILabel lblSaleByType;

        UIView BestSellingView;
        UILabel lblBestSelling;
        UICollectionView BestSellingItemCollection;

        UIView BestEmployeeView;
        UILabel lblBestEmployee;
        UICollectionView BestEmpCollection;

        UIImageView EmptyDashboard;
        UILabel lbl_emptyDashboard;

        public static Branch BranchSelect = null;
        UIBarButtonItem selectBranch;
        public static bool isModifyBranch = false;
        BranchManage setBranch = new BranchManage();

        DashBoardBranchController DashBranchPage = null;
        ChartView chartViewSalePeriod, chartViewSalePayment;

        public DashBoardController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("dashboard", "Dashboard"));
                this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                if (isModifyBranch)
                {
                    BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(BranchSelect.SysBranchID));
                    if (BranchSelect.BranchName.Length >= 15)
                    {
                        selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
                    }
                    else
                    {
                        selectBranch.Title = BranchSelect.BranchName;
                    }
                    GetDataDashboard();
                    isModifyBranch = false;
                }
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        public async override void ViewDidLoad()
        {

            try
            {
                base.ViewDidLoad();
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                View.BackgroundColor = UIColor.White;

                BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(Preferences.Get("Branch", "")));

                selectBranch = new UIBarButtonItem();
                if (BranchSelect.BranchName.Length >= 15)
                {
                    selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
                }
                else
                {
                    selectBranch.Title = BranchSelect.BranchName;
                }
                selectBranch.TintColor = UIColor.FromRGB(0, 149, 218);

                selectBranch.Clicked += (sender, e) =>
                {
                    if (DashBranchPage == null)
                    {
                        DashBranchPage = new DashBoardBranchController("dash");
                    }
                    this.NavigationController.PushViewController(DashBranchPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectBranch;

                initAttribute();
                setupAutoLayout();

                GetDataDashboard();

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget((obj, sender) =>
                {
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                _contentView.AddSubview(refreshControl);

                _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        async void GetDataDashboard()
        {
            try
            {
                lblTextDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));


                 var result = await GabanaAPI.GetDataDashboard(Convert.ToInt32(BranchSelect.SysBranchID));
                if (result == null)
                {
                    chartViewSalePayment.Hidden = true;
                    BestSellingItemCollection.Hidden = true;
                    BestEmpCollection.Hidden = true;
                    chartViewSalePeriod.Hidden = true;
                    lblSaleAvgTotal.Text = "฿ 00.00";
                    lblSaleCountTotal.Text = "0";
                    lblSaleProfitTotal.Text = "฿ 00.00";
                    lblSaleTotal.Text = "฿ 00.00";
                    lblSaleComparisonTotal.Text = "0";

                    BestSellingView.Hidden = true;
                    SaleTotalView.Hidden = true;
                    SaleCountView.Hidden = true;
                    SaleProfitView.Hidden = true;
                    SaleAverageView.Hidden = true;
                    SaleComparisonView.Hidden = true;
                    SalePeriodGraphView.Hidden = true;
                    BestEmployeeView.Hidden = true;
                    SaleByTypeView.Hidden = true;

                    EmptyDashboard.Hidden = false;
                    lbl_emptyDashboard.Hidden = false;
                    return;
                }

                BestSellingView.Hidden = false;
                SaleTotalView.Hidden = false;
                SaleCountView.Hidden = false;
                SaleProfitView.Hidden = false;
                SaleAverageView.Hidden = false;
                SaleComparisonView.Hidden = false;
                SalePeriodGraphView.Hidden = false;
                SaleByTypeView.Hidden = false;
                BestEmployeeView.Hidden = false;
                EmptyDashboard.Hidden = true;
                lbl_emptyDashboard.Hidden = true;
                chartViewSalePayment.Hidden = false;
                chartViewSalePeriod.Hidden = false;
                BestSellingItemCollection.Hidden = false;
                BestEmpCollection.Hidden = false;
                lblSaleTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.salesTotal);
                lblSaleAvgTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.averageSale);
                lblSaleCountTotal.Text = result.salesCount.ToString();
                if (result.profit == null || (int)result.profit == 0)
                {
                    lblSaleProfitTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " 00.00";
                }
                else
                {
                    lblSaleProfitTotal.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(result.profit);
                }
                lblSaleComparisonTotal.Text = result.comparison.ToString("#,##0.00") + "%";

                List<Model.ReportHourly> reportHourlies = new List<Model.ReportHourly>();
                int h = 0;
                for (h = 0; h < 24; h++)
                {
                    Model.ReportHourly reportHourly = new Model.ReportHourly()
                    {
                        IdHourly = h + 1,
                        Hourlyname = h.ToString("D2") + ":00",
                        value = 0
                    };

                    reportHourlies.Add(reportHourly);
                }

                foreach (var item in reportHourlies)
                {
                    item.value = result.salesByPeriods.Where(x => x.hourly == item.IdHourly).Sum(x => x.salesPeriodTotal);
                }


                List<ChartEntry> SalesByPeriods = new List<ChartEntry>();

                var firstIndex = reportHourlies.FindIndex(x => x.value > 0) - 1;
                var lastIndex = reportHourlies.FindLastIndex(x => x.value > 0) + 2;

                if (firstIndex < 0) firstIndex = 0;
                if (lastIndex > reportHourlies.Count) lastIndex = reportHourlies.Count;

                var dif = Math.Abs(lastIndex - firstIndex);
                if (dif > 8)
                {
                    firstIndex = reportHourlies.FindIndex(x => x.value > 0);
                    lastIndex = reportHourlies.FindLastIndex(x => x.value > 0);
                    if (firstIndex % 3 != 0) firstIndex = firstIndex - (firstIndex % 3);
                    if (lastIndex % 3 != 0) lastIndex = lastIndex + (3 - (lastIndex % 3));

                    foreach (var item in reportHourlies)
                    {
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Hourlyname.ToString(),
                            ValueLabel = item.value.ToString("#,###"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };

                        ChartEntry entry2 = new ChartEntry((float)item.value)
                        {
                            Label = "",
                            TextColor = SKColor.Parse("#fff"),
                            ValueLabel = item.value.ToString("#,###"),
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
                    for (int a = firstIndex; a < lastIndex; a++)
                    {
                        var item = reportHourlies[a];
                        ChartEntry entry = new ChartEntry((float)item.value)
                        {
                            Label = item.Hourlyname.ToString(),
                            ValueLabel = item.value.ToString("#,###.##"),
                            Color = SKColor.Parse("#0095DA"),
                            ValueLabelColor = SKColor.Parse("#0095DA")
                        };
                        SalesByPeriods.Add(entry);
                    }

                }


                //foreach (var item in reportHourlies)
                //{
                //    Entry entry = new Entry((float)item.value)
                //    {
                //        Label = item.IdHourly.ToString(),
                //        ValueLabel = Utils.DisplayDecimal(item.value),
                //        Color = SKColor.Parse("#0095DA")
                //    };

                //    SalesByPeriods.Add(entry);
                //}
                
                var chart = new LineChart() 
                {
                    Entries = SalesByPeriods,
                    LabelTextSize = 16f,
                    BackgroundColor = SKColor.Parse("#FFF"),
                    LineMode = LineMode.Straight,

                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Vertical,
                    IsAnimated = false

                };

                chart.MinValue = 0;
                chartViewSalePeriod.Chart = chart;

                List<string> color = new List<string>();
                color.Add("#0095DA");
                color.Add("#E32D49");
                color.Add("#37AA52");
                color.Add("#F8971D");
                List<ChartEntry> SalesByPayments = new List<ChartEntry>();
                int i = 0;
                foreach (var item in result.salesByPayments)
                {
                    ChartEntry entry = new ChartEntry((float)item.percentTotal)
                    {
                        Label = Utils.SetPaymentName(item.paymentType.ToUpper().ToString()),
                        ValueLabel = item.percentTotal.ToString("##0.00") + " % ",
                        Color = SKColor.Parse(color[i]),
                        ValueLabelColor = SKColor.Parse(color[i])
                    };
                    i++;
                    if (i >= color.Count) i = 0;
                    SalesByPayments.Add(entry);
                }
                var chart2 = new DonutChart() { Entries = SalesByPayments, LabelTextSize = 20, HoleRadius = 0.7f };
                chartViewSalePayment.Chart = chart2;

                List<Item> items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();

                ListBestSallItem listBestSallIteem = new ListBestSallItem(result.bestSellingItems);
                BestSellingItemDataSource iTemDataList = new BestSellingItemDataSource(items, listBestSallIteem); // ส่ง list ไป
                BestSellingItemCollection.DataSource = iTemDataList;
                BestSellingItemCollection.ReloadData();

                if (result.bestSellingItems.Count < 5)
                {
                    Utils.SetConstant(BestSellingView.Constraints, NSLayoutAttribute.Height, (result.bestSellingItems.Count * 70) + 50);
                    BestSellingView.LayoutIfNeeded();
                }
                else
                {
                    Utils.SetConstant(BestSellingView.Constraints, NSLayoutAttribute.Height, (5 * 70) + 50);
                    BestSellingView.LayoutIfNeeded();
                }

                #region BestEmployeeView
                result.bestEmployees = result.bestEmployees.OrderByDescending(x=>x.totalAmount).ToList();
                decimal maxvalue = result.bestEmployees.Sum(x => x.totalAmount);
                BestEmployeeDataSource employeeDataList = new BestEmployeeDataSource(result.bestEmployees, maxvalue);
                BestEmpCollection.DataSource = employeeDataList;
                BestEmpCollection.ReloadData();

                if (result.bestEmployees.Count < 5)
                {
                    Utils.SetConstant(BestEmployeeView.Constraints, NSLayoutAttribute.Height, (result.bestEmployees.Count * 70) + 50);
                    BestEmployeeView.LayoutIfNeeded();
                }
                else
                {
                    Utils.SetConstant(BestEmployeeView.Constraints, NSLayoutAttribute.Height, (5 * 70) + 50);
                    BestEmployeeView.LayoutIfNeeded();
                }
                #endregion
                //List<Entry> BestEmp = new List<Entry>();
                //decimal sumAllAmount = 0;
                //foreach (var item in result.bestEmployees)
                //{
                //    Entry entry = new Entry((float)item.totalAmount)
                //    {
                //        Label = item.employeeName.ToString(),
                //        ValueLabel = Utils.DisplayDecimal(item.totalAmount),
                //        Color = SKColor.Parse("#0095DA")
                //    };
                //    BestEmp.Add(entry);
                //    sumAllAmount += item.totalAmount;
                //}



            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"),Utils.TextBundle("cannotload", "Unable to load data."));
            }

        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region TopView
            TopView = new UIView();
            TopView.BackgroundColor = UIColor.White;
            TopView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(TopView);

            lblTextDay = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextDay.Font = lblTextDay.Font.WithSize(15);
            lblTextDay.Text = "Today, ";
            TopView.AddSubview(lblTextDay);

            lblTextDate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextDate.Font = lblTextDate.Font.WithSize(15);
            lblTextDate.Text = "DD MMM YYYY";
            TopView.AddSubview(lblTextDate);

            #endregion

            #region EmptyDashBoard
            EmptyDashboard = new UIImageView();
            EmptyDashboard.BackgroundColor = UIColor.Clear;
            EmptyDashboard.Hidden = true;
            EmptyDashboard.Image = UIImage.FromFile("DefaultDashbard.png"); // wait for calender icon
            EmptyDashboard.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(EmptyDashboard);

            lbl_emptyDashboard = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162, 162, 162),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lbl_emptyDashboard.Font = lbl_emptyDashboard.Font.WithSize(20);
            lbl_emptyDashboard.Hidden = true;
            lbl_emptyDashboard.Text = Utils.TextBundle("nothavedatasell", "You don't have any sales information yet.");
            _contentView.AddSubview(lbl_emptyDashboard);

            #endregion
            #region SaleTotalView
            SaleTotalView = new UIView();
            SaleTotalView.Layer.CornerRadius = 5;
            SaleTotalView.Hidden = true;
            SaleTotalView.ClipsToBounds = true;
            SaleTotalView.BackgroundColor = UIColor.White;
            SaleTotalView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(20);
            lblSaleTotal.Text = "฿ 000.00";
            SaleTotalView.AddSubview(lblSaleTotal);

            lblTextSaleTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleTotal.Font = lblTextSaleTotal.Font.WithSize(13);
            lblTextSaleTotal.Text = Utils.TextBundle("salestotal", "Sales Total");
            SaleTotalView.AddSubview(lblTextSaleTotal);

            SaleIcon = new UIImageView();
            SaleIcon.BackgroundColor = UIColor.White;
            SaleIcon.Image = UIImage.FromBundle("DbSales"); // wait for calender icon
            SaleIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleTotalView.AddSubview(SaleIcon);

            #endregion
            #region SaleCountView
            SaleCountView = new UIView();
            SaleCountView.BackgroundColor = UIColor.White;
            SaleCountView.Layer.CornerRadius = 5;
            SaleCountView.Hidden = true;
            SaleCountView.ClipsToBounds = true;
            SaleCountView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleCountTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleCountTotal.Font = lblSaleCountTotal.Font.WithSize(15);
            lblSaleCountTotal.Text = "0";
            SaleCountView.AddSubview(lblSaleCountTotal);

            lblTextSaleCountTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleCountTotal.Font = lblTextSaleCountTotal.Font.WithSize(12);
            lblTextSaleCountTotal.Text = Utils.TextBundle("salescount", "Sales Count");
            SaleCountView.AddSubview(lblTextSaleCountTotal);

            SaleCountIcon = new UIImageView();
            SaleCountIcon.BackgroundColor = UIColor.White;
            SaleCountIcon.Image = UIImage.FromBundle("DbCount"); // wait for calender icon
            SaleCountIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleCountView.AddSubview(SaleCountIcon);
            #endregion
            #region SaleProfitView
            SaleProfitView = new UIView();
            SaleProfitView.Layer.CornerRadius = 5;
            SaleProfitView.ClipsToBounds = true;
            SaleProfitView.Hidden = true;
            SaleProfitView.BackgroundColor = UIColor.White;
            SaleProfitView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleProfitTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleProfitTotal.Font = lblSaleProfitTotal.Font.WithSize(15);
            lblSaleProfitTotal.Text = "฿ 00.00";
            SaleProfitView.AddSubview(lblSaleProfitTotal);

            lblTextSaleProfitTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleProfitTotal.Font = lblTextSaleProfitTotal.Font.WithSize(12);
            lblTextSaleProfitTotal.Text = Utils.TextBundle("profit", "Profit");
            SaleProfitView.AddSubview(lblTextSaleProfitTotal);

            SaleProfitIcon = new UIImageView();
            SaleProfitIcon.BackgroundColor = UIColor.White;
            SaleProfitIcon.Image = UIImage.FromBundle("DbProfit"); // wait for calender icon
            SaleProfitIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleProfitView.AddSubview(SaleProfitIcon);
            #endregion
            #region SaleAverageView
            SaleAverageView = new UIView();
            SaleAverageView.Layer.CornerRadius = 5;
            SaleAverageView.ClipsToBounds = true;
            SaleAverageView.Hidden = true;
            SaleAverageView.BackgroundColor = UIColor.White;
            SaleAverageView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleAvgTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleAvgTotal.Font = lblSaleAvgTotal.Font.WithSize(15);
            lblSaleAvgTotal.Text = "฿ 00.00";
            SaleAverageView.AddSubview(lblSaleAvgTotal);

            lblTextSaleAvgTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleAvgTotal.Font = lblTextSaleAvgTotal.Font.WithSize(12);
            lblTextSaleAvgTotal.Text = Utils.TextBundle("averagesale", "Average Sale");
            SaleAverageView.AddSubview(lblTextSaleAvgTotal);

            SaleAvgIcon = new UIImageView();
            SaleAvgIcon.BackgroundColor = UIColor.White;
            SaleAvgIcon.Image = UIImage.FromBundle("DbAvg"); // wait for calender icon
            SaleAvgIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleAverageView.AddSubview(SaleAvgIcon);
            #endregion
            #region SaleComparisonView
            SaleComparisonView = new UIView();
            SaleComparisonView.Layer.CornerRadius = 5;
            SaleComparisonView.ClipsToBounds = true;
            SaleComparisonView.Hidden = true;
            SaleComparisonView.BackgroundColor = UIColor.White;
            SaleComparisonView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleComparisonTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleComparisonTotal.Font = lblSaleComparisonTotal.Font.WithSize(15);
            lblSaleComparisonTotal.Text = "฿ 00.00";
            SaleComparisonView.AddSubview(lblSaleComparisonTotal);

            lblTextSaleComparisonTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleComparisonTotal.Font = lblTextSaleComparisonTotal.Font.WithSize(12);
            lblTextSaleComparisonTotal.Text = Utils.TextBundle("comparison", "Comparison");
            SaleComparisonView.AddSubview(lblTextSaleComparisonTotal);

            SaleComparisonIcon = new UIImageView();
            SaleComparisonIcon.BackgroundColor = UIColor.White;
            SaleComparisonIcon.Image = UIImage.FromBundle("DbCompare"); // wait for calender icon
            SaleComparisonIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleComparisonView.AddSubview(SaleComparisonIcon);
            #endregion
            #region SalePeriodGraphView
            SalePeriodGraphView = new UIView();
            SalePeriodGraphView.Hidden = true;
            SalePeriodGraphView.BackgroundColor = UIColor.White;
            SalePeriodGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblPeriodGraph = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblPeriodGraph.Font = lblPeriodGraph.Font.WithSize(15);
            lblPeriodGraph.Text = Utils.TextBundle("salesbyperiod", "Sales By Period");
            SalePeriodGraphView.AddSubview(lblPeriodGraph);

            chartViewSalePeriod = new ChartView();
            chartViewSalePeriod.TranslatesAutoresizingMaskIntoConstraints = false;
            chartViewSalePeriod.BackgroundColor = UIColor.White;
            SalePeriodGraphView.AddSubview(chartViewSalePeriod);
            #endregion

            #region SaleByTypeView
            SaleByTypeView = new UIView();
            SaleByTypeView.Hidden = true;
            SaleByTypeView.BackgroundColor = UIColor.White;
            SaleByTypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleByType = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByType.Font = lblSaleByType.Font.WithSize(15);
            lblSaleByType.Text = Utils.TextBundle("salesbypaymenttype", "Sales By Payment Type");
            SaleByTypeView.AddSubview(lblSaleByType);

            chartViewSalePayment = new ChartView();
            chartViewSalePayment.TranslatesAutoresizingMaskIntoConstraints = false;
            chartViewSalePayment.BackgroundColor = UIColor.White;
            SaleByTypeView.AddSubview(chartViewSalePayment);
            #endregion

            #region BestSellingView
            BestSellingView = new UIView();
            BestSellingView.Hidden = true;
            BestSellingView.BackgroundColor = UIColor.White;
            BestSellingView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblBestSelling = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBestSelling.Font = lblBestSelling.Font.WithSize(15);
            lblBestSelling.Text = Utils.TextBundle("bestsellingitems", "Best Selling Items");
            BestSellingView.AddSubview(lblBestSelling);

            // BestSellingItemCollection
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            BestSellingItemCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            BestSellingItemCollection.BackgroundColor = UIColor.White;
            BestSellingItemCollection.ShowsVerticalScrollIndicator = false;
            BestSellingItemCollection.ScrollEnabled = false;
            BestSellingItemCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            BestSellingItemCollection.RegisterClassForCell(cellType: typeof(BestSellingItemViewCell), reuseIdentifier: "BestSellingItemViewCell");
            BestSellingView.AddSubview(BestSellingItemCollection);

            #endregion

            #region NewBestEmployeeView
            BestEmployeeView = new UIView();
            BestEmployeeView.Hidden = true;
            BestEmployeeView.BackgroundColor = UIColor.White;
            BestEmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblBestEmployee = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBestEmployee.Font = lblBestEmployee.Font.WithSize(15);
            lblBestEmployee.Text = Utils.TextBundle("bestemployee", "Best Employee");
            BestEmployeeView.AddSubview(lblBestEmployee);

            // BestEmployeeCollection
            UICollectionViewFlowLayout empflowLayoutList = new UICollectionViewFlowLayout();
            empflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 60);
            empflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;


            BestEmpCollection = new UICollectionView(frame: View.Frame, layout: empflowLayoutList);
            BestEmpCollection.BackgroundColor = UIColor.White;
            BestEmpCollection.ShowsVerticalScrollIndicator = false;
            BestEmpCollection.ScrollEnabled = false;
            BestEmpCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            BestEmpCollection.RegisterClassForCell(cellType: typeof(BestEmployeeViewCell), reuseIdentifier: "BestEmployeeViewCell");
            BestEmployeeView.AddSubview(BestEmpCollection);
            #endregion

            #region BestEmployeeView
            //BestEmployeeView = new UIView();
            //BestEmployeeView.Hidden = true;
            //BestEmployeeView.BackgroundColor = UIColor.White;
            //BestEmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;

            //lblBestEmployee = new UILabel
            //{
            //    TextAlignment = UITextAlignment.Left,
            //    TextColor = UIColor.FromRGB(247, 86, 0),

            //    TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            //};
            //lblBestEmployee.Font = lblBestEmployee.Font.WithSize(15);
            //lblBestEmployee.Text = "Best Employee";
            //BestEmployeeView.AddSubview(lblBestEmployee);

            //chartView3 = new ChartView();
            //chartView3.TranslatesAutoresizingMaskIntoConstraints = false;
            //chartView3.BackgroundColor = UIColor.White;
            //BestEmployeeView.AddSubview(chartView3);
            #endregion


            _contentView.AddSubview(SaleTotalView);
            _contentView.AddSubview(SaleCountView);
            _contentView.AddSubview(SaleProfitView);
            _contentView.AddSubview(SaleAverageView);
            _contentView.AddSubview(SaleComparisonView);
            _contentView.AddSubview(SalePeriodGraphView);
            _contentView.AddSubview(SaleByTypeView);
            _contentView.AddSubview(BestSellingView);
            _contentView.AddSubview(BestEmployeeView);


            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
        }
        void setupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(TopView.BottomAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region TopView
            TopView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblTextDay.CenterYAnchor.ConstraintEqualTo(lblTextDay.Superview.CenterYAnchor, 0).Active = true;
            lblTextDay.LeftAnchor.ConstraintEqualTo(lblTextDay.Superview.LeftAnchor, 20).Active = true;
            lblTextDay.HeightAnchor.ConstraintEqualTo(80).Active = true;

            lblTextDate.CenterYAnchor.ConstraintEqualTo(lblTextDate.Superview.CenterYAnchor, 0).Active = true;
            lblTextDate.LeftAnchor.ConstraintEqualTo(lblTextDay.SafeAreaLayoutGuide.RightAnchor, 6).Active = true;
            lblTextDate.HeightAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region EmptyDashboard
            EmptyDashboard.TopAnchor.ConstraintEqualTo(EmptyDashboard.Superview.TopAnchor, 38).Active = true;
            EmptyDashboard.HeightAnchor.ConstraintEqualTo(175).Active = true;
            EmptyDashboard.WidthAnchor.ConstraintEqualTo(300).Active = true;
            EmptyDashboard.CenterXAnchor.ConstraintEqualTo(EmptyDashboard.Superview.CenterXAnchor).Active = true;

            lbl_emptyDashboard.TopAnchor.ConstraintEqualTo(EmptyDashboard.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_emptyDashboard.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_emptyDashboard.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_emptyDashboard.CenterXAnchor.ConstraintEqualTo(EmptyDashboard.Superview.CenterXAnchor).Active = true;
            #endregion

            #region SaleTotalView
            SaleTotalView.TopAnchor.ConstraintEqualTo(SaleTotalView.Superview.TopAnchor, 10).Active = true;
            SaleTotalView.LeftAnchor.ConstraintEqualTo(SaleTotalView.Superview.LeftAnchor, 10).Active = true;
            SaleTotalView.RightAnchor.ConstraintEqualTo(SaleTotalView.Superview.RightAnchor, -10).Active = true;
            SaleTotalView.HeightAnchor.ConstraintEqualTo(75).Active = true;

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(lblSaleTotal.Superview.CenterYAnchor, -6).Active = true;
            lblSaleTotal.LeftAnchor.ConstraintEqualTo(SaleIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(lblSaleTotal.Superview.RightAnchor, -15).Active = true;
            lblSaleTotal.HeightAnchor.ConstraintEqualTo(24).Active = true;

            lblTextSaleTotal.TopAnchor.ConstraintEqualTo(lblSaleTotal.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            lblTextSaleTotal.LeftAnchor.ConstraintEqualTo(SaleIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTextSaleTotal.RightAnchor.ConstraintEqualTo(lblTextSaleTotal.Superview.RightAnchor, -15).Active = true;
            lblTextSaleTotal.HeightAnchor.ConstraintEqualTo(15).Active = true;

            SaleIcon.CenterYAnchor.ConstraintEqualTo(SaleIcon.Superview.CenterYAnchor, 0).Active = true;
            SaleIcon.LeftAnchor.ConstraintEqualTo(SaleIcon.Superview.LeftAnchor, 15).Active = true;
            SaleIcon.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SaleIcon.WidthAnchor.ConstraintEqualTo(40).Active = true;

            #endregion

            #region SaleCountView
            SaleCountView.TopAnchor.ConstraintEqualTo(SaleTotalView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleCountView.LeftAnchor.ConstraintEqualTo(SaleCountView.Superview.LeftAnchor, 10).Active = true;
            SaleCountView.RightAnchor.ConstraintEqualTo(SaleCountView.Superview.CenterXAnchor, -5).Active = true;
            SaleCountView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSaleCountTotal.CenterYAnchor.ConstraintEqualTo(lblSaleCountTotal.Superview.CenterYAnchor, -6).Active = true;
            lblSaleCountTotal.LeftAnchor.ConstraintEqualTo(SaleCountIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSaleCountTotal.RightAnchor.ConstraintEqualTo(lblSaleCountTotal.Superview.RightAnchor, -15).Active = true;
            lblSaleCountTotal.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblTextSaleCountTotal.TopAnchor.ConstraintEqualTo(lblSaleCountTotal.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblTextSaleCountTotal.LeftAnchor.ConstraintEqualTo(SaleCountIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTextSaleCountTotal.RightAnchor.ConstraintEqualTo(lblTextSaleCountTotal.Superview.RightAnchor, -15).Active = true;
            lblTextSaleCountTotal.HeightAnchor.ConstraintEqualTo(14).Active = true;

            SaleCountIcon.CenterYAnchor.ConstraintEqualTo(SaleCountIcon.Superview.CenterYAnchor, 0).Active = true;
            SaleCountIcon.LeftAnchor.ConstraintEqualTo(SaleCountIcon.Superview.LeftAnchor, 15).Active = true;
            SaleCountIcon.HeightAnchor.ConstraintEqualTo(32).Active = true;
            SaleCountIcon.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region SaleProfitView
            SaleProfitView.TopAnchor.ConstraintEqualTo(SaleTotalView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleProfitView.LeftAnchor.ConstraintEqualTo(SaleProfitView.Superview.CenterXAnchor, 5).Active = true;
            SaleProfitView.RightAnchor.ConstraintEqualTo(SaleProfitView.Superview.RightAnchor, -10).Active = true;
            SaleProfitView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSaleProfitTotal.CenterYAnchor.ConstraintEqualTo(lblSaleProfitTotal.Superview.CenterYAnchor, -6).Active = true;
            lblSaleProfitTotal.LeftAnchor.ConstraintEqualTo(SaleProfitIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSaleProfitTotal.RightAnchor.ConstraintEqualTo(lblSaleProfitTotal.Superview.RightAnchor, -15).Active = true;
            lblSaleProfitTotal.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblTextSaleProfitTotal.TopAnchor.ConstraintEqualTo(lblSaleProfitTotal.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblTextSaleProfitTotal.LeftAnchor.ConstraintEqualTo(SaleProfitIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTextSaleProfitTotal.RightAnchor.ConstraintEqualTo(lblTextSaleProfitTotal.Superview.RightAnchor, -15).Active = true;
            lblTextSaleProfitTotal.HeightAnchor.ConstraintEqualTo(14).Active = true;

            SaleProfitIcon.CenterYAnchor.ConstraintEqualTo(SaleProfitIcon.Superview.CenterYAnchor, 0).Active = true;
            SaleProfitIcon.LeftAnchor.ConstraintEqualTo(SaleProfitIcon.Superview.LeftAnchor, 15).Active = true;
            SaleProfitIcon.HeightAnchor.ConstraintEqualTo(32).Active = true;
            SaleProfitIcon.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region SaleAverageView
            SaleAverageView.TopAnchor.ConstraintEqualTo(SaleCountView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleAverageView.LeftAnchor.ConstraintEqualTo(SaleAverageView.Superview.LeftAnchor, 10).Active = true;
            SaleAverageView.RightAnchor.ConstraintEqualTo(SaleAverageView.Superview.CenterXAnchor, -5).Active = true;
            SaleAverageView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSaleAvgTotal.CenterYAnchor.ConstraintEqualTo(lblSaleAvgTotal.Superview.CenterYAnchor, -6).Active = true;
            lblSaleAvgTotal.LeftAnchor.ConstraintEqualTo(SaleAvgIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSaleAvgTotal.RightAnchor.ConstraintEqualTo(lblSaleAvgTotal.Superview.RightAnchor, -15).Active = true;
            lblSaleAvgTotal.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblTextSaleAvgTotal.TopAnchor.ConstraintEqualTo(lblSaleAvgTotal.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblTextSaleAvgTotal.LeftAnchor.ConstraintEqualTo(SaleAvgIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTextSaleAvgTotal.RightAnchor.ConstraintEqualTo(lblTextSaleAvgTotal.Superview.RightAnchor, -15).Active = true;
            lblTextSaleAvgTotal.HeightAnchor.ConstraintEqualTo(14).Active = true;

            SaleAvgIcon.CenterYAnchor.ConstraintEqualTo(SaleAvgIcon.Superview.CenterYAnchor, 0).Active = true;
            SaleAvgIcon.LeftAnchor.ConstraintEqualTo(SaleAvgIcon.Superview.LeftAnchor, 15).Active = true;
            SaleAvgIcon.HeightAnchor.ConstraintEqualTo(32).Active = true;
            SaleAvgIcon.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region SaleComparisonView
            SaleComparisonView.TopAnchor.ConstraintEqualTo(SaleProfitView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleComparisonView.LeftAnchor.ConstraintEqualTo(SaleComparisonView.Superview.CenterXAnchor, 5).Active = true;
            SaleComparisonView.RightAnchor.ConstraintEqualTo(SaleComparisonView.Superview.RightAnchor, -10).Active = true;
            SaleComparisonView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSaleComparisonTotal.CenterYAnchor.ConstraintEqualTo(lblSaleComparisonTotal.Superview.CenterYAnchor, -6).Active = true;
            lblSaleComparisonTotal.LeftAnchor.ConstraintEqualTo(SaleComparisonIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSaleComparisonTotal.RightAnchor.ConstraintEqualTo(lblSaleComparisonTotal.Superview.RightAnchor, -15).Active = true;
            lblSaleComparisonTotal.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblTextSaleComparisonTotal.TopAnchor.ConstraintEqualTo(lblSaleComparisonTotal.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblTextSaleComparisonTotal.LeftAnchor.ConstraintEqualTo(SaleComparisonIcon.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTextSaleComparisonTotal.RightAnchor.ConstraintEqualTo(lblTextSaleComparisonTotal.Superview.RightAnchor, -15).Active = true;
            lblTextSaleComparisonTotal.HeightAnchor.ConstraintEqualTo(14).Active = true;

            SaleComparisonIcon.CenterYAnchor.ConstraintEqualTo(SaleComparisonIcon.Superview.CenterYAnchor, 0).Active = true;
            SaleComparisonIcon.LeftAnchor.ConstraintEqualTo(SaleComparisonIcon.Superview.LeftAnchor, 15).Active = true;
            SaleComparisonIcon.HeightAnchor.ConstraintEqualTo(32).Active = true;
            SaleComparisonIcon.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region SalePeriodGraphView
            SalePeriodGraphView.TopAnchor.ConstraintEqualTo(SaleComparisonView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SalePeriodGraphView.LeftAnchor.ConstraintEqualTo(SalePeriodGraphView.Superview.LeftAnchor, 0).Active = true;
            SalePeriodGraphView.RightAnchor.ConstraintEqualTo(SalePeriodGraphView.Superview.RightAnchor, 0).Active = true;
            SalePeriodGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;

            lblPeriodGraph.TopAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.TopAnchor, 12).Active = true;
            lblPeriodGraph.LeftAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.LeftAnchor, 15).Active = true;
            lblPeriodGraph.RightAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.RightAnchor, -15).Active = true;
            lblPeriodGraph.HeightAnchor.ConstraintEqualTo(18).Active = true;

            chartViewSalePeriod.TopAnchor.ConstraintEqualTo(lblPeriodGraph.SafeAreaLayoutGuide.BottomAnchor, 21).Active = true;
            chartViewSalePeriod.CenterXAnchor.ConstraintEqualTo(chartViewSalePeriod.Superview.CenterXAnchor).Active = true;
            chartViewSalePeriod.LeftAnchor.ConstraintEqualTo(chartViewSalePeriod.Superview.LeftAnchor, 15).Active = true;
            chartViewSalePeriod.RightAnchor.ConstraintEqualTo(chartViewSalePeriod.Superview.RightAnchor, -15).Active = true;
            chartViewSalePeriod.HeightAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region SaleByTypeView
            SaleByTypeView.TopAnchor.ConstraintEqualTo(SalePeriodGraphView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleByTypeView.LeftAnchor.ConstraintEqualTo(SaleByTypeView.Superview.LeftAnchor, 0).Active = true;
            SaleByTypeView.RightAnchor.ConstraintEqualTo(SaleByTypeView.Superview.RightAnchor, 0).Active = true;
            SaleByTypeView.HeightAnchor.ConstraintEqualTo(210).Active = true;

            lblSaleByType.TopAnchor.ConstraintEqualTo(lblSaleByType.Superview.TopAnchor, 12).Active = true;
            lblSaleByType.LeftAnchor.ConstraintEqualTo(lblSaleByType.Superview.LeftAnchor, 15).Active = true;
            lblSaleByType.RightAnchor.ConstraintEqualTo(lblSaleByType.Superview.RightAnchor, -15).Active = true;
            lblSaleByType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            chartViewSalePayment.RightAnchor.ConstraintEqualTo(chartViewSalePayment.Superview.RightAnchor, -15).Active = true;
            chartViewSalePayment.TopAnchor.ConstraintEqualTo(lblSaleByType.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            chartViewSalePayment.LeftAnchor.ConstraintEqualTo(chartViewSalePayment.Superview.LeftAnchor, 15).Active = true;
            chartViewSalePayment.HeightAnchor.ConstraintEqualTo(170).Active = true;
            #endregion

            #region BestSellingView
            BestSellingView.TopAnchor.ConstraintEqualTo(SaleByTypeView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            BestSellingView.LeftAnchor.ConstraintEqualTo(SaleByTypeView.Superview.LeftAnchor, 0).Active = true;
            BestSellingView.RightAnchor.ConstraintEqualTo(SaleByTypeView.Superview.RightAnchor, 0).Active = true;
            BestSellingView.HeightAnchor.ConstraintEqualTo(0).Active = true;

            lblBestSelling.TopAnchor.ConstraintEqualTo(lblBestSelling.Superview.TopAnchor, 12).Active = true;
            lblBestSelling.LeftAnchor.ConstraintEqualTo(lblBestSelling.Superview.LeftAnchor, 15).Active = true;
            lblBestSelling.RightAnchor.ConstraintEqualTo(lblBestSelling.Superview.RightAnchor, -15).Active = true;
            lblBestSelling.HeightAnchor.ConstraintEqualTo(18).Active = true;


            BestSellingItemCollection.TopAnchor.ConstraintEqualTo(lblBestSelling.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            BestSellingItemCollection.LeftAnchor.ConstraintEqualTo(BestSellingItemCollection.Superview.LeftAnchor, 15).Active = true;
            BestSellingItemCollection.RightAnchor.ConstraintEqualTo(BestSellingItemCollection.Superview.RightAnchor, -15).Active = true;
            BestSellingItemCollection.BottomAnchor.ConstraintEqualTo(BestSellingItemCollection.Superview.BottomAnchor, -10).Active = true;
            #endregion

            #region BestEmployeeView
            BestEmployeeView.TopAnchor.ConstraintEqualTo(BestSellingView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            BestEmployeeView.LeftAnchor.ConstraintEqualTo(BestSellingView.Superview.LeftAnchor, 0).Active = true;
            BestEmployeeView.RightAnchor.ConstraintEqualTo(BestSellingView.Superview.RightAnchor, 0).Active = true;
            BestEmployeeView.BottomAnchor.ConstraintEqualTo(BestEmployeeView.Superview.BottomAnchor, -10).Active = true;
            BestEmployeeView.HeightAnchor.ConstraintEqualTo(0).Active = true;

            lblBestEmployee.TopAnchor.ConstraintEqualTo(lblBestEmployee.Superview.TopAnchor, 12).Active = true;
            lblBestEmployee.LeftAnchor.ConstraintEqualTo(lblBestEmployee.Superview.LeftAnchor, 15).Active = true;
            lblBestEmployee.RightAnchor.ConstraintEqualTo(lblBestEmployee.Superview.RightAnchor, -15).Active = true;
            lblBestEmployee.HeightAnchor.ConstraintEqualTo(18).Active = true;

            BestEmpCollection.TopAnchor.ConstraintEqualTo(lblBestEmployee.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            BestEmpCollection.LeftAnchor.ConstraintEqualTo(BestEmpCollection.Superview.LeftAnchor, 15).Active = true;
            BestEmpCollection.RightAnchor.ConstraintEqualTo(BestEmpCollection.Superview.RightAnchor, -15).Active = true;
            BestEmpCollection.BottomAnchor.ConstraintEqualTo(BestEmpCollection.Superview.BottomAnchor, -10).Active = true;
            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}