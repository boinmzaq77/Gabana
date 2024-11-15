using Foundation;
using Gabana.iOS;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.POS.Cart
{
    public partial class ChangeController : UIViewController
    {
        public static decimal Change, Cash;
        UIView TopAmountView, ChangeView,buttonView;
        ReceiptController RecieptPage = null;
        UILabel lblTxtAmount, lblAmount;
        UILabel lblTxtChange, lblChange, lblBaht;
        UIButton btnViewRecept;
        TranWithDetailsLocal tranWithDetails;
        public ChangeController()
        {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TopItem.Title = "Change";
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            #region TopAmountView
            TopAmountView = new UIView();
            TopAmountView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            TopAmountView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(TopAmountView);

            lblTxtAmount = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false 
            };
            lblTxtAmount.Font = lblTxtAmount.Font.WithSize(15);
            lblTxtAmount.Text = "Amount received";
            TopAmountView.AddSubview(lblTxtAmount);

            lblAmount = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false 
            };
            lblAmount.Font = lblAmount.Font.WithSize(20);
            TopAmountView.AddSubview(lblAmount);
            #endregion

            #region ChangeView
            ChangeView = new UIView();
            ChangeView.BackgroundColor = UIColor.White;
            ChangeView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ChangeView);


            lblTxtChange = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTxtChange.Font = lblTxtAmount.Font.WithSize(15);
            lblTxtChange.Text = "Change";
            ChangeView.AddSubview(lblTxtChange);

            lblChange = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblChange.Font = lblChange.Font.WithSize(60);
            lblChange.Text = "XX.XX";
            ChangeView.AddSubview(lblChange);

            lblBaht = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBaht.Font = lblBaht.Font.WithSize(15);
            lblBaht.Text = "Baht";
            ChangeView.AddSubview(lblBaht);

            #endregion

            #region buttonView
            buttonView = new UIView();
            buttonView.BackgroundColor = UIColor.White;
            buttonView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(buttonView);

            btnViewRecept = new UIButton();
            btnViewRecept.Layer.CornerRadius = 5;
            btnViewRecept.ClipsToBounds = true;
            btnViewRecept.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnViewRecept.Layer.BorderWidth = 0.5f;
            btnViewRecept.SetTitle("View Receipt", UIControlState.Normal);
            btnViewRecept.BackgroundColor = UIColor.White;
            btnViewRecept.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnViewRecept.TranslatesAutoresizingMaskIntoConstraints = false;
            btnViewRecept.TouchUpInside += async (sender, e) => {
                //print reciept
                if(RecieptPage == null)
                {
                    RecieptPage = new ReceiptController();
                }
                this.NavigationController.PushViewController(RecieptPage, false);
            };
            buttonView.AddSubview(btnViewRecept);
            #endregion

            lblAmount.Text = Cash.ToString("#,##0.00");
            lblChange.Text = Change.ToString("#,##0.00");
            setUpAutoLayout();
        }
        public void Setitem(double change, double cash)
        {
            Change = Convert.ToDecimal(change);
            Cash = Convert.ToDecimal(cash);
        }
        void setUpAutoLayout()
        {
            #region TopAmountView
            TopAmountView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor,0).Active=true;
            TopAmountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopAmountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            TopAmountView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height*97/1000).Active = true;

            lblTxtAmount.CenterYAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblTxtAmount.LeftAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblTxtAmount.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblAmount.CenterYAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAmount.RightAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            lblAmount.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region ChangeView
            ChangeView.TopAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ChangeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ChangeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ChangeView.BottomAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            // lblTxtChange, lblChange, lblBaht;
            lblTxtChange.TopAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.TopAnchor, (int)View.Frame.Height*44/1000).Active = true;
            lblTxtChange.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblChange.TopAnchor.ConstraintEqualTo(lblTxtChange.SafeAreaLayoutGuide.BottomAnchor, (int)View.Frame.Height * 41 / 1000).Active = true;
            lblChange.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblBaht.TopAnchor.ConstraintEqualTo(lblChange.SafeAreaLayoutGuide.BottomAnchor, (int)View.Frame.Height * 41 / 1000).Active = true;
            lblBaht.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            #region buttonView
            buttonView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            buttonView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            buttonView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            buttonView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height * 97 / 1000).Active = true;

            btnViewRecept.TopAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnViewRecept.LeftAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnViewRecept.RightAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnViewRecept.BottomAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            #endregion
        }
    }
}