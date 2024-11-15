using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class DashBoardController : UIViewController
    {
        BranchSettingController branchPage = null;
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
        UIView cashColorView, CardColorView, GiftColorView, ePayColorView;
        UILabel lblSaleByType,lblSaleByCash, lblSaleByCard, lblSaleByGift, lblSaleByePayment;
        UILabel lblSaleByCashValue, lblSaleByCardValue, lblSaleByGiftValue, lblSaleByePaymentValue;

        UIView BestSellingView;
        UILabel lblBestSelling;
        UICollectionView BestSellingItemCollection;

        UIView BestEmployeeView;
        UILabel lblBestEmployee;
        UICollectionView BestEmployeeCollection;

        public DashBoardController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.Title = "Dash Board";

            BranchManage branchManage = new BranchManage();
            int sysbranch = Convert.ToInt32(Preferences.Get("branch", ""));
            var listBranch = await branchManage.GetBranch((int)MainController.merchantlocal.MerchantID, sysbranch);

            UIBarButtonItem selectBranchBtn = new UIBarButtonItem();
            selectBranchBtn.Title = listBranch.BranchName;
            selectBranchBtn.Clicked += (sender, e) => {
                // open select customer page
                if (branchPage == null)
                {
                    branchPage = new BranchSettingController();
                }
                this.NavigationController.PushViewController(branchPage, false);
            };
            this.NavigationItem.RightBarButtonItem = selectBranchBtn;

            initAttribute();
            setupAutoLayout();
            // View.BackgroundColor = UIColor.FromRGB(248,248,248);
            View.BackgroundColor = UIColor.Red;
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region TopView
            TopView = new UIView();
            TopView.BackgroundColor = UIColor.White;
           // TopView.Layer.CornerRadius = 5;
            TopView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblTextDay = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextDay.Font = lblTextDay.Font.WithSize(15);
            lblTextDay.Text = "Day";
            TopView.AddSubview(lblTextDay);

            lblTextDate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextDate.Font = lblTextDate.Font.WithSize(15);
            lblTextDate.Text = "01 Apr 2021";
            TopView.AddSubview(lblTextDate);

            #endregion
            #region SaleTotalView
            SaleTotalView = new UIView();
            SaleTotalView.Layer.CornerRadius = 5;
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

            lblTextSaleTotal= new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleTotal.Font = lblTextSaleTotal.Font.WithSize(13);
            lblTextSaleTotal.Text = "Sales Total";
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
            SaleCountView.ClipsToBounds = true;
            SaleCountView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleCountTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleCountTotal.Font = lblSaleCountTotal.Font.WithSize(15);
            lblSaleCountTotal.Text = "999";
            SaleCountView.AddSubview(lblSaleCountTotal);

            lblTextSaleCountTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleCountTotal.Font = lblTextSaleCountTotal.Font.WithSize(12);
            lblTextSaleCountTotal.Text = "Sales Count";
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
            SaleProfitView.BackgroundColor = UIColor.White;
            SaleProfitView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleProfitTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleProfitTotal.Font = lblSaleProfitTotal.Font.WithSize(15);
            lblSaleProfitTotal.Text = "฿ 999,999";
            SaleProfitView.AddSubview(lblSaleProfitTotal);

            lblTextSaleProfitTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleProfitTotal.Font = lblTextSaleProfitTotal.Font.WithSize(12);
            lblTextSaleProfitTotal.Text = "Profit";
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
            SaleAverageView.BackgroundColor = UIColor.White;
            SaleAverageView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleAvgTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleAvgTotal.Font = lblSaleAvgTotal.Font.WithSize(15);
            lblSaleAvgTotal.Text = "฿ 999,999";
            SaleAverageView.AddSubview(lblSaleAvgTotal);

            lblTextSaleAvgTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleAvgTotal.Font = lblTextSaleAvgTotal.Font.WithSize(12);
            lblTextSaleAvgTotal.Text = "Average Sale";
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
            SaleComparisonView.BackgroundColor = UIColor.White;
            SaleComparisonView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleComparisonTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(0, 149, 218),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleComparisonTotal.Font = lblSaleComparisonTotal.Font.WithSize(15);
            lblSaleComparisonTotal.Text = "฿ 999,999";
            SaleComparisonView.AddSubview(lblSaleComparisonTotal);

            lblTextSaleComparisonTotal = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextSaleComparisonTotal.Font = lblTextSaleComparisonTotal.Font.WithSize(12);
            lblTextSaleComparisonTotal.Text = "Comparison";
            SaleComparisonView.AddSubview(lblTextSaleComparisonTotal);

            SaleComparisonIcon = new UIImageView();
            SaleComparisonIcon.BackgroundColor = UIColor.White;
            SaleComparisonIcon.Image = UIImage.FromBundle("DbCompare"); // wait for calender icon
            SaleComparisonIcon.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleComparisonView.AddSubview(SaleComparisonIcon);
            #endregion
            #region SalePeriodGraphView
            SalePeriodGraphView = new UIView();
            SalePeriodGraphView.BackgroundColor = UIColor.White;
            SalePeriodGraphView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblPeriodGraph = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247,86,0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblPeriodGraph.Font = lblPeriodGraph.Font.WithSize(15);
            lblPeriodGraph.Text = "Sales By Period";
            SalePeriodGraphView.AddSubview(lblPeriodGraph);
            #endregion
            #region SaleByTypeView
            SaleByTypeView = new UIView();
            SaleByTypeView.BackgroundColor = UIColor.White;
            SaleByTypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblSaleByType = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByType.Font = lblSaleByType.Font.WithSize(15);
            lblSaleByType.Text = "Sales By Payment Type";
            SaleByTypeView.AddSubview(lblSaleByType);

            cashColorView = new UIView();
            cashColorView.BackgroundColor = UIColor.FromRGB(0,149,218);
            cashColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleByTypeView.AddSubview(cashColorView);

            lblSaleByCash= new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112,112,112),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByCash.Font = lblSaleByCash.Font.WithSize(15);
            lblSaleByCash.Text = "Cash";
            SaleByTypeView.AddSubview(lblSaleByCash);

            lblSaleByCashValue = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64,64,64),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByCashValue.Font = lblSaleByCashValue.Font.WithSize(15);
            lblSaleByCashValue.Text = "30%";
            SaleByTypeView.AddSubview(lblSaleByCashValue);

            CardColorView = new UIView();
            CardColorView.BackgroundColor = UIColor.FromRGB(227,45,73);
            CardColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleByTypeView.AddSubview(CardColorView);

            lblSaleByCard = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByCard.Font = lblSaleByCard.Font.WithSize(15);
            lblSaleByCard.Text = "Credit/Debit \nCard";
            lblSaleByCard.Lines = 2;
            SaleByTypeView.AddSubview(lblSaleByCard);

            lblSaleByCardValue = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64, 64, 64),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByCardValue.Font = lblSaleByCardValue.Font.WithSize(15);
            lblSaleByCardValue.Text = "20%";
            SaleByTypeView.AddSubview(lblSaleByCardValue);

            GiftColorView = new UIView();
            GiftColorView.BackgroundColor = UIColor.FromRGB(55,170,82);
            GiftColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleByTypeView.AddSubview(GiftColorView);

            lblSaleByGift = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByGift.Font = lblSaleByGift.Font.WithSize(15);
            lblSaleByGift.Text = "Gift Voucher";
            SaleByTypeView.AddSubview(lblSaleByGift);

            lblSaleByGiftValue = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64, 64, 64),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByGiftValue.Font = lblSaleByGiftValue.Font.WithSize(15);
            lblSaleByGiftValue.Text = "20%";
            SaleByTypeView.AddSubview(lblSaleByGiftValue);

            ePayColorView = new UIView();
            ePayColorView.BackgroundColor = UIColor.FromRGB(248,151,29);
            ePayColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleByTypeView.AddSubview(ePayColorView);

            lblSaleByePayment = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(112, 112, 112),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByePayment.Font = lblSaleByePayment.Font.WithSize(15);
            lblSaleByePayment.Text = "ePayment";
            SaleByTypeView.AddSubview(lblSaleByePayment);

            lblSaleByePaymentValue = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64, 64, 64),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblSaleByePaymentValue.Font = lblSaleByePaymentValue.Font.WithSize(15);
            lblSaleByePaymentValue.Text = "10%";
            SaleByTypeView.AddSubview(lblSaleByePaymentValue);

            #endregion

            #region BestSellingView
            BestSellingView = new UIView();
            BestSellingView.BackgroundColor = UIColor.White;
            BestSellingView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblBestSelling = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBestSelling.Font = lblBestSelling.Font.WithSize(15);
            lblBestSelling.Text = "Best Selling Items";
            BestSellingView.AddSubview(lblBestSelling);

            // BestSellingItemCollection
            #endregion

            #region BestEmployeeView
            BestEmployeeView = new UIView();
            BestEmployeeView.BackgroundColor = UIColor.White;
            BestEmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblBestEmployee = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),

                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBestEmployee.Font = lblBestEmployee.Font.WithSize(15);
            lblBestEmployee.Text = "Best Employee";
            BestEmployeeView.AddSubview(lblBestEmployee);
            #endregion

            _contentView.AddSubview(TopView);
            _contentView.AddSubview(SaleTotalView);
            _contentView.AddSubview(SaleCountView);
            _contentView.AddSubview(SaleProfitView);
            _contentView.AddSubview(SaleAverageView);
            _contentView.AddSubview(SaleComparisonView);
            _contentView.AddSubview(SalePeriodGraphView);
            _contentView.AddSubview(SaleByTypeView);
            _contentView.AddSubview(BestSellingView);
            _contentView.AddSubview(BestEmployeeView);


            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
        }
        void setupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
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
            TopView.TopAnchor.ConstraintEqualTo(TopView.Superview.TopAnchor, 1).Active = true;
            TopView.LeftAnchor.ConstraintEqualTo(TopView.Superview.LeftAnchor, 0).Active = true;
            TopView.RightAnchor.ConstraintEqualTo(TopView.Superview.RightAnchor, 0).Active = true;
            TopView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblTextDay.CenterYAnchor.ConstraintEqualTo(lblTextDay.Superview.CenterYAnchor, 0).Active = true;
            lblTextDay.LeftAnchor.ConstraintEqualTo(lblTextDay.Superview.LeftAnchor, 20).Active = true;
            lblTextDay.HeightAnchor.ConstraintEqualTo(80).Active = true;

            lblTextDate.CenterYAnchor.ConstraintEqualTo(lblTextDate.Superview.CenterYAnchor, 0).Active = true;
            lblTextDate.LeftAnchor.ConstraintEqualTo(lblTextDay.SafeAreaLayoutGuide.RightAnchor,6).Active = true;
            lblTextDate.HeightAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region SaleTotalView
            SaleTotalView.TopAnchor.ConstraintEqualTo(TopView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleTotalView.LeftAnchor.ConstraintEqualTo(SaleTotalView.Superview.LeftAnchor, 10).Active = true;
            SaleTotalView.RightAnchor.ConstraintEqualTo(SaleTotalView.Superview.RightAnchor, -10).Active = true;
        //    SaleTotalView.BottomAnchor.ConstraintEqualTo(SaleTotalView.Superview.BottomAnchor, 0).Active = true;
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
          //  SaleComparisonView.BottomAnchor.ConstraintEqualTo(SaleComparisonView.Superview.BottomAnchor,0).Active = true;
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
     //       SalePeriodGraphView.BottomAnchor.ConstraintEqualTo(SalePeriodGraphView.Superview.BottomAnchor, 0).Active = true;
            SalePeriodGraphView.HeightAnchor.ConstraintEqualTo(210).Active = true;

            lblPeriodGraph.TopAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.TopAnchor, 12).Active = true;
            lblPeriodGraph.LeftAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.LeftAnchor, 15).Active = true;
            lblPeriodGraph.RightAnchor.ConstraintEqualTo(lblPeriodGraph.Superview.RightAnchor, -15).Active = true;
            lblPeriodGraph.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region SaleByTypeView
            SaleByTypeView.TopAnchor.ConstraintEqualTo(SalePeriodGraphView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            SaleByTypeView.LeftAnchor.ConstraintEqualTo(SaleByTypeView.Superview.LeftAnchor, 0).Active = true;
            SaleByTypeView.RightAnchor.ConstraintEqualTo(SaleByTypeView.Superview.RightAnchor, 0).Active = true;
         //   SaleByTypeView.BottomAnchor.ConstraintEqualTo(SaleByTypeView.Superview.BottomAnchor, 0).Active = true;
            SaleByTypeView.HeightAnchor.ConstraintEqualTo(210).Active = true;

            lblSaleByType.TopAnchor.ConstraintEqualTo(lblSaleByType.Superview.TopAnchor, 12).Active = true;
            lblSaleByType.LeftAnchor.ConstraintEqualTo(lblSaleByType.Superview.LeftAnchor, 15).Active = true;
            lblSaleByType.RightAnchor.ConstraintEqualTo(lblSaleByType.Superview.RightAnchor, -15).Active = true;
            lblSaleByType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            cashColorView.TopAnchor.ConstraintEqualTo(lblSaleByType.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            cashColorView.LeftAnchor.ConstraintEqualTo(cashColorView.Superview.CenterXAnchor, 0).Active = true;
            cashColorView.WidthAnchor.ConstraintEqualTo(12).Active = true;
            cashColorView.HeightAnchor.ConstraintEqualTo(12).Active = true;

            lblSaleByCashValue.TopAnchor.ConstraintEqualTo(lblSaleByType.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            lblSaleByCashValue.RightAnchor.ConstraintEqualTo(lblSaleByCashValue.Superview.RightAnchor, -15).Active = true;
            lblSaleByCashValue.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblSaleByCashValue.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblSaleByCash.TopAnchor.ConstraintEqualTo(lblSaleByType.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            lblSaleByCash.LeftAnchor.ConstraintEqualTo(cashColorView.SafeAreaLayoutGuide.RightAnchor, 18).Active = true;
            lblSaleByCash.RightAnchor.ConstraintEqualTo(lblSaleByCashValue.SafeAreaLayoutGuide.LeftAnchor,-5).Active = true;
            lblSaleByCash.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //-----------------------------------------------------------------------------------

            CardColorView.TopAnchor.ConstraintEqualTo(cashColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            CardColorView.LeftAnchor.ConstraintEqualTo(CardColorView.Superview.CenterXAnchor, 0).Active = true;
            CardColorView.WidthAnchor.ConstraintEqualTo(12).Active = true;
            CardColorView.HeightAnchor.ConstraintEqualTo(12).Active = true;

            lblSaleByCardValue.TopAnchor.ConstraintEqualTo(cashColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            lblSaleByCardValue.RightAnchor.ConstraintEqualTo(lblSaleByCardValue.Superview.RightAnchor, -15).Active = true;
            lblSaleByCardValue.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblSaleByCardValue.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblSaleByCard.TopAnchor.ConstraintEqualTo(cashColorView.SafeAreaLayoutGuide.BottomAnchor, 24).Active = true;
            lblSaleByCard.LeftAnchor.ConstraintEqualTo(CardColorView.SafeAreaLayoutGuide.RightAnchor, 18).Active = true;
            lblSaleByCard.RightAnchor.ConstraintEqualTo(lblSaleByCardValue.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblSaleByCard.HeightAnchor.ConstraintEqualTo(36).Active = true;

            //-----------------------------------------------------------------------------------

            GiftColorView.TopAnchor.ConstraintEqualTo(CardColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            GiftColorView.LeftAnchor.ConstraintEqualTo(GiftColorView.Superview.CenterXAnchor, 0).Active = true;
            GiftColorView.WidthAnchor.ConstraintEqualTo(12).Active = true;
            GiftColorView.HeightAnchor.ConstraintEqualTo(12).Active = true;

            lblSaleByGiftValue.TopAnchor.ConstraintEqualTo(CardColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            lblSaleByGiftValue.RightAnchor.ConstraintEqualTo(lblSaleByCardValue.Superview.RightAnchor, -15).Active = true;
            lblSaleByGiftValue.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblSaleByGiftValue.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblSaleByGift.TopAnchor.ConstraintEqualTo(CardColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            lblSaleByGift.LeftAnchor.ConstraintEqualTo(GiftColorView.SafeAreaLayoutGuide.RightAnchor, 18).Active = true;
            lblSaleByGift.RightAnchor.ConstraintEqualTo(lblSaleByGiftValue.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblSaleByGift.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //-----------------------------------------------------------------------------------

            ePayColorView.TopAnchor.ConstraintEqualTo(GiftColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            ePayColorView.LeftAnchor.ConstraintEqualTo(ePayColorView.Superview.CenterXAnchor, 0).Active = true;
            ePayColorView.WidthAnchor.ConstraintEqualTo(12).Active = true;
            ePayColorView.HeightAnchor.ConstraintEqualTo(12).Active = true;

            lblSaleByePaymentValue.TopAnchor.ConstraintEqualTo(GiftColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            lblSaleByePaymentValue.RightAnchor.ConstraintEqualTo(lblSaleByePaymentValue.Superview.RightAnchor, -15).Active = true;
            lblSaleByePaymentValue.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblSaleByePaymentValue.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblSaleByePayment.TopAnchor.ConstraintEqualTo(GiftColorView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            lblSaleByePayment.LeftAnchor.ConstraintEqualTo(ePayColorView.SafeAreaLayoutGuide.RightAnchor, 18).Active = true;
            lblSaleByePayment.RightAnchor.ConstraintEqualTo(lblSaleByePaymentValue.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblSaleByePayment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region BestSellingView
            BestSellingView.TopAnchor.ConstraintEqualTo(SaleByTypeView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            BestSellingView.LeftAnchor.ConstraintEqualTo(SaleByTypeView.Superview.LeftAnchor, 0).Active = true;
            BestSellingView.RightAnchor.ConstraintEqualTo(SaleByTypeView.Superview.RightAnchor, 0).Active = true;
            BestSellingView.HeightAnchor.ConstraintEqualTo(360).Active = true;

            lblBestSelling.TopAnchor.ConstraintEqualTo(lblBestSelling.Superview.TopAnchor, 12).Active = true;
            lblBestSelling.LeftAnchor.ConstraintEqualTo(lblBestSelling.Superview.LeftAnchor, 15).Active = true;
            lblBestSelling.RightAnchor.ConstraintEqualTo(lblBestSelling.Superview.RightAnchor, -15).Active = true;
            lblBestSelling.HeightAnchor.ConstraintEqualTo(18).Active = true;

            #endregion

            #region BestEmployeeView
            BestEmployeeView.TopAnchor.ConstraintEqualTo(BestSellingView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            BestEmployeeView.LeftAnchor.ConstraintEqualTo(BestEmployeeView.Superview.LeftAnchor, 0).Active = true;
            BestEmployeeView.RightAnchor.ConstraintEqualTo(BestEmployeeView.Superview.RightAnchor, 0).Active = true;
            BestEmployeeView.BottomAnchor.ConstraintEqualTo(BestEmployeeView.Superview.BottomAnchor, -10).Active = true;
            BestEmployeeView.HeightAnchor.ConstraintEqualTo(360).Active = true;

            lblBestEmployee.TopAnchor.ConstraintEqualTo(lblBestEmployee.Superview.TopAnchor, 12).Active = true;
            lblBestEmployee.LeftAnchor.ConstraintEqualTo(lblBestEmployee.Superview.LeftAnchor, 15).Active = true;
            lblBestEmployee.RightAnchor.ConstraintEqualTo(lblBestEmployee.Superview.RightAnchor, -15).Active = true;
            lblBestEmployee.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}