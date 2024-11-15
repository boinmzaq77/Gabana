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
    
    public partial class ProfitReportTodayController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        UICollectionView TimeCollectionView;

        UIView timeView, TimeHeadView;
        UILabel lblHours, lblSale;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch;

        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView GraphView;
        UILabel lbl_GraphHead;
        ChartView SaleChartView;
        ReportSale reportSale;
        List<Gabana.ORM.Period.SummaryHourly> GetDataReportSummary;
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
        private List<ReportHourly> report;
        private string datenamefile;

        public ProfitReportTodayController()
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
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
            }

        }
        private async void Search()
        {
            var result = await SearchBytxt(txtSearch.Text);
            ReportSaleHourlyDataSource report_adapter_showHour = new ReportSaleHourlyDataSource(result);
            TimeCollectionView.DataSource = report_adapter_showHour;
            TimeCollectionView.ReloadData();
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
        internal async void setupData()
        {
            try
            {

           
            lblDay.Text = Utils.TextBundle("today", "Items")+", ";
            lblDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                datenamefile = DateTime.Now.ToString("DD-MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
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
                //graph
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

                var latestTranDate = DateTime.UtcNow;
            var datenow = Utils.ChangeDateTimeReport(latestTranDate);

            GetDataReportSummary = await GabanaAPI.GetDataReportSummaryHourly(sysbranIdSelect, datenow, datenow);

            reportSale = new ReportSale();
            if (GetDataReportSummary != null && GetDataReportSummary.Count != 0)
            {
                    reportSale = await CalReport.CalReportProfit(GetDataReportSummary);
                    List<ChartEntry> SalesByPeriod = new List<ChartEntry>();
                    var result = reportSale.reportHourlies;
                    lbl_total.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + result.Sum(x => x.value).ToString("#,##0.00");
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
                    TimeCollectionView.DataSource = report_adapter_showHour;
                    TimeCollectionView.ReloadData();
                    var chart = new LineChart() { Entries = SalesByPeriod, LabelTextSize = 20, BackgroundColor = SKColor.Parse("#FFFFFF"), LineMode = LineMode.Straight };
                    SaleChartView.Chart = chart;
                    Utils.SetConstant(Viewnull.Constraints, NSLayoutAttribute.Height, 0);
                }
                else
                {
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
            catch (Exception ex )
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

            #region GraphView
            GraphView = new UIView();
            GraphView.BackgroundColor = UIColor.White;
            GraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_GraphHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_GraphHead.Text =  Utils.TextBundle("profitsbyhour", "Items");
            lbl_GraphHead.Font = lbl_GraphHead.Font.WithSize(15);
            GraphView.AddSubview(lbl_GraphHead);

            SaleChartView = new ChartView();
            SaleChartView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleChartView.BackgroundColor = UIColor.White;
            GraphView.AddSubview(SaleChartView);
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
                var filterPage = new ReportFilter2Controller(4);
                this.NavigationController.PushViewController(filterPage, false);
            };
            SearchbarView.AddSubview(btnfilter);
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
            lblHours.Text = Utils.TextBundle("hour", "Items");
            TimeHeadView.AddSubview(lblHours);

            lblSale = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(112, 112, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSale.Font = lblSale.Font.WithSize(15);
            lblSale.Text = Utils.TextBundle("profit", "Items");
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
            TimeCollectionView.RegisterClassForCell(cellType: typeof(HourSaleViewCell), reuseIdentifier: "HourSaleViewCell");
            timeView.AddSubview(TimeCollectionView);
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

            _contentView.AddSubview(timeView);
            _contentView.AddSubview(TopView);
            _contentView.AddSubview(GraphView);
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
                ReportManager.createDetail("ProfitReport", file, reportSale, "Hourly", "Hourly", null, null, GetDataReportSummary, ReportController.listChooseBranch[0].SysBranchID.ToString(), lblBranch.Text);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }


        }

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
            #region GraphView
            GraphView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            GraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;
            GraphView.LeftAnchor.ConstraintEqualTo(GraphView.Superview.LeftAnchor, 0).Active = true;
            GraphView.RightAnchor.ConstraintEqualTo(GraphView.Superview.RightAnchor, 0).Active = true;

            lbl_GraphHead.TopAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.TopAnchor, 12).Active = true;
            lbl_GraphHead.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_GraphHead.LeftAnchor.ConstraintEqualTo(lbl_GraphHead.Superview.LeftAnchor, 15).Active = true;
            lbl_GraphHead.WidthAnchor.ConstraintEqualTo(120).Active = true;

            SaleChartView.TopAnchor.ConstraintEqualTo(lbl_GraphHead.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            SaleChartView.RightAnchor.ConstraintEqualTo(SaleChartView.Superview.RightAnchor).Active = true;
            SaleChartView.LeftAnchor.ConstraintEqualTo(SaleChartView.Superview.LeftAnchor, 0).Active = true;
            SaleChartView.BottomAnchor.ConstraintEqualTo(SaleChartView.Superview.BottomAnchor).Active = true;
            #endregion

            #region totalView
            totalView.TopAnchor.ConstraintEqualTo(GraphView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
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

            #region TimeView
            timeView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            timeView.RightAnchor.ConstraintEqualTo(timeView.Superview.RightAnchor).Active = true;
            timeView.LeftAnchor.ConstraintEqualTo(timeView.Superview.LeftAnchor, 0).Active = true;
            timeView.HeightAnchor.ConstraintEqualTo(990).Active = true;
            timeView.BottomAnchor.ConstraintEqualTo(timeView.Superview.BottomAnchor).Active = true;

            #region TimeHead
            TimeHeadView.TopAnchor.ConstraintEqualTo(TimeHeadView.Superview.TopAnchor, 0).Active = true;
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
    }
}