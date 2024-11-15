using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.POS.Cart
{
    public partial class PaymentController : UIViewController
    {
        UIView PaymentHeaderView,PaymentMenuView,BottomView, myQRView, QrCashView, QrCreditView, WechatView;
        UILabel lblTextPaymentAmount, lblPaymentAmount, lblBath;
        UIButton btnNext;
        public double  amount;
        UIView CashView, GiftView, CreditView, ePayView, QRView , SaveBillView;
        UILabel lblCash, lblGift, lblCredit, lblePay, lblQR  , lblSaveBill, lblmyQR, lblQrCash, lblQrCredit, lblWechat;
        UIImageView CashImg, GiftImg, CreditImg, ePayImg, QRImg , SaveBillImg, myQRImg, QrCashImg, QrCreditImg, WechatImg;
        Model.TranWithDetailsLocal tranWithDetails;
        TransManage transManage = new TransManage();
        public static Customer 
            edCustomer = null;
        POSCustomerController selectCustomerPage = null;
        Char payment; 
        public PaymentController()
        {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            POSController.tranWithDetails = this.tranWithDetails;

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("payment", "Items"));
                decimal paymentAmount = 0;
                paymentAmount = tranWithDetails.tranPayments.Sum(x => x.PaymentAmount);

                //amount คือ ยอดที่ต้องจ่าย  
                amount = Convert.ToDouble(tranWithDetails.tran.GrandTotal - paymentAmount);
                lblPaymentAmount.Text = Utils.DisplayDecimal((decimal)amount);
                //this.NavigationController.SetNavigationBarHidden(false, false);
                //this.NavigationController.NavigationBar.TintColor = UIColor.White;
                if (POSController.SelectedCustomer != null)
                {
                    UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                    uIImageView.Image = UIImage.FromBundle("CustB");
                    UIButton btn = new UIButton();
                    //btn.SetImage(UIImage.FromBundle("Cust"), default);
                    btn.ImageView.BackgroundColor = UIColor.Black;
                    btn.Frame = new CGRect(0, 0, 200, 50);
                    btn.Layer.CornerRadius = 5f;
                    btn.Layer.BorderWidth = 0.5f;
                    btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    //btn.BackgroundColor = UIColor.Red;
                    UILabel lab = new UILabel();
                    lab.TextColor = UIColor.FromRGB(0, 149, 218);
                    lab.Text = POSController.SelectedCustomer.CustomerName;
                    lab.TextAlignment = UITextAlignment.Right;
                    lab.TranslatesAutoresizingMaskIntoConstraints = false;
                    uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                    btn.AddSubview(uIImageView);
                    btn.AddSubview(lab);

                    lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                    lab.HeightAnchor.ConstraintEqualTo(50).Active = true;
                    lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                    uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                    uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                    uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;

                    UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                    btn.TouchUpInside += (sender, e) => {
                        // open select customer page
                        //if (selectCustomerPage == null)
                        //{
                            var selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
                    };
                    this.NavigationItem.RightBarButtonItem = selectCustomer;
                }
                else
                {
                    UIBarButtonItem selectCustomer = new UIBarButtonItem();
                    selectCustomer.Image = UIImage.FromBundle("Cust");
                    selectCustomer.Clicked += (sender, e) => {
                        // open select customer page
                        //if (selectCustomerPage == null)
                        //{
                            selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
                    };
                    this.NavigationItem.RightBarButtonItem = selectCustomer;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIBarButtonItem selectCustomer = new UIBarButtonItem();
            selectCustomer.Image = UIImage.FromBundle("Cust");
            selectCustomer.Clicked += (sender, e) => {
                // open select customer page
                //if (selectCustomerPage == null)
                //{
                    selectCustomerPage = new POSCustomerController();
                //}
                this.NavigationController.PushViewController(selectCustomerPage, false);
            };
            this.NavigationItem.RightBarButtonItem = selectCustomer;
            View.BackgroundColor = UIColor.White;
            payment = PaymentType.Cash;

            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            //this.NavigationController.NavigationBar.TopItem.Title = "Payment";
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("payment", "Items"));

            #region PaymentHeaderView
            PaymentHeaderView = new UIView();
            PaymentHeaderView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            PaymentHeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(PaymentHeaderView);

            lblTextPaymentAmount = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = new UIColor(red: 162 / 225f, green: 162 / 255f, blue: 162 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTextPaymentAmount.Text = Utils.TextBundle("paymentamount", "Items");
            lblTextPaymentAmount.Font = lblTextPaymentAmount.Font.WithSize(15);
            PaymentHeaderView.AddSubview(lblTextPaymentAmount);

            lblPaymentAmount = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = new UIColor(red: 0 / 225f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            
            lblPaymentAmount.Font = lblPaymentAmount.Font.WithSize(60);
            PaymentHeaderView.AddSubview(lblPaymentAmount);

            lblBath = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBath.Text = Utils.TextCURRENCYSYMBOLS(DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS);
            lblBath.Font = lblBath.Font.WithSize(15);
            PaymentHeaderView.AddSubview(lblBath);
            #endregion

            #region PaymentMenuView
            PaymentMenuView = new UIView();
            PaymentMenuView.BackgroundColor = UIColor.White;
            PaymentMenuView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(PaymentMenuView);
            //CashView, GiftView, CreditView, ePayView, QRView;
            #region CashView
            CashView = new UIView();
            CashView.BackgroundColor = UIColor.White;
            CashView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(CashView);

            CashImg = new UIImageView();
            CashImg.Image = UIImage.FromBundle("PaymentCash");
            CashImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            CashImg.BackgroundColor = UIColor.White;
            CashImg.TranslatesAutoresizingMaskIntoConstraints = false;
            CashView.AddSubview(CashImg);

            lblCash = new UILabel();
            lblCash.Text = Utils.TextBundle("cash", "Items");
            lblCash.TranslatesAutoresizingMaskIntoConstraints = false;
            lblCash.TextAlignment = UITextAlignment.Center;
            lblCash.Font = lblCash.Font.WithSize(15);
            lblCash.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            CashView.AddSubview(lblCash);

            CashView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Cash:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            CashView.AddGestureRecognizer(tapGesture0);
            #endregion 

            #region SaveBillView
            SaveBillView = new UIView();
            SaveBillView.BackgroundColor = UIColor.White;
            SaveBillView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(SaveBillView);

            SaveBillImg = new UIImageView();
            SaveBillImg.Image = UIImage.FromBundle("PaymentSaveOrder");
            SaveBillImg.BackgroundColor = UIColor.White;
            SaveBillImg.TranslatesAutoresizingMaskIntoConstraints = false;
            SaveBillView.AddSubview(SaveBillImg);

            lblSaveBill = new UILabel();
            lblSaveBill.Text = Utils.TextBundle("saveorder", "Items");
            lblSaveBill.TranslatesAutoresizingMaskIntoConstraints = false;
            lblSaveBill.TextAlignment = UITextAlignment.Center;
            lblSaveBill.Font = lblCash.Font.WithSize(15);
            lblSaveBill.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            SaveBillView.AddSubview(lblSaveBill);

            SaveBillView.UserInteractionEnabled = true;
            var tapGesture9 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("save:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            SaveBillView.AddGestureRecognizer(tapGesture9);
            #endregion



            #region GiftView
            GiftView = new UIView();
            GiftView.BackgroundColor = UIColor.White;
            GiftView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(GiftView);

            GiftImg = new UIImageView();
            GiftImg.Image = UIImage.FromBundle("PaymentGiftVoucher");
            GiftImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            GiftImg.BackgroundColor = UIColor.White;
            GiftImg.TranslatesAutoresizingMaskIntoConstraints = false;
            GiftView.AddSubview(GiftImg);

            lblGift = new UILabel();
            lblGift.Text = Utils.TextBundle("giftvoucher", "Items");
            lblGift.TranslatesAutoresizingMaskIntoConstraints = false;
            lblGift.TextAlignment = UITextAlignment.Center;
            lblGift.Font = lblGift.Font.WithSize(15);
            lblGift.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            GiftView.AddSubview(lblGift);

            GiftView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Gift:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            GiftView.AddGestureRecognizer(tapGesture1);
            #endregion

            #region CreditView
            CreditView = new UIView();
            CreditView.BackgroundColor = UIColor.White;
            CreditView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(CreditView);

            CreditImg = new UIImageView();
            CreditImg.Image = UIImage.FromBundle("PaymentCredit");
            CreditImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            CreditImg.BackgroundColor = UIColor.White;
            CreditImg.TranslatesAutoresizingMaskIntoConstraints = false;
            CreditView.AddSubview(CreditImg);

            lblCredit = new UILabel();
            lblCredit.Text = Utils.TextBundle("creditdebit", "Items");
            lblCredit.TranslatesAutoresizingMaskIntoConstraints = false;
            lblCredit.TextAlignment = UITextAlignment.Center;
            lblCredit.Font = lblCredit.Font.WithSize(15);
            lblCredit.LineBreakMode = UILineBreakMode.WordWrap;
            lblCredit.Lines = 2;
            lblCredit.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            CreditView.AddSubview(lblCredit);

            CreditView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Credit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            CreditView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region myQRView
            myQRView = new UIView();
            myQRView.BackgroundColor = UIColor.White;
            myQRView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(myQRView);

            myQRImg = new UIImageView();
            myQRImg.Image = UIImage.FromBundle("PaymentQr");
            myQRImg.BackgroundColor = UIColor.White;
            myQRImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            myQRImg.TranslatesAutoresizingMaskIntoConstraints = false;
            myQRView.AddSubview(myQRImg);

            lblmyQR = new UILabel();
            lblmyQR.Text = Utils.TextBundle("myqr", "Items");
            lblmyQR.TranslatesAutoresizingMaskIntoConstraints = false;
            lblmyQR.TextAlignment = UITextAlignment.Center;
            lblmyQR.Font = lblCredit.Font.WithSize(15);
            lblmyQR.LineBreakMode = UILineBreakMode.WordWrap;
            lblmyQR.Lines = 2;
            lblmyQR.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            myQRView.AddSubview(lblmyQR);

            myQRView.UserInteractionEnabled = true;
            var tapGesture8 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("MyQr:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            myQRView.AddGestureRecognizer(tapGesture8);
            #endregion

            #region QrCashView
            QrCashView = new UIView();
            QrCashView.BackgroundColor = UIColor.White;
            QrCashView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(QrCashView);

            QrCashImg = new UIImageView();
            QrCashImg.Image = UIImage.FromBundle("PaymentQRCash");
            QrCashImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            QrCashImg.BackgroundColor = UIColor.White;
            QrCashImg.TranslatesAutoresizingMaskIntoConstraints = false;
            QrCashView.AddSubview(QrCashImg);
            QrCashView.Alpha = 0.5f;

            lblQrCash = new UILabel();
            lblQrCash.Text = Utils.TextBundle("qrcash", "Items");
            lblQrCash.TranslatesAutoresizingMaskIntoConstraints = false;
            lblQrCash.TextAlignment = UITextAlignment.Center;
            lblQrCash.Font = lblCredit.Font.WithSize(15);
            lblQrCash.LineBreakMode = UILineBreakMode.WordWrap;
            lblQrCash.Lines = 2;
            lblQrCash.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            QrCashView.AddSubview(lblQrCash);

            QrCashView.UserInteractionEnabled = true;
            var tapGesture7 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("QRcash:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            QrCashView.AddGestureRecognizer(tapGesture7);
            #endregion

            #region QrCreditView
            QrCreditView = new UIView();
            QrCreditView.BackgroundColor = UIColor.White;
            QrCreditView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(QrCreditView);

            QrCreditImg = new UIImageView();
            QrCreditImg.Image = UIImage.FromBundle("PaymentQRCredit");
            QrCreditImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            QrCreditImg.BackgroundColor = UIColor.White;
            QrCreditImg.TranslatesAutoresizingMaskIntoConstraints = false;
            QrCreditView.AddSubview(QrCreditImg);
            QrCreditView.Alpha = 0.5f;

            lblQrCredit = new UILabel();
            lblQrCredit.Text = Utils.TextBundle("qrcredit", "Items");
            lblQrCredit.TranslatesAutoresizingMaskIntoConstraints = false;
            lblQrCredit.TextAlignment = UITextAlignment.Center;
            lblQrCredit.Font = lblCredit.Font.WithSize(15);
            lblQrCredit.LineBreakMode = UILineBreakMode.WordWrap;
            lblQrCredit.Lines = 2;
            lblQrCredit.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            QrCreditView.AddSubview(lblQrCredit);

            //QrCreditView.UserInteractionEnabled = true;
            //var tapGesture5 = new UITapGestureRecognizer(this,
            //   new ObjCRuntime.Selector("Credit:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //QrCreditView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region WechatView
            WechatView = new UIView();
            WechatView.BackgroundColor = UIColor.White;
            WechatView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentMenuView.AddSubview(WechatView);

            WechatImg = new UIImageView();
            WechatImg.Image = UIImage.FromBundle("PaymentQRWeChat");
            WechatImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            WechatImg.BackgroundColor = UIColor.White;
            WechatImg.TranslatesAutoresizingMaskIntoConstraints = false;
            WechatView.AddSubview(WechatImg);
            WechatView.Alpha = 0.5f;

            lblWechat = new UILabel();
            lblWechat.Text = Utils.TextBundle("wechatpay", "Items");
            lblWechat.TranslatesAutoresizingMaskIntoConstraints = false;
            lblWechat.TextAlignment = UITextAlignment.Center;
            lblWechat.Font = lblCredit.Font.WithSize(15);
            lblWechat.LineBreakMode = UILineBreakMode.WordWrap;
            lblWechat.Lines = 2;
            lblWechat.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1);
            WechatView.AddSubview(lblWechat);

            //WechatView.UserInteractionEnabled = true;
            //var tapGesture6 = new UITapGestureRecognizer(this,
            //   new ObjCRuntime.Selector("Credit:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //WechatView.AddGestureRecognizer(tapGesture2);
            #endregion



            #endregion

            //#region BottomView
            //BottomView = new UIView();
            //BottomView.BackgroundColor = UIColor.White;
            //BottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            //View.AddSubview(BottomView);

            //btnNext = new UIButton();
            //btnNext.Layer.CornerRadius = 5;
            //btnNext.SetTitle("Next", UIControlState.Normal);
            //btnNext.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            //btnNext.SetTitleColor(UIColor.White, UIControlState.Normal);
            //btnNext.TranslatesAutoresizingMaskIntoConstraints = false;
            //btnNext.TouchUpInside += (sender, e) => {
            //    switch (payment)
            //    {
            //        case PaymentType.Cash:
            //            CashController cashController = new CashController(); 
            //            this.NavigationController.PushViewController(cashController, false);
            //            break;
            //        case PaymentType.CreaditCard:

            //            break;
            //        case PaymentType.ePayment:

            //            break;
            //        case PaymentType.GiftVoucher:

            //            break;
            //        default:
            //            break;
            //    }
            //};
            //View.AddSubview(btnNext);
            //#endregion
            checkPaymentType();
            SetAutoLayout();
        }
        private void checkPaymentType()
        {

        }

        
        void SetAutoLayout()
        {
            #region PaymentHeaderView
            PaymentHeaderView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            PaymentHeaderView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PaymentHeaderView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            PaymentHeaderView.HeightAnchor.ConstraintEqualTo((((int)View.SafeAreaLayoutGuide.LayoutFrame.Height - (int)this.NavigationController.NavigationBar.Frame.Height) * 250) / 1000).Active = true;

            lblPaymentAmount.CenterXAnchor.ConstraintEqualTo(PaymentHeaderView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblPaymentAmount.CenterYAnchor.ConstraintEqualTo(PaymentHeaderView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPaymentAmount.HeightAnchor.ConstraintEqualTo(84).Active = true;

            lblTextPaymentAmount.CenterXAnchor.ConstraintEqualTo(PaymentHeaderView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblTextPaymentAmount.BottomAnchor.ConstraintEqualTo(lblPaymentAmount.SafeAreaLayoutGuide.TopAnchor,-5).Active = true;
            lblTextPaymentAmount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblBath.CenterXAnchor.ConstraintEqualTo(PaymentHeaderView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblBath.TopAnchor.ConstraintEqualTo(lblPaymentAmount.SafeAreaLayoutGuide.BottomAnchor,5).Active = true;
            lblBath.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
            
            #region PaymentMenuView
            PaymentMenuView.TopAnchor.ConstraintEqualTo(PaymentHeaderView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            PaymentMenuView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PaymentMenuView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            PaymentMenuView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            var height = (((((int)View.SafeAreaLayoutGuide.LayoutFrame.Height-(int)this.NavigationController.NavigationBar.Frame.Height) * 720) / 1000)/4)-10;
            #region CashView
            CashView.TopAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            CashView.LeftAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            CashView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            CashView.WidthAnchor.ConstraintEqualTo((View.Frame.Width)/2).Active = true;
            //CashView.BackgroundColor = UIColor.Blue;


            CashImg.CenterYAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.CenterYAnchor,-20).Active = true;
            CashImg.CenterXAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            CashImg.HeightAnchor.ConstraintEqualTo(height-40).Active = true;
            CashImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;
            //CashImg.BackgroundColor = UIColor.Red; 

            lblCash.TopAnchor.ConstraintEqualTo(CashImg.SafeAreaLayoutGuide.BottomAnchor,6).Active = true;
            lblCash.CenterXAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblCash.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region SavebillView
            SaveBillView.TopAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            SaveBillView.LeftAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            SaveBillView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            SaveBillView.WidthAnchor.ConstraintEqualTo((View.Frame.Width-1) / 2).Active = true;

            SaveBillImg.CenterYAnchor.ConstraintEqualTo(SaveBillView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            SaveBillImg.CenterXAnchor.ConstraintEqualTo(SaveBillView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            SaveBillImg.HeightAnchor.ConstraintEqualTo(height-40).Active = true;
            SaveBillImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblSaveBill.TopAnchor.ConstraintEqualTo(SaveBillImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblSaveBill.CenterXAnchor.ConstraintEqualTo(SaveBillView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblSaveBill.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region GiftView
            GiftView.TopAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            GiftView.LeftAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            GiftView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            GiftView.WidthAnchor.ConstraintEqualTo((View.Frame.Width) / 2).Active = true;

            GiftImg.CenterYAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            GiftImg.CenterXAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            GiftImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            GiftImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblGift.TopAnchor.ConstraintEqualTo(GiftImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblGift.CenterXAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblGift.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region CreditView
            CreditView.TopAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CreditView.LeftAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            CreditView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            CreditView.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 1) / 2).Active = true;

            CreditImg.CenterYAnchor.ConstraintEqualTo(CreditView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            CreditImg.CenterXAnchor.ConstraintEqualTo(CreditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            CreditImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            CreditImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblCredit.TopAnchor.ConstraintEqualTo(CreditImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblCredit.CenterXAnchor.ConstraintEqualTo(CreditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblCredit.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region myQRView
            myQRView.TopAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            myQRView.LeftAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            myQRView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            myQRView.WidthAnchor.ConstraintEqualTo((View.Frame.Width) / 2).Active = true;

            myQRImg.CenterYAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            myQRImg.CenterXAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            myQRImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            myQRImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblmyQR.TopAnchor.ConstraintEqualTo(myQRImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblmyQR.CenterXAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblmyQR.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region QrCashView
            QrCashView.TopAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            QrCashView.LeftAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            QrCashView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            QrCashView.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 1) / 2).Active = true;

            QrCashImg.CenterYAnchor.ConstraintEqualTo(QrCashView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            QrCashImg.CenterXAnchor.ConstraintEqualTo(QrCashView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            QrCashImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            QrCashImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblQrCash.TopAnchor.ConstraintEqualTo(QrCashImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblQrCash.CenterXAnchor.ConstraintEqualTo(QrCashView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblQrCash.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region QrCreditView
            QrCreditView.TopAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            QrCreditView.LeftAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            QrCreditView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            QrCreditView.WidthAnchor.ConstraintEqualTo((View.Frame.Width) / 2).Active = true;

            QrCreditImg.CenterYAnchor.ConstraintEqualTo(QrCreditView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            QrCreditImg.CenterXAnchor.ConstraintEqualTo(QrCreditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            QrCreditImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            QrCreditImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblQrCredit.TopAnchor.ConstraintEqualTo(QrCreditImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblQrCredit.CenterXAnchor.ConstraintEqualTo(QrCreditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblQrCredit.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region WechatView
            WechatView.TopAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            WechatView.LeftAnchor.ConstraintEqualTo(myQRView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            WechatView.HeightAnchor.ConstraintEqualTo(height).Active = true;
            WechatView.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 1) / 2).Active = true;

            WechatImg.CenterYAnchor.ConstraintEqualTo(WechatView.SafeAreaLayoutGuide.CenterYAnchor, -20).Active = true;
            WechatImg.CenterXAnchor.ConstraintEqualTo(WechatView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            WechatImg.HeightAnchor.ConstraintEqualTo(height - 40).Active = true;
            WechatImg.WidthAnchor.ConstraintEqualTo(height - 40).Active = true;

            lblWechat.TopAnchor.ConstraintEqualTo(WechatImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblWechat.CenterXAnchor.ConstraintEqualTo(WechatView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblWechat.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            //#region ePayView
            //ePayView.TopAnchor.ConstraintEqualTo(CashView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            //ePayView.LeftAnchor.ConstraintEqualTo(PaymentMenuView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //ePayView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 154) / 1000).Active = true;
            //ePayView.WidthAnchor.ConstraintEqualTo(125).Active = true;

            //ePayImg.CenterYAnchor.ConstraintEqualTo(ePayView.SafeAreaLayoutGuide.CenterYAnchor,-25).Active = true;
            //ePayImg.CenterXAnchor.ConstraintEqualTo(ePayView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //ePayImg.HeightAnchor.ConstraintEqualTo(51).Active = true;
            //ePayImg.WidthAnchor.ConstraintEqualTo(51).Active = true;

            //lblePay.TopAnchor.ConstraintEqualTo(ePayImg.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            //lblePay.CenterXAnchor.ConstraintEqualTo(ePayView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //lblePay.WidthAnchor.ConstraintEqualTo(100).Active = true;
            //#endregion

            //#region QRView
            //QRView.TopAnchor.ConstraintEqualTo(GiftView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            //QRView.LeftAnchor.ConstraintEqualTo(ePayView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            //QRView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*154)/1000).Active = true;
            //QRView.WidthAnchor.ConstraintEqualTo(125).Active = true;

            //QRImg.CenterYAnchor.ConstraintEqualTo(QRView.SafeAreaLayoutGuide.CenterYAnchor,-25).Active = true;
            //QRImg.CenterXAnchor.ConstraintEqualTo(QRView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //QRImg.HeightAnchor.ConstraintEqualTo(51).Active = true;
            //QRImg.WidthAnchor.ConstraintEqualTo(51).Active = true;

            //lblQR.TopAnchor.ConstraintEqualTo(QRImg.SafeAreaLayoutGuide.BottomAnchor, 12).Active = true;
            //lblQR.CenterXAnchor.ConstraintEqualTo(QRView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //lblQR.WidthAnchor.ConstraintEqualTo(100).Active = true;
            //#endregion

            //#region BottomView
            //BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            //BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            //btnNext.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            //btnNext.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            //btnNext.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            //btnNext.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //#endregion

            #endregion
        }
        public class PaymentType
        {
            public const char Cash = 'C';
            public const char GiftVoucher = 'G';
            public const char CreaditCard = 'D';
            public const char ePayment = 'E';

        }
        [Export("Cash:")]
        public void Cash(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("cash", "Items"));
            CashController CashPage = new CashController();
            this.NavigationController.PushViewController(CashPage, false);
        }
        [Export("Gift:")]
        public void Gift(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("giftvoucher", "Items"));
            var giftpage = new GiftVoucherPayController();
            this.NavigationController.PushViewController(giftpage, false);
            
        }
        [Export("MyQr:")]
        public void MyQr(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("myqr", "Items"));
            var myQrController = new MyQrController(true);
            this.NavigationController.PushViewController(myQrController, false);
        }
        [Export("Credit:")]
        public void Credit(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("creditcard", "Items"));
            var page = new CreditController();
            this.NavigationController.PushViewController(page, false);
        }
        //[Export("ePay:")]
        //public void ePay(UIGestureRecognizer sender)
        //{
        //    payment = PaymentType.ePayment;
        //    CashImg.Image = UIImage.FromFile("Cash.png");
        //    GiftImg.Image = UIImage.FromFile("Giftvoucher.png");
        //    CreditImg.Image = UIImage.FromFile("Credit.png");
        //    ePayImg.Image = UIImage.FromBundle("ePaymentB");
        //    QRImg.Image = UIImage.FromFile("PaymentQr.png");
        //}
        [Export("QRcash:")]
        public async void  QR(UIGestureRecognizer sender)
        {
            if (!await GabanaAPI.CheckNetWork())
            {
                Utils.ShowMessage(Utils.TextBundle("nointernet", " Internet,Please Connect"));
                return;
            }

            //if (!await GabanaAPI.CheckSpeedConnection())
            //{
            //    Utils.ShowMessage(Utils.TextBundle("nointernet", " Internet,Please Connect"));
            //    return;
            //}

            double Amount = 0;
            Amount = amount;

            //ทดสอบยิง KQr
            respone_QrKBank respone_Qr = new respone_QrKBank();
            respone_Qr = await GabanaAPI.GetDataQRPayment(tranWithDetails.tran.TranNo, Amount);

            if (respone_Qr == null)
            {
                return;
            }

            //case too many request
            if (respone_Qr.statusCode == "-1")
            {
                Utils.ShowMessage(respone_Qr.errorDesc);

                return;
            }

            if (respone_Qr.statusCode == "10")
            {

                Utils.ShowMessage(respone_Qr.errorDesc);
                return;
            }

            //r/*espone_QrKBank respone_Qr = new respone_QrKBank() {qrCode = "1234*/56789012345678901234567890123456789012345678901234567890123456789012345678901234567890" };

            Utils.SetTitle(this.NavigationController, Utils.TextBundle("creditcard", "Items"));
            var page = new QrCashController(respone_Qr);
            this.NavigationController.PushViewController(page, false);
        }
        [Export("save:")]
        public async void save(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("saveorder", "Items"));
            SaveOrderController saveOrder = new SaveOrderController();
            this.NavigationController.PushViewController(saveOrder, false);
        }
        
    }
}