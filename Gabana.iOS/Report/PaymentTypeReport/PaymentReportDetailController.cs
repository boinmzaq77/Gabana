using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Report;
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

    public partial class PaymentReportDetailController : UIViewController
    {
        UICollectionView PaymentCollectionView;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch,lblNameCate;

        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel  lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch,btnfilter;

        TranPaymentManage setCate = new TranPaymentManage();
        List<PaymentType> ListPayment = new List<PaymentType>();
        ReportFilterController filterPage = null;

        List<SalesByPaymentResponse> paymentResponses;
        private List<SalesByPaymentResponse> result;
        int filterDateReport = 0;

        public static int filterReport = 1;
        public static bool isModifyFilter = false;

        string startdate = "";
        string enddate = "";
        private string filename;
        string nameCus = "";
        private UIView Viewnull;
        private UIImageView imgnull;
        private UILabel lblnull;
        ChartView PaymentChart;
        private string datenamefile;

        public PaymentReportDetailController(List<PaymentType> cater,string name, int filter,string start , string end ,string filename )
        {
            this.ListPayment = cater;
            this.nameCus = name;
            this.filterDateReport = filter;
            this.startdate = start;
            this.enddate = end;
            this.filename = filename;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if(isModifyFilter)
            {
                setupCollection();
                isModifyFilter = false;
            }
            
        }
        public async override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.FromRGB(248,248,248);
            try
            {
                base.ViewDidLoad();
                initAttribute();
                SetupAutoLayout();
                setupData();
                setupCollection();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
            }

        }
        async void setupData()
        {
            string namebranch = "";
            if (ReportController.listChooseBranch.Count == ReportController.listAllBranch.Count)
            {
                namebranch = "All branch";
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
            if (filterDateReport == 1) //today
            {
                lblDay.Text = Utils.TextBundle("today", "Items") + ",";
                lblDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                datenamefile = DateTime.Now.ToString("DD-MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else if (filterDateReport == 2) //Month
            {
                lblDay.Text = Utils.TextBundle("thismonth", "This Month") + ",";
                lblDate.Text = DateTime.Now.ToString("MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                datenamefile = DateTime.Now.ToString("MM-yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else if (filterDateReport == 3) //Year
            {
                lblDay.Text = Utils.TextBundle("thisyear", "This Year") + ",";
                lblDate.Text = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                datenamefile = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else
            {
                lblDay.Text = Utils.TextBundle("custom", "Custom") + ",";
                lblDate.Text = nameCus;
                datenamefile = filename;
            }
            //listChooseCategory



        }
        async void setupCollection()
        {
            try
            {

            
            lblNameCate.Text = "";
            List<string> customerName = new List<string>();
            if(ListPayment.Count == 5)
            {
                lblNameCate.Text += Utils.TextBundle("allpayment", "Items");
                foreach (var item in ListPayment)
                {
                    customerName.Add(item.Type);
                }
            }
            else
            {
                foreach (var item in ListPayment)
                {
                    customerName.Add(item.Type);
                    if (lblNameCate.Text != "")
                    {
                        lblNameCate.Text += "," + item.Detail ;
                    }
                    else
                    {
                        lblNameCate.Text = item.Detail;
                    }
                }
            }
            
            Gabana3.JAM.Report.SalesByPaymentRequest empRequest = new Gabana3.JAM.Report.SalesByPaymentRequest();
            empRequest.sysBranchIDs = new List<int>(ReportController.listChooseBranch.Select(x => int.Parse(x.BranchID)).ToList());
            empRequest.startDate = this.startdate;
            empRequest.endDate = this.enddate;
            empRequest.paymentTypes = customerName;

             paymentResponses = new List<SalesByPaymentResponse>();
            result = await GabanaAPI.GetDataReportSummaryDailyPayment(empRequest);
                if (result == null || result.Count == 0)
                {
                    btnfilter.Enabled = false;
                    SearchbarView.Hidden = true;
                    PaymentCollectionView.Hidden = true;
                    BottomView.Hidden = true;
                    Viewnull.Hidden = false;
                    return;
                }
                else
                {
                    SearchbarView.Hidden = false;
                    PaymentCollectionView.Hidden = false;
                    BottomView.Hidden = false;
                    Viewnull.Hidden = true;
                }
                btnfilter.Enabled = true;
            switch (filterReport)
            {
                case 1:
                    paymentResponses = result.OrderByDescending(x => x.sumTotalAmount).ToList();
                    break;
                case 2:
                    paymentResponses = result.OrderBy(x => x.sumTotalAmount).ToList();
                    break;
                case 3:
                    paymentResponses = result.OrderByDescending(x => x.paymentType).ToList();
                    break;
                case 4:
                    paymentResponses = result.OrderBy(x => x.paymentType).ToList();
                    break;
                default:
                    break;
            }

            List<string> color = new List<string>();
            color.Add("#0095DA");
            color.Add("#E32D49");
            color.Add("#37AA52");
            color.Add("#F8971D");
            color.Add("#F75600");
            List<ChartEntry> SalesByPayments = new List<ChartEntry>();
            foreach (var item in paymentResponses)
            {
                    string colors = ""; 
                    switch (item.paymentType)
                    {
                        case "CH":
                            colors = "#0095DA";
                            break;
                        case "MYQR":
                            colors = "#F75600";
                            break;
                        case "GV":
                            colors = "#37AA52";
                            break;
                        case "Cr":
                            colors = "#F8971D";
                            break;
                        case "Dr":
                            colors = "#E32D49";
                            break;
                        default:
                            colors = "#A2A2A2";
                            break;
                    }
                    ChartEntry entry = new ChartEntry((float)item.percentTotal)
                {
                    //Label = Utils.SetPaymentName(item.paymentType.ToString()),
                    //ValueLabel = item.percentTotal.ToString() + "%",
                    Color = SKColor.Parse(colors),
                    //ValueLabelColor = SKColor.Parse(color[i])
                };

                SalesByPayments.Add(entry);
            }
            var chart2 = new DonutChart() { Entries = SalesByPayments, LabelTextSize = 30, HoleRadius = 0.7f };
            PaymentChart.Chart = chart2;

            ReportPaymentDataSource report_adapter = new ReportPaymentDataSource(paymentResponses, ListPayment);
            PaymentCollectionView.DataSource = report_adapter;
            PaymentCollectionView.ReloadData();

            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }

        }
        void initAttribute()
        {
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
                SearchBytxt();
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
                    SearchBytxt();
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
                //filter
                filterPage = new ReportFilterController(4);
                this.NavigationController.PushViewController(filterPage,false);
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
            lblDate.Font = lblDate.Font.WithSize(14);
            TopView.AddSubview(lblDate);

            lblNameCate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNameCate.Font = lblNameCate.Font.WithSize(15);
            TopView.AddSubview(lblNameCate);

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

            PaymentChart = new ChartView();
            PaymentChart.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentChart.BackgroundColor = UIColor.White;
            View.AddSubview(PaymentChart);

            #region CustomerCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 40);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            PaymentCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            PaymentCollectionView.BackgroundColor = UIColor.White;
            PaymentCollectionView.ShowsVerticalScrollIndicator = false;
            PaymentCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentCollectionView.RegisterClassForCell(cellType: typeof(PaymentReportDataViewCell), reuseIdentifier: "PaymentReportDataViewCell");
            View.AddSubview(PaymentCollectionView);
            #endregion

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
            View.AddSubview(TopView);
            Viewnull = new UIView();
            Viewnull.TranslatesAutoresizingMaskIntoConstraints = false;
            //Viewnull.Hidden = true;
            View.AddSubview(Viewnull);

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
                var r = new UIGraphicsImageRenderer(PaymentCollectionView.Bounds.Size);


                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    PaymentCollectionView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                var r2 = new UIGraphicsPdfRenderer(PaymentCollectionView.Bounds, UIGraphicsPdfRendererFormat.DefaultFormat);
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
                var r = new UIGraphicsImageRenderer(PaymentCollectionView.Bounds.Size);
                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    PaymentCollectionView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                ////var img = View.Capture(true);
                uIImage = img;
            }
            else
            {
                UIGraphics.BeginImageContextWithOptions(PaymentCollectionView.Bounds.Size, PaymentCollectionView.Opaque, 0);
                PaymentCollectionView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
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
                var file = "SaleReport_Payment_" + datenamefile; ;
                ReportManager.createDetail("PaymentReport", file, result, null, null, null, null, null, null, lblBranch.Text);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }
        }

        #endregion
        async void SearchBytxt()
        {
            var Cus = await GetFilterCustomerList();
            ((ReportPaymentDataSource)PaymentCollectionView.DataSource).ReloadData(Cus);
            PaymentCollectionView.ReloadData();
        }
        async Task<List<SalesByPaymentResponse>> GetFilterCustomerList()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = this.paymentResponses.Where(m => m.paymentType.Contains(txtSearch.Text)).ToList();
                    return itemlst;
                }
                else
                {
                    return paymentResponses;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
                return null;
            }
        }
        void SetupAutoLayout()
        {
            #region TopView
            TopView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDay.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblDay.WidthAnchor.ConstraintEqualTo(85).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.LeftAnchor,-10).Active = true;

            lblNameCate.TopAnchor.ConstraintEqualTo(lblDate.BottomAnchor, 2).Active = true;
            lblNameCate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblNameCate.LeftAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNameCate.RightAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.RightAnchor,-15).Active = true;

            lblBranch.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            //  lblBranch.WidthAnchor.ConstraintEqualTo(150).Active = true;

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
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
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

            PaymentChart.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            PaymentChart.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor,-10).Active = true;
            PaymentChart.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            PaymentChart.HeightAnchor.ConstraintEqualTo(150).Active = true;

            #region PaymentCollectionView
            PaymentCollectionView.TopAnchor.ConstraintEqualTo(PaymentChart.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            PaymentCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            PaymentCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PaymentCollectionView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor,-10).Active = true;
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