using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.Period;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Microcharts;
using Microcharts.iOS;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{

    public partial class SaleReportMonthController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        UICollectionView HourTimeCollectionView, DaillyTimeCollectionView, WeeklyTimeCollectionView;

        UIView Hour_timeView, Hour_TimeHeadView;
        UILabel lblHours, lblSaleHour;

        UIView Daily_timeView, Daily_TimeHeadView;
        UILabel lblDaily, lblSaleDaily;

        UIView Weekly_timeView, Weekly_TimeHeadView;
        UILabel lblWeekly, lblSaleWeekly;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch;

        UIView MenuView,HourView,DailyView,WeeklyView;
        UILabel lblHourMenu, lblDailyMenu, lblWeeklyMenu;
        UIView HourLineView, DailyLineView, WeeklyLineView;


        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView HourlyGraphView, DailyGraphView, WeeklyGraphView;
        UILabel lbl_GraphHead_Hour, lbl_GraphHead_Daily, lbl_GraphHead_Weekly;
        ChartView SaleHourChartView, SaleDailyChartView, SaleWeeklyChartView;
        List<int> lstsysBranchID = new List<int>();
        bool typCustomBydate = false;
        string StartDate ;
        string DateSet;
        string Enddate ;
        private string filename;
        private ReportSale reportSale;
        private List<ReportHourly> report;
        private string Typetime;
        private UIView Viewnull;
        private UIImageView imgnull;
        private UILabel lblnull;
        List<SummaryHourly> GetDataReportMonth;
        private UIView totalView;
        private UILabel lbl_totalHead;
        private UILabel lbl_total;
        private string ltotalhour;
        private string ltotaldaily;
        private string ltotalWeekly;
        private UIView SearchbarView;
        private UITextField txtSearch;
        private UIButton btnSearch;
        private UIButton btnfilter;
        internal static int filterReport;
        internal static bool isModifyFilter;
        private List<ReportDaily> report2;
        private List<ReportWeekly> report3;
        private string datenamefile;

        public SaleReportMonthController(bool custom,string dateSet, string start, string end,string filename)
        {
            this.typCustomBydate = custom;
            this.StartDate = start;
            this.DateSet = dateSet;
            this.Enddate = end;
            this.filename = filename;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("sale_report", "Items"));
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
            lblHourMenu.TextColor = UIColor.FromRGB(0,149,218);
            HourLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblDailyMenu.TextColor = UIColor.FromRGB(64,64,64);
            DailyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = false;
            DailyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = false;
            Daily_timeView.Hidden = true;
            Weekly_timeView.Hidden = true;
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
            lblDay.Font = lblDay.Font.WithSize(14);
            TopView.AddSubview(lblDay);

            lblDate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0,149,218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDate.Font = lblDate.Font.WithSize(12);
            TopView.AddSubview(lblDate);

            lblBranch = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112,112,112),
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
                TextColor = UIColor.FromRGB(0,149,218),
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

            #region DailyView
            DailyView = new UIView();
            DailyView.BackgroundColor = UIColor.White;
            DailyView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuView.AddSubview(DailyView);

            lblDailyMenu = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDailyMenu.Text = Utils.TextBundle("daily", "Items");
            lblDailyMenu.Font = lblDailyMenu.Font.WithSize(15);
            DailyView.AddSubview(lblDailyMenu);

            DailyLineView = new UIView();
            DailyLineView.BackgroundColor = UIColor.White;
            DailyLineView.TranslatesAutoresizingMaskIntoConstraints = false;
            DailyView.AddSubview(DailyLineView);

            DailyView.UserInteractionEnabled = true;
            var tapGesturebtnDaily = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Daily:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DailyView.AddGestureRecognizer(tapGesturebtnDaily);
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
            #endregion

            #region HourlyGraphView
            HourlyGraphView = new UIView();
            HourlyGraphView.BackgroundColor = UIColor.White;
            HourlyGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead_Hour = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247,86,0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead_Hour.Text = Utils.TextBundle("salesbyhour", "Items");
            lbl_GraphHead_Hour.Font = lbl_GraphHead_Hour.Font.WithSize(15);
            HourlyGraphView.AddSubview(lbl_GraphHead_Hour);

            SaleHourChartView = new ChartView();
            SaleHourChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleHourChartView.BackgroundColor = UIColor.White;
            HourlyGraphView.AddSubview(SaleHourChartView);
            #endregion

            #region DailyGraphView
            DailyGraphView = new UIView();
            DailyGraphView.BackgroundColor = UIColor.White;
            DailyGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead_Daily = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead_Daily.Text = Utils.TextBundle("salesbydate", "Items");
            lbl_GraphHead_Daily.Font = lbl_GraphHead_Daily.Font.WithSize(15);
            DailyGraphView.AddSubview(lbl_GraphHead_Daily);

            SaleDailyChartView = new ChartView();
            SaleDailyChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleDailyChartView.BackgroundColor = UIColor.White;
            DailyGraphView.AddSubview(SaleDailyChartView);
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
            lbl_GraphHead_Weekly.Text = Utils.TextBundle("salesbyweek", "Items");
            lbl_GraphHead_Weekly.Font = lbl_GraphHead_Weekly.Font.WithSize(15);
            WeeklyGraphView.AddSubview(lbl_GraphHead_Weekly);

            SaleWeeklyChartView = new ChartView();
            SaleWeeklyChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleWeeklyChartView.BackgroundColor = UIColor.Red;
            WeeklyGraphView.AddSubview(SaleWeeklyChartView);
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
            lbl_totalHead.Text = Utils.TextBundle("totalsales", "Items");
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
                //saletoday 1 salemonth 2 saleyear 3  
                var filterPage = new ReportFilter2Controller(2);
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
            lblHours.Text = Utils.TextBundle("Hourly", "Items");
            Hour_TimeHeadView.AddSubview(lblHours);

            lblSaleHour = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112,112,112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleHour.Font = lblSaleHour.Font.WithSize(15);
            lblSaleHour.Text = Utils.TextBundle("sale", "Items");
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

            #region Daily_timeView
            Daily_timeView = new UIView();
            Daily_timeView.BackgroundColor = UIColor.White;
            Daily_timeView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region TimeHead
            Daily_TimeHeadView = new UIView();
            Daily_TimeHeadView.BackgroundColor = UIColor.White;
            Daily_TimeHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            Daily_timeView.AddSubview(Daily_TimeHeadView);

            lblDaily = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDaily.Font = lblDaily.Font.WithSize(15);
            lblDaily.Text = Utils.TextBundle("daily", "Items");
            Daily_TimeHeadView.AddSubview(lblDaily);

            lblSaleDaily = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleDaily.Font = lblSaleDaily.Font.WithSize(15);
            lblSaleDaily.Text = Utils.TextBundle("sale", "Items");
            Daily_TimeHeadView.AddSubview(lblSaleDaily);

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
            UICollectionViewFlowLayout item2flowLayoutList = new UICollectionViewFlowLayout();
            item2flowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 30);
            item2flowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            DaillyTimeCollectionView = new UICollectionView(frame: View.Frame, layout: item2flowLayoutList);
            DaillyTimeCollectionView.BackgroundColor = UIColor.White;
            DaillyTimeCollectionView.ScrollEnabled = false;
            DaillyTimeCollectionView.ShowsVerticalScrollIndicator = false;
            DaillyTimeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            DaillyTimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleViewCell), reuseIdentifier: "HourSaleViewCell");
            Daily_timeView.AddSubview(DaillyTimeCollectionView);
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
            lblWeekly.Text = Utils.TextBundle("weekly", "Items");
            Weekly_TimeHeadView.AddSubview(lblWeekly);

            lblSaleWeekly = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSaleWeekly.Font = lblSaleWeekly.Font.WithSize(15);
            lblSaleWeekly.Text = Utils.TextBundle("sale", "Items");
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

            _contentView.AddSubview(MenuView);
            _contentView.AddSubview(TopView);
            _contentView.AddSubview(HourlyGraphView);
            _contentView.AddSubview(Hour_timeView);
            _contentView.AddSubview(WeeklyGraphView);
            _contentView.AddSubview(Weekly_timeView);
            _contentView.AddSubview(DailyGraphView);
            _contentView.AddSubview(Daily_timeView);

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

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
            View.AddSubview(BottomView);
        }
        #region bottom view toggle
        [Export("Hour:")]
        public void Hour(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            HourLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblDailyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            DailyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = false;
            DailyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = false;
            Daily_timeView.Hidden = true;
            Weekly_timeView.Hidden = true;
            Typetime = Utils.TextBundle("Hourly", "Items");
            lbl_total.Text = ltotalhour;
            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 1440);
            _contentView.LayoutIfNeeded();
        }
        [Export("Daily:")]
        public void Daily(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(64,64,64);
            HourLineView.BackgroundColor = UIColor.White;

            lblDailyMenu.TextColor = UIColor.FromRGB(0, 149, 218);
            DailyLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            lblWeeklyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            WeeklyLineView.BackgroundColor = UIColor.White;

            HourlyGraphView.Hidden = true;
            DailyGraphView.Hidden = false;
            WeeklyGraphView.Hidden = true;

            Hour_timeView.Hidden = true;
            Daily_timeView.Hidden = false;
            Weekly_timeView.Hidden = true;
            Typetime = Utils.TextBundle("daily", "Items");
            lbl_total.Text = ltotaldaily;
            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 1690);
            _contentView.LayoutIfNeeded();

        }
        [Export("Weekly:")]
        public void Weekly(UIGestureRecognizer sender)
        {
            lblHourMenu.TextColor = UIColor.FromRGB(64,64,64);
            HourLineView.BackgroundColor = UIColor.White;

            lblDailyMenu.TextColor = UIColor.FromRGB(64, 64, 64);
            DailyLineView.BackgroundColor = UIColor.White;

            lblWeeklyMenu.TextColor = UIColor.FromRGB(0,149,218);
            WeeklyLineView.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            HourlyGraphView.Hidden = true;
            DailyGraphView.Hidden = true;
            WeeklyGraphView.Hidden = false;

            Hour_timeView.Hidden = true;
            Daily_timeView.Hidden = true;
            Weekly_timeView.Hidden = false;
            Typetime = Utils.TextBundle("weekly", "Items");
            lbl_total.Text = ltotalWeekly;
            Utils.SetConstant(_contentView.Constraints, NSLayoutAttribute.Height, 740);
            _contentView.LayoutIfNeeded();
        }
        #endregion

        private async void Search()
        {
            var result = await SearchBytxt(txtSearch.Text);
            var result2 = await SearchBytxt2(txtSearch.Text);
            var result3 = await SearchBytxt3(txtSearch.Text);

            ReportSaleHourlyDataSource report_adapter_showHour = new ReportSaleHourlyDataSource(result);
            HourTimeCollectionView.DataSource = report_adapter_showHour;
            HourTimeCollectionView.ReloadData();

            ReportSaleDailyDataSource report_adapter_showDaily = new ReportSaleDailyDataSource(result2);
            DaillyTimeCollectionView.DataSource = report_adapter_showDaily;
            DaillyTimeCollectionView.ReloadData();

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
        private async Task<List<ReportDaily>> SearchBytxt2(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return report2;
            }
            else
            {
                return report2.Where(m => m.Dailyname.Contains(text)).ToList();
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
        void SetupAutoLayout()
        {
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
            _contentView.HeightAnchor.ConstraintEqualTo(1440).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region TopView
            TopView.TopAnchor.ConstraintEqualTo(TopView.Superview.TopAnchor, 0).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(TopView.Superview.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(TopView.Superview.RightAnchor, 0).Active = true;

            lblDay.CenterYAnchor.ConstraintEqualTo(lblDay.Superview.CenterYAnchor, 0).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(lblDay.Superview.LeftAnchor, 15).Active = true;
            lblDay.WidthAnchor.ConstraintEqualTo(85).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(lblDate.Superview.CenterYAnchor, 0).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.LeftAnchor,-15).Active = true;

            lblBranch.CenterYAnchor.ConstraintEqualTo(lblBranch.Superview.CenterYAnchor, 0).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(lblBranch.Superview.RightAnchor, -20).Active = true;
            //   lblBranch.WidthAnchor.ConstraintEqualTo(150).Active = true;

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
            MenuView.TopAnchor.ConstraintEqualTo(Viewnull.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
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

            #region DailyView
            DailyView.TopAnchor.ConstraintEqualTo(DailyView.Superview.TopAnchor, 0).Active = true;
            DailyView.BottomAnchor.ConstraintEqualTo(DailyView.Superview.BottomAnchor, 0).Active = true;
            DailyView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
            DailyView.LeftAnchor.ConstraintEqualTo(HourView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDailyMenu.CenterXAnchor.ConstraintEqualTo(lblDailyMenu.Superview.CenterXAnchor).Active = true;
            lblDailyMenu.CenterYAnchor.ConstraintEqualTo(lblDailyMenu.Superview.CenterYAnchor).Active = true;
            lblDailyMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            DailyLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            DailyLineView.BottomAnchor.ConstraintEqualTo(DailyLineView.Superview.BottomAnchor, 0).Active = true;
            DailyLineView.RightAnchor.ConstraintEqualTo(DailyLineView.Superview.RightAnchor).Active = true;
            DailyLineView.LeftAnchor.ConstraintEqualTo(DailyLineView.Superview.LeftAnchor).Active = true;
            #endregion

            #region WeeklyView
            WeeklyView.TopAnchor.ConstraintEqualTo(WeeklyView.Superview.TopAnchor, 0).Active = true;
            WeeklyView.BottomAnchor.ConstraintEqualTo(WeeklyView.Superview.BottomAnchor, 0).Active = true;
            WeeklyView.RightAnchor.ConstraintEqualTo(WeeklyView.Superview.RightAnchor).Active = true;
            WeeklyView.LeftAnchor.ConstraintEqualTo(DailyView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblWeeklyMenu.CenterXAnchor.ConstraintEqualTo(lblWeeklyMenu.Superview.CenterXAnchor).Active = true;
            lblWeeklyMenu.CenterYAnchor.ConstraintEqualTo(lblWeeklyMenu.Superview.CenterYAnchor).Active = true;
            lblWeeklyMenu.HeightAnchor.ConstraintEqualTo(18).Active = true;

            WeeklyLineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            WeeklyLineView.BottomAnchor.ConstraintEqualTo(WeeklyLineView.Superview.BottomAnchor, 0).Active = true;
            WeeklyLineView.RightAnchor.ConstraintEqualTo(WeeklyLineView.Superview.RightAnchor).Active = true;
            WeeklyLineView.LeftAnchor.ConstraintEqualTo(WeeklyLineView.Superview.LeftAnchor).Active = true;
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
            lbl_GraphHead_Hour.WidthAnchor.ConstraintEqualTo(100).Active = true;

            SaleHourChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Hour.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            SaleHourChartView.RightAnchor.ConstraintEqualTo(SaleHourChartView.Superview.RightAnchor).Active = true;
            SaleHourChartView.LeftAnchor.ConstraintEqualTo(SaleHourChartView.Superview.LeftAnchor, 0).Active = true;
            SaleHourChartView.BottomAnchor.ConstraintEqualTo(SaleHourChartView.Superview.BottomAnchor).Active = true;
            #endregion

            #region DailyGraphView
            DailyGraphView.TopAnchor.ConstraintEqualTo(MenuView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            DailyGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;
            DailyGraphView.LeftAnchor.ConstraintEqualTo(DailyGraphView.Superview.LeftAnchor, 0).Active = true;
            DailyGraphView.RightAnchor.ConstraintEqualTo(DailyGraphView.Superview.RightAnchor, 0).Active = true;

            lbl_GraphHead_Daily.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Daily.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead_Daily.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead_Daily.LeftAnchor.ConstraintEqualTo(lbl_GraphHead_Daily.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead_Daily.WidthAnchor.ConstraintEqualTo(150).Active = true;

            SaleDailyChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead_Daily.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            SaleDailyChartView.RightAnchor.ConstraintEqualTo(SaleDailyChartView.Superview.RightAnchor).Active = true;
            SaleDailyChartView.LeftAnchor.ConstraintEqualTo(SaleDailyChartView.Superview.LeftAnchor, 0).Active = true;
            SaleDailyChartView.BottomAnchor.ConstraintEqualTo(SaleDailyChartView.Superview.BottomAnchor).Active = true;
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

            #region Daily_timeView
            Daily_timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            Daily_timeView.RightAnchor.ConstraintEqualTo(Daily_timeView.Superview.RightAnchor).Active = true;
            Daily_timeView.LeftAnchor.ConstraintEqualTo(Daily_timeView.Superview.LeftAnchor, 0).Active = true;
            Daily_timeView.BottomAnchor.ConstraintEqualTo(Daily_timeView.Superview.BottomAnchor).Active = true;
            Daily_timeView.HeightAnchor.ConstraintEqualTo(1300);

            #region Daily_TimeHeadView
            Daily_TimeHeadView.TopAnchor.ConstraintEqualTo(Daily_TimeHeadView.Superview.TopAnchor, 0).Active = true;
            Daily_TimeHeadView.RightAnchor.ConstraintEqualTo(Daily_TimeHeadView.Superview.RightAnchor).Active = true;
            Daily_TimeHeadView.LeftAnchor.ConstraintEqualTo(Daily_TimeHeadView.Superview.LeftAnchor, 0).Active = true;
            Daily_TimeHeadView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lblDaily.CenterYAnchor.ConstraintEqualTo(lblDaily.Superview.CenterYAnchor, 0).Active = true;
            lblDaily.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDaily.LeftAnchor.ConstraintEqualTo(lblDaily.Superview.LeftAnchor, 20).Active = true;
            lblDaily.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblSaleDaily.CenterYAnchor.ConstraintEqualTo(lblSaleDaily.Superview.CenterYAnchor, 0).Active = true;
            lblSaleDaily.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSaleDaily.RightAnchor.ConstraintEqualTo(lblSaleDaily.Superview.RightAnchor, -20).Active = true;
            lblSaleDaily.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region DaillyTimeCollectionView
            DaillyTimeCollectionView.TopAnchor.ConstraintEqualTo(Daily_TimeHeadView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DaillyTimeCollectionView.RightAnchor.ConstraintEqualTo(DaillyTimeCollectionView.Superview.RightAnchor).Active = true;
            DaillyTimeCollectionView.LeftAnchor.ConstraintEqualTo(DaillyTimeCollectionView.Superview.LeftAnchor, 0).Active = true;
            DaillyTimeCollectionView.BottomAnchor.ConstraintEqualTo(DaillyTimeCollectionView.Superview.BottomAnchor).Active = true;
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
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        internal async void setupData()
        {
            try
            {
                Typetime = Utils.TextBundle("hourly", "Items");
                GetDataReportMonth = new List<SummaryHourly>();
                var now = DateTime.UtcNow;
                if (typCustomBydate)
                {
                    lblDay.Text = Utils.TextBundle("custom", "Items")+", ";
                    lblDate.Text = this.DateSet;
                    datenamefile = this.DateSet;
                }
                else
                {
                    lblDay.Text = Utils.TextBundle("thismonth", "Items")+", ";
                    lblDate.Text = DateTime.Now.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    datenamefile = DateTime.Now.ToString("MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                var sysbranIdSelect = "";
                foreach (var item2 in ReportController.listChooseBranch)
                {
                    if (sysbranIdSelect != "")
                    {
                        sysbranIdSelect += "," + item2.SysBranchID.ToString();
                        lstsysBranchID.Add((int)item2.SysBranchID);

                    }
                    else
                    {
                        sysbranIdSelect = item2.SysBranchID.ToString();
                        lstsysBranchID = new List<int>();
                        lstsysBranchID.Add((int)item2.SysBranchID);
                    }
                }

                GetDataReportMonth = await GabanaAPI.GetDataReportSummaryHourly(sysbranIdSelect, StartDate, Enddate);


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

                reportSale = new ReportSale();
                if (GetDataReportMonth != null && GetDataReportMonth.Count != 0)
                {


                    reportSale = await CalReport.CalReportSale(GetDataReportMonth);
                    #region Hour
                    //switch (filterReport)
                    //{
                    //    case 1:
                    //        cusResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                    //        break;
                    //    case 2:
                    //        cusResponses = result.OrderBy(x => x.sumTotalAmount).ToList();
                    //        break;
                    //    case 3:
                    //        cusResponses = result.OrderByDescending(x => x.customerName).ToList();
                    //        break;
                    //    case 4:
                    //        cusResponses = result.OrderBy(x => x.customerName).ToList();
                    //        break;
                    //    default:
                    //        break;
                    //}
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
                    List<ChartEntry> SalesByPeriod = new List<ChartEntry>();

                    ReportSaleHourlyDataSource report_adapter_showHour = new ReportSaleHourlyDataSource(report);
                    HourTimeCollectionView.DataSource = report_adapter_showHour;
                    HourTimeCollectionView.ReloadData();
                    ltotalhour = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + reportSale.reportHourlies.Sum(x => x.value).ToString("#,##0.00");
                    lbl_total.Text = ltotalhour;
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
                                SalesByPeriod.Add(entry);
                            }
                            else if (item.IdHourly % 3 == 0 && item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriod.Add(entry);
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
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            ChartEntry entry2 = new ChartEntry((float)item.value)
                            {
                                Label = item.Hourlyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            if (string.IsNullOrEmpty(entry.ValueLabel))
                            {
                                entry.ValueLabel = (0).ToString();
                            }
                            if (string.IsNullOrEmpty(entry2.ValueLabel))
                            {
                                entry2.ValueLabel = (0).ToString();
                            }

                            if (item.IdHourly == firstIndex || item.IdHourly == lastIndex)
                            {
                                SalesByPeriod.Add(entry);
                            }
                            else if (item.IdHourly % 3 == 0 && item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriod.Add(entry);
                            }
                            else if (item.IdHourly > firstIndex && item.IdHourly < lastIndex)
                            {
                                SalesByPeriod.Add(entry2);
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
                            SalesByPeriod.Add(entry);
                        }
                    }

                    var chart = new LineChart() { Entries = SalesByPeriod, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleHourChartView.Chart = chart;
                    #endregion
                    #region Daily
                    report2 = new List<ReportDaily>();
                    switch (filterReport)
                    {
                        case 1:
                            report2 = reportSale.reportDailies.ToList();
                            break;
                        case 2:
                            report2 = reportSale.reportDailies.OrderByDescending(x => x.value).ToList();
                            break;
                        case 3:
                            report2 = reportSale.reportDailies.OrderBy(x => x.value).ToList();
                            break;
                        default:
                            report2 = reportSale.reportDailies.ToList();
                            break;
                    }

                    List<ChartEntry> SalesByPeriods1 = new List<ChartEntry>();
                    ReportSaleDailyDataSource report_adapter_showDaily = new ReportSaleDailyDataSource(report2);
                    DaillyTimeCollectionView.DataSource = report_adapter_showDaily;
                    DaillyTimeCollectionView.ReloadData();


                    ltotaldaily = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + reportSale.reportDailies.Sum(x => x.value).ToString("#,##0.00");
                    var firstIndex2 = reportSale.reportDailies.FindIndex(x => x.value > 0) - 1;
                    var lastIndex2 = reportSale.reportDailies.FindLastIndex(x => x.value > 0) + 2;
                    if (firstIndex2 < 0) firstIndex2 = 0;
                    if (lastIndex2 > reportSale.reportDailies.Count) lastIndex2 = reportSale.reportDailies.Count;

                    var dif2 = Math.Abs(lastIndex2 - firstIndex2);
                    if (dif2 > 16)
                    {
                        foreach (var item in reportSale.reportDailies)
                        {
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Dailyname .ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            if (item.IdDaily == firstIndex2 || item.IdDaily == lastIndex2)
                            {
                                SalesByPeriods1.Add(entry);
                            }
                            else if (item.IdDaily % 3 == 0 && item.IdDaily > firstIndex2 && item.IdDaily < lastIndex2)
                            {
                                SalesByPeriods1.Add(entry);
                            }
                        }
                    }
                    else if (dif > 8)
                    {
                        firstIndex2 = reportSale.reportDailies.FindIndex(x => x.value > 0);
                        lastIndex2 = reportSale.reportDailies.FindLastIndex(x => x.value > 0);
                        if (firstIndex2 % 3 != 0) firstIndex2 = firstIndex2 - (firstIndex2 % 3);
                        if (lastIndex2 % 3 != 0) lastIndex2 = lastIndex2 + (3 - (lastIndex2 % 3));


                        foreach (var item in reportSale.reportDailies)
                        {

                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Dailyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            ChartEntry entry2 = new ChartEntry((float)item.value)
                            {
                                Label = item.Dailyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            if (string.IsNullOrEmpty(entry.ValueLabel))
                            {
                                entry.ValueLabel = (0).ToString();
                            }
                            if (string.IsNullOrEmpty(entry2.ValueLabel))
                            {
                                entry2.ValueLabel = (0).ToString();
                            }

                            if (item.IdDaily == firstIndex2 || item.IdDaily == lastIndex2)
                            {
                                SalesByPeriods1.Add(entry);
                            }
                            else if (item.IdDaily % 3 == 0 && item.IdDaily > firstIndex2 && item.IdDaily < lastIndex2)
                            {
                                SalesByPeriods1.Add(entry);
                            }
                            else if (item.IdDaily > firstIndex2 && item.IdDaily < lastIndex2)
                            {
                                SalesByPeriods1.Add(entry2);
                            }
                        }
                    }
                    else
                    {
                        for (int i = firstIndex2; i < lastIndex2; i++)
                        {
                            var item = reportSale.reportDailies[i];
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Dailyname.ToString(),
                                ValueLabel = item.value.ToString("#,###.##"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            SalesByPeriods1.Add(entry);
                        }
                    }


                    

                    var chart1 = new LineChart() { Entries = SalesByPeriods1, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleDailyChartView.Chart = chart1;
                    SaleDailyChartView.Hidden = false;

                    #endregion
                    #region Weekly
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
                    List<ChartEntry> SalesByPeriods2 = new List<ChartEntry>();
                    ReportSaleWeeklyDataSource report_adapter_showWeekly = new ReportSaleWeeklyDataSource(report3);
                    WeeklyTimeCollectionView.DataSource = report_adapter_showWeekly;
                    WeeklyTimeCollectionView.ReloadData();

                    ltotalWeekly = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + reportSale.reportWeeklies.Sum(x => x.value).ToString("#,##0.00");

                    var firstIndex3 = reportSale.reportWeeklies.FindIndex(x => x.value > 0) - 1;
                    var lastIndex3 = reportSale.reportWeeklies.FindLastIndex(x => x.value > 0) + 2;
                    if (firstIndex3 < 0) firstIndex3 = 0;
                    if (lastIndex3 > reportSale.reportWeeklies.Count) lastIndex3 = reportSale.reportWeeklies.Count;

                    var dif3 = Math.Abs(lastIndex3 - firstIndex3);
                    if (dif3 > 16)
                    {
                        foreach (var item in reportSale.reportWeeklies)
                        {
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Weeklyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            if (item.IdWeekly == firstIndex3 || item.IdWeekly == lastIndex3)
                            {
                                SalesByPeriods2.Add(entry);
                            }
                            else if (item.IdWeekly % 3 == 0 && item.IdWeekly > firstIndex3 && item.IdWeekly < lastIndex3)
                            {
                                SalesByPeriods2.Add(entry);
                            }
                        }
                    }
                    else if (dif > 8)
                    {
                        firstIndex3 = reportSale.reportWeeklies.FindIndex(x => x.value > 0);
                        lastIndex3 = reportSale.reportWeeklies.FindLastIndex(x => x.value > 0);
                        if (firstIndex3 % 3 != 0) firstIndex3 = firstIndex3 - (firstIndex3 % 3);
                        if (lastIndex3 % 3 != 0) lastIndex3 = lastIndex3 + (3 - (lastIndex3 % 3));


                        foreach (var item in reportSale.reportWeeklies)
                        {

                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Weeklyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };

                            ChartEntry entry2 = new ChartEntry((float)item.value)
                            {
                                Label = item.Weeklyname.ToString(),
                                ValueLabel = item.value.ToString("#,###"),
                                ValueLabelColor = SKColor.Parse("#404040"),
                                TextColor = SKColor.Parse("#404040"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            if (string.IsNullOrEmpty(entry.ValueLabel))
                            {
                                entry.ValueLabel = (0).ToString();
                            }
                            if (string.IsNullOrEmpty(entry2.ValueLabel))
                            {
                                entry2.ValueLabel = (0).ToString();
                            }

                            if (item.IdWeekly == firstIndex3 || item.IdWeekly == lastIndex3)
                            {
                                SalesByPeriods2.Add(entry);
                            }
                            else if (item.IdWeekly % 3 == 0 && item.IdWeekly > firstIndex3 && item.IdWeekly < lastIndex3)
                            {
                                SalesByPeriods2.Add(entry);
                            }
                            else if (item.IdWeekly > firstIndex3 && item.IdWeekly < lastIndex3)
                            {
                                SalesByPeriods2.Add(entry2);
                            }
                        }
                    }
                    else
                    {
                        for (int i = firstIndex3; i < lastIndex3; i++)
                        {
                            var item = reportSale.reportWeeklies[i];
                            ChartEntry entry = new ChartEntry((float)item.value)
                            {
                                Label = item.Weeklyname.ToString(),
                                ValueLabel = item.value.ToString("#,###.##"),
                                Color = SKColor.Parse("#0095DA")
                            };
                            SalesByPeriods2.Add(entry);
                        }
                    }

                    var chart2 = new LineChart() { Entries = SalesByPeriods2, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleWeeklyChartView.Chart = chart2;
                    #endregion
                    Utils.SetConstant(Viewnull.Constraints, NSLayoutAttribute.Height, 0);
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

                
                
                

                if (typCustomBydate)
                {
                    var file = "SaleReport_" + this.filename;
                    ReportManager.createDetail("SalesReport", file, reportSale, "CustomDate", Typetime, null, null, GetDataReportMonth,null, lblBranch.Text);
                }
                else
                {
                    var file = "SaleReport_" + datenamefile;
                    ReportManager.createDetail("SalesReport", file, reportSale, "Month", Typetime, lblDate.Text, null, GetDataReportMonth, null, lblBranch.Text);
                }

                

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }


        }

    }
}