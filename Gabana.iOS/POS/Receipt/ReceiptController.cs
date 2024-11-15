using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Gabana.POS.Cart
{
    public partial class ReceiptController : UIViewController
    {
        UIView headerView, bottomView, _contentView, Viewdismember, ViewVat, Viewdiscount, ViewService;
        UIScrollView _scrollView;
        UIImageView merchantImg;
        UIView LineView1, LineView2, LineView3;
        UILabel lblMerchantName,lblTransNo,lblDate,lblCustomer;
        UICollectionView ReceiptCollectionview;
        ReceiptDataSource receiptDataSource;
        private TranWithDetailsLocal tranWithDetails;
        UIView LineView4;
        UILabel lblThank;
        UILabel lbltxtQuantityTotal, lblQuantityTotal, lbltxtService , lblService, lbldiscount;
        UILabel lbltxtSubtotal, lblSubtotal, lbltxtMemDiscount, lblMemdiscount, lbltxtVat, lblVat, lbltxtDiscount;
        UILabel lbltxtTotal, lblTotal, lblCash, lbltxtCash, lbltxtChange, lblChange, lbltxtCashier, lblCashier;
        POSController posPage;
        private UILabel lblPowerby;
        private UILabel lbltxtGift;
        private UILabel lblGift;
        private UILabel lbltxtmyqr;
        private UILabel lblmyqr;
        private UILabel lbltxtcredit;
        private UILabel lblcredit;
        UIView BottomView;
        UIView btnPrint, btnPDF, btnEmail, btnShare;
        UIImageView btnPrintImg, btnPDFImg, btnEmailImg, btnShareImg,  imgvoidbill;
        UILabel lbl_btnPrint, lbl_btnPDF, lbl_btnEmail, lbl_btnShare ;
        private UILabel lbltxtremark;
        private UILabel lblremark;
        private UILabel lblslip;
        private UIImageView btnslipimg;
        private bool havepic;
        private string picpath;
        private UILabel lab;

        public ReceiptController(TranWithDetailsLocal tranWithDetails) 
        {
            this.tranWithDetails = tranWithDetails;
        }
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.Items[this.NavigationController.NavigationBar.Items.Length - 2].BackBarButtonItem.Title = "POS";
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
           
           
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
    //        POSController.tranWithDetails = this.tranWithDetails;
            
        }
        public async override void ViewDidLoad()
        {
            try
            {

            
                View.BackgroundColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                base.ViewDidLoad();

                var Paymentpicture = tranWithDetails.tranPayments.Where(x => !string.IsNullOrEmpty(x.PicturePath)).FirstOrDefault();

                if (!string.IsNullOrEmpty(Paymentpicture?.PicturePath))
                {
                    havepic = true;
                    picpath = Paymentpicture?.PicturePath;
                }

                //Utils.SetTitle(this.NavigationController, "Reciept");
                InitAttribute();
                setUpAutoLayout();

                ShareSource.Manage.MerchantManage merchantManage = new ShareSource.Manage.MerchantManage();
                var merchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                var path = merchantlocal.LogoLocalPath;
                if (!string.IsNullOrEmpty(path))
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Utils.SetImage(merchantImg, Path.Combine(docFolder, path));
                }
                else
                {
                    merchantImg.Image = UIImage.FromFile("LogoDefault.png");
                }


                setDataField();

                //var count = this.NavigationController.ViewControllers.Length;
                //for (int i = 1; i < count; i++)
                //{
                //    if (i != count - 1)
                //    {
                //        this.NavigationController.ViewControllers[1].View.RemoveFromSuperview();
                //        this.NavigationController.ViewControllers[1].RemoveFromParentViewController();
                //        //this.NavigationController.NavigationBar.PopNavigationItem(false);
                //    }

                //}
                //MainController.POS = true;
                //this.NavigationController.ReloadInputViews();
                //this.NavigationController.NavigationBar.ReloadInputViews();
                if (tranWithDetails.tran.SysCustomerID!=null)
                {
                    lbltxtMemDiscount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    lblMemdiscount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                ReceiptCollectionview.ReloadData(); 
                ReceiptCollectionview.LayoutIfNeeded();

                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                button.SetTitle("  Back", UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

                UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                uIImageView.Image = UIImage.FromBundle("SettingPrinter");
                UIButton btn = new UIButton();
                //btn.SetImage(UIImage.FromBundle("Cust"), default);
                btn.ImageView.BackgroundColor = UIColor.Black;
                btn.Frame = new CGRect(0, 0, 200, 50);
                btn.Layer.CornerRadius = 5f;
                btn.Layer.BorderWidth = 0.7f;
                btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btn.BackgroundColor = UIColor.Red;
                lab = new UILabel();
                lab.TextColor = UIColor.FromRGB(0, 149, 218);
                lab.Text = " " + tranWithDetails.tran.PrintCounter;
                lab.TextAlignment = UITextAlignment.Right;
                lab.TranslatesAutoresizingMaskIntoConstraints = false;
                uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                btn.AddSubview(uIImageView);
                btn.AddSubview(lab);

                lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                lab.HeightAnchor.ConstraintEqualTo(40).Active = true;
                lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                btn.TouchUpInside += (sender, e) => {

                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
            catch (Exception ex)
            {

            }
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopToViewController(DataCaching.posPage, false);
        }
        void InitAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.White;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.White;

            #region headerView
            headerView = new UIView();
            headerView.BackgroundColor = UIColor.White;
            headerView.TranslatesAutoresizingMaskIntoConstraints = false;

            merchantImg = new UIImageView();
         //   merchantImg.Image = Utils.SetImage(merchantImg,);
            merchantImg.TranslatesAutoresizingMaskIntoConstraints = false;
            merchantImg.Layer.CornerRadius = 25;
            merchantImg.ClipsToBounds = true;
            headerView.AddSubview(merchantImg);

            lblMerchantName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerchantName.Font = lblMerchantName.Font.WithSize(15);
            lblMerchantName.Font = UIFont.BoldSystemFontOfSize(size: lblMerchantName.Font.PointSize);
            lblMerchantName.Text = Utils.TextBundle("merchantname", "Merchant Name");
            headerView.AddSubview(lblMerchantName);

            lblTransNo = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTransNo.Font = lblTransNo.Font.WithSize(20);
            lblTransNo.Text = "#TransNO.";
            headerView.AddSubview(lblTransNo);

            lblDate = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDate.Font = lblDate.Font.WithSize(15);
            lblDate.Text = "DD/MM/YYY 00.00 PM";
            headerView.AddSubview(lblDate);

            lblCustomer = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCustomer.Font = lblCustomer.Font.WithSize(15);
            lblCustomer.Text = Utils.TextBundle("customername", "Customer Name");
            headerView.AddSubview(lblCustomer);

            LineView1 = new UIView();
            LineView1.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView1.BackgroundColor = UIColor.FromRGB(226,226,226);
            headerView.AddSubview(LineView1);
            #endregion

            #region CollectionView
            UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
            //itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            //itemflowLayout.ItemSize = new CoreGraphics.CGSize(View.Frame.Width-60,30);
            itemflowLayout.MinimumLineSpacing = 1;
            itemflowLayout.MinimumInteritemSpacing = 1;
            itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;

            ReceiptCollectionview = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
            
            ReceiptCollectionview.ScrollEnabled = false; 

            ReceiptCollectionview.ShowsVerticalScrollIndicator = false;
            
            ReceiptCollectionview.BackgroundColor = UIColor.White;
            ReceiptCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
            ReceiptCollectionview.RegisterClassForCell(cellType: typeof(ReceiptCollectionViewCell), reuseIdentifier: "receiptCollectionViewCell");
            receiptDataSource = new ReceiptDataSource(tranWithDetails.tranDetailItemWithToppings, ReceiptCollectionview.Frame);
            ReceiptCollectionview.DataSource = receiptDataSource;
            _contentView.AddSubview(ReceiptCollectionview);
            #endregion

            #region bottomView
            bottomView = new UIView();
            bottomView.BackgroundColor= UIColor.White;
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbltxtQuantityTotal = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtQuantityTotal.Font = lbltxtQuantityTotal.Font.WithSize(15);
            lbltxtQuantityTotal.Text = Utils.TextBundle("quantitytotal", "Quantity Total")+" :";
            bottomView.AddSubview(lbltxtQuantityTotal);

            lblQuantityTotal = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 148, 218),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            }; 
            lblQuantityTotal.Font = lblQuantityTotal.Font.WithSize(15);
            lblQuantityTotal.Text = " "+Utils.TextBundle("item", "Items");
            bottomView.AddSubview(lblQuantityTotal);

            LineView2 = new UIView();
            LineView2.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            bottomView.AddSubview(LineView2);


            lbltxtSubtotal = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtSubtotal.Font = lbltxtSubtotal.Font.WithSize(15);
            lbltxtSubtotal.Text = Utils.TextBundle("subtotal", "Subtotal");
            bottomView.AddSubview(lbltxtSubtotal);

            lblSubtotal = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSubtotal.Font = lblSubtotal.Font.WithSize(15);
            lblSubtotal.Text = "00.00";
            bottomView.AddSubview(lblSubtotal);

            Viewdismember = new UIView();
            Viewdismember.TranslatesAutoresizingMaskIntoConstraints = false;
            Viewdismember.BackgroundColor = UIColor.White;
            bottomView.AddSubview(Viewdismember);

            lbltxtMemDiscount = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtMemDiscount.Font = lbltxtMemDiscount.Font.WithSize(15);
            lbltxtMemDiscount.Text = "Member 5%";
            Viewdismember.AddSubview(lbltxtMemDiscount);

            lblMemdiscount = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMemdiscount.Font = lblMemdiscount.Font.WithSize(15);
            lblMemdiscount.Text = "00.00";
            Viewdismember.AddSubview(lblMemdiscount);

            ViewVat = new UIView();
            ViewVat.TranslatesAutoresizingMaskIntoConstraints = false;
            ViewVat.BackgroundColor = UIColor.White;
            bottomView.AddSubview(ViewVat);

            lbltxtVat= new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtVat.Font = lbltxtMemDiscount.Font.WithSize(15);
            lbltxtVat.Text = "Member 5%";
            ViewVat.AddSubview(lbltxtVat);

            lblVat = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblVat.Font = lblMemdiscount.Font.WithSize(15);
            lblVat.Text = "00.00";
            ViewVat.AddSubview(lblVat);

            Viewdiscount = new UIView();
            Viewdiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            Viewdiscount.BackgroundColor = UIColor.White;
            bottomView.AddSubview(Viewdiscount);

            lbltxtDiscount = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtDiscount.Font = lbltxtMemDiscount.Font.WithSize(15);
            lbltxtDiscount.Text = "Member 5%";
            Viewdiscount.AddSubview(lbltxtDiscount);

            lbldiscount = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbldiscount.Font = lblMemdiscount.Font.WithSize(15);
            lbldiscount.Text = "00.00";
            Viewdiscount.AddSubview(lbldiscount);

            ViewService = new UIView();
            ViewService.TranslatesAutoresizingMaskIntoConstraints = false;
            ViewService.BackgroundColor = UIColor.White;
            bottomView.AddSubview(ViewService);



            lbltxtService = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtService.Font = lbltxtMemDiscount.Font.WithSize(15);
            lbltxtService.Text = "Member 5%";
            ViewService.AddSubview(lbltxtService);

            lblService = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblService.Font = lblMemdiscount.Font.WithSize(15);
            lblService.Text = "00.00";
            ViewService.AddSubview(lblService);

            LineView3 = new UIView();
            LineView3.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView3.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            bottomView.AddSubview(LineView3);

            lbltxtTotal = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtTotal.Font = lbltxtTotal.Font.WithSize(15);
            lbltxtTotal.Text = Utils.TextBundle("total", "Total");
            bottomView.AddSubview(lbltxtTotal);

            lblTotal = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTotal.Font = lblTotal.Font.WithSize(20);
            lblTotal.Text = "00.00";
            bottomView.AddSubview(lblTotal);

            LineView4 = new UIView();
            LineView4.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView4.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            bottomView.AddSubview(LineView4);

            lbltxtCash = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtCash.Font = lbltxtCash.Font.WithSize(15);
            lbltxtCash.Text = Utils.TextBundle("cash", "Cash");
            bottomView.AddSubview(lbltxtCash);

            lblCash = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCash.Font = lblCash.Font.WithSize(15);
            lblCash.Text = "00.00";
            bottomView.AddSubview(lblCash);

            lbltxtGift = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtGift.Font = lbltxtGift.Font.WithSize(15);
            lbltxtGift.Text = Utils.TextBundle("giftvoucher", "Gift Voucher");
            bottomView.AddSubview(lbltxtGift);

            lblGift = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblGift.Font = lblGift.Font.WithSize(15);
            lblGift.Text = "00.00";
            bottomView.AddSubview(lblGift);

            lbltxtmyqr = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtmyqr.Font = lbltxtmyqr.Font.WithSize(15);
            lbltxtmyqr.Text = Utils.TextBundle("myqr", "My Qr");
            bottomView.AddSubview(lbltxtmyqr);

            lblmyqr = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblmyqr.Font = lblmyqr.Font.WithSize(15);
            lblmyqr.Text = "00.00";
            bottomView.AddSubview(lblmyqr);

            lbltxtcredit = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtcredit.Font = lbltxtcredit.Font.WithSize(15);
            lbltxtcredit.Text = Utils.TextBundle("creditcard", "Credit Card");
            bottomView.AddSubview(lbltxtcredit);

            lblcredit = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblcredit.Font = lblcredit.Font.WithSize(15);
            lblcredit.Text = "00.00";
            bottomView.AddSubview(lblcredit);

            lbltxtChange = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtChange.Font = lbltxtChange.Font.WithSize(15);
            lbltxtChange.Text = Utils.TextBundle("change", "Change");
            bottomView.AddSubview(lbltxtChange);

            lblChange = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblChange.Font = lblChange.Font.WithSize(15);
            lblChange.Text = "00.00";
            bottomView.AddSubview(lblChange);

            lbltxtCashier = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtCashier.Font = lbltxtCashier.Font.WithSize(15);
            lbltxtCashier.Text = Utils.TextBundle("cashier", "Cashier");
            bottomView.AddSubview(lbltxtCashier);

            lblCashier = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCashier.Font = lblCashier.Font.WithSize(15);
            lblCashier.Text = Utils.TextBundle("cashiername", "Cashier Name");
            bottomView.AddSubview(lblCashier);

            lbltxtremark = new UILabel

            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtremark.Font = lbltxtremark.Font.WithSize(15);
            lbltxtremark.Text = Utils.TextBundle("cashier", "Cashier" );
            bottomView.AddSubview(lbltxtremark);

            lblremark = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblremark.Font = lblremark.Font.WithSize(15);
            lblremark.Text = Utils.TextBundle("cashiername", "Cashier Name");
            bottomView.AddSubview(lblremark);

            lblThank = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblThank.Font = lblThank.Font.WithSize(18);
            lblThank.Text = "...THANK YOU...";
            bottomView.AddSubview(lblThank);

            lblPowerby = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblPowerby.Font = lblPowerby.Font.WithSize(15);
            lblPowerby.Text = "Powered by SeniorSoft";
            bottomView.AddSubview(lblPowerby);
            #endregion

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.White;
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region btnPrint
            btnPrint = new UIView();
            btnPrint.Layer.CornerRadius = 5;
            btnPrint.Layer.BorderWidth = 1;
            btnPrint.Layer.BorderColor = UIColor.FromRGB(200, 200, 200).CGColor;
            btnPrint.ClipsToBounds = true;
            btnPrint.TranslatesAutoresizingMaskIntoConstraints = false;
            BottomView.AddSubview(btnPrint);

            btnPrintImg = new UIImageView();
            btnPrintImg.Image = UIImage.FromFile("BillPrint.png");
            btnPrintImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnPrint.AddSubview(btnPrintImg);

            lbl_btnPrint = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_btnPrint.Text = "Print";
            lbl_btnPrint.Font = lbl_btnPrint.Font.WithSize(13);
            btnPrint.AddSubview(lbl_btnPrint);

            btnPrint.UserInteractionEnabled = true;
            var tapGesturePrint = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Print:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnPrint.AddGestureRecognizer(tapGesturePrint);

            #endregion

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
            lbl_btnEmail.Text = Utils.TextBundle("email", "Email");
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
            lbl_btnShare.Text = Utils.TextBundle("share", "Share");
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
            imgvoidbill = new UIImageView();
            imgvoidbill.Image = UIImage.FromFile("VoidText.png");
            imgvoidbill.TranslatesAutoresizingMaskIntoConstraints = false;

            imgvoidbill.Hidden = true;
            _contentView.AddSubview(headerView);
            _contentView.AddSubview(bottomView);


            lblslip = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };


            lblslip.Font = lblslip.Font.WithSize(18);
            lblslip.Text = Utils.TextBundle("receiptpicture", "Receipt Picture")+" : ";
            _contentView.AddSubview(lblslip);

            btnslipimg = new UIImageView();
            btnslipimg.ContentMode = UIViewContentMode.ScaleAspectFit;
            //btnslipimg.Image =Utils.SetImageURL UIImage.("BillVoid.png");
            btnslipimg.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(btnslipimg);
            if (havepic)
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                UIImage img = UIImage.FromFile(Path.Combine(docFolder, picpath));
                btnslipimg.Image = img;
            }
            
            //Utils.SetImage(btnslipimg, picpath);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
            View.AddSubview(BottomView);

            View.AddSubview(imgvoidbill);
            View.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("closeReciept:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            View.AddGestureRecognizer(tapGesture0);
        }
        [Export("closeReciept:")]
        public void Potal(UIGestureRecognizer sender)
        {
            if (this.NavigationController == null)
            {
                return;
            }
            POSController.clearData();
            if(posPage== null)
            {
                posPage = new POSController();
            }
            MainController.POS = true; 
            //this.NavigationController.PopToRootViewController(false);
            this.NavigationController.PopToRootViewController(false);
        }

        #region bottom view toggle
        [Export("Print:")]
        public async void Print(UIGestureRecognizer sender)
        {
            try
            {
                await Utils.Print(tranWithDetails, this);
                PostPrintcounter(tranWithDetails);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        private async void PostPrintcounter(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                TranWithDetailsLocal tran = tranWithDetails;
                //บันทึกการพิมพ์ที่ Local    
                //if (tran != null)
                //{
                //    printCounter = printCounter + 1;
                //    getTran.PrintCounterLocal = printCounter;
                //    transManage = new TransManage();
                //    var updatePrint = await transManage.UpdateTran(tran);
                //    lab.Text = (printCounter).ToString("#,##0");

                //    //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                //    UtilsAll.PostPrintCounter(Convert.ToInt32(branchID), tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                //}
                //else
                //{
                tranWithDetails.tran.PrintCounter++;
                //เพิ่มฟังก์ชันสำหรับนับการพิมพ์ PrintCounter
                UtilsAll.PostPrintCounter(Convert.ToInt32(tran.tran.SysBranchID), tran.tran.TranNo, Utils.ChangeDateTime(tran.tran.TranDate), 1, tran.tran);
                lab.Text = (tranWithDetails.tran.PrintCounter).ToString("#,##0");
                //}
            }
            catch (Exception ex)
            {
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

        }
        public int ThaiLength(string stringthai)
        {
            int len = 0;
            int l = stringthai.Length;
            for (int i = 0; i < l; ++i)
            {
                if (char.GetUnicodeCategory(stringthai[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    ++len;
            }
            return len;
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
                var r2 = new UIGraphicsPdfRenderer();
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
        public void Share(UIGestureRecognizer sender)
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


            UIActivity[] applicationActivities = null;

            var items = new List<NSObject>();

            items.Add(uIImage);

            var controller = new UIActivityViewController(items.ToArray(), applicationActivities);

            //var activityController = new UIActivityViewController(activityItems, null);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // Phone
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(controller, true, null);
            }
        }
        [Export("VoidBill:")]
        public async void VoidBill(UIGestureRecognizer sender)
        {
            var LoginType = Preferences.Get("LoginType", "");
            if (LoginType.ToLower() == "owner" || LoginType.ToLower() == "admin" || LoginType.ToLower() == "manager" || LoginType.ToLower() == "invoice" || LoginType.ToLower() == "editor")
            {
                try
                {
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("ruwantvoidbill", "Are you sure you want to void bill?"), UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => VoidBill()));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                    PresentViewController(okCancelAlertController, true, null);


                }
                catch (Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("LnVoid_Click at Bill Detail");
                    Utils.ShowMessage(ex.Message);
                }
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }

        private async void VoidBill()
        {
            try
            {
                //Update Fcancel = 1 -> void bill
                TransManage transManage = new TransManage();
                var BillVoid = this.tranWithDetails.tran;
                string tranNo = BillVoid.TranNo;

                //บิลเก่า จะสามาถ cancel ได้ต้องไม่เกิน 6 เดือน
                var DateNow = DateTime.UtcNow;

                int month = (DateNow.Month - BillVoid.TranDate.Month);
                if (month > 6)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotvoidbill6m", "The bill cannot be voided as the bill is over 6 months old."));

                    return;
                }



                if (await GabanaAPI.CheckNetWork())
                {
                    //online
                    #region online
                    var result = await GabanaAPI.DeleteDataTran(DataCashingAll.SysBranchId, tranNo, UtilsAll.ChangeDateTimeUS(BillVoid.TranDate));
                    if (result.Status)
                    {
                        BillVoid.FCancel = 1;
                        //this.tranWithDetails.tran = BillVoid;
                        Utils.ShowMessage(Utils.TextBundle("voidbillsuccess", "Void bill successful."));
                        //btnVoid.BackgroundColor = UIColor.Black;
                        Utils.SetConstant(BottomView.Constraints, NSLayoutAttribute.Height, 0);// BottomView
                        foreach (var item in BottomView.Subviews)
                        {
                            item.Hidden = true;
                        }
                        imgvoidbill.Hidden = false;



                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotvoidbill", "Unable to void bills."));
                        //.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Long).Show();
                        return;
                    }
                    #endregion
                }
                else
                {
                    //offine
                    #region offine                    
                    var getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranNo);
                    if (getTran != null)
                    {
                        getTran.FCancel = 1;
                        getTran.FWaitSending = 2;
                        getTran.WaitSendingTime = DateTime.UtcNow;
                        getTran.LastDateModified = DateTime.UtcNow;
                        getTran.TranDate = Utils.GetTranDate(getTran.TranDate);

                        var updatetran = await transManage.UpdateTran(getTran);
                        if (!updatetran)
                        {
                            Utils.ShowMessage(Utils.TextBundle("cart", "Cart"));
                            return;
                        }


                        BillVoid.FCancel = 1;
                        //this.tranWithDetails.tran = BillVoid;
                        Utils.ShowMessage(Utils.TextBundle("voidbillsuccess", "Void bill successful."));
                        Utils.SetConstant(BottomView.Constraints, NSLayoutAttribute.Height, 0);// BottomView
                        foreach (var item in BottomView.Subviews)
                        {
                            item.Hidden = true;
                        }
                        imgvoidbill.Hidden = false;
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotvoidbill", "Unable to void bills."));
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        #endregion

        void setDataField()
        {
            

            lblMerchantName.Text = MainController.merchantlocal.Name;
            //  Utils.SetImage(merchantImg, MainController.merchantlocal.LogoPath);
            //merchantImg.Image = UIImage.FromBundle("LangTH");
            lblTransNo.Text = tranWithDetails.tran.TranNo;
            var timezoneslocal = TimeZoneInfo.Local;
            var date = tranWithDetails.tran.TranDate;
            lblDate.Text = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("dd/MM/yyy HH:mm tt", new CultureInfo("en-US"));
            lblCustomer.Text = tranWithDetails.tran.CustomerName;
            lblQuantityTotal.Text = " " + tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("#,###") +" Items";
            lblCashier.Text = tranWithDetails.tran.SellerName?.ToString();
            var VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
            //if (VatType == "I")
            //{
            //    lblSubtotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.Total);
            //}
            //else
            //{
            //    lblSubtotal.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount));
            //}
            lblSubtotal.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount));


            var cash = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "CH" ).Sum(x => x.PaymentAmount);
            var qr = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "MYQR").Sum(x => x.PaymentAmount);
            var credit = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "CR" || x.PaymentType?.ToUpper() == "DR").Sum(x => x.PaymentAmount);
            var gift = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "GV" ).Sum(x => x.PaymentAmount);
            

            lblCash.Text = Utils.DisplayDecimal(cash);
            lblmyqr.Text = Utils.DisplayDecimal(qr);
            lblcredit.Text = Utils.DisplayDecimal(credit);
            lblGift.Text = Utils.DisplayDecimal(gift);
            lblChange.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
            lblTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);




            


            if (this.tranWithDetails.tranTradDiscounts.Count > 0 )
            {
                foreach (var discount in this.tranWithDetails.tranTradDiscounts)
                {
                    if (discount.DiscountType == "MD")
                    {
                        string CartDiscount;
                        double discount2, disDiscont;
                        //DiscountView.Hidden = false;
                        var check = discount.FmlDiscount.IndexOf('%');
                        if (check == -1)
                        {
                            CartDiscount = discount.FmlDiscount;
                            discount2 = Convert.ToDouble(CartDiscount);
                            lbltxtDiscount.Text = Utils.TextBundle("discount", "Discount") + " " + discount.FmlDiscount;
                            disDiscont = discount2;
                        }
                        else
                        {

                            discount2 = Convert.ToDouble(discount.FmlDiscount.Remove(check));
                            lbltxtDiscount.Text = Utils.TextBundle("discount", "Discount")+" " + discount.FmlDiscount;
                            discount2 = discount2 / 100;
                            disDiscont = Convert.ToDouble(this.tranWithDetails.tran.GrandTotal) * discount2;
                        }
                        //lbltxtDiscount.Text = "ส่วนลด " + discount2 + "%";
                        var discountval = Convert.ToDouble(discount.TradeDiscNoneVat + discount.TradeDiscHaveVat);
                        lbldiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(discountval));
                        
                        //lbldiscount.Text = "-" + discount.FmlDiscount + "%";
                        //Viewdiscount.TopAnchor.ConstraintEqualTo(ViewVat.BottomAnchor, 10).Active = true;
                        Utils.SetConstant(Viewdiscount.Constraints, NSLayoutAttribute.Height, 28);
                    }
                    if (discount.DiscountType == "PS")
                    {
                        var disMember = Convert.ToDouble(discount.TradeDiscNoneVat + discount.TradeDiscHaveVat);
                        lbltxtMemDiscount.Text = Utils.TextBundle("member", "Member")+" " + discount.FmlDiscount;
                        ////textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                        //DiscountCusView.Hidden = false;
                        lblMemdiscount.Text =  "-"+Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                        //Viewdismember.TopAnchor.ConstraintEqualTo(lbltxtSubtotal.BottomAnchor, 10).Active = true;
                        Utils.SetConstant(Viewdismember.Constraints, NSLayoutAttribute.Height, 28);
                    }
                }
            }
            if (tranWithDetails.tran.ServiceCharge != 0)
            {
                Utils.SetConstant(ViewService.Constraints, NSLayoutAttribute.Height, 28);
                //ViewService.TopAnchor.ConstraintEqualTo(Viewdiscount.BottomAnchor, 10).Active = true;
                lblService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                //lbltxtservice.Hidden = false;
                if (tranWithDetails.tran.FmlServiceCharge.Contains("%"))
                {
                    lbltxtService.Text = Utils.TextBundle("servicecharge", "Service Charge") + " " + tranWithDetails.tran.FmlServiceCharge;
                }
                else
                {
                    lbltxtService.Text = Utils.TextBundle("servicecharge", "Service Charge");
                }

            }
            if (tranWithDetails.tran.TotalVat != 0)
            {
                lbltxtVat.Text = Utils.TextBundle("vat", "Vat")+" " + Utils.DisplayDecimal(tranWithDetails.tran.TaxRate?? 0) + " %";
                lblVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                Utils.SetConstant(ViewVat.Constraints, NSLayoutAttribute.Height, 28);
                //ViewVat.TopAnchor.ConstraintEqualTo(Viewdismember.BottomAnchor, 10).Active = true;
            }

        }
        void setUpAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;



            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region headerView

            headerView.TopAnchor.ConstraintEqualTo(headerView.Superview.TopAnchor, 0).Active = true;
            headerView.LeftAnchor.ConstraintEqualTo(headerView.Superview.LeftAnchor, 0).Active = true;
            headerView.RightAnchor.ConstraintEqualTo(headerView.Superview.RightAnchor, 0).Active = true;
            headerView.HeightAnchor.ConstraintEqualTo(180).Active = true;

            merchantImg.TopAnchor.ConstraintEqualTo(merchantImg.Superview.TopAnchor,27).Active = true;
            merchantImg.LeftAnchor.ConstraintEqualTo(merchantImg.Superview.LeftAnchor,30).Active = true;
            merchantImg.HeightAnchor.ConstraintEqualTo(48).Active = true;
            merchantImg.WidthAnchor.ConstraintEqualTo(48).Active = true;

            lblMerchantName.TopAnchor.ConstraintEqualTo(lblMerchantName.Superview.TopAnchor, 42).Active = true;
            lblMerchantName.LeftAnchor.ConstraintEqualTo(merchantImg.RightAnchor, 15).Active = true;
            lblMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerchantName.RightAnchor.ConstraintEqualTo(lblMerchantName.Superview.RightAnchor, -30).Active = true;

            lblTransNo.TopAnchor.ConstraintEqualTo(merchantImg.BottomAnchor, 26).Active = true;
            lblTransNo.LeftAnchor.ConstraintEqualTo(lblTransNo.Superview.LeftAnchor, 30).Active = true;
            lblTransNo.HeightAnchor.ConstraintEqualTo(24).Active = true;
            lblTransNo.RightAnchor.ConstraintEqualTo(lblTransNo.Superview.RightAnchor, -30).Active = true;

            lblDate.TopAnchor.ConstraintEqualTo(lblTransNo.BottomAnchor, 16).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(lblDate.Superview.LeftAnchor, 30).Active = true;
            lblDate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDate.WidthAnchor.ConstraintEqualTo(200).Active = true;

            lblCustomer.TopAnchor.ConstraintEqualTo(lblTransNo.BottomAnchor, 16).Active = true;
            lblCustomer.RightAnchor.ConstraintEqualTo(lblCustomer.Superview.RightAnchor, -30).Active = true;
            lblCustomer.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCustomer.WidthAnchor.ConstraintEqualTo(200).Active = true;

            LineView1.BottomAnchor.ConstraintEqualTo(LineView1.Superview.BottomAnchor).Active = true;
            LineView1.CenterXAnchor.ConstraintEqualTo(LineView1.Superview.CenterXAnchor).Active = true;
            LineView1.HeightAnchor.ConstraintEqualTo(5).Active = true;
            LineView1.WidthAnchor.ConstraintEqualTo(325).Active = true;
            #endregion

            ReceiptCollectionview.TopAnchor.ConstraintEqualTo(headerView.BottomAnchor).Active = true;
            ReceiptCollectionview.LeftAnchor.ConstraintEqualTo(ReceiptCollectionview.Superview.LeftAnchor,30).Active = true;
            ReceiptCollectionview.RightAnchor.ConstraintEqualTo(ReceiptCollectionview.Superview.RightAnchor, -30).Active = true;
            
            var h = (40 * tranWithDetails.tranDetailItemWithToppings.Count) + ((tranWithDetails.tranDetailItemWithToppings.Sum(s => s.tranDetailItemToppings.Count) * 25));
            h += (tranWithDetails.tranDetailItemWithToppings.Where(x => !string.IsNullOrEmpty(x.tranDetailItem.Comments)).ToList().Count * 25);
            h += (tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.Discount > 0).ToList().Count * 25);
            ReceiptCollectionview.HeightAnchor.ConstraintGreaterThanOrEqualTo(h).Active = true;
            //ReceiptCollectionview.BackgroundColor = UIColor.Red;

            #region bottomView
            bottomView.TopAnchor.ConstraintEqualTo(ReceiptCollectionview.BottomAnchor).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(bottomView.Superview.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(bottomView.Superview.RightAnchor, 0).Active = true; 
            bottomView.HeightAnchor.ConstraintEqualTo(460).Active = true;
            //bottomView.BottomAnchor.ConstraintEqualTo(bottomView.Superview.BottomAnchor, 0).Active = true;

            if (havepic)
            {
                lblslip.TopAnchor.ConstraintEqualTo(bottomView.BottomAnchor, 10).Active = true;
                lblslip.LeftAnchor.ConstraintEqualTo(lblslip.Superview.LeftAnchor, 0).Active = true;
                lblslip.RightAnchor.ConstraintEqualTo(lblslip.Superview.RightAnchor, 0).Active = true;
                lblslip.HeightAnchor.ConstraintEqualTo(20).Active = true;

                btnslipimg.TopAnchor.ConstraintEqualTo(lblslip.BottomAnchor, 5).Active = true;
                btnslipimg.CenterXAnchor.ConstraintEqualTo(btnslipimg.Superview.CenterXAnchor).Active = true;
                btnslipimg.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 200).Active = true;
                btnslipimg.HeightAnchor.ConstraintEqualTo((View.Frame.Width - 200) * 1.3f).Active = true;
            }
            else
            {
                lblslip.TopAnchor.ConstraintEqualTo(bottomView.BottomAnchor, 0).Active = true;
                lblslip.LeftAnchor.ConstraintEqualTo(lblslip.Superview.LeftAnchor, 0).Active = true;
                lblslip.RightAnchor.ConstraintEqualTo(lblslip.Superview.RightAnchor, 0).Active = true;
                lblslip.HeightAnchor.ConstraintEqualTo(0).Active = true;

                btnslipimg.TopAnchor.ConstraintEqualTo(lblslip.BottomAnchor, 5).Active = true;
                btnslipimg.CenterXAnchor.ConstraintEqualTo(btnslipimg.Superview.CenterXAnchor).Active = true;
                btnslipimg.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 200).Active = true;
                btnslipimg.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }


            btnslipimg.BottomAnchor.ConstraintEqualTo(bottomView.Superview.BottomAnchor, 0).Active = true;



            lbltxtQuantityTotal.TopAnchor.ConstraintEqualTo(lbltxtQuantityTotal.Superview.TopAnchor, 16).Active = true;
            lbltxtQuantityTotal.LeftAnchor.ConstraintEqualTo(lbltxtQuantityTotal.Superview.LeftAnchor, 30).Active = true;
            lbltxtQuantityTotal.WidthAnchor.ConstraintEqualTo(115).Active = true;
            lbltxtQuantityTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblQuantityTotal.TopAnchor.ConstraintEqualTo(lblQuantityTotal.Superview.TopAnchor, 16).Active = true;
            lblQuantityTotal.LeftAnchor.ConstraintEqualTo(lbltxtQuantityTotal.RightAnchor, 0).Active = true;
            lblQuantityTotal.RightAnchor.ConstraintEqualTo(lblQuantityTotal.Superview.RightAnchor, -30).Active = true;
            lblQuantityTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            LineView2.TopAnchor.ConstraintEqualTo(lblQuantityTotal.BottomAnchor,16).Active = true;
            LineView2.CenterXAnchor.ConstraintEqualTo(LineView2.Superview.CenterXAnchor).Active = true;
            LineView2.HeightAnchor.ConstraintEqualTo(5).Active = true;
            LineView2.WidthAnchor.ConstraintEqualTo(325).Active = true;



            lbltxtSubtotal.TopAnchor.ConstraintEqualTo(LineView2.BottomAnchor, 16).Active = true;
            lbltxtSubtotal.LeftAnchor.ConstraintEqualTo(lbltxtSubtotal.Superview.LeftAnchor, 30).Active = true;
            lbltxtSubtotal.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lbltxtSubtotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblSubtotal.TopAnchor.ConstraintEqualTo(LineView2.BottomAnchor, 16).Active = true;
            lblSubtotal.RightAnchor.ConstraintEqualTo(lblSubtotal.Superview.RightAnchor, -30).Active = true;
            lblSubtotal.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblSubtotal.HeightAnchor.ConstraintEqualTo(18).Active = true;


            Viewdismember.TopAnchor.ConstraintEqualTo(lbltxtSubtotal.BottomAnchor, 0).Active = true;
            Viewdismember.LeftAnchor.ConstraintEqualTo(Viewdismember.Superview.LeftAnchor, 30).Active = true;
            Viewdismember.RightAnchor.ConstraintEqualTo(Viewdismember.Superview.RightAnchor, -30).Active = true;
            if (tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "PS").Count() > 0)
            {
                Viewdismember.HeightAnchor.ConstraintEqualTo(28).Active = true;
            }
            else
            {
                Viewdismember.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }
            


            lbltxtMemDiscount.TopAnchor.ConstraintEqualTo(Viewdismember.TopAnchor,10).Active = true;
            lbltxtMemDiscount.LeftAnchor.ConstraintEqualTo(Viewdismember.LeftAnchor).Active = true;
            //lbltxtMemDiscount.RightAnchor.ConstraintEqualTo(Viewdismember.Superview.RightAnchor).Active = true;
            lbltxtMemDiscount.BottomAnchor.ConstraintEqualTo(Viewdismember.BottomAnchor).Active = true;
            //lbltxtMemDiscount.BackgroundColor = UIColor.Red;

            lblMemdiscount.TopAnchor.ConstraintEqualTo(Viewdismember.TopAnchor,10).Active = true;
            lblMemdiscount.LeftAnchor.ConstraintEqualTo(lbltxtMemDiscount.RightAnchor , 10).Active = true;
            lblMemdiscount.RightAnchor.ConstraintEqualTo(Viewdismember.RightAnchor).Active = true;
            lblMemdiscount.BottomAnchor.ConstraintEqualTo(Viewdismember.BottomAnchor).Active = true;
            lblMemdiscount.WidthAnchor.ConstraintEqualTo(150).Active = true;
            //lblMemdiscount.BackgroundColor = UIColor.Black;


            ViewVat.TopAnchor.ConstraintEqualTo(Viewdismember.BottomAnchor, 0).Active = true;
            ViewVat.LeftAnchor.ConstraintEqualTo(ViewVat.Superview.LeftAnchor, 30).Active = true;
            ViewVat.RightAnchor.ConstraintEqualTo(ViewVat.Superview.RightAnchor, -30).Active = true;
            if (tranWithDetails.tran.TotalVat != 0)
            {
                ViewVat.HeightAnchor.ConstraintEqualTo(28).Active = true;
            }
            else
            {
                ViewVat.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }
            

            lbltxtVat.TopAnchor.ConstraintEqualTo(ViewVat.TopAnchor,10).Active = true;
            lbltxtVat.LeftAnchor.ConstraintEqualTo(ViewVat.LeftAnchor).Active = true;
            //lbltxtMemDiscount.RightAnchor.ConstraintEqualTo(Viewdismember.Superview.RightAnchor).Active = true;
            lbltxtVat.BottomAnchor.ConstraintEqualTo(ViewVat.BottomAnchor).Active = true;
            //lbltxtVat.BackgroundColor = UIColor.Red;

            lblVat.TopAnchor.ConstraintEqualTo(ViewVat.TopAnchor,10).Active = true;
            lblVat.LeftAnchor.ConstraintEqualTo(lbltxtVat.RightAnchor, 10).Active = true;
            lblVat.RightAnchor.ConstraintEqualTo(ViewVat.RightAnchor).Active = true;
            lblVat.BottomAnchor.ConstraintEqualTo(ViewVat.BottomAnchor).Active = true;
            lblVat.WidthAnchor.ConstraintEqualTo(150).Active = true;
            //lblVat.BackgroundColor = UIColor.Black;

            Viewdiscount.TopAnchor.ConstraintEqualTo(ViewVat.BottomAnchor, 0).Active = true;
            Viewdiscount.LeftAnchor.ConstraintEqualTo(Viewdiscount.Superview.LeftAnchor, 30).Active = true;
            Viewdiscount.RightAnchor.ConstraintEqualTo(Viewdiscount.Superview.RightAnchor, -30).Active = true;
            Viewdiscount.HeightAnchor.ConstraintEqualTo(0).Active = true;

            lbltxtDiscount.TopAnchor.ConstraintEqualTo(Viewdiscount.TopAnchor,10).Active = true;
            lbltxtDiscount.LeftAnchor.ConstraintEqualTo(Viewdiscount.LeftAnchor).Active = true;
            //lbltxtMemDiscount.RightAnchor.ConstraintEqualTo(Viewdismember.Superview.RightAnchor).Active = true;
            lbltxtDiscount.BottomAnchor.ConstraintEqualTo(Viewdiscount.BottomAnchor).Active = true;
            //lbltxtDiscount.BackgroundColor = UIColor.Red;

            lbldiscount.TopAnchor.ConstraintEqualTo(Viewdiscount.TopAnchor,10).Active = true;
            lbldiscount.LeftAnchor.ConstraintEqualTo(lbltxtDiscount.RightAnchor, 10).Active = true;
            lbldiscount.RightAnchor.ConstraintEqualTo(Viewdiscount.RightAnchor).Active = true;
            lbldiscount.BottomAnchor.ConstraintEqualTo(Viewdiscount.BottomAnchor).Active = true;
            lbldiscount.WidthAnchor.ConstraintEqualTo(150).Active = true;
            //lbldiscount.BackgroundColor = UIColor.Black;

            ViewService.TopAnchor.ConstraintEqualTo(Viewdiscount.BottomAnchor, 0).Active = true;
            ViewService.LeftAnchor.ConstraintEqualTo(ViewService.Superview.LeftAnchor, 30).Active = true;
            ViewService.RightAnchor.ConstraintEqualTo(ViewService.Superview.RightAnchor, -30).Active = true;
            if (tranWithDetails.tran.ServiceCharge != 0)
            {
                ViewService.HeightAnchor.ConstraintEqualTo(28).Active = true;
            }
            else
            {
                ViewService.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }

            lbltxtService.TopAnchor.ConstraintEqualTo(ViewService.TopAnchor,10).Active = true;
            lbltxtService.LeftAnchor.ConstraintEqualTo(ViewService.LeftAnchor).Active = true;
            //lbltxtMemDiscount.RightAnchor.ConstraintEqualTo(Viewdismember.Superview.RightAnchor).Active = true;
            lbltxtService.BottomAnchor.ConstraintEqualTo(ViewService.BottomAnchor).Active = true;
            //lbltxtService.BackgroundColor = UIColor.Red;

            lblService.TopAnchor.ConstraintEqualTo(ViewService.TopAnchor,10).Active = true;
            lblService.LeftAnchor.ConstraintEqualTo(lbltxtService.RightAnchor, 10).Active = true;
            lblService.RightAnchor.ConstraintEqualTo(ViewService.RightAnchor).Active = true;
            lblService.BottomAnchor.ConstraintEqualTo(ViewService.BottomAnchor).Active = true;
            lblService.WidthAnchor.ConstraintEqualTo(150).Active = true;
            //lblService.BackgroundColor = UIColor.Black;





            LineView3.TopAnchor.ConstraintEqualTo(ViewService.BottomAnchor, 16).Active = true;
            LineView3.CenterXAnchor.ConstraintEqualTo(LineView3.Superview.CenterXAnchor).Active = true;
            LineView3.HeightAnchor.ConstraintEqualTo(5).Active = true;
            LineView3.WidthAnchor.ConstraintEqualTo(325).Active = true;

            lbltxtTotal.TopAnchor.ConstraintEqualTo(LineView3.BottomAnchor, 16).Active = true;
            lbltxtTotal.LeftAnchor.ConstraintEqualTo(lbltxtTotal.Superview.LeftAnchor, 30).Active = true;
            lbltxtTotal.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lbltxtTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblTotal.TopAnchor.ConstraintEqualTo(LineView3.BottomAnchor, 16).Active = true;
            lblTotal.RightAnchor.ConstraintEqualTo(lblTotal.Superview.RightAnchor, -30).Active = true;
            lblTotal.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            LineView4.TopAnchor.ConstraintEqualTo(lblTotal.BottomAnchor, 16).Active = true;
            LineView4.CenterXAnchor.ConstraintEqualTo(LineView4.Superview.CenterXAnchor).Active = true;
            LineView4.HeightAnchor.ConstraintEqualTo(5).Active = true;
            LineView4.WidthAnchor.ConstraintEqualTo(325).Active = true;


            var cash = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "CH" ).Sum(x => x.PaymentAmount);
            var qr = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "MYQR").Sum(x => x.PaymentAmount);
            var credit = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "CR" || x.PaymentType?.ToUpper() == "DR").Sum(x => x.PaymentAmount);
            var gift = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "GV" ).Sum(x => x.PaymentAmount);
            
            lbltxtCash.LeftAnchor.ConstraintEqualTo(lbltxtCash.Superview.LeftAnchor, 30).Active = true;
            lbltxtCash.WidthAnchor.ConstraintEqualTo(150).Active = true;
            
            lblCash.RightAnchor.ConstraintEqualTo(lblTotal.Superview.RightAnchor, -30).Active = true;
            lblCash.WidthAnchor.ConstraintEqualTo(150).Active = true;

            if (cash > 0)
            {
                lbltxtCash.TopAnchor.ConstraintEqualTo(LineView4.BottomAnchor, 16).Active = true;
                lbltxtCash.HeightAnchor.ConstraintEqualTo(18).Active = true;

                lblCash.TopAnchor.ConstraintEqualTo(LineView4.BottomAnchor, 16).Active = true;
                lblCash.HeightAnchor.ConstraintEqualTo(18).Active = true;

            }
            else
            {
                lbltxtCash.TopAnchor.ConstraintEqualTo(LineView4.BottomAnchor, 0).Active = true;
                lbltxtCash.HeightAnchor.ConstraintEqualTo(0).Active = true;

                lblCash.TopAnchor.ConstraintEqualTo(LineView4.BottomAnchor, 0).Active = true;
                lblCash.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }

            lbltxtGift.LeftAnchor.ConstraintEqualTo(lbltxtCash.Superview.LeftAnchor, 30).Active = true;
            lbltxtGift.WidthAnchor.ConstraintEqualTo(150).Active = true;
                        
            lblGift.RightAnchor.ConstraintEqualTo(lblTotal.Superview.RightAnchor, -30).Active = true;
            lblGift.WidthAnchor.ConstraintEqualTo(150).Active = true;
            
            if (gift > 0)
            {
                lbltxtGift.TopAnchor.ConstraintEqualTo(lblCash.BottomAnchor, 10).Active = true;
                lbltxtGift.HeightAnchor.ConstraintEqualTo(18).Active = true;

                lblGift.TopAnchor.ConstraintEqualTo(lblCash.BottomAnchor, 10).Active = true;
                lblGift.HeightAnchor.ConstraintEqualTo(18).Active = true;
            }
            else
            {
                lbltxtGift.TopAnchor.ConstraintEqualTo(lblCash.BottomAnchor, 0).Active = true;
                lbltxtGift.HeightAnchor.ConstraintEqualTo(0).Active = true;

                lblGift.TopAnchor.ConstraintEqualTo(lblCash.BottomAnchor, 0).Active = true;
                lblGift.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }

            lbltxtmyqr.LeftAnchor.ConstraintEqualTo(lbltxtCash.Superview.LeftAnchor, 30).Active = true;
            lbltxtmyqr.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblmyqr.RightAnchor.ConstraintEqualTo(lblTotal.Superview.RightAnchor, -30).Active = true;
            lblmyqr.WidthAnchor.ConstraintEqualTo(150).Active = true;

            if (qr > 0)
            {
                lbltxtmyqr.TopAnchor.ConstraintEqualTo(lbltxtGift.BottomAnchor, 10).Active = true;
                lbltxtmyqr.HeightAnchor.ConstraintEqualTo(18).Active = true;

                lblmyqr.TopAnchor.ConstraintEqualTo(lbltxtGift.BottomAnchor, 10).Active = true;
                lblmyqr.HeightAnchor.ConstraintEqualTo(18).Active = true;
            }
            else
            {
                lbltxtmyqr.TopAnchor.ConstraintEqualTo(lbltxtGift.BottomAnchor, 0).Active = true;
                lbltxtmyqr.HeightAnchor.ConstraintEqualTo(0).Active = true;

                lblmyqr.TopAnchor.ConstraintEqualTo(lbltxtGift.BottomAnchor, 0).Active = true;
                lblmyqr.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }


           
            lbltxtcredit.LeftAnchor.ConstraintEqualTo(lbltxtCash.Superview.LeftAnchor, 30).Active = true;
            lbltxtcredit.WidthAnchor.ConstraintEqualTo(150).Active = true;
            
            lblcredit.RightAnchor.ConstraintEqualTo(lblTotal.Superview.RightAnchor, -30).Active = true;
            lblcredit.WidthAnchor.ConstraintEqualTo(150).Active = true;
            

            if (credit > 0)
            {
                lbltxtcredit.TopAnchor.ConstraintEqualTo(lbltxtmyqr.BottomAnchor, 10).Active = true;
                lbltxtcredit.HeightAnchor.ConstraintEqualTo(18).Active = true;

                lblcredit.TopAnchor.ConstraintEqualTo(lbltxtmyqr.BottomAnchor, 10).Active = true;
                lblcredit.HeightAnchor.ConstraintEqualTo(18).Active = true;
                var cre = tranWithDetails.tranPayments.Where(x => x.PaymentType?.ToUpper() == "CR" || x.PaymentType?.ToUpper() == "DR").FirstOrDefault();
                if (cre.PaymentType.ToUpper() == "CR")
                {
                    lbltxtcredit.Text = Utils.TextBundle("creditcard", "Credit Card");
                }
                else
                {
                    lbltxtcredit.Text = Utils.TextBundle("debitcard", "Debit Card");
                }
            }
            else
            {
                lbltxtcredit.TopAnchor.ConstraintEqualTo(lbltxtmyqr.BottomAnchor, 0).Active = true;
                lbltxtcredit.HeightAnchor.ConstraintEqualTo(0).Active = true;

                lblcredit.TopAnchor.ConstraintEqualTo(lbltxtmyqr.BottomAnchor, 0).Active = true;
                lblcredit.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }


            lbltxtChange.TopAnchor.ConstraintEqualTo(lblcredit.BottomAnchor, 10).Active = true;
            lbltxtChange.LeftAnchor.ConstraintEqualTo(lbltxtChange.Superview.LeftAnchor, 30).Active = true;
            lbltxtChange.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lbltxtChange.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblChange.TopAnchor.ConstraintEqualTo(lblcredit.BottomAnchor, 10).Active = true;
            lblChange.RightAnchor.ConstraintEqualTo(lblChange.Superview.RightAnchor, -30).Active = true;
            lblChange.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblChange.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbltxtCashier.TopAnchor.ConstraintEqualTo(lbltxtChange.BottomAnchor, 10).Active = true;
            lbltxtCashier.LeftAnchor.ConstraintEqualTo(lbltxtCashier.Superview.LeftAnchor, 30).Active = true;
            lbltxtCashier.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lbltxtCashier.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblCashier.TopAnchor.ConstraintEqualTo(lbltxtChange.BottomAnchor, 10).Active = true;
            lblCashier.RightAnchor.ConstraintEqualTo(lblCashier.Superview.RightAnchor, -30).Active = true;
            lblCashier.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblCashier.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //lbltxtremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 0).Active = true;
            lbltxtremark.LeftAnchor.ConstraintEqualTo(lbltxtremark.Superview.LeftAnchor, 30).Active = true;
            lbltxtremark.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lbltxtremark.HeightAnchor.ConstraintEqualTo(0).Active = true;

            //lblremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 0).Active = true;
            lblremark.RightAnchor.ConstraintEqualTo(lblremark.Superview.RightAnchor, -30).Active = true;
            lblremark.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblremark.HeightAnchor.ConstraintEqualTo(0).Active = true;


            if (!string.IsNullOrEmpty(tranWithDetails.tran.Comments))
            {
                lbltxtremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 10).Active = true;
                lblremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 10).Active = true;
                Utils.SetConstant(lbltxtremark.Constraints,NSLayoutAttribute.Height,18);
                Utils.SetConstant(lblremark.Constraints, NSLayoutAttribute.Height, 18);
                lbltxtremark.Text = Utils.TextBundle("remark", "Remark");
                lblremark.Text = tranWithDetails.tran.Comments;
            }
            else
            {
                lbltxtremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 0).Active = true;
                lblremark.TopAnchor.ConstraintEqualTo(lbltxtCashier.BottomAnchor, 0).Active = true;
                Utils.SetConstant(lbltxtremark.Constraints, NSLayoutAttribute.Height, 0);
                Utils.SetConstant(lblremark.Constraints, NSLayoutAttribute.Height, 0);

            }

            lblThank.TopAnchor.ConstraintEqualTo(lblCashier.BottomAnchor, 36).Active = true;
            lblThank.CenterXAnchor.ConstraintEqualTo(lblThank.Superview.CenterXAnchor).Active = true;
            //lblThank.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblThank.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblPowerby.TopAnchor.ConstraintEqualTo(lblThank.BottomAnchor, 5).Active = true;
            lblPowerby.CenterXAnchor.ConstraintEqualTo(lblPowerby.Superview.CenterXAnchor).Active = true;
            //lblThank.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblPowerby.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion


            imgvoidbill.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            imgvoidbill.CenterYAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imgvoidbill.HeightAnchor.ConstraintEqualTo(200).Active = true;
            imgvoidbill.WidthAnchor.ConstraintEqualTo(200).Active = true;

            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(((View.Frame.Width - 60) / 5) + 20).Active = true;
            BottomView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            BottomView.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 60).Active = true;

            #region btnPrint
            btnPrint.LeftAnchor.ConstraintEqualTo(BottomView.LeftAnchor, 10).Active = true;
            btnPrint.CenterYAnchor.ConstraintEqualTo(btnPrint.Superview.CenterYAnchor).Active = true;
            btnPrint.HeightAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;
            btnPrint.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;

            btnPrintImg.TopAnchor.ConstraintEqualTo(btnPrintImg.Superview.TopAnchor, 9).Active = true;
            btnPrintImg.CenterXAnchor.ConstraintEqualTo(btnPrintImg.Superview.CenterXAnchor).Active = true;
            btnPrintImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnPrintImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lbl_btnPrint.TopAnchor.ConstraintEqualTo(btnPrintImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_btnPrint.CenterXAnchor.ConstraintEqualTo(btnPrintImg.Superview.CenterXAnchor).Active = true;
            lbl_btnPrint.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbl_btnPrint.WidthAnchor.ConstraintEqualTo(50).Active = true;
            #endregion

            #region btnPDF
            btnPDF.LeftAnchor.ConstraintEqualTo(btnPrint.RightAnchor, 10).Active = true;
            btnPDF.CenterYAnchor.ConstraintEqualTo(btnPDF.Superview.CenterYAnchor).Active = true;
            btnPDF.HeightAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;
            btnPDF.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;

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
            btnEmail.LeftAnchor.ConstraintEqualTo(btnPDF.RightAnchor, 10).Active = true;
            btnEmail.CenterYAnchor.ConstraintEqualTo(btnEmail.Superview.CenterYAnchor).Active = true;
            btnEmail.HeightAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;
            btnEmail.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;

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
            btnShare.LeftAnchor.ConstraintEqualTo(btnEmail.RightAnchor, 10).Active = true;
            btnShare.CenterYAnchor.ConstraintEqualTo(btnShare.Superview.CenterYAnchor).Active = true;
            btnShare.HeightAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;
            btnShare.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 60) / 5).Active = true;

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
    }
}