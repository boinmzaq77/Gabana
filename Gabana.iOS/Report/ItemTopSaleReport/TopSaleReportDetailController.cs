using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using Gabana3.JAM.Report;
using Microcharts;

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

    public partial class TopSaleReportDetailController : UIViewController
    {
        UICollectionView TopSaleCollectionView;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch, lblNameCate;

        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch, btnfilter;

        ItemManage setCate = new ItemManage();
        List<Item> ListItem = new List<Item>();
        ReportFilterController filterPage = null;
        private UIView Viewnull;
        private UIImageView imgnull;
        private UILabel lblnull;
        List<Item> ItemTopsaleResponses;

        int filterDateReport = 0;

        public static int filterReport = 1;
        int Reporttype = 0;
        private string filename;
        public static bool isModifyFilter = false;

        string startdate = "";
        string enddate = "";
        string nameCus = "";
        private List<SummaryItemModel> SummaryItemModel;
        private List<SummaryItemModel> summaryItems;
        private List<Item> items;
        private string datenamefile;

        public TopSaleReportDetailController(string name, int filter, int reporttypu, string start, string end, string filename)
        {
            this.nameCus = name;
            this.filterDateReport = filter;
            this.startdate = start;
            this.enddate = end;
            this.Reporttype = reporttypu;
            this.filename = filename;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (isModifyFilter)
            {
                setupCollection();
                isModifyFilter = false;
            }

        }
        public async override void ViewDidLoad()
        {

            try
            {
                View.BackgroundColor = UIColor.FromRGB(226, 226, 226);
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
            try { 
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
            }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        async void setupCollection()
        {
            try
            {

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
                SummaryItemModel = await GabanaAPI.GetDataReportSummaryDailyItem(sysbranIdSelect, startdate, enddate);
            summaryItems = new List<Gabana3.JAM.Report.SummaryItemModel>();
            switch (Reporttype)
            {
                case 0: // sell
                    //Title = "รายงานสินค้าขายดีตามยอดขาย";
                    //summaryItems = SummaryItemModel.OrderByDescending(x => x.SumTotalAmount).ToList();
                        switch (filterReport)
                        {
                            case 1:
                                summaryItems = SummaryItemModel.OrderByDescending(x => x.SumTotalAmount).ToList();
                                break;
                            case 2:
                                summaryItems = SummaryItemModel.OrderBy(x => x.SumTotalAmount).ToList();
                                break;
                            case 3:
                                summaryItems = SummaryItemModel.OrderByDescending(x => x.ItemName).ToList();
                                break;
                            case 4:
                                summaryItems = SummaryItemModel.OrderBy(x => x.ItemName).ToList();
                                break;
                            default:
                                break;
                        }

                        break;
                case 1: //unit - quantity
                    //Title = "รายงานสินค้าขายดีตามจำนวน";
                    //summaryItems = SummaryItemModel.OrderByDescending(x => x.SumQuantity).ToList();
                        switch (filterReport)
                        {
                            case 1:
                                summaryItems = SummaryItemModel.OrderByDescending(x => x.SumQuantity).ToList();
                                break;
                            case 2:
                                summaryItems = SummaryItemModel.OrderBy(x => x.SumQuantity).ToList();
                                break;
                            case 3:
                                summaryItems = SummaryItemModel.OrderByDescending(x => x.ItemName).ToList();
                                break;
                            case 4:
                                summaryItems = SummaryItemModel.OrderBy(x => x.ItemName).ToList();
                                break;
                            default:
                                break;
                        }
                        break;
                default:
                    break;
                }

                switch (filterReport)
                {
                    case 1:
                        summaryItems = SummaryItemModel.OrderByDescending(x => x.SumTotalAmount).ToList();
                        break;
                    case 2:
                        summaryItems = SummaryItemModel.OrderBy(x => x.SumTotalAmount).ToList();
                        break;
                    case 3:
                        summaryItems = SummaryItemModel.OrderByDescending(x => x.ItemName).ToList();
                        break;
                    case 4:
                        summaryItems = SummaryItemModel.OrderBy(x => x.ItemName).ToList();
                        break;
                    default:
                        break;
                }
                if (summaryItems == null || summaryItems.Count == 0)
                {
                    btnfilter.Enabled = false;
                    //SearchbarView.Hidden = true;
                    TopSaleCollectionView.Hidden = true;
                    BottomView.Hidden = true;
                    Viewnull.Hidden = false;
                    return;
                }
                else
                {
                    //SearchbarView.Hidden = false;
                    TopSaleCollectionView.Hidden = false;
                    BottomView.Hidden = false;
                    Viewnull.Hidden = true;
                }
                 items = new List<Item>();
            ItemManage itemManage = new ItemManage();
            items = await itemManage.GetAllItem();

            ReportItemTopSaleDataSource report_adapter = new ReportItemTopSaleDataSource(summaryItems, items, Reporttype);
            TopSaleCollectionView.DataSource = report_adapter;
            TopSaleCollectionView.ReloadData();
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
                filterPage = new ReportFilterController(5);
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
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDate.Font = lblDate.Font.WithSize(14);
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



            #region CustomerCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            TopSaleCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            TopSaleCollectionView.BackgroundColor = UIColor.White;
            TopSaleCollectionView.ShowsVerticalScrollIndicator = false;
            TopSaleCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            TopSaleCollectionView.RegisterClassForCell(cellType: typeof(ItemTopSaleReportDataViewCell), reuseIdentifier: "ItemTopSaleReportDataViewCell");
            View.AddSubview(TopSaleCollectionView);
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
            View.AddSubview(BottomView);

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
                var r = new UIGraphicsImageRenderer(TopSaleCollectionView.Bounds.Size);


                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    TopSaleCollectionView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                var r2 = new UIGraphicsPdfRenderer(TopSaleCollectionView.Bounds, UIGraphicsPdfRendererFormat.DefaultFormat);
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
                var r = new UIGraphicsImageRenderer(TopSaleCollectionView.Bounds.Size);
                var img = r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
                {
                    TopSaleCollectionView.Layer.RenderInContext(ctxt.CGContext);
                    //View.Capture(true);
                });
                ////var img = View.Capture(true);
                uIImage = img;
            }
            else
            {
                UIGraphics.BeginImageContextWithOptions(TopSaleCollectionView.Bounds.Size, TopSaleCollectionView.Opaque, 0);
                TopSaleCollectionView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
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
                var filename = "BestItemReport_";
                if (Reporttype==0)
                {
                    filename += "BySale_";
                }
                else
                {
                    filename += "ByQuantity_";
                }
                filename += datenamefile;
                

                ReportManager.createDetail("ReportBestSale", filename, SummaryItemModel, null, null, null, null, null, null, lblBranch.Text);




            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }


        }

        #endregion
        async void SearchBytxt()
        {
            var Cus = await GetFilterItemList();
            ((ReportItemTopSaleDataSource)TopSaleCollectionView.DataSource).ReloadData(Cus);
            TopSaleCollectionView.ReloadData();
        }
        async Task<List<SummaryItemModel>> GetFilterItemList()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text)) 
                {
                    var itemlst = summaryItems.Where(m => m.ItemName.Contains(txtSearch.Text)).ToList();
                    return itemlst;
                }
                else
                {
                    return summaryItems;
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
            TopView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDay.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblDay.WidthAnchor.ConstraintEqualTo(85).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;


            lblBranch.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
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

            #region TopSaleCollectionView
            TopSaleCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            TopSaleCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            TopSaleCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopSaleCollectionView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
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