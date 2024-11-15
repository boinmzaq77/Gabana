using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Report;
using Microcharts;
//using Microcharts.Droid;
//using Microcharts.iOS;
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
using Microcharts.iOS;

namespace Gabana.iOS
{

    public partial class SaleReportBranchTodayController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        UICollectionView TimeCollectionView;

        UIView timeView, TimeHeadView;
        UILabel lblHours, lblSale;

        UIView TopView;
        UILabel lblDate, lblDay;

        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel  lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView GraphView;
        UILabel lbl_GraphHead;
        ChartView SaleChartView;
        private UIView NullView;
        private UILabel lbl_null;
        private UIImageView imgnull;
        bool typCustomBydate = false;
        string StartDate;
        string DateSet;
        string Enddate;
        private string filename;
        int type;
        string Typetime;
        List<SaleReportBranch> reportSale;
        List<SalesByBranchModel> salesByBranches;
        private UILabel lbl_total;
        private UIView SearchbarView;
        private UITextField txtSearch;
        private UIButton btnSearch;
        private UIButton btnfilter;
        private string datenamefile;

        public SaleReportBranchTodayController(int type)
        {
            this.type=type;
        }

        public SaleReportBranchTodayController(int type ,  string v2, string start, string end, string filename)
        {
            this.type = type;
            this.DateSet = v2;
            this.StartDate = start;
            this.Enddate = end;
            this.filename = filename;
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
        }
        public async override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.White;
            try
            {
                base.ViewDidLoad();
                initAttribute();
                SetupAutoLayout();
                setupData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
            }

        }
        internal async void setupData()
        {
            try
            {

            
            lblDay.Text = Utils.TextBundle("today", "Items")+", ";
            lblDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            string namebranch = "";
            salesByBranches = new List<SalesByBranchModel>();
            var now = DateTime.UtcNow;
            
            switch (type)
            {
                case 1:
                        Typetime = Utils.TextBundle("date", "Items");
                    lblDay.Text = Utils.TextBundle("today", "Items") + ",";
                        lblDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    datenamefile = DateTime.Now.ToString("DD-MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        var latestTranDate = DateTime.UtcNow;
                    var datenow = Utils.ChangeDateTimeReport(latestTranDate);
                    salesByBranches = await GabanaAPI.GetDataReportSalesByBranchModel(datenow, datenow);
                    break;
                case 2:
                        Typetime = Utils.TextBundle("month", "Items");
                        lblDay.Text = Utils.TextBundle("thismonth", "This Month") + ",";
                        lblDate.Text = DateTime.Now.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    datenamefile = DateTime.Now.ToString("MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                        var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    var lastDayOfMonth = new DateTime(now.Year, now.Month, DaysInMonth);
                    var startDate = Utils.ChangeDateTimeReport(startOfMonth);
                    var endDate = Utils.ChangeDateTimeReport(lastDayOfMonth);
                    
                    salesByBranches = await GabanaAPI.GetDataReportSalesByBranchModel(startDate, endDate);
                    break;
                case 3:
                        Typetime = Utils.TextBundle("year", "Items");
                        lblDay.Text = Utils.TextBundle("thisyear", "This Year") + ",";
                        lblDate.Text = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    datenamefile = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                        var year = DateTime.UtcNow.Year;
                    var startOfYear = new DateTime(year, 1, 1);
                    var lastDayOfYear = DateTime.UtcNow;
                    var startDate2 = Utils.ChangeDateTimeReport(startOfYear);
                    var endDate2 = Utils.ChangeDateTimeReport(lastDayOfYear);
                    salesByBranches = await GabanaAPI.GetDataReportSalesByBranchModel(startDate2, endDate2);
                    break;
                case 4:
                    Typetime = Utils.TextBundle("customdate", "Items");
                    lblDay.Text = Utils.TextBundle("custom", "Custom") + ",";
                    lblDate.Text = this.DateSet;
                        datenamefile = filename; 

                    salesByBranches = await GabanaAPI.GetDataReportSalesByBranchModel(StartDate, Enddate);

                    break;
                default:
                    break;
            }

            //graph
            

            
            //var GetDataReportSummary = await GabanaAPI.GetDataReportSummaryHourly(ReportController.listChooseBranch[0].SysBranchID.ToString(), datenow, datenow);
            BranchManage branchManage = new BranchManage();
            var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
           

            if (salesByBranches.Count != 0)
            {
                reportSale = await CalReport.initialBranch(salesByBranches, lstBranch);
                List<ChartEntry> SalesByPeriod = new List<ChartEntry>();
                reportSale = reportSale.OrderByDescending(x => x.sumGrandTotal).ToList();
                ReportSaleBranchDailyDataSource report_adapter_showHour = new ReportSaleBranchDailyDataSource(reportSale);
                TimeCollectionView.DataSource = report_adapter_showHour;
                TimeCollectionView.ReloadData();
                    lbl_total.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS +" "+ Utils.DisplayDecimal(reportSale.Sum(x=>x.sumGrandTotal));

                //List<string> Colors = new List<string> { "#0077AF", "#0086C4", "#0086C4", "#1AA0DE", "#33AAE2", "#4DB5E5", "#66C0E9", "#80CAEC", "#99D4F0", "#B3DFF4" };
                //int i = 0;
                //foreach (var item in reportSale)
                ////{
                //    var ascii = Encoding.UTF32.GetBytes(item.BranchName?.ToString());
                //    var text = Encoding.UTF32.GetString(ascii);
                //    var plainTextBytes = Encoding.UTF8.GetBytes(item.BranchName?.ToString());
                //    var textutf8 =   Convert.ToBase64String(plainTextBytes);
                //    ChartEntry entry = new ChartEntry((float)item.sumGrandTotal)
                //    {
                //        Label = textutf8,
                //        //ValueLabel = item.sumGrandTotal.ToString("#,###"),
                //        Color = SKColor.Parse(Colors[i]),
                //        ValueLabelColor = SKColor.Parse(Colors[i])
                //    };

                    //    SalesByPeriod.Add(entry);
                    //    i++;

                    //    }

                    //var chart = new BarChart() { Entries = SalesByPeriod, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF")  };
                    //BarChart chart = new BarChart
                    //{
                    //    Entries = SalesByPeriod,
                    //    LabelTextSize = 20f,
                    //    ValueLabelOrientation = Orientation.Horizontal,
                    //    LabelOrientation = Orientation.Horizontal,

                    //    //Typeface = SKFontManager.Default.MatchCharacter('ก'),
                    //    BackgroundColor = SKColor.Parse("#FFF"),
                    //    //Margin = 10f,
                    //};

                    //SaleChartView.Chart = chart;

                    NullView.Hidden = true;
                GraphView.Hidden = false;
                timeView.Hidden = false;
                BottomView.Hidden = false;
            }
            else
            {
                NullView.Hidden = false;
                GraphView.Hidden = true;
                timeView.Hidden = true;
                BottomView.Hidden = true;
            }

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }

        }
        void initAttribute()
        {


            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.White;
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

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
                var listsearch = new List<SaleReportBranch>(); 
                listsearch = reportSale.Where(x=>x.BranchName.Contains(txtSearch.Text)).OrderBy(x => x.BranchName).ToList();
                ReportSaleBranchDailyDataSource report_adapter_showHour = new ReportSaleBranchDailyDataSource(listsearch);
                TimeCollectionView.DataSource = report_adapter_showHour;
                TimeCollectionView.ReloadData();
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
                    var listsearch = new List<SaleReportBranch>();
                    listsearch = reportSale.Where(x => x.BranchName.Contains(txtSearch.Text)).OrderBy(x => x.BranchName).ToList();
                    ReportSaleBranchDailyDataSource report_adapter_showHour = new ReportSaleBranchDailyDataSource(listsearch);
                    TimeCollectionView.DataSource = report_adapter_showHour;
                    TimeCollectionView.ReloadData();
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
                
                var filterPage = new ReportFilterController(2);
                this.NavigationController.PushViewController(filterPage, false);
            };
            SearchbarView.AddSubview(btnfilter);
            #endregion


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
                TextColor = UIColor.FromRGB(0,149,218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDate.Font = lblDate.Font.WithSize(15);
            TopView.AddSubview(lblDate);

            

            #endregion




            #region GraphView
            GraphView = new UIView();
            GraphView.BackgroundColor = UIColor.White;
            GraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247,86,0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead.Text = Utils.TextBundle("allbranch", "Items");
            lbl_GraphHead.Font = lbl_GraphHead.Font.WithSize(15);
            GraphView.AddSubview(lbl_GraphHead);

            lbl_total = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_total.TextAlignment = UITextAlignment.Right;
            lbl_total.Text = "";
            lbl_total.Font = lbl_total.Font.WithSize(20);
            GraphView.AddSubview(lbl_total);

            //SaleChartView = new ChartView();
            //SaleChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            //SaleChartView.BackgroundColor = UIColor.White;
            //GraphView.AddSubview(SaleChartView);
            #endregion

            #region GraphView
            NullView = new UIView();
            NullView.BackgroundColor = UIColor.White;
            NullView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_null = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_null.Text = Utils.TextBundle("nothavedatasell", "Items");
            lbl_null.TextAlignment = UITextAlignment.Center;
            lbl_null.Font = lbl_GraphHead.Font.WithSize(15); 
            NullView.AddSubview(lbl_null);

            imgnull = new UIImageView();
            imgnull.Image = UIImage.FromFile("DefaultReport.png");
            imgnull.TranslatesAutoresizingMaskIntoConstraints = false;
            NullView.AddSubview(imgnull);
            #endregion

            #region TimeView
            timeView = new UIView();
            timeView.BackgroundColor = UIColor.White;
            timeView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region TimeHead
            TimeHeadView = new UIView();
            TimeHeadView.BackgroundColor = UIColor.White;
            TimeHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            timeView.AddSubview(TimeHeadView);

            lblHours = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHours.Font = lblHours.Font.WithSize(15);
            lblHours.Text = Utils.TextBundle("branch", "Items");
            TimeHeadView.AddSubview(lblHours);

            lblSale = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112,112,112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSale.Font = lblSale.Font.WithSize(15);
            lblSale.Text = Utils.TextBundle("sale", "Items");
            TimeHeadView.AddSubview(lblSale);

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
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 30);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            TimeCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            TimeCollectionView.BackgroundColor = UIColor.White;
            TimeCollectionView.ScrollEnabled = false;
            TimeCollectionView.ShowsVerticalScrollIndicator = false;
            TimeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            TimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleBranchViewCell), reuseIdentifier: "HourSaleBranchViewCell");
            timeView.AddSubview(TimeCollectionView);
            #endregion
            #endregion

            _contentView.AddSubview(SearchbarView);
            _contentView.AddSubview(timeView);
            _contentView.AddSubview(TopView);
            _contentView.AddSubview(GraphView); 
            _contentView.AddSubview(NullView);
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

            View.BackgroundColor = UIColor.White;
            View.AddSubview(BottomView);
        }
        #region bottom view toggle
        

        #endregion
        void SetupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region TopView
            TopView.TopAnchor.ConstraintEqualTo(TopView.Superview.TopAnchor, 0).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(TopView.Superview.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(TopView.Superview.RightAnchor, 0).Active = true;

            lblDay.CenterYAnchor.ConstraintEqualTo(lblDay.Superview.CenterYAnchor, 0).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(lblDay.Superview.LeftAnchor, 15).Active = true;
            lblDay.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(lblDate.Superview.CenterYAnchor, 0).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            //lblDate.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(lblDay.Superview.RightAnchor, -15).Active = true;



            #endregion

            

            #region GraphView
            GraphView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            GraphView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            GraphView.LeftAnchor.ConstraintEqualTo(GraphView.Superview.LeftAnchor, 0).Active = true;
            GraphView.RightAnchor.ConstraintEqualTo(GraphView.Superview.RightAnchor, 0).Active = true;

            NullView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            NullView.HeightAnchor.ConstraintEqualTo(500).Active = true;
            NullView.LeftAnchor.ConstraintEqualTo(NullView.Superview.LeftAnchor, 0).Active = true;
            NullView.RightAnchor.ConstraintEqualTo(NullView.Superview.RightAnchor, 0).Active = true;



            lbl_GraphHead.TopAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead.LeftAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead.RightAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.RightAnchor, -15).Active = true;

            lbl_total.TopAnchor.ConstraintEqualTo(lbl_GraphHead.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lbl_total.RightAnchor.ConstraintEqualTo(lbl_total.Superview.RightAnchor,-15).Active = true;
            lbl_total.LeftAnchor.ConstraintEqualTo(lbl_total.Superview.LeftAnchor, 0).Active = true;
            lbl_total.BottomAnchor.ConstraintEqualTo(lbl_total.Superview.BottomAnchor).Active = true;

            imgnull.TopAnchor.ConstraintEqualTo(imgnull.Superview.TopAnchor, 50).Active = true;
            imgnull.CenterXAnchor.ConstraintEqualTo(NullView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            imgnull.WidthAnchor.ConstraintEqualTo(300).Active = true;
            imgnull.HeightAnchor.ConstraintEqualTo(175).Active = true;


            lbl_null.TopAnchor.ConstraintEqualTo(imgnull.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            lbl_null.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lbl_null.LeftAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.LeftAnchor, 15).Active = true;
            lbl_null.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lbl_null.CenterXAnchor.ConstraintEqualTo(NullView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            #endregion


            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(GraphView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
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

            #region TimeView
            timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            timeView.RightAnchor.ConstraintEqualTo(timeView.Superview.RightAnchor).Active = true;
            timeView.LeftAnchor.ConstraintEqualTo(timeView.Superview.LeftAnchor, 0).Active = true;
            timeView.HeightAnchor.ConstraintEqualTo(400).Active = true;
            timeView.BottomAnchor.ConstraintEqualTo(timeView.Superview.BottomAnchor).Active = true;




            #region TimeHead
            TimeHeadView.TopAnchor.ConstraintEqualTo(TimeHeadView.Superview.TopAnchor,0).Active = true;
            TimeHeadView.RightAnchor.ConstraintEqualTo(TimeHeadView.Superview.RightAnchor).Active = true;
            TimeHeadView.LeftAnchor.ConstraintEqualTo(TimeHeadView.Superview.LeftAnchor, 0).Active = true;
            TimeHeadView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lblHours.CenterYAnchor.ConstraintEqualTo(lblHours.Superview.CenterYAnchor, 0).Active = true;
            lblHours.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblHours.LeftAnchor.ConstraintEqualTo(lblHours.Superview.LeftAnchor, 20).Active = true;
            lblHours.WidthAnchor.ConstraintEqualTo(50).Active = true;

            lblSale.CenterYAnchor.ConstraintEqualTo(lblSale.Superview.CenterYAnchor, 0).Active = true;
            lblSale.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSale.RightAnchor.ConstraintEqualTo(lblSale.Superview.RightAnchor, -20).Active = true;
            lblSale.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region TimeCollectionView
            TimeCollectionView.TopAnchor.ConstraintEqualTo(TimeHeadView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            
            TimeCollectionView.RightAnchor.ConstraintEqualTo(TimeCollectionView.Superview.RightAnchor).Active = true;
            TimeCollectionView.LeftAnchor.ConstraintEqualTo(TimeCollectionView.Superview.LeftAnchor, 0).Active = true;
            TimeCollectionView.BottomAnchor.ConstraintEqualTo(TimeCollectionView.Superview.BottomAnchor).Active = true;
            //TimeCollectionView.HeightAnchor.ConstraintEqualTo(400).Active = true;

            //TimeCollectionView.BackgroundColor = UIColor.Red;
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
                var file = "SaleReport_Branch_" + datenamefile;
                ReportManager.createDetail("SalesReportByBranch", file, reportSale, Typetime, null, null, null, null, null, null);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }


        }
    }
}