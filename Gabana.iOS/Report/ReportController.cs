using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ReportController : UIViewController
    {
        UIView SaleHeadView, ItemHeadView, _contentView;
        UIImageView SaleHead_img, ItemHead_img;
        UILabel lbl_SaleHead, lbl_ItemHead;
        UIScrollView _scrollView;
        public static List<Branch> listAllBranch = new List<Branch>();
        public static List<Branch> listChooseBranch = new List<Branch>();
        #region SaleHeadView attribute
        UIView Sale_SaleView, Sale_ProfitView, Sale_CategoryView, Sale_CustomerView, Sale_EmployeeView, Sale_PaymentTypeView;
        UIImageView Sale_Sale_img, Sale_Profit_img, Sale_Category_img, Sale_Customer_img, Sale_Employee_img, Sale_PaymentType_img;
        UILabel lbl_Sale_Sale, lbl_Sale_Profit, lbl_Sale_Category, lbl_Sale_Customer, lbl_Sale_Employee, lbl_Sale_PaymentType;
        #endregion

        #region ItemHeadView attribute
        UIView Item_TopSaleView, Item_StockView;
        UIImageView Item_TopSale_img, Item_Stock_img;
        UILabel lbl_Item_TopSale, lbl_Item_Stock;
        #endregion

        
        UIBarButtonItem selectBranch;
        public static bool isModifyBranch = false;
        BranchManage setBranch = new BranchManage();
        public Gabana3.JAM.Merchant.Merchants merchant;

        #region Page
        ReportBranchController ReportBranchPage = null;
        SaleReportController SaleReportPage = null;
        ProfitReportController ProfitReportPage = null;
        CategoryReportController CategoryReportPage = null;
        CustomerReportController CustomerReportPage = null;
        PaymentTypeReportController PaymentPAge = null;
        EmployeeReportController EmployeePage = null;
        ItemTopSaleReportController ItemTopSalePage = null;
        ItemsStockReportController ItemStockPage = null;

        UIBarButtonItem backButton;
        UIView Sale_SaleBranchView;
        UIImageView Sale_SaleBranch_img;
        UILabel lbl_Sale_SaleBranch;
        BranchManage branchManage = new BranchManage();
        #endregion

        public ReportController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController,"Report"); 
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);


            //if (isModifyBranch)
            //{
            //    BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(BranchSelect.SysBranchID));
            //    if (BranchSelect.BranchName.Length >= 15)
            //    {
            //        selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
            //    }
            //    else
            //    {
            //        selectBranch.Title = BranchSelect.BranchName;
            //    }
            //    isModifyBranch = false;
            //}
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                var pinCodePage = new PinCodeController("Pincode");
                pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                await this.PresentViewControllerAsync(pinCodePage, false);
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                base.ViewDidLoad();
                var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<ORM.MerchantDB.Branch, ChooseBranch>());
                var mapper = mapperConfiguration.CreateMapper();
                var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                listAllBranch = new List<Branch>();
                foreach (var item in lstBranch)
                {
                    listAllBranch.Add(item);
                }





                //BranchSelect.SysBranchID = Convert.ToInt64(Preferences.Get("Branch", ""));
                //BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(BranchSelect.SysBranchID));


                //selectBranch = new UIBarButtonItem();
                //if (BranchSelect.BranchName.Length >= 15)
                //{
                //    selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
                //}
                //else
                //{
                //    selectBranch.Title = BranchSelect.BranchName;
                //}
                //selectBranch.TintColor = UIColor.FromRGB(0, 149, 218);

                //selectBranch.Clicked += (sender, e) => {
                //    // open select customer page
                //    if (ReportBranchPage == null)
                //    {

                //        backButton.Title = "Choose Branch";
                //        ReportBranchPage = new ReportBranchController();
                //    }
                //    this.NavigationController.PushViewController(ReportBranchPage, false);
                //};
                //this.NavigationItem.RightBarButtonItem = selectBranch;

                initAttribute();
                SetupAutoLayout();
            }
            catch (Exception ex )
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248); ;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248,248,248);

            #region SaleHeadView
            SaleHeadView = new UIView();
            SaleHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleHeadView.BackgroundColor = UIColor.White;

            lbl_SaleHead = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_SaleHead.Font = lbl_SaleHead.Font.WithSize(15);
            lbl_SaleHead.Text = Utils.TextBundle("salecategory", "หมวดหมู่การขาย");
            SaleHeadView.AddSubview(lbl_SaleHead);

            SaleHead_img = new UIImageView();
            SaleHead_img.Image = UIImage.FromFile("ReportSale.png");
            SaleHead_img.TranslatesAutoresizingMaskIntoConstraints = false;
            SaleHeadView.AddSubview(SaleHead_img);
            #endregion

            #region Sale_SaleView
            Sale_SaleView = new UIView();
            Sale_SaleView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_SaleView.BackgroundColor = UIColor.White;

            Sale_Sale_img = new UIImageView();
            Sale_Sale_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_Sale_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_SaleView.AddSubview(Sale_Sale_img);

            lbl_Sale_Sale = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_Sale.Font = lbl_Sale_Sale.Font.WithSize(15);
            lbl_Sale_Sale.Text = Utils.TextBundle("sale_report", "รายงานยอดขาย");
            Sale_SaleView.AddSubview(lbl_Sale_Sale);

            Sale_SaleView.UserInteractionEnabled = true;
            var tapGestureSale_SaleView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_SaleView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_SaleView.AddGestureRecognizer(tapGestureSale_SaleView);
            #endregion

            #region Sale_SaleBranchView
            Sale_SaleBranchView = new UIView();
            Sale_SaleBranchView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_SaleBranchView.BackgroundColor = UIColor.White;

            Sale_SaleBranch_img = new UIImageView();
            Sale_SaleBranch_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_SaleBranch_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_SaleBranchView.AddSubview(Sale_SaleBranch_img);

            lbl_Sale_SaleBranch = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_SaleBranch.Font = lbl_Sale_SaleBranch.Font.WithSize(15);
            lbl_Sale_SaleBranch.Text = Utils.TextBundle("salebybranch_report", "รายงานยอดขายตามสาขา");
            Sale_SaleBranchView.AddSubview(lbl_Sale_SaleBranch);

            Sale_SaleBranchView.UserInteractionEnabled = true;
            var tapGestureSale_SaleBranchView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_SaleBranchView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_SaleBranchView.AddGestureRecognizer(tapGestureSale_SaleBranchView);
            #endregion

            #region Sale_ProfitView
            Sale_ProfitView = new UIView();
            Sale_ProfitView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_ProfitView.BackgroundColor = UIColor.White;

            Sale_Profit_img = new UIImageView();
            Sale_Profit_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_Profit_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_ProfitView.AddSubview(Sale_Profit_img);

            lbl_Sale_Profit = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_Profit.Font = lbl_Sale_Profit.Font.WithSize(15);
            lbl_Sale_Profit.Text = Utils.TextBundle("profitbycostestimate_report", "รายงานกำไรจากทุนประเมิน");
            Sale_ProfitView.AddSubview(lbl_Sale_Profit);

            Sale_ProfitView.UserInteractionEnabled = true;
            var tapGestureSale_ProfitView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_ProfitView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_ProfitView.AddGestureRecognizer(tapGestureSale_ProfitView);
            #endregion

            #region Sale_CategoryView
            Sale_CategoryView = new UIView();
            Sale_CategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_CategoryView.BackgroundColor = UIColor.White;

            Sale_Category_img = new UIImageView();
            Sale_Category_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_Category_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_CategoryView.AddSubview(Sale_Category_img);

            lbl_Sale_Category = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_Category.Font = lbl_Sale_Category.Font.WithSize(15);
            lbl_Sale_Category.Text = Utils.TextBundle("salebycategory_report", "รายงานยอดขายตามหมวดหมู่");
            Sale_CategoryView.AddSubview(lbl_Sale_Category);

            Sale_CategoryView.UserInteractionEnabled = true;
            var tapGestureSale_CategoryView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_CategoryView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_CategoryView.AddGestureRecognizer(tapGestureSale_CategoryView);
            #endregion

            #region Sale_CustomerView
            Sale_CustomerView = new UIView();
            Sale_CustomerView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_CustomerView.BackgroundColor = UIColor.White;

            Sale_Customer_img = new UIImageView();
            Sale_Customer_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_Customer_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_CustomerView.AddSubview(Sale_Customer_img);

            lbl_Sale_Customer = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_Customer.Font = lbl_Sale_Customer.Font.WithSize(15);
            lbl_Sale_Customer.Text = Utils.TextBundle("salebycustomer_report", "รายงานยอดขายตามลูกค้า");
            Sale_CustomerView.AddSubview(lbl_Sale_Customer);

            Sale_CustomerView.UserInteractionEnabled = true;
            var tapGestureSale_Customer = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_CustomerView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_CustomerView.AddGestureRecognizer(tapGestureSale_Customer);
            #endregion

            #region Sale_EmployeeView
            Sale_EmployeeView = new UIView();
            Sale_EmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_EmployeeView.BackgroundColor = UIColor.White;

            Sale_Employee_img = new UIImageView();
            Sale_Employee_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_Employee_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_EmployeeView.AddSubview(Sale_Employee_img);

            lbl_Sale_Employee = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_Employee.Font = lbl_Sale_Employee.Font.WithSize(15);
            lbl_Sale_Employee.Text = Utils.TextBundle("salebyemployee_report", "รายงานยอดขายตามพนักงาน");
            Sale_EmployeeView.AddSubview(lbl_Sale_Employee);

            Sale_EmployeeView.UserInteractionEnabled = true;
            var tapGestureSale_EmployeeView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_EmployeeView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_EmployeeView.AddGestureRecognizer(tapGestureSale_EmployeeView);
            #endregion

            #region Sale_PaymentTypeView
            Sale_PaymentTypeView = new UIView();
            Sale_PaymentTypeView.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_PaymentTypeView.BackgroundColor = UIColor.White;

            Sale_PaymentType_img = new UIImageView();
            Sale_PaymentType_img.Image = UIImage.FromFile("ReportBulleted.png");
            Sale_PaymentType_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Sale_PaymentTypeView.AddSubview(Sale_PaymentType_img);

            lbl_Sale_PaymentType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Sale_PaymentType.Font = lbl_Sale_PaymentType.Font.WithSize(15);
            lbl_Sale_PaymentType.Text = Utils.TextBundle("salebypaymentmethod_report", "รายงานยอดขายตามประเภทการชำระเงิน");
            Sale_PaymentTypeView.AddSubview(lbl_Sale_PaymentType);

            Sale_PaymentTypeView.UserInteractionEnabled = true;
            var tapGestureSale_PaymentTypeView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Sale_PaymentTypeView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Sale_PaymentTypeView.AddGestureRecognizer(tapGestureSale_PaymentTypeView);
            #endregion

            //----------------------------------------------

            #region ItemHeadView
            ItemHeadView = new UIView();
            ItemHeadView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemHeadView.BackgroundColor = UIColor.White;

            lbl_ItemHead = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_ItemHead.Font = lbl_ItemHead.Font.WithSize(15);
            lbl_ItemHead.Text = Utils.TextBundle("itemcategory", "หมวดหมู่สินค้า");
            ItemHeadView.AddSubview(lbl_ItemHead);

            ItemHead_img = new UIImageView();
            ItemHead_img.Image = UIImage.FromFile("ReportItem.png");
            ItemHead_img.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemHeadView.AddSubview(ItemHead_img);
            #endregion

            #region Item_TopSaleView
            Item_TopSaleView = new UIView();
            Item_TopSaleView.TranslatesAutoresizingMaskIntoConstraints = false;
            Item_TopSaleView.BackgroundColor = UIColor.White;

            Item_TopSale_img = new UIImageView();
            Item_TopSale_img.Image = UIImage.FromFile("ReportBulleted.png");
            Item_TopSale_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Item_TopSaleView.AddSubview(Item_TopSale_img);

            lbl_Item_TopSale = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Item_TopSale.Font = lbl_Item_TopSale.Font.WithSize(15);
            lbl_Item_TopSale.Text = Utils.TextBundle("bestitem_report", "รายงานสินค้าขายดี");
            Item_TopSaleView.AddSubview(lbl_Item_TopSale);

            Item_TopSaleView.UserInteractionEnabled = true;
            var tapGestureItem_TopSaleView = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Item_TopSaleView_Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Item_TopSaleView.AddGestureRecognizer(tapGestureItem_TopSaleView);
            #endregion

            //#region Item_StockView
            //Item_StockView = new UIView();
            //Item_StockView.TranslatesAutoresizingMaskIntoConstraints = false;
            //Item_StockView.BackgroundColor = UIColor.White;

            //Item_Stock_img = new UIImageView();
            //Item_Stock_img.Image = UIImage.FromFile("ReportBulleted.png");
            //Item_Stock_img.TranslatesAutoresizingMaskIntoConstraints = false;
            //Item_StockView.AddSubview(Item_Stock_img);

            //lbl_Item_Stock = new UILabel
            //{
            //    TextColor = UIColor.FromRGB(64, 64, 64),
            //    TranslatesAutoresizingMaskIntoConstraints = false
            //};
            //lbl_Item_Stock.Font = lbl_Item_Stock.Font.WithSize(15);
            //lbl_Item_Stock.Text = "รายงานสินค้าคงเหลือ";
            //Item_StockView.AddSubview(lbl_Item_Stock);

            //Item_StockView.UserInteractionEnabled = true;
            //var tapGestureItem_StockView = new UITapGestureRecognizer(this,
            //    new ObjCRuntime.Selector("Item_StockView_Select:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //Item_StockView.AddGestureRecognizer(tapGestureItem_StockView);
            //#endregion

            _contentView.AddSubview(SaleHeadView);
            _contentView.AddSubview(ItemHeadView);
            _contentView.AddSubview(Sale_SaleView);
            _contentView.AddSubview(Sale_SaleBranchView);
            _contentView.AddSubview(Sale_ProfitView);
            _contentView.AddSubview(Sale_CategoryView);
            _contentView.AddSubview(Sale_CustomerView);
            _contentView.AddSubview(Sale_EmployeeView);
            _contentView.AddSubview(Sale_PaymentTypeView);
            _contentView.AddSubview(Item_TopSaleView);
            //_contentView.AddSubview(Item_StockView);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
        }
        #region Go To Report
        [Export("Sale_SaleView_Select:")]
        public void Sale_SaleView_Select(UIGestureRecognizer sender)
        {
            //if(SaleReportPage == null)
            //{
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("sale_report", "รายงานยอดขาย"));
            
                SaleReportPage = new SaleReportController();
            //}
            this.NavigationController.PushViewController(SaleReportPage,false);
        }
        [Export("Sale_SaleBranchView_Select:")]
        public void Sale_SaleBranchView_Select(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("salebybranch_report", "รายงานยอดขายตามสาขา"));
            
            var  SaleReportPage = new SaleReportBranchController();
            this.NavigationController.PushViewController(SaleReportPage, false);
        }
        [Export("Sale_ProfitView_Select:")]
        public void Sale_ProfitView_Select(UIGestureRecognizer sender)
        {
            //ProfitReportPage
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("profitbycostestimate_report", "รายงานกำไรจากทุนประเมิน"));
            
                ProfitReportPage = new ProfitReportController();
            
            this.NavigationController.PushViewController(ProfitReportPage, false);
        }
        [Export("Sale_CategoryView_Select:")]
        public void Sale_CategoryView_Select(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("salebycategory_report", "รายงานยอดขายตามหมวดหมู่")); 
            
                CategoryReportPage = new CategoryReportController();
            
            this.NavigationController.PushViewController(CategoryReportPage, false);
        }
        [Export("Sale_CustomerView_Select:")]
        public void Sale_CustomerView_Select(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("salebycustomer_report", "รายงานยอดขายตามลูกค้า")); 
            
                CustomerReportPage = new CustomerReportController();
            
            this.NavigationController.PushViewController(CustomerReportPage, false);
        }
        [Export("Sale_EmployeeView_Select:")]
        public void Sale_EmployeeView_Select(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("salebyemployee_report", "รายงานยอดขายตามพนักงาน")); 
            
                EmployeePage = new EmployeeReportController();
            
            this.NavigationController.PushViewController(EmployeePage, false);
        }
        [Export("Sale_PaymentTypeView_Select:")]
        public void Sale_PaymentTypeView_Select(UIGestureRecognizer sender)
        {

            Utils.SetTitle(this.NavigationController,Utils.TextBundle("salebypaymentmethod_report", "รายงานยอดขายตามประเภทการชำระเงิน")); 
           
                PaymentPAge = new PaymentTypeReportController();
            
            this.NavigationController.PushViewController(PaymentPAge, false);
        }
        [Export("Item_TopSaleView_Select:")]
        public void Item_TopSaleView_Select(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("bestitem_report", "รายงานสินค้าขายดี")); 
            
                ItemTopSalePage = new ItemTopSaleReportController();
            
            this.NavigationController.PushViewController(ItemTopSalePage, false);
        }
        //[Export("Item_StockView_Select:")]
        //public void Item_StockView_Select(UIGestureRecognizer sender)
        //{
        //    if (ItemStockPage == null)
        //    {
        //        backButton.Title = "รายงานสินค้าคงเหลือ";
        //        ItemStockPage = new ItemsStockReportController();
        //    }
        //    this.NavigationController.PushViewController(ItemStockPage, false);
        //}
        #endregion
        void SetupAutoLayout()
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

            #region SaleHeadView
            SaleHeadView.TopAnchor.ConstraintEqualTo(SaleHeadView.Superview.TopAnchor, 0).Active = true;
            SaleHeadView.LeftAnchor.ConstraintEqualTo(SaleHeadView.Superview.LeftAnchor, 0).Active = true;
            SaleHeadView.RightAnchor.ConstraintEqualTo(SaleHeadView.Superview.RightAnchor, 0).Active = true;
            SaleHeadView.HeightAnchor.ConstraintEqualTo(48).Active = true;

            SaleHead_img.CenterYAnchor.ConstraintEqualTo(SaleHead_img.Superview.CenterYAnchor, 0).Active = true;
            SaleHead_img.LeftAnchor.ConstraintEqualTo(SaleHead_img.Superview.LeftAnchor, 15).Active = true;
            SaleHead_img.HeightAnchor.ConstraintEqualTo(28).Active = true;
            SaleHead_img.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_SaleHead.CenterYAnchor.ConstraintEqualTo(SaleHead_img.Superview.CenterYAnchor, 0).Active = true;
            lbl_SaleHead.LeftAnchor.ConstraintEqualTo(SaleHead_img.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_SaleHead.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_SaleHead.RightAnchor.ConstraintEqualTo(lbl_SaleHead.Superview.RightAnchor,0).Active = true;
            #endregion

            #region Sale_SaleView
            Sale_SaleView.TopAnchor.ConstraintEqualTo(SaleHeadView.BottomAnchor, 0.4f).Active = true;
            Sale_SaleView.LeftAnchor.ConstraintEqualTo(Sale_SaleView.Superview.LeftAnchor, 0).Active = true;
            Sale_SaleView.RightAnchor.ConstraintEqualTo(Sale_SaleView.Superview.RightAnchor, 0).Active = true;
            Sale_SaleView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_Sale_img.CenterYAnchor.ConstraintEqualTo(Sale_Sale_img.Superview.CenterYAnchor).Active = true;
            Sale_Sale_img.LeftAnchor.ConstraintEqualTo(Sale_Sale_img.Superview.LeftAnchor, 15).Active = true;
            Sale_Sale_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_Sale_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_Sale.CenterYAnchor.ConstraintEqualTo(lbl_Sale_Sale.Superview.CenterYAnchor).Active = true;
            lbl_Sale_Sale.LeftAnchor.ConstraintEqualTo(Sale_Sale_img.RightAnchor, 15).Active = true;
            lbl_Sale_Sale.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_Sale.RightAnchor.ConstraintEqualTo(lbl_Sale_Sale.Superview.RightAnchor,-30).Active = true;
            #endregion

            #region Sale_SaleBranchView
            Sale_SaleBranchView.TopAnchor.ConstraintEqualTo(Sale_SaleView.BottomAnchor, 0.4f).Active = true;
            Sale_SaleBranchView.LeftAnchor.ConstraintEqualTo(Sale_SaleBranchView.Superview.LeftAnchor, 0).Active = true;
            Sale_SaleBranchView.RightAnchor.ConstraintEqualTo(Sale_SaleBranchView.Superview.RightAnchor, 0).Active = true;
            Sale_SaleBranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_SaleBranch_img.CenterYAnchor.ConstraintEqualTo(Sale_SaleBranch_img.Superview.CenterYAnchor).Active = true;
            Sale_SaleBranch_img.LeftAnchor.ConstraintEqualTo(Sale_SaleBranch_img.Superview.LeftAnchor, 15).Active = true;
            Sale_SaleBranch_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_SaleBranch_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_SaleBranch.CenterYAnchor.ConstraintEqualTo(lbl_Sale_SaleBranch.Superview.CenterYAnchor).Active = true;
            lbl_Sale_SaleBranch.LeftAnchor.ConstraintEqualTo(Sale_SaleBranch_img.RightAnchor, 15).Active = true;
            lbl_Sale_SaleBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_SaleBranch.RightAnchor.ConstraintEqualTo(lbl_Sale_SaleBranch.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region Sale_ProfitView
            Sale_ProfitView.TopAnchor.ConstraintEqualTo(Sale_SaleBranchView.BottomAnchor, 0.4f).Active = true;
            Sale_ProfitView.LeftAnchor.ConstraintEqualTo(Sale_ProfitView.Superview.LeftAnchor, 0).Active = true;
            Sale_ProfitView.RightAnchor.ConstraintEqualTo(Sale_ProfitView.Superview.RightAnchor, 0).Active = true;
            Sale_ProfitView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_Profit_img.CenterYAnchor.ConstraintEqualTo(Sale_Profit_img.Superview.CenterYAnchor).Active = true;
            Sale_Profit_img.LeftAnchor.ConstraintEqualTo(Sale_Profit_img.Superview.LeftAnchor, 15).Active = true;
            Sale_Profit_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_Profit_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_Profit.CenterYAnchor.ConstraintEqualTo(lbl_Sale_Profit.Superview.CenterYAnchor).Active = true;
            lbl_Sale_Profit.LeftAnchor.ConstraintEqualTo(Sale_Profit_img.RightAnchor, 15).Active = true;
            lbl_Sale_Profit.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_Profit.RightAnchor.ConstraintEqualTo(lbl_Sale_Profit.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region Sale_CategoryView
            Sale_CategoryView.TopAnchor.ConstraintEqualTo(Sale_ProfitView.BottomAnchor, 0.4f).Active = true;
            Sale_CategoryView.LeftAnchor.ConstraintEqualTo(Sale_CategoryView.Superview.LeftAnchor, 0).Active = true;
            Sale_CategoryView.RightAnchor.ConstraintEqualTo(Sale_CategoryView.Superview.RightAnchor, 0).Active = true;
            Sale_CategoryView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_Category_img.CenterYAnchor.ConstraintEqualTo(Sale_Category_img.Superview.CenterYAnchor).Active = true;
            Sale_Category_img.LeftAnchor.ConstraintEqualTo(Sale_Category_img.Superview.LeftAnchor, 15).Active = true;
            Sale_Category_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_Category_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_Category.CenterYAnchor.ConstraintEqualTo(lbl_Sale_Category.Superview.CenterYAnchor).Active = true;
            lbl_Sale_Category.LeftAnchor.ConstraintEqualTo(Sale_Category_img.RightAnchor, 15).Active = true;
            lbl_Sale_Category.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_Category.RightAnchor.ConstraintEqualTo(lbl_Sale_Category.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region Sale_CustomerView
            Sale_CustomerView.TopAnchor.ConstraintEqualTo(Sale_CategoryView.BottomAnchor, 0.4f).Active = true;
            Sale_CustomerView.LeftAnchor.ConstraintEqualTo(Sale_CustomerView.Superview.LeftAnchor, 0).Active = true;
            Sale_CustomerView.RightAnchor.ConstraintEqualTo(Sale_CustomerView.Superview.RightAnchor, 0).Active = true;
            Sale_CustomerView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_Customer_img.CenterYAnchor.ConstraintEqualTo(Sale_Customer_img.Superview.CenterYAnchor).Active = true;
            Sale_Customer_img.LeftAnchor.ConstraintEqualTo(Sale_Customer_img.Superview.LeftAnchor, 15).Active = true;
            Sale_Customer_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_Customer_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_Customer.CenterYAnchor.ConstraintEqualTo(lbl_Sale_Customer.Superview.CenterYAnchor).Active = true;
            lbl_Sale_Customer.LeftAnchor.ConstraintEqualTo(Sale_Customer_img.RightAnchor, 15).Active = true;
            lbl_Sale_Customer.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_Customer.RightAnchor.ConstraintEqualTo(lbl_Sale_Customer.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region Sale_EmployeeView
            Sale_EmployeeView.TopAnchor.ConstraintEqualTo(Sale_CustomerView.BottomAnchor, 0.4f).Active = true;
            Sale_EmployeeView.LeftAnchor.ConstraintEqualTo(Sale_EmployeeView.Superview.LeftAnchor, 0).Active = true;
            Sale_EmployeeView.RightAnchor.ConstraintEqualTo(Sale_EmployeeView.Superview.RightAnchor, 0).Active = true;
            Sale_EmployeeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_Employee_img.CenterYAnchor.ConstraintEqualTo(Sale_Employee_img.Superview.CenterYAnchor).Active = true;
            Sale_Employee_img.LeftAnchor.ConstraintEqualTo(Sale_Employee_img.Superview.LeftAnchor, 15).Active = true;
            Sale_Employee_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_Employee_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_Employee.CenterYAnchor.ConstraintEqualTo(lbl_Sale_Employee.Superview.CenterYAnchor).Active = true;
            lbl_Sale_Employee.LeftAnchor.ConstraintEqualTo(Sale_Employee_img.RightAnchor, 15).Active = true;
            lbl_Sale_Employee.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_Employee.RightAnchor.ConstraintEqualTo(lbl_Sale_Employee.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region Sale_PaymentTypeView
            Sale_PaymentTypeView.TopAnchor.ConstraintEqualTo(Sale_EmployeeView.BottomAnchor, 0.4f).Active = true;
            Sale_PaymentTypeView.LeftAnchor.ConstraintEqualTo(Sale_PaymentTypeView.Superview.LeftAnchor, 0).Active = true;
            Sale_PaymentTypeView.RightAnchor.ConstraintEqualTo(Sale_PaymentTypeView.Superview.RightAnchor, 0).Active = true;
            Sale_PaymentTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Sale_PaymentType_img.CenterYAnchor.ConstraintEqualTo(Sale_PaymentType_img.Superview.CenterYAnchor).Active = true;
            Sale_PaymentType_img.LeftAnchor.ConstraintEqualTo(Sale_PaymentType_img.Superview.LeftAnchor, 15).Active = true;
            Sale_PaymentType_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Sale_PaymentType_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Sale_PaymentType.CenterYAnchor.ConstraintEqualTo(lbl_Sale_PaymentType.Superview.CenterYAnchor).Active = true;
            lbl_Sale_PaymentType.LeftAnchor.ConstraintEqualTo(Sale_PaymentType_img.RightAnchor, 15).Active = true;
            lbl_Sale_PaymentType.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Sale_PaymentType.RightAnchor.ConstraintEqualTo(lbl_Sale_PaymentType.Superview.RightAnchor, -30).Active = true;
            #endregion

            #region ItemHeadView
            ItemHeadView.TopAnchor.ConstraintEqualTo(Sale_PaymentTypeView.BottomAnchor, 5).Active = true;
            ItemHeadView.LeftAnchor.ConstraintEqualTo(ItemHeadView.Superview.LeftAnchor, 0).Active = true;
            ItemHeadView.RightAnchor.ConstraintEqualTo(ItemHeadView.Superview.RightAnchor, 0).Active = true;
            ItemHeadView.HeightAnchor.ConstraintEqualTo(38).Active = true;

            ItemHead_img.CenterYAnchor.ConstraintEqualTo(ItemHead_img.Superview.CenterYAnchor, 0).Active = true;
            ItemHead_img.LeftAnchor.ConstraintEqualTo(ItemHead_img.Superview.LeftAnchor, 15).Active = true;
            ItemHead_img.HeightAnchor.ConstraintEqualTo(28).Active = true;
            ItemHead_img.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_ItemHead.CenterYAnchor.ConstraintEqualTo(lbl_ItemHead.Superview.CenterYAnchor, 0).Active = true;
            lbl_ItemHead.LeftAnchor.ConstraintEqualTo(ItemHead_img.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_ItemHead.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_ItemHead.RightAnchor.ConstraintEqualTo(lbl_SaleHead.Superview.RightAnchor, 0).Active = true;
            #endregion

            #region Item_TopSaleView
            Item_TopSaleView.TopAnchor.ConstraintEqualTo(ItemHeadView.BottomAnchor, 0.4f).Active = true;
            Item_TopSaleView.LeftAnchor.ConstraintEqualTo(Item_TopSaleView.Superview.LeftAnchor, 0).Active = true;
            Item_TopSaleView.RightAnchor.ConstraintEqualTo(Item_TopSaleView.Superview.RightAnchor, 0).Active = true;
            Item_TopSaleView.BottomAnchor.ConstraintEqualTo(Item_TopSaleView.Superview.BottomAnchor, 0).Active = true;
            Item_TopSaleView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            Item_TopSale_img.CenterYAnchor.ConstraintEqualTo(Item_TopSale_img.Superview.CenterYAnchor).Active = true;
            Item_TopSale_img.LeftAnchor.ConstraintEqualTo(Item_TopSale_img.Superview.LeftAnchor, 15).Active = true;
            Item_TopSale_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            Item_TopSale_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lbl_Item_TopSale.CenterYAnchor.ConstraintEqualTo(lbl_Item_TopSale.Superview.CenterYAnchor).Active = true;
            lbl_Item_TopSale.LeftAnchor.ConstraintEqualTo(Item_TopSale_img.RightAnchor, 15).Active = true;
            lbl_Item_TopSale.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_Item_TopSale.RightAnchor.ConstraintEqualTo(lbl_Item_TopSale.Superview.RightAnchor, -30).Active = true;
            #endregion

            //#region Item_StockView
            //Item_StockView.TopAnchor.ConstraintEqualTo(Item_TopSaleView.BottomAnchor, 0.4f).Active = true;
            //Item_StockView.LeftAnchor.ConstraintEqualTo(Item_StockView.Superview.LeftAnchor, 0).Active = true;
            //Item_StockView.RightAnchor.ConstraintEqualTo(Item_StockView.Superview.RightAnchor, 0).Active = true;
            //Item_StockView.BottomAnchor.ConstraintEqualTo(Item_StockView.Superview.BottomAnchor, 0).Active = true;
            //Item_StockView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            //Item_Stock_img.CenterYAnchor.ConstraintEqualTo(Item_Stock_img.Superview.CenterYAnchor).Active = true;
            //Item_Stock_img.LeftAnchor.ConstraintEqualTo(Item_Stock_img.Superview.LeftAnchor, 15).Active = true;
            //Item_Stock_img.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //Item_Stock_img.WidthAnchor.ConstraintEqualTo(18).Active = true;

            //lbl_Item_Stock.CenterYAnchor.ConstraintEqualTo(lbl_Item_Stock.Superview.CenterYAnchor).Active = true;
            //lbl_Item_Stock.LeftAnchor.ConstraintEqualTo(Item_Stock_img.RightAnchor, 15).Active = true;
            //lbl_Item_Stock.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lbl_Item_Stock.RightAnchor.ConstraintEqualTo(lbl_Item_Stock.Superview.RightAnchor, -30).Active = true;
            //#endregion

        }
    }
}