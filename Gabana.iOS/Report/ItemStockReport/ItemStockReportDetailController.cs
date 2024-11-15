using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
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

    public partial class ItemStockReportDetailController : UIViewController
    {
        UICollectionView StockCollectionView;

        UIView TopView;
        UILabel lblDate, lblDay, lblBranch, lblNameCate;

        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg;
        UILabel  lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare;

        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch,btnfilter;

        ItemManage setCate = new ItemManage();
        List<Item> ListItem = new List<Item>();
        ReportFilterController filterPage = null;


        public static int filterReport = 1;
        public static bool isModifyFilter = false;

        private static string queryType;

        private static List<int> lstsysitem;
        private static string  TextGroup;
        List<Gabana.ORM.Master.ItemOnBranch> itemOnBranches;

        public ItemStockReportDetailController(string v, List<int> l, string itemSelect)
        {
            queryType = v;
            lstsysitem = l;
            TextGroup = itemSelect;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (isModifyFilter)
            {
                ShowBalanceReport();
                isModifyFilter = false;
            }
        }
        public async override void ViewDidLoad()
        {
           
            try
            {
                View.BackgroundColor = UIColor.FromRGB(226,226,226);
                base.ViewDidLoad();
                initAttribute();
                SetupAutoLayout();
                setupData();
                ShowBalanceReport();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
            }

        }
        async void setupData()
        {

            Title = Utils.TextBundle("itemtotal_report", "Items");
            lblNameCate.Text = TextGroup;

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
            lblDay.Text = Utils.TextBundle("today", "Items")+",";
            lblDate.Text = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("en-US"));
        }
        async void ShowBalanceReport()
        {

            Gabana3.JAM.Report.ItemsBalanceStockRequest itemsBalanceStock = new Gabana3.JAM.Report.ItemsBalanceStockRequest()
            {
                queryType = queryType,
                sysBranchID = (int)ReportController.listChooseBranch[0].SysBranchID,
                sysIds = lstsysitem
            };

            var result = await GabanaAPI.GetDataReportBalanceItemsStock(itemsBalanceStock);
            if(result == null)
            {
                return;
            }
            itemOnBranches = new List<Gabana.ORM.Master.ItemOnBranch>(result);

            switch (filterReport)
            {
                case 1:
                    itemOnBranches = result.OrderByDescending(x => x.BalanceStock).ToList();
                    break;
                case 2:
                    itemOnBranches = result.OrderBy(x => x.BalanceStock).ToList();
                    break;
                case 3:
                    itemOnBranches = result.OrderByDescending(x => x.SysItemID).ToList();
                    break;
                case 4:
                    itemOnBranches = result.OrderBy(x => x.SysItemID).ToList();
                    break;
                default:
                    break;
            }
            if (result == null)
            {
                return;
            }

            ListItem = await setCate.GetAllItem();

            ReportItemStockDataSource report_adapter_itembalance = new ReportItemStockDataSource(itemOnBranches, ListItem);
            StockCollectionView.DataSource = report_adapter_itembalance;
            StockCollectionView.ReloadData();

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
                filterPage = new ReportFilterController(6);
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
            lblDate.Font = lblDate.Font.WithSize(14);
            TopView.AddSubview(lblDate);

            lblNameCate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNameCate.Font = lblNameCate.Font.WithSize(14);
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

            #region CustomerCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: View.Frame.Width, height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            StockCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            StockCollectionView.BackgroundColor = UIColor.White;
            StockCollectionView.ShowsVerticalScrollIndicator = false;
            StockCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            StockCollectionView.RegisterClassForCell(cellType: typeof(ItemTopSaleReportDataViewCell), reuseIdentifier: "ItemTopSaleReportDataViewCell");
            View.AddSubview(StockCollectionView);
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
        }
        #region bottom view toggle
        [Export("Print:")]
        public void Print(UIGestureRecognizer sender)
        {

        }
        [Export("PDF:")]
        public void PDF(UIGestureRecognizer sender)
        {
           
        }
        [Export("EMail:")]
        public void EMail(UIGestureRecognizer sender)
        {

        }
        [Export("Share:")]
        public void Share(UIGestureRecognizer sender)
        {

        }

        #endregion
        async void SearchBytxt()
        {
            // var items = await GetFilterItemList();
            var items = await setCate.GetAllItem();

            ReportItemStockDataSource report_adapter_itembalance = new ReportItemStockDataSource(itemOnBranches, items);
            StockCollectionView.DataSource = report_adapter_itembalance;
            StockCollectionView.ReloadData();
        }
        async Task<List<Item>> GetFilterItemList()
        {
            try
            {
                var item = await setCate.GetAllItem();
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var search = item.Where(m => m.ItemName.Contains(txtSearch.Text)).ToList();
                    return search;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
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

            lblDay.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor,-10).Active = true;
            lblDay.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDay.LeftAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblDay.WidthAnchor.ConstraintEqualTo(85).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor,-10).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.LeftAnchor,-10).Active = true;

            lblNameCate.TopAnchor.ConstraintEqualTo(lblDay.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            lblNameCate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblNameCate.LeftAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNameCate.RightAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.RightAnchor,-20).Active = true;

            lblBranch.CenterYAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.CenterYAnchor,-10).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            #endregion

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
            StockCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            StockCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            StockCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            StockCollectionView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor,-10).Active = true;
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